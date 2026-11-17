# WesAdmin

<p align="center">
  <strong>一个基于 Vue 3 + .NET 10 的企业级后台管理系统</strong>
</p>

<p align="center">
  <img src="https://img.shields.io/badge/Vite-8.1-646CFF" alt="Vite">
  <img src="https://img.shields.io/badge/Vue-3.4-brightgreen" alt="Vue">
  <img src="https://img.shields.io/badge/.NET-10.0-blue" alt=".NET">
  <img src="https://img.shields.io/badge/Element--Plus-2.13-blue" alt="Element Plus">
  <img src="https://img.shields.io/badge/MySQL-8.0-orange" alt="MySQL">
  <img src="https://img.shields.io/badge/license-MIT-green" alt="license">
</p>

---

## 简介

WesAdmin 是一套基于若依（RuoYi）的全新 UI 全栈企业级后台管理系统，提供用户管理、RBAC 权限控制、可视化工作流引擎、AI 智能开发、数据服务编排、分布式定时任务等核心功能，并内置完整的 **WMS 仓储管理系统**（入库 / 出库 / 库内 / 库存账务 / 来料质检），帮助开发者快速搭建和交付企业级后台应用。

---

## 技术栈

| 层级 | 技术 | 说明 |
|------|------|------|
| **前端框架** | Vue 3 + Vite 8 | Composition API, SCSS |
| **UI 组件库** | Element Plus 2.13 | 企业级 UI 组件 |
| **状态管理** | Vuex 4 | 全局状态管理 |
| **前端路由** | Vue Router 4 | 动态路由, 权限路由 |
| **流程图** | @antv/x6 | 工作流可视化设计 |
| **国际化** | vue-i18n | 简体中文 / English |
| **后端框架** | ASP.NET Core Web API | .NET 10.0 |
| **ORM** | SqlSugarCore 5.1 | Code-First, MySQL |
| **DI 容器** | Autofac 11.0 | 依赖注入，模块化扫描 |
| **认证授权** | JWT Bearer | Token 认证 |
| **定时任务** | Quartz.NET | 分布式定时任务调度 |
| **API 文档** | Swagger (Swashbuckle) | 自动 API 文档，按模块分组 |
| **缓存** | MemoryCache / Redis | 可选 Redis 支持 |

---

## 功能概览

### 系统管理
- **用户管理** — 用户 CRUD、状态管理、角色分配、岗位分配、密码重置、Excel 导出
- **角色管理** — 角色 CRUD、菜单权限分配、数据权限（部门隔离）、用户分配
- **菜单管理** — 菜单树 CRUD、前端动态路由自动生成
- **部门管理** — 部门树 CRUD、部门选择器
- **岗位管理** — 岗位 CRUD
- **字典管理** — 字典类型 + 字典数据维护，支持树结构
- **参数配置** — 系统参数 CRUD、缓存刷新、Excel 导出、钉钉/企微/飞书/邮件集成配置
- **编号规则** — 自定义编码规则生成器
- **数据服务** — 可视化 API 编排（SQL + JavaScript 步骤组合），对外暴露 API
- **许可证** — 设备指纹绑定、许可证激活/验证、试用版/企业试运行版/企业正式版
- **文件管理** — 文件上传下载
- **日志管理** — 登录日志 + 操作日志查询
- **站内信** — 消息中心（已读/未读）
- **在线用户** — 在线用户监控

### 工作流管理
- **流程定义** — 基于 @antv/x6 的可视化流程设计器，拖拽节点和连线
- **流程版本** — 版本管理、版本复制、版本启用/停用
- **流程实例** — 发起流程、审批任务处理、流程跟踪
- **节点类型** — 开始、结束、任务（审批）、通知
- **审批方式** — 按用户、按角色、按部门
- **防死循环** — 流程引擎内置深度保护机制

