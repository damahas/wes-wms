<template>
  <div class="app-container">
    <div class="main-panel p16">
      <query-form :config="queryConfig" v-model:visible="showSearch" v-model="queryParams" @query="handleQuery" @reset="resetQuery" />

      <el-row :gutter="10" class="mb8">
        <el-col :span="1.5">
          <el-button type="primary" plain icon="Plus" @click="handleAdd" v-hasPermi="['wms:issueOrder:add']">{{ t('common.add') }}</el-button>
        </el-col>
        <el-col :span="1.5">
          <el-button v-if="canEdit(selectedRows[0] || {})" type="success" plain icon="Edit" :disabled="single" @click="handleUpdate" v-hasPermi="['wms:issueOrder:edit']">{{ t('common.edit') }}</el-button>
          <el-button v-else type="info" plain icon="View" :disabled="single" @click="handleView(selectedRows[0])">{{ t('wms.receipt.details') }}</el-button>
        </el-col>
        <el-col :span="1.5">
          <el-button type="danger" plain icon="Delete" :disabled="multiple || selectedRows.some((r) => !canEdit(r))" @click="handleDelete" v-hasPermi="['wms:issueOrder:remove']">{{ t('common.delete') }}</el-button>
        </el-col>
        <right-toolbar v-model:showSearch="showSearch" @queryTable="getList"></right-toolbar>
      </el-row>

      <el-table v-loading="loading" :data="issueOrderList" @selection-change="handleSelectionChange">
        <el-table-column type="selection" width="55" align="center" />
        <el-table-column :label="t('wms.issueOrder.issueNo')" align="center" prop="issueNo" :show-overflow-tooltip="true" />
        <el-table-column :label="t('wms.issueOrder.typeLabel')" align="center" width="90">
          <template #default="scope">
            <el-tag :type="scope.row.outboundType === 'sales' ? 'success' : scope.row.outboundType === 'other' ? 'info' : ''" size="small">
              {{ outboundTypeLabel(scope.row.outboundType) }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column :label="t('wms.issueOrder.workOrderOrCustomer')" align="center" width="140" :show-overflow-tooltip="true">
          <template #default="scope">
            {{ scope.row.outboundType === "sales" ? (scope.row.customerName || "-") : (scope.row.workOrderNo || "-") }}
          </template>
        </el-table-column>
        <el-table-column :label="t('wms.issueOrder.dept')" align="center" prop="deptName" width="110" :show-overflow-tooltip="true">
          <template #default="scope">{{ scope.row.deptName || "-" }}</template>
        </el-table-column>
        <el-table-column :label="t('wms.common.warehouse')" align="center" min-width="120" :show-overflow-tooltip="true">
          <template #default="scope">
            {{ scope.row.warehouse?.warehouseName || scope.row.warehouseName || "-" }}
          </template>
        </el-table-column>
        <el-table-column :label="t('wms.issueOrder.totalRequire')" align="center" prop="totalQuantity" width="80" />
        <el-table-column :label="t('wms.issueOrder.issued')" align="center" prop="issuedQuantity" width="80" />
        <el-table-column :label="t('wms.issueOrder.shortage')" align="center" width="90">
          <template #default="scope">
            <span :class="{ 'text-danger': (scope.row.shortageQuantity ?? 0) > 0 }">
              {{ scope.row.shortageQuantity ?? 0 }}
            </span>
          </template>
        </el-table-column>
        <el-table-column :label="t('wms.issueOrder.requireDate')" align="center" width="110">
          <template #default="scope">{{ (scope.row.requireDate || "").slice(0, 10) || "-" }}</template>
        </el-table-column>
        <el-table-column :label="t('common.status')" align="center" width="100">
          <template #default="scope">
            <el-tag :type="issueStatusTag(scope.row.issueStatus)">
              {{ issueStatusLabel(scope.row.issueStatus) }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column :label="t('common.createTime')" align="center" prop="createTime" min-width="170" />
        <el-table-column :label="t('common.actions')" align="center" width="230" class-name="small-padding fixed-width">
          <template #default="scope">
            <el-button v-if="canIssue(scope.row)" link type="success" icon="Promotion" @click="handleIssue(scope.row)">{{ t('wms.issueOrder.issue') }}</el-button>
            <el-button v-if="canEdit(scope.row)" link type="primary" icon="Edit" @click="handleUpdate(scope.row)" v-hasPermi="['wms:issueOrder:edit']">{{ t('common.edit') }}</el-button>
            <el-button v-else link type="info" icon="View" @click="handleView(scope.row)">{{ t('wms.receipt.details') }}</el-button>
            <el-button v-if="canEdit(scope.row)" link type="danger" icon="Delete" @click="handleDelete(scope.row)" v-hasPermi="['wms:issueOrder:remove']">{{ t('common.delete') }}</el-button>
          </template>
        </el-table-column>
      </el-table>

      <pagination v-show="total > 0" :total="total" v-model:page="queryParams.pageNum" v-model:limit="queryParams.pageSize" @pagination="getList" />

      <!-- 添加或修改对话框 -->
      <el-dialog :title="title" v-model="open" width="1040px" append-to-body>
        <el-form ref="issueOrderRef" :model="form" :rules="rules" label-width="92px" class="nowrap-labels" :disabled="viewMode">
          <el-row :gutter="16">
            <el-col :span="8">
              <el-form-item :label="t('wms.issueOrder.issueNo')">
                <el-input v-model="form.issueNo" :placeholder="t('wms.receipt.receiptNoAuto')" readonly />
              </el-form-item>
            </el-col>
            <el-col :span="8">
              <el-form-item :label="t('wms.issueOrder.outboundType')" prop="outboundType">
                <el-select v-model="form.outboundType" :placeholder="t('wms.issueOrder.ph.selectType')" style="width: 100%">
                  <el-option :label="t('wms.issueOrder.typeIssue')" value="issue" />
                  <el-option :label="t('wms.issueOrder.typeSales')" value="sales" />
                  <el-option :label="t('wms.issueOrder.typeOther')" value="other" />
                </el-select>
              </el-form-item>
            </el-col>
            <el-col :span="8">
              <el-form-item :label="t('wms.issueOrder.sourceType')">
                <el-select v-model="form.sourceType" :placeholder="t('wms.issueOrder.ph.selectSourceType')" style="width: 100%">
                  <el-option :label="t('wms.issueOrder.sourceMes')" value="mes" />
                  <el-option :label="t('wms.issueOrder.sourceManual')" value="manual" />
                </el-select>
              </el-form-item>
            </el-col>
            <el-col :span="8">
              <el-form-item :label="t('wms.issueOrder.sourceNo')">
                <el-input v-model="form.sourceNo" :placeholder="t('wms.issueOrder.ph.sourceNo')" maxlength="50" />
              </el-form-item>
            </el-col>
            <el-col v-if="form.outboundType !== 'sales'" :span="8">
              <el-form-item :label="t('wms.issueOrder.workOrderNo')" prop="workOrderNo">
                <el-input v-model="form.workOrderNo" :placeholder="t('wms.issueOrder.ph.workOrderNo')" maxlength="50" />
              </el-form-item>
            </el-col>
            <el-col v-if="form.outboundType !== 'sales'" :span="8">
              <el-form-item :label="t('wms.issueOrder.dept')">
                <el-tree-select
                  v-model="deptSelectedId"
                  :data="deptTree"
                  node-key="id"
                  :props="{ label: 'label', children: 'children' }"
                  check-strictly
                  filterable
                  clearable
                  :disabled="viewMode"
                  :placeholder="t('wms.issueOrder.ph.selectDept')"
                  style="width: 100%"
                  @change="onDeptChange"
                />
              </el-form-item>
            </el-col>
            <el-col v-if="form.outboundType === 'sales'" :span="8">
              <el-form-item :label="t('wms.issueOrder.customerName')" prop="customerName">
                <el-input v-model="form.customerName" :placeholder="t('wms.issueOrder.ph.customerName')" maxlength="100" />
              </el-form-item>
            </el-col>
            <el-col :span="8">
              <el-form-item :label="form.outboundType === 'sales' ? t('wms.issueOrder.consignee') : t('wms.issueOrder.receiver')">
                <el-select
                  v-if="form.outboundType !== 'sales'"
                  v-model="form.receiverId"
                  :loading="userLoading"
                  filterable
                  clearable
                  :placeholder="deptSelectedId ? t('wms.issueOrder.ph.selectReceiver') : t('wms.issueOrder.ph.selectDeptFirst')"
                  :disabled="!deptSelectedId || viewMode"
                  style="width: 100%"
                  @change="onReceiverPick"
                >
                  <el-option v-for="u in userOptions" :key="u.value" :label="u.label" :value="u.value" />
                </el-select>
                <el-input v-else v-model="form.receiverName" :placeholder="t('wms.issueOrder.consignee')" maxlength="50" />
              </el-form-item>
            </el-col>
            <el-col :span="8">
              <el-form-item :label="t('wms.common.warehouse')" prop="warehouseId">
                <WarehouseSelect v-model="form.warehouseId" :placeholder="t('wms.warehouse.ph.selectWarehouse')" :disabled="viewMode" />
              </el-form-item>
            </el-col>
            <el-col :span="8">
              <el-form-item :label="t('wms.issueOrder.requireDate')">
                <el-date-picker v-model="form.requireDate" type="date" value-format="YYYY-MM-DD"
                  :placeholder="t('wms.issueOrder.ph.requireDate')" style="width: 100%" />
              </el-form-item>
            </el-col>
            <el-col :span="24">
              <el-form-item :label="t('common.remark')">
                <el-input v-model="form.remark" type="textarea" :autosize="{ minRows: 2, maxRows: 4 }"
                  :placeholder="t('common.input')" maxlength="500" />
              </el-form-item>
            </el-col>
          </el-row>
        </el-form>

        <!-- 明细操作按钮 -->
        <el-row :gutter="10" class="mb8" type="flex" align="middle">
          <el-col :span="3">
            <el-button v-if="!form.issueId && !viewMode" class="detail-add-btn" type="primary" plain icon="Plus" @click="addDetail">{{ t('wms.receiving.addDetail') }}</el-button>
          </el-col>
          <el-col :span="21" class="text-muted" style="line-height: 32px;">
            {{ t('wms.issueOrder.totalSummary', { require: totalRequire, issued: totalIssued, shortage: totalShortage }) }}
          </el-col>
        </el-row>

        <el-table :data="form.details" border size="small" max-height="360">
          <el-table-column :label="t('wms.material.title')" align="center" min-width="260">
            <template #default="scope">
              <MaterialSelect
                v-model="scope.row.materialId"
                :label="scope.row.materialCode && scope.row.materialName ? scope.row.materialCode + ' - ' + scope.row.materialName : ''"
                :placeholder="t('wms.material.ph.selectMaterial')" :disabled="viewMode" @change="(m) => onMaterialChange(scope.row, m)"
                style="width: 100%" />
            </template>
          </el-table-column>
          <el-table-column :label="t('wms.receiving.batchNo')" align="center" width="140">
            <template #default="scope">
              <el-input v-model="scope.row.batchNo" :placeholder="t('wms.issueOrder.ph.batchNo')" size="small" />
            </template>
          </el-table-column>
          <el-table-column :label="t('wms.issueOrder.requireQuantity')" align="center" width="110">
            <template #default="scope">
              <el-input-number v-model="scope.row.requireQuantity" :min="0" :controls="false"
                size="small" :placeholder="t('wms.receipt.quantity')" style="width: 100%" />
            </template>
          </el-table-column>
          <el-table-column :label="t('wms.issueOrder.issued')" align="center" width="80">
            <template #default="scope">
              <span class="text-muted">{{ scope.row.issuedQuantity || 0 }}</span>
            </template>
          </el-table-column>
          <el-table-column :label="t('wms.issueOrder.shortage')" align="center" width="80">
            <template #default="scope">
              <span :class="{ 'text-danger': Math.max(scope.row.requireQuantity - (scope.row.issuedQuantity || 0), 0) > 0 }">
                {{ Math.max(scope.row.requireQuantity - (scope.row.issuedQuantity || 0), 0) }}
              </span>
            </template>
          </el-table-column>
          <el-table-column :label="t('common.remark')" align="center" min-width="140">
            <template #default="scope">
              <el-input v-model="scope.row.remark" size="small" :placeholder="t('common.remark')" maxlength="255" />
            </template>
          </el-table-column>
          <el-table-column :label="t('common.actions')" align="center" width="60">
            <template #default="scope">
              <el-button link type="danger" icon="Delete" @click="removeDetail(scope.$index)"></el-button>
            </template>
          </el-table-column>
        </el-table>

        <template #footer>
          <div class="dialog-footer">
            <el-button v-if="!viewMode" type="primary" @click="submitForm">{{ t('common.submit') }}</el-button>
            <el-button @click="cancel">{{ viewMode ? t('common.close') : t('common.cancel') }}</el-button>
          </div>
        </template>
      </el-dialog>
    </div>
  </div>
</template>

<script setup name="WmsIssueOrder">
import { ref, reactive, toRefs, computed } from "vue";
import { ElMessage, ElMessageBox } from "element-plus";
import { useI18n } from "vue-i18n";
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

const { t } = useI18n();
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
function sourceTypeLabel(v) {
  const map = { mes: t('wms.issueOrder.sourceMes'), manual: t('wms.issueOrder.sourceManual') };
  return map[v] || "-";
}

// 出库类型
function outboundTypeLabel(v) {
  const map = {
    issue: t('wms.issueOrder.typeIssue'),
    sales: t('wms.issueOrder.typeSales'),
    other: t('wms.issueOrder.typeOther'),
  };
  return map[v] || t('wms.issueOrder.typeIssue');
}

// 发料状态：待发料 → 部分发料 → 已发完 / 已取消
function issueStatusLabel(s) {
  const map = {
    pending: t('wms.issueOrder.statusPending'),
    partial: t('wms.issueOrder.statusPartial'),
    issued: t('wms.issueOrder.statusIssued'),
    cancelled: t('wms.issueOrder.statusCancelled'),
  };
  return map[s] || "-";
}
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
    warehouseId: [{ required: true, message: () => t('wms.receipt.rules.warehouseRequired'), trigger: "change" }],
    outboundType: [{ required: true, message: () => t('wms.issueOrder.rules.typeRequired'), trigger: "change" }],
  },
});

