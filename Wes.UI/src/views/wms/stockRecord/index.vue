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

      <el-row :gutter="10" class="mb8">
        <right-toolbar
          v-model:showSearch="showSearch"
          :hint="t('wms.stockRecord.hint')"
          @queryTable="getList"
        ></right-toolbar>
      </el-row>

      <el-table v-loading="loading" :data="stockRecordList">
        <el-table-column :label="t('wms.stockRecord.recordNo')" align="center" prop="recordNo" width="170" :show-overflow-tooltip="true" />
        <el-table-column :label="t('wms.stockRecord.bizType')" align="center" width="100">
          <template #default="scope">
            <el-tag size="small" :type="bizTagType(scope.row.bizType)">{{ bizTypeMap[scope.row.bizType] || scope.row.bizType || "-" }}</el-tag>
          </template>
        </el-table-column>
        <el-table-column :label="t('wms.stockRecord.direction')" align="center" width="80">
          <template #default="scope">
            <span :class="scope.row.direction === 'out' ? 'dir-out' : 'dir-in'">
              {{ scope.row.direction === "out" ? t("wms.stockRecord.out") : t("wms.stockRecord.in") }}
            </span>
          </template>
        </el-table-column>
        <el-table-column :label="t('wms.stockRecord.sourceNo')" align="center" prop="sourceNo" width="170" :show-overflow-tooltip="true">
          <template #default="scope">{{ scope.row.sourceNo || "-" }}</template>
        </el-table-column>
        <el-table-column :label="t('wms.stock.materialCode')" align="center" :show-overflow-tooltip="true">
          <template #default="scope">{{ scope.row.material?.materialCode || "-" }}</template>
        </el-table-column>
        <el-table-column :label="t('wms.stock.materialName')" align="center" :show-overflow-tooltip="true">
          <template #default="scope">{{ scope.row.material?.materialName || "-" }}</template>
        </el-table-column>
        <el-table-column :label="t('wms.common.batchNo')" align="center" prop="batchNo" :show-overflow-tooltip="true">
          <template #default="scope">{{ scope.row.batchNo || "-" }}</template>
        </el-table-column>
        <el-table-column :label="t('wms.common.warehouse')" align="center" :show-overflow-tooltip="true">
          <template #default="scope">{{ scope.row.warehouse?.warehouseName || "-" }}</template>
        </el-table-column>
        <el-table-column :label="t('wms.common.zone')" align="center" :show-overflow-tooltip="true">
          <template #default="scope">{{ scope.row.zone?.zoneName || "-" }}</template>
        </el-table-column>
        <el-table-column :label="t('wms.common.location')" align="center" :show-overflow-tooltip="true">
          <template #default="scope">{{ scope.row.location?.locationCode || scope.row.location?.locationName || "-" }}</template>
        </el-table-column>
        <el-table-column :label="t('wms.stockRecord.before')" align="center" prop="beforeQuantity" width="90" />
        <el-table-column :label="t('wms.stockRecord.changeQuantity')" align="center" prop="changeQuantity" width="100">
          <template #default="scope">
            <span :class="scope.row.direction === 'out' ? 'dir-out' : 'dir-in'">
              {{ scope.row.direction === "out" ? "-" : "+" }}{{ scope.row.changeQuantity ?? 0 }}
            </span>
          </template>
        </el-table-column>
        <el-table-column :label="t('wms.stockRecord.after')" align="center" prop="afterQuantity" width="90" />
        <el-table-column :label="t('wms.stockRecord.operator')" align="center" prop="operatorName" width="110">
          <template #default="scope">{{ scope.row.operatorName || scope.row.createBy || "-" }}</template>
        </el-table-column>
        <el-table-column :label="t('wms.stockRecord.operateTime')" align="center" prop="operateTime" width="170">
          <template #default="scope">{{ scope.row.operateTime || scope.row.createTime || "-" }}</template>
        </el-table-column>
        <el-table-column :label="t('common.remark')" align="center" prop="remark" :show-overflow-tooltip="true" />
      </el-table>

      <pagination
        v-show="total > 0"
        :total="total"
        v-model:page="queryParams.pageNum"
        v-model:limit="queryParams.pageSize"
        @pagination="getList"
      />
    </div>
  </div>
</template>

<script setup name="WmsStockRecord">
import { ref, reactive, toRefs, computed } from "vue";
import { useI18n } from "vue-i18n";
import { listStockRecord } from "@/api/wms/stockRecord";
import QueryForm from "@/components/QueryForm/index.vue";
import RightToolbar from "@/components/RightToolbar/index.vue";

const { t } = useI18n();
const stockRecordList = ref([]);
const loading = ref(true);
const total = ref(0);
const showSearch = ref(true);

const data = reactive({
  queryParams: {
    pageNum: 1,
    pageSize: 10,
    keywords: undefined,
  },
});

const { queryParams } = toRefs(data);

/** 业务类型字典 */
const bizTypeMap = computed(() => ({
  receipt: t("wms.stockRecord.bizReceipt"),
  delivery: t("wms.stockRecord.bizDelivery"),
  move: t("wms.stockRecord.bizMove"),
  check: t("wms.stockRecord.bizCheck"),
  adjust: t("wms.stockRecord.bizAdjust"),
  freeze: t("wms.stockRecord.bizFreeze"),
  thaw: t("wms.stockRecord.bizThaw"),
  lock: t("wms.stockRecord.bizLock"),
  unlock: t("wms.stockRecord.bizUnlock"),
}));

function bizTagType(bizType) {
  if (bizType === "receipt") return "success";
  if (bizType === "delivery") return "danger";
  if (bizType === "adjust") return "warning";
  return "info";
}

const queryConfig = computed(() => [
  {
    label: t("wms.common.keywords"),
    prop: "keywords",
    type: "input",
    placeholder: t("wms.stockRecord.ph.keywords"),
  },
]);

/** 查询列表 */
function getList() {
  loading.value = true;
  listStockRecord(queryParams.value).then((res) => {
    stockRecordList.value = res.rows || [];
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
    pageNum: 1,
    pageSize: 10,
  };
  handleQuery();
}

getList();
</script>

<style scoped>
.dir-in {
  color: #67c23a;
}
.dir-out {
  color: #f56c6c;
}
</style>
