# WMS 开发手册

> 上次更新：2026-11-17。本文档描述 Wes.Wms 模块的分层架构、编码规范与业务领域设计。框架级内容（系统管理、工作流、AI 等）见根目录 `README.md`。

## 一、项目概述

WMS（仓库管理系统）基于 .NET 10 + Vue 3 前后端分离架构，作为 WesAdmin 的业务模块运行。

| 层级 | 技术栈 |
|------|--------|
| 后端框架 | .NET 10 / ASP.NET Core |
| ORM | SqlSugar（SqlSugarScope，Scoped 注册） |
| 数据库 | MySQL 8.0（另提供 PostgreSQL 初始化脚本） |
| 依赖注入 | Autofac（命名约定自动注册） |
| 主键策略 | 雪花 ID（应用侧生成，数据库不设自增） |
| 前端框架 | Vue 3 + Vite |
| UI 组件库 | Element Plus |

业务模块划分：

| 模块 | 内容 |
|------|------|
| Basic | 仓库 / 库区 / 库位、物料、包装规格、往来单位、省市区、库位锁定 |
| Inbound | 收货单、入库单、上架、入库退货 |
| Outbound | 出库申请（领料/销售发货）、发料、出库单、出库退货、移库、调拨、缺料分析 |
| Inventory | 实时库存、库存流水、冻结、锁定、库存调整、库存预警 |
| Quality | IQC 检验项、来料检验单、来料不良台账 |

---

## 二、项目结构

```
WesAdmin.sln
├── Wes.WebApi/                  # Web 入口项目（Program.cs、Extensions/）
├── Wes.Wms/                     # WMS 业务模块
│   ├── Business/                # 业务逻辑层（Biz）
│   │   ├── Interface/{模块}/    #   I{Name}Biz 接口
│   │   └── {模块}/              #   {Name}Biz 实现
│   ├── Services/                # 数据访问层（Service）
│   │   ├── Interface/{模块}/    #   I{Name}Service 接口
│   │   └── {模块}/              #   {Name}Service 实现
│   ├── Controllers/{模块}/      # API 控制器（在 Wes.Wms 内，不在 WebApi）
│   └── Model/
│       ├── Entities/{模块}/     # ORM 实体（Basic/Inbound/Outbound/Inventory/Quality）
│       └── ViewModels/{模块}/   # Param / DTO / 返回体
├── Wes.System/                  # 系统模块（用户权限、编码规则、部门）
└── Wes.UI/src/
    ├── views/wms/{模块}/        # WMS 页面（每模块一个目录，index.vue）
    └── api/wms/{name}.js        # API 请求封装（每模块一个文件）
```

模块目录名统一小写：`Basic / Inbound / Outbound / Inventory / Quality / Dashboard`。

---

## 三、分层架构

### 3.1 分层概览

```
Controller  →  请求分发、参数绑定
    ↓
  Biz       →  业务逻辑、参数校验、事务管理、跨模块调用
    ↓
  Service   →  纯 CRUD 数据访问
    ↓
  Entity    →  实体定义（ORM 映射）
```

**核心原则：**
- Controller 只做请求转发，不写业务逻辑
- Biz 层负责业务逻辑、参数校验、事务管理、**跨模块调用**
- Service 层**只做纯 CRUD**，不写业务逻辑，不跨模块调用
- **跨模块调用走 Biz→Biz**，不走 Biz→Service

### 3.2 Service 层

**文件位置：** `Wes.Wms/Services/Interface/{模块}/I{Name}Service.cs`，命名空间 `Wes.Service`。

```csharp
namespace Wes.Service
{
    public interface IMaterialService
    {
        List<WmsMaterialEntity> GetList(ParamData<WmsMaterialParam> param, out int total);
        WmsMaterialEntity? GetById(long id);
        bool Save(WmsMaterialEntity model);
        bool Delete(List<long> ids);
    }
}
```

**方法签名规范：**
- `GetList(ParamData<TParam>, out int total)` — 分页查询，用 `param.PageNum / param.PageSize`，SqlSugar `ToPageList(pageNum, pageSize, ref total)`
- `GetById(long id)` — 单条查询
- `Save(T model)` — 新增/修改（主键 > 0 为修改）；新增后回写实体主键
- `Delete(List<long> ids)` — 逻辑删除
- Service 继承 `Repository<T>`，直接用 `Context` 属性访问数据库，**不接受 `ISqlSugarClient` 参数**

**实现要点：**