### 定时任务
- **任务管理** — CRUD、Cron 表达式校验、状态管理（启用/暂停）
- **任务日志** — 执行历史查询、成功/失败统计
- **分布式调度** — 基于 Quartz.NET，支持集群部署
- **Cron 校验** — 启动时自动过滤无效表达式，编辑/启用时实时校验

---

## WMS 仓储管理（Wes.Wms）

### 模块清单

| 模块 | 功能 |
|------|------|
| **入库管理** | 收货单（收货登记）→ 入库单（收货/质检合格转入库）→ 上架（明细级上架进度） |
| **出库管理** | 出库申请（生产领料 / 销售发货 / 其他）→ 发料（自动分配批次生成出库单并执行）→ 出库单（单据明细与执行进度） |
| **库内管理** | 移库（库位间移动）、调拨（跨仓）、库存调整（报损/报溢/盘盈/盘亏等，唯一改账入口）、冻结（整批隔离）、锁定（按单占用）、缺料分析 |
| **库存管理** | 实时库存（条码级）、库存流水（出入库/移库/调整全量账务，只读）、库存预警 + 预警配置 |
| **质量管理** | IQC 检验项、来料检验单（IQC）、来料不良台账 |
| **基础数据** | 仓库 / 库区 / 库位（三层结构）、物料、包装规格、往来单位（联系人/地址）、省市区、库位锁定记录 |

### 核心业务流程

```
【入库】收货单 → IQC 来料检验（合格/让步/退货）→ 入库单 → 上架 → 实时库存增加 + 库存流水

【出库】出库申请（领料/销售发货）
           │ 点「发料」一步完成：
           │   1. 自动分配库存 —— 先过期先出(FEFO)，其次先进先出(FIFO)
           │   2. 生成出库单并立即执行 —— 扣减实时库存、写库存流水
           │   3. 回写申请单 —— 累计已发、重算欠料、状态流转
           ▼
        待发料 → 部分发料（有多少发多少，可再次发料补发）→ 已发完

【库内】移库 / 调拨 / 库存调整：各自单据执行时同步更新实时库存并写流水
【隔离】冻结（整批禁止动用）/ 锁定（指定单据预占可用量）
```

### 库存口径与账务

- **可用量** = 库存数量 − 冻结数量 − 锁定数量（三张隔离单据实时占减）
- **流水即账本**：入库上架、出库执行、移库、调整全部写库存流水（变动前/变动后数量、方向、来源单据），流水累加与实时库存可对账，每个批次出给哪张单据可全程追溯
- **改账唯一入口**：库存只能通过库存调整单修改（报损/报溢/盘盈/盘亏等），调整单执行后立即生效且留痕
- **库存预警**：按预警配置（最小/最大库存阈值）自动触发预警记录，通知相关用户

### 数据库脚本

见 `Document/Db/MySql/`（业务表结构 + 系统初始化数据）与 `Document/Db/PostgreSQL/`（PostgreSQL 版本）。

---

## 项目架构

