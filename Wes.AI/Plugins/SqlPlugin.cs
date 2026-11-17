using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using SqlSugar;
using System.Text.RegularExpressions;

namespace Wes.AI.Plugins;

/// <summary>
/// SQL 插件（数据查询 Agent 专用）：生成并执行只读 SQL，支持查询表结构。
/// 仅允许 SELECT / WITH 查询，禁止写操作与 DDL，避免破坏数据。
/// 插件自身无状态，每次调用时在方法内创建 DI Scope 解析 Scoped 的 ISqlSugarClient，
/// 以便安全地注册到单例缓存的 Kernel 中。
/// </summary>
public class SqlPlugin
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<SqlPlugin> _logger;

    public SqlPlugin(IServiceProvider serviceProvider, ILogger<SqlPlugin> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    /// <summary>
    /// 执行只读 SQL 查询并返回结果（用于回答用户关于数据库数据的提问）
    /// </summary>
    [KernelFunction("execute_sql")]
    [Description("执行一条只读 SQL 查询（仅允许 SELECT 或 WITH 语句），返回查询结果（表格形式）。" +
                  "用于回答用户关于数据库数据的提问，例如统计用户数、查询列表等。禁止执行 INSERT/UPDATE/DELETE/DROP 等写操作。")]
    public async Task<string> ExecuteSqlAsync(
        [Description("要执行的 SELECT 或 WITH 查询语句")] string sql)
    {
        if (string.IsNullOrWhiteSpace(sql))
            return "错误：SQL 为空。";

        var (ok, error) = ValidateReadOnly(sql);
        if (!ok)
            return error!;

        _logger.LogInformation("SqlPlugin.ExecuteSql: {Sql}", sql);

        try
        {
            using var scope = _serviceProvider.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ISqlSugarClient>();
            var dt = await db.Ado.GetDataTableAsync(sql);
            return FormatDataTable(dt);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "SqlPlugin.ExecuteSql failed: {Sql}", sql);
            return $"执行 SQL 出错：{ex.Message}";
        }
    }

    /// <summary>
    /// 获取指定表的结构（列名与类型），帮助模型生成正确的 SQL
    /// </summary>
    [KernelFunction("get_table_schema")]
    [Description("查询指定数据库表的列结构（列名、类型、是否可空），帮助生成正确的 SQL。若不确定表名，可先查看系统提示中给出的可用表列表。")]
    public async Task<string> GetTableSchemaAsync(
        [Description("表名")] string tableName)
    {
        if (string.IsNullOrWhiteSpace(tableName))
            return "错误：表名为空。";

        try
        {
            using var scope = _serviceProvider.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ISqlSugarClient>();
            var cols = await Task.Run(() => db.DbMaintenance.GetColumnInfosByTableName(tableName, false));
            if (cols == null || cols.Count == 0)
                return $"未找到表 {tableName} 的结构信息（表可能不存在，请核对可用表列表）。";

            var sb = new StringBuilder();
            sb.AppendLine($"表 {tableName} 结构：");
            foreach (var c in cols)
                sb.AppendLine($"- {c.DbColumnName} {c.DataType}{(c.IsNullable ? " NULL" : " NOT NULL")}");
            return sb.ToString();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "SqlPlugin.GetTableSchema failed: {Table}", tableName);
            return $"查询表结构出错：{ex.Message}";
        }
    }

    /// <summary>
    /// 只读校验：剥离注释 → 禁止多语句 → 禁止写操作/DDL 关键字 → 首词必须是 SELECT / WITH。
    /// 返回 (是否通过, 拒绝原因)。
    /// </summary>
    private static (bool Ok, string? Error) ValidateReadOnly(string sql)
    {
        var s = StripComments(sql).Trim();
        if (s.Length == 0)
            return (false, "错误：SQL 为空。");

        // 禁止多语句堆叠：形如 "select 1; drop table t"，MySQL 驱动会依次执行后面的语句
        if (s.Contains(';'))
            return (false, "错误：不允许一次执行多条语句（SQL 中不能包含分号）。");

        // 写操作 / DDL / 文件导出 / 加锁：即便写在 SELECT 之后也一并拒绝
        if (DangerousKeyword.IsMatch(s))
            return (false, "错误：检测到写操作或 DDL 关键字，仅允许执行 SELECT / WITH 只读查询。");

        // SELECT ... INTO OUTFILE / DUMPFILE / 变量赋值（会写文件或改会话状态）
        if (IntoTarget.IsMatch(s))
            return (false, "错误：不允许 SELECT ... INTO（导出文件或变量赋值）。");

        // 加锁读同样视为非只读
        if (LockingRead.IsMatch(s))
            return (false, "错误：不允许加锁查询（FOR UPDATE / LOCK IN SHARE MODE）。");

        var first = s.TrimStart('(')
            .Split(new[] { ' ', '\t', '\n', '\r', '(' }, StringSplitOptions.RemoveEmptyEntries)
            .FirstOrDefault()?.ToLowerInvariant();

        return first is "select" or "with"
            ? (true, null)
            : (false, "错误：仅允许执行只读查询（SELECT / WITH），已拒绝写操作或 DDL。");
    }

    /// <summary>块注释 /* */</summary>
    private static readonly Regex BlockComment = new(@"/\*.*?\*/", RegexOptions.Singleline | RegexOptions.Compiled);

    /// <summary>行注释 -- 与 MySQL 的 #</summary>
    private static readonly Regex LineComment = new(@"(--[^\r\n]*|#[^\r\n]*)", RegexOptions.Compiled);

    /// <summary>
    /// 写操作 / DDL / 文件类危险关键字。
    /// 用 \b 边界匹配，create_time、is_delete 这类下划线连接的列名不会误判。
    /// </summary>
    private static readonly Regex DangerousKeyword = new(
        @"\b(insert|update|delete|replace|drop|alter|create|truncate|rename|grant|revoke|call|handler|load_file|outfile|dumpfile|lock|unlock|set|begin|commit|rollback|exec|execute)\b",
        RegexOptions.IgnoreCase | RegexOptions.Compiled);

    private static readonly Regex IntoTarget = new(@"\binto\b", RegexOptions.IgnoreCase | RegexOptions.Compiled);

    private static readonly Regex LockingRead = new(
        @"\b(for\s+update|lock\s+in\s+share\s+mode)\b",
        RegexOptions.IgnoreCase | RegexOptions.Compiled);

    /// <summary>去掉 SQL 注释，避免 "/*x*/ delete ..." 这类写法绕过首词判断</summary>
    private static string StripComments(string sql)
    {
        var s = BlockComment.Replace(sql, " ");
        return LineComment.Replace(s, " ");
    }

    private static string FormatDataTable(DataTable dt)
    {
        if (dt.Rows.Count == 0)
            return "查询成功，但没有返回任何行。";

        const int maxRows = 50;
        var sb = new StringBuilder();
        sb.AppendLine($"查询返回 {dt.Rows.Count} 行（最多展示 {maxRows} 行）：");
        var headers = dt.Columns.Cast<DataColumn>().Select(c => c.ColumnName).ToArray();
        sb.AppendLine(string.Join(" | ", headers));
        var rows = Math.Min(dt.Rows.Count, maxRows);
        for (var i = 0; i < rows; i++)
        {
            var vals = dt.Rows[i].ItemArray.Select(v => v?.ToString() ?? "NULL");
            sb.AppendLine(string.Join(" | ", vals));
        }
        return sb.ToString();
    }
}
