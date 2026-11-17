<template>
  <div class="app-container">
    <div class="main-panel p16">
      <query-form :config="queryConfig" v-model:visible="showSearch" v-model="queryParams" @query="handleQuery"
        @reset="resetQuery" />
      <el-row :gutter="10" class="mb8">
        <right-toolbar v-model:showSearch="showSearch" @queryTable="getList" />
      </el-row>

      <!-- 列表 -->
      <el-table v-loading="loading" :data="dataList">
        <el-table-column label="检验单号" align="center" min-width="200" show-overflow-tooltip>
          <template #default="scope">
            <el-link type="primary" @click="handleView(scope.row)">{{ scope.row.inspectionNo }}</el-link>
          </template>
        </el-table-column>
        <el-table-column label="收货单号" align="center" min-width="160" show-overflow-tooltip>
          <template #default="scope">{{ scope.row.receiving?.receivingNo || "-" }}</template>
        </el-table-column>
        <el-table-column label="供应商" align="center" min-width="180" show-overflow-tooltip>
          <template #default="scope">{{ scope.row.supplier?.partnerName || "-" }}</template>
        </el-table-column>
        <el-table-column label="物料" align="center" min-width="160" show-overflow-tooltip>
          <template #default="scope">{{ scope.row.material?.materialName || scope.row.material?.materialCode || "-"
            }}</template>
        </el-table-column>
        <el-table-column label="规格" align="center" prop="spec" min-width="120" show-overflow-tooltip />
        <el-table-column label="收货数量" align="center" prop="deliveryQuantity" width="100" />
        <el-table-column label="允许入库数" align="center" prop="allowQuantity" width="100" />
        <el-table-column label="不良数量" align="center" prop="defectQuantity" width="100" />
        <el-table-column label="退货数量" align="center" prop="returnQuantity" width="100" />
        <el-table-column label="状态" align="center" width="100">
          <template #default="scope">
            <el-tag :type="statusTagType(scope.row.inspectStatus)">{{ statusLabel(scope.row.inspectStatus) }}</el-tag>
          </template>
        </el-table-column>
        <el-table-column label="检验人" align="center" width="100">
          <template #default="scope">{{ scope.row.inspector || "-" }}</template>
        </el-table-column>
        <el-table-column label="检验日期" align="center" min-width="170">
          <template #default="scope">{{ scope.row.inspectDate ? scope.row.inspectDate.slice(0, 10) : "-" }}</template>
        </el-table-column>
        <el-table-column label="操作" align="center" width="160" fixed="right" class-name="iqc-op-col">
          <template #default="scope">
            <el-button link type="primary" icon="View" @click="handleView(scope.row)">查看</el-button>
            <el-button v-if="scope.row.inspectStatus !== 'completed'" link type="primary" icon="Edit"
              @click="handleEdit(scope.row)" v-hasPermi="['wms:iqcOrder:edit']">检验</el-button>
          </template>
        </el-table-column>
      </el-table>

      <pagination v-show="total > 0" :total="total" v-model:page="queryParams.pageNum"
        v-model:limit="queryParams.pageSize" @pagination="getList" />

      <!-- 编辑/查看弹窗 -->
      <el-dialog :title="title" v-model="open" width="90%" append-to-body @close="cancel">
        <!-- 检验单头信息 -->
        <el-descriptions :column="4" border class="mb16 desc-even">
          <el-descriptions-item label="检验单号">{{ form.inspectionNo || "-" }}</el-descriptions-item>
          <el-descriptions-item label="收货单号">{{ form.receiving?.receivingNo || "-" }}</el-descriptions-item>
          <el-descriptions-item label="供应商">{{ form.supplier?.partnerName || "-" }}</el-descriptions-item>
          <el-descriptions-item label="状态">
            <el-tag :type="statusTagType(form.inspectStatus)">{{ statusLabel(form.inspectStatus) }}</el-tag>
          </el-descriptions-item>
          <el-descriptions-item label="物料编码">{{ form.material?.materialCode || "-" }}</el-descriptions-item>
          <el-descriptions-item label="物料名称">{{ form.material?.materialName || "-" }}</el-descriptions-item>
          <el-descriptions-item label="规格">{{ form.spec || "-" }}</el-descriptions-item>
          <el-descriptions-item label="单位">{{ form.unit || "-" }}</el-descriptions-item>
          <el-descriptions-item label="批次号">{{ form.batchNo || "-" }}</el-descriptions-item>
          <el-descriptions-item label="收货数量">{{ form.deliveryQuantity || 0 }}</el-descriptions-item>
          <el-descriptions-item label="检验人">
            <UserInput v-if="!readOnly" v-model="inspectorId" :label="form.inspector" @change="onInspectorChange" />
            <span v-else>{{ form.inspector || "-" }}</span>
          </el-descriptions-item>
          <el-descriptions-item label="允许入库数">
            <el-input-number v-if="!readOnly" v-model="form.allowQuantity" :min="0" :max="form.deliveryQuantity || 0"
              controls-position="right" />
            <span v-else>{{ form.allowQuantity ?? 0 }}</span>
          </el-descriptions-item>
          <el-descriptions-item label="处置方式">
            <el-select v-if="!readOnly" v-model="form.disposeType" placeholder="请选择" style="width:100%"
              @change="onDisposeTypeChange">
              <el-option label="无需处置" value="none" />
              <el-option label="退回供应商" value="return" />
              <el-option label="让步接收" value="concession" />
              <el-option label="挑选使用" value="sorting" />
            </el-select>
            <span v-else>{{ disposeLabel(form.disposeType) }}</span>
          </el-descriptions-item>
          <el-descriptions-item v-if="showReturnQty" label="退货数量">
            <el-input-number v-if="!readOnly" v-model="form.returnQuantity" :min="0" :max="form.deliveryQuantity || 0"
              controls-position="right" />
            <span v-else>{{ form.returnQuantity ?? 0 }}</span>
          </el-descriptions-item>
          <el-descriptions-item v-if="showDefectQty" label="不良数量">
            <el-input-number v-if="!readOnly" v-model="form.defectQuantity" :min="0" :max="form.deliveryQuantity || 0"
              controls-position="right" />
            <span v-else>{{ form.defectQuantity ?? 0 }}</span>
          </el-descriptions-item>
          <el-descriptions-item label="备注" :span="4">
            <el-input v-if="!readOnly" v-model="form.remark" placeholder="备注" />
            <span v-else>{{ form.remark || "-" }}</span>
          </el-descriptions-item>
        </el-descriptions>

        <!-- 检验明细 -->
        <div class="iqc-section-title">检验明细</div>
        <div v-if="items.length > 0">
          <el-table :data="items" border style="width:100%">
            <el-table-column label="序号" type="index" width="55" align="center" />
            <el-table-column label="检验项编码" width="130" align="center">
              <template #default="s">
                <span class="item-code-cell">
                  <span v-if="s.row.isCritical === 1" class="critical-icon" title="关键项">!</span>
                  <span>{{ s.row.itemCode }}</span>
                </span>
              </template>
            </el-table-column>
            <el-table-column label="检验项名称" prop="itemName" min-width="160" show-overflow-tooltip />
            <el-table-column label="检验类型" prop="inspectionType" width="90" show-overflow-tooltip />
            <el-table-column label="标准值" width="140" show-overflow-tooltip>
              <template #default="s">{{ s.row.standardValue || "-" }}{{ s.row.unit ? " " + s.row.unit : "" }}</template>
            </el-table-column>
            <el-table-column label="检测方法/工具" prop="testMethod" min-width="160" show-overflow-tooltip>
              <template #default="s">{{ s.row.testMethod || "-" }}</template>
            </el-table-column>
            <el-table-column label="检验结果" width="90" align="center" v-if="!readOnly">
              <template #default="s">
                <el-switch v-model="s.row.itemResult" active-value="OK" inactive-value="NG" active-text="OK"
                  inactive-text="NG" inline-prompt />
              </template>
            </el-table-column>
            <el-table-column label="检验结果" width="80" align="center" v-else>
              <template #default="s">
                <el-tag :type="s.row.itemResult === 'OK' ? 'success' : s.row.itemResult === 'NG' ? 'danger' : ''">
                  {{ s.row.itemResult || "-" }}
                </el-tag>
              </template>
            </el-table-column>
          </el-table>
        </div>
        <div v-else style="color:#909399;font-size:13px;padding:16px 0;">暂无检验项，请先配置物料分类的检验项</div>

        <template #footer>
          <div class="dialog-footer" style="display:flex;justify-content:space-between;align-items:center;">
            <div>
              <template v-if="!readOnly && form.inspectionId && form.inspectStatus !== 'completed'">
                <el-button type="success" icon="CircleCheck" @click="handleComplete">完成检验</el-button>
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
    </div>
  </div>
