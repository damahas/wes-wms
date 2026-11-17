<template>
  <div class="app-container">
    <div class="main-panel p16">
      <query-form :config="queryConfig" v-model:visible="showSearch" v-model="queryParams" @query="handleQuery" @reset="resetQuery" />

      <el-row :gutter="10" class="mb8">
        <el-col :span="1.5">
          <el-button type="primary" plain icon="Plus" @click="handleAdd" v-hasPermi="['wms:issueOrder:add']">新增</el-button>
        </el-col>
        <el-col :span="1.5">
          <el-button v-if="canEdit(selectedRows[0] || {})" type="success" plain icon="Edit" :disabled="single" @click="handleUpdate" v-hasPermi="['wms:issueOrder:edit']">修改</el-button>
          <el-button v-else type="info" plain icon="View" :disabled="single" @click="handleView(selectedRows[0])">明细</el-button>
        </el-col>
        <el-col :span="1.5">
          <el-button type="danger" plain icon="Delete" :disabled="multiple || selectedRows.some((r) => !canEdit(r))" @click="handleDelete" v-hasPermi="['wms:issueOrder:remove']">删除</el-button>
        </el-col>
        <right-toolbar v-model:showSearch="showSearch" @queryTable="getList"></right-toolbar>
      </el-row>

      <el-table v-loading="loading" :data="issueOrderList" @selection-change="handleSelectionChange">
        <el-table-column type="selection" width="55" align="center" />
        <el-table-column label="申请单号" align="center" prop="issueNo" :show-overflow-tooltip="true" />
        <el-table-column label="类型" align="center" width="90">
          <template #default="scope">
            <el-tag :type="scope.row.outboundType === 'sales' ? 'success' : scope.row.outboundType === 'other' ? 'info' : ''" size="small">
              {{ outboundTypeMap[scope.row.outboundType] || "生产领料" }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column label="工单号/客户" align="center" width="140" :show-overflow-tooltip="true">
          <template #default="scope">
            {{ scope.row.outboundType === "sales" ? (scope.row.customerName || "-") : (scope.row.workOrderNo || "-") }}
          </template>
        </el-table-column>
        <el-table-column label="领用部门" align="center" prop="deptName" width="110" :show-overflow-tooltip="true">
          <template #default="scope">{{ scope.row.deptName || "-" }}</template>
        </el-table-column>
        <el-table-column label="仓库" align="center" min-width="120" :show-overflow-tooltip="true">
          <template #default="scope">
            {{ scope.row.warehouse?.warehouseName || scope.row.warehouseName || "-" }}
          </template>
        </el-table-column>
        <el-table-column label="需求数" align="center" prop="totalQuantity" width="80" />
        <el-table-column label="已发" align="center" prop="issuedQuantity" width="80" />
        <el-table-column label="欠料" align="center" width="90">
          <template #default="scope">
            <span :class="{ 'text-danger': (scope.row.shortageQuantity ?? 0) > 0 }">
              {{ scope.row.shortageQuantity ?? 0 }}
            </span>
          </template>
        </el-table-column>
        <el-table-column label="需求日期" align="center" width="110">
          <template #default="scope">{{ (scope.row.requireDate || "").slice(0, 10) || "-" }}</template>
        </el-table-column>
        <el-table-column label="状态" align="center" width="100">
          <template #default="scope">
            <el-tag :type="issueStatusTag(scope.row.issueStatus)">
              {{ issueStatusMap[scope.row.issueStatus] || "-" }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column label="创建时间" align="center" prop="createTime" min-width="170" />
        <el-table-column label="操作" align="center" width="230" class-name="small-padding fixed-width">
          <template #default="scope">
            <el-button v-if="canIssue(scope.row)" link type="success" icon="Promotion" @click="handleIssue(scope.row)">发料</el-button>
            <el-button v-if="canEdit(scope.row)" link type="primary" icon="Edit" @click="handleUpdate(scope.row)" v-hasPermi="['wms:issueOrder:edit']">修改</el-button>
            <el-button v-else link type="info" icon="View" @click="handleView(scope.row)">明细</el-button>
            <el-button v-if="canEdit(scope.row)" link type="danger" icon="Delete" @click="handleDelete(scope.row)" v-hasPermi="['wms:issueOrder:remove']">删除</el-button>
          </template>
        </el-table-column>
      </el-table>

      <pagination v-show="total > 0" :total="total" v-model:page="queryParams.pageNum" v-model:limit="queryParams.pageSize" @pagination="getList" />

      <!-- 添加或修改对话框 -->
      <el-dialog :title="title" v-model="open" width="1040px" append-to-body>
        <el-form ref="issueOrderRef" :model="form" :rules="rules" label-width="92px" class="nowrap-labels" :disabled="viewMode">
          <el-row :gutter="16">
            <el-col :span="8">
              <el-form-item label="申请单号">
                <el-input v-model="form.issueNo" placeholder="保存后自动生成" readonly />
              </el-form-item>
            </el-col>
            <el-col :span="8">
              <el-form-item label="出库类型" prop="outboundType">
                <el-select v-model="form.outboundType" placeholder="请选择出库类型" style="width: 100%">
                  <el-option v-for="(label, value) in outboundTypeMap" :key="value" :label="label" :value="value" />
                </el-select>
              </el-form-item>
            </el-col>
            <el-col :span="8">
              <el-form-item label="来源类型">
                <el-select v-model="form.sourceType" placeholder="请选择来源类型" style="width: 100%">
                  <el-option v-for="(label, value) in sourceTypeMap" :key="value" :label="label" :value="value" />
                </el-select>
              </el-form-item>
            </el-col>
            <el-col :span="8">
              <el-form-item label="来源单号">
                <el-input v-model="form.sourceNo" placeholder="MES/ERP 下发时带入" maxlength="50" />
              </el-form-item>
            </el-col>
            <el-col v-if="form.outboundType !== 'sales'" :span="8">
              <el-form-item label="生产工单号" prop="workOrderNo">
                <el-input v-model="form.workOrderNo" placeholder="生产工单号（选填）" maxlength="50" />
              </el-form-item>
            </el-col>
            <el-col v-if="form.outboundType !== 'sales'" :span="8">
              <el-form-item label="领用部门">
                <el-tree-select
                  v-model="deptSelectedId"
                  :data="deptTree"
                  node-key="id"
                  :props="{ label: 'label', children: 'children' }"
                  check-strictly
                  filterable
                  clearable
                  :disabled="viewMode"
                  placeholder="请选择领用部门"
                  style="width: 100%"
                  @change="onDeptChange"
                />
              </el-form-item>
            </el-col>
            <el-col v-if="form.outboundType === 'sales'" :span="8">
              <el-form-item label="客户名称" prop="customerName">
                <el-input v-model="form.customerName" placeholder="请输入客户名称" maxlength="100" />
              </el-form-item>
            </el-col>
            <el-col :span="8">
              <el-form-item :label="form.outboundType === 'sales' ? '收货人' : '领用人'">
                <el-select
                  v-if="form.outboundType !== 'sales'"
                  v-model="form.receiverId"
                  :loading="userLoading"
                  filterable
                  clearable
                  :placeholder="deptSelectedId ? '请选择领用人' : '先选部门'"
                  :disabled="!deptSelectedId || viewMode"
                  style="width: 100%"
                  @change="onReceiverPick"
                >
                  <el-option v-for="u in userOptions" :key="u.value" :label="u.label" :value="u.value" />
                </el-select>
                <el-input v-else v-model="form.receiverName" placeholder="收货人" maxlength="50" />
              </el-form-item>
            </el-col>
            <el-col :span="8">
              <el-form-item label="仓库" prop="warehouseId">
                <WarehouseSelect v-model="form.warehouseId" placeholder="请选择仓库" :disabled="viewMode" />
              </el-form-item>
            </el-col>
            <el-col :span="8">
              <el-form-item label="需求日期">
                <el-date-picker v-model="form.requireDate" type="date" value-format="YYYY-MM-DD"
                  placeholder="请选择需求日期" style="width: 100%" />
              </el-form-item>
            </el-col>
            <el-col :span="24">
              <el-form-item label="备注">
                <el-input v-model="form.remark" type="textarea" :autosize="{ minRows: 2, maxRows: 4 }"
                  placeholder="请输入备注" maxlength="500" />
              </el-form-item>
            </el-col>
          </el-row>
        </el-form>

        <!-- 明细操作按钮 -->
        <el-row :gutter="10" class="mb8" type="flex" align="middle">
          <el-col :span="3">
            <el-button v-if="!form.issueId && !viewMode" class="detail-add-btn" type="primary" plain icon="Plus" @click="addDetail">添加明细</el-button>
          </el-col>
          <el-col :span="21" class="text-muted" style="line-height: 32px;">
            合计需求 {{ totalRequire }}　已发 {{ totalIssued }}　欠料 {{ totalShortage }}
          </el-col>
        </el-row>

        <el-table :data="form.details" border size="small" max-height="360">
          <el-table-column label="物料" align="center" min-width="260">
            <template #default="scope">
              <MaterialSelect
                v-model="scope.row.materialId"
                :label="scope.row.materialCode && scope.row.materialName ? scope.row.materialCode + ' - ' + scope.row.materialName : ''"
                placeholder="请选择物料" :disabled="viewMode" @change="(m) => onMaterialChange(scope.row, m)"
                style="width: 100%" />
            </template>
          </el-table-column>
          <el-table-column label="批次号" align="center" width="140">
            <template #default="scope">
              <el-input v-model="scope.row.batchNo" placeholder="空则按 FEFO 自动挑" size="small" />
            </template>
          </el-table-column>
          <el-table-column label="需求数量" align="center" width="110">
            <template #default="scope">
              <el-input-number v-model="scope.row.requireQuantity" :min="0" :controls="false"
                size="small" placeholder="数量" style="width: 100%" />
            </template>
          </el-table-column>
          <el-table-column label="已发" align="center" width="80">
            <template #default="scope">
              <span class="text-muted">{{ scope.row.issuedQuantity || 0 }}</span>
            </template>
          </el-table-column>
          <el-table-column label="欠料" align="center" width="80">
            <template #default="scope">
              <span :class="{ 'text-danger': Math.max(scope.row.requireQuantity - (scope.row.issuedQuantity || 0), 0) > 0 }">
                {{ Math.max(scope.row.requireQuantity - (scope.row.issuedQuantity || 0), 0) }}
              </span>
            </template>
          </el-table-column>
          <el-table-column label="备注" align="center" min-width="140">
            <template #default="scope">
              <el-input v-model="scope.row.remark" size="small" placeholder="备注" maxlength="255" />
            </template>
          </el-table-column>
          <el-table-column label="操作" align="center" width="60">
            <template #default="scope">
              <el-button link type="danger" icon="Delete" @click="removeDetail(scope.$index)"></el-button>
            </template>
          </el-table-column>
        </el-table>

        <template #footer>
          <div class="dialog-footer">
            <el-button v-if="!viewMode" type="primary" @click="submitForm">确 定</el-button>
            <el-button @click="cancel">{{ viewMode ? "关 闭" : "取 消" }}</el-button>
          </div>
        </template>
      </el-dialog>
    </div>
  </div>
</template>

<script setup name="WmsIssueOrder">
import { ref, reactive, toRefs, computed } from "vue";
import { ElMessage, ElMessageBox } from "element-plus";
import {
  listIssueOrder,
  getIssueOrder,
  addIssueOrder,
  updateIssueOrder,
  delIssueOrder,
  issueIssueOrder,
  cancelIssueOrder,
} from "@/api/wms/issueOrder";
import QueryForm from "@/components/QueryForm/index.vue";
import RightToolbar from "@/components/RightToolbar/index.vue";
import WarehouseSelect from "@/views/wms/warehouse/input.vue";
import MaterialSelect from "@/views/wms/material/input.vue";
import { listUser, deptTreeSelect } from "@/api/system/user";

const issueOrderRef = ref(null);

/** 部门树 / 领用人选项 */
const deptTree = ref([]);
const deptSelectedId = ref(undefined);
const userOptions = ref([]);
const userLoading = ref(false);
const userAllOptions = ref([]);

/** 组装领用人选项：本系统用户姓名字段是 account（无 nickName） */
function makeUserOption(u) {
  const name = u.account || u.userName || "";
  return {
    value: u.userId,
    label: name && name !== u.userName ? `${name}（${u.userName}）` : u.userName,
    name,
  };
}

/** 递归查部门树节点的显示名 */
function findDeptLabel(nodes, id) {
  for (const n of nodes || []) {
    if (n.id === id) return n.label;
    const hit = findDeptLabel(n.children, id);
    if (hit) return hit;
  }
  return undefined;
}

/** 拉取部门树（表单弹窗打开时调用一次） */
function loadDeptTree() {
  if (deptTree.value.length > 0) return Promise.resolve();
  return deptTreeSelect().then((res) => {
    deptTree.value = res.data || [];
  });
}

/** 按部门名在树里反查节点 id（历史单据只存了名字） */
function matchDeptByName(nodes, name) {
  for (const n of nodes || []) {
    if (n.label === name) return n.id;
    const hit = matchDeptByName(n.children, name);
    if (hit != null) return hit;
  }
  return null;
}

/** 领用部门选择：带出部门名，联动过滤该部门人员 */
function onDeptChange(val) {
  form.value.deptName = val ? findDeptLabel(deptTree.value, val) : undefined;
  if (!val) {
    userOptions.value = userAllOptions.value;
    return;
  }
  listUser({ pageNum: 1, pageSize: 999, status: "0", deptId: val })
    .then((res) => {
      userOptions.value = (res.rows || []).map(makeUserOption);
    })
    .catch(() => {
      userOptions.value = [];
    });
}

/** 拉取领用人选项（默认全量在编用户，页面存续期间缓存） */
function loadUsers() {
  if (userAllOptions.value.length > 0) return Promise.resolve();
  userLoading.value = true;
  return listUser({ pageNum: 1, pageSize: 999, status: "0" })
    .then((res) => {
      userAllOptions.value = (res.rows || []).map(makeUserOption);
      userOptions.value = userAllOptions.value;
    })
    .finally(() => {
      userLoading.value = false;
    });
}

/** 领用人选择：带出姓名 */
function onReceiverPick(val) {
  const hit = userOptions.value.find((u) => u.value === val);
  form.value.receiverName = hit ? hit.name : undefined;
}
const issueOrderList = ref([]);
const open = ref(false);
const loading = ref(false);
const ids = ref([]);
const single = ref(true);
const multiple = ref(true);
const total = ref(0);
const title = ref("");
/** 只读查看模式：已发料的单据只能看 */
const viewMode = ref(false);
/** 列表当前勾选的行 */
const selectedRows = ref([]);

const showSearch = ref(true);

// 来源类型
const sourceTypeMap = { mes: "MES下发", manual: "手工新建" };

// 出库类型
const outboundTypeMap = { issue: "生产领料", sales: "销售发货", other: "其他" };

// 发料状态：待发料 → 部分发料 → 已发完 / 已取消
const issueStatusMap = {
  pending: "待发料",
  partial: "部分发料",
  issued: "已发完",
  cancelled: "已取消",
};
const issueStatusTag = (s) =>
  s === "pending" ? "info" : s === "partial" ? "warning" : s === "issued" ? "success" : "danger";

/** 还没发过料且未取消的单据可以修改删除 */
function canEdit(row) {
  return (row.issuedQuantity ?? 0) <= 0 && row.issueStatus !== "cancelled";
}

/** 待发料 / 部分发料状态可以继续发料 */
function canIssue(row) {
  return row.issueStatus === "pending" || row.issueStatus === "partial";
}

const data = reactive({
  form: {},
  queryParams: {
    pageNum: 1,
    pageSize: 10,
    keywords: undefined,
    warehouseId: undefined,
    issueStatus: undefined,
    outboundType: undefined,
    workOrderNo: undefined,
  },
  rules: {
    warehouseId: [{ required: true, message: "请选择仓库", trigger: "change" }],
    outboundType: [{ required: true, message: "请选择出库类型", trigger: "change" }],
  },
});

const { form, queryParams, rules } = toRefs(data);

const queryConfig = computed(() => [
  { label: "关键字", prop: "keywords", type: "input", placeholder: "单号/工单号/部门/客户" },
  {
    label: "出库类型",
    prop: "outboundType",
    type: "select",
    placeholder: "请选择出库类型",
    options: [
      { label: "生产领料", value: "issue" },
      { label: "销售发货", value: "sales" },
      { label: "其他", value: "other" },
    ],
  },
  {
    label: "状态",
    prop: "issueStatus",
    type: "select",
    placeholder: "请选择状态",
    options: [
      { label: "待发料", value: "pending" },
      { label: "部分发料", value: "partial" },
      { label: "已发完", value: "issued" },
      { label: "已取消", value: "cancelled" },
    ],
  },
]);

/** 明细合计 */
const totalRequire = computed(() =>
  (form.value.details || []).reduce((sum, d) => sum + Number(d.requireQuantity || 0), 0)
);
const totalIssued = computed(() =>
  (form.value.details || []).reduce((sum, d) => sum + Number(d.issuedQuantity || 0), 0)
);
const totalShortage = computed(() => totalRequire.value - totalIssued.value);

/** 查询列表 */
function getList() {
  loading.value = true;
  listIssueOrder(queryParams.value).then((res) => {
    issueOrderList.value = res.rows || [];
    total.value = res.total || 0;
    loading.value = false;
  });
}

/** 标签页重新激活时由布局回调（是否刷新由页面决定） */
function tabRefresh() {
  getList();
}

/** 搜索 */
function handleQuery() {
  queryParams.value.pageNum = 1;
  getList();
}

/** 重置 */
function resetQuery() {
  queryParams.value = {
    pageNum: 1,
    pageSize: 10,
    keywords: undefined,
    warehouseId: undefined,
    issueStatus: undefined,
    outboundType: undefined,
    workOrderNo: undefined,
  };
  handleQuery();
}

/** 多选框 */
function handleSelectionChange(selection) {
  selectedRows.value = selection;
  ids.value = selection.map((item) => item.issueId);
  single.value = selection.length !== 1;
  multiple.value = !selection.length;
}

/** 表单重置 */
function reset() {
  form.value = {
    issueId: undefined,
    issueNo: undefined,
    outboundType: "issue",
    customerName: undefined,
    sourceType: "manual",
    sourceNo: undefined,
    workOrderNo: undefined,
    deptName: undefined,
    receiverId: undefined,
    receiverName: undefined,
    warehouseId: undefined,
    requireDate: undefined,
    remark: undefined,
    issueStatus: undefined,
    issuedQuantity: 0,
    shortageQuantity: 0,
    details: [],
  };
  viewMode.value = false;
  deptSelectedId.value = undefined;
  userOptions.value = userAllOptions.value;
  if (issueOrderRef.value) issueOrderRef.value.resetFields();
  // 预载部门树与用户选项（内部有缓存，不会重复请求）
  loadDeptTree();
  loadUsers();
}

/** 新增 */
function handleAdd() {
  reset();
  open.value = true;
  title.value = "添加出库申请";
}

/** 修改 */
function handleUpdate(row) {
  reset();
  const id = row?.issueId || ids.value[0];
  getIssueOrder(id).then((res) => fillForm(res.data || {}));
}

/** 查看明细：已发料的单据只读打开 */
function handleView(row) {
  if (!row?.issueId) return;
  reset();
  viewMode.value = true;
  getIssueOrder(row.issueId).then((res) => fillForm(res.data || {}));
  title.value = "出库申请明细";
}

/** 填充表单 */
function fillForm(d) {
  form.value = {
    issueId: d.issueId,
    issueNo: d.issueNo,
    outboundType: d.outboundType || "issue",
    customerName: d.customerName,
    sourceType: d.sourceType || "manual",
    sourceNo: d.sourceNo,
    workOrderNo: d.workOrderNo,
    deptName: d.deptName,
    receiverId: d.receiverId,
    receiverName: d.receiverName,
    warehouseId: d.warehouseId,
    requireDate: d.requireDate ? String(d.requireDate).slice(0, 10) : undefined,
    remark: d.remark,
    issueStatus: d.issueStatus,
    issuedQuantity: d.issuedQuantity || 0,
    shortageQuantity: d.shortageQuantity || 0,
    details: (d.details || []).map((x) => ({
      detailId: x.detailId,
      materialId: x.materialId,
      materialCode: x.materialCode,
      materialName: x.materialName,
      spec: x.spec,
      unit: x.unit,
      batchNo: x.batchNo,
      requireQuantity: x.requireQuantity,
      issuedQuantity: x.issuedQuantity || 0,
      shortageQuantity: x.shortageQuantity || 0,
      remark: x.remark,
    })),
  };
  // 回显部门/领用人控件：部门按名字匹配树节点，领用人按 id 匹配用户
  Promise.all([loadDeptTree(), loadUsers()]).then(() => {
    deptSelectedId.value = d.deptName ? matchDeptByName(deptTree.value, d.deptName) : undefined;
    if (d.receiverId && !userAllOptions.value.some((u) => u.value === d.receiverId)) {
      // 用户已不在系统（离职等）：补临时选项保住显示
      if (d.receiverName) {
        userAllOptions.value.push({ value: d.receiverId, label: `${d.receiverName}（原）`, name: d.receiverName });
        userOptions.value = userAllOptions.value;
      } else {
        form.value.receiverId = undefined;
      }
    }
  });
  open.value = true;
  title.value = "修改出库申请";
}

/** 明细：新增一行 */
function addDetail() {
  if (!form.value.details) form.value.details = [];
  form.value.details.push({
    detailId: undefined,
    materialId: undefined,
    materialCode: "",
    materialName: "",
    batchNo: undefined,
    requireQuantity: undefined,
    issuedQuantity: 0,
    allowQuantity: undefined,
    remark: undefined,
  });
}

/** 明细：删除一行 */
function removeDetail(index) {
  form.value.details.splice(index, 1);
}

/** 明细：物料变化回填编码名称 */
function onMaterialChange(row, material) {
  if (!material) return;
  row.materialCode = material.materialCode || "";
  row.materialName = material.materialName || "";
  row.spec = material.spec || "";
  row.unit = material.unit || "";
}

/** 提交 */
function submitForm() {
  issueOrderRef.value.validate((valid) => {
    if (!valid) return;

    const details = form.value.details || [];
    if (details.length === 0) {
      ElMessage.warning("请至少添加一条领料明细！");
      return;
    }
    for (const d of details) {
      if (!d.materialId) {
        ElMessage.warning("请选择领料物料！");
        return;
      }
      if (!(d.requireQuantity > 0)) {
        ElMessage.warning("需求数量必须大于0！");
        return;
      }
    }

    if (form.value.outboundType === "sales" && !form.value.customerName) {
      ElMessage.warning("销售发货请填写客户名称！");
      return;
    }

    const payload = {
      issueId: form.value.issueId,
      issueNo: form.value.issueNo,
      outboundType: form.value.outboundType,
      customerName: form.value.customerName,
      sourceNo: form.value.sourceNo,
      sourceType: form.value.sourceType,
      workOrderNo: form.value.outboundType === "sales" ? undefined : form.value.workOrderNo,
      deptName: form.value.outboundType === "sales" ? undefined : form.value.deptName,
      receiverId: form.value.receiverId,
      receiverName: form.value.receiverName,
      warehouseId: form.value.warehouseId,
      requireDate: form.value.requireDate,
      remark: form.value.remark,
      issueStatus: form.value.issueStatus,
      details: details.map((x) => ({
        detailId: x.detailId,
        materialId: x.materialId,
        batchNo: x.batchNo,
        requireQuantity: x.requireQuantity,
        issuedQuantity: x.issuedQuantity || 0,
        remark: x.remark,
      })),
    };

    const api = form.value.issueId ? updateIssueOrder : addIssueOrder;
    api(payload).then(() => {
      ElMessage.success("保存成功");
      open.value = false;
      getList();
    });
  });
}

/** 取消按钮 */
function cancel() {
  open.value = false;
  reset();
}

/** 发料 */
function handleIssue(row) {
  ElMessageBox.confirm(
    `对领料单「${row.issueNo}」执行发料：按先过期先出挑批次，库存有多少发多少，不足部分记为欠料。是否继续？`,
    "发料确认",
    { confirmButtonText: "确定发料", cancelButtonText: "取消", type: "warning" }
  )
    .then(() => issueIssueOrder(row.issueId))
    .then((res) => {
      ElMessage.success(res.msg || "发料成功");
      getList();
    });
}

/** 取消领料单 */
function handleCancel(row) {
  ElMessageBox.confirm(`确认取消领料单「${row.issueNo}」？`, "提示", {
    confirmButtonText: "确定",
    cancelButtonText: "取消",
    type: "warning",
  })
    .then(() => cancelIssueOrder(row.issueId))
    .then(() => {
      ElMessage.success("已取消");
      getList();
    });
}

/** 删除 */
function handleDelete(row) {
  const delIds = row.issueId || ids.value.join(",");
  ElMessageBox.confirm(`是否确认删除领料单编号为"${delIds}"的数据项？`, "警告", {
    confirmButtonText: "确定",
    cancelButtonText: "取消",
    type: "warning",
  })
    .then(() => delIssueOrder(delIds))
    .then(() => {
      getList();
      ElMessage.success("删除成功");
    });
}

getList();
</script>

<style scoped>
.text-muted {
  color: #909399;
  font-size: 12px;
}
.text-danger {
  color: var(--el-color-danger);
}
</style>
