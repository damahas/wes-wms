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
        <el-button type="primary" plain icon="Plus" @click="handleAdd" v-hasPermi="['wms:receiving:add']">新增</el-button>
      </el-col>
      <el-col :span="1.5">
        <el-button type="success" plain icon="Edit" :disabled="single" @click="handleUpdate" v-hasPermi="['wms:receiving:edit']">修改</el-button>
      </el-col>
      <el-col :span="1.5">
        <el-button type="danger" plain icon="Delete" :disabled="multiple" @click="handleDelete" v-hasPermi="['wms:receiving:remove']">删除</el-button>
      </el-col>
        <right-toolbar
          v-model:showSearch="showSearch"
          @queryTable="getList"
        ></right-toolbar>
    </el-row>

    <!-- 列表 -->
    <el-table v-loading="loading" :data="receivingList" @selection-change="handleSelectionChange">
      <el-table-column type="selection" width="55" align="center" />
      <el-table-column label="收货单号" align="center" prop="receivingNo" :show-overflow-tooltip="true" width="180" />
      <el-table-column label="供应商" align="center" :show-overflow-tooltip="true" min-width="180">
        <template #default="scope">
          {{ scope.row.supplier?.partnerName || "-" }}
        </template>
      </el-table-column>
      <el-table-column label="预计到货日期" align="center" min-width="170">
        <template #default="scope">
          {{ scope.row.expectDate ? scope.row.expectDate.slice(0, 10) : "-" }}
        </template>
      </el-table-column>
      <el-table-column label="承运人" align="center" prop="carrierName" :show-overflow-tooltip="true" width="100" />
      <el-table-column label="承运人电话" align="center" prop="carrierPhone" :show-overflow-tooltip="true" width="130" />

      <el-table-column label="状态" align="center" prop="receivingStatus" width="80">
        <template #default="scope">
          <el-tag :type="statusTagType(scope.row.receivingStatus)" size="small">
            {{ statusLabel(scope.row.receivingStatus) }}
          </el-tag>
        </template>
      </el-table-column>
      <el-table-column label="创建时间" align="center" prop="createTime" min-width="170" />
      <el-table-column label="操作" align="center" width="150" class-name="small-padding fixed-width">
        <template #default="scope">
          <el-button link type="primary" icon="View" @click="handleUpdate(scope.row)" v-hasPermi="['wms:receiving:edit']" v-if="isFinalStatus(scope.row.receivingStatus)">查看</el-button>
          <el-button link type="primary" icon="Edit" @click="handleUpdate(scope.row)" v-hasPermi="['wms:receiving:edit']" v-else>修改</el-button>
          <el-button link type="danger" icon="Delete" @click="handleDelete(scope.row)" v-hasPermi="['wms:receiving:remove']" v-if="scope.row.receivingStatus === 'pending'">删除</el-button>
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
      <el-form ref="receivingRef" :model="form" :rules="rules" label-width="110px" class="mb16">
        <el-row :gutter="20">
          <el-col :span="8">
            <el-form-item label="收货单号" prop="receivingNo">
              <el-input v-model="form.receivingNo" placeholder="系统自动生成" maxlength="50" :readonly="true" />
            </el-form-item>
          </el-col>
          <el-col :span="8">
            <el-form-item label="供应商" prop="supplierId">
              <PartnerSelect v-model="form.supplierId" partner-type="supplier" placeholder="请选择供应商" @change="onSupplierChange" :disabled="readOnly" />
            </el-form-item>
          </el-col>
          <el-col :span="8">
            <el-form-item label="预计到货日期" prop="expectDate">
              <el-date-picker v-model="form.expectDate" type="date" value-format="YYYY-MM-DD"
                placeholder="选择日期" style="width:100%" :disabled="readOnly" />
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="20">
          <el-col :span="8">
            <el-form-item label="承运人" prop="carrierName">
              <el-input v-model="form.carrierName" placeholder="请输入承运人" maxlength="100" :disabled="readOnly" />
            </el-form-item>
          </el-col>
          <el-col :span="8">
            <el-form-item label="承运人电话" prop="carrierPhone">
              <el-input v-model="form.carrierPhone" placeholder="请输入承运人电话" maxlength="20" :disabled="readOnly" />
            </el-form-item>
          </el-col>
          <el-col :span="8">
            <el-form-item label="状态" prop="receivingStatus">
              <el-select v-model="form.receivingStatus" placeholder="请选择状态" style="width:100%" :disabled="readOnly">
                <el-option label="待收货" value="pending" />
                <el-option label="收货中" value="arriving" />
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
          <span class="detail-title">收货明细</span>
          <el-button class="detail-add-btn" type="primary" icon="Plus" @click="openMaterialDialog" v-hasPermi="['wms:receiving:add']" v-if="!readOnly">添加明细</el-button>
        </div>
        <el-table :data="detailList" border stripe max-height="350" style="width:100%">
          <el-table-column label="序号" type="index" width="55" align="center" />
          <el-table-column label="操作" width="60" align="center" v-if="!readOnly">
            <template #default="scope">
              <el-button link type="danger" icon="Delete"
                @click="removeDetailRow(scope.$index)" />
            </template>
          </el-table-column>
          <el-table-column label="物料" min-width="220">
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
          <el-table-column label="单位" prop="unit" width="120">
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
          <el-table-column label="收货数量" prop="planQuantity" width="160">
            <template #default="scope">
              <el-input-number controls-position="right" v-model="scope.row.planQuantity" :min="1" :controls="true"
                style="width:100%" :disabled="readOnly" />
            </template>
          </el-table-column>
          <el-table-column label="已入库数" prop="stockInQuantity" width="100" align="center">
            <template #default="scope">
              {{ scope.row.stockInQuantity ?? 0 }}
            </template>
          </el-table-column>
          <el-table-column label="待入库数" prop="pendingInboundQty" width="100" align="center">
            <template #default="scope">
              {{ scope.row.pendingInboundQty ?? 0 }}
            </template>
          </el-table-column>
          <el-table-column label="退货数量" prop="returnQty" width="100" align="center">
            <template #default="scope">
              {{ scope.row.returnQty ?? 0 }}
            </template>
          </el-table-column>