</template>

<script setup name="WmsIqcOrder">
import { ref, reactive, computed } from "vue";
import { useStore } from "vuex";
import { ElMessage, ElMessageBox } from "element-plus";
import {
  listIqcOrder,
  getIqcOrder,
  getIqcOrderItems,
  updateIqcOrder,
  completeIqcOrder,
} from "@/api/wms/iqcOrder";
import { listIqcItemByMaterial } from "@/api/wms/iqcItem";
import QueryForm from "@/components/QueryForm/index.vue";
import RightToolbar from "@/components/RightToolbar/index.vue";
import UserInput from "@/views/system/user/input.vue";

const store = useStore();
const currentUser = computed(() => store.getters["user/userInfo"]);

const dataList = ref([]);
const open = ref(false);
const loading = ref(true);
const total = ref(0);
const title = ref("");
const readOnly = ref(false);
const showSearch = ref(true);

const queryParams = reactive({ pageNum: 1, pageSize: 10, keywords: undefined, inspectStatus: undefined });
const queryConfig = [
  { label: "关键词", prop: "keywords", type: "input", placeholder: "检验单号/收货单号" },
  {
    label: "状态", prop: "inspectStatus", type: "select", placeholder: "请选择状态",
    options: [{ label: "待检验", value: "pending" }, { label: "检验中", value: "inspecting" }, { label: "已完成", value: "completed" }]
  },
];

