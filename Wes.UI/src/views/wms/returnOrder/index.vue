<template>
  <div class="app-container">
    <div class="main-panel p16">
      <query-form
        :config="queryConfig"
        v-model:visible="showSearch"
        v-model="queryParams"
        @query="handleQuery"
        @reset="resetQuery"
      />

    <!-- 操作按钮 -->
    <el-row :gutter="10" class="mb8">
      <el-col :span="1.5">
        <el-button type="primary" plain icon="Plus" @click="handleAdd" v-hasPermi="['wms:returnOrder:add']">新增</el-button>
      </el-col>
      <el-col :span="1.5">
        <el-button type="success" plain icon="Edit" :disabled="single" @click="handleUpdate" v-hasPermi="['wms:returnOrder:edit']">修改</el-button>
      </el-col>
      <el-col :span="1.5">
        <el-button type="danger" plain icon="Delete" :disabled="multiple" @click="handleDelete" v-hasPermi="['wms:returnOrder:remove']">删除</el-button>
      </el-col>
      <el-col :span="1.5">
        <el-button type="success" plain icon="CircleCheck" :disabled="multiple" @click="handleConfirmReturn" v-hasPermi="['wms:returnOrder:edit']">确认退货</el-button>
      </el-col>
      <el-col :span="1.5">
        <el-button type="danger" plain icon="CircleClose" :disabled="multiple" @click="handleCancelReturn" v-hasPermi="['wms:returnOrder:edit']">取消退货</el-button>
      </el-col>
        <right-toolbar
          v-model:showSearch="showSearch"
          @queryTable="getList"
        ></right-toolbar>
    </el-row>

    <!-- 列表 -->
    <el-table v-loading="loading" :data="returnOrderList" @selection-change="handleSelectionChange">
      <el-table-column type="selection" width="55" align="center" />
      <el-table-column label="退货单号" align="center" prop="returnOrderNo" :show-overflow-tooltip="true" width="180" />
      <el-table-column label="退货类型" align="center" prop="returnType" width="100">
        <template #default="scope">
          <el-tag :type="returnTypeTagType(scope.row.returnType)" size="small">
            {{ returnTypeLabel(scope.row.returnType) }}
          </el-tag>
        </template>
      </el-table-column>
      <el-table-column label="供应商" align="center" :show-overflow-tooltip="true" min-width="180">
        <template #default="scope">
          {{ scope.row.supplier?.partnerName || "-" }}
        </template>
      </el-table-column>
      <el-table-column label="状态" align="center" prop="returnStatus" width="80">
        <template #default="scope">
          <el-tag :type="statusTagType(scope.row.returnStatus)" size="small">
            {{ statusLabel(scope.row.returnStatus) }}
          </el-tag>
        </template>
      </el-table-column>
      <el-table-column label="创建时间" align="center" prop="createTime" min-width="170" />
      <el-table-column label="操作" align="center" width="240" class-name="small-padding fixed-width">
        <template #default="scope">
          <template v-if="isFinalStatus(scope.row.returnStatus)">
            <el-button link type="primary" icon="View" @click="handleUpdate(scope.row)" v-hasPermi="['wms:returnOrder:edit']">查看</el-button>
          </template>
          <template v-else>
            <el-button link type="primary" icon="Edit" @click="handleUpdate(scope.row)" v-hasPermi="['wms:returnOrder:edit']">修改</el-button>
            <el-button link type="success" icon="CircleCheck" @click="handleConfirmReturn(scope.row)" v-hasPermi="['wms:returnOrder:edit']">确认退货</el-button>
            <el-button link type="danger" icon="CircleClose" @click="handleCancelReturn(scope.row)" v-hasPermi="['wms:returnOrder:edit']">取消退货</el-button>
            <el-button link type="danger" icon="Delete" @click="handleDelete(scope.row)" v-hasPermi="['wms:returnOrder:remove']">删除</el-button>
          </template>
        </template>
      </el-table-column>
    </el-table>

    <pagination
      v-show="total > 0"
      :total="total"
      v-model:page="queryParams.pageNum"
      v-model:limit="queryParams.pageSize"
      @pagination="getList"
    />

    <!-- 添加或修改对话框 -->
    <el-dialog :title="title" v-model="open" width="70%" append-to-body @close="cancel">
      <!-- 主表信息 -->
      <el-form ref="returnOrderRef" :model="form" :rules="rules" label-width="110px" class="mb16">
        <el-row :gutter="20">
          <el-col :span="8">
            <el-form-item label="退货单号" prop="returnOrderNo">
              <el-input v-model="form.returnOrderNo" placeholder="系统自动生成" maxlength="50" :readonly="true" />
            </el-form-item>
          </el-col>
          <el-col :span="8">
            <el-form-item label="退货类型" prop="returnType">
              <el-select v-model="form.returnType" placeholder="请选择退货类型" style="width:100%" :disabled="readOnly">
                <el-option label="质检不合格" value="iqc_unqualified" />
                <el-option label="其他" value="other" />
              </el-select>
            </el-form-item>
          </el-col>
          <el-col :span="8">
            <el-form-item label="供应商" prop="supplierId">
              <PartnerSelect v-model="form.supplierId" partner-type="supplier" placeholder="请选择供应商" @change="onSupplierChange" :disabled="readOnly" />
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="20">
          <el-col :span="8">
            <el-form-item label="状态" prop="returnStatus">
              <el-select v-model="form.returnStatus" placeholder="请选择状态" style="width:100%" :disabled="readOnly">
                <el-option label="待处理" value="pending" />
                <el-option label="已完成" value="completed" />
                <el-option label="已取消" value="cancelled" />
              </el-select>
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="20">
          <el-col :span="24">
            <el-form-item label="备注" prop="remark">
              <el-input v-model="form.remark" type="textarea" placeholder="请输入备注" maxlength="500" :disabled="readOnly" />
            </el-form-item>
          </el-col>
        </el-row>
      </el-form>

      <!-- 明细表 -->
      <div class="detail-section">
        <div class="detail-header mb8">
          <span class="detail-title">退货明细</span>
          <el-button class="detail-add-btn" type="primary" icon="Plus" @click="addDetailRow" v-hasPermi="['wms:returnOrder:add']" v-if="!readOnly">添加明细</el-button>
        </div>
        <el-table :data="detailList" border stripe max-height="350" style="width:100%">
          <el-table-column label="序号" type="index" width="55" align="center" />
          <el-table-column label="物料" min-width="160">
            <template #default="scope">
              <MaterialSelect
                v-model="scope.row.materialId"
                :label="scope.row.materialCode && scope.row.materialName ? scope.row.materialCode + ' - ' + scope.row.materialName : ''"
                placeholder="请选择物料"
                @change="onMaterialChange($event, scope.$index)"
                :disabled="readOnly"
              />
            </template>
          </el-table-column>
          <el-table-column label="规格" prop="spec" width="160">
            <template #default="scope">
              <el-input :model-value="scope.row.spec" placeholder="从物料带出" readonly />
            </template>
          </el-table-column>
          <el-table-column label="单位" prop="unit" width="160">
            <template #default="scope">
              <el-select v-model="scope.row.unit" placeholder="单位" filterable style="width:100%" :disabled="readOnly">
                <el-option
                  v-for="opt in unitOptions(scope.row)"
                  :key="opt.specId"
                  :label="opt.label"
                  :value="opt.value"
                />
              </el-select>
            </template>
          </el-table-column>
          <el-table-column label="批次号" prop="batchNo" width="160">
            <template #default="scope">
              <el-input v-model="scope.row.batchNo" placeholder="批次号" :disabled="readOnly" />
            </template>
          </el-table-column>
          <el-table-column label="退货数量" prop="planQuantity" width="160">
            <template #default="scope">
              <el-input-number controls-position="right" v-model="scope.row.planQuantity" :min="1" :controls="true"
                style="width:100%" :disabled="readOnly" />
            </template>
          </el-table-column>
          <el-table-column label="退货原因" prop="returnReason" min-width="150">
            <template #default="scope">
              <el-input v-model="scope.row.returnReason" placeholder="退货原因" :disabled="readOnly" />
            </template>
          </el-table-column>
          <el-table-column label="默认库位" width="220">
            <template #default="scope">
              <el-cascader
                :model-value="scope.row.locationCascader"
                :options="locationCascaderOptions"
                :props="{ expandTrigger: 'hover' }"
                placeholder="仓库/库区/库位"
                clearable
                :disabled="readOnly"
                style="width:100%"
                @change="(val) => onLocationCascaderChange(val, scope.$index)"
              />
            </template>
          </el-table-column>
          <el-table-column label="操作" width="60" align="center" v-if="!readOnly">
            <template #default="scope">
              <el-button link type="danger" icon="Delete"
                @click="removeDetailRow(scope.$index)" />
            </template>
          </el-table-column>
        </el-table>
        <div class="detail-footer mt8">
          <span>合计数量：<b>{{ totalPlanQuantity }}</b></span>
        </div>
      </div>

      <template #footer>
        <div class="dialog-footer">
          <template v-if="readOnly">
            <el-button @click="cancel">关 闭</el-button>
          </template>
          <template v-else>
            <el-button type="primary" @click="submitForm">保 存</el-button>
            <el-button @click="cancel">取 消</el-button>
          </template>
        </div>
      </template>
    </el-dialog>
    </div>
  </div>
