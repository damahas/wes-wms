<template>
  <div class="app-container">
    <div class="main-panel p16">
      <query-form
        :config="queryConfig"
        v-model:visible="showSearch"
        v-model="queryParams"
        @query="handleQuery"
        @reset="resetQuery"
      />

    <!-- 操作按钮 -->
    <el-row :gutter="10" class="mb8">
      <el-col :span="1.5">
        <el-button type="primary" plain icon="Plus" @click="handleAdd" v-hasPermi="['wms:warehouse:add']">{{ t('common.add') }}</el-button>
      </el-col>
      <el-col :span="1.5">
        <el-button type="success" plain icon="Edit" :disabled="single" @click="handleUpdate" v-hasPermi="['wms:warehouse:edit']">{{ t('common.edit') }}</el-button>
      </el-col>
      <el-col :span="1.5">
        <el-button type="danger" plain icon="Delete" :disabled="multiple" @click="handleDelete" v-hasPermi="['wms:warehouse:remove']">{{ t('common.delete') }}</el-button>
      </el-col>
        <right-toolbar
          v-model:showSearch="showSearch"
          @queryTable="getList"
        ></right-toolbar>
    </el-row>

    <!-- 列表 -->
    <el-table v-loading="loading" :data="warehouseList" @selection-change="handleSelectionChange">
      <el-table-column type="selection" width="55" align="center" />
      <el-table-column :label="t('wms.warehouse.warehouseCode')" align="center" prop="warehouseCode" :show-overflow-tooltip="true" />
      <el-table-column :label="t('wms.warehouse.warehouseName')" align="center" prop="warehouseName" :show-overflow-tooltip="true" />
      <el-table-column :label="t('wms.warehouse.warehouseType')" align="center" prop="warehouseType">
        <template #default="scope">
          <dict-tag :options="warehouse_type" :value="scope.row.warehouseType" />
        </template>
      </el-table-column>
      <el-table-column :label="t('wms.warehouse.manager')" align="center" prop="managerName" :show-overflow-tooltip="true" />
      <el-table-column :label="t('common.status')" align="center" prop="status" width="80">
        <template #default="scope">
          <el-switch v-model="scope.row.status" active-value="0" inactive-value="1" @change="handleStatusChange(scope.row)" />
        </template>
      </el-table-column>
      <el-table-column :label="t('common.createTime')" align="center" prop="createTime" min-width="170" />
      <el-table-column :label="t('common.actions')" align="center" width="250" class-name="small-padding fixed-width">
        <template #default="scope">
          <el-button link type="primary" icon="Edit" @click="handleUpdate(scope.row)" v-hasPermi="['wms:warehouse:edit']">{{ t('common.edit') }}</el-button>
          <el-button link type="danger" icon="Delete" @click="handleDelete(scope.row)" v-hasPermi="['wms:warehouse:remove']">{{ t('common.delete') }}</el-button>
          <el-button link type="primary" icon="Grid" @click="handleLayout(scope.row)">{{ t('wms.warehouse.layout') }}</el-button>
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

    <!-- 添加或修改对话框 -->
    <el-dialog :title="title" v-model="open" width="700px" append-to-body>
      <el-form ref="warehouseRef" :model="form" :rules="rules" label-width="auto">
        <el-form-item :label="t('wms.warehouse.warehouseCode')" prop="warehouseCode">
          <el-input v-model="form.warehouseCode" :placeholder="t('wms.warehouse.ph.inputCode')" maxlength="50" />
        </el-form-item>
        <el-form-item :label="t('wms.warehouse.warehouseName')" prop="warehouseName">
          <el-input v-model="form.warehouseName" :placeholder="t('wms.warehouse.ph.inputName')" maxlength="100" />
        </el-form-item>
        <el-form-item :label="t('wms.warehouse.warehouseType')" prop="warehouseType">
          <el-select v-model="form.warehouseType" :placeholder="t('wms.warehouse.ph.selectType')" clearable style="width: 100%">
            <el-option v-for="dict in warehouse_type" :key="dict.value" :label="dict.label" :value="dict.value" />
          </el-select>
        </el-form-item>
                <el-form-item :label="t('wms.warehouse.region')">
          <el-cascader :options="regionOptions" v-model="regionArr" :placeholder="t('wms.warehouse.ph.selectRegion')" style="width: 100%" />
        </el-form-item>
        <el-form-item :label="t('wms.warehouse.address')" prop="address">
          <el-input v-model="form.address" type="textarea" :placeholder="t('wms.warehouse.ph.inputAddress')" maxlength="255" />
        </el-form-item>
        <el-form-item :label="t('wms.warehouse.manager')" prop="managerId">
          <ManagerInput v-model="form.managerId" style="width:100%" :placeholder="t('wms.warehouse.ph.selectManager')" />
        </el-form-item>
        <el-form-item :label="t('wms.warehouse.phone')" prop="phone">
          <el-input v-model="form.phone" :placeholder="t('wms.warehouse.ph.inputPhone')" maxlength="20" />
        </el-form-item>
        <el-form-item :label="t('wms.warehouse.totalArea')" prop="totalArea">
          <el-input-number controls-position="right" v-model="form.totalArea" :min="0" :precision="2" :controls="false" style="width: 100%" />
        </el-form-item>
        <el-form-item :label="t('wms.warehouse.usableArea')" prop="usableArea">
          <el-input-number controls-position="right" v-model="form.usableArea" :min="0" :precision="2" :controls="false" style="width: 100%" />
        </el-form-item>
        <el-form-item :label="t('common.status')" prop="status">
          <el-radio-group v-model="form.status">
            <el-radio value="0">{{ t('common.enable') }}</el-radio>
            <el-radio value="1">{{ t('common.disable') }}</el-radio>
          </el-radio-group>
        </el-form-item>
        <el-form-item :label="t('common.remark')" prop="remark">
          <el-input v-model="form.remark" type="textarea" :placeholder="t('common.input')" maxlength="500" />
        </el-form-item>
      </el-form>
      <template #footer>
        <div class="dialog-footer">
          <el-button type="primary" @click="submitForm">{{ t('common.submit') }}</el-button>
          <el-button @click="cancel">{{ t('common.cancel') }}</el-button>
        </div>
      </template>
    </el-dialog>

    <!-- 平面布局设计器 -->
    <layout-designer v-model="layoutOpen" :warehouse="layoutWarehouse" />
    </div>
  </div>
