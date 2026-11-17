<template>
  <div class="app-container">
    <div class="split-panel">
      <!-- 左侧物料分类树 -->
      <div class="split-left-panel">
        <div class="split-left__header">
          <div class="split-left__title">物料分类</div>
          <div class="split-left__search">
            <el-input
              v-model="categoryName"
              placeholder="搜索分类"
              clearable
              prefix-icon="Search"
            />
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
      <!-- 右侧物料列表 -->
      <div class="split-right-panel">
        <query-form
          :config="queryConfig"
          v-model:visible="showSearch"
          v-model="queryParams.params"
          @query="handleQuery"
          @reset="resetQuery"
        />

        <!-- 操作按钮 -->
        <el-row :gutter="10" class="mb8">
          <el-col :span="1.5">
            <el-button
              type="primary"
              plain
              icon="Plus"
              @click="handleAdd"
              v-hasPermi="['wms:material:add']"
              >新增</el-button
            >
          </el-col>
          <el-col :span="1.5">
            <el-button
              type="success"
              plain
              icon="Edit"
              :disabled="single"
              @click="handleUpdate"
              v-hasPermi="['wms:material:edit']"
              >修改</el-button
            >
          </el-col>
          <el-col :span="1.5">
            <el-button
              type="danger"
              plain
              icon="Delete"
              :disabled="multiple"
              @click="handleDelete"
              v-hasPermi="['wms:material:remove']"
              >删除</el-button
            >
          </el-col>
          <right-toolbar
            v-model:showSearch="showSearch"
            @queryTable="getList"
          ></right-toolbar>
        </el-row>

        <!-- 列表 -->
        <el-table
          v-loading="loading"
          :data="materialList"
          :cell-style="fixedCellBg"
          :header-cell-style="fixedCellBg"
          @selection-change="handleSelectionChange"
        >
          <el-table-column type="selection" width="55" align="center" />
          <el-table-column
            label="物料编码"
            align="center"
            prop="materialCode"
            :show-overflow-tooltip="true"
            min-width="120"
          />
          <el-table-column
            label="物料名称"
            align="center"
            prop="materialName"
            :show-overflow-tooltip="true"
            min-width="180"
          />
          <el-table-column
            label="物料类型"
            align="center"
            prop="materialType"
            width="100"
          >
            <template #default="scope">
              <dict-tag :options="wms_material_type" :value="scope.row.materialType" />
            </template>
          </el-table-column>
          <el-table-column
            label="物料分类"
            align="center"
            prop="categoryId"
            :show-overflow-tooltip="true"
            min-width="120"
          >
            <template #default="scope">
              <span>{{ categoryLabel(scope.row.categoryId) }}</span>
            </template>
          </el-table-column>
          <el-table-column
            label="规格"
            align="center"
            prop="spec"
            :show-overflow-tooltip="true"
            min-width="120"
          />
          <el-table-column label="基本单位" align="center" prop="unit" width="90">
            <template #default="scope">
              <span>{{ unitLabel(scope.row.unit) }}</span>
            </template>
          </el-table-column>
          <el-table-column
            label="安全库存"
            align="center"
            prop="safetyStock"
            width="100"
          />
          <el-table-column label="状态" align="center" prop="status" width="80">
            <template #default="scope">
              <el-switch
                v-model="scope.row.status"
                active-value="0"
                inactive-value="1"
                @change="handleStatusChange(scope.row)"
              />
            </template>
          </el-table-column>
          <el-table-column
            label="创建时间"
            align="center"
            prop="createTime"
            min-width="170"
            :show-overflow-tooltip="true"
          />
          <el-table-column
            label="操作"
            align="center"
            width="150"
            class-name="small-padding fixed-width"
            fixed="right"
          >
            <template #default="scope">
              <el-button
                link
                type="primary"
                icon="Edit"
                @click="handleUpdate(scope.row)"
                v-hasPermi="['wms:material:edit']"
                >修改</el-button
              >
              <el-button
                link
                type="danger"
                icon="Delete"
                @click="handleDelete(scope.row)"
                v-hasPermi="['wms:material:remove']"
                >删除</el-button
              >
            </template>
          </el-table-column>
          <el-table-column label="默认仓库" align="center" :show-overflow-tooltip="true">
            <template #default="scope">
              {{ scope.row.defaultWarehouse?.warehouseName || "-" }}
            </template>
          </el-table-column>
          <el-table-column label="默认库区" align="center" :show-overflow-tooltip="true">
            <template #default="scope">
              {{ scope.row.defaultZone?.zoneCode || "-" }}
            </template>
          </el-table-column>
          <el-table-column label="默认库位" align="center" :show-overflow-tooltip="true">
            <template #default="scope">
              {{ scope.row.defaultLocation?.locationCode || "-" }}
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
      </div>
    </div>

    <!-- 添加或修改物料对话框 -->
    <el-dialog :title="title" v-model="open" width="80%" append-to-body @close="cancel">
      <el-tabs v-model="activeTab">
        <el-tab-pane label="基本信息" name="base">
          <el-form ref="materialRef" :model="form" :rules="rules" label-width="auto">
            <el-row :gutter="20">
              <el-col :span="12">
                <el-form-item label="物料编码" prop="materialCode">
                  <el-input v-model="form.materialCode" placeholder="请输入物料编码" maxlength="50" />
                </el-form-item>
              </el-col>
              <el-col :span="12">
                <el-form-item label="物料名称" prop="materialName">
                  <el-input v-model="form.materialName" placeholder="请输入物料名称" maxlength="100" />
                </el-form-item>
              </el-col>
            </el-row>
            <el-row :gutter="20">
              <el-col :span="12">
                <el-form-item label="物料类型" prop="materialType">
                  <el-select v-model="form.materialType" placeholder="请选择" style="width: 100%">
                    <el-option v-for="dict in wms_material_type" :key="dict.value" :label="dict.label" :value="dict.value" />
                  </el-select>
                </el-form-item>
              </el-col>
              <el-col :span="12">
                <el-form-item label="物料分类" prop="categoryId">
                  <DictSelect v-model="form.categoryId" :options="wms_material_category" placeholder="请选择物料分类" style="width: 100%" />
                </el-form-item>
              </el-col>
            </el-row>
            <el-row :gutter="20">
              <el-col :span="12">
                <el-form-item label="包装规格" prop="packSpecId">
                  <el-select v-model="form.packSpecId" placeholder="请选择包装规格（可选）" clearable filterable style="width: 100%">
                    <el-option v-for="ps in packSpecOptions" :key="ps.specId" :label="ps.levelName" :value="ps.specId" />
                  </el-select>
                </el-form-item>
              </el-col>
              <el-col :span="12">
                <el-form-item label="规格" prop="spec">
                  <el-input v-model="form.spec" placeholder="请输入规格" maxlength="100" />
                </el-form-item>
              </el-col>
            </el-row>
            <el-row :gutter="20">
              <el-col :span="12">
                <el-form-item label="型号" prop="model">
                  <el-input v-model="form.model" placeholder="请输入型号" maxlength="100" />
                </el-form-item>
              </el-col>
              <el-col :span="12">
                <el-form-item label="基本单位" prop="unit">
                  <el-select v-model="form.unit" placeholder="请选择基本单位" clearable filterable style="width: 100%">
                    <el-option v-for="dict in wms_sys_unit_type" :key="dict.value" :label="dict.label" :value="dict.value" />
                  </el-select>
                </el-form-item>
              </el-col>
            </el-row>
            <el-row :gutter="20">
              <el-col :span="12">
                <el-form-item label="条码" prop="barcode">
                  <el-input v-model="form.barcode" placeholder="请输入条码" maxlength="50" />
                </el-form-item>
              </el-col>
              <el-col :span="12">
                <el-form-item label="安全库存" prop="safetyStock">
                  <el-input-number controls-position="right" v-model="form.safetyStock" :min="0" :precision="2" :controls="false" style="width: 100%" />
                </el-form-item>
              </el-col>
            </el-row>
            <el-row :gutter="20">
              <el-col :span="12">
                <el-form-item label="状态" prop="status">
                  <el-radio-group v-model="form.status">
                    <el-radio value="0">启用</el-radio>
                    <el-radio value="1">停用</el-radio>
                  </el-radio-group>
                </el-form-item>
              </el-col>
              <el-col :span="12">
                <el-form-item label="备注" prop="remark">
                  <el-input v-model="form.remark" placeholder="请输入备注" maxlength="500" />
                </el-form-item>
              </el-col>
            </el-row>
            <el-divider content-position="left">默认库位</el-divider>
            <el-row :gutter="20">
              <el-col :span="12">
                <el-form-item label="默认库位" prop="defaultWarehouseId">
                  <LocationCascader v-model="cascaderValue" placeholder="请选择仓库/库区/库位" />
                </el-form-item>
              </el-col>
            </el-row>
          </el-form>
        </el-tab-pane>
        <el-tab-pane label="供应商" name="supplier">
          <template v-if="activeTab === 'supplier'">
          <div class="mb8">
            <el-button type="primary" plain icon="Plus" @click="handleAddSupplier"
              >新增</el-button
            >
            <el-button
              type="danger"
              plain
              icon="Delete"
              :disabled="supplierIds.length === 0"
              @click="handleDeleteSupplier"
              >删除</el-button
            >
          </div>
          <el-table
            :data="form.suppliers"
            ref="supplierTableRef"
            row-key="_rowKey"
            :row-class-name="supplierRowClass"
            @selection-change="handleSupplierSelectionChange"
            border
          >
          
            <el-table-column type="expand" width="30">
              <template #default="scope">
                <div v-if="scope.row.status === '0' && scope.row.inspectionMode && scope.row.inspectionMode !== 'exempt'" class="inspection-expand">
                  <el-descriptions :column="3" border>
                    <el-descriptions-item label="AQL等级">
                      <el-input v-model="scope.row.aqlLevel" placeholder="如 1.0/2.5/4.0" maxlength="20" clearable />
                    </el-descriptions-item>
                    <el-descriptions-item label="抽检比例(%)" :span="2">
                      <el-input-number controls-position="right" v-model="scope.row.sampleRatio" :min="0" :max="100" :precision="2" :controls="false" style="width:100%" />
                    </el-descriptions-item>
                    <el-descriptions-item label="严重缺陷上限">
                      <el-input-number controls-position="right" v-model="scope.row.criticalQty" :min="0" :controls="false" style="width:100%" />
                    </el-descriptions-item>
                    <el-descriptions-item label="主要缺陷上限">
                      <el-input-number controls-position="right" v-model="scope.row.majorQty" :min="0" :controls="false" style="width:100%" />
                    </el-descriptions-item>
                    <el-descriptions-item label="次要缺陷上限">
                      <el-input-number controls-position="right" v-model="scope.row.minorQty" :min="0" :controls="false" style="width:100%" />
                    </el-descriptions-item>
                  </el-descriptions>
                </div>
              </template>
            </el-table-column>
            
            <el-table-column type="selection" width="50" align="center" />

            <el-table-column label="供应商" align="center" min-width="220" :show-overflow-tooltip="true">
              <template #default="scope">
                <PartnerSelect
                  v-model="scope.row.supplierId"
                  :label="scope.row.supplierName"
                  partner-type="supplier"
                  placeholder="点击选择供应商"
                  @change="onSupplierChange(scope.row, $event)"
                />
              </template>
            </el-table-column>
            <el-table-column label="最小起订量" align="center">
              <template #default="scope">
                <el-input-number controls-position="right"
                  v-model="scope.row.moq"
                  :min="0"
                  :precision="2"
                  :controls="false"
                  style="width: 100%"
                />
              </template>
            </el-table-column>
            <el-table-column label="交货周期(天)" align="center">
              <template #default="scope">
                <el-input-number controls-position="right"
                  v-model="scope.row.leadTime"
                  :min="0"
                  :controls="false"
                  style="width: 100%"
                />
              </template>
            </el-table-column>
            <el-table-column label="默认" align="center" width="70">
              <template #default="scope">
                <el-switch
                  v-model="scope.row.isDefault"
                  active-value="1"
                  inactive-value="0"
                />
              </template>
            </el-table-column>
            <el-table-column label="状态" align="center" width="70">
              <template #default="scope">
                <el-switch
                  v-model="scope.row.status"
                  active-value="0"
                  inactive-value="1"
                />
              </template>
            </el-table-column>
            <el-table-column label="质检模式" align="center" width="100">
              <template #default="scope">
                <el-select v-model="scope.row.inspectionMode" placeholder="免检" style="width:100%" @change="onInspectionModeChange(scope.row)">
                  <el-option label="全检" value="all" />
                  <el-option label="抽检" value="sampling" />
                  <el-option label="免检" value="exempt" />
                </el-select>
              </template>
            </el-table-column>
            <el-table-column label="备注" align="center" :show-overflow-tooltip="true">
              <template #default="scope">
                <el-input v-model="scope.row.remark" placeholder="备注" maxlength="500" />
              </template>
            </el-table-column>
          </el-table>
          </template>
        </el-tab-pane>
      </el-tabs>
      <template #footer>
        <div class="dialog-footer">
          <el-button type="primary" @click="submitForm">确 定</el-button>
          <el-button @click="cancel">取 消</el-button>
        </div>
      </template>
    </el-dialog>
  </div>
