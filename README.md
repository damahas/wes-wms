# WesWMS

<p align="center">
  <strong>一套基于 Vue 3 + .NET 10 的 WMS 仓储管理系统</strong>
</p>

<p align="center">
  <img src="https://img.shields.io/badge/Vue-3.4-brightgreen" alt="Vue">
  <img src="https://img.shields.io/badge/.NET-10.0-blue" alt=".NET">
  <img src="https://img.shields.io/badge/MySQL-8.0-orange" alt="MySQL">
  <img src="https://img.shields.io/badge/license-MIT-green" alt="license">
</p>

---

## 简介

WesWMS 是一套覆盖完整仓储业务闭环的 WMS 仓储管理系统：**收货 / 质检 / 入库上架 / 出库发料 / 退货 / 库内作业（移库、调拨、调整、冻结、锁定）/ 库存账务与预警 / 仓库平面布局可视化**，开箱即用即可支撑生产级仓储场景。

系统同时内置用户权限、工作流、定时任务、AI 助手等平台能力（见文末「平台能力」），但本项目的核心是 WMS 业务本身。

---

## 功能特性

### 入库管理
- **收货单** — 采购到货登记，生成收货明细，是入库流程的起点
- **IQC 来料检验** — 检验项维护 + 来料检验单（合格 / 让步接收 / 退货），不良台账留痕
- **入库单** — 收货 / 质检合格转入库，**明细级上架进度**（一单分多次上架）
- **退货单** — 对已收货物资发起供应商退货

### 出库管理
- **出库申请** — 生产领料 / 销售发货 / 其他出库
- **发料** — 一键完成：**自动分配库存（先过期先出 FEFO，其次先进先出 FIFO）→ 生成出库单并立即执行 → 回写申请单**（累计已发、重算欠料、状态流转）；支持部分发料、多次补发
- **出库单** — 单据明细与执行进度查询
- **缺料分析** — 申请单欠料明细与齐套情况
- **出库退货** — 已出库物资的退回处理

### 库内管理
- **移库** — 库位间移动，执行即更新库存并写流水
- **调拨** — 跨仓库调拨
- **库存调整** — 报损 / 报溢 / 盘盈 / 盘亏，**库存改账的唯一入口**，执行即生效且全程留痕
- **冻结** — 整批隔离（如待检、不良），禁止动用
- **锁定** — 按单据预占可用量（如工单备料），防超发

### 库存管理
- **实时库存** — 批次级库存（物料 / 批次 / 库位 / 数量 / 冻结 / 锁定 / 可用 / 有效期）
- **库存流水** — 出入库 / 移库 / 调整全量账务（变动前后数量、方向、来源单据），**流水累加与实时库存可对账**，每个批次流向全程可追溯
- **库存预警 + 预警配置** — 按最小 / 最大库存阈值自动触发预警并通知相关用户

### 仓库平面布局可视化
- **网格画布设计器** — 拖拽绘制三类块：**储位**（绑定库位/库区）、**分区**（库区容器，可容纳子块）、**标注**（文字说明），支持 L/U/T 形拼接
- **块 ⇄ 业务实体绑定** — 储位 / 分区可直接绑定库位或库区实体，绑定后块名自动同步实体名
- **草稿 / 发布** — 版本递增管理，发布后按发布版渲染
- **预览模式** — 悬浮查看块信息，**点击储位 / 分区直接查看该库位 / 库区下的库存明细**（物料 / 批次 / 库存 / 冻结 / 锁定 / 可用 / 有效期）

### 基础数据
- **仓库 / 库区 / 库位** 三层结构（库区-库位级联选择器）
- **物料**、**包装规格**（层级树）、**往来单位**（联系人 / 地址）
- **库位锁定记录**、省市区数据

### 仪表盘
- 库存 / 单据 / 预警等汇总数据一览

---

## 核心业务流程

```
【入库】收货单 → IQC 来料检验（合格/让步/退货）→ 入库单 → 上架（明细级进度）→ 实时库存增加 + 库存流水
              └→ 收货后可发起退货单（供应商退货）

【出库】出库申请（领料/销售发货）
           │ 点「发料」一步完成：
           │   1. 自动分配库存 —— 先过期先出(FEFO)，其次先进先出(FIFO)
           │   2. 生成出库单并立即执行 —— 扣减实时库存、写库存流水
           │   3. 回写申请单 —— 累计已发、重算欠料、状态流转
           ▼
        待发料 → 部分发料（有多少发多少，可再次发料补发）→ 已发完
        已出库货物需退回 → 出库退货重新入库

【库内】移库 / 调拨 / 库存调整：单据执行时同步更新实时库存并写流水
【隔离】冻结（整批禁止动用）/ 锁定（指定单据预占可用量）
【预警】按最小/最大阈值自动比对实时库存，触发预警记录并通知
```

### 库存口径与账务

- **可用量 = 库存数量 − 冻结数量 − 锁定数量**（冻结 / 锁定单据实时占减）
- **流水即账本**：所有库存变动必写流水，可按物料 / 批次 / 库位 / 单据追溯
- **改账唯一入口**：库存只能通过库存调整单修改，杜绝直接改表
- 出库分配遵循 FEFO → FIFO，临期批次优先消耗

---

## 快速开始

