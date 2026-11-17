<template>
  <div class="app-container">
    <div class="main-panel p16">
      <query-form :config="queryConfig" v-model:visible="showSearch" v-model="queryParams" @query="handleQuery" @reset="resetQuery" />

      <el-row :gutter="10" class="mb8">
        <el-button-group>
          <el-button :type="showGapOnly ? 'primary' : 'default'" plain @click="toggleGapOnly">
            {{ showGapOnly ? "显示全部缺料" : "只看真实缺口" }}
          </el-button>
        </el-button-group>
        <span class="text-muted ml8">
          真实缺口 = 欠料 − 可用 − 已预留；可用指扣除冻结与锁定后的库存
        </span>
      </el-row>

      <!-- 汇总卡 -->
      <el-row :gutter="12" class="mb16">
        <el-col :span="4">
          <div class="stat-card">
            <div class="stat-value">{{ summary.issueCount || 0 }}</div>
            <div class="stat-label">在等料的领料单</div>
          </div>
        </el-col>
        <el-col :span="4">
          <div class="stat-card">
            <div class="stat-value">{{ summary.workOrderCount || 0 }}</div>
            <div class="stat-label">涉及生产工单</div>
          </div>
        </el-col>
        <el-col :span="4">
          <div class="stat-card">
            <div class="stat-value">{{ summary.shortageQuantity || 0 }}</div>
            <div class="stat-label">欠料总数</div>
          </div>
        </el-col>
        <el-col :span="4">
          <div class="stat-card danger">
            <div class="stat-value">{{ summary.gapQuantity || 0 }}</div>
            <div class="stat-label">真实缺口</div>
          </div>
        </el-col>
        <el-col :span="4">
          <div class="stat-card warning">
            <div class="stat-value">{{ summary.overdueCount || 0 }}</div>
            <div class="stat-label">已超期</div>
          </div>
        </el-col>
      </el-row>

      <el-table v-loading="loading" :data="shortageList" border>
        <el-table-column label="生产工单号" align="center" prop="workOrderNo" width="130" :show-overflow-tooltip="true">
          <template #default="scope">{{ scope.row.workOrderNo || "-" }}</template>
        </el-table-column>
        <el-table-column label="领料单号" align="center" prop="issueNo" width="140" :show-overflow-tooltip="true" />
        <el-table-column label="领用部门" align="center" prop="deptName" width="110" :show-overflow-tooltip="true">
          <template #default="scope">{{ scope.row.deptName || "-" }}</template>
        </el-table-column>
        <el-table-column label="物料" align="center" min-width="170" :show-overflow-tooltip="true">
          <template #default="scope">
            {{ scope.row.materialCode }}
            <span v-if="scope.row.materialName"> - {{ scope.row.materialName }}</span>
          </template>
        </el-table-column>
        <el-table-column label="需求" align="center" prop="requireQuantity" width="70" />
        <el-table-column label="已发" align="center" prop="issuedQuantity" width="70" />
        <el-table-column label="欠料" align="center" width="80">
          <template #default="scope">
            <span :class="{ 'text-danger': scope.row.shortageQuantity > 0 }">{{ scope.row.shortageQuantity }}</span>
          </template>
        </el-table-column>
        <el-table-column label="可用库存" align="center" prop="availableQuantity" width="90">
          <template #default="scope">
            <span :class="{ 'text-success': scope.row.availableQuantity > 0 }">{{ scope.row.availableQuantity }}</span>
          </template>
        </el-table-column>
        <el-table-column label="已预留" align="center" prop="lockedQuantity" width="80">
          <template #default="scope">
            <span :class="{ 'text-success': scope.row.lockedQuantity > 0 }">{{ scope.row.lockedQuantity }}</span>
          </template>
        </el-table-column>
        <el-table-column label="待检" align="center" prop="inspectingQuantity" width="70">
          <template #default="scope">
            {{ scope.row.inspectingQuantity || 0 }}
          </template>
        </el-table-column>
        <el-table-column label="真实缺口" align="center" width="100">
          <template #default="scope">
            <el-tag :type="scope.row.gapQuantity > 0 ? 'danger' : 'success'" size="small">
              {{ scope.row.gapQuantity }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column label="需求日期" align="center" width="110">
          <template #default="scope">{{ (scope.row.requireDate || "").slice(0, 10) || "-" }}</template>
        </el-table-column>
        <el-table-column label="等待" align="center" width="90">
          <template #default="scope">
            <span v-if="scope.row.daysToRequire === null || scope.row.daysToRequire === undefined">-</span>
            <span v-else-if="scope.row.daysToRequire < 0" class="text-danger">
              超期 {{ Math.abs(scope.row.daysToRequire) }} 天
            </span>
            <span v-else>{{ scope.row.daysToRequire }} 天</span>
          </template>
        </el-table-column>
        <el-table-column label="操作" align="center" width="120" class-name="small-padding fixed-width">
          <template #default="scope">
            <el-button link type="success" :disabled="scope.row.availableQuantity <= 0" @click="handleIssue(scope.row)">
              去发料
            </el-button>
          </template>
        </el-table-column>
      </el-table>

      <pagination v-show="total > 0" :total="total" v-model:page="queryParams.pageNum" v-model:limit="queryParams.pageSize" @pagination="getList" />
    </div>
  </div>
</template>

<script setup name="WmsShortage">
import { ref, reactive, toRefs, computed } from "vue";
import { ElMessage, ElMessageBox } from "element-plus";
import { listShortage, summaryShortage } from "@/api/wms/shortage";
import { issueIssueOrder } from "@/api/wms/issueOrder";
import QueryForm from "@/components/QueryForm/index.vue";

const shortageList = ref([]);
const loading = ref(false);
const total = ref(0);
const summary = ref({});
const showSearch = ref(true);
/** 只显示真实缺口 > 0 的行 */
const showGapOnly = ref(false);

const data = reactive({
  queryParams: {
    pageNum: 1,
    pageSize: 10,
    keywords: undefined,
    warehouseId: undefined,
    workOrderNo: undefined,
    onlyGap: undefined,
  },
});

const { queryParams } = toRefs(data);

const queryConfig = computed(() => [
  { label: "关键字", prop: "keywords", type: "input", placeholder: "物料编码/名称" },
  { label: "生产工单号", prop: "workOrderNo", type: "input", placeholder: "请输入工单号" },
]);

/** 查询列表 */
function getList() {
  loading.value = true;
  queryParams.value.onlyGap = showGapOnly.value || undefined;
  listShortage(queryParams.value).then((res) => {
    shortageList.value = res.rows || [];
    total.value = res.total || 0;
    loading.value = false;
  });
  loadSummary();
}

/** 标签页重新激活时由布局回调（是否刷新由页面决定） */
function tabRefresh() {
  getList();
}

function loadSummary() {
  summaryShortage({ workOrderNo: queryParams.value.workOrderNo }).then((res) => {
    summary.value = res.data || {};
  });
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
    workOrderNo: undefined,
    onlyGap: undefined,
  };
  handleQuery();
}