</template>

<script setup name="WmsMaterial">
import { ref, reactive, toRefs, computed, onMounted, nextTick, watch } from "vue";
import { ElMessage, ElMessageBox } from "element-plus";
import DictTag from "@/components/DictTag/index.vue";
import DictSelect from "@/components/DictSelect";
import { getDict, handleTree } from "@/utils";
import {
  listMaterial,
  getMaterial,
  addMaterial,
  updateMaterial,
  delMaterial,
  changeMaterialStatus,
} from "@/api/wms/material";
import { listPackSpecTop } from "@/api/wms/packSpec";
import QueryForm from "@/components/QueryForm/index.vue";
import RightToolbar from "@/components/RightToolbar/index.vue";
import LocationCascader from "@/views/wms/location/cascader.vue";
import PartnerSelect from "@/views/wms/partner/input.vue";

// 字典：物料类型、计量单位、物料分类
const { wms_material_type, wms_sys_unit_type, wms_material_category } = getDict(
  "wms_material_type",
  "wms_sys_unit_type",
  "wms_material_category"
);

/** 根据字典值返回单位标签 */
function unitLabel(val) {
  if (val === undefined || val === null || val === "") return "-";
  const item = wms_sys_unit_type.value.find((d) => d.value === val);
  return item ? item.label : val;
}

