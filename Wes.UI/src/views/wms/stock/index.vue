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
          :hint="t('wms.stock.hint')"
          @queryTable="getList"
        ></right-toolbar>
      </el-row>

      <el-table v-loading="loading" :data="stockList">
        <el-table-column :label="t('wms.stock.barcode')" align="center" prop="barcode" width="160" :show-overflow-tooltip="true" />
        <el-table-column :label="t('wms.stock.materialCode')" align="center" prop="materialCode" :show-overflow-tooltip="true" />
        <el-table-column :label="t('wms.stock.materialName')" align="center" prop="materialName" :show-overflow-tooltip="true" />
        <el-table-column :label="t('wms.stock.spec')" align="center" prop="spec" :show-overflow-tooltip="true" />
        <el-table-column :label="t('wms.common.batchNo')" align="center" prop="batchNo" :show-overflow-tooltip="true" />
        <el-table-column :label="t('wms.common.warehouse')" align="center" :show-overflow-tooltip="true">
          <template #default="scope">{{ scope.row.warehouseName || scope.row.warehouse?.warehouseName || "-" }}</template>
        </el-table-column>
        <el-table-column :label="t('wms.common.zone')" align="center" :show-overflow-tooltip="true">
          <template #default="scope">{{ scope.row.zoneName || scope.row.zone?.zoneName || "-" }}</template>
        </el-table-column>
        <el-table-column :label="t('wms.location.locationCode')" align="center" :show-overflow-tooltip="true">
          <template #default="scope">{{ scope.row.locationCode || scope.row.location?.locationCode || "-" }}</template>
        </el-table-column>
        <el-table-column :label="t('wms.receiving.unit')" align="center" prop="unit" :show-overflow-tooltip="true" />
        <el-table-column :label="t('wms.stock.stockQuantity')" align="center" prop="stockQuantity" />
        <el-table-column :label="t('wms.stock.availableQuantity')" align="center" prop="availableQuantity" />
        <el-table-column :label="t('wms.stock.freezeQuantity')" align="center" prop="freezeQuantity" />
        <el-table-column :label="t('wms.stock.lockQuantity')" align="center" prop="lockQuantity" />
        <el-table-column :label="t('wms.stock.productionDate')" align="center" prop="productionDate" width="120" />
        <el-table-column :label="t('wms.stock.expireDate')" align="center" prop="expireDate" width="120" />
        <el-table-column :label="t('common.createTime')" align="center" prop="createTime" min-width="170" />
        <el-table-column :label="t('wms.stock.supplier')" align="center" :show-overflow-tooltip="true">
          <template #default="scope">
            {{ scope.row.supplier?.partnerName || "-" }}
          </template>
        </el-table-column>
        <el-table-column :label="t('wms.stock.sourceReceipt')" align="center" min-width="170" :show-overflow-tooltip="true">
          <template #default="scope">
            {{ scope.row.receiptId || "-" }}
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
    </div>
  </div>
</template>

<script setup name="WmsStock">
import { ref, reactive, toRefs, computed } from "vue";
import { useI18n } from "vue-i18n";
import { listStock } from "@/api/wms/stock";
import QueryForm from "@/components/QueryForm/index.vue";
import RightToolbar from "@/components/RightToolbar/index.vue";

const { t } = useI18n();
const stockList = ref([]);
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

const queryConfig = computed(() => [
  {
    label: t("wms.common.keywords"),
    prop: "keywords",
    type: "input",
    placeholder: t("wms.common.ph.inputKeywords"),
  },
]);

/** 查询列表 */
function getList() {
  loading.value = true;
  listStock(queryParams.value).then((res) => {
    stockList.value = res.rows || [];
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