/** 切换"只看真实缺口" */
function toggleGapOnly() {
  showGapOnly.value = !showGapOnly.value;
  getList();
}

/** 直接对该领料单发料 */
function handleIssue(row) {
  ElMessageBox.confirm(
    `对领料单「${row.issueNo}」发料：按先过期先出挑批次，库存有多少发多少。是否继续？`,
    "发料确认",
    { confirmButtonText: "确定发料", cancelButtonText: "取消", type: "warning" }
  )
    .then(() => issueIssueOrder(row.issueId))
    .then((res) => {
      ElMessage.success(res.msg || "发料成功");
      getList();
    });
}

getList();
</script>

<style scoped>
.stat-card {
  padding: 14px 16px;
  border: 1px solid #ebeef5;
  border-radius: 4px;
  background: #fff;
}
.stat-card.danger {
  border-color: var(--el-color-danger-light-5);
  background: var(--el-color-danger-light-9);
}
.stat-card.warning {
  border-color: var(--el-color-warning-light-5);
  background: var(--el-color-warning-light-9);
}
.stat-value {
  font-size: 22px;
  font-weight: 600;
}
.stat-label {
  margin-top: 4px;
  font-size: 12px;
  color: #909399;
}
.mb16 {
  margin-bottom: 16px;
}
.ml8 {
  margin-left: 8px;
}
.text-muted {
  color: #909399;
  font-size: 12px;
}
.text-danger {
  color: var(--el-color-danger);
}
.text-success {
  color: var(--el-color-success);
}
</style>