/** 根据字典值返回物料分类标签 */
function categoryLabel(val) {
  if (val === undefined || val === null || val === "") return "-";
  // 从树形字典扁平查找
  const findLabel = (list, v) => {
    for (const item of list) {
      if (item.value === v) return item.label;
      if (item.children && item.children.length) {
        const found = findLabel(item.children, v);
        if (found) return found;
      }
    }
    return null;
  };
  return findLabel(wms_material_category.value, val) || val;
}

/** 物料分类树数据 */
const categoryTreeRef = ref(null);
const categoryName = ref("");

const categoryOptions = computed(() => {
  if (
    wms_material_category?.value &&
    Array.isArray(wms_material_category.value) &&
    wms_material_category.value.length > 0
  ) {
    return handleTree(wms_material_category.value, "id", "parentId");
  }
  return [];
});

/** 过滤树节点 */
const filterNode = (value, data) => {
  if (!value) return true;
  return data.label.indexOf(value) !== -1;
};

watch(categoryName, (val) => {
  categoryTreeRef.value?.filter(val);
});

/** 树节点点击 */
function handleNodeClick(data) {
  const clickedKey = data.value;
  const isSame = queryParams.value.params.categoryId === clickedKey;
  queryParams.value.params.categoryId = isSame ? undefined : clickedKey;
  categoryTreeRef.value?.setCurrentKey(isSame ? null : clickedKey);
  handleQuery();
}