const { form, queryParams, rules } = toRefs(data);

const queryConfig = computed(() => [
  { label: t('wms.common.keywords'), prop: "keywords", type: "input", placeholder: t('wms.issueOrder.ph.keywords') },
  {
    label: t('wms.issueOrder.outboundType'),
    prop: "outboundType",
    type: "select",
    placeholder: t('wms.issueOrder.ph.selectType'),
    options: [
      { label: t('wms.issueOrder.typeIssue'), value: "issue" },
      { label: t('wms.issueOrder.typeSales'), value: "sales" },
      { label: t('wms.issueOrder.typeOther'), value: "other" },
    ],
  },
  {
    label: t('common.status'),
    prop: "issueStatus",
    type: "select",
    placeholder: t('wms.warehouse.ph.selectStatus'),
    options: [
      { label: t('wms.issueOrder.statusPending'), value: "pending" },
      { label: t('wms.issueOrder.statusPartial'), value: "partial" },
      { label: t('wms.issueOrder.statusIssued'), value: "issued" },
      { label: t('wms.issueOrder.statusCancelled'), value: "cancelled" },
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
  title.value = t('wms.issueOrder.addTitle');
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
  title.value = t('wms.issueOrder.viewTitle');
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
        userAllOptions.value.push({ value: d.receiverId, label: `${d.receiverName}（${t('wms.issueOrder.ph.former')}）`, name: d.receiverName });
        userOptions.value = userAllOptions.value;
      } else {
        form.value.receiverId = undefined;
      }
    }
  });
  open.value = true;
  title.value = t('wms.issueOrder.editTitle');
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
      ElMessage.warning(t('wms.issueOrder.selectAtLeastOneDetail'));
      return;
    }
    for (const d of details) {
      if (!d.materialId) {
        ElMessage.warning(t('wms.issueOrder.selectMaterialRequired'));
        return;
      }
      if (!(d.requireQuantity > 0)) {
        ElMessage.warning(t('wms.issueOrder.requireQtyRequired'));
        return;
      }
    }

    if (form.value.outboundType === "sales" && !form.value.customerName) {
      ElMessage.warning(t('wms.issueOrder.customerRequired'));
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
      ElMessage.success(t('wms.common.savedSuccess'));
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
    t('wms.issueOrder.issueConfirmMsg', { no: row.issueNo }),
    t('wms.issueOrder.issueConfirmTitle'),
    { confirmButtonText: t('wms.issueOrder.issueConfirmBtn'), cancelButtonText: t('common.cancel'), type: "warning" }
  )
    .then(() => issueIssueOrder(row.issueId))
    .then((res) => {
      ElMessage.success(res.msg || t('wms.issueOrder.issueSuccess'));
      getList();
    });
}

/** 取消领料单 */
function handleCancel(row) {
  ElMessageBox.confirm(t('wms.issueOrder.cancelConfirm', { no: row.issueNo }), t('wms.common.tip'), {
    confirmButtonText: t('common.confirm'),
    cancelButtonText: t('common.cancel'),
    type: "warning",
  })
    .then(() => cancelIssueOrder(row.issueId))
    .then(() => {
      ElMessage.success(t('wms.returnOrder.cancelledTip'));
      getList();
    });
}

/** 删除 */
function handleDelete(row) {
  const delIds = row.issueId || ids.value.join(",");
  ElMessageBox.confirm(t('wms.issueOrder.deleteConfirm', { id: delIds }), t('wms.common.warning'), {
    confirmButtonText: t('common.confirm'),
    cancelButtonText: t('common.cancel'),
    type: "warning",
  })
    .then(() => delIssueOrder(delIds))
    .then(() => {
      getList();
      ElMessage.success(t('wms.common.deleteSuccess'));
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