const statusMap = { pending: "待检验", inspecting: "检验中", completed: "已完成" };
function statusLabel(s) { return statusMap[s] || s; }
function statusTagType(s) { const m = { pending: "warning", inspecting: "", completed: "success" }; return m[s] || ""; }

const form = ref({});
const items = ref([]);

/* 检验人选择 -- UserInput 使用 inspectorId（userId）驱动回显，实际存储 userName 到 form.inspector */
const inspectorId = ref(null);
function onInspectorChange(item) {
  if (item) {
    form.value.inspector = item.userName || item.nickName || "";
    inspectorId.value = item.userId;
  } else {
    form.value.inspector = "";
    inspectorId.value = null;
  }
}

/** 初始化检验人：已有值用 label 模式，无值时默认当前登录人 */
function initInspectorId() {
  if (readOnly.value) return;
  if (form.value.inspector) {
    inspectorId.value = null;
  } else if (currentUser.value) {
    form.value.inspector = currentUser.value.nickName || currentUser.value.userName || "";
    inspectorId.value = currentUser.value.userId ?? null;
  }
}

/** 防止数量联动造成循环触发 */
/** 收货数量与允许入库数的差额（仅用于初始化默认处置方式，不做联动计算） */
const diffQty = computed(() =>
  Math.max(0, (form.value.deliveryQuantity || 0) - (form.value.allowQuantity || 0))
);