> 🚀 **体验测试环境**: [http://8.153.171.43:8980/](http://8.153.171.43:8980/)　　账号：**admin**　　密码：**123456**

### 环境要求

- **后端**: [.NET 10.0 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- **前端**: [Node.js](https://nodejs.org/) 20.19+
- **数据库**: [MySQL](https://www.mysql.com/) 8.0+

### 后端启动

1. 配置数据库连接，编辑 `Wes.WebApi/appsettings.Development.json`：
```json
{
  "ConnectionStrings": {
    "WesConnectionString": "DataBase=wes_wms;Data Source=127.0.0.1;User Id=root;Password=your_password;CharSet=utf8;port=3306"
  }
}
```

2. 运行后端
```bash
cd Wes.WebApi
dotnet run

# 热更新模式
dotnet watch --project Wes.WebApi/Wes.WebApi.csproj --urls "http://localhost:5113"
```

> 应用启动后自动建表（Code-First），无需手动执行 SQL。数据库脚本见 `Document/Db/MySql/`（业务表结构 + 初始化数据）与 `Document/Db/PostgreSQL/`。

后端启动在 `http://localhost:5113`，Swagger 按模块分组（Dashboard / Inbound / Outbound / Inventory / Quality / Basic / System / Flow / AI）。

### 前端启动

```bash
cd Wes.UI
npm install
npm run dev
```

前端启动在 `http://localhost:3000`，API 自动代理到后端 `http://localhost:5113`。

### 生产构建

```bash
cd Wes.UI && npm run build
cd ../Wes.WebApi && dotnet publish -c Release
```

### Docker 部署

```bash
# 一键构建（前端编译 → 后端发布 → 打包镜像）
bash build-docker.sh

# 或手动构建
cd Wes.UI && npm run build
cd ../Wes.WebApi && dotnet publish -c Release
cd .. && docker build -f Wes.WebApi/Dockerfile -t wes-wms .

# 运行
docker run -d \
  --name wes-wms \
  -p 80:80 \
  -v /host/path/appsettings.json:/app/appsettings.json \
  -v /host/path/UploadFile:/app/UploadFile \
  -v /etc/machine-id:/etc/machine-id:ro \
  wes-wms
```

> Nginx 监听 80 端口，反向代理 `/api/` → `localhost:8088`。`appsettings.json`、`UploadFile`（含 DataProtection 密钥）通过 `-v` 挂载实现配置外挂和持久化；`/etc/machine-id` 用作许可证设备指纹。

---

## 项目结构

```
WesWMS.sln
├── Wes.UI/               # 前端 SPA (Vue3 + Vite + Element Plus)
│   └── src/views/wms/    #   WMS 业务页面（按模块分目录）
├── Wes.WebApi/           # Web API 入口 / 宿主项目
├── Wes.Wms/              # WMS 仓储管理模块（核心）
│   ├── Controllers/      #   Basic / Inbound / Outbound / Inventory / Quality / Dashboard
│   ├── Business/         #   业务逻辑（发料分配、账务流水、预警等）
│   ├── Services/         #   数据访问
│   └── Model/            #   实体 / ViewModel（按模块分包）
├── Wes.System/           # 平台基础层（用户权限 / 工作流 / 字典等）
├── Wes.Scheduler/        # 定时任务调度模块 (Quartz.NET)
├── Wes.AI/               # AI 智能助手 (Semantic Kernel)
└── Wes.Utils/            # 基础设施（缓存 / 三方集成 / 安全工具）
```

**依赖关系**: `WebApi` → `Scheduler` / `System` / `Wms` / `AI` → `Utils`

---

## 技术栈

| 层级 | 技术 |
|------|------|
| 前端 | Vue 3 + Vite + Element Plus + Vuex + Vue Router |
| 后端 | ASP.NET Core (.NET 10) + SqlSugar + Autofac + JWT |
| 数据库 | MySQL 8.0（兼容 PostgreSQL） |
| 调度 | Quartz.NET |
| AI | Semantic Kernel 1.77（OpenAI / DeepSeek / Qwen / GLM 等兼容模型） |

---

## 默认账户

账号：**admin**　　密码：**123456**

---

## 界面预览

> TODO: 补充界面截图

---

## 开发计划

- [x] WMS 仓储管理（入库/出库/库内/库存账务/来料质检）
- [x] FEFO/FIFO 库存自动分配发料
- [x] 库存流水对账体系（冻结/锁定/调整全留痕）
- [x] 仓库平面布局可视化设计器（储位/分区/标注 + 点击查库存）
- [x] 出库退货 / 采购退货闭环
- [ ] 盘点单（全仓/循环盘点）
- [ ] 波次拣选 / 拣货路径优化
- [ ] 布局设计器 ⇄ 库区/库位实体双向同步
- [ ] 文件安全增强，支持分布式文件系统

---

## 平台能力

除 WMS 业务外，系统内置以下平台级能力（基于若依架构思想扩展）：

- **系统管理** — 用户 / 角色 / 菜单 / 部门 / 岗位 / 字典 / 参数 / 编号规则 / 文件 / 日志 / 站内信 / 在线用户，RBAC 权限 + 数据权限
- **可视化工作流引擎** — 基于 @antv/x6 的流程设计器，版本管理、审批任务（按用户 / 角色 / 部门）
- **分布式定时任务** — Quartz.NET 集群调度，Cron 校验与执行日志
- **数据服务** — 可视化 API 编排（SQL + JavaScript 步骤组合，对外暴露 API）
- **AI 智能助手** — Semantic Kernel 多模型接入（模型配置存库，支持能力位筛选），内置 SQL 生成 / 工单操作 / 知识库检索等 Function Calling 插件，支持对话与 SSE 流式输出
- **三方集成** — 钉钉 / 企业微信 / 飞书 / 邮件（登录 + 消息通知）
- **许可证授权** — 设备指纹绑定，试用版 / 企业版分级授权
- **国际化 / 暗黑模式 / 滑块验证码**

---

## 贡献指南

欢迎提交 Issue 和 Pull Request。

---

## 鸣谢

本项目基于 [RuoYi](https://gitee.com/y_project/RuoYi-Vue) 开发，感谢若依团队的开源贡献。

---

## 许可证

本项目基于 [MIT License](LICENSE) 开源协议发布。
