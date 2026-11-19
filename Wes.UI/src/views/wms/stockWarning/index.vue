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
          <el-button type="primary" plain icon="Promotion" @click="handleRun" v-hasPermi="['wms:stockWarning:edit']">{{ t('wms.stockWarning.runNow') }}</el-button>
        </el-col>
        <el-col :span="1.5">
          <el-button type="danger" plain icon="Delete" :disabled="multiple || selectedRows.some((r) => r.warningStatus !== 'pending')"
            @click="handleDelete" v-hasPermi="['wms:stockWarning:remove']">{{ t('wms.stockWarning.delete') }}</el-button>
        </el-col>
        <right-toolbar
          v-model:showSearch="showSearch"
          @queryTable="getList"
          :hint="t('wms.stockWarning.hint')"
        />
      </el-row>

      <!-- 列表 -->
      <el-table v-loading="loading" :data="warningList" @selection-change="handleSelectionChange">
        <el-table-column type="selection" width="55" align="center" />
        <el-table-column :label="t('wms.stockWarning.warningType')" align="center" width="110">
          <template #default="scope">
            <el-tag size="small" :type="scope.row.warningType === 'min_stock' ? 'danger' : 'warning'">
              {{ warningTypeMap[scope.row.warningType] || "-" }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column :label="t('wms.stockWarning.materialCode')" align="center" min-width="130" :show-overflow-tooltip="true">
          <template #default="scope">{{ scope.row.material?.materialCode || "-" }}</template>
        </el-table-column>
        <el-table-column :label="t('wms.stockWarning.materialName')" align="center" min-width="150" :show-overflow-tooltip="true">
          <template #default="scope">{{ scope.row.material?.materialName || "-" }}</template>
        </el-table-column>
        <el-table-column :label="t('wms.stockWarning.warehouse')" align="center" :show-overflow-tooltip="true">
          <template #default="scope">{{ scope.row.warehouse?.warehouseName || "-" }}</template>
        </el-table-column>
        <el-table-column :label="t('wms.stockWarning.zone')" align="center" :show-overflow-tooltip="true">
          <template #default="scope">{{ scope.row.zone?.zoneName || "-" }}</template>
        </el-table-column>
        <el-table-column :label="t('wms.stockWarning.location')" align="center" width="100" :show-overflow-tooltip="true">
          <template #default="scope">{{ scope.row.location?.locationCode || "-" }}</template>
        </el-table-column>
        <el-table-column :label="t('wms.stockWarning.currentQuantity')" align="center" prop="currentQuantity" width="90" />
        <el-table-column :label="t('wms.stockWarning.thresholdValue')" align="center" prop="thresholdValue" width="90" />
        <el-table-column :label="t('wms.stockWarning.diffValue')" align="center" width="90">
          <template #default="scope">
            <span :class="scope.row.warningType === 'min_stock' ? 'dir-out' : 'dir-in'">
              {{ scope.row.diffValue }}
            </span>
          </template>
        </el-table-column>
        <el-table-column :label="t('wms.stockWarning.level')" align="center" width="80">
          <template #default="scope">
            <el-tag size="small" :type="levelTag(scope.row.warningLevel)">
              {{ levelMap[scope.row.warningLevel] || "-" }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column :label="t('wms.stockWarning.status')" align="center" width="90">
          <template #default="scope">
            <el-tag size="small" :type="statusTag(scope.row.warningStatus)">
              {{ statusMap[scope.row.warningStatus] || "-" }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column :label="t('wms.stockWarning.triggerCount')" align="center" prop="triggerCount" width="80" />
        <el-table-column :label="t('wms.stockWarning.lastTrigger')" align="center" min-width="160">
          <template #default="scope">{{ scope.row.lastTriggerTime || scope.row.createTime }}</template>
        </el-table-column>
        <el-table-column :label="t('wms.stockWarning.warningTime')" align="center" prop="createTime" width="170" />
        <el-table-column :label="t('wms.stockWarning.handleRemark')" align="center" prop="handleRemark" min-width="140" :show-overflow-tooltip="true">
          <template #default="scope">{{ scope.row.handleRemark || "-" }}</template>
        </el-table-column>
        <el-table-column :label="t('wms.stockWarning.actions')" align="center" fixed="right" width="240" class-name="warn-op-col">
          <template #default="scope">
            <el-button link type="primary" icon="View" @click="openDetail(scope.row)">{{ t('wms.stockWarning.detail') }}</el-button>
            <template v-if="scope.row.warningStatus === 'pending'">
              <el-button link type="success" icon="Promotion" @click="openHandle(scope.row, 'handled')" v-hasPermi="['wms:stockWarning:edit']">{{ t('wms.stockWarning.handle') }}</el-button>
              <el-button link type="info" icon="Remove" @click="openHandle(scope.row, 'ignored')" v-hasPermi="['wms:stockWarning:edit']">{{ t('wms.stockWarning.ignore') }}</el-button>
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

      <!-- 详情对话框：预警周期 + 触发历史 -->
      <el-dialog :title="t('wms.stockWarning.detailTitle')" v-model="detailOpen" width="720px" append-to-body>
        <el-descriptions :column="3" border size="small" v-if="detailRow">
          <el-descriptions-item :label="t('wms.stockWarning.material')">{{ detailRow.material?.materialCode }} - {{ detailRow.material?.materialName }}</el-descriptions-item>
          <el-descriptions-item :label="t('wms.stockWarning.typeLabel')">{{ warningTypeMap[detailRow.warningType] }}</el-descriptions-item>
          <el-descriptions-item :label="t('wms.stockWarning.level')">{{ levelMap[detailRow.warningLevel] }}</el-descriptions-item>
          <el-descriptions-item :label="t('wms.stockWarning.warehouse')">{{ detailRow.warehouse?.warehouseName || "-" }}</el-descriptions-item>
          <el-descriptions-item :label="t('wms.stockWarning.zone')">{{ detailRow.zone?.zoneName || "-" }}</el-descriptions-item>
          <el-descriptions-item :label="t('wms.stockWarning.location')">{{ detailRow.location?.locationCode || "-" }}</el-descriptions-item>
          <el-descriptions-item :label="t('wms.stockWarning.currentQuantity')">{{ detailRow.currentQuantity }}</el-descriptions-item>
          <el-descriptions-item :label="t('wms.stockWarning.thresholdValue')">{{ detailRow.thresholdValue }}</el-descriptions-item>
          <el-descriptions-item :label="t('wms.stockWarning.status')">
            <el-tag size="small" :type="statusTag(detailRow.warningStatus)">{{ statusMap[detailRow.warningStatus] }}</el-tag>
          </el-descriptions-item>
          <el-descriptions-item :label="t('wms.stockWarning.firstTrigger')">{{ detailRow.createTime }}</el-descriptions-item>
          <el-descriptions-item :label="t('wms.stockWarning.lastTrigger')">{{ detailRow.lastTriggerTime || detailRow.createTime }}</el-descriptions-item>
          <el-descriptions-item :label="t('wms.stockWarning.triggerCount')">{{ t('wms.stockWarning.times', { count: detailRow.triggerCount }) }}</el-descriptions-item>
          <el-descriptions-item v-if="detailRow.warningStatus === 'resolved'" :label="t('wms.stockWarning.resolvedTime')">{{ detailRow.resolvedTime }}</el-descriptions-item>
          <el-descriptions-item v-if="detailRow.warningStatus === 'resolved'" :label="t('wms.stockWarning.resolvedQuantity')">{{ detailRow.resolvedQuantity }}</el-descriptions-item>
          <el-descriptions-item v-if="detailRow.handleRemark" :label="t('wms.stockWarning.handleRemark')">{{ detailRow.handleRemark }}</el-descriptions-item>
        </el-descriptions>

        <div class="detail-section-title mb8">
          {{ t('wms.stockWarning.triggerHistory') }}
          <el-tooltip :content="t('wms.stockWarning.snapshotTip')" placement="top">
            <el-icon class="section-tip-icon"><question-filled /></el-icon>
          </el-tooltip>
        </div>
        <el-table v-loading="logLoading" :data="logList" size="small" max-height="320">
          <el-table-column :label="t('wms.common.index')" type="index" width="80" align="center" />
          <el-table-column :label="t('wms.stockWarning.triggerTime')" align="center" prop="createTime" min-width="170" />
          <el-table-column :label="t('wms.stockWarning.currentQty')" align="center" prop="currentQuantity" />
          <el-table-column :label="t('wms.stockWarning.thresholdValue')" align="center" prop="thresholdValue" />
          <el-table-column :label="t('wms.stockWarning.diffValue')" align="center" prop="diffValue" />
          <el-table-column :label="t('wms.stockWarning.messageSent')" align="center" width="90">
            <template #default="scope">
              <el-tag size="small" :type="scope.row.messageSent === 1 ? 'success' : 'info'">
                {{ scope.row.messageSent === 1 ? t('wms.stockWarning.sent') : t('wms.stockWarning.notSent') }}
              </el-tag>
            </template>
          </el-table-column>
        </el-table>
        <template #footer>
          <div class="dialog-footer">
            <el-button @click="detailOpen = false">{{ t('wms.stockWarning.close') }}</el-button>
          </div>
        </template>
      </el-dialog>

      <!-- 处理对话框 -->
      <el-dialog :title="handleForm.status === 'handled' ? t('wms.stockWarning.handleTitle') : t('wms.stockWarning.ignoreTitle')" v-model="open" width="520px" append-to-body>
        <el-form label-width="88px" class="nowrap-labels">
          <el-form-item :label="t('wms.stockWarning.handleMethod')">
            <el-radio-group v-model="handleForm.status">
              <el-radio label="handled">{{ t('wms.stockWarning.handledOption') }}</el-radio>
              <el-radio label="ignored">{{ t('wms.stockWarning.ignoredOption') }}</el-radio>
            </el-radio-group>
          </el-form-item>
          <el-form-item :label="t('wms.stockWarning.handleRemark')">
            <el-input
              v-model="handleForm.remark"
              type="textarea"
              :autosize="{ minRows: 2, maxRows: 4 }"
              maxlength="500"
              :placeholder="t('wms.stockWarning.remarkPlaceholder')"
            />
          </el-form-item>
        </el-form>
        <template #footer>
          <div class="dialog-footer">
            <el-button type="primary" @click="submitHandle">{{ t('wms.stockWarning.confirm') }}</el-button>
            <el-button @click="open = false">{{ t('wms.stockWarning.cancel') }}</el-button>
          </div>
        </template>
      </el-dialog>
    </div>
  </div>
</template>

<script setup name="WmsStockWarning">
import { ref, reactive, toRefs, computed } from "vue";
import { ElMessage, ElMessageBox } from "element-plus";
import { useI18n } from "vue-i18n";
import { listStockWarning, delStockWarning, runStockWarning, handleStockWarning, getWarningLogs } from "@/api/wms/stockWarning";
import { listWarehouse } from "@/api/wms/warehouse";
import QueryForm from "@/components/QueryForm/index.vue";
import RightToolbar from "@/components/RightToolbar/index.vue";

const { t } = useI18n();
const warningList = ref([]);
const open = ref(false);
const loading = ref(true);
const ids = ref([]);
const multiple = ref(true);
const total = ref(0);
const selectedRows = ref([]);
const warehouseOptions = ref([]);

const showSearch = ref(true);

const warningTypeMap = computed(() => ({ min_stock: t("wms.stockWarning.typeMinStock"), max_stock: t("wms.stockWarning.typeMaxStock") }));
const statusMap = computed(() => ({
  pending: t("wms.stockWarning.statusPending"),
  handled: t("wms.stockWarning.statusHandled"),
  ignored: t("wms.stockWarning.statusIgnored"),
  resolved: t("wms.stockWarning.statusResolved"),
}));
const levelMap = computed(() => ({
  normal: t("wms.stockWarning.levelNormal"),
  urgent: t("wms.stockWarning.levelUrgent"),
  critical: t("wms.stockWarning.levelCritical"),
}));
const levelTag = (l) => (l === "critical" ? "danger" : l === "urgent" ? "warning" : "info");
const statusTag = (s) => (s === "pending" ? "danger" : s === "handled" || s === "resolved" ? "success" : "info");

const detailOpen = ref(false);
const detailRow = ref(null);
const logList = ref([]);
const logLoading = ref(false);

/** 打开详情：预警周期信息 + 触发历史 */
function openDetail(row) {
  detailRow.value = row;
  detailOpen.value = true;
  logLoading.value = true;
  getWarningLogs(row.warningId).then((res) => {
    logList.value = res.data || [];
    logLoading.value = false;
  }).catch(() => {
    logLoading.value = false;
  });
}

const data = reactive({
  queryParams: {
    pageNum: 1,
    pageSize: 10,
    warningStatus: undefined,
    warehouseId: undefined,
  },
  handleForm: {
    warningId: undefined,
    status: "handled",
    remark: "",
  },
});

const { queryParams, handleForm } = toRefs(data);

const queryConfig = computed(() => [
  {
    label: t("wms.stockWarning.status"),
    prop: "warningStatus",
    type: "select",
    placeholder: t("wms.stockWarning.ph.selectStatus"),
    options: [
      { label: t("wms.stockWarning.statusPending"), value: "pending" },
      { label: t("wms.stockWarning.statusHandled"), value: "handled" },
      { label: t("wms.stockWarning.statusIgnored"), value: "ignored" },
      { label: t("wms.stockWarning.statusResolved"), value: "resolved" },
    ],
  },
  {
    label: t("wms.stockWarning.warehouse"),
    prop: "warehouseId",
    type: "select",
    placeholder: t("wms.stockWarning.ph.selectWarehouse"),
    options: warehouseOptions.value.map((w) => ({ label: w.warehouseName, value: w.warehouseId })),
  },
]);

function loadWarehouses() {
  listWarehouse({ pageNum: 1, pageSize: 9999 }).then((res) => {
    warehouseOptions.value = res.rows || [];
  });
}

/** 查询列表 */
function getList() {
  loading.value = true;
  listStockWarning(queryParams.value).then((res) => {
    warningList.value = res.rows || [];
    total.value = res.total || 0;
    loading.value = false;
  });
}

/** 标签页重新激活时由布局回调（是否刷新由页面决定） */
function tabRefresh() {
  getList();
}

function handleQuery() {
  queryParams.value.pageNum = 1;
  getList();
}

function resetQuery() {
  queryParams.value = { warningStatus: undefined, warehouseId: undefined, pageNum: 1, pageSize: 10 };
  handleQuery();
}

function handleSelectionChange(selection) {
  selectedRows.value = selection;
  ids.value = selection.map((item) => item.warningId);
  multiple.value = !selection.length;
}

/** 手动跑一遍预警引擎 */
function handleRun() {
  loading.value = true;
  runStockWarning().then((res) => {
    loading.value = false;
    if (res.code === 200) {
      ElMessage.success(res.msg || t("wms.stockWarning.runDone"));
      getList();
    } else {
      ElMessage.error(res.msg || t("wms.stockWarning.runFailed"));
    }
  }).catch(() => {
    loading.value = false;
  });
}

/** 打开处理对话框 */
function openHandle(row, status) {
  handleForm.value = { warningId: row.warningId, status, remark: "" };
  open.value = true;
}

/** 提交处理 */
function submitHandle() {
  if (handleForm.value.status === "handled" && !handleForm.value.remark) {
    ElMessage.warning(t("wms.stockWarning.handleRemarkRequired"));
    return;
  }
  handleStockWarning(handleForm.value.warningId, handleForm.value.status, handleForm.value.remark).then((res) => {
    if (res.code === 200) {
      ElMessage.success(t("wms.stockWarning.handleSuccess"));
      open.value = false;
      getList();
    } else {
      ElMessage.error(res.msg || t("wms.stockWarning.handleFailed"));
    }
  });
}

function handleDelete(row) {
  const delIds = row?.warningId ? [row.warningId] : ids.value;
  if (!delIds || delIds.length === 0) return;
  ElMessageBox.confirm(t("wms.stockWarning.deleteConfirm", { count: delIds.length }), t("wms.common.tip"), { type: "warning" })
    .then(() => delStockWarning(delIds.join(",")))
    .then((res) => {
      if (res.code === 200) {
        ElMessage.success(t("wms.stockWarning.deleteSuccess"));
        getList();
      } else {
        ElMessage.error(res.msg || t("wms.stockWarning.deleteFailed"));
      }
    })
    .catch(() => {});
}

loadWarehouses();
getList();
</script>

<style scoped>
.detail-section-title {
  margin-top: 14px;
  font-weight: 600;
  display: flex;
  align-items: center;
}
.section-tip-icon {
  margin-left: 4px;
  color: #909399;
  cursor: pointer;
}
</style>

<style>
/* 固定操作列：背景不透明 + 层级高于普通单元格（element-plus 默认 background:inherit 会透），同检验单 iqc-op-col */
.warn-op-col {
  position: sticky !important;
  right: 0 !important;
  z-index: 10 !important;
  background-color: #fff !important;
}
.el-table__body tr:hover > .warn-op-col,
.el-table__body tr.hover-row > .warn-op-col {
  background-color: #f5f7fa !important;
}
.el-table__header-wrapper th.warn-op-col {
  background-color: #f7f6f4 !important;
}
html.dark .warn-op-col {
  background-color: var(--bg-card) !important;
}
html.dark .el-table__body tr:hover > .warn-op-col,
html.dark .el-table__body tr.hover-row > .warn-op-col {
  background-color: var(--bg-hover) !important;
}
html.dark .el-table__header-wrapper th.warn-op-col {
  background-color: var(--bg-hover) !important;
}
</style>
