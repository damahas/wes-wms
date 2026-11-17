using System.Text;
using Wes.AI.Models.ViewModel;

namespace Wes.AI.Context;

/// <summary>
/// 上下文压缩模块：
/// 1. 估算 token（CJK 按 1 token/字、其余按 4 字符/token 的启发式，无需分词器）；
/// 2. 按「模型最大上下文 - 系统提示 - 输出预留」的预算裁剪历史，保留最近若干轮；
/// 3. 把被淘汰的早期对话折叠成一条 system 摘要，保持上下文连贯；
/// 4. 对超长单条消息（如 SQL 表格结果）做中段截断，保留头尾。
///
/// 说明：压缩只影响「送给模型的上下文」，不改动数据库中保存的完整会话历史，
/// 因此用户切换会话后仍能看到全部消息，只是模型不再重复接收过期内容。
/// </summary>
public static class ContextCompressor
{
    /// <summary>
    /// 模型未配置 max_context 时的兜底上下文窗口。
    /// 取保守值：窗口填大只会导致超窗报错（接口直接失败），填小只是历史多压缩一点，
    /// 因此未知模型按 32K 处理比按 128K 安全。
    /// </summary>
    public const int DefaultMaxContext = 32768;

    /// <summary>送入模型的总量（系统提示 + 历史 + 当前消息 + 输出预留）不超过窗口的该比例，留出安全余量</summary>
    private const double SafeRatio = 0.95;

    /// <summary>历史预算的下限：无论如何给历史保留这么多 token</summary>
    private const int MinHistoryBudget = 2000;

    /// <summary>输出预留的下限：压缩到极限时至少给模型留这么多生成空间</summary>
    private const int MinReservedOutput = 512;

    /// <summary>当前这条消息最多占用历史预算的比例，超出则截断，避免一条粘贴文本吃掉整个窗口</summary>
    private const double CurrentMessageRatio = 0.5;

    /// <summary>无论如何至少保留的历史条数（含当前这一轮的上下文）</summary>
    private const int MinKeepMessages = 4;

    /// <summary>摘要中最多列出几条早期消息</summary>
    private const int SummaryMaxItems = 8;

    /// <summary>摘要中每条消息保留的字符数</summary>
    private const int SummaryItemChars = 60;

    /// <summary>单条消息超过该 token 数时做中段截断</summary>
    private const int LongMessageTokens = 2000;

    /// <summary>
    /// 估算文本的 token 数：CJK 约 1 token/字，其余约 4 字符/token。
    /// </summary>
    public static int EstimateTokens(string? text)
    {
        if (string.IsNullOrEmpty(text)) return 0;

        int ascii = 0, cjk = 0;
        foreach (var ch in text)
        {
            // 0x2E80 之后基本都是 CJK/全角字符，按 1 token/字 估算
            if (ch >= 0x2E80) cjk++;
            else ascii++;
        }
        return cjk + (int)Math.Ceiling(ascii / 4.0);
    }

    /// <summary>
    /// 估算消息列表的 token 数（每条额外计入角色与分隔的开销）。
    /// </summary>
    public static int EstimateMessages(IEnumerable<HistoryMessage>? messages)
    {
        if (messages == null) return 0;
        var total = 0;
        foreach (var m in messages)
            total += EstimateTokens(m?.Content) + 4;
        return total;
    }