</template>

<script setup name="WmsReturnOrder">
import { ref, reactive, toRefs, computed } from "vue";
import { ElMessage, ElMessageBox } from "element-plus";
import {
  listReturnOrder,
  getReturnOrder,
  getReturnOrderDetailList,
  addReturnOrder,
  updateReturnOrder,
  delReturnOrder,
} from "@/api/wms/returnOrder";
import { listPackSpecTree } from "@/api/wms/packSpec";
import { getMaterial } from "@/api/wms/material";
import { listWarehouse } from "@/api/wms/warehouse";
import { listZone } from "@/api/wms/zone";
import { listLocation } from "@/api/wms/location";
import QueryForm from "@/components/QueryForm/index.vue";
import RightToolbar from "@/components/RightToolbar/index.vue";
import MaterialSelect from "@/views/wms/material/input.vue";
import PartnerSelect from "@/views/wms/partner/input.vue";

const returnOrderRef = ref(null);
const returnOrderList = ref([]);
const open = ref(false);
const loading = ref(true);
const ids = ref([]);
const single = ref(true);
const multiple = ref(true);
const total = ref(0);
const title = ref("");

const showSearch = ref(true);
const readOnly = ref(false);

const detailList = ref([]);

// 全部包装规格（弹窗打开时拉取一次）
const allPackSpecs = ref([]);