```
WesAdmin.sln
├── Wes.UI/                          # 前端 SPA (Vue3 + Vite + Element Plus)
│   ├── src/
│   │   ├── views/                   #   页面组件
│   │   ├── components/              #   公共组件
│   │   ├── utils/                   #   工具函数
│   │   └── router/                  #   路由配置
│   ├── nginx.conf                   #   Nginx 配置
│   └── vite.config.js
│
├── Wes.WebApi/                      # Web API 入口 / 宿主项目
│   ├── Program.cs                   #   启动入口（模块化配置）
│   ├── Controllers/                 #   公共控制器
│   ├── Extensions/                  #   启动扩展（Setup 模式）
│   │   ├── SqlsugarSetup.cs         #     数据库 / ORM
│   │   ├── AutofacSetup.cs          #     DI 容器
│   │   ├── JwtSetup.cs              #     JWT 认证
│   │   ├── SwaggerSetup.cs          #     Swagger 文档 + 分组约定
│   │   ├── QuartzSetup.cs           #     分布式定时任务
│   │   └── MiddlewareSetup.cs       #     中间件（异常/403/Token）
│   ├── Dockerfile
│   └── UploadFile/                  #   静态资源（验证码、IP库、模板）
│
├── Wes.System/                      # 核心业务层（单项目，内部按职责分文件夹）
│   ├── Business/                    #   业务逻辑层 (Biz)
│   │   ├── SystemManage/            #     系统管理业务
│   │   └── FlowManage/              #     流程管理业务 + 流程引擎
│   ├── Controllers/                 #   API 控制器
│   │   ├── SystemManage/            #     系统管理接口
│   │   └── FlowManage/              #     流程管理接口
│   ├── Model/
│   │   ├── DbModel/                 #   数据库模型 (ORM 实体)
│   │   └── ViewModel/               #   视图模型 (DTO / Param / Result)
│   └── Service/                     #   数据访问服务层 (Service / Repository)
│
├── Wes.Wms/                         # WMS 仓储管理模块
│   ├── Business/                    #   业务逻辑（入库/出库/库内/库存/质检/缺料分析）
│   ├── Services/                    #   数据访问服务层
│   ├── Controllers/                 #   API 控制器
│   └── Model/                       #   实体 / ViewModel
│       ├── Entities/                #     Basic/Inbound/Outbound/Inventory/Quality 分包
│       └── ViewModels/
│
├── Wes.Scheduler/                   # 定时任务调度模块
│   ├── Business/                    #   任务业务逻辑
│   ├── Controllers/                 #   任务管理接口
│   ├── Services/                    #   任务数据访问
│   ├── SchedulerHostedService.cs    #   托管服务（启动加载）
│   └── JobSchedulerService.cs       #   Quartz 调度封装
│
└── Wes.Utils/                       # 基础设施 / 工具层
    ├── Cache/                       #   缓存抽象 (Memory / Redis)
    ├── Integration/                 #   三方集成 (钉钉 / 企微 / 飞书 / 邮件)
    ├── Security/                    #   安全工具 (AES / JWT / MD5)
    └── Hepler/                      #   通用工具 (图片 / IP归属地 / 网络)
```

**依赖关系**: `WebApi` → `Scheduler` / `System` / `Wms` / `AI` → `Utils`

### AI 模块（Wes.AI）— 基于 Semantic Kernel

Wes.AI 采用 **Microsoft Semantic Kernel 1.77** 重写，支持多模型接入和 Function Calling。

```
Wes.AI/
├── Models/                     # 实体（ai_model 数据库模型配置）和 DTO
├── Kernel/
│   └── KernelProvider.cs       # SK Kernel 工厂，从数据库读取配置，ConcurrentDictionary 缓存
├── Plugins/                    # 工具插件（[KernelFunction] 属性注解注册）
│   ├── SqlPlugin.cs            #   SQL 生成 / 解释
│   ├── TicketPlugin.cs         #   工单操作
│   └── KnowledgePlugin.cs      #   知识库检索
├── Services/
│   └── AgentService.cs         #   Agent 对话（含 "chat" 类型，复用框架，无独立纯对话实现）
├── Controllers/
│   └── AiAgentController.cs    #   /api/ai/agent/{type}，含 providers / models
└── Extensions/
    └── AiServiceExtensions.cs  #   DI 注册（AddAiServices）
```

**支持的模型**：OpenAI / DeepSeek / Qwen / GLM 等兼容 OpenAI 协议的模型。AI 提供商与模型配置已迁移到数据库，存储在 `ai_model` 表中，由 `ModelConfigSeeder` 初始化种子数据（见 `Wes.AI/Extensions/ModelConfigSeeder.cs`），运行时由 `KernelProvider` 从数据库读取，不再依赖 `appsettings.json` 中的 `AI` 节点。可通过 AI 配置接口或数据库直接维护 `Provider` / `ApiKey` / `BaseUrl` / `ModelId` / `Capabilities` 等字段。