```csharp
public class MaterialService : Repository<WmsMaterialEntity>, IMaterialService
{
    public bool Save(WmsMaterialEntity model)
    {
        var account = GlobalContext.CurrentUser?.Account;
        var now = DateTime.Now;
        if (model.MaterialId > 0)
        {
            model.UpdateBy = account;
            model.UpdateTime = now;
            return Context.Updateable(model)
                .IgnoreColumns(p => new { p.CreateBy, p.CreateTime })
                .ExecuteCommand() > 0;
        }
        model.CreateBy = account;
        model.CreateTime = now;
        model.UpdateBy = account;
        model.UpdateTime = now;
        model.MaterialId = Context.Insertable(model).ExecuteReturnSnowflakeId();
        return model.MaterialId > 0;
    }

    public bool Delete(List<long> ids)
    {
        var account = GlobalContext.CurrentUser?.Account;
        var now = DateTime.Now;
        return Context.Updateable<WmsMaterialEntity>()
            .SetColumns(p => new WmsMaterialEntity { IsDel = 1, UpdateTime = now, UpdateBy = account })
            .Where(p => ids.Contains(p.MaterialId) && p.IsDel == 0)
            .ExecuteCommand() > 0;
    }
}
```

**明细表 Service** 单独建一对接口，典型签名：

```csharp
public interface IReceiptDetailService
{
    bool DeleteByReceiptId(long receiptId);
    bool BatchSave(long receiptId, List<WmsReceiptDetailEntity> details);
}
```

### 3.3 Biz 层

**文件位置：** `Wes.Wms/Business/Interface/{模块}/I{Name}Biz.cs`，命名空间 `Wes.Business`。

**返回类型规范：**
- `RowData<T>` — 分页列表：`{ Code, Msg, Total, Rows }`
- `ResultData<T>` — 单条数据：`{ Code, Msg, Data }`
- `ReturnData` — 操作结果：`{ Code, Msg }`
- Code=200 成功，Code=500 失败

**依赖注入：**
- Service 接口、跨模块的 Biz 接口、`ISqlSugarClient`（**仅用于事务管理**）
- 参数校验在 Biz 层完成
- **禁止用 `_client` 直接操作数据库**，必须通过 Service；禁止注入其他模块的 Service

**事务模式：**

```csharp
_client.Ado.BeginTran();
try
{
    var ok = _materialService.Save(model);
    if (!ok) { _client.Ado.RollbackTran(); return new ReturnData(500, "保存失败"); }

    _materialDetailService.DeleteByMaterialId(model.MaterialId);
    _materialDetailService.BatchSave(model.MaterialId, model.Details);

    _client.Ado.CommitTran();
    return new ReturnData();
}
catch (Exception ex)
{
    _client.Ado.RollbackTran();
    return new ReturnData(500, ex.Message);
}
```

### 3.4 Controller 层

**文件位置：** `Wes.Wms/Controllers/{模块}/{Name}Controller.cs`（WMS 控制器在模块内，不入 WebApi）。

路由风格：`[Route("wms/{name}")]`，按业务语义用 GET/POST，Swagger 按模块分组（`ApiExplorerSettings(GroupName = ...)`）。

```csharp
[ApiController]
[Route("wms/material")]
public class MaterialController : ControllerBase
{
    private readonly IMaterialBiz _materialBiz;

    public MaterialController(IMaterialBiz materialBiz) { _materialBiz = materialBiz; }

    [HttpPost("list")]
    public RowData<WmsMaterialEntity> GetList([FromBody] ParamData<WmsMaterialParam> param)
        => _materialBiz.GetList(param);
}
```

**Controller 只做请求转发，注入 Biz 接口，不注入 Service。**

### 3.5 Entity 层

**文件位置：** `Wes.Wms/Model/Entities/{模块}/Wms{Name}Entity.cs`，命名空间 `Wes.Model`。

```csharp
[SugarTable("wms_material", "物料表")]
public class WmsMaterialEntity
{
    [JsonConverter(typeof(LongToStringConverter))]
    [SugarColumn(ColumnName = "material_id", IsPrimaryKey = true, ColumnDescription = "物料id")]
    public long MaterialId { get; set; }

    [SugarColumn(ColumnName = "material_code", Length = 50, ColumnDescription = "物料编码")]
    public string MaterialCode { get; set; }
}
```

**实体规范：**
- 主键为各实体自有名称（`MaterialId`、`ReceiptId`…），`long` 雪花 ID，数据库**不设自增**
- **主键必须加 `[JsonConverter(typeof(LongToStringConverter))]`**——long 超出 JS 安全整数范围，序列化为字符串给前端
- 每个实体自带公共字段：`IsDel`（逻辑删除 0/1）、`CreateBy/CreateTime`、`UpdateBy/UpdateTime`
- `[SugarColumn]` 必须标注 `ColumnName`（snake_case）与 `ColumnDescription`（中文说明）；**修改时禁止整体覆盖特性**，避免丢失已有描述
- 导航属性用 `[Navigate]` + Service 查询时 `.Includes()`（见 4.2）

### 3.6 依赖注入

