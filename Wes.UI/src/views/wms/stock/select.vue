<template>
  <el-dialog title="选择库存" v-model="isOpen" width="1080px" append-to-body>
    <query-form
      :config="queryConfig"
      v-model="queryParams.params"
      @query="handleQuery"
      @reset="resetQuery"
    />

    <el-row :gutter="10" class="mb8" v-if="!props.isSingle">
      <el-checkbox
        v-for="(item, index) in checked"
        :key="index"
        :model-value="true"
        @change="handleCloseTag(index)"
        style="margin-right: 12px"
      >
        {{ tagText(item) }}
      </el-checkbox>
    </el-row>

    <el-table v-loading="loading" :data="dataList" @row-click="handleRowClick">
      <el-table-column width="55" align="center">
        <template #default="scope">
          <el-checkbox :model-value="isChecked(scope.row)" @change="handleToggle(scope.row)" />
        </template>
      </el-table-column>
      <el-table-column label="物料编码" align="center" prop="materialCode" width="120" :show-overflow-tooltip="true" />
      <el-table-column label="物料名称" align="center" prop="materialName" min-width="150" :show-overflow-tooltip="true" />
      <el-table-column label="批次" align="center" prop="batchNo" width="110">
        <template #default="scope">
          {{ scope.row.batchNo || "-" }}
        </template>
      </el-table-column>
      <el-table-column label="仓库" align="center" width="110" :show-overflow-tooltip="true">
        <template #default="scope">
          {{ scope.row.warehouse?.warehouseName || "-" }}
        </template>
      </el-table-column>
      <el-table-column label="库位" align="center" prop="locationCode" width="110" :show-overflow-tooltip="true" />
      <el-table-column label="库存" align="center" prop="stockQuantity" width="80" />
      <el-table-column label="冻结" align="center" prop="freezeQuantity" width="80">
        <template #default="scope">
          <span :class="{ 'text-frozen': (scope.row.freezeQuantity ?? 0) > 0 }">
            {{ scope.row.freezeQuantity ?? 0 }}
          </span>
        </template>
      </el-table-column>
      <el-table-column label="锁定" align="center" prop="lockQuantity" width="80">
        <template #default="scope">
          <span :class="{ 'text-frozen': (scope.row.lockQuantity ?? 0) > 0 }">
            {{ scope.row.lockQuantity ?? 0 }}
          </span>
        </template>
      </el-table-column>
      <el-table-column label="可用" align="center" width="90">
        <template #default="scope">
          <span class="text-available">{{ available(scope.row) }}</span>
        </template>
      </el-table-column>
      <el-table-column label="有效期至" align="center" width="110">
        <template #default="scope">
          {{ scope.row.expireDate ? String(scope.row.expireDate).slice(0, 10) : "-" }}
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

    <template #footer>
      <div class="dialog-footer">
        <el-button type="primary" @click="submitForm">确 定</el-button>
        <el-button @click="isOpen = false">取 消</el-button>
      </div>
    </template>
  </el-dialog>
</template>

<script setup>
import { ref, reactive, computed } from "vue";
import { ElMessage } from "element-plus";
import { listStock } from "@/api/wms/stock";
import QueryForm from "@/components/QueryForm/index.vue";

const props = defineProps({
  isSingle: { type: Boolean, default: true },
  // 仓库必传：移库限定在本仓内挑，调拨限定在调出仓内挑
  warehouseId: { type: [String, Number], default: undefined },
  // 默认只看可用的：冻着的在等检验，锁着的已被工单占住，都不能搬
  availableOnly: { type: Boolean, default: true },
});

const emit = defineEmits(["handleData"]);

const isOpen = ref(false);
const loading = ref(true);
const total = ref(0);
const dataList = ref([]);
const checked = ref([]);

const queryParams = reactive({
  pageNum: 1,
  pageSize: 10,
  params: {},
});

const queryConfig = computed(() => [
  { label: "关键字", prop: "keywords", type: "input", placeholder: "物料编码/名称/批次/库位" },
  { label: "批次号", prop: "batchNo", type: "input", placeholder: "请输入批次号" },
]);

/** 可用 = 库存 - 冻结 - 锁定，不直接用 available_quantity，那个字段可能是脏的 */
function available(row) {
  const v = (row.stockQuantity ?? 0) - (row.freezeQuantity ?? 0) - (row.lockQuantity ?? 0);
  return v > 0 ? v : 0;
}

function tagText(item) {
  return `${item.materialName} / ${item.batchNo || "无批次"} / ${item.locationCode}`;
}

function isChecked(row) {
  return checked.value.some((p) => p.stockId === row.stockId);
}

function handleToggle(row) {
  if (isChecked(row)) {
    checked.value = checked.value.filter((p) => p.stockId !== row.stockId);
  } else {
    if (checked.value.length && props.isSingle) {
      checked.value = [];
    }
    checked.value.push(row);
  }
}

function handleRowClick(row) {
  handleToggle(row);
}

function handleCloseTag(index) {
  checked.value.splice(index, 1);
}

function getList() {
  loading.value = true;
  const query = {
    ...queryParams.params,
    warehouseId: props.warehouseId,
    availableOnly: props.availableOnly,
    pageNum: queryParams.pageNum,
    pageSize: queryParams.pageSize,
  };
  listStock(query).then((response) => {
    dataList.value = response.rows || [];
    total.value = response.total || 0;
    loading.value = false;
  });
}

function handleQuery() {
  queryParams.pageNum = 1;
  getList();
}

function resetQuery() {
  queryParams.params = {};
  queryParams.pageNum = 1;
  getList();
}

function submitForm() {
  if (checked.value.length === 0) {
    ElMessage.warning("请至少选择一行库存");
    return;
  }
  isOpen.value = false;
  emit("handleData", checked.value.slice());
}

function open() {
  isOpen.value = true;
  checked.value = [];
  queryParams.params = {};
  queryParams.pageNum = 1;
  getList();
}

defineExpose({ open });
</script>

<style lang="scss" scoped>
:deep(.el-dialog__body) {
  padding-top: 0;
}

.text-available {
  color: var(--el-color-success);
  font-weight: 500;
}

.text-frozen {
  color: var(--el-color-warning);
}
</style>