**模型能力（推荐关注点）**：模型不再用单一 `ModelType` 描述，而是用**能力位掩码** `Capabilities` 描述（见 `Wes.AI/Models/Enum/AiModelCapability.cs`）——
能力 = 输入模态（`InputText` / `InputImage` / `InputFile` / `InputAudio`）+ 输出模态（`OutputText` / `OutputImage` / `OutputAudio`）+ 附加能力（`ToolCall` / `Streaming` / `JsonMode` / `Reasoning`）。
新增能力只需在枚举里加一个位，不动表结构。`ModelType` 已降级为兼容字段（`llm` / `vision`，由能力派生；历史数据 `Capabilities=0` 时按它推导，启动时由 `ModelConfigSeeder.BackfillCapabilities()` 回填）。

**模型配置管理**：`AiModelController`（`/ai/model`）提供模型 CRUD 与能力字典（`/ai/model/capabilities`），前端页面为 `Wes.UI/src/views/system/aiModel/index.vue`；`KernelProvider.GetModelList(provider, modelType, require)` 支持按能力位筛选模型（例如只要能看图的模型传 `InputImage`）。

**三层调用流水**：

```
Controller          → Service              → SK Kernel
──────────────────────────────────────────────────────────
/api/ai/agent/chat  AgentService("chat")  IChatCompletionService
                       (复用 Agent 框架)      ↓ OpenAI 兼容 API

/api/ai/agent/{t}     AgentService           ChatCompletionAgent
                       (Function Calling)     ↓ kernel.Plugins
                                              ├─ sql/SqlPlugin
                                              ├─ ticket/TicketPlugin
                                              └─ knowledge/KnowledgePlugin
/api/ai/agent/{t}
  /stream            SSE 流式               IAsyncEnumerable
```

**待扩展**：

- **RAG 向量检索** — `KnowledgePlugin` 接入 `ISemanticTextMemory` / Qdrant / Azure AI Search，实现知识库语义搜索
- **多 Agent 协作** — `AgentGroupChat` 编排多个 Agent 协同完成复杂任务
- **Planner 任务规划** — `HandlebarsPlanner` / `FunctionCallingStepwisePlanner` 自动拆解多步骤任务
- **新 Plugin** — 新建类 + `[KernelFunction]` 属性注解，注册到 `EnsurePlugins()` 即可

---

## 快速开始