</template>

<script setup name="WmsWarehouse">
import { ref, reactive, toRefs, computed, onMounted } from "vue";
import { ElMessage, ElMessageBox } from "element-plus";
import { useI18n } from "vue-i18n";
import {
  listWarehouse,
  getWarehouse,
  addWarehouse,
  updateWarehouse,
  delWarehouse,
  changeWarehouseStatus,
} from "@/api/wms/warehouse";
import { treeRegion } from "@/api/wms/region";
import ManagerInput from "@/views/system/user/input.vue";
import DictTag from "@/components/DictTag/index.vue";
import QueryForm from "@/components/QueryForm/index.vue";
import RightToolbar from "@/components/RightToolbar/index.vue";
import { getDict } from "@/utils";
import LayoutDesigner from "@/views/wms/warehouse/layout/Designer.vue";

const { t } = useI18n();
const { warehouse_type } = getDict("wms_warehouse_type");

const warehouseRef = ref(null);
const warehouseList = ref([]);
const open = ref(false);
const loading = ref(true);
const ids = ref([]);
const single = ref(true);
const multiple = ref(true);
const total = ref(0);
const title = ref("");
const regionOptions = ref([]);
const regionArr = ref([]);

const showSearch = ref(true);
const layoutOpen = ref(false);
const layoutWarehouse = ref({});

const data = reactive({
  form: {},
  queryParams: {
    pageNum: 1,
    pageSize: 10,
    keywords: undefined,
    warehouseType: undefined,
    status: undefined,
  },
  rules: {
    warehouseCode: [{ required: true, message: () => t('wms.warehouse.rules.codeRequired'), trigger: "blur" }],
    warehouseName: [{ required: true, message: () => t('wms.warehouse.rules.nameRequired'), trigger: "blur" }],
  },
});