Autofac 命名约定自动注册 + SqlSugar Scoped 连接：

| 对象 | 注册方式 | 生命周期 | 说明 |
|------|---------|---------|------|
| `ISqlSugarClient` | 手动 `AddScoped` | Scoped | 每请求独立实例，保证事务线程安全 |
| Service / Biz | Autofac 自动扫描 | 按 `I{Name}`→`{Name}` 配对 | 新增类零配置 |

**为什么用 Scoped 而非 Singleton：** SqlSugarScope 内部共享 `Ado` 对象管理事务。Singleton 下多请求并发会争抢同一个 `Ado.Transaction` 导致事务错乱。Scoped 后 Biz 的 `_client` 与 Service 的 `Context` 是同一实例，`_client.Ado.BeginTran()` 对整个请求内所有 Service 操作生效。

---

## 四、WMS 业务领域

### 4.1 业务流程

```
【入库】收货单 → IQC 来料检验（合格/让步/退货）→ 入库单 → 上架（明细级进度）→ 库存增加 + 流水

【出库】出库申请（outbound_type: issue-生产领料 / sales-销售发货 / other-其他）
           │ 点「发料」一步完成（IssueOrderBiz.Issue）：
           │   1. 按缺量（需求-已发）调用分配器挑批次
           │   2. 生成出库单（source_type='issue' 回指申请）并立即执行扣库存
           │   3. 回写已发数量、重算欠料、状态流转
           ▼
        pending(待发料) → partial(部分发料，有多少发多少，可补发) → issued(已发完)

【库内】移库 / 调拨 / 库存调整：执行时同步更新实时库存并写流水
【隔离】冻结（整批禁止动用）/ 锁定（按单据预占可用量）
```

### 4.2 库存口径

```
可用量 Available = 库存数量 − 冻结数量 − 锁定数量
```

- 冻结 = 质量隔离/查封，整批禁止动用；锁定 = 单据级预留（出库锁定、配货预留）
- 口径统一在**库存分配器**实现，所有出库动作共用，禁止各处自行计算

### 4.3 库存分配器（IStockAllocaterBiz）

出库挑货的**唯一入口**：

| 方法 | 作用 |
|------|------|
| `Available(row)` | 单行可用量 |
| `Candidates(...)` | 候选库存行排序：**FEFO（先过期先出）→ FIFO（先进先出）**——失效日期升序 → 生产日期升序 → 入库时间升序；**空日期一律排最后**，否则没填保质期的物料会被永久优先出掉 |
| `Allocate(...)` | 分配（只算不扣），不够时返回行合计 < 需求量（有多少发多少） |
| `AvailableTotal(...)` | 物料在仓库的可用量合计 |

### 4.4 库存流水（wms_stock_record）—— 账本

- **流水即账本**：入库上架、出库执行、移库、调整全部写流水，记录变动前/变动数量/变动后、方向（in/out）、来源单据（source_type/source_id/source_no）
- 流水累加与实时库存可对账；每个批次出给哪张单据可全程追溯
- 写流水与更新库存必须在**同一事务**，流水写失败整个操作回滚
- 库存流水页面对外**只读**，无新增/修改入口

### 4.5 单据通用设计

- 状态机：保存/待处理 → 执行中（部分）→ 完成；取消态单独标记（如 `cancelled`）
- 执行类操作（发料、移库执行、调整执行、解冻/释放）一律**事务内完成库存变动 + 流水 + 回写**，执行后单据不可修改和删除（前端二次确认）
- 上游→下游单据用 `source_type / source_id / source_no` 三字段回指，不用外键
- 单号由编码规则生成：`_codeRuleBiz.GetCode("WMS_MATERIAL", out string err)`，规则配置在系统管理的「编号规则」

### 4.6 库存预警

按预警配置（物料 + 仓库维度的最小/最大阈值）定时扫描（Quartz 任务）：触发预警记录 → 站内信通知 → 库存回补后自动恢复（resolved）。

---

## 五、前端开发规范

| 规范 | 说明 |
|------|------|
| 表格列宽 | 复选框 `width="55"`，序号 `width="80"`，日期 `min-width="170"`，其他不设宽度 |
| 字体大小 | 非特殊情况最小 `14px` |
| 导航属性 | 前端用 `row.xxx?.yyy`（camelCase）访问关联对象名称 |
| 返回体 | 后端 JSON 全局 camelCase 序列化；主键是字符串（LongToStringConverter） |
| 表单布局 | `el-row + el-col`，常规字段 `span=8`（一行三个），长内容字段 `span=24` |
| 查询区 | 用公共组件 `QueryForm`（`queryConfig` 数组声明式配置） |
| 工具栏 | 用公共组件 `RightToolbar`（显隐查询/刷新，支持 `hint` 提示文案） |
| 权限字符 | 统一 `wms:{模块}:{动作}`（如 `wms:stockRecord:list`），v-perms 指令控制按钮 |
| 国际化 | `src/locales/{zh|en}/{模块}.js` 分文件维护，页面用 `useI18n()` 的 `t()` |
| 组织关联 | 领用部门用 `el-tree-select`（数据 `deptTreeSelect`，在 `api/system/user.js`）；人员下拉数据源用户姓名字段是 `account`（**无 nickName**） |