</el-table>
        <div class="detail-footer mt8">
          <span>合计数量：<b>{{ totalPlanQuantity }}</b></span>
        </div>
      </div>

      <template #footer>
        <div class="dialog-footer" style="display: flex; justify-content: space-between; align-items: center;">
          <div>
            <template v-if="!readOnly && form.receivingId && form.receivingStatus === 'pending'">
              <el-button type="success" icon="CircleCheck" @click="handleConfirmReceiptFromForm">确认收货</el-button>
              <el-button type="danger" icon="CircleClose" @click="handleCancelReceiptFromForm">取消收货</el-button>
            </template>
          </div>
          <div>
            <template v-if="readOnly">
              <el-button @click="cancel">关 闭</el-button>
            </template>
            <template v-else>
              <el-button type="primary" @click="submitForm">保 存</el-button>
              <el-button @click="cancel">取 消</el-button>
            </template>
          </div>
        </div>
      </template>
    </el-dialog>

    <!-- 隐藏的物料多选组件，通过 ref 调用 open() 弹出 -->
    <MaterialMultiSelect
      ref="materialMultiSelectRef"
      :multiple="true"
      @change="onMaterialMultiSelect"
      v-show="false"
    />
    </div>
  </div>
</template>

<script setup name="WmsReceiving">
import { ref, reactive, toRefs, computed, watch, nextTick } from "vue";
import { ElMessage, ElMessageBox } from "element-plus";
import {
  listReceiving,
  getReceiving,
  getReceivingDetailList,
  addReceiving,
  updateReceiving,
  delReceiving,
  confirmReceipt,
} from "@/api/wms/receiving";
import { listPackSpecTree } from "@/api/wms/packSpec";
import { getMaterial } from "@/api/wms/material";
import QueryForm from "@/components/QueryForm/index.vue";
import RightToolbar from "@/components/RightToolbar/index.vue";
import MaterialSelect from "@/views/wms/material/input.vue";
import MaterialMultiSelect from "@/views/wms/material/select.vue";
import PartnerSelect from "@/views/wms/partner/input.vue";

const receivingRef = ref(null);
const receivingList = ref([]);
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

// 全部包装规格（弹窗打开时拉取一次，用于按物料包装规格构建单位下拉项）
const allPackSpecs = ref([]);

// ============ 物料多选弹窗（复用 material/select.vue）============
const materialMultiSelectRef = ref(null);

function openMaterialDialog() {
  materialMultiSelectRef.value?.open();
}