const materialRef = ref(null);
const supplierTableRef = ref(null);
const materialList = ref([]);
const open = ref(false);

const loading = ref(true);
const ids = ref([]);
const single = ref(true);
const multiple = ref(true);
const total = ref(0);
const title = ref("");
const activeTab = ref("base");

const showSearch = ref(true);

const data = reactive({
  form: {},
  queryParams: {
    pageNum: 1,
    pageSize: 10,
    params: {
      keywords: undefined,
      materialType: undefined,
      categoryId: undefined,
    },
  },
  rules: {
    materialCode: [{ required: true, message: "物料编码不能为空", trigger: "blur" }],
    materialName: [{ required: true, message: "物料名称不能为空", trigger: "blur" }],
    materialType: [{ required: true, message: "请选择物料类型", trigger: "change" }],
    categoryId: [{ required: true, message: "请选择物料分类", trigger: "change" }],
  },
});

const { form, queryParams, rules } = toRefs(data);

/** 级联选择器：数组 ↔ 三个独立字段的双向桥接 */
const cascaderValue = computed({
  get() {
    const ids = [form.value.defaultWarehouseId, form.value.defaultZoneId, form.value.defaultLocationId];
    return ids.filter((id) => id !== undefined && id !== null && id !== "");
  },
  set(val) {
    form.value.defaultWarehouseId = val?.[0] || undefined;
    form.value.defaultZoneId = val?.[1] || undefined;
    form.value.defaultLocationId = val?.[2] || undefined;
  },
});