// 库位级联选项
const locationCascaderOptions = ref([]);

const data = reactive({
  form: {},
  queryParams: {
    pageNum: 1,
    pageSize: 10,
    keywords: undefined,
    supplierId: undefined,
    returnType: undefined,
    returnStatus: undefined,
  },
  rules: {
    supplierName: [{ required: true, message: "请选择供应商", trigger: "change" }],
  },
});

const { form, queryParams, rules } = toRefs(data);

const queryConfig = computed(() => [
  {
    label: "关键字",
    prop: "keywords",
    type: "input",
    placeholder: "退货单号/供应商",
  },
  {
    label: "退货类型",
    prop: "returnType",
    type: "select",
    placeholder: "请选择类型",
    options: [
      { label: "质检不合格", value: "iqc_unqualified" },
      { label: "其他", value: "other" },
    ],
  },
  {
    label: "状态",
    prop: "returnStatus",
    type: "select",
    placeholder: "请选择状态",
    options: [
      { label: "待处理", value: "pending" },
      { label: "已完成", value: "completed" },
      { label: "已取消", value: "cancelled" },
    ],
  },
]);

// 状态映射
const statusMap = {
  pending: "待处理",
  completed: "已完成",
  cancelled: "已取消",
};
function statusLabel(s) { return statusMap[s] || s; }
function statusTagType(s) {
  const map = { pending: "warning", completed: "success", cancelled: "info" };
  return map[s] || "";
}

