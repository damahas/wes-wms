<template>
  <div class="app-container">
    <div class="main-panel p16">
      <query-form :config="queryConfig" v-model:visible="showSearch" v-model="queryParams" @query="handleQuery"
        @reset="resetQuery" />
      <el-row :gutter="10" class="mb8">
        <right-toolbar v-model:showSearch="showSearch" @queryTable="getList" />
      </el-row>

      <el-table v-loading="loading" :data="dataList">
        <el-table-column label="检验单号" align="center" min-width="180" show-overflow-tooltip prop="inspectionNo" />
        <el-table-column label="收货单号" align="center" min-width="150" show-overflow-tooltip>
          <template #default="scope">{{ scope.row.receiving?.receivingNo || "-" }}</template>
        </el-table-column>
        <el-table-column label="供应商" align="center" min-width="160" show-overflow-tooltip>
          <template #default="scope">{{ scope.row.supplier?.partnerName || "-" }}</template>
        </el-table-column>
        <el-table-column label="物料" align="center" min-width="180" show-overflow-tooltip>
          <template #default="scope">
            {{ scope.row.material?.materialCode
              ? scope.row.material.materialCode + " - " + scope.row.material.materialName
              : (scope.row.material?.materialName || "-") }}
          </template>
        </el-table-column>
        <el-table-column label="规格" align="center" prop="spec" width="110" show-overflow-tooltip />
        <el-table-column label="批次号" align="center" prop="batchNo" width="120" show-overflow-tooltip />
        <el-table-column label="收货数量" align="center" prop="deliveryQuantity" width="100" />
        <el-table-column label="允许入库数" align="center" prop="allowQuantity" width="100" />
        <el-table-column label="不良数量" align="center" width="100">
          <template #default="scope">
            <span class="defect-qty">{{ scope.row.defectQuantity || 0 }}</span>
          </template>
        </el-table-column>
        <el-table-column label="退货数量" align="center" prop="returnQuantity" width="100" />
        <el-table-column label="处置方式" align="center" width="110">
          <template #default="scope">
            <el-tag :type="disposeTagType(scope.row.disposeType)">{{ disposeLabel(scope.row.disposeType) }}</el-tag>
          </template>
        </el-table-column>
        <el-table-column label="检验人" align="center" prop="inspector" width="100" />
        <el-table-column label="检验日期" align="center" width="120">
          <template #default="scope">{{ scope.row.inspectDate ? scope.row.inspectDate.slice(0, 10) : "-" }}</template>
        </el-table-column>
        <el-table-column label="操作" align="center" width="120" fixed="right">
          <template #default="scope">
            <el-button link type="primary" icon="View" @click="handleViewItems(scope.row)">不良项</el-button>
          </template>
        </el-table-column>
      </el-table>

      <pagination v-show="total > 0" :total="total" v-model:page="queryParams.pageNum"
        v-model:limit="queryParams.pageSize" @pagination="getList" />

      <!-- 不良检验项明细 -->
      <el-dialog title="不良检验项" v-model="itemOpen" width="60%" append-to-body>
        <el-table v-loading="itemLoading" :data="ngItems" border max-height="400">
          <el-table-column label="检验项编码" prop="itemCode" width="130" />
          <el-table-column label="检验项名称" prop="itemName" min-width="140" />
          <el-table-column label="检验类型" prop="inspectionType" width="100" />
          <el-table-column label="标准值" prop="standardValue" min-width="120" />
          <el-table-column label="检验结果" prop="inspectResult" min-width="120" />
          <el-table-column label="关键项" width="90" align="center">
            <template #default="scope">{{ scope.row.isCritical === 1 ? "是" : "否" }}</template>
          </el-table-column>
          <el-table-column label="检测方法" prop="testMethod" min-width="140" show-overflow-tooltip />
        </el-table>
        <template #footer>
          <el-button @click="itemOpen = false">关 闭</el-button>
        </template>
      </el-dialog>
    </div>
  </div>
</template>

<script setup name="WmsIqcDefect">
import { ref, reactive } from "vue";
import { listIqcDefect, getIqcNgItems } from "@/api/wms/iqcDefect";
import QueryForm from "@/components/QueryForm/index.vue";
import RightToolbar from "@/components/RightToolbar/index.vue";

const dataList = ref([]);
const loading = ref(true);
const showSearch = ref(true);
const total = ref(0);

const queryParams = reactive({
  pageNum: 1,
  pageSize: 10,
  keywords: undefined,
  disposeType: undefined,
  dateRange: undefined,
});

const queryConfig = [
  { label: "关键词", prop: "keywords", type: "input", placeholder: "检验单号/收货单号/物料" },
  {
    label: "处置方式", prop: "disposeType", type: "select", placeholder: "请选择处置方式",
    options: [
      { label: "退回供应商", value: "return" },
      { label: "让步接收", value: "concession" },
      { label: "挑选使用", value: "sorting" },
    ],
  },
  { label: "检验日期", prop: "dateRange", type: "daterange" },
];

// 不良品处置方式
const disposeMap = { none: "无需处置", return: "退回供应商", concession: "让步接收", sorting: "挑选使用" };
function disposeLabel(v) { return disposeMap[v] || "-"; }
function disposeTagType(v) {
  const m = { return: "danger", concession: "warning", sorting: "primary" };
  return m[v] || "info";
}

function getList() {
  loading.value = true;
  const [beginDate, endDate] = queryParams.dateRange || [];
  listIqcDefect({
    pageNum: queryParams.pageNum,
    pageSize: queryParams.pageSize,
    keywords: queryParams.keywords || undefined,
    disposeType: queryParams.disposeType || undefined,
    beginDate,
    endDate,
  }).then((resp) => {
    dataList.value = resp.rows || [];
    total.value = resp.total || 0;
  }).finally(() => {
    loading.value = false;
  });
}

/** 标签页重新激活时由布局回调（是否刷新由页面决定） */
function tabRefresh() {
  getList();
}

function handleQuery() { queryParams.pageNum = 1; getList(); }
function resetQuery() {
  queryParams.keywords = undefined;
  queryParams.disposeType = undefined;
  queryParams.dateRange = undefined;
  queryParams.pageNum = 1;
  getList();
}

// ============ 不良项明细 ============
const itemOpen = ref(false);
const itemLoading = ref(false);
const ngItems = ref([]);

function handleViewItems(row) {
  itemOpen.value = true;
  itemLoading.value = true;
  ngItems.value = [];
  getIqcNgItems(row.inspectionId)
    .then((res) => {
      ngItems.value = res.data || [];
    })
    .finally(() => {
      itemLoading.value = false;
    });
}

getList();
</script>

<style scoped>
.defect-qty {
  color: #f56c6c;
  font-weight: 600;
}
</style>