const { form, queryParams, rules } = toRefs(data);

const queryConfig = computed(() => [
  {
    label: t('wms.warehouse.keywords'),
    prop: "keywords",
    type: "input",
    placeholder: t('wms.warehouse.ph.inputKeywords'),
  },
  {
    label: t('wms.warehouse.warehouseType'),
    prop: "warehouseType",
    type: "select",
    placeholder: t('wms.warehouse.ph.selectType'),
    options: warehouse_type?.value || [],
  },
  {
    label: t('common.status'),
    prop: "status",
    type: "select",
    placeholder: t('wms.warehouse.ph.selectStatus'),
  },
]);

/** 查询列表 */
function getList() {
  loading.value = true;
  listWarehouse(queryParams.value).then((res) => {
    warehouseList.value = res.rows || [];
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
  queryParams.value = {
    keywords: undefined,
    warehouseType: undefined,
    status: undefined,
    pageNum: 1,
    pageSize: 10,
  };
  handleQuery();
}

/** 表单重置 */
function reset() {
  form.value = {
    warehouseId: undefined,
    warehouseCode: undefined,
    warehouseName: undefined,
    warehouseType: undefined,
    province: undefined,
    city: undefined,
    district: undefined,
    address: undefined,
    managerId: undefined,
    phone: undefined,
    totalArea: undefined,
    usableArea: undefined,
    status: "0",
    remark: undefined,
  };
  regionArr.value = [];
  if (warehouseRef.value) warehouseRef.value.resetFields();
}

/** 新增 */
function handleAdd() {
  reset();
  open.value = true;
  title.value = t('wms.warehouse.addTitle');
}

/** 修改 */
function handleUpdate(row) {
  reset();
  const id = row.warehouseId || ids.value[0];
  getWarehouse(id).then((res) => {
    form.value = res.data || {};
    regionArr.value = [res.data?.province, res.data?.city, res.data?.district].filter(Boolean);
    open.value = true;
    title.value = t('wms.warehouse.editTitle');
  });
}

/** 提交 */
function submitForm() {
  warehouseRef.value.validate((valid) => {
    if (!valid) return;
    form.value.province = regionArr.value?.[0];
    form.value.city = regionArr.value?.[1];
    form.value.district = regionArr.value?.[2];
    const api = form.value.warehouseId ? updateWarehouse : addWarehouse;
    api(form.value).then(() => {
      ElMessage.success(t('wms.common.savedSuccess'));
      open.value = false;
      getList();
    });
  });
}

/** 取消 */
function cancel() {
  open.value = false;
  reset();
}

/** 删除 */
function handleDelete(row) {
  const delIds = row.warehouseId || ids.value.join(",");
  ElMessageBox.confirm(t('wms.common.deleteConfirm', { id: delIds }), t('wms.common.warning'), {
    confirmButtonText: t('common.confirm'),
    cancelButtonText: t('common.cancel'),
    type: "warning",
  })
    .then(() => delWarehouse(delIds))
    .then(() => {
      getList();
      ElMessage.success(t('wms.common.deleteSuccess'));
    });
}

/** 平面布局 */
function handleLayout(row) {
  layoutWarehouse.value = row;
  layoutOpen.value = true;
}

/** 状态切换 */
function handleStatusChange(row) {
  const text = row.status === "0" ? t('common.enable') : t('common.disable');
  ElMessageBox.confirm(t('wms.common.statusConfirm', { action: text }), t('wms.common.tip'), {
    confirmButtonText: t('common.confirm'),
    cancelButtonText: t('common.cancel'),
    type: "warning",
  })
    .then(() => {
      changeWarehouseStatus({ warehouseId: row.warehouseId, status: row.status }).then(() => {
        ElMessage.success(t('wms.common.operateSuccess'));
      });
    })
    .catch(() => {
      row.status = row.status === "0" ? "1" : "0";
    });
}

onMounted(() => {
  treeRegion().then((res) => {
    regionOptions.value = res.data || res.rows || res || [];
  });
  getList();
});
</script>
