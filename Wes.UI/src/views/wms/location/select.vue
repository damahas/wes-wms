<template>
  <el-dialog :title="t('wms.location.selectTitle')" v-model="isOpen" width="998px" append-to-body>
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
      <el-table-column :label="t('wms.location.locationCode')" align="center" prop="locationCode" :show-overflow-tooltip="true" />
      <el-table-column :label="t('wms.location.locationName')" align="center" prop="locationName" :show-overflow-tooltip="true" />
      <el-table-column :label="t('wms.common.warehouse')" align="center" :show-overflow-tooltip="true">
        <template #default="scope">
          {{ scope.row.warehouse?.warehouseName || "-" }}
        </template>
      </el-table-column>
      <el-table-column :label="t('wms.common.zone')" align="center" :show-overflow-tooltip="true">
        <template #default="scope">
          {{ scope.row.zone?.zoneName || "-" }}
        </template>
      </el-table-column>
      <el-table-column :label="t('common.status')" align="center" prop="locationStatus">
        <template #default="scope">
          <el-tag :type="scope.row.locationStatus === '0' ? 'success' : 'info'" size="small">
            {{ scope.row.locationStatus === '0' ? t('common.enable') : t('common.disable') }}
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
import { listLocation } from "@/api/wms/location";
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
  { label: t('wms.common.keywords'), prop: "keywords", type: "input", placeholder: t('wms.location.ph.inputCodeOrName') },
]);

const emit = defineEmits(["handleData"]);

function open() {
  checked.value = [];
  isOpen.value = true;
  getList();
}

function getList() {
  loading.value = true;
  listLocation(queryParams).then((response) => {
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
  return checked.value.some((item) => item.locationId === row.locationId);
}

function handleSelect(row) {
  const index = checked.value.findIndex((item) => item.locationId === row.locationId);
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
    ElMessage.warning(t('wms.location.selectAtLeastOne'));
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
