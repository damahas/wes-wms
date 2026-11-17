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
          hint="库存流水为系统审计记录，由入库、出库、移库、库存调整等业务自动生成。
不能手工新增、修改或删除。"
          @queryTable="getList"
        ></right-toolbar>
      </el-row>

      <el-table v-loading="loading" :data="stockRecordList">
        <el-table-column label="流水号" align="center" prop="recordNo" width="170" :show-overflow-tooltip="true" />
        <el-table-column label="业务类型" align="center" width="100">
          <template #default="scope">
            <el-tag size="small" :type="bizTagType(scope.row.bizType)">{{ bizTypeMap[scope.row.bizType] || scope.row.bizType || "-" }}</el-tag>
          </template>
        </el-table-column>
        <el-table-column label="方向" align="center" width="80">
          <template #default="scope">
            <span :class="scope.row.direction === 'out' ? 'dir-out' : 'dir-in'">
              {{ scope.row.direction === "out" ? "出" : "入" }}
            </span>
          </template>
        </el-table-column>
        <el-table-column label="来源单号" align="center" prop="sourceNo" width="170" :show-overflow-tooltip="true">
          <template #default="scope">{{ scope.row.sourceNo || "-" }}</template>
        </el-table-column>
        <el-table-column label="物料编码" align="center" :show-overflow-tooltip="true">
          <template #default="scope">{{ scope.row.material?.materialCode || "-" }}</template>
        </el-table-column>
        <el-table-column label="物料名称" align="center" :show-overflow-tooltip="true">
          <template #default="scope">{{ scope.row.material?.materialName || "-" }}</template>
        </el-table-column>
        <el-table-column label="批次号" align="center" prop="batchNo" :show-overflow-tooltip="true">
          <template #default="scope">{{ scope.row.batchNo || "-" }}</template>
        </el-table-column>
        <el-table-column label="仓库" align="center" :show-overflow-tooltip="true">
          <template #default="scope">{{ scope.row.warehouse?.warehouseName || "-" }}</template>
        </el-table-column>
        <el-table-column label="库区" align="center" :show-overflow-tooltip="true">
          <template #default="scope">{{ scope.row.zone?.zoneName || "-" }}</template>
        </el-table-column>
        <el-table-column label="库位" align="center" :show-overflow-tooltip="true">
          <template #default="scope">{{ scope.row.location?.locationCode || scope.row.location?.locationName || "-" }}</template>
        </el-table-column>
        <el-table-column label="变动前" align="center" prop="beforeQuantity" width="90" />
        <el-table-column label="变动数量" align="center" prop="changeQuantity" width="100">
          <template #default="scope">
            <span :class="scope.row.direction === 'out' ? 'dir-out' : 'dir-in'">
              {{ scope.row.direction === "out" ? "-" : "+" }}{{ scope.row.changeQuantity ?? 0 }}
            </span>
          </template>
        </el-table-column>
        <el-table-column label="变动后" align="center" prop="afterQuantity" width="90" />
        <el-table-column label="操作人" align="center" prop="operatorName" width="110">
          <template #default="scope">{{ scope.row.operatorName || scope.row.createBy || "-" }}</template>
        </el-table-column>
        <el-table-column label="操作时间" align="center" prop="operateTime" width="170">
          <template #default="scope">{{ scope.row.operateTime || scope.row.createTime || "-" }}</template>
        </el-table-column>
        <el-table-column label="备注" align="center" prop="remark" :show-overflow-tooltip="true" />
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
import { listStockRecord } from "@/api/wms/stockRecord";
import QueryForm from "@/components/QueryForm/index.vue";
import RightToolbar from "@/components/RightToolbar/index.vue";

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
const bizTypeMap = {
  receipt: "入库",
  delivery: "出库",
  move: "移库",
  check: "盘点",
  adjust: "调整",
  freeze: "冻结",
  thaw: "解冻",
  lock: "锁定",
  unlock: "解锁",
};

function bizTagType(bizType) {
  if (bizType === "receipt") return "success";
  if (bizType === "delivery") return "danger";
  if (bizType === "adjust") return "warning";
  return "info";
}

const queryConfig = computed(() => [
  {
    label: "关键字",
    prop: "keywords",
    type: "input",
    placeholder: "流水号 / 批次号 / 来源单号",
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
