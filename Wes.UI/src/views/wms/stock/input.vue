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
      class="select-input stock-select"
    >
      <template #default="{ item }">
        <div class="stock-option">
          <span class="stock-option__name">
            {{ item.materialCode }} {{ item.materialName }}
            / {{ item.batchNo || "无批次" }}
            / {{ item.locationCode }}
          </span>
          <span class="stock-option__qty">可用 {{ available(item) }}</span>
        </div>
      </template>
      <template #append>
        <el-button :icon="Search" :disabled="disabled" @click="openDialog" />
      </template>
    </el-autocomplete>

    <!-- 弹窗逻辑全部委托给 select.vue，这里不重复实现 -->
    <SelectStock
      ref="selectRef"
      :isSingle="true"
      :warehouse-id="warehouseId"
      :available-only="availableOnly"
      @handleData="handleSelectData"
      v-show="false"
    />
  </div>
</template>

<script setup>
import { ref, watch } from "vue";
import { Search } from "@element-plus/icons-vue";
import { listStock, getStock } from "@/api/wms/stock";
import SelectStock from "./select.vue";

const props = defineProps({
  modelValue: { type: [String, Number], default: undefined },
  placeholder: { type: String, default: "请选择库存" },
  label: { type: String, default: "" },
  width: { type: String, default: "100%" },
  disabled: { type: Boolean, default: false },
  // 限定在指定仓库内挑库存
  warehouseId: { type: [String, Number], default: undefined },
  availableOnly: { type: Boolean, default: true },
});

const emit = defineEmits(["update:modelValue", "change"]);

const selectRef = ref(null);
const fetchedItem = ref(null);
const searchText = ref("");

function available(row) {
  if (!row) return 0;
  const v = (row.stockQuantity ?? 0) - (row.freezeQuantity ?? 0) - (row.lockQuantity ?? 0);
  return v > 0 ? v : 0;
}

function stockText(item) {
  if (!item) return "";
  return `${item.materialCode || ""} ${item.materialName || ""} / ${item.batchNo || "无批次"} / ${item.locationCode || ""}`;
}

watch(
  () => props.modelValue,
  async (id) => {
    if (id && !props.label) {
      try {
        const res = await getStock(id);
        if (res.data) {
          fetchedItem.value = res.data;
          searchText.value = stockText(res.data);
        }
      } catch {
        searchText.value = props.label || "";
        fetchedItem.value = null;
      }
    } else if (!id || id === undefined) {
      fetchedItem.value = null;
      searchText.value = props.label || "";
    }
  },
  { immediate: true }
);

watch(
  () => props.label,
  (label) => {
    if (label) searchText.value = label;
  },
  { immediate: true }
);

function openDialog() {
  selectRef.value?.open();
}

function handleSelectData(selected) {
  if (!selected || selected.length === 0) return;
  const item = selected[0];
  fetchedItem.value = item;
  searchText.value = stockText(item);
  emit("update:modelValue", item.stockId);
  emit("change", item);
}

async function fetchSuggestions(queryString, callback) {
  if (!queryString || queryString.trim() === "") {
    callback([]);
    return;
  }
  try {
    const response = await listStock({
      keywords: queryString.trim(),
      warehouseId: props.warehouseId,
      availableOnly: props.availableOnly,
      pageNum: 1,
      pageSize: 20,
    });
    callback(response.rows || []);
  } catch {
    callback([]);
  }
}

function handleSuggestionSelect(item) {
  fetchedItem.value = item;
  searchText.value = stockText(item);
  emit("update:modelValue", item.stockId);
  emit("change", item);
}

function handleClear() {
  fetchedItem.value = null;
  searchText.value = "";
  emit("update:modelValue", undefined);
  emit("change", undefined);
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

.stock-option {
  display: flex;
  justify-content: space-between;
  width: 100%;
  line-height: normal;
  column-gap: 12px;
}

.stock-option__name {
  font-size: 14px;
}

.stock-option__qty {
  font-size: 12px;
  color: var(--el-color-success);
  white-space: nowrap;
}
</style>

<style lang="scss">
.stock-select .el-input__clear {
  display: inline-flex !important;
}
</style>