/** 处置方式 none-无需处置 return-退回供应商 concession-让步接收 sorting-挑选使用 */
const disposeMap = { none: "无需处置", return: "退回供应商", concession: "让步接收", sorting: "挑选使用" };
function disposeLabel(v) { return disposeMap[v] || "-"; }

/** 是否存在不良：存在差额 或 存在 NG 检验项 */
const hasDefect = computed(() => diffQty.value > 0 || items.value.some((i) => i.itemResult === "NG"));

/** 无需处置时不显示退货数量 */
const showReturnQty = computed(() =>
  !!form.value.disposeType && form.value.disposeType !== "none"
);

/** 仅让步接收/挑选使用时显示不良数量 */
const showDefectQty = computed(() =>
  form.value.disposeType === "concession" || form.value.disposeType === "sorting"
);

/** 初始化处置方式：已保存的值保持不变；否则允许入库数=收货数量 → 无需处置，有不良 → 退回供应商 */
function initDisposeType() {
  const cur = form.value.disposeType;
  if (cur && disposeMap[cur]) return;
  form.value.disposeType = hasDefect.value ? "return" : "none";
}

/** 处置方式变化：无需处置时清空退货/不良数量 */
function onDisposeTypeChange() {
  if (form.value.disposeType === "none") {
    form.value.returnQuantity = 0;
    form.value.defectQuantity = 0;
  }
}

function getList() {
  loading.value = true;
  listIqcOrder(queryParams).then(resp => {
    dataList.value = resp.rows || [];
    total.value = resp.total || 0;
  }).catch(() => {
    dataList.value = [];
    total.value = 0;
  }).finally(() => { loading.value = false; });
}

/** 标签页重新激活时由布局回调（是否刷新由页面决定） */
function tabRefresh() {
  getList();
}

function handleQuery() { queryParams.pageNum = 1; getList(); }
function resetQuery() { queryParams.keywords = undefined; queryParams.inspectStatus = undefined; queryParams.pageNum = 1; getList(); }
function handleView(row) { loadForm(row.inspectionId, true); }
function handleEdit(row) { loadForm(row.inspectionId, false); }

/** 加载弹窗数据 */
async function loadForm(inspectionId, isReadOnly) {
  readOnly.value = isReadOnly;
  title.value = isReadOnly ? "查看检验单" : "检验单";
  open.value = true;
  items.value = [];

  try {
    const res = await getIqcOrder(inspectionId);
    form.value = res.data || {};
  } catch {
    open.value = false;
    return;
  }
  // 允许入库数默认等于收货数量；退货数量/不良数量不做联动计算，由用户按处置方式自行填写
  if (form.value.allowQuantity == null) form.value.allowQuantity = form.value.deliveryQuantity || 0;
  if (form.value.returnQuantity == null) form.value.returnQuantity = 0;
  if (form.value.defectQuantity == null) form.value.defectQuantity = 0;

  // 加载已有的检验项结果
  try {
    const itemsRes = await getIqcOrderItems(inspectionId);
    const raw = itemsRes.rows || itemsRes.data || [];
    if (raw.length > 0) {
      items.value = raw.map(i => ({
        ...i,
        itemResult: i.itemResult || (i.isPassed === true ? "OK" : i.isPassed === false ? "NG" : "OK"),
      }));
    } else {
      // 没有已保存的检验项，从模板加载
      await loadTemplateItems();
    }
  } catch { await loadTemplateItems(); }
  // 检验项加载完成后初始化处置方式（需依赖 NG 项判断是否存在不良）
  initDisposeType();
  initInspectorId();
}

