<template>
  <div class="app-container">
    <div class="split-panel">
      <!-- 左侧物料分类树 -->
      <div class="split-left-panel">
        <div class="split-left__header">
          <div class="split-left__title">{{ t('wms.material.category') }}</div>
          <div class="split-left__search">
            <el-input v-model="categoryName" :placeholder="t('wms.material.ph.searchCategory')" clearable prefix-icon="Search" />
          </div>
        </div>
        <div class="split-left__body">
          <el-tree
            :data="categoryOptions"
            :props="{ label: 'label', children: 'children' }"
            :expand-on-click-node="false"
            :filter-node-method="filterNode"
            ref="categoryTreeRef"
            node-key="value"
            highlight-current
            default-expand-all
            @node-click="handleNodeClick"
          >
            <template #default="{ data }">
              <span class="tree-node">
                <el-icon><FolderOpened /></el-icon>
                <span>{{ data.label }}</span>
              </span>
            </template>
          </el-tree>
        </div>
      </div>

      <!-- 右侧检验项表格 -->
      <div class="split-right-panel">
        <template v-if="selectedCategory">
          <div class="split-right__header">
            <span class="split-right__title">{{ selectedCategory.label }}</span>
            <span class="split-right__sub">（{{ t('wms.iqcItem.configSub') }}）</span>
          </div>

          <el-row :gutter="10" class="mb8">
            <el-col :span="2">
              <el-button class="detail-add-btn" type="primary" plain icon="Plus" @click="handleAddRow">{{ t('wms.iqcItem.addRow') }}</el-button>
            </el-col>
            <el-col :span="1.5">
              <el-button type="danger" plain icon="Delete" :disabled="selectedRows.length === 0" @click="handleDeleteRows">{{ t('wms.partner.deleteSelected') }}</el-button>
            </el-col>
            <el-col :span="1.5">
              <el-button type="success" icon="Check" @click="handleBatchSave">{{ t('common.save') }}</el-button>
            </el-col>
          </el-row>

          <el-table
            v-loading="loading"
            :data="tableData"
            @selection-change="handleSelectionChange"
            border
          >
            <el-table-column type="selection" width="55" align="center" />

            <el-table-column :label="t('wms.common.index')" align="center" width="80">
              <template #default="scope">
                <el-input-number controls-position="right"
                  v-model="scope.row.sortNo"
                  :min="0"
                  :controls="false"
                  style="width: 100%"
                />
              </template>
            </el-table-column>

            <el-table-column :label="t('wms.iqcItem.itemCode')" align="center" min-width="140">
              <template #default="scope">
                <el-input v-model="scope.row.itemCode" :placeholder="t('wms.common.code')" maxlength="50" />
              </template>
            </el-table-column>

            <el-table-column :label="t('wms.iqcItem.itemName')" align="center" min-width="160">
              <template #default="scope">
                <el-input v-model="scope.row.itemName" :placeholder="t('wms.common.name')" maxlength="100" />
              </template>
            </el-table-column>

            <el-table-column :label="t('wms.iqcItem.itemType')" align="center" width="120">
              <template #default="scope">
                <el-select v-model="scope.row.itemType" :placeholder="t('wms.common.pleaseSelect')" style="width: 100%">
                  <el-option :label="t('wms.iqcItem.quantitative')" value="quantitative" />
                  <el-option :label="t('wms.iqcItem.qualitative')" value="qualitative" />
                  <el-option :label="t('wms.iqcItem.visual')" value="visual" />
                </el-select>
              </template>
            </el-table-column>

            <el-table-column :label="t('wms.iqcItem.specValue')" align="center" min-width="140">
              <template #default="scope">
                <el-input v-model="scope.row.specValue" :placeholder="t('wms.iqcItem.ph.specExample')" maxlength="200" />
              </template>
            </el-table-column>

            <el-table-column :label="t('wms.iqcItem.tolUpper')" align="center" width="110">
              <template #default="scope">
                <el-input-number controls-position="right" v-model="scope.row.tolUpper" :controls="false" style="width: 100%" />
              </template>
            </el-table-column>

            <el-table-column :label="t('wms.iqcItem.tolLower')" align="center" width="110">
              <template #default="scope">
                <el-input-number controls-position="right" v-model="scope.row.tolLower" :controls="false" style="width: 100%" />
              </template>
            </el-table-column>

            <el-table-column :label="t('wms.receiving.unit')" align="center" width="100">
              <template #default="scope">
                <el-select v-model="scope.row.unit" :placeholder="t('wms.receiving.unit')" clearable filterable style="width: 100%">
                  <el-option v-for="dict in wms_sys_unit_type" :key="dict.value" :label="dict.label" :value="dict.value" />
                </el-select>
              </template>
            </el-table-column>

            <el-table-column :label="t('wms.iqcItem.testMethod')" align="center" min-width="160">
              <template #default="scope">
                <el-input v-model="scope.row.testMethod" :placeholder="t('wms.iqcItem.ph.methodExample')" maxlength="200" />
              </template>
            </el-table-column>

            <el-table-column :label="t('wms.iqcItem.isCritical')" align="center" width="75">
              <template #default="scope">
                <el-switch v-model="scope.row.isCritical" active-value="1" inactive-value="0" />
              </template>
            </el-table-column>

            <el-table-column :label="t('common.status')" align="center" width="75">
              <template #default="scope">
                <el-switch v-model="scope.row.status" active-value="0" inactive-value="1" />
              </template>
            </el-table-column>

            <el-table-column :label="t('common.remark')" align="center" min-width="140">
              <template #default="scope">
                <el-input v-model="scope.row.remark" :placeholder="t('common.remark')" maxlength="500" />
              </template>
            </el-table-column>

            <el-table-column :label="t('common.actions')" align="center" width="60" fixed="right">
              <template #default="scope">
                <el-button link type="danger" icon="Delete" @click="handleDeleteRow(scope.$index)" />
              </template>
            </el-table-column>
          </el-table>

          <el-empty v-if="!loading && tableData.length === 0" :description="t('wms.iqcItem.empty')" :image-size="80" />
        </template>

        <el-empty v-else :description="t('wms.iqcItem.selectCategoryFirst')" :image-size="100" />
      </div>
    </div>
  </div>
