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
        <el-button type="primary" plain icon="Plus" @click="handleAdd" v-hasPermi="['wms:delivery:add']">新增</el-button>
      </el-col>
      <el-col :span="1.5">
        <el-button type="success" plain icon="Edit" :disabled="single" @click="handleUpdate" v-hasPermi="['wms:delivery:edit']">修改</el-button>
      </el-col>
      <el-col :span="1.5">
        <el-button type="danger" plain icon="Delete" :disabled="multiple" @click="handleDelete" v-hasPermi="['wms:delivery:remove']">删除</el-button>
      </el-col>
        <right-toolbar
          v-model:showSearch="showSearch"
          @queryTable="getList"
        ></right-toolbar>
    </el-row>

    <!-- 列表 -->
    <el-table v-loading="loading" :data="deliveryList" @selection-change="handleSelectionChange">
      <el-table-column type="selection" width="55" align="center" />
      <el-table-column label="出库单号" align="center" prop="deliveryNo" :show-overflow-tooltip="true" />
      
      
      
      <el-table-column label="创建时间" align="center" prop="createTime" min-width="170" />
      <el-table-column label="操作" align="center" width="150" class-name="small-padding fixed-width">
        <template #default="scope">
          <el-button v-if="scope.row.deliveryStatus !== 'shipped' && scope.row.deliveryStatus !== 'delivered'" link type="success" icon="Promotion" @click="handleExecute(scope.row)" v-hasPermi="['wms:delivery:edit']">出库</el-button>
          <el-button link type="primary" icon="Edit" @click="handleUpdate(scope.row)" v-hasPermi="['wms:delivery:edit']">修改</el-button>
          <el-button link type="danger" icon="Delete" @click="handleDelete(scope.row)" v-hasPermi="['wms:delivery:remove']">删除</el-button>
        </template>
      </el-table-column>
        <el-table-column label="仓库" align="center" :show-overflow-tooltip="true">
      <template #default="scope">
        {{ scope.row.warehouse?.warehouseName || "-" }}
      </template>
    </el-table-column>
    <el-table-column label="库区编码" align="center" :show-overflow-tooltip="true">
      <template #default="scope">
        {{ scope.row.zone?.zoneCode || "-" }}
      </template>
    </el-table-column>
    <el-table-column label="库区" align="center" :show-overflow-tooltip="true">
      <template #default="scope">
        {{ scope.row.zone?.zoneName || "-" }}
      </template>
    </el-table-column>
    <el-table-column label="客户" align="center" :show-overflow-tooltip="true">
      <template #default="scope">
        {{ scope.row.customer?.partnerName || "-" }}
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
      <el-form ref="deliveryRef" :model="form" :rules="rules" label-width="auto">
        <el-form-item label="出库单号" prop="deliveryNo">
          <el-input v-model="form.deliveryNo" placeholder="系统自动生成" maxlength="50" :readonly="true" />
        </el-form-item>

        <el-form-item label="仓库id" prop="warehouseId">
          <el-input v-model="form.warehouseId" placeholder="请输入仓库id" />
        </el-form-item>
        
        <el-form-item label="发货库区id" prop="zoneId">
          <el-input v-model="form.zoneId" placeholder="请输入发货库区id" />
        </el-form-item>
        
        <el-form-item label="客户id" prop="customerId">
          <el-input v-model="form.customerId" placeholder="请输入客户id" />
        </el-form-item>
        
        <el-form-item label="发货日期" prop="deliveryDate">
          <el-date-picker v-model="form.deliveryDate" type="datetime" value-format="YYYY-MM-DD HH:mm:ss" placeholder="请输入发货日期" style="width: 100%" />
        </el-form-item>
        <el-form-item label="总数量" prop="totalQuantity">
          <el-input-number controls-position="right" v-model="form.totalQuantity" :min="0" :controls="false" style="width: 100%" />
        </el-form-item>
        <el-form-item label="总金额" prop="totalAmount">
          <el-input-number controls-position="right" v-model="form.totalAmount" :min="0" :precision="2" :controls="false" style="width: 100%" />
        </el-form-item>
        <el-form-item label="承运商" prop="carrierName">
          <el-input v-model="form.carrierName" placeholder="请输入承运商" maxlength="100" />
        </el-form-item>
        <el-form-item label="运单号" prop="waybillNo">
          <el-input v-model="form.waybillNo" placeholder="请输入运单号" maxlength="100" />
        </el-form-item>
        <el-form-item label="包裹数量" prop="packageCount">
          <el-input-number controls-position="right" v-model="form.packageCount" :min="0" :controls="false" style="width: 100%" />
        </el-form-item>
        <el-form-item label="总重量" prop="totalWeight">
          <el-input-number controls-position="right" v-model="form.totalWeight" :min="0" :precision="2" :controls="false" style="width: 100%" />
        </el-form-item>
        <el-form-item label="出库状态" prop="deliveryStatus">
          <el-input v-model="form.deliveryStatus" placeholder="请输入出库状态" maxlength="20" />
        </el-form-item>
        <el-form-item label="发货时间" prop="shipTime">
          <el-date-picker v-model="form.shipTime" type="datetime" value-format="YYYY-MM-DD HH:mm:ss" placeholder="请输入发货时间" style="width: 100%" />
        </el-form-item>
        <el-form-item label="签收时间" prop="signTime">
          <el-date-picker v-model="form.signTime" type="datetime" value-format="YYYY-MM-DD HH:mm:ss" placeholder="请输入签收时间" style="width: 100%" />
        </el-form-item>
        <el-form-item label="操作人id" prop="operatorId">
          <el-input v-model="form.operatorId" placeholder="请输入操作人id" />
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

