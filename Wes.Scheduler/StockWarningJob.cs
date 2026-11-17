using Wes.Business;
using Wes.Scheduler.Business;

namespace Wes.Scheduler;

/// <summary>
/// 库存预警任务 — 按预警配置检查库存，触发预警并给配置的处理人发站内信
/// </summary>
/// <remarks>
/// invoke_target: StockWarning
/// 需要至少一条启用的库存预警配置（基础数据 → 预警配置）
/// </remarks>
public class StockWarningJob : ITaskExecutor
{
    private readonly IStockWarningBiz _stockWarningBiz;

    public StockWarningJob(IStockWarningBiz stockWarningBiz)
    {
        _stockWarningBiz = stockWarningBiz;
    }

    /// <summary>
    /// 任务标识，对应 sys_job.invoke_target
    /// </summary>
    public string Name => "StockWarning";

    /// <summary>
    /// 执行库存预警检查
    /// </summary>
    public async Task<string> ExecuteAsync(string? parameters)
    {
        // 预警引擎是同步 DB 操作，放到线程池里跑，避免阻塞 Quartz 调度线程
        var result = await Task.Run(() => _stockWarningBiz.Run());
        return $"[{result.Code}] {result.Msg}";
    }
}