async function onMaterialMultiSelect(materials) {
  if (!materials || materials.length === 0) return;
  for (const mat of materials) {
    // 检查是否已存在相同物料
    if (detailList.value.some((d) => d.materialId === mat.materialId)) {
      continue;
    }
    const row = {
      detailId: 0,
      receivingId: 0,
      materialId: mat.materialId,
      materialCode: mat.materialCode || "",
      materialName: mat.materialName || "",
      materialPackSpecId: null,
      materialUnit: "",
      spec: mat.spec || "",
      unit: "",
      batchNo: "",
      planQuantity: 1,
      receivedQuantity: 0,
        stockInQuantity: 0,
    };
    // 加载包装规格和免检状态
    if (mat.materialId) {
      try {
        const m = await getMaterial(mat.materialId);
        const fullMat = m.data || m;
        row.materialPackSpecId = fullMat.packSpecId || null;
        row.materialUnit = fullMat.unit || "";
        await ensurePackSpecLoaded(fullMat.packSpecId);
        row.unit = defaultUnitFor(row);
      } catch { /* ignore */ }
    }
    detailList.value.push(row);
  }
  ElMessage.success(`已添加 ${detailList.value.length} 条明细`);
}
// ============ 物料多选弹窗 END ============

const data = reactive({
  form: {},
  queryParams: {
    pageNum: 1,
    pageSize: 10,
    keywords: undefined,
    supplierId: undefined,
    receivingStatus: undefined,
  },
  rules: {
    supplierId: [{ required: true, message: "请选择供应商", trigger: "change" }],
    expectDate: [{ required: true, message: "请选择预计到货日期", trigger: "change" }],
  },
});

const { form, queryParams, rules } = toRefs(data);

const queryConfig = computed(() => [
  {
    label: "关键字",
    prop: "keywords",
    type: "input",
    placeholder: "收货单号/供应商/承运人",
  },
  {
    label: "状态",
    prop: "receivingStatus",
    type: "select",
    placeholder: "请选择状态",
    options: [
      { label: "待收货", value: "pending" },
      { label: "收货中", value: "arriving" },
      { label: "已完成", value: "completed" },
      { label: "已取消", value: "cancelled" },
    ],
  },
]);

// 状态映射（与后端一致：pending-待收货 arriving-收货中 completed-已完成 cancelled-已取消）
const statusMap = {
  pending: "待收货",
  arriving: "收货中",
  completed: "已完成",
  cancelled: "已取消",
};
function statusLabel(s) { return statusMap[s] || s; }
function statusTagType(s) {
  const map = { pending: "warning", arriving: "primary", completed: "success", cancelled: "info" };
  return map[s] || "";
}

// 明细合计
const totalPlanQuantity = computed(() =>
  detailList.value.reduce((s, r) => s + (r.planQuantity || 0), 0)
);