**页面三段式结构**（现有 WMS 页面统一模式）：

```
<QueryForm :query-config />  →  查询条件
<el-table> + <pagination>    →  列表
<el-dialog> + <el-form>      →  新增/修改/查看弹窗（viewMode 控制只读）
```

**执行类操作**：列表行按钮 + `ElMessageBox.confirm` 二次确认，文案说明不可回退（"执行后不可修改和删除"）。

---

## 六、常见模式速查

### 主从单据保存（Biz 层事务）

```csharp
_client.Ado.BeginTran();
try
{
    _orderService.Save(model);                                  // 主表
    _detailService.DeleteByOrderId(model.OrderId);              // 明细先删后插
    _detailService.BatchSave(model.OrderId, model.Details);
    _client.Ado.CommitTran();
    return new ReturnData();
}
catch (Exception ex) { _client.Ado.RollbackTran(); return new ReturnData(500, ex.Message); }
```

### 跨模块调用（Biz→Biz）

```csharp
public class ReceivingBiz : IReceivingBiz
{
    private readonly IReceivingService _receivingService;
    private readonly IReceiptBiz _receiptBiz;      // 注入其他模块的 Biz
    private readonly ISqlSugarClient _client;

    public ReturnData CreateReceipt(WmsReceivingEntity receiving)
    {
        _client.Ado.BeginTran();
        try
        {
            _receivingService.Save(receiving);
            _receiptBiz.CreateFromReceiving(receiving);   // Biz→Biz
            _client.Ado.CommitTran();
            return new ReturnData();
        }
        catch (Exception ex) { _client.Ado.RollbackTran(); return new ReturnData(500, ex.Message); }
    }
}
```

### 执行扣账 + 写流水（同事务）

```csharp
// 1. 分配（只算不扣）
var items = _allocater.Allocate(warehouseId, materialId, quantity, null, null);
// 2. 逐行扣库存 + 写流水（同一事务内，失败整体回滚）
foreach (var it in items)
{
    // update wms_stock: stock_quantity / available
    // insert wms_stock_record: before/change/after + source_no
}
// 3. 回写上游单据进度（已发数量/欠料/状态）
```

### 编码生成

```csharp
var code = _codeRuleBiz.GetCode("WMS_MATERIAL", out string err);
if (!string.IsNullOrEmpty(err)) return new ReturnData(500, err);
model.MaterialCode = code;
```

### 逻辑删除

```csharp
Context.Updateable<T>()
    .SetColumns(p => new T { IsDel = 1, UpdateTime = now, UpdateBy = account })
    .Where(p => ids.Contains(p.Id) && p.IsDel == 0)
    .ExecuteCommand();
```

---

## 七、注意事项

| # | 事项 | 说明 |
|---|------|------|
| 1 | **ColumnDescription 保护** | 修改 `[SugarColumn]` 特性时绝不能整体覆盖，否则丢失字段中文描述 |
| 2 | **主键字符串化** | 实体主键必须加 `LongToStringConverter`，前端拿到的主键是字符串 |
| 3 | **DLL 锁定问题** | 编译前确保后端进程已停止，否则 DLL 被占用导致编译失败 |
| 4 | **Windows Python** | 临时脚本用 `py -X utf8` 启动器而非 `python`（微软商店占位符会静默失败） |
| 5 | **Service 纯 CRUD** | Service 不写业务逻辑，不跨模块调用，不接受 `ISqlSugarClient` 参数 |
| 6 | **跨模块调用** | 统一走 Biz→Biz，禁止 Biz 注入其他模块的 Service |
| 7 | **事务线程安全** | `ISqlSugarClient` 为 Scoped 注册，`_client.Ado.BeginTran()` 仅影响当前请求 |
| 8 | **库存改动三件套** | 任何改库存的执行操作必须同事务完成：更新实时库存 + 写流水 + 回写单据进度 |
| 9 | **可用量口径** | 一律走分配器 `Available()`，禁止散落手算 |
| 10 | **数据库脚本** | 表结构与初始化数据在 `Document/Db/MySql/` 与 `Document/Db/PostgreSQL/`；应用启动 Code-First 自动建表，脚本用于新环境部署与文档对账 |
| 11 | **空日期排序** | 分配器候选行中失效/生产日期为空的排最后，改动排序逻辑时勿破坏此规则 |
