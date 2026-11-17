using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.OpenApi;

namespace Wes.WebApi.Extensions;

/// <summary>
/// Swagger 文档配置（按模块分组）
/// </summary>
public static class SwaggerSetup
{
    public static IServiceCollection AddSwaggerSetup(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("SystemManage", new OpenApiInfo { Title = "系统管理", Version = "v1" });
            c.SwaggerDoc("FlowManage",   new OpenApiInfo { Title = "流程管理", Version = "v1" });
            c.SwaggerDoc("Scheduler",    new OpenApiInfo { Title = "定时任务", Version = "v1" });
        });
        return services;
    }

    public static IApplicationBuilder UseSwaggerSetup(this IApplicationBuilder app)
    {
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/SystemManage/swagger.json", "系统管理");
            c.SwaggerEndpoint("/swagger/FlowManage/swagger.json",   "流程管理");
            c.SwaggerEndpoint("/swagger/Scheduler/swagger.json",    "定时任务");
        });
        return app;
    }
}

/// <summary>
/// 根据 Controller 的 namespace 自动分配 Swagger 分组
/// </summary>
public class SwaggerGroup : IControllerModelConvention
{
    /// <summary>
    /// SwaggerSetup 中已注册的分组，未注册的分组不会出现在 Swagger UI 中
    /// </summary>
    private static readonly string[] KnownGroups = { "SystemManage", "FlowManage", "Scheduler" };

    public void Apply(ControllerModel controller)
    {
        var ns = controller.ControllerType.Namespace ?? string.Empty;
        var route = controller.Attributes.OfType<RouteAttribute>()
            .Select(r => r.Template)
            .FirstOrDefault(t => !string.IsNullOrWhiteSpace(t)) ?? string.Empty;

        var group = ResolveGroup(ns, route.TrimStart('/'));
        if (!KnownGroups.Contains(group))
            group = "SystemManage";

        foreach (var action in controller.Actions)
        {
            action.ApiExplorer.GroupName = group;
        }
    }

    /// <summary>
    /// 按模块解析分组：命名空间优先，路由前缀兜底
    /// </summary>
    private static string ResolveGroup(string ns, string route)
    {
        // "Wes.Scheduler.Controllers"（路由 scheduler/job） → "Scheduler"
        if (ns.StartsWith("Wes.Scheduler", StringComparison.OrdinalIgnoreCase)
            || route.StartsWith("scheduler/", StringComparison.OrdinalIgnoreCase))
            return "Scheduler";

        // "Wes.WebApi.Areas.FlowManage" 或路由 flow/* → "FlowManage"
        if (ns.EndsWith("Areas.FlowManage", StringComparison.OrdinalIgnoreCase)
            || route.StartsWith("flow/", StringComparison.OrdinalIgnoreCase))
            return "FlowManage";

        // 其余（Wes.WebApi.Areas.SystemManage、Wes.WebApi.Controllers、Wes.AI.Controllers 等）→ "SystemManage"
        return "SystemManage";
    }
}
