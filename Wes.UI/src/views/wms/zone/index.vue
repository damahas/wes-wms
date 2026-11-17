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
        <el-button type="primary" plain icon="Plus" @click="handleAdd" v-hasPermi="['wms:zone:add']">新增</el-button>
      </el-col>
      <el-col :span="1.5">
        <el-button type="success" plain icon="Edit" :disabled="single" @click="handleUpdate" v-hasPermi="['wms:zone:edit']">修改</el-button>
      </el-col>
      <el-col :span="1.5">
        <el-button type="danger" plain icon="Delete" :disabled="multiple" @click="handleDelete" v-hasPermi="['wms:zone:remove']">删除</el-button>
      </el-col>
        <right-toolbar
          v-model:showSearch="showSearch"
          @queryTable="getList"
        ></right-toolbar>
    </el-row>

    <!-- 列表 -->
    <el-table v-loading="loading" :data="zoneList" @selection-change="handleSelectionChange" table-layout="fixed">
      <el-table-column type="selection" width="55" align="center" />
      <el-table-column label="仓库" align="center" :show-overflow-tooltip="true">
        <template #default="scope">{{ scope.row.warehouse?.warehouseName || "-" }}</template>
      </el-table-column>
      <el-table-column label="库区编码" align="center" prop="zoneCode" :show-overflow-tooltip="true" />
      <el-table-column label="库区名称" align="center" prop="zoneName" :show-overflow-tooltip="true" />
      <el-table-column label="库区类型" align="center" prop="zoneType">
        <template #default="scope">
          <dict-tag :options="zone_type" :value="scope.row.zoneType" />
        </template>
      </el-table-column>
      <el-table-column label="状态" align="center" prop="zoneStatus" width="80">
        <template #default="scope">
          <el-switch v-model="scope.row.zoneStatus" active-value="0" inactive-value="1" @change="handleStatusChange(scope.row)" />
        </template>
      </el-table-column>
      <el-table-column label="创建时间" align="center" prop="createTime" min-width="170" />
      <el-table-column label="操作" align="center" width="150" class-name="small-padding fixed-width">
        <template #default="scope">
          <el-button link type="primary" icon="Edit" @click="handleUpdate(scope.row)" v-hasPermi="['wms:zone:edit']">修改</el-button>
          <el-button link type="danger" icon="Delete" @click="handleDelete(scope.row)" v-hasPermi="['wms:zone:remove']">删除</el-button>
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
      <el-form ref="zoneRef" :model="form" :rules="rules" label-width="auto">
        <el-form-item label="仓库" prop="warehouseId">
          <el-select v-model="form.warehouseId" placeholder="请选择仓库" style="width: 100%">
            <el-option v-for="w in warehouseOptions" :key="w.warehouseId" :label="w.warehouseName" :value="w.warehouseId" />
          </el-select>
        </el-form-item>
        <el-form-item label="库区编码" prop="zoneCode">
          <el-input v-model="form.zoneCode" placeholder="请输入库区编码" maxlength="50" />
        </el-form-item>
        <el-form-item label="库区名称" prop="zoneName">
          <el-input v-model="form.zoneName" placeholder="请输入库区名称" maxlength="100" />
        </el-form-item>
        <el-form-item label="库区类型" prop="zoneType">
          <el-select v-model="form.zoneType" placeholder="请选择库区类型" clearable style="width: 100%">
            <el-option v-for="dict in zone_type" :key="dict.value" :label="dict.label" :value="dict.value" />
          </el-select>
        </el-form-item>
        <el-form-item label="库区属性" prop="zoneAttr">
          <el-input v-model="form.zoneAttr" placeholder="请输入库区属性" maxlength="50" />
        </el-form-item>
        <el-form-item label="所在楼层" prop="floor">
          <el-input-number controls-position="right" v-model="form.floor" :min="0" :controls="false" style="width: 100%" />
        </el-form-item>
        <el-form-item label="库区面积" prop="zoneArea">
          <el-input-number controls-position="right" v-model="form.zoneArea" :min="0" :precision="2" :controls="false" style="width: 100%" />
        </el-form-item>
        <el-form-item label="货架排数" prop="shelfRows">
          <el-input-number controls-position="right" v-model="form.shelfRows" :min="0" :controls="false" style="width: 100%" />
        </el-form-item>
        <el-form-item label="货架列数" prop="shelfCols">
          <el-input-number controls-position="right" v-model="form.shelfCols" :min="0" :controls="false" style="width: 100%" />
        </el-form-item>
        <el-form-item label="货架层数" prop="shelfLevels">
          <el-input-number controls-position="right" v-model="form.shelfLevels" :min="0" :controls="false" style="width: 100%" />
        </el-form-item>
        <el-form-item label="库区状态" prop="zoneStatus">
          <el-radio-group v-model="form.zoneStatus">
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

