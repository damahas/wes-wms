<template>
  <div class="select-inline" :style="{ width: width }">
    <el-autocomplete
      v-model="searchText"
      :fetch-suggestions="fetchSuggestions"
      :placeholder="placeholderText"
      value-key="locationName"
      :trigger-on-focus="false"
      :debounce="300"
      :disabled="disabled"
      clearable
      @select="handleSuggestionSelect"
      @clear="handleClear"
      class="select-input location-select"
    >
      <template #append>
        <el-button :icon="Search" :disabled="disabled" @click="openDialog" />
      </template>
    </el-autocomplete>
    <el-dialog :title="t('wms.location.selectTitle')" v-model="dialogVisible" width="998px" append-to-body @open="onDialogOpen">
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
              :model-value="dialogItem?.locationId === scope.row.locationId"
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
        <el-table-column :label="t('common.status')" align="center" prop="locationStatus" width="80">
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
          <el-button @click="dialogVisible = false">{{ t('common.cancel') }}</el-button>
        </div>
      </template>
    </el-dialog>
  </div>
</template>

<script setup>
import { ref, reactive, computed, watch } from "vue";
import { ElMessage } from "element-plus";
import { useI18n } from "vue-i18n";
import { Search } from "@element-plus/icons-vue";
import { listLocation, getLocation } from "@/api/wms/location";
import QueryForm from "@/components/QueryForm/index.vue";

const { t } = useI18n();

const props = defineProps({
  modelValue: { type: [String, Number], default: undefined },
  warehouseId: { type: [String, Number], default: undefined },
  zoneId: { type: [String, Number], default: undefined },
  placeholder: { type: String, default: "" },
  label: { type: String, default: "" },
  width: { type: String, default: "100%" },
  disabled: { type: Boolean, default: false },
});

const emit = defineEmits(["update:modelValue", "change"]);

const loading = ref(true);
const total = ref(0);
const dataList = ref([]);
const dialogVisible = ref(false);
const fetchedItem = ref(null);
const searchText = ref("");
const dialogItem = ref(null);

const queryParams = reactive({
  pageNum: 1,
  pageSize: 10,
  keywords: undefined,
});

const placeholderText = computed(() => props.placeholder || t('wms.location.ph.selectLocation'));

const queryConfig = computed(() => [
  { label: t('wms.common.keywords'), prop: "keywords", type: "input", placeholder: t('wms.location.ph.inputCodeOrName') },
]);

// 根据 modelValue 加载默认显示
watch(() => props.modelValue, async (id) => {
  if (id && !props.label) {
    try {
      const res = await getLocation(id);
      if (res.data) {
        fetchedItem.value = res.data;
        searchText.value = res.data.locationName || "";
      }
    } catch { /* ignore */ }
  } else if (!id || id === undefined) {
    fetchedItem.value = null;
    searchText.value = "";
  }
}, { immediate: true });

// 库区变化时清空已选库位
watch(() => props.zoneId, (newVal, oldVal) => {
  if (oldVal !== undefined && newVal !== oldVal) {
    searchText.value = "";
    fetchedItem.value = null;
    emit("update:modelValue", undefined);
    emit("change", undefined);
  }
});

function openDialog() {
  dialogItem.value = fetchedItem.value;
  dialogVisible.value = true;
  getList();
}

function onDialogOpen() {
  dialogItem.value = fetchedItem.value;
  queryParams.pageNum = 1;
}

function getList() {
  loading.value = true;
  const params = { ...queryParams };
  if (props.warehouseId) {
    params.warehouseId = props.warehouseId;
  }
  if (props.zoneId) {
    params.zoneId = props.zoneId;
  }
  listLocation(params).then((response) => {
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

// 输入搜索下拉
async function fetchSuggestions(queryString, callback) {
  if (!queryString || queryString.trim() === "") {
    callback([]);
    return;
  }
  try {
    const params = {
      pageNum: 1,
      pageSize: 20,
      keywords: queryString.trim(),
    };
    if (props.warehouseId) {
      params.warehouseId = props.warehouseId;
    }
    if (props.zoneId) {
      params.zoneId = props.zoneId;
    }
    const response = await listLocation(params);
    callback(response.rows || []);
  } catch {
    callback([]);
  }
}

function handleSuggestionSelect(item) {
  fetchedItem.value = item;
  searchText.value = item.locationName || "";
  emit("update:modelValue", item.locationId);
  emit("change", item);
}

function handleClear() {
  fetchedItem.value = null;
  searchText.value = "";
  emit("update:modelValue", undefined);
  emit("change", undefined);
}

function handleSelect(row) {
  if (dialogItem.value?.locationId === row.locationId) {
    dialogItem.value = null;
  } else {
    dialogItem.value = row;
  }
}

function handleRowClick(row) {
  handleSelect(row);
}

function submitForm() {
  if (!dialogItem.value) {
    ElMessage.warning(t('wms.location.selectAtLeastOne'));
    return;
  }
  dialogVisible.value = false;
  fetchedItem.value = dialogItem.value;
  searchText.value = dialogItem.value.locationName || "";
  emit("update:modelValue", dialogItem.value.locationId);
  emit("change", dialogItem.value);
  dialogItem.value = null;
}

defineExpose({ open: openDialog });
</script>

<style lang="scss" scoped>
.select-inline {
  display: inline-block;
  vertical-align: middle;
}

.select-input {
  :deep(.el-input__suffix) {
    right: 2px;
  }
  :deep(.el-input__inner) {
    padding-right: 24px;
  }
  :deep(.el-input__clear) {
    display: flex !important;
  }
}

::deep(.el-dialog__body) {
  padding-top: 0;
}
</style>

<style lang="scss">
.location-select .el-input__clear {
  display: inline-flex !important;
}
</style>
