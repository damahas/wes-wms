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
          hint="实时库存由入库上架、出库、移库、盘点、库存调整等业务单据驱动，不能直接修改或删除。
需要修正库存数量请使用「库存调整」。"
          @queryTable="getList"
        ></right-toolbar>
      </el-row>

      <el-table v-loading="loading" :data="stockList">
        <el-table-column label="库存条码" align="center" prop="barcode" width="160" :show-overflow-tooltip="true" />
        <el-table-column label="物料编码" align="center" prop="materialCode" :show-overflow-tooltip="true" />
        <el-table-column label="物料名称" align="center" prop="materialName" :show-overflow-tooltip="true" />
        <el-table-column label="规格" align="center" prop="spec" :show-overflow-tooltip="true" />
        <el-table-column label="批次号" align="center" prop="batchNo" :show-overflow-tooltip="true" />
        <el-table-column label="仓库" align="center" :show-overflow-tooltip="true">
          <template #default="scope">{{ scope.row.warehouseName || scope.row.warehouse?.warehouseName || "-" }}</template>
        </el-table-column>
        <el-table-column label="库区" align="center" :show-overflow-tooltip="true">
          <template #default="scope">{{ scope.row.zoneName || scope.row.zone?.zoneName || "-" }}</template>
        </el-table-column>
        <el-table-column label="库位编码" align="center" :show-overflow-tooltip="true">
          <template #default="scope">{{ scope.row.locationCode || scope.row.location?.locationCode || "-" }}</template>
        </el-table-column>
        <el-table-column label="单位" align="center" prop="unit" :show-overflow-tooltip="true" />
        <el-table-column label="库存数量" align="center" prop="stockQuantity" />
        <el-table-column label="可用数量" align="center" prop="availableQuantity" />
        <el-table-column label="冻结数量" align="center" prop="freezeQuantity" />
        <el-table-column label="锁定数量" align="center" prop="lockQuantity" />
        <el-table-column label="生产日期" align="center" prop="productionDate" width="120" />
        <el-table-column label="失效日期" align="center" prop="expireDate" width="120" />
        <el-table-column label="创建时间" align="center" prop="createTime" min-width="170" />
        <el-table-column label="供应商" align="center" :show-overflow-tooltip="true">
          <template #default="scope">
            {{ scope.row.supplier?.partnerName || "-" }}
          </template>
        </el-table-column>
        <el-table-column label="来源入库单" align="center" min-width="170" :show-overflow-tooltip="true">
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
import { listStock } from "@/api/wms/stock";
import QueryForm from "@/components/QueryForm/index.vue";
import RightToolbar from "@/components/RightToolbar/index.vue";

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
    label: "关键字",
    prop: "keywords",
    type: "input",
    placeholder: "请输入关键字",
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