const queryConfig = computed(() => [
  {
    label: "物料编码/名称",
    prop: "keywords",
    type: "input",
    placeholder: "请输入物料编码或名称",
  },
  {
    label: "物料类型",
    prop: "materialType",
    type: "select",
    placeholder: "请选择物料类型",
    options: wms_material_type?.value || [],
  },
]);

onMounted(() => {
  getList();
  loadPackSpecOptions();
});

/** 查询物料列表 */
function getList() {
  loading.value = true;
  listMaterial(queryParams.value).then((res) => {
    materialList.value = res.rows || [];
    total.value = res.total || 0;
    loading.value = false;
  });
}

/** 标签页重新激活时由布局回调（是否刷新由页面决定） */
function tabRefresh() {
  getList();
}

/** 搜索 */
function handleQuery() {
  queryParams.value.pageNum = 1;
  getList();
}

/** 重置 */
function resetQuery() {
  queryParams.value.params = {
    keywords: undefined,
    materialType: undefined,
    categoryId: undefined,
  };
  categoryTreeRef.value?.setCurrentKey(null);
  handleQuery();
}

/** 表单重置 */
function reset() {
  form.value = {
    materialId: undefined,
    materialCode: undefined,
    materialName: undefined,
    materialType: undefined,
    spec: undefined,
    model: undefined,
    unit: undefined,
    barcode: undefined,
    safetyStock: undefined,
    packSpecId: undefined,
    status: "0",
    remark: undefined,
    defaultWarehouseId: undefined,
    defaultZoneId: undefined,
    defaultLocationId: undefined,
    suppliers: [],
  };
  if (materialRef.value) materialRef.value.resetFields();
}

