<template>
  <div class="select-inline" :style="{ width: width }">
    <el-autocomplete
      v-model="searchText"
      :fetch-suggestions="fetchSuggestions"
      :placeholder="placeholderText"
      value-key="partnerName"
      :trigger-on-focus="false"
      :debounce="300"
      :disabled="disabled"
      clearable
      @select="handleSuggestionSelect"
      @clear="handleClear"
      class="select-input partner-select"
    >
      <template #append>
        <el-button :icon="Search" :disabled="disabled" @click="openDialog" />
      </template>
    </el-autocomplete>
    <el-dialog :title="dialogTitle" v-model="dialogVisible" width="998px" append-to-body @open="onDialogOpen">
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
              :model-value="dialogItem?.partnerId === scope.row.partnerId"
              @change="handleSelect(scope.row)"
            />
          </template>
        </el-table-column>
        <el-table-column :label="typeLabel + '编码'" align="center" prop="partnerCode" :show-overflow-tooltip="true" />
        <el-table-column :label="typeLabel + '名称'" align="center" prop="partnerName" :show-overflow-tooltip="true" />
        <el-table-column label="省份" align="center" prop="province" :show-overflow-tooltip="true" />
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
import { listPartner, getPartner } from "@/api/wms/partner";
import QueryForm from "@/components/QueryForm/index.vue";

const props = defineProps({
  modelValue: { type: [String, Number], default: undefined },
  placeholder: { type: String, default: "" },
  label: { type: String, default: "" },
  partnerType: { type: String, default: "" },
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
  partnerCode: undefined,
  partnerName: undefined,
});

const typeLabel = computed(() => {
  if (props.partnerType === "supplier") return "供应商";
  if (props.partnerType === "customer") return "客户";
  return "往来单位";
});

const placeholderText = computed(() => props.placeholder || "请选择" + typeLabel.value);
const dialogTitle = computed(() => "选择" + typeLabel.value);

const queryConfig = computed(() => [
  { label: typeLabel.value + "编码", prop: "partnerCode", type: "input", placeholder: `请输入${typeLabel.value}编码` },
  { label: typeLabel.value + "名称", prop: "partnerName", type: "input", placeholder: `请输入${typeLabel.value}名称` },
]);

// 根据 modelValue 加载默认显示
watch(() => props.modelValue, async (id) => {
  if (id && !props.label) {
    try {
      const res = await getPartner(id);
      if (res.data) {
        fetchedItem.value = res.data;
        searchText.value = res.data.partnerName || "";
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
  const params = {
    pageNum: queryParams.pageNum,
    pageSize: queryParams.pageSize,
    params: {
      partnerCode: queryParams.partnerCode,
      partnerName: queryParams.partnerName,
      partnerType: props.partnerType || undefined,
    },
  };
  listPartner(params).then((response) => {
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
  queryParams.partnerCode = undefined;
  queryParams.partnerName = undefined;
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
      params: {
        partnerName: queryString.trim(),
        partnerType: props.partnerType || undefined,
      },
    };
    const response = await listPartner(params);
    callback(response.rows || []);
  } catch {
    callback([]);
  }
}

function handleSuggestionSelect(item) {
  fetchedItem.value = item;
  searchText.value = item.partnerName || "";
  emit("update:modelValue", item.partnerId);
  emit("change", item);
}

function handleClear() {
  fetchedItem.value = null;
  searchText.value = "";
  emit("update:modelValue", undefined);
  emit("change", undefined);
}

function handleSelect(row) {
  if (dialogItem.value?.partnerId === row.partnerId) {
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
    ElMessage.warning("请至少选择一个" + typeLabel.value);
    return;
  }
  dialogVisible.value = false;
  fetchedItem.value = dialogItem.value;
  searchText.value = dialogItem.value.partnerName || "";
  emit("update:modelValue", dialogItem.value.partnerId);
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
.partner-select .el-input__clear {
  display: inline-flex !important;
}
</style>
