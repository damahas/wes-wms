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
        <el-button type="primary" plain icon="Plus" @click="handleAdd" v-hasPermi="['wms:location:add']">新增</el-button>
      </el-col>
      <el-col :span="1.5">
        <el-button type="success" plain icon="Edit" :disabled="single" @click="handleUpdate" v-hasPermi="['wms:location:edit']">修改</el-button>
      </el-col>
      <el-col :span="1.5">
        <el-button type="danger" plain icon="Delete" :disabled="multiple" @click="handleDelete" v-hasPermi="['wms:location:remove']">删除</el-button>
      </el-col>
        <right-toolbar
          v-model:showSearch="showSearch"
          @queryTable="getList"
        ></right-toolbar>
    </el-row>

    <!-- 列表 -->
    <el-table v-loading="loading" :data="locationList" @selection-change="handleSelectionChange" table-layout="fixed">
      <el-table-column type="selection" width="55" align="center" />
      <el-table-column label="仓库" align="center" :show-overflow-tooltip="true">
        <template #default="scope">{{ scope.row.warehouse?.warehouseName || "-" }}</template>
      </el-table-column>
      <el-table-column label="库区" align="center" :show-overflow-tooltip="true">
        <template #default="scope">{{ scope.row.zone?.zoneName || "-" }}</template>
      </el-table-column>
      <el-table-column label="库位编码" align="center" prop="locationCode" :show-overflow-tooltip="true" />
      <el-table-column label="库位类型" align="center" prop="locationType">
        <template #default="scope">
          <dict-tag :options="location_type" :value="scope.row.locationType" />
        </template>
      </el-table-column>
      <el-table-column label="状态" align="center" prop="locationStatus" width="80">
        <template #default="scope">
          <el-switch v-model="scope.row.locationStatus" active-value="0" inactive-value="1" @change="handleStatusChange(scope.row)" />
        </template>
      </el-table-column>
      <el-table-column label="创建时间" align="center" prop="createTime" min-width="170" />
      <el-table-column label="操作" align="center" width="150" class-name="small-padding fixed-width">
        <template #default="scope">
          <el-button link type="primary" icon="Edit" @click="handleUpdate(scope.row)" v-hasPermi="['wms:location:edit']">修改</el-button>
          <el-button link type="danger" icon="Delete" @click="handleDelete(scope.row)" v-hasPermi="['wms:location:remove']">删除</el-button>
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
      <el-form ref="locationRef" :model="form" :rules="rules" label-width="auto">
        <el-form-item label="仓库" prop="warehouseId">
          <el-select v-model="form.warehouseId" placeholder="请选择仓库" @change="onWarehouseChange" style="width: 100%">
            <el-option v-for="w in warehouseOptions" :key="w.warehouseId" :label="w.warehouseName" :value="w.warehouseId" />
          </el-select>
        </el-form-item>
        <el-form-item label="库区" prop="zoneId">
          <el-select v-model="form.zoneId" placeholder="请选择库区" style="width: 100%">
            <el-option v-for="z in zoneOptions" :key="z.zoneId" :label="z.zoneName" :value="z.zoneId" />
          </el-select>
        </el-form-item>
        <el-form-item label="库位编码" prop="locationCode">
          <el-input v-model="form.locationCode" placeholder="请输入库位编码" maxlength="50" />
        </el-form-item>
        <el-form-item label="库位名称" prop="locationName">
          <el-input v-model="form.locationName" placeholder="请输入库位名称" maxlength="100" />
        </el-form-item>
        <el-form-item label="库位类型" prop="locationType">
          <el-select v-model="form.locationType" placeholder="请选择库位类型" clearable style="width: 100%">
            <el-option v-for="dict in location_type" :key="dict.value" :label="dict.label" :value="dict.value" />
          </el-select>
        </el-form-item>
        <el-form-item label="库位属性" prop="locationAttr">
          <el-input v-model="form.locationAttr" placeholder="请输入库位属性" maxlength="50" />
        </el-form-item>
        <el-form-item label="货架编号" prop="shelfCode">
          <el-input v-model="form.shelfCode" placeholder="请输入货架编号" maxlength="20" />
        </el-form-item>
        <el-form-item label="长度" prop="length">
          <el-input-number controls-position="right" v-model="form.length" :min="0" :precision="2" :controls="false" style="width: 100%" />
        </el-form-item>
        <el-form-item label="宽度" prop="width">
          <el-input-number controls-position="right" v-model="form.width" :min="0" :precision="2" :controls="false" style="width: 100%" />
        </el-form-item>
        <el-form-item label="高度" prop="height">
          <el-input-number controls-position="right" v-model="form.height" :min="0" :precision="2" :controls="false" style="width: 100%" />
        </el-form-item>
        <el-form-item label="最大承重" prop="maxWeight">
          <el-input-number controls-position="right" v-model="form.maxWeight" :min="0" :precision="2" :controls="false" style="width: 100%" />
        </el-form-item>
        <el-form-item label="最大容量" prop="maxVolume">
          <el-input-number controls-position="right" v-model="form.maxVolume" :min="0" :precision="2" :controls="false" style="width: 100%" />
        </el-form-item>
        <el-form-item label="最大存储数量" prop="maxQuantity">
          <el-input-number controls-position="right" v-model="form.maxQuantity" :min="0" :controls="false" style="width: 100%" />
        </el-form-item>
        <el-form-item label="库位状态" prop="locationStatus">
          <el-radio-group v-model="form.locationStatus">
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
    </div>
  </div>