/** 加载顶层包装规格（供物料选择，多物料可共用同一套） */
const packSpecOptions = ref([]);
function loadPackSpecOptions() {
  listPackSpecTop({ pageSize: 0 }).then((res) => {
    packSpecOptions.value = res.rows || [];
  });
}

/** 新增 */
function handleAdd() {
  reset();
  resetSupplier();
  loadPackSpecOptions();
  open.value = true;
  title.value = "添加物料";
}

/** 修改 */
function handleUpdate(row) {
  reset();
  resetSupplier();
  const materialId = row.materialId || ids.value[0];
  getMaterial(materialId).then((res) => {
    const data = res.data || {};
    // debugger
    // const { suppliers, defaultWarehouse, defaultZone, ...rest } = data;
    // Object.assign(form.value, rest);
    form.value = data;
    // 处理 suppliers：转换 isDefault/status 为字符串、回填 supplierName
    if (data.suppliers?.length) {
      data.suppliers.forEach((r) => {
        r._rowKey = r.materialSupplierId ? `s_${r.materialSupplierId}` : `new_${r._tempId || Date.now()}`;
        r.isDefault = r.isDefault != null ? String(r.isDefault) : "0";
        r.status = r.status != null ? String(r.status) : "0";
        r.inspectionMode = r.inspectionMode || "exempt";
        if (r.supplier) {
          r.supplierName = r.supplier.partnerName;
        }
      });
      // 有效的排前面，无效的排后面
      data.suppliers.sort((a, b) => {
        if (a.status === '0' && b.status !== '0') return -1;
        if (a.status !== '0' && b.status === '0') return 1;
        return 0;
      });
    }
    loadPackSpecOptions();
    open.value = true;
    title.value = "修改物料";
    // 有效且非免检的供应商自动展开
    nextTick(() => {
      if (!supplierTableRef.value || !data.suppliers?.length) return;
      data.suppliers.forEach((r) => {
        if (r.status === '0' && r.inspectionMode && r.inspectionMode !== 'exempt') {
          supplierTableRef.value.toggleRowExpansion(r, true);
        }
      });
    });
  });
}

/** 提交 */
async function submitForm() {
  const valid = await materialRef.value.validate().catch(() => false);
  if (!valid) return;
  const isAdd = !form.value.materialId;
  if (isAdd) {
    await addMaterial(form.value);
  } else {
    await updateMaterial(form.value);
  }
  ElMessage.success("保存成功");
  open.value = false;
  getList();
}

/** 取消 */
function cancel() {
  open.value = false;
  reset();
  resetSupplier();
}

// ========== 供应商 Tab ==========
const supplierIds = ref([]);

function resetSupplier() {
  activeTab.value = "base";
  supplierIds.value = [];
}

/** 供应商选择回调 */
function onSupplierChange(row, data) {
  row.supplierId = data.partnerId;
  row.supplierName = data.partnerName;
  row.supplier = data;
}

/** 隐藏免检/无效供应商的展开箭头 */
function supplierRowClass({ row }) {
  if (row.status !== '0' || !row.inspectionMode || row.inspectionMode === 'exempt') {
    return 'row-no-expand';
  }
  return '';
}

/** 质检模式切换回调 */
function onInspectionModeChange(row) {
  if (!supplierTableRef.value) return;
  const key = row._rowKey || row.materialSupplierId || row._tempId;
  if (!key) return;
  if (row.status === '0' && row.inspectionMode && row.inspectionMode !== 'exempt') {
    supplierTableRef.value.toggleRowExpansion(row, true);
  } else {
    supplierTableRef.value.toggleRowExpansion(row, false);
    // 切回免检时清空旧值
    row.aqlLevel = undefined;
    row.sampleRatio = undefined;
    row.criticalQty = undefined;
    row.majorQty = undefined;
    row.minorQty = undefined;
  }
}