<script setup name="WmsDelivery">
import { ref, reactive, toRefs, computed } from "vue";
import { ElMessage, ElMessageBox } from "element-plus";
import {
  listDelivery,
  getDelivery,
  addDelivery,
  updateDelivery,
  delDelivery,
  executeDelivery,
} from "@/api/wms/delivery";
import QueryForm from "@/components/QueryForm/index.vue";
import RightToolbar from "@/components/RightToolbar/index.vue";

const deliveryRef = ref(null);
const deliveryList = ref([]);
const open = ref(false);
const loading = ref(true);
const ids = ref([]);
const single = ref(true);
const multiple = ref(true);
const total = ref(0);
const title = ref("");

const showSearch = ref(true);

const data = reactive({
  form: {},
  queryParams: {
    pageNum: 1,
    pageSize: 10,
    keywords: undefined,
  },
  rules: {

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

/** 查询列表 */
function getList() {
  loading.value = true;
  listDelivery(queryParams.value).then((res) => {
    deliveryList.value = res.rows || [];
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
    deliveryId: undefined,
    deliveryNo: undefined,
    waveId: undefined,
    waveNo: undefined,
    warehouseId: undefined,
    warehouseName: undefined,
    zoneId: undefined,
    zoneCode: undefined,
    customerId: undefined,
    customerName: undefined,
    deliveryDate: undefined,
    totalQuantity: undefined,
    totalAmount: undefined,
    carrierName: undefined,
    waybillNo: undefined,
    packageCount: undefined,
    totalWeight: undefined,
    deliveryStatus: undefined,
    shipTime: undefined,
    signTime: undefined,
    operatorId: undefined,
    operatorName: undefined,
  };
  if (deliveryRef.value) deliveryRef.value.resetFields();
}

/** 新增 */
function handleAdd() {
  reset();
  open.value = true;
  title.value = "添加出库单";
}

/** 修改 */
function handleUpdate(row) {
  reset();
  const id = row.deliveryId || ids.value[0];
  getDelivery(id).then((res) => {
    form.value = res.data || {};
    open.value = true;
    title.value = "修改出库单";
  });
}

/** 提交 */
function submitForm() {
  deliveryRef.value.validate((valid) => {
    if (!valid) return;
    const api = form.value.deliveryId ? updateDelivery : addDelivery;
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

/** 执行出库（发货） */
function handleExecute(row) {
  ElMessageBox.confirm(
    `出库单「${row.deliveryNo}」出库将立即扣减库存可用量并写出库流水，执行后不可修改和删除。是否继续？`,
    "出库确认",
    { confirmButtonText: "确定出库", cancelButtonText: "取消", type: "warning" }
  )
    .then(() => executeDelivery(row.deliveryId))
    .then(() => {
      ElMessage.success("出库成功，库存已扣减");
      getList();
    });
}

/** 删除 */
function handleDelete(row) {
  const delIds = row.deliveryId || ids.value.join(",");
  ElMessageBox.confirm(`是否确认删除编号为"${delIds}"的数据项？`, "警告", {
    confirmButtonText: "确定",
    cancelButtonText: "取消",
    type: "warning",
  })
    .then(() => delDelivery(delIds))
    .then(() => {
      getList();
      ElMessage.success("删除成功");
    });
}

getList();
</script>