</template>

<script setup name="WmsLocation">
import { ref, reactive, toRefs, computed } from "vue";
import { ElMessage, ElMessageBox } from "element-plus";
import {
  listLocation,
  getLocation,
  addLocation,
  updateLocation,
  delLocation,
  changeLocationStatus,
} from "@/api/wms/location";
import { listWarehouse } from "@/api/wms/warehouse";
import { listZone } from "@/api/wms/zone";
import DictTag from "@/components/DictTag/index.vue";
import QueryForm from "@/components/QueryForm/index.vue";
import RightToolbar from "@/components/RightToolbar/index.vue";
import { getDict } from "@/utils";

const { location_type } = getDict("wms_location_type");

const locationRef = ref(null);
const locationList = ref([]);
const open = ref(false);
const loading = ref(true);
const ids = ref([]);
const single = ref(true);
const multiple = ref(true);
const total = ref(0);
const title = ref("");
const warehouseOptions = ref([]);
const zoneOptions = ref([]);

const showSearch = ref(true);

const data = reactive({
  form: {},
  queryParams: {
    pageNum: 1,
    pageSize: 10,
    keywords: undefined,
  },
  rules: {
    locationCode: [{ required: true, message: "库位编码不能为空", trigger: "blur" }],
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
]);

/** 获取仓库下拉数据 */
function fetchWarehouses() {
  listWarehouse({ pageSize: 0 }).then((res) => {
    warehouseOptions.value = res.rows || [];
  });
}

/** 获取库区下拉数据（按仓库过滤） */
function fetchZones(warehouseId) {
  listZone({ pageSize: 0, params: { warehouseId } }).then((res) => {
    zoneOptions.value = res.rows || [];
  });
}

/** 仓库变更时清空库区并重新加载 */
function onWarehouseChange(val) {
  form.value.zoneId = undefined;
  if (val) {
    fetchZones(val);
  } else {
    zoneOptions.value = [];
  }
}

/** 查询列表 */
function getList() {
  loading.value = true;
  listLocation(queryParams.value).then((res) => {
    locationList.value = res.rows || [];
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
    pageNum: 1,
    pageSize: 10,
  };
  handleQuery();
}

/** 表单重置 */
function reset() {
  form.value = {
    locationId: undefined,
    warehouseId: undefined,
    zoneId: undefined,
    locationCode: undefined,
    locationName: undefined,
    locationType: undefined,
    locationAttr: undefined,
    shelfCode: undefined,
    length: undefined,
    width: undefined,
    height: undefined,
    maxWeight: undefined,
    maxVolume: undefined,
    maxQuantity: undefined,
    locationStatus: "0",
    remark: undefined,
  };
  zoneOptions.value = [];
  if (locationRef.value) locationRef.value.resetFields();
}

/** 新增 */
function handleAdd() {
  reset();
  open.value = true;
  title.value = "添加库位";
}

/** 修改 */
function handleUpdate(row) {
  reset();
  const id = row.locationId || ids.value[0];
  getLocation(id).then((res) => {
    form.value = res.data || {};
    // 加载该仓库下的库区列表
    if (form.value.warehouseId) {
      fetchZones(form.value.warehouseId);
    }
    open.value = true;
    title.value = "修改库位";
  });
}

/** 提交 */
function submitForm() {
  locationRef.value.validate((valid) => {
    if (!valid) return;
    const api = form.value.locationId ? updateLocation : addLocation;
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
  const delIds = row.locationId || ids.value.join(",");
  ElMessageBox.confirm(`是否确认删除编号为"${delIds}"的数据项？`, "警告", {
    confirmButtonText: "确定",
    cancelButtonText: "取消",
    type: "warning",
  })
    .then(() => delLocation(delIds))
    .then(() => {
      getList();
      ElMessage.success("删除成功");
    });
}

/** 状态切换 */
function handleStatusChange(row) {
  changeLocationStatus({ locationId: row.locationId, locationStatus: row.locationStatus }).then(() => {
    ElMessage.success("状态更新成功");
  });
}

/** 多选框 */
function handleSelectionChange(selection) {
  ids.value = selection.map((item) => item.locationId);
  single.value = selection.length !== 1;
  multiple.value = !selection.length;
}

fetchWarehouses();
getList();
</script>