/** 新增供应商行 */
let supplierTempId = 0;
function handleAddSupplier() {
  supplierTempId++;
  form.value.suppliers.push({
    _tempId: `new_${supplierTempId}`,
    _rowKey: `new_${supplierTempId}`,
    materialSupplierId: undefined,
    materialId: form.value.materialId,
    supplierId: undefined,
    supplierName: undefined,
    moq: undefined,
    leadTime: undefined,
    isDefault: "0",
    status: "0",
    inspectionMode: "exempt",
    remark: undefined,
  });
}

/** 删除选中的供应商 */
function handleDeleteSupplier() {
  if (supplierIds.value.length === 0) {
    ElMessage.warning("请选择要删除的供应商");
    return;
  }
  const suppliers = form.value.suppliers;
  const toDelete = suppliers.filter(
    (r) => r.materialSupplierId && supplierIds.value.includes(r.materialSupplierId)
  );
  const toRemove = suppliers.filter(
    (r) => !r.materialSupplierId && r._tempId && supplierIds.value.includes(r._tempId)
  );
  // 已持久化的先标记删除，统一在 submitForm 时提交
  if (toDelete.length > 0) {
    const delIds = toDelete.map((r) => r.materialSupplierId);
    form.value.suppliers = suppliers.filter(
      (r) => !delIds.includes(r.materialSupplierId)
    );
    supplierIds.value = [];
    ElMessage.success("删除成功");
  }
  // 新增未保存的直接从列表移除
  if (toRemove.length > 0) {
    const tempIds = toRemove.map((r) => r._tempId);
    form.value.suppliers = suppliers.filter((r) => !tempIds.includes(r._tempId));
    supplierIds.value = [];
  }
}

/** 供应商多选 */
function handleSupplierSelectionChange(selection) {
  supplierIds.value = selection.map((i) => i.materialSupplierId || i._tempId);
}

/** 删除 */
function handleDelete(row) {
  const materialIds = row.materialId || ids.value.join(",");
  ElMessageBox.confirm(`是否确认删除物料编号为"${materialIds}"的数据项？`, "警告", {
    confirmButtonText: "确定",
    cancelButtonText: "取消",
    type: "warning",
  })
    .then(() => delMaterial(materialIds))
    .then(() => {
      getList();
      ElMessage.success("删除成功");
    });
}

/** 状态切换 */
let statusChanging = false;
function handleStatusChange(row) {
  if (statusChanging) return;
  const text = row.status === "0" ? "启用" : "停用";
  ElMessageBox.confirm(`确认要"${text}"物料"${row.materialName}"吗？`, "提示", {
    confirmButtonText: "确定",
    cancelButtonText: "取消",
    type: "warning",
  })
    .then(() => {
      changeMaterialStatus({ materialId: row.materialId, status: row.status }).then(() => {
        ElMessage.success("操作成功");
      });
    })
    .catch(() => {
      statusChanging = true;
      row.status = row.status === "0" ? "1" : "0";
      nextTick(() => { statusChanging = false; });
    });
}

/** 多选 */
function handleSelectionChange(selection) {
  ids.value = selection.map((i) => i.materialId);
  single.value = selection.length !== 1;
  multiple.value = !selection.length;
}

/** 固定列（左/右）单元格与表头强制不透明背景，避免主表行 hover 高亮与溢出文字透出、压在固定列上 */
function fixedCellBg({ column }) {
  if (column && (column.fixed === "right" || column.fixed === "left")) {
    return { backgroundColor: "var(--el-bg-color)", zIndex: 2 };
  }
  return {};
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

.inspection-expand {
  padding: 10px;
}
.inspection-expand .el-input-number,
.inspection-expand .el-input {
  width: 100%;
}

/* 免检/无效供应商隐藏展开箭头 */
.el-table .row-no-expand .el-table__expand-icon {
  visibility: hidden;
}
.el-table .row-no-expand .el-table__indent {
  display: none;
}
</style>