<script setup name="WmsZone">
import { ref, reactive, toRefs, computed } from "vue";
import { ElMessage, ElMessageBox } from "element-plus";
import {
  listZone,
  getZone,
  addZone,
  updateZone,
  delZone,
  changeZoneStatus,
} from "@/api/wms/zone";
import { listWarehouse } from "@/api/wms/warehouse";
import DictTag from "@/components/DictTag/index.vue";
import QueryForm from "@/components/QueryForm/index.vue";
import RightToolbar from "@/components/RightToolbar/index.vue";
import { getDict } from "@/utils";

const { zone_type } = getDict("wms_zone_type");

const zoneRef = ref(null);
const zoneList = ref([]);
const open = ref(false);
const loading = ref(true);
const ids = ref([]);
const single = ref(true);
const multiple = ref(true);
const total = ref(0);
const title = ref("");
const warehouseOptions = ref([]);

const showSearch = ref(true);

const data = reactive({
  form: {},
  queryParams: {
    pageNum: 1,
    pageSize: 10,
    keywords: undefined,
  },
  rules: {
    zoneCode: [{ required: true, message: "库区编码不能为空", trigger: "blur" }],
    zoneName: [{ required: true, message: "库区名称不能为空", trigger: "blur" }],
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

/** 查询列表 */
function getList() {
  loading.value = true;
  listZone(queryParams.value).then((res) => {
    zoneList.value = res.rows || [];
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
    zoneId: undefined,
    warehouseId: undefined,
    zoneCode: undefined,
    zoneName: undefined,
    zoneType: undefined,
    zoneAttr: undefined,
    floor: undefined,
    zoneArea: undefined,
    shelfRows: undefined,
    shelfCols: undefined,
    shelfLevels: undefined,
    zoneStatus: "0",
    remark: undefined,
  };
  if (zoneRef.value) zoneRef.value.resetFields();
}

/** 新增 */
function handleAdd() {
  reset();
  open.value = true;
  title.value = "添加库区";
}

/** 修改 */
function handleUpdate(row) {
  reset();
  const id = row.zoneId || ids.value[0];
  getZone(id).then((res) => {
    form.value = res.data || {};
    open.value = true;
    title.value = "修改库区";
  });
}

/** 提交 */
function submitForm() {
  zoneRef.value.validate((valid) => {
    if (!valid) return;
    const api = form.value.zoneId ? updateZone : addZone;
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
  const delIds = row.zoneId || ids.value.join(",");
  ElMessageBox.confirm(`是否确认删除编号为"${delIds}"的数据项？`, "警告", {
    confirmButtonText: "确定",
    cancelButtonText: "取消",
    type: "warning",
  })
    .then(() => delZone(delIds))
    .then(() => {
      getList();
      ElMessage.success("删除成功");
    });
}

/** 状态切换 */
function handleStatusChange(row) {
  changeZoneStatus({ zoneId: row.zoneId, zoneStatus: row.zoneStatus }).then(() => {
    ElMessage.success("状态更新成功");
  });
}

/** 多选框 */
function handleSelectionChange(selection) {
  ids.value = selection.map((item) => item.zoneId);
  single.value = selection.length !== 1;
  multiple.value = !selection.length;
}

fetchWarehouses();
getList();
</script>
