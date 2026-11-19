<template>
  <div class="select-inline" :style="{ width: width }">
    <el-autocomplete
      v-model="searchText"
      :fetch-suggestions="multiple ? fetchNoSuggestions : fetchSuggestions"
      :placeholder="multiple ? t('wms.material.ph.clickMulti') : (placeholder || t('wms.material.ph.selectMaterial'))"
      value-key="materialName"
      :trigger-on-focus="false"
      :debounce="300"
      clearable
      @select="handleSuggestionSelect"
      @clear="handleClear"
      class="select-input material-select"
    >
      <template #append>
        <el-button :icon="Search" @click="openDialog" />
      </template>
    </el-autocomplete>
    <el-dialog :title="t('wms.material.selectTitle')" v-model="dialogVisible" width="998px" append-to-body @open="onDialogOpen">
      <query-form
        :config="queryConfig"
        v-model="queryParams"
        @query="handleQuery"
        @reset="resetQuery"
      />

      <el-table v-loading="loading" :data="dataList" ref="multiTableRef" @row-click="handleRowClick" @selection-change="onSelectionChange">
        <el-table-column v-if="!multiple" width="55" align="center">
          <template #default="scope">
            <el-checkbox
              :model-value="dialogItem?.materialId === scope.row.materialId"
              @change="handleSelect(scope.row)"
            />
          </template>
        </el-table-column>
        <el-table-column v-if="multiple" type="selection" width="55" align="center" />
        <el-table-column :label="t('wms.material.materialCode')" align="center" prop="materialCode" :show-overflow-tooltip="true" />
        <el-table-column :label="t('wms.material.materialName')" align="center" prop="materialName" :show-overflow-tooltip="true" />
        <el-table-column :label="t('wms.material.materialType')" align="center" prop="materialType">
          <template #default="scope">
            <dict-tag :options="wms_material_type" :value="scope.row.materialType" />
          </template>
        </el-table-column>
        <el-table-column :label="t('wms.material.spec')" align="center" prop="spec" :show-overflow-tooltip="true" />
        <el-table-column :label="t('wms.material.basicUnit')" align="center" prop="unit">
          <template #default="scope">
            <span>{{ unitLabel(scope.row.unit) }}</span>
          </template>
        </el-table-column>
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
import { listMaterial, getMaterial } from "@/api/wms/material";
import { getDict } from "@/utils";
import DictTag from "@/components/DictTag/index.vue";
import QueryForm from "@/components/QueryForm/index.vue";

const { t } = useI18n();

const { wms_material_type, wms_sys_unit_type } = getDict("wms_material_type", "wms_sys_unit_type");

const props = defineProps({
  modelValue: { type: [String, Number], default: undefined },
  placeholder: { type: String, default: "" },
  label: { type: String, default: "" },
  width: { type: String, default: "100%" },
  multiple: { type: Boolean, default: false },
});

const emit = defineEmits(["update:modelValue", "change"]);

const loading = ref(true);
const total = ref(0);
const dataList = ref([]);
const dialogVisible = ref(false);
const fetchedItem = ref(null);
const searchText = ref("");
const dialogItem = ref(null); // 弹窗内临时选择（单选）
const dialogItems = ref([]); // 弹窗内临时选择（多选）
const multiTableRef = ref(null);

const queryParams = reactive({
  pageNum: 1,
  pageSize: 10,
  materialCode: undefined,
  materialName: undefined,
});

const queryConfig = computed(() => [
  { label: t('wms.material.materialCode'), prop: "materialCode", type: "input", placeholder: t('wms.material.ph.inputCode') },
  { label: t('wms.material.materialName'), prop: "materialName", type: "input", placeholder: t('wms.material.ph.inputName') },
]);

function unitLabel(val) {
  if (val === undefined || val === null || val === "") return "-";
  const item = wms_sys_unit_type.value.find((d) => d.value === val);
  return item ? item.label : val;
}

// 根据 modelValue 加载默认显示
watch(() => props.modelValue, async (id) => {
  if (id && !props.label) {
    try {
      const res = await getMaterial(id);
      if (res.data) {
        fetchedItem.value = res.data;
        searchText.value = res.data.materialName || "";
      }
    } catch { /* ignore */ }
  } else if (!id || id === undefined) {
    fetchedItem.value = null;
    searchText.value = "";
  }
}, { immediate: true });

// 外部传入 label 时同步输入框显示
watch(() => props.label, (label) => {
  if (label) {
    searchText.value = label;
  }
}, { immediate: true });

function openDialog() {
  if (props.multiple) {
    dialogItems.value = [];
  } else {
    dialogItem.value = fetchedItem.value;
  }
  dialogVisible.value = true;
  getList();
}

function onDialogOpen() {
  if (props.multiple) {
    dialogItems.value = [];
  } else {
    dialogItem.value = fetchedItem.value;
  }
  queryParams.pageNum = 1;
}

function getList() {
  loading.value = true;
  listMaterial(queryParams).then((response) => {
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
  queryParams.materialCode = undefined;
  queryParams.materialName = undefined;
  queryParams.pageNum = 1;
  getList();
}

// 多选模式下自动完成不显示建议
function fetchNoSuggestions(_queryString, callback) {
  callback([]);
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
      materialCode: undefined,
      materialName: queryString.trim(),
    };
    const response = await listMaterial(params);
    callback(response.rows || []);
  } catch {
    callback([]);
  }
}

function handleSuggestionSelect(item) {
  if (props.multiple) return; // 多选模式下自动完成无效
  fetchedItem.value = item;
  searchText.value = item.materialName || "";
  emit("update:modelValue", item.materialId);
  emit("change", item);
}

function handleClear() {
  fetchedItem.value = null;
  searchText.value = "";
  if (!props.multiple) {
    emit("update:modelValue", undefined);
    emit("change", undefined);
  }
}

function handleSelect(row) {
  if (dialogItem.value?.materialId === row.materialId) {
    dialogItem.value = null;
  } else {
    dialogItem.value = row;
  }
}

function handleRowClick(row) {
  if (props.multiple) {
    multiTableRef.value?.toggleRowSelection(row, undefined);
    return;
  }
  handleSelect(row);
}

function onSelectionChange(selection) {
  if (props.multiple) dialogItems.value = selection;
}

function submitForm() {
  if (props.multiple) {
    if (dialogItems.value.length === 0) {
      ElMessage.warning(t('wms.material.selectAtLeastOne'));
      return;
    }
    dialogVisible.value = false;
    searchText.value = dialogItems.value.map((m) => m.materialName).join("、");
    emit("change", dialogItems.value);
    dialogItems.value = [];
  } else {
    if (!dialogItem.value) {
      ElMessage.warning(t('wms.material.selectAtLeastOne'));
      return;
    }
    dialogVisible.value = false;
    fetchedItem.value = dialogItem.value;
    searchText.value = dialogItem.value.materialName || "";
    emit("update:modelValue", dialogItem.value.materialId);
    emit("change", dialogItem.value);
    dialogItem.value = null;
  }
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
  // 始终预留清空按钮空间，避免 hover 时出现导致宽度跳变
  :deep(.el-input__inner) {
    padding-right: 24px;
  }
  :deep(.el-input__clear) {
    display: flex !important;
  }
}

:deep(.el-dialog__body) {
  padding-top: 0;
}
</style>

<!-- append-to-body 的 dialog 内元素无法被 scoped 样式穿透，使用全局样式 -->
<style lang="scss">
.material-select .el-input__clear {
  display: inline-flex !important;
}
</style>