/** 从物料分类的检验项模板加载 */
async function loadTemplateItems() {
  const materialId = form.value.material?.materialId || form.value.materialId || 0;
  if (!materialId) return;
  let templates = [];
  try {
    const res = await listIqcItemByMaterial(materialId);
    templates = res.rows || res.data || [];
  } catch { templates = []; }
  if (templates.length === 0) return;
  items.value = templates.map((t, idx) => ({
    itemId: 0,
    inspectionId: form.value.inspectionId || 0,
    itemCode: t.itemCode,
    itemName: t.itemName,
    inspectionType: t.inspectionType,
    standardValue: t.standardValue,
    unit: t.unit,
    isCritical: t.isCritical,
    testMethod: t.testMethod,
    sortNo: t.sortNo ?? idx + 1,
    itemResult: "OK",
  }));
}

/** 保存 */
async function submitForm() {
  try {
    if (form.value.inspectStatus === "pending") form.value.inspectStatus = "inspecting";
    const itemsToSave = items.value.map(i => ({
      ...i,
      isPassed: i.itemResult === "OK",
    }));
    await updateIqcOrder({
      inspection: { ...form.value, supplier: undefined, material: undefined, receiving: undefined, receivingDetail: undefined },
      items: itemsToSave,
    });
    ElMessage.success("保存成功");
    open.value = false;
    getList();
  } catch { /* API 层已统一提示 */ }
}

/** 完成检验 */
function handleComplete() {
  const id = form.value.inspectionId;
  if (!id) { ElMessage.warning("请先保存检验单"); return; }
  ElMessageBox.confirm("确认完成检验？完成后数据将回写到收货单明细。", "提示",
    { confirmButtonText: "确定", cancelButtonText: "取消", type: "success" }
  ).then(async () => {
    try {
      if (form.value.inspectStatus === "pending") form.value.inspectStatus = "inspecting";
      const itemsToSave = items.value.map(i => ({ ...i, isPassed: i.itemResult === "OK" }));
      await updateIqcOrder({
        inspection: { ...form.value, supplier: undefined, material: undefined, receiving: undefined, receivingDetail: undefined },
        items: itemsToSave,
      });
      await completeIqcOrder(id);
      ElMessage.success("检验完成，数据已回写到收货单");
      open.value = false;
      getList();
    } catch { /* API 层已统一提示 */ }
  }).catch(() => { });
}

function cancel() { open.value = false; items.value = []; }

getList();
</script>

<style>
/* 检验单 el-descriptions 四列均分 */
.desc-even .el-descriptions__table {
  table-layout: fixed;
}

/* 检验项编码单元格，确保图标和文字水平中线对齐 */
.item-code-cell {
  display: inline-flex;
  align-items: center;
}

/* 检验项关键项图标 */
.critical-icon {
  display: inline-flex;
  align-items: center;
  justify-content: center;
  width: 14px;
  height: 14px;
  font-size: 10px;
  font-weight: bold;
  color: #fff;
  background: #f56c6c;
  border-radius: 50%;
  margin-right: 4px;
  cursor: default;
  flex-shrink: 0;
}

.iqc-section-title {
  margin: 4px 0 12px;
  padding-left: 8px;
  font-size: 14px;
  font-weight: 600;
  color: #303133;
  border-left: 3px solid var(--el-color-primary);
}

/* 固定操作列：背景不透明 + 层级高于普通单元格（element-plus 默认 background:inherit 会透） */
.iqc-op-col {
  position: sticky !important;
  right: 0 !important;
  z-index: 10 !important;
  background-color: #fff !important;
}
.el-table__body tr:hover > .iqc-op-col,
.el-table__body tr.hover-row > .iqc-op-col {
  background-color: #f5f7fa !important;
}
.el-table__header-wrapper th.iqc-op-col {
  background-color: #f7f6f4 !important;
}
html.dark .iqc-op-col {
  background-color: var(--bg-card) !important;
}
html.dark .el-table__body tr:hover > .iqc-op-col,
html.dark .el-table__body tr.hover-row > .iqc-op-col {
  background-color: var(--bg-hover) !important;
}
html.dark .el-table__header-wrapper th.iqc-op-col {
  background-color: var(--bg-hover) !important;
}

</style>