    /// <summary>
    /// 压缩历史：按预算保留最近的消息，早期消息折叠为摘要，超长消息截断。
    /// </summary>
    /// <param name="history">完整历史（按时间正序，不含当前这条用户消息）</param>
    /// <param name="systemTokens">系统提示与工具定义占用的 token</param>
    /// <param name="reservedOutputTokens">为模型输出预留的 token（一般为 max_tokens）</param>
    /// <param name="maxContext">模型最大上下文窗口，&lt;=0 时使用兜底值</param>
    /// <param name="currentMessage">本次用户消息（保证不被裁掉）</param>
    public static CompressResult Compress(
        IReadOnlyList<HistoryMessage>? history,
        int systemTokens,
        int reservedOutputTokens,
        int maxContext,
        string? currentMessage = null)
    {
        var limit = maxContext > 0 ? maxContext : DefaultMaxContext;

        // 1) 预算：系统提示先占位，输出预留按需让位，剩下的才给历史（系统提示 + 历史 + 当前消息 + 输出预留 ≤ 窗口 × SafeRatio）
        //    兜底不再硬给 2000：小窗口模型上那样会算出超过窗口的总量，导致接口报 context length exceeded。
        var available = (int)(limit * SafeRatio) - systemTokens;
        if (available < MinHistoryBudget) available = MinHistoryBudget;

        var reserved = reservedOutputTokens > 0 ? reservedOutputTokens : 0;
        var reservedCap = Math.Max(MinReservedOutput, available / 3);
        if (reserved > reservedCap) reserved = reservedCap;

        var budget = available - reserved;
        if (budget < MinHistoryBudget)
        {
            budget = MinHistoryBudget;
            reserved = MinReservedOutput;
        }

        // 2) 当前消息：保证不被裁掉，但过长时会挤掉历史，先做保护性截断（保留头尾）
        var current = currentMessage ?? "";
        var currentTokens = EstimateTokens(current) + 4;
        var currentBudget = Math.Max(MinHistoryBudget, (int)(budget * CurrentMessageRatio));
        if (currentTokens > currentBudget)
        {
            current = TruncateToTokens(current, currentBudget);
            currentTokens = EstimateTokens(current) + 4;
        }

        // 3) 先把超长消息处理掉：结构化内容（代码块 / JSON）整体省略，普通内容中段截断
        var normalized = new List<HistoryMessage>();
        var truncated = 0;
        if (history != null)
        {
            foreach (var m in history)
            {
                if (m == null) continue;
                if (EstimateTokens(m.Content) > LongMessageTokens)
                {
                    normalized.Add(IsStructured(m.Content) ? Placeholder(m) : TruncateMiddle(m));
                    truncated++;
                }
                else
                {
                    normalized.Add(m);
                }
            }
        }

        // 4) 从最新往回保留，直到超出预算（至少保留 MinKeepMessages 条）
        var kept = new List<HistoryMessage>();
        var used = currentTokens;
        for (var i = normalized.Count - 1; i >= 0; i--)
        {
            var msg = normalized[i];
            var cost = EstimateTokens(msg.Content) + 4;
            if (kept.Count >= MinKeepMessages && used + cost > budget)
            {
                // 预算不够时，结构化内容宁可整条省略也不截断：
                // 残缺的 ECharts JSON / SQL 进了历史，会诱导模型接着输出非法内容
                if (IsStructured(msg.Content))
                {
                    var ph = Placeholder(msg);
                    var phCost = EstimateTokens(ph.Content) + 4;
                    if (used + phCost > budget) break;
                    used += phCost;
                    kept.Add(ph);
                    continue;
                }
                break;
            }
            used += cost;
            kept.Add(msg);
        }
        kept.Reverse();

        var dropped = normalized.Count - kept.Count;

        // 5) 被淘汰的早期对话折叠为一条 system 摘要
        var result = new List<HistoryMessage>();
        if (dropped >= 2)
        {
            result.Add(new HistoryMessage
            {
                Role = "system",
                Content = BuildSummary(normalized.Take(dropped).ToList(), dropped)
            });
        }
        result.AddRange(kept);

        var usedTokens = systemTokens + EstimateMessages(result) + currentTokens;
        return new CompressResult
        {
            Messages = result,
            CurrentMessage = current,
            Usage = new ContextUsage
            {
                UsedTokens = usedTokens,
                MaxContext = limit,
                Budget = budget,
                Ratio = limit > 0 ? Math.Min(1d, usedTokens / (double)limit) : 0,
                Compressed = dropped > 0 || truncated > 0,
                DroppedMessages = dropped,
                TruncatedMessages = truncated
            }
        };
    }

    /// <summary>
    /// 超长消息中段截断：保留开头与结尾，中间用省略提示替代。
    /// </summary>
    private static HistoryMessage TruncateMiddle(HistoryMessage m)
    {
        var content = m.Content ?? "";
        if (EstimateTokens(content) <= LongMessageTokens) return m;

        return new HistoryMessage
        {
            Role = m.Role,
            Name = m.Name,
            ToolCallId = m.ToolCallId,
            Content = TruncateToTokens(content, LongMessageTokens)
        };
    }

