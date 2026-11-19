<template>
  <el-dialog :title="t('wms.warehouse.selectTitle')" v-model="isOpen" width="998px" append-to-body>
    <query-form
      :config="queryConfig"
      v-model="queryParams"
      @query="handleQuery"
      @reset="resetQuery"
    />

    <el-table v-loading="loading" :data="dataList" @row-click="handleRowClick">
      <el-table-column width="55" align="center">
        <template #default="scope">
          <el-checkbox
            :model-value="isSelected(scope.row)"
            @change="handleSelect(scope.row)"
          />
        </template>
      </el-table-column>
      <el-table-column :label="t('wms.warehouse.warehouseCode')" align="center" prop="warehouseCode" :show-overflow-tooltip="true" />
      <el-table-column :label="t('wms.warehouse.warehouseName')" align="center" prop="warehouseName" :show-overflow-tooltip="true" />
      <el-table-column :label="t('common.status')" align="center" prop="status">
        <template #default="scope">
          <el-tag :type="scope.row.status === '0' ? 'success' : 'info'" size="small">
            {{ scope.row.status === '0' ? t('common.enable') : t('common.disable') }}
          </el-tag>
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
        <el-button type="primary" @click="submitForm">{{ t('common.submit') }}</el-button>
        <el-button @click="isOpen = false">{{ t('common.cancel') }}</el-button>
      </div>
    </template>
  </el-dialog>
</template>

<script setup>
import { ref, reactive, computed } from "vue";
import { ElMessage } from "element-plus";
import { useI18n } from "vue-i18n";
import { listWarehouse } from "@/api/wms/warehouse";
import QueryForm from "@/components/QueryForm/index.vue";

const { t } = useI18n();

const loading = ref(true);
const checked = ref([]);
const total = ref(0);
const dataList = ref([]);
const isOpen = ref(false);

const queryParams = reactive({
  pageNum: 1,
  pageSize: 10,
  keywords: undefined,
});

const queryConfig = computed(() => [
  { label: t('wms.warehouse.keywords'), prop: "keywords", type: "input", placeholder: t('wms.warehouse.ph.inputCodeOrName') },
]);

const emit = defineEmits(["handleData"]);

function open() {
  checked.value = [];
  isOpen.value = true;
  getList();
}

function getList() {
  loading.value = true;
  listWarehouse(queryParams).then((response) => {
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
  queryParams.keywords = undefined;
  queryParams.pageNum = 1;
  getList();
}

function isSelected(row) {
  return checked.value.some((item) => item.warehouseId === row.warehouseId);
}

function handleSelect(row) {
  const index = checked.value.findIndex((item) => item.warehouseId === row.warehouseId);
  if (index > -1) {
    checked.value.splice(index, 1);
  } else {
    checked.value.push(row);
  }
}

function handleRowClick(row) {
  handleSelect(row);
}

function submitForm() {
  if (checked.value.length === 0) {
    ElMessage.warning(t('wms.warehouse.selectAtLeastOne'));
    return false;
  }
  isOpen.value = false;
  emit("handleData", checked.value);
}

defineExpose({ open });
</script>

<style lang="scss" scoped>
::v-deep(.el-dialog__body) {
  padding-top: 0;
}
</style>