> 🚀 **体验测试环境**: [http://8.153.171.43:8980/](http://8.153.171.43:8980/)　　账号：**admin**　　密码：**123456**

### 环境要求

- **后端**: [.NET 10.0 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- **前端**: [Node.js](https://nodejs.org/) 20.19+（Vite 7 要求）
- **数据库**: [MySQL](https://www.mysql.com/) 8.0+

### 后端启动

1. 克隆仓库
```bash
git clone https://github.com/wes-admin/wes-admin.git
cd wes-admin
```

2. 配置数据库连接

编辑 `Wes.WebApi/appsettings.Development.json`（或 `appsettings.json`）：
```json
{
  "ConnectionStrings": {
    "WesConnectionString": "DataBase=wes_admin;Data Source=127.0.0.1;User Id=root;Password=your_password;CharSet=utf8;port=3306"
  }
}
```

3. 运行后端
```bash
cd Wes.WebApi
dotnet run

# 热更新模式：改动代码自动重新编译并重启
dotnet watch --project Wes.WebApi/Wes.WebApi.csproj --urls "http://localhost:5113"
```

> 应用启动后会自动建表（Code-First），无需手动执行 SQL。

后端启动在 `http://localhost:5113`，Swagger 文档在 `http://localhost:5113/swagger`。

### 前端启动

1. 安装依赖
```bash
cd Wes.UI
npm install
```

2. 启动开发服务器
```bash
npm run dev
```

前端启动在 `http://localhost:3000`，API 请求自动代理到后端 `http://localhost:5113`。

### 生产构建

```bash
# 前端构建
cd Wes.UI && npm run build

# 后端发布
cd Wes.WebApi && dotnet publish -c Release
```

### Docker 部署

项目提供 `build-docker.sh` 一键构建脚本和 Dockerfile，将前后端打包到同一镜像（Nginx 反向代理 + ASP.NET Core）。

**容器内目录结构**：

```
/app/
├── Wes.WebApi.dll              # 后端入口
├── appsettings.json            # 配置文件（可外挂）
├── UploadFile/                 # 上传文件目录（可外挂）
├── UploadFile/DataProtection/  # 密钥持久化（可外挂）
└── *.dll                       # 其他发布产物
/front/                         # 前端静态文件
/etc/nginx/nginx.conf           # Nginx 配置
```

**方式一：一键构建**

```bash
bash build-docker.sh
```

脚本自动完成：前端编译 → 后端发布 → 打包镜像，并输出每步耗时统计。

**方式二：手动构建**

```bash
# 1. 前端构建 + 后端发布
cd Wes.UI && npm run build
cd ../Wes.WebApi && dotnet publish -c Release

# 2. 构建镜像
cd ..
docker build -f Wes.WebApi/Dockerfile -t wes-admin .
```

**运行容器**

```bash
docker run -d \
  --name wes-admin \
  -p 80:80 \
  -v /host/path/appsettings.json:/app/appsettings.json \
  -v /host/path/UploadFile:/app/UploadFile \
  -v /etc/machine-id:/etc/machine-id:ro \
  wes-admin
```

> nginx 监听 80 端口，反向代理 `/api/` → `localhost:8088`，前端静态文件由 `/front` 提供，`/file/` 静态文件由 `/app/UploadFile` 提供。`appsettings.json`、`UploadFile`（含 DataProtection 密钥）通过 `-v` 挂载宿主机目录实现配置外挂和文件持久化。  
> `/etc/machine-id` 挂载宿主机机器 ID，用作许可证设备指纹，保证容器重建后 License 不失效。

---

## 默认账户

账号：**admin**　　密码：**123456**

---

## 界面预览

> TODO: 补充界面截图

---

## 开发计划

- [x] RBAC 权限管理
- [x] 可视化工作流引擎
- [x] AI 智能开发
- [x] 国际化（中/英）
- [x] 暗黑模式
- [x] 滑块验证码
- [x] 许可证授权机制
- [x] 数据服务 API 编排
- [x] 钉钉/企微/飞书三方集成（登录 + 组织同步）
- [x] 邮件集成
- [x] 分布式定时任务（Quartz.NET）
- [x] WMS 仓储管理（入库/出库/库内/库存账务/来料质检）
- [x] FEFO/FIFO 库存自动分配发料
- [x] 库存流水对账体系（冻结/锁定/调整全留痕）
- [ ] 文件安全增强，支持分布式文件系统
- [ ] 数据服务请求日志功能
- [ ] TODO: 许可证体系重新设计
- [ ] TODO: 后续计划补充

---

## AI 智能开发

推荐使用 AI 编程助手（如 [CodeBuddy](https://www.codebuddy.ai)）替代传统代码生成器，开发效率大幅提升：

- **智能编码** — AI 理解项目上下文，自动生成 Model / Service / Biz / Controller / Vue 组件
- **代码重构** — 一键重构、代码审查、性能优化建议
- **Bug 修复** — 智能定位问题，快速修复
- **知识问答** — 项目架构、技术选型随时答疑
- **自然语言驱动** — 用自然语言描述需求，AI 即可完成开发任务

---

## 贡献指南

欢迎提交 Issue 和 Pull Request。

---

## 鸣谢

本项目基于 [RuoYi](https://gitee.com/y_project/RuoYi-Vue) 开发，感谢若依团队的开源贡献。

---

## 许可证

本项目基于 [MIT License](LICENSE) 开源协议发布。
