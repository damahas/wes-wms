<template>
  <div class="app-container">
    <div class="main-panel p16">
      <query-form :config="queryConfig" v-model:visible="showSearch" v-model="queryParams" @query="handleQuery"
        @reset="resetQuery" />
      <el-row :gutter="10" class="mb8">
        <right-toolbar v-model:showSearch="showSearch" @queryTable="getList" />
      </el-row>

      <el-table v-loading="loading" :data="dataList">
        <el-table-column :label="t('wms.iqcOrder.inspectionNo')" align="center" min-width="180" show-overflow-tooltip prop="inspectionNo" />
        <el-table-column :label="t('wms.receiving.receivingNo')" align="center" min-width="150" show-overflow-tooltip>
          <template #default="scope">{{ scope.row.receiving?.receivingNo || "-" }}</template>
        </el-table-column>
        <el-table-column :label="t('wms.material.supplier')" align="center" min-width="160" show-overflow-tooltip>
          <template #default="scope">{{ scope.row.supplier?.partnerName || "-" }}</template>
        </el-table-column>
        <el-table-column :label="t('wms.material.title')" align="center" min-width="180" show-overflow-tooltip>
          <template #default="scope">
            {{ scope.row.material?.materialCode
              ? scope.row.material.materialCode + " - " + scope.row.material.materialName
              : (scope.row.material?.materialName || "-") }}
          </template>
        </el-table-column>
        <el-table-column :label="t('wms.material.spec')" align="center" prop="spec" width="110" show-overflow-tooltip />
        <el-table-column :label="t('wms.receiving.batchNo')" align="center" prop="batchNo" width="120" show-overflow-tooltip />
        <el-table-column :label="t('wms.receiving.planQuantity')" align="center" prop="deliveryQuantity" width="100" />
        <el-table-column :label="t('wms.iqcDefect.allowQuantity')" align="center" prop="allowQuantity" width="100" />
        <el-table-column :label="t('wms.iqcDefect.defectQuantity')" align="center" width="100">
          <template #default="scope">
            <span class="defect-qty">{{ scope.row.defectQuantity || 0 }}</span>
          </template>
        </el-table-column>
        <el-table-column :label="t('wms.receiving.returnQty')" align="center" prop="returnQuantity" width="100" />
        <el-table-column :label="t('wms.iqcDefect.disposeType')" align="center" width="110">
          <template #default="scope">
            <el-tag :type="disposeTagType(scope.row.disposeType)">{{ disposeLabel(scope.row.disposeType) }}</el-tag>
          </template>
        </el-table-column>
        <el-table-column :label="t('wms.iqcDefect.inspector')" align="center" prop="inspector" width="100" />
        <el-table-column :label="t('wms.iqcDefect.inspectDate')" align="center" width="120">
          <template #default="scope">{{ scope.row.inspectDate ? scope.row.inspectDate.slice(0, 10) : "-" }}</template>
        </el-table-column>
        <el-table-column :label="t('common.actions')" align="center" width="120" fixed="right">
          <template #default="scope">
            <el-button link type="primary" icon="View" @click="handleViewItems(scope.row)">{{ t('wms.iqcDefect.ngItems') }}</el-button>
          </template>
        </el-table-column>
      </el-table>

      <pagination v-show="total > 0" :total="total" v-model:page="queryParams.pageNum"
        v-model:limit="queryParams.pageSize" @pagination="getList" />

      <!-- 不良检验项明细 -->
      <el-dialog :title="t('wms.iqcDefect.ngItemsTitle')" v-model="itemOpen" width="60%" append-to-body>
        <el-table v-loading="itemLoading" :data="ngItems" border max-height="400">
          <el-table-column :label="t('wms.iqcItem.itemCode')" prop="itemCode" width="130" />
          <el-table-column :label="t('wms.iqcItem.itemName')" prop="itemName" min-width="140" />
          <el-table-column :label="t('wms.iqcItem.itemType')" prop="inspectionType" width="100" />
          <el-table-column :label="t('wms.iqcDefect.standardValue')" prop="standardValue" min-width="120" />
          <el-table-column :label="t('wms.iqcDefect.inspectResult')" prop="inspectResult" min-width="120" />
          <el-table-column :label="t('wms.iqcItem.isCritical')" width="90" align="center">
            <template #default="scope">{{ scope.row.isCritical === 1 ? t('wms.common.yes') : t('wms.common.no') }}</template>
          </el-table-column>
          <el-table-column :label="t('wms.iqcDefect.testMethod')" prop="testMethod" min-width="140" show-overflow-tooltip />
        </el-table>
        <template #footer>
          <el-button @click="itemOpen = false">{{ t('common.close') }}</el-button>
        </template>
      </el-dialog>
    </div>
  </div>
</template>

<script setup name="WmsIqcDefect">
import { ref, reactive, computed } from "vue";
import { useI18n } from "vue-i18n";
import { listIqcDefect, getIqcNgItems } from "@/api/wms/iqcDefect";
import QueryForm from "@/components/QueryForm/index.vue";
import RightToolbar from "@/components/RightToolbar/index.vue";

const { t } = useI18n();

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

const queryConfig = computed(() => [
  { label: t('wms.common.keywords'), prop: "keywords", type: "input", placeholder: t('wms.iqcDefect.ph.keywords') },
  {
    label: t('wms.iqcDefect.disposeType'), prop: "disposeType", type: "select", placeholder: t('wms.iqcDefect.ph.selectDispose'),
    options: [
      { label: t('wms.iqcDefect.disposeReturn'), value: "return" },
      { label: t('wms.iqcDefect.disposeConcession'), value: "concession" },
      { label: t('wms.iqcDefect.disposeSorting'), value: "sorting" },
    ],
  },
  { label: t('wms.iqcDefect.inspectDate'), prop: "dateRange", type: "daterange" },
]);

// 不良品处置方式
function disposeLabel(v) {
  const map = {
    none: t('wms.iqcDefect.disposeNone'),
    return: t('wms.iqcDefect.disposeReturn'),
    concession: t('wms.iqcDefect.disposeConcession'),
    sorting: t('wms.iqcDefect.disposeSorting'),
  };
  return map[v] || "-";
}
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
