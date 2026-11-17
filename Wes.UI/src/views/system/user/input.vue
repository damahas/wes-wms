<template>
  <div class="select-inline" :style="{ width: width }">
    <el-autocomplete
      v-model="searchText"
      :fetch-suggestions="fetchSuggestions"
      :placeholder="placeholder"
      value-key="userName"
      :trigger-on-focus="false"
      :debounce="300"
      :disabled="disabled"
      clearable
      @select="handleSuggestionSelect"
      @clear="handleClear"
      class="select-input user-select"
    >
      <template #default="{ item }">
        <div class="user-option">
          <span class="user-option__name">{{ item.nickName || item.userName }}</span>
          <span class="user-option__dept">{{ item.dept?.deptName || '' }}</span>
        </div>
      </template>
      <template #append>
        <el-button :icon="Search" :disabled="disabled" @click="openDialog" />
      </template>
    </el-autocomplete>
    <SelectUser ref="userSelectRef" :isSingle="true" @handleData="handleUserSelect" />
  </div>
</template>

<script setup>
import { ref, watch } from "vue";
import { Search } from "@element-plus/icons-vue";
import { listUser, getUser } from "@/api/system/user";
import SelectUser from "./select.vue";

const props = defineProps({
  modelValue: { type: [String, Number], default: undefined },
  placeholder: { type: String, default: "请选择用户" },
  label: { type: String, default: "" },
  width: { type: String, default: "100%" },
  disabled: { type: Boolean, default: false },
});

const emit = defineEmits(["update:modelValue", "change"]);

const userSelectRef = ref(null);
const fetchedItem = ref(null);
const searchText = ref("");

// 根据 modelValue 加载默认显示
watch(() => props.modelValue, async (id) => {
  if (id && !props.label) {
    try {
      const res = await getUser(id);
      if (res.data) {
        fetchedItem.value = res.data;
        searchText.value = res.data.nickName || res.data.userName || "";
      }
    } catch {
      searchText.value = props.label || "";
      fetchedItem.value = null;
    }
  } else if (!id || id === undefined) {
    fetchedItem.value = null;
    searchText.value = props.label || "";
  }
}, { immediate: true });

// 外部传入 label 时同步输入框显示
watch(() => props.label, (label) => {
  if (label) {
    searchText.value = label;
  }
}, { immediate: true });

function openDialog() {
  userSelectRef.value.open();
}

// 输入搜索下拉
async function fetchSuggestions(queryString, callback) {
  if (!queryString || queryString.trim() === "") {
    callback([]);
    return;
  }
  try {
    const response = await listUser({ params: { userName: queryString.trim() }, pageNum: 1, pageSize: 20 });
    const rows = response.rows || [];
    callback(rows);
  } catch {
    callback([]);
  }
}

function handleSuggestionSelect(item) {
  fetchedItem.value = item;
  searchText.value = item.nickName || item.userName || "";
  emit("update:modelValue", item.userId);
  emit("change", item);
}

function handleClear() {
  fetchedItem.value = null;
  searchText.value = "";
  emit("update:modelValue", undefined);
  emit("change", undefined);
}

/** select.vue 弹窗确认回调 */
function handleUserSelect(selected) {
  if (!selected || selected.length === 0) return;
  const item = selected[0];
  fetchedItem.value = item;
  searchText.value = item.nickName || item.userName || "";
  emit("update:modelValue", item.userId);
  emit("change", item);
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

::deep(.el-dialog__body) {
  padding-top: 0;
}

.user-option {
  display: flex;
  justify-content: space-between;
  width: 100%;
  line-height: normal;
}
.user-option__name {
  font-size: 14px;
}
.user-option__dept {
  font-size: 12px;
  color: var(--el-text-color-secondary);
}
</style>

<!-- append-to-body 的 dialog 内元素无法被 scoped 样式穿透，使用全局样式 -->
<style lang="scss">
.user-select .el-input__clear {
  display: inline-flex !important;
}
</style>