    /// <summary>
    /// 结构化内容判定：代码块（含 echart / sql / json 围栏）或大段裸 JSON。
    /// 这类内容一旦被中段截断就会变成残缺语法，回传给模型反而会污染后续输出。
    /// </summary>
    private static bool IsStructured(string? content)
    {
        if (string.IsNullOrWhiteSpace(content)) return false;
        if (content.Contains("```")) return true;
        var s = content.TrimStart();
        return (s.StartsWith("{") || s.StartsWith("[")) && s.Length > 200;
    }

    /// <summary>
    /// 用一句占位说明替换整条内容，只保留「存在过什么」的信息，不保留残缺结构。
    /// </summary>
    private static HistoryMessage Placeholder(HistoryMessage m)
    {
        var tokens = EstimateTokens(m.Content);
        return new HistoryMessage
        {
            Role = m.Role,
            Name = m.Name,
            ToolCallId = m.ToolCallId,
            Content = $"（本条内容较长，约 {tokens} token，是图表配置或查询结果，已整体省略；需要时请重新查询）"
        };
    }

    /// <summary>
    /// 按 token 预算截断文本：保留开头约 70% 与结尾约 30%，中间用省略提示替代。
    /// </summary>
    private static string TruncateToTokens(string text, int maxTokens)
    {
        var total = EstimateTokens(text);
        if (total <= maxTokens) return text;
        if (maxTokens <= 0) return "";

        var headBudget = maxTokens * 0.7;
        var tailBudget = maxTokens - headBudget;

        double acc = 0;
        var headLen = 0;
        while (headLen < text.Length && acc + CharCost(text[headLen]) <= headBudget)
        {
            acc += CharCost(text[headLen]);
            headLen++;
        }

        acc = 0;
        var tailLen = 0;
        var maxTail = text.Length - headLen;
        while (tailLen < maxTail && acc + CharCost(text[text.Length - 1 - tailLen]) <= tailBudget)
        {
            acc += CharCost(text[text.Length - 1 - tailLen]);
            tailLen++;
        }

        var head = text[..headLen];
        var tail = tailLen > 0 ? text[^tailLen..] : "";
        return $"{head}\n…（本条过长已省略，原约 {total} token）…\n{tail}";
    }

    /// <summary>单个字符的 token 成本，与 EstimateTokens 的口径一致</summary>
    private static double CharCost(char ch) => ch >= 0x2E80 ? 1.0 : 0.25;

    /// <summary>
    /// 生成早期对话摘要：只取每条消息的开头，压缩成要点列表。
    /// </summary>
    private static string BuildSummary(List<HistoryMessage> dropped, int count)
    {
        var sb = new StringBuilder();
        sb.AppendLine("【早期对话摘要】以下要点来自已被压缩的较早对话，仅用于保持上下文连贯，不得当作工具返回的真实数据：");

        var picked = dropped
            .Where(m => m is { Role: "user" or "assistant" } && !string.IsNullOrWhiteSpace(m.Content))
            .TakeLast(SummaryMaxItems)
            .ToList();

        foreach (var m in picked)
        {
            var role = m.Role == "user" ? "用户" : "小W";
            var text = OneLine(m.Content);
            if (text.Length > SummaryItemChars)
                text = text[..SummaryItemChars] + "…";
            sb.AppendLine($"- {role}：{text}");
        }

        var omitted = count - picked.Count;
        if (omitted > 0)
            sb.AppendLine($"- （另有 {omitted} 条更早的消息已省略）");

        return sb.ToString();
    }

    /// <summary>
    /// 折叠空白，保证摘要中每条消息只占一行。
    /// </summary>
    private static string OneLine(string text)
    {
        var sb = new StringBuilder(text.Length);
        var lastSpace = false;
        foreach (var ch in text)
        {
            if (char.IsWhiteSpace(ch))
            {
                if (!lastSpace) sb.Append(' ');
                lastSpace = true;
            }
            else
            {
                sb.Append(ch);
                lastSpace = false;
            }
        }
        return sb.ToString().Trim();
    }
}

/// <summary>
/// 压缩结果：压缩后的消息列表 + 本次上下文用量。
/// </summary>
public class CompressResult
{
    public List<HistoryMessage> Messages { get; set; } = new();
    /// <summary>当前这条用户消息（可能被截断，调用方应使用本值而非原始入参）</summary>
    public string CurrentMessage { get; set; } = "";
    public ContextUsage Usage { get; set; } = new();
}