/** 查询列表 */
function getList() {
  loading.value = true;
  listReceiving(queryParams.value).then((res) => {
    receivingList.value = res.rows || [];
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
    receivingStatus: undefined,
    pageNum: 1,
    pageSize: 10,
  };
  handleQuery();
}

/** 多选 */
function handleSelectionChange(selection) {
  ids.value = selection.map((item) => item.receivingId);
  single.value = selection.length !== 1;
  multiple.value = !selection.length;
}

function getDefaultExpectDate() {
  const d = new Date();
  d.setDate(d.getDate() + 7);
  return d.toISOString().slice(0, 10);
}

/** 表单重置 */
function reset() {
  form.value = {
    receivingId: undefined,
    receivingNo: undefined,
    supplierId: undefined,
    supplierName: undefined,
    expectDate: getDefaultExpectDate(),
    carrierName: undefined,
    carrierPhone: undefined,
    totalQuantity: 0,
    receivingStatus: "pending",
    remark: undefined,
  };
  detailList.value = [];
  if (receivingRef.value) receivingRef.value.resetFields();
}

/** 是否终态（已完成/已取消 → 不可编辑） */
function isFinalStatus(s) { return s === "completed" || s === "cancelled"; }

/** 新增 */
async function handleAdd() {
  reset();
  readOnly.value = false;
  open.value = true;
  title.value = "添加收货单";
}

/** 修改/查看 */
async function handleUpdate(row) {
  reset();
  const id = row.receivingId || ids.value[0];
  const res = await getReceiving(id);
  form.value = res.data || {};
  readOnly.value = isFinalStatus(form.value.receivingStatus);
  // 加载明细，并补全物料对应的包装规格/最小单位，回填默认单位
  const dRes = await getReceivingDetailList(id);
  const details = dRes.data || [];
  await Promise.all(
    details.map(async (d) => {
      d.materialPackSpecId = null;
      d.materialUnit = d.unit || "";
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
  title.value = readOnly.value ? "查看收货单" : "修改收货单";
}

/** 提交 */
function submitForm() {
  receivingRef.value.validate((valid) => {
    if (!valid) return;
    if (detailList.value.length === 0) {
      ElMessage.warning("请至少添加一条收货明细");
      return;
    }
    const dto = {
      receiving: { ...form.value },
      details: detailList.value.map((d) => ({
        ...d,
        materialCode: d.materialCode || "",
        materialName: d.materialName || "",
      })),
    };
    const api = form.value.receivingId ? updateReceiving : addReceiving;
    api(dto).then(() => {
      ElMessage.success("保存成功");
      open.value = false;
      getList();
    });
  });
}

/** 确认收货（弹窗内 → 确认框 → 调接口：免检→入库，需检→检验单） */
function handleConfirmReceiptFromForm() {
  const id = form.value.receivingId;
  if (!id) { ElMessage.warning("请先保存收货单"); return; }
  ElMessageBox.confirm("确认收货将自动根据物料检验配置，免检物料直接生成入库单，需检物料生成检验单，是否继续？", "提示", {
    confirmButtonText: "确定",
    cancelButtonText: "取消",
    type: "success",
  }).then(async () => {
    await confirmReceipt(id);
    ElMessage.success("收货确认成功");
    open.value = false;
    getList();
  }).catch(() => { /* 用户取消 */ });
}

/** 取消收货（弹窗内 → 确认框 → 调接口） */
function handleCancelReceiptFromForm() {
  const id = form.value.receivingId;
  if (!id) { ElMessage.warning("请先保存收货单"); return; }
  ElMessageBox.confirm("确认取消该收货单？", "警告", {
    confirmButtonText: "确定",
    cancelButtonText: "取消",
    type: "warning",
  }).then(async () => {
    const details = detailList.value.map((d) => ({
      ...d,
      materialCode: d.materialCode || "",
      materialName: d.materialName || "",
    }));
    await updateReceiving({ receiving: { ...form.value, receivingStatus: "cancelled" }, details });
    ElMessage.success("已取消");
    open.value = false;
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
  const delIds = row.receivingId || ids.value.join(",");
  ElMessageBox.confirm(`是否确认删除该收货单及其明细？`, "警告", {
    confirmButtonText: "确定",
    cancelButtonText: "取消",
    type: "warning",
  })
    .then(() => delReceiving(delIds))
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
  // 按需加载该物料的包装规格树（只加载一次，缓存复用）
  await ensurePackSpecLoaded(data.packSpecId);
  row.unit = defaultUnitFor(row);
}

// ============ 明细操作 ============
function addDetailRow() {
  detailList.value.push({
    detailId: 0,
    receivingId: 0,
    materialId: undefined,
    materialCode: "",
    materialName: "",
    materialPackSpecId: null,
    materialUnit: "",
    spec: "",
    unit: "",
    batchNo: "",
    planQuantity: 1,
    receivedQuantity: 0,
    stockInQuantity: 0,
  });
}


// ============ 包装规格/单位 辅助 ============
/**
 * 按需加载指定 rootId 的包装规格层级树（缓存复用，避免弹窗打开时全局加载）
 */
async function ensurePackSpecLoaded(rootId) {
  if (!rootId) return;
  if (allPackSpecs.value.some((p) => p.rootId === rootId)) return;
  try {
    const r = await listPackSpecTree(rootId);
    const nodes = r.rows || r.data || [];
    allPackSpecs.value.push(...nodes);
  } catch { /* ignore */ }
}

/**
 * 按层级排序（自顶向下）
 */
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

/**
 * 计算某节点到基本单位的换算系数（从本层到 is_base=1 层所有 qtyPerParent 的乘积）
 */
function convFactor(node, allNodes) {
  if (node.isBase === 1) return 1;
  const child = allNodes.find((n) => n.parentId === node.specId);
  if (!child) return 1;
  return (child.qtyPerParent || 1) * convFactor(child, allNodes);
}

/**
 * 某物料对应的单位下拉项：
 * - 取该包装规格各层的 uom 去重，自顶向下排序
 * - label 带层级/换算信息：顶层、中间层带换算、基本单位
 * - 无包装规格时取物料最小单位
 */
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
          // 中间层：显示 1本层=N基本单位
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
  // 兜底：保证已存单位可选
  if (row.unit && !opts.some((o) => o.value === row.unit)) {
    opts.push({ specId: "cu", value: row.unit, label: row.unit });
  }
  return opts;
}

/**
 * 默认单位：收货场景优先取顶层（最大包装单位，如"箱"/"托盘"）；
 * 无法判断顶层则取基本单位；无包装规格则用物料基础单位。
 */
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

function removeDetailRow(index) {
  detailList.value.splice(index, 1);
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
