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
          <el-button type="primary" plain icon="Promotion" @click="handleRun" v-hasPermi="['wms:stockWarning:edit']">立即执行</el-button>
        </el-col>
        <el-col :span="1.5">
          <el-button type="danger" plain icon="Delete" :disabled="multiple || selectedRows.some((r) => r.warningStatus !== 'pending')"
            @click="handleDelete" v-hasPermi="['wms:stockWarning:remove']">删除</el-button>
        </el-col>
        <right-toolbar
          v-model:showSearch="showSearch"
          @queryTable="getList"
          hint="预警由定时任务按「基础数据 → 预警配置」自动生成，这里只负责查看和处理；点「立即执行」可以不等定时任务马上跑一遍。
未处理的预警每天重复触发时会刷新数量，但不会重复发站内信。"
        />
      </el-row>

      <!-- 列表 -->
      <el-table v-loading="loading" :data="warningList" @selection-change="handleSelectionChange">
        <el-table-column type="selection" width="55" align="center" />
        <el-table-column label="预警类型" align="center" width="110">
          <template #default="scope">
            <el-tag size="small" :type="scope.row.warningType === 'min_stock' ? 'danger' : 'warning'">
              {{ warningTypeMap[scope.row.warningType] || "-" }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column label="物料编码" align="center" min-width="130" :show-overflow-tooltip="true">
          <template #default="scope">{{ scope.row.material?.materialCode || "-" }}</template>
        </el-table-column>
        <el-table-column label="物料名称" align="center" min-width="150" :show-overflow-tooltip="true">
          <template #default="scope">{{ scope.row.material?.materialName || "-" }}</template>
        </el-table-column>
        <el-table-column label="仓库" align="center" :show-overflow-tooltip="true">
          <template #default="scope">{{ scope.row.warehouse?.warehouseName || "-" }}</template>
        </el-table-column>
        <el-table-column label="库区" align="center" :show-overflow-tooltip="true">
          <template #default="scope">{{ scope.row.zone?.zoneName || "-" }}</template>
        </el-table-column>
        <el-table-column label="库位" align="center" width="100" :show-overflow-tooltip="true">
          <template #default="scope">{{ scope.row.location?.locationCode || "-" }}</template>
        </el-table-column>
        <el-table-column label="当前数量" align="center" prop="currentQuantity" width="90" />
        <el-table-column label="阈值" align="center" prop="thresholdValue" width="90" />
        <el-table-column label="差值" align="center" width="90">
          <template #default="scope">
            <span :class="scope.row.warningType === 'min_stock' ? 'dir-out' : 'dir-in'">
              {{ scope.row.diffValue }}
            </span>
          </template>
        </el-table-column>
        <el-table-column label="级别" align="center" width="80">
          <template #default="scope">
            <el-tag size="small" :type="levelTag(scope.row.warningLevel)">
              {{ levelMap[scope.row.warningLevel] || "-" }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column label="状态" align="center" width="90">
          <template #default="scope">
            <el-tag size="small" :type="statusTag(scope.row.warningStatus)">
              {{ statusMap[scope.row.warningStatus] || "-" }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column label="触发次数" align="center" prop="triggerCount" width="80" />
        <el-table-column label="最近触发" align="center" min-width="160">
          <template #default="scope">{{ scope.row.lastTriggerTime || scope.row.createTime }}</template>
        </el-table-column>
        <el-table-column label="预警时间" align="center" prop="createTime" width="170" />
        <el-table-column label="处理说明" align="center" prop="handleRemark" min-width="140" :show-overflow-tooltip="true">
          <template #default="scope">{{ scope.row.handleRemark || "-" }}</template>
        </el-table-column>
        <el-table-column label="操作" align="center" fixed="right" width="240" class-name="warn-op-col">
          <template #default="scope">
            <el-button link type="primary" icon="View" @click="openDetail(scope.row)">详情</el-button>
            <template v-if="scope.row.warningStatus === 'pending'">
              <el-button link type="success" icon="Promotion" @click="openHandle(scope.row, 'handled')" v-hasPermi="['wms:stockWarning:edit']">处理</el-button>
              <el-button link type="info" icon="Remove" @click="openHandle(scope.row, 'ignored')" v-hasPermi="['wms:stockWarning:edit']">忽略</el-button>
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
      <el-dialog title="预警详情" v-model="detailOpen" width="720px" append-to-body>
        <el-descriptions :column="3" border size="small" v-if="detailRow">
          <el-descriptions-item label="物料">{{ detailRow.material?.materialCode }} - {{ detailRow.material?.materialName }}</el-descriptions-item>
          <el-descriptions-item label="类型">{{ warningTypeMap[detailRow.warningType] }}</el-descriptions-item>
          <el-descriptions-item label="级别">{{ levelMap[detailRow.warningLevel] }}</el-descriptions-item>
          <el-descriptions-item label="仓库">{{ detailRow.warehouse?.warehouseName || "-" }}</el-descriptions-item>
          <el-descriptions-item label="库区">{{ detailRow.zone?.zoneName || "-" }}</el-descriptions-item>
          <el-descriptions-item label="库位">{{ detailRow.location?.locationCode || "-" }}</el-descriptions-item>
          <el-descriptions-item label="当前数量">{{ detailRow.currentQuantity }}</el-descriptions-item>
          <el-descriptions-item label="阈值">{{ detailRow.thresholdValue }}</el-descriptions-item>
          <el-descriptions-item label="状态">
            <el-tag size="small" :type="statusTag(detailRow.warningStatus)">{{ statusMap[detailRow.warningStatus] }}</el-tag>
          </el-descriptions-item>
          <el-descriptions-item label="首次触发">{{ detailRow.createTime }}</el-descriptions-item>
          <el-descriptions-item label="最近触发">{{ detailRow.lastTriggerTime || detailRow.createTime }}</el-descriptions-item>
          <el-descriptions-item label="触发次数">{{ detailRow.triggerCount }} 次</el-descriptions-item>
          <el-descriptions-item v-if="detailRow.warningStatus === 'resolved'" label="恢复时间">{{ detailRow.resolvedTime }}</el-descriptions-item>
          <el-descriptions-item v-if="detailRow.warningStatus === 'resolved'" label="恢复时库存">{{ detailRow.resolvedQuantity }}</el-descriptions-item>
          <el-descriptions-item v-if="detailRow.handleRemark" label="处理说明">{{ detailRow.handleRemark }}</el-descriptions-item>
        </el-descriptions>

        <div class="detail-section-title mb8">
          触发历史
          <el-tooltip content="每次触发一条快照" placement="top">
            <el-icon class="section-tip-icon"><question-filled /></el-icon>
          </el-tooltip>
        </div>
        <el-table v-loading="logLoading" :data="logList" size="small" max-height="320">
          <el-table-column label="序号" type="index" width="80" align="center" />
          <el-table-column label="触发时间" align="center" prop="createTime" min-width="170" />
          <el-table-column label="当时库存" align="center" prop="currentQuantity" />
          <el-table-column label="阈值" align="center" prop="thresholdValue" />
          <el-table-column label="差值" align="center" prop="diffValue" />
          <el-table-column label="站内信" align="center" width="90">
            <template #default="scope">
              <el-tag size="small" :type="scope.row.messageSent === 1 ? 'success' : 'info'">
                {{ scope.row.messageSent === 1 ? "已发送" : "未发送" }}
              </el-tag>
            </template>
          </el-table-column>
        </el-table>
        <template #footer>
          <div class="dialog-footer">
            <el-button @click="detailOpen = false">关 闭</el-button>
          </div>
        </template>
      </el-dialog>

      <!-- 处理对话框 -->
      <el-dialog :title="handleForm.status === 'handled' ? '处理预警' : '忽略预警'" v-model="open" width="520px" append-to-body>
        <el-form label-width="88px" class="nowrap-labels">
          <el-form-item label="处理方式">
            <el-radio-group v-model="handleForm.status">
              <el-radio label="handled">已处理</el-radio>
              <el-radio label="ignored">忽略</el-radio>
            </el-radio-group>
          </el-form-item>
          <el-form-item label="处理说明">
            <el-input
              v-model="handleForm.remark"
              type="textarea"
              :autosize="{ minRows: 2, maxRows: 4 }"
              maxlength="500"
              placeholder="例如：已补货 50 个 / 滞销已调拨走 / 暂不处理"
            />
          </el-form-item>
        </el-form>
        <template #footer>
          <div class="dialog-footer">
            <el-button type="primary" @click="submitHandle">确 定</el-button>
            <el-button @click="open = false">取 消</el-button>
          </div>
        </template>
      </el-dialog>
    </div>
  </div>
</template>

<script setup name="WmsStockWarning">
import { ref, reactive, toRefs, computed } from "vue";
import { ElMessage, ElMessageBox } from "element-plus";
import { listStockWarning, delStockWarning, runStockWarning, handleStockWarning, getWarningLogs } from "@/api/wms/stockWarning";
import { listWarehouse } from "@/api/wms/warehouse";
import QueryForm from "@/components/QueryForm/index.vue";
import RightToolbar from "@/components/RightToolbar/index.vue";

const warningList = ref([]);
const open = ref(false);
const loading = ref(true);
const ids = ref([]);
const multiple = ref(true);
const total = ref(0);
const selectedRows = ref([]);
const warehouseOptions = ref([]);

const showSearch = ref(true);

const warningTypeMap = { min_stock: "低库存", max_stock: "高库存" };
const statusMap = { pending: "待处理", handled: "已处理", ignored: "已忽略", resolved: "已恢复" };
const levelMap = { normal: "一般", urgent: "紧急", critical: "危机" };
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
    label: "状态",
    prop: "warningStatus",
    type: "select",
    placeholder: "请选择状态",
    options: [
      { label: "待处理", value: "pending" },
      { label: "已处理", value: "handled" },
      { label: "已忽略", value: "ignored" },
      { label: "已恢复", value: "resolved" },
    ],
  },
  {
    label: "仓库",
    prop: "warehouseId",
    type: "select",
    placeholder: "请选择仓库",
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
      ElMessage.success(res.msg || "执行完成");
      getList();
    } else {
      ElMessage.error(res.msg || "执行失败");
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
    ElMessage.warning("处理时请填写处理说明");
    return;
  }
  handleStockWarning(handleForm.value.warningId, handleForm.value.status, handleForm.value.remark).then((res) => {
    if (res.code === 200) {
      ElMessage.success("处理成功");
      open.value = false;
      getList();
    } else {
      ElMessage.error(res.msg || "处理失败");
    }
  });
}

function handleDelete(row) {
  const delIds = row?.warningId ? [row.warningId] : ids.value;
  if (!delIds || delIds.length === 0) return;
  ElMessageBox.confirm(`确认删除选中的 ${delIds.length} 条预警记录吗？`, "提示", { type: "warning" })
    .then(() => delStockWarning(delIds.join(",")))
    .then((res) => {
      if (res.code === 200) {
        ElMessage.success("删除成功");
        getList();
      } else {
        ElMessage.error(res.msg || "删除失败");
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
