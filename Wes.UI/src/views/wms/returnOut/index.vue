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
        <el-button type="primary" plain icon="Plus" @click="handleAdd" v-hasPermi="['wms:returnOut:add']">新增</el-button>
      </el-col>
      <el-col :span="1.5">
        <el-button type="success" plain icon="Edit" :disabled="single" @click="handleUpdate" v-hasPermi="['wms:returnOut:edit']">修改</el-button>
      </el-col>
      <el-col :span="1.5">
        <el-button type="danger" plain icon="Delete" :disabled="multiple" @click="handleDelete" v-hasPermi="['wms:returnOut:remove']">删除</el-button>
      </el-col>
        <right-toolbar
          v-model:showSearch="showSearch"
          @queryTable="getList"
        ></right-toolbar>
    </el-row>

    <!-- 列表 -->
    <el-table v-loading="loading" :data="returnOutList" @selection-change="handleSelectionChange">
      <el-table-column type="selection" width="55" align="center" />
      <el-table-column label="退货单号" align="center" prop="returnNo" :show-overflow-tooltip="true" width="180" />
      <el-table-column label="退货类型" align="center" prop="returnType" width="100">
        <template #default="scope">
          <el-tag type="warning" size="small">{{ scope.row.returnType || "-" }}</el-tag>
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
      <el-table-column label="出库单号" align="center" :show-overflow-tooltip="true">
        <template #default="scope">
          {{ scope.row.delivery?.deliveryNo || "-" }}
        </template>
      </el-table-column>
      <el-table-column label="状态" align="center" prop="returnStatus" width="80">
        <template #default="scope">
          <el-tag :type="statusTagType(scope.row.returnStatus)" size="small">
            {{ statusLabel(scope.row.returnStatus) }}
          </el-tag>
        </template>
      </el-table-column>
      <el-table-column label="创建时间" align="center" prop="createTime" min-width="170" />
      <el-table-column label="操作" align="center" width="150" class-name="small-padding fixed-width">
        <template #default="scope">
          <el-button link type="primary" icon="Edit" @click="handleUpdate(scope.row)" v-hasPermi="['wms:returnOut:edit']">修改</el-button>
          <el-button link type="danger" icon="Delete" @click="handleDelete(scope.row)" v-hasPermi="['wms:returnOut:remove']">删除</el-button>
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
      <el-form ref="returnOutRef" :model="form" :rules="rules" label-width="110px">
        <el-form-item label="退货单号" prop="returnNo">
          <el-input v-model="form.returnNo" placeholder="系统自动生成" maxlength="50" :readonly="true" />
        </el-form-item>
        <el-form-item label="退货类型" prop="returnType">
          <el-select v-model="form.returnType" placeholder="请选择退货类型" style="width:100%">
            <el-option label="退货出库" value="return_out" />
          </el-select>
        </el-form-item>
        <el-form-item label="原出库id" prop="deliveryId">
          <el-input v-model="form.deliveryId" placeholder="请输入原出库id" />
        </el-form-item>
        <el-form-item label="客户" prop="customerId">
          <PartnerSelect v-model="form.customerId" partner-type="customer" placeholder="请选择客户" @change="onCustomerChange" />
        </el-form-item>
        <el-form-item label="仓库" prop="warehouseId">
          <el-input v-model="form.warehouseId" placeholder="请输入仓库id" />
        </el-form-item>
        <el-form-item label="库区" prop="zoneId">
          <el-input v-model="form.zoneId" placeholder="请输入库区id" />
        </el-form-item>
        <el-form-item label="退货日期" prop="returnDate">
          <el-date-picker v-model="form.returnDate" type="datetime" value-format="YYYY-MM-DD HH:mm:ss" placeholder="请输入退货日期" style="width: 100%" />
        </el-form-item>
        <el-form-item label="退货总数量" prop="totalQuantity">
          <el-input-number controls-position="right" v-model="form.totalQuantity" :min="0" :controls="false" style="width: 100%" />
        </el-form-item>
        <el-form-item label="退货原因" prop="returnReason">
          <el-input v-model="form.returnReason" type="textarea" placeholder="请输入退货原因" maxlength="255" />
        </el-form-item>
        <el-form-item label="退货状态" prop="returnStatus">
          <el-select v-model="form.returnStatus" placeholder="请选择状态" style="width:100%">
            <el-option label="待审核" value="pending" />
            <el-option label="已审核" value="approved" />
            <el-option label="已收货" value="received" />
            <el-option label="已完成" value="completed" />
          </el-select>
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

<script setup name="WmsReturnOut">
import { ref, reactive, toRefs, computed } from "vue";
import { ElMessage, ElMessageBox } from "element-plus";
import {
  listReturnOut,
  getReturnOut,
  addReturnOut,
  updateReturnOut,
  delReturnOut,
} from "@/api/wms/returnOut";
import QueryForm from "@/components/QueryForm/index.vue";
import RightToolbar from "@/components/RightToolbar/index.vue";
import PartnerSelect from "@/views/wms/partner/input.vue";

const returnOutRef = ref(null);
const returnOutList = ref([]);
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
    placeholder: "退货单号",
  },
]);

// 状态映射
const statusMap = {
  pending: "待审核",
  approved: "已审核",
  received: "已收货",
  completed: "已完成",
};
function statusLabel(s) { return statusMap[s] || s; }
function statusTagType(s) {
  const map = { pending: "warning", approved: "", received: "info", completed: "success" };
  return map[s] || "";
}

/** 查询列表 */
function getList() {
  loading.value = true;
  listReturnOut(queryParams.value).then((res) => {
    returnOutList.value = res.rows || [];
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

/** 多选 */
function handleSelectionChange(selection) {
  ids.value = selection.map((item) => item.returnId);
  single.value = selection.length !== 1;
  multiple.value = !selection.length;
}

/** 表单重置 */
function reset() {
  form.value = {
    returnId: undefined,
    returnNo: undefined,
    returnType: "return_out",
    deliveryId: undefined,
    customerId: undefined,
    customerName: undefined,
    warehouseId: undefined,
    zoneId: undefined,
    returnDate: undefined,
    totalQuantity: undefined,
    returnReason: undefined,
    returnStatus: "pending",
  };
  if (returnOutRef.value) returnOutRef.value.resetFields();
}

/** 新增 */
function handleAdd() {
  reset();
  open.value = true;
  title.value = "添加出库退货";
}

/** 修改 */
function handleUpdate(row) {
  reset();
  const id = row.returnId || ids.value[0];
  getReturnOut(id).then((res) => {
    form.value = res.data || {};
    open.value = true;
    title.value = "修改出库退货";
  });
}

/** 提交 */
function submitForm() {
  returnOutRef.value.validate((valid) => {
    if (!valid) return;
    const api = form.value.returnId ? updateReturnOut : addReturnOut;
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
  const delIds = row.returnId || ids.value.join(",");
  ElMessageBox.confirm(`是否确认删除编号为"${delIds}"的数据项？`, "警告", {
    confirmButtonText: "确定",
    cancelButtonText: "取消",
    type: "warning",
  })
    .then(() => delReturnOut(delIds))
    .then(() => {
      getList();
      ElMessage.success("删除成功");
    });
}

function onCustomerChange(data) {
  form.value.customerName = data.partnerName || "";
}

getList();
</script>
