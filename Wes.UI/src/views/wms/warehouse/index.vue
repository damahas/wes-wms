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
        <el-button type="primary" plain icon="Plus" @click="handleAdd" v-hasPermi="['wms:warehouse:add']">新增</el-button>
      </el-col>
      <el-col :span="1.5">
        <el-button type="success" plain icon="Edit" :disabled="single" @click="handleUpdate" v-hasPermi="['wms:warehouse:edit']">修改</el-button>
      </el-col>
      <el-col :span="1.5">
        <el-button type="danger" plain icon="Delete" :disabled="multiple" @click="handleDelete" v-hasPermi="['wms:warehouse:remove']">删除</el-button>
      </el-col>
        <right-toolbar
          v-model:showSearch="showSearch"
          @queryTable="getList"
        ></right-toolbar>
    </el-row>

    <!-- 列表 -->
    <el-table v-loading="loading" :data="warehouseList" @selection-change="handleSelectionChange">
      <el-table-column type="selection" width="55" align="center" />
      <el-table-column label="仓库编码" align="center" prop="warehouseCode" :show-overflow-tooltip="true" />
      <el-table-column label="仓库名称" align="center" prop="warehouseName" :show-overflow-tooltip="true" />
      <el-table-column label="仓库类型" align="center" prop="warehouseType">
        <template #default="scope">
          <dict-tag :options="warehouse_type" :value="scope.row.warehouseType" />
        </template>
      </el-table-column>
      <el-table-column label="负责人" align="center" prop="managerName" :show-overflow-tooltip="true" />
      <el-table-column label="状态" align="center" prop="status" width="80">
        <template #default="scope">
          <el-switch v-model="scope.row.status" active-value="0" inactive-value="1" @change="handleStatusChange(scope.row)" />
        </template>
      </el-table-column>
      <el-table-column label="创建时间" align="center" prop="createTime" min-width="170" />
      <el-table-column label="操作" align="center" width="250" class-name="small-padding fixed-width">
        <template #default="scope">
          <el-button link type="primary" icon="Edit" @click="handleUpdate(scope.row)" v-hasPermi="['wms:warehouse:edit']">修改</el-button>
          <el-button link type="danger" icon="Delete" @click="handleDelete(scope.row)" v-hasPermi="['wms:warehouse:remove']">删除</el-button>
          <el-button link type="primary" icon="Grid" @click="handleLayout(scope.row)">平面布局</el-button>
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
        <el-form-item label="仓库编码" prop="warehouseCode">
          <el-input v-model="form.warehouseCode" placeholder="请输入仓库编码" maxlength="50" />
        </el-form-item>
        <el-form-item label="仓库名称" prop="warehouseName">
          <el-input v-model="form.warehouseName" placeholder="请输入仓库名称" maxlength="100" />
        </el-form-item>
        <el-form-item label="仓库类型" prop="warehouseType">
          <el-select v-model="form.warehouseType" placeholder="请选择仓库类型" clearable style="width: 100%">
            <el-option v-for="dict in warehouse_type" :key="dict.value" :label="dict.label" :value="dict.value" />
          </el-select>
        </el-form-item>
                <el-form-item label="省/市/区">
          <el-cascader :options="regionOptions" v-model="regionArr" placeholder="请选择省/市/区" style="width: 100%" />
        </el-form-item>
        <el-form-item label="详细地址" prop="address">
          <el-input v-model="form.address" type="textarea" placeholder="请输入详细地址" maxlength="255" />
        </el-form-item>
        <el-form-item label="仓库负责人" prop="managerId">
          <ManagerInput v-model="form.managerId" style="width:100%" placeholder="请选择仓库负责人" />
        </el-form-item>
        <el-form-item label="联系电话" prop="phone">
          <el-input v-model="form.phone" placeholder="请输入联系电话" maxlength="20" />
        </el-form-item>
        <el-form-item label="总面积" prop="totalArea">
          <el-input-number controls-position="right" v-model="form.totalArea" :min="0" :precision="2" :controls="false" style="width: 100%" />
        </el-form-item>
        <el-form-item label="可用面积" prop="usableArea">
          <el-input-number controls-position="right" v-model="form.usableArea" :min="0" :precision="2" :controls="false" style="width: 100%" />
        </el-form-item>
        <el-form-item label="状态" prop="status">
          <el-radio-group v-model="form.status">
            <el-radio value="0">启用</el-radio>
            <el-radio value="1">停用</el-radio>
          </el-radio-group>
        </el-form-item>
        <el-form-item label="备注" prop="remark">
          <el-input v-model="form.remark" type="textarea" placeholder="请输入备注" maxlength="500" />
        </el-form-item>
      </el-form>
      <template #footer>
        <div class="dialog-footer">
          <el-button type="primary" @click="submitForm">确 定</el-button>
          <el-button @click="cancel">取 消</el-button>
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
    warehouseCode: [{ required: true, message: "仓库编码不能为空", trigger: "blur" }],
    warehouseName: [{ required: true, message: "仓库名称不能为空", trigger: "blur" }],
  },
});

const { form, queryParams, rules } = toRefs(data);

const queryConfig = computed(() => [
  {
    label: "关键字",
    prop: "keywords",
    type: "input",
    placeholder: "请输入关键字",
  },
  {
    label: "仓库类型",
    prop: "warehouseType",
    type: "select",
    placeholder: "请选择仓库类型",
    options: warehouse_type?.value || [],
  },
  {
    label: "状态",
    prop: "status",
    type: "select",
    placeholder: "请选择状态",
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
  title.value = "添加仓库";
}

/** 修改 */
function handleUpdate(row) {
  reset();
  const id = row.warehouseId || ids.value[0];
  getWarehouse(id).then((res) => {
    form.value = res.data || {};
    regionArr.value = [res.data?.province, res.data?.city, res.data?.district].filter(Boolean);
    open.value = true;
    title.value = "修改仓库";
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
      ElMessage.success("保存成功");
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
  ElMessageBox.confirm(`是否确认删除编号为"${delIds}"的数据项？`, "警告", {
    confirmButtonText: "确定",
    cancelButtonText: "取消",
    type: "warning",
  })
    .then(() => delWarehouse(delIds))
    .then(() => {
      getList();
      ElMessage.success("删除成功");
    });
}

/** 平面布局 */
function handleLayout(row) {
  layoutWarehouse.value = row;
  layoutOpen.value = true;
}

/** 状态切换 */
function handleStatusChange(row) {
  const text = row.status === "0" ? "启用" : "停用";
  ElMessageBox.confirm(`确认要"${text}"该记录吗？`, "提示", {
    confirmButtonText: "确定",
    cancelButtonText: "取消",
    type: "warning",
  })
    .then(() => {
      changeWarehouseStatus({ warehouseId: row.warehouseId, status: row.status }).then(() => {
        ElMessage.success("操作成功");
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
