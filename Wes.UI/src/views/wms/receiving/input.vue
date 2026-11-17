<template>
  <div class="select-inline" :style="{ width: width }">
    <el-autocomplete
      v-model="searchText"
      :fetch-suggestions="fetchSuggestions"
      :placeholder="placeholderText"
      value-key="receivingNo"
      :trigger-on-focus="false"
      :debounce="300"
      :disabled="disabled"
      clearable
      @select="handleSuggestionSelect"
      @clear="handleClear"
      class="select-input receiving-select"
    >
      <template #append>
        <el-button :icon="Search" :disabled="disabled" @click="openDialog" />
      </template>
    </el-autocomplete>
    <el-dialog :title="'选择收货单'" v-model="dialogVisible" width="998px" append-to-body @open="onDialogOpen">
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
              :model-value="dialogItem?.receivingId === scope.row.receivingId"
              @change="handleSelect(scope.row)"
            />
          </template>
        </el-table-column>
        <el-table-column label="收货单号" align="center" prop="receivingNo" :show-overflow-tooltip="true" min-width="160" />
        <el-table-column label="供应商" align="center" :show-overflow-tooltip="true" min-width="160">
          <template #default="scope">
            {{ scope.row.supplier?.partnerName || "-" }}
          </template>
        </el-table-column>
        <el-table-column label="预计到货日期" align="center" min-width="120">
          <template #default="scope">
            {{ scope.row.expectDate ? scope.row.expectDate.slice(0, 10) : "-" }}
          </template>
        </el-table-column>
        <el-table-column label="承运人" align="center" prop="carrierName" :show-overflow-tooltip="true" min-width="100" />
        <el-table-column label="状态" align="center" width="80">
          <template #default="scope">
            <el-tag :type="statusTagType(scope.row.receivingStatus)" size="small">
              {{ statusLabel(scope.row.receivingStatus) }}
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
import { listReceiving, getReceiving } from "@/api/wms/receiving";
import QueryForm from "@/components/QueryForm/index.vue";

const props = defineProps({
  modelValue: { type: [String, Number], default: undefined },
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
  receivingNo: undefined,
  supplierName: undefined,
});

const placeholderText = computed(() => props.placeholder || "请选择收货单");

const queryConfig = computed(() => [
  { label: "收货单号", prop: "receivingNo", type: "input", placeholder: "请输入收货单号" },
  { label: "供应商", prop: "supplierName", type: "input", placeholder: "请输入供应商名称" },
]);

const statusMap = { pending: "待收货", arriving: "收货中", completed: "已完成", cancelled: "已取消" };
function statusLabel(s) { return statusMap[s] || s; }
function statusTagType(s) {
  const map = { pending: "warning", arriving: "primary", completed: "success", cancelled: "info" };
  return map[s] || "";
}

// 根据 modelValue 加载默认显示
watch(() => props.modelValue, async (id) => {
  if (id && !props.label) {
    try {
      const res = await getReceiving(id);
      if (res.data) {
        fetchedItem.value = res.data;
        searchText.value = res.data.receivingNo || "";
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
  const params = {
    pageNum: queryParams.pageNum,
    pageSize: queryParams.pageSize,
    params: {
      keywords: queryParams.receivingNo || undefined,
      receivingStatus: "pending",
    },
  };
  listReceiving(params).then((response) => {
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
  queryParams.receivingNo = undefined;
  queryParams.supplierName = undefined;
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
        keywords: queryString.trim(),
        receivingStatus: "pending",
      },
    };
    const response = await listReceiving(params);
    callback((response.rows || []).map((r) => ({
      ...r,
      value: r.receivingNo,
    })));
  } catch {
    callback([]);
  }
}

function handleSuggestionSelect(item) {
  fetchedItem.value = item;
  searchText.value = item.receivingNo || "";
  emit("update:modelValue", item.receivingId);
  emit("change", item);
}

function handleClear() {
  fetchedItem.value = null;
  searchText.value = "";
  emit("update:modelValue", undefined);
  emit("change", undefined);
}

function handleSelect(row) {
  if (dialogItem.value?.receivingId === row.receivingId) {
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
    ElMessage.warning("请至少选择一个收货单");
    return;
  }
  dialogVisible.value = false;
  fetchedItem.value = dialogItem.value;
  searchText.value = dialogItem.value.receivingNo || "";
  emit("update:modelValue", dialogItem.value.receivingId);
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
.receiving-select .el-input__clear {
  display: inline-flex !important;
}
</style>