</template>

<script setup name="WmsIqcItem">
import { ref, computed, watch, nextTick } from "vue";
import { ElMessage, ElMessageBox } from "element-plus";
import { useI18n } from "vue-i18n";
import { getDict, handleTree } from "@/utils";
import { listIqcItem, batchSaveIqcItem } from "@/api/wms/iqcItem";

const { t } = useI18n();
const { wms_material_category, wms_sys_unit_type } = getDict("wms_material_category", "wms_sys_unit_type");

// ========== 左侧分类树 ==========
const categoryTreeRef = ref(null);
const categoryName = ref("");
const selectedCategory = ref(null);

const categoryOptions = computed(() => {
  if (wms_material_category?.value && Array.isArray(wms_material_category.value) && wms_material_category.value.length > 0) {
    return handleTree(wms_material_category.value, "id", "parentId");
  }
  return [];
});

const filterNode = (value, data) => {
  if (!value) return true;
  return data.label.indexOf(value) !== -1;
};

watch(categoryName, (val) => {
  categoryTreeRef.value?.filter(val);
});

function handleNodeClick(data) {
  selectedCategory.value = data;
  loadTableData(data.value);
}

// ========== 右侧检验项表格 ==========
const loading = ref(false);
const tableData = ref([]);
const selectedRows = ref([]);

function loadTableData(categoryId) {
  loading.value = true;
  listIqcItem(categoryId)
    .then((res) => {
      const rows = (res.rows || []).map((r) => ({
        ...r,
        isCritical: r.isCritical ?? "0",
        status: r.status ?? "0",
        itemType: r.itemType ?? "",
        sortNo: r.sortNo ?? 0,
      }));
      tableData.value = rows;
    })
    .finally(() => {
      loading.value = false;
    });
}

function handleAddRow() {
  tableData.value.push({
    itemId: 0,
    categoryId: selectedCategory.value.value,
    itemCode: "",
    itemName: "",
    itemType: "",
    specValue: "",
    tolUpper: undefined,
    tolLower: undefined,
    unit: "",
    testMethod: "",
    isCritical: "0",
    sortNo: tableData.value.length + 1,
    status: "0",
    remark: "",
  });
}

function handleDeleteRow(index) {
  tableData.value.splice(index, 1);
}

function handleDeleteRows() {
  ElMessageBox.confirm(t('wms.iqcItem.deleteRowsConfirm'), t('wms.common.tip'), { type: "warning" })
    .then(() => {
      tableData.value = tableData.value.filter((r) => !selectedRows.value.includes(r));
      selectedRows.value = [];
      ElMessage.success(t('wms.common.deleteSuccess'));
    });
}

function handleSelectionChange(rows) {
  selectedRows.value = rows;
}

function handleBatchSave() {
  // 基本校验
  for (let i = 0; i < tableData.value.length; i++) {
    const row = tableData.value[i];
    if (!row.itemCode) {
      ElMessage.warning(t('wms.iqcItem.rowCodeRequired', { no: i + 1 }));
      return;
    }
    if (!row.itemName) {
      ElMessage.warning(t('wms.iqcItem.rowNameRequired', { no: i + 1 }));
      return;
    }
  }

  const categoryId = selectedCategory.value.value;
  const data = tableData.value.map((r) => {
    const { ...rest } = r;
    return {
      itemCode: rest.itemCode,
      itemName: rest.itemName,
      itemType: rest.itemType || null,
      specValue: rest.specValue || null,
      tolUpper: rest.tolUpper ?? null,
      tolLower: rest.tolLower ?? null,
      unit: rest.unit || null,
      testMethod: rest.testMethod || null,
      isCritical: rest.isCritical ?? "0",
      sortOrder: rest.sortOrder ?? 0,
      status: rest.status ?? "0",
      remark: rest.remark || null,
    };
  });

  loading.value = true;
  batchSaveIqcItem(categoryId, data)
    .then(() => {
      ElMessage.success(t('wms.common.savedSuccess'));
      loadTableData(categoryId);
    })
    .finally(() => {
      loading.value = false;
    });
}
</script>

<style scoped>
.tree-node {
  display: flex;
  align-items: center;
  gap: 6px;
  font-size: 14px;
}

.tree-node .el-icon {
  font-size: 14px;
}

.split-right__header {
  margin-bottom: 12px;
}

.split-right__title {
  font-size: 16px;
  font-weight: 600;
}

.split-right__sub {
  font-size: 13px;
  color: var(--el-text-color-secondary);
}
</style>

