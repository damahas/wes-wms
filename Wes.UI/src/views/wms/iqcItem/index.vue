<template>
  <div class="app-container">
    <div class="split-panel">
      <!-- 左侧物料分类树 -->
      <div class="split-left-panel">
        <div class="split-left__header">
          <div class="split-left__title">物料分类</div>
          <div class="split-left__search">
            <el-input v-model="categoryName" placeholder="搜索分类" clearable prefix-icon="Search" />
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
            <span class="split-right__sub">（检验项配置）</span>
          </div>

          <el-row :gutter="10" class="mb8">
            <el-col :span="2">
              <el-button class="detail-add-btn" type="primary" plain icon="Plus" @click="handleAddRow">新增行</el-button>
            </el-col>
            <el-col :span="1.5">
              <el-button type="danger" plain icon="Delete" :disabled="selectedRows.length === 0" @click="handleDeleteRows">删除选中</el-button>
            </el-col>
            <el-col :span="1.5">
              <el-button type="success" icon="Check" @click="handleBatchSave">保存</el-button>
            </el-col>
          </el-row>

          <el-table
            v-loading="loading"
            :data="tableData"
            @selection-change="handleSelectionChange"
            border
          >
            <el-table-column type="selection" width="55" align="center" />

            <el-table-column label="序号" align="center" width="80">
              <template #default="scope">
                <el-input-number controls-position="right"
                  v-model="scope.row.sortNo"
                  :min="0"
                  :controls="false"
                  style="width: 100%"
                />
              </template>
            </el-table-column>

            <el-table-column label="检验项编码" align="center" min-width="140">
              <template #default="scope">
                <el-input v-model="scope.row.itemCode" placeholder="编码" maxlength="50" />
              </template>
            </el-table-column>

            <el-table-column label="检验项名称" align="center" min-width="160">
              <template #default="scope">
                <el-input v-model="scope.row.itemName" placeholder="名称" maxlength="100" />
              </template>
            </el-table-column>

            <el-table-column label="检验类型" align="center" width="120">
              <template #default="scope">
                <el-select v-model="scope.row.itemType" placeholder="请选择" style="width: 100%">
                  <el-option label="计量" value="quantitative" />
                  <el-option label="定性" value="qualitative" />
                  <el-option label="目视" value="visual" />
                </el-select>
              </template>
            </el-table-column>

            <el-table-column label="规格/标准值" align="center" min-width="140">
              <template #default="scope">
                <el-input v-model="scope.row.specValue" placeholder="如 10±0.1" maxlength="200" />
              </template>
            </el-table-column>

            <el-table-column label="上限公差" align="center" width="110">
              <template #default="scope">
                <el-input-number controls-position="right" v-model="scope.row.tolUpper" :controls="false" style="width: 100%" />
              </template>
            </el-table-column>

            <el-table-column label="下限公差" align="center" width="110">
              <template #default="scope">
                <el-input-number controls-position="right" v-model="scope.row.tolLower" :controls="false" style="width: 100%" />
              </template>
            </el-table-column>

            <el-table-column label="单位" align="center" width="100">
              <template #default="scope">
                <el-select v-model="scope.row.unit" placeholder="单位" clearable filterable style="width: 100%">
                  <el-option v-for="dict in wms_sys_unit_type" :key="dict.value" :label="dict.label" :value="dict.value" />
                </el-select>
              </template>
            </el-table-column>

            <el-table-column label="检测方法/工具" align="center" min-width="160">
              <template #default="scope">
                <el-input v-model="scope.row.testMethod" placeholder="如 卡尺测量 / 游标卡尺" maxlength="200" />
              </template>
            </el-table-column>

            <el-table-column label="关键项" align="center" width="75">
              <template #default="scope">
                <el-switch v-model="scope.row.isCritical" active-value="1" inactive-value="0" />
              </template>
            </el-table-column>

            <el-table-column label="状态" align="center" width="75">
              <template #default="scope">
                <el-switch v-model="scope.row.status" active-value="0" inactive-value="1" />
              </template>
            </el-table-column>

            <el-table-column label="备注" align="center" min-width="140">
              <template #default="scope">
                <el-input v-model="scope.row.remark" placeholder="备注" maxlength="500" />
              </template>
            </el-table-column>

            <el-table-column label="操作" align="center" width="60" fixed="right">
              <template #default="scope">
                <el-button link type="danger" icon="Delete" @click="handleDeleteRow(scope.$index)" />
              </template>
            </el-table-column>
          </el-table>

          <el-empty v-if="!loading && tableData.length === 0" description="暂无检验项，请点击新增行添加" :image-size="80" />
        </template>

        <el-empty v-else description="请在左侧选择物料分类" :image-size="100" />
      </div>
    </div>
  </div>
</template>

<script setup name="WmsIqcItem">
import { ref, computed, watch, nextTick } from "vue";
import { ElMessage, ElMessageBox } from "element-plus";
import { getDict, handleTree } from "@/utils";
import { listIqcItem, batchSaveIqcItem } from "@/api/wms/iqcItem";

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
  ElMessageBox.confirm("确认删除选中的检验项吗？", "提示", { type: "warning" })
    .then(() => {
      tableData.value = tableData.value.filter((r) => !selectedRows.value.includes(r));
      selectedRows.value = [];
      ElMessage.success("删除成功");
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
      ElMessage.warning(`第 ${i + 1} 行检验项编码不能为空`);
      return;
    }
    if (!row.itemName) {
      ElMessage.warning(`第 ${i + 1} 行检验项名称不能为空`);
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
      ElMessage.success("保存成功");
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