// 退货类型映射
const returnTypeMap = {
  iqc_unqualified: "质检不合格",
  other: "其他",
};
function returnTypeLabel(s) { return returnTypeMap[s] || s; }
function returnTypeTagType(s) {
  const map = { iqc_unqualified: "danger", other: "warning" };
  return map[s] || "";
}

// 明细合计
const totalPlanQuantity = computed(() =>
  detailList.value.reduce((s, r) => s + (r.planQuantity || 0), 0)
);

/** 查询列表 */
function getList() {
  loading.value = true;
  listReturnOrder(queryParams.value).then((res) => {
    returnOrderList.value = res.rows || [];
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
    keywords: undefined,
    supplierId: undefined,
    returnType: undefined,
    returnStatus: undefined,
    pageNum: 1,
    pageSize: 10,
  };
  handleQuery();
}

/** 多选 */
function handleSelectionChange(selection) {
  ids.value = selection.map((item) => item.returnOrderId);
  single.value = selection.length !== 1;
  multiple.value = !selection.length;
}

/** 是否终态（已完成/已取消 → 不可编辑） */
function isFinalStatus(s) { return s === "completed" || s === "cancelled"; }

/** 表单重置 */
function reset() {
  form.value = {
    returnOrderId: undefined,
    returnOrderNo: undefined,
    returnType: "iqc_unqualified",
    supplierId: undefined,
    supplierName: undefined,
    totalQuantity: 0,
    returnStatus: "pending",
    remark: undefined,
  };
  detailList.value = [];
  if (returnOrderRef.value) returnOrderRef.value.resetFields();
}

/** 新增 */
async function handleAdd() {
  reset();
  readOnly.value = false;
  await loadLocationTree();
  open.value = true;
  title.value = "添加退货单";
}

/** 修改/查看 */
async function handleUpdate(row) {
  reset();
  const id = row.returnOrderId || ids.value[0];
  await loadLocationTree();
  const res = await getReturnOrder(id);
  form.value = res.data || {};
  readOnly.value = isFinalStatus(form.value.returnStatus);
  // 加载明细，并补全物料对应的包装规格/最小单位
  const dRes = await getReturnOrderDetailList(id);
  const details = dRes.data || [];
  await Promise.all(
    details.map(async (d) => {
      d.materialPackSpecId = null;
      d.materialUnit = d.unit || "";
      d.locationCascader = findCascaderPath(d.locationId);
      if (d.materialId) {
        try {
          const m = await getMaterial(d.materialId);
          const mat = m.data || m;
          d.materialPackSpecId = mat.packSpecId || null;
          d.materialUnit = mat.unit || d.unit || "";
          await ensurePackSpecLoaded(mat.packSpecId);
          if (!d.unit) d.unit = defaultUnitFor(d);
        } catch (e) {
          /* 忽略单个物料加载失败 */
        }
      }
    })
  );
  detailList.value = details;
  open.value = true;
  title.value = readOnly.value ? "查看退货单" : "修改退货单";
}

/** 提交 */
function submitForm() {
  returnOrderRef.value.validate((valid) => {
    if (!valid) return;
    if (detailList.value.length === 0) {
      ElMessage.warning("请至少添加一条退货明细");
      return;
    }
    const dto = {
      returnOrder: { ...form.value },
      details: detailList.value.map((d) => ({
        ...d,
        materialCode: d.materialCode || "",
        materialName: d.materialName || "",
      })),
    };
    const api = form.value.returnOrderId ? updateReturnOrder : addReturnOrder;
    api(dto).then(() => {
      ElMessage.success("保存成功");
      open.value = false;
      getList();
    });
  });
}

/** 确认退货（左侧按钮 → 确认框 → 调接口） */
function handleConfirmReturn(row) {
  const id = row?.returnOrderId || ids.value[0];
  if (!id) { ElMessage.warning("请先选择一条退货单"); return; }
  ElMessageBox.confirm("确认该退货单已完成退货？", "提示", {
    confirmButtonText: "确定",
    cancelButtonText: "取消",
    type: "success",
  }).then(async () => {
    await updateReturnOrder({ returnOrder: { returnOrderId: id, returnStatus: "completed" }, details: [] });
    ElMessage.success("退货确认成功");
    getList();
  });
}

/** 取消退货（左侧按钮 → 确认框 → 调接口） */
function handleCancelReturn(row) {
  const id = row?.returnOrderId || ids.value[0];
  if (!id) { ElMessage.warning("请先选择一条退货单"); return; }
  ElMessageBox.confirm("确认取消该退货单？", "警告", {
    confirmButtonText: "确定",
    cancelButtonText: "取消",
    type: "warning",
  }).then(async () => {
    await updateReturnOrder({ returnOrder: { returnOrderId: id, returnStatus: "cancelled" }, details: [] });
    ElMessage.success("已取消");
    getList();
  });
}

/** 取消 */
function cancel() {
  open.value = false;
  reset();
}

/** 删除 */
function handleDelete(row) {
  const delIds = row.returnOrderId || ids.value.join(",");
  ElMessageBox.confirm("是否确认删除该退货单及其明细？", "警告", {
    confirmButtonText: "确定",
    cancelButtonText: "取消",
    type: "warning",
  })
    .then(() => delReturnOrder(delIds))
    .then(() => {
      getList();
      ElMessage.success("删除成功");
    });
}

// ============ 供应商选择回调 ============
function onSupplierChange(data) {
  form.value.supplierName = data.partnerName || "";
}

// ============ 物料选择回调 ============
async function onMaterialChange(data, index) {
  const row = detailList.value[index];
  row.materialCode = data.materialCode;
  row.materialName = data.materialName;
  row.spec = data.spec || "";
  row.materialPackSpecId = data.packSpecId || null;
  row.materialUnit = data.unit || "";
  await ensurePackSpecLoaded(data.packSpecId);
  row.unit = defaultUnitFor(row);
}

// ============ 库位级联 ============
async function loadLocationTree() {
  if (locationCascaderOptions.value.length) return;
  try {
    const [wRes, zRes, lRes] = await Promise.all([
      listWarehouse({ pageSize: 0 }),
      listZone({ pageSize: 0 }),
      listLocation({ pageSize: 0 }),
    ]);
    const warehouses = wRes.rows || wRes.data || [];
    const zones = zRes.rows || zRes.data || [];
    const locations = lRes.rows || lRes.data || [];
    locationCascaderOptions.value = warehouses
      .filter(w => w.isDel !== 1)
      .map(w => ({
        value: w.warehouseId,
        label: w.warehouseName,
        children: zones
          .filter(z => z.warehouseId === w.warehouseId && z.isDel !== 1)
          .map(z => ({
            value: z.zoneId,
            label: z.zoneName,
            children: locations
              .filter(l => l.zoneId === z.zoneId && l.isDel !== 1)
              .map(l => ({
                value: l.locationId,
                label: l.locationCode,
              }))
          }))
          .filter(z => z.children.length > 0)
      }))
      .filter(w => w.children.length > 0);
  } catch { /* ignore */ }
}

function findCascaderPath(locationId) {
  if (!locationId || !locationCascaderOptions.value.length) return [];
  for (const w of locationCascaderOptions.value) {
    for (const z of (w.children || [])) {
      for (const l of (z.children || [])) {
        if (l.value === locationId) return [w.value, z.value, l.value];
      }
    }
  }
  return [];
}

function onLocationCascaderChange(val, index) {
  const row = detailList.value[index];
  row.locationCascader = val || [];
  row.locationId = val && val.length ? val[val.length - 1] : undefined;
}

// ============ 明细操作 ============
function addDetailRow() {
  detailList.value.push({
    detailId: 0,
    returnOrderId: 0,
    materialId: undefined,
    materialCode: "",
    materialName: "",
    materialPackSpecId: null,
    materialUnit: "",
    spec: "",
    unit: "",
    batchNo: "",
    planQuantity: undefined,
    returnReason: "",
    locationId: undefined,
    locationCascader: [],
  });
}

function removeDetailRow(index) {
  detailList.value.splice(index, 1);
}

// ============ 包装规格/单位 辅助 ============
async function ensurePackSpecLoaded(rootId) {
  if (!rootId) return;
  if (allPackSpecs.value.some((p) => p.rootId === rootId)) return;
  try {
    const r = await listPackSpecTree(rootId);
    const nodes = r.rows || r.data || [];
    allPackSpecs.value.push(...nodes);
  } catch { /* ignore */ }
}

function sortTopDown(nodes) {
  const map = new Map(nodes.map((n) => [n.specId, n]));
  const roots = nodes.filter((n) => !n.parentId);
  const result = [];
  function walk(node) {
    result.push(node);
    nodes.filter((n) => n.parentId === node.specId).forEach(walk);
  }
  roots.forEach(walk);
  return result;
}

function convFactor(node, allNodes) {
  if (node.isBase === 1) return 1;
  const child = allNodes.find((n) => n.parentId === node.specId);
  if (!child) return 1;
  return (child.qtyPerParent || 1) * convFactor(child, allNodes);
}

function unitOptions(row) {
  const opts = [];
  const psId = row.materialPackSpecId;
  if (psId && allPackSpecs.value.length) {
    const set = allPackSpecs.value.filter((p) => p.rootId === psId && p.isDel !== 1);
    const sorted = sortTopDown(set);
    sorted.forEach((p) => {
      if (p.uom && !opts.some((o) => o.value === p.uom)) {
        let label = p.uom;
        if (p.isBase === 1) {
          label += "（基本单位）";
        } else if (!p.parentId) {
          label += "（顶层）";
        } else {
          const factor = convFactor(p, set);
          const baseUom = set.find((n) => n.isBase === 1)?.uom || p.uom;
          label += `（1${p.uom}=${factor}${baseUom}）`;
        }
        opts.push({ specId: p.specId, value: p.uom, label });
      }
    });
  }
  if (row.materialUnit && !opts.some((o) => o.value === row.materialUnit)) {
    opts.push({ specId: "mu", value: row.materialUnit, label: row.materialUnit });
  }
  if (row.unit && !opts.some((o) => o.value === row.unit)) {
    opts.push({ specId: "cu", value: row.unit, label: row.unit });
  }
  return opts;
}

function defaultUnitFor(row) {
  const psId = row.materialPackSpecId;
  if (psId && allPackSpecs.value.length) {
    const set = allPackSpecs.value.filter((p) => p.rootId === psId && p.isDel !== 1);
    if (set.length) {
      const root = set.find((p) => !p.parentId);
      if (root) return root.uom;
      const base = set.find((p) => p.isBase === 1);
      if (base) return base.uom;
    }
  }
  return row.materialUnit || "";
}

getList();
</script>

<style scoped>
.detail-section {
  border-top: 1px solid #ebeef5;
  padding-top: 16px;
  margin-top: 4px;
}
.detail-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
}
.detail-title {
  font-size: 14px;
  font-weight: 600;
  color: #303133;
}
.detail-footer {
  text-align: right;
  font-size: 14px;
  color: #606266;
}
.detail-footer b {
  color: #303133;
  font-weight: 600;
}
</style>
