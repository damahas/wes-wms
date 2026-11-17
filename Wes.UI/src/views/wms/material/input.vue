<template>
  <div class="select-inline" :style="{ width: width }">
    <el-autocomplete
      v-model="searchText"
      :fetch-suggestions="fetchSuggestions"
      :placeholder="placeholder"
      value-key="materialName"
      :trigger-on-focus="false"
      :debounce="300"
      :disabled="disabled"
      clearable
      @select="handleSuggestionSelect"
      @clear="handleClear"
      class="select-input material-select"
    >
      <template #append>
        <el-button :icon="Search" :disabled="disabled" @click="openDialog" />
      </template>
    </el-autocomplete>
    <el-dialog title="选择物料" v-model="dialogVisible" width="998px" append-to-body @open="onDialogOpen">
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
              :model-value="dialogItem?.materialId === scope.row.materialId"
              @change="handleSelect(scope.row)"
            />
          </template>
        </el-table-column>
        <el-table-column label="物料编码" align="center" prop="materialCode" :show-overflow-tooltip="true" />
        <el-table-column label="物料名称" align="center" prop="materialName" :show-overflow-tooltip="true" />
        <el-table-column label="物料类型" align="center" prop="materialType">
          <template #default="scope">
            <dict-tag :options="wms_material_type" :value="scope.row.materialType" />
          </template>
        </el-table-column>
        <el-table-column label="规格" align="center" prop="spec" :show-overflow-tooltip="true" />
        <el-table-column label="基本单位" align="center" prop="unit">
          <template #default="scope">
            <span>{{ unitLabel(scope.row.unit) }}</span>
          </template>
        </el-table-column>
        <el-table-column label="状态" align="center" prop="status">
          <template #default="scope">
            <el-tag :type="scope.row.status === '0' ? 'success' : 'info'" size="small">
              {{ scope.row.status === '0' ? '启用' : '停用' }}
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
          <el-button type="primary" @click="submitForm">确 定</el-button>
          <el-button @click="dialogVisible = false">取 消</el-button>
        </div>
      </template>
    </el-dialog>
  </div>
</template>

<script setup>
import { ref, reactive, computed, watch } from "vue";
import { ElMessage } from "element-plus";
import { Search } from "@element-plus/icons-vue";
import { listMaterial, getMaterial } from "@/api/wms/material";
import { getDict } from "@/utils";
import DictTag from "@/components/DictTag/index.vue";
import QueryForm from "@/components/QueryForm/index.vue";

const { wms_material_type, wms_sys_unit_type } = getDict("wms_material_type", "wms_sys_unit_type");

const props = defineProps({
  modelValue: { type: [String, Number], default: undefined },
  placeholder: { type: String, default: "请选择物料" },
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
const dialogItem = ref(null); // 弹窗内临时选择

const queryParams = reactive({
  pageNum: 1,
  pageSize: 10,
  materialCode: undefined,
  materialName: undefined,
});

const queryConfig = computed(() => [
  { label: "物料编码", prop: "materialCode", type: "input", placeholder: "请输入物料编码" },
  { label: "物料名称", prop: "materialName", type: "input", placeholder: "请输入物料名称" },
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
  dialogItem.value = fetchedItem.value; // 保留当前值作为初始选中
  dialogVisible.value = true;
  getList();
}

function onDialogOpen() {
  dialogItem.value = fetchedItem.value;
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
  fetchedItem.value = item;
  searchText.value = item.materialName || "";
  emit("update:modelValue", item.materialId);
  emit("change", item);
}

function handleClear() {
  fetchedItem.value = null;
  searchText.value = "";
  emit("update:modelValue", undefined);
  emit("change", undefined);
}

function handleSelect(row) {
  if (dialogItem.value?.materialId === row.materialId) {
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
    ElMessage.warning("请至少选择一个物料");
    return;
  }
  dialogVisible.value = false;
  fetchedItem.value = dialogItem.value;
  searchText.value = dialogItem.value.materialName || "";
  emit("update:modelValue", dialogItem.value.materialId);
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
