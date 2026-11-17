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
          <el-button type="primary" plain icon="Plus" @click="handleAdd" v-hasPermi="['wms:freeze:add']">新增</el-button>
        </el-col>
        <el-col :span="1.5">
          <el-button v-if="canEdit(selectedRows[0] || {})" type="success" plain icon="Edit" :disabled="single" @click="handleUpdate" v-hasPermi="['wms:freeze:edit']">修改</el-button>
          <el-button v-else type="info" plain icon="View" :disabled="single" @click="handleView(selectedRows[0])">明细</el-button>
        </el-col>
        <el-col :span="1.5">
          <el-button type="danger" plain icon="Delete" :disabled="multiple || selectedRows.some((r) => !canEdit(r))" @click="handleDelete" v-hasPermi="['wms:freeze:remove']">删除</el-button>
        </el-col>
        <right-toolbar
          v-model:showSearch="showSearch"
          @queryTable="getList"
        ></right-toolbar>
      </el-row>

      <!-- 列表 -->
      <el-table v-loading="loading" :data="freezeList" @selection-change="handleSelectionChange">
        <el-table-column type="selection" width="55" align="center" />
        <el-table-column label="冻结单号" align="center" prop="freezeNo" :show-overflow-tooltip="true" />
        <el-table-column label="冻结类型" align="center" width="100">
          <template #default="scope">
            {{ freezeTypeMap[scope.row.freezeType] || "-" }}
          </template>
        </el-table-column>
        <el-table-column label="仓库" align="center" :show-overflow-tooltip="true">
          <template #default="scope">
            {{ scope.row.warehouse?.warehouseName || scope.row.warehouseName || "-" }}
          </template>
        </el-table-column>
        <el-table-column label="库区" align="center" :show-overflow-tooltip="true">
          <template #default="scope">
            {{ scope.row.zone?.zoneName || scope.row.zoneCode || "-" }}
          </template>
        </el-table-column>
        <el-table-column label="库位" align="center" width="120" :show-overflow-tooltip="true">
          <template #default="scope">
            {{ scope.row.location?.locationCode || scope.row.locationCode || "-" }}
          </template>
        </el-table-column>
        <el-table-column label="物料" align="center" min-width="170" :show-overflow-tooltip="true">
          <template #default="scope">
            {{ materialLabel(scope.row) }}
          </template>
        </el-table-column>
        <el-table-column label="批次号" align="center" prop="batchNo" width="110" :show-overflow-tooltip="true">
          <template #default="scope">
            {{ scope.row.batchNo || "-" }}
          </template>
        </el-table-column>
        <el-table-column label="冻结数量" align="center" prop="freezeQuantity" width="90" />
        <el-table-column label="状态" align="center" width="100">
          <template #default="scope">
            <el-tag :type="freezeStatusTag(scope.row.freezeStatus)">
              {{ freezeStatusMap[scope.row.freezeStatus] || "-" }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column label="创建时间" align="center" prop="createTime" min-width="170" />
        <el-table-column label="操作" align="center" width="220" class-name="small-padding fixed-width">
          <template #default="scope">
            <el-button v-if="canEdit(scope.row)" link type="success" icon="Promotion" @click="handleExecute(scope.row)" v-hasPermi="['wms:freeze:edit']">冻结</el-button>
            <el-button v-if="scope.row.freezeStatus === 'frozen'" link type="warning" icon="Sunny" @click="handleThaw(scope.row)" v-hasPermi="['wms:freeze:edit']">解冻</el-button>
            <el-button v-if="canEdit(scope.row)" link type="primary" icon="Edit" @click="handleUpdate(scope.row)" v-hasPermi="['wms:freeze:edit']">修改</el-button>
            <el-button v-else link type="info" icon="View" @click="handleView(scope.row)">明细</el-button>
            <el-button v-if="canEdit(scope.row)" link type="danger" icon="Delete" @click="handleDelete(scope.row)" v-hasPermi="['wms:freeze:remove']">删除</el-button>
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
      <el-dialog :title="title" v-model="open" width="780px" append-to-body>
        <el-form ref="freezeRef" :model="form" :rules="rules" label-width="88px" class="nowrap-labels" :disabled="viewMode">
          <el-row :gutter="16">
            <el-col :span="12">
              <el-form-item label="冻结单号">
                <el-input v-model="form.freezeNo" placeholder="保存后自动生成" readonly />
              </el-form-item>
            </el-col>
            <el-col :span="12">
              <el-form-item label="冻结类型" prop="freezeType">
                <el-select v-model="form.freezeType" placeholder="请选择冻结类型" style="width:100%">
                  <el-option v-for="(label, value) in freezeTypeMap" :key="value" :label="label" :value="value" />
                </el-select>
              </el-form-item>
            </el-col>
            <el-col :span="12">
              <el-form-item label="仓库" prop="warehouseId">
                <WarehouseSelect v-model="form.warehouseId" placeholder="请选择仓库" :disabled="viewMode" @change="onWarehouseChange" />
              </el-form-item>
            </el-col>
            <el-col :span="12">
              <el-form-item label="库位" prop="locationId">
                <LocationCascader
                  v-model="form.locCascader"
                  :warehouse-id="form.warehouseId"
                  :disabled="!form.warehouseId || viewMode"
                  :placeholder="form.warehouseId ? '请选择库区/库位' : '请先选择仓库'"
                  @update:model-value="onLocation"
                />
              </el-form-item>
            </el-col>
            <el-col :span="24">
              <el-form-item label="库存行" prop="stockId">
                <el-select
                  v-if="!viewMode"
                  v-model="form.stockId"
                  :loading="stockLoading"
                  :placeholder="form.locationId ? '选择要冻结的库存行（同物料同批次可能多条码）' : '先选库位'"
                  :disabled="!form.locationId"
                  style="width:100%"
                  @change="onStockPick"
                >
                  <el-option
                    v-for="opt in stockOptions"
                    :key="opt.value"
                    :label="opt.label"
                    :value="opt.value"
                  />
                </el-select>
                <el-input v-else :value="form.stockLabel || '-'" readonly />
              </el-form-item>
            </el-col>
            <el-col :span="12">
              <el-form-item label="物料">
                <span>{{ form.materialCode }} - {{ form.materialName || "-" }}</span>
              </el-form-item>
            </el-col>
            <el-col :span="12">
              <el-form-item label="批次号">
                <span>{{ form.batchNo || "无" }}</span>
              </el-form-item>
            </el-col>
            <el-col :span="12">
              <el-form-item label="冻结数量" prop="freezeQuantity">
                <el-input-number v-model="form.freezeQuantity" :min="0" :controls="false"
                  :placeholder="form.stockAvailable != null ? `可冻结 ${form.stockAvailable}` : '冻结数量'"
                  style="width:100%" />
              </el-form-item>
            </el-col>
            <el-col :span="24">
              <el-form-item label="冻结原因" prop="freezeReason">
                <el-input v-model="form.freezeReason" type="textarea" :autosize="{ minRows: 2, maxRows: 4 }"
                  placeholder="请说明冻结原因" maxlength="255" />
              </el-form-item>
            </el-col>
          </el-row>

          <div class="form-tip text-muted">
            精确到库存行：同物料同批次多条码时必须指定冻哪一行。执行冻结后该行可用库存减少、冻结数量增加，库存总量不变；解冻后可用库存自动回补。
          </div>
        </el-form>
        <template #footer>
          <div class="dialog-footer">
            <el-button v-if="!viewMode" type="primary" @click="submitForm">确 定</el-button>
            <el-button @click="cancel">{{ viewMode ? "关 闭" : "取 消" }}</el-button>
          </div>
        </template>
      </el-dialog>
    </div>
  </div>
</template>

<script setup name="WmsFreeze">
import { ref, reactive, toRefs, computed } from "vue";
import { ElMessage, ElMessageBox } from "element-plus";
import {
  listFreeze,
  getFreeze,
  addFreeze,
  updateFreeze,
  delFreeze,
  executeFreeze,
  thawFreeze,
} from "@/api/wms/freeze";
import { listStock } from "@/api/wms/stock";
import QueryForm from "@/components/QueryForm/index.vue";
import RightToolbar from "@/components/RightToolbar/index.vue";
import WarehouseSelect from "@/views/wms/warehouse/input.vue";
import LocationCascader from "@/views/wms/location/cascader.vue";

const freezeRef = ref(null);
const freezeList = ref([]);
const stockOptions = ref([]);
const stockLoading = ref(false);
const open = ref(false);
const loading = ref(true);
const ids = ref([]);
const single = ref(true);
const multiple = ref(true);
const total = ref(0);
const title = ref("");
/** 只读查看模式：已冻结/已解冻后只用于查看 */
const viewMode = ref(false);
/** 列表当前勾选的行 */
const selectedRows = ref([]);

const showSearch = ref(true);

// 冻结类型
const freezeTypeMap = {
  quality: "质量隔离",
  legal: "司法查封",
  reserve: "预留锁定",
  other: "其他",
};

// 冻结状态：待冻结 → 已冻结 → 已解冻
const freezeStatusMap = { pending: "待冻结", frozen: "已冻结", thawed: "已解冻" };
const freezeStatusTag = (s) => (s === "frozen" ? "warning" : s === "thawed" ? "success" : "info");

/** 只有待冻结的单据可以修改/删除/执行；已冻结后只能解冻或查看 */
function canEdit(row) {
  return row.freezeStatus !== "frozen" && row.freezeStatus !== "thawed";
}

const data = reactive({
  form: {},
  queryParams: {
    pageNum: 1,
    pageSize: 10,
    keywords: undefined,
    freezeStatus: undefined,
  },
  rules: {
    warehouseId: [{ required: true, message: "请选择仓库", trigger: "change" }],
    materialId: [{ required: true, message: "请选择物料", trigger: "change" }],
    locationId: [{ required: true, message: "请选择库位", trigger: "change" }],
    freezeQuantity: [{ required: true, message: "请输入冻结数量", trigger: "blur" }],
    freezeType: [{ required: true, message: "请选择冻结类型", trigger: "change" }],
  },
});

const { form, queryParams, rules } = toRefs(data);

const queryConfig = computed(() => [
  {
    label: "关键字",
    prop: "keywords",
    type: "input",
    placeholder: "请输入冻结单号",
  },
  {
    label: "状态",
    prop: "freezeStatus",
    type: "select",
    placeholder: "请选择状态",
    options: [
      { label: "待冻结", value: "pending" },
      { label: "已冻结", value: "frozen" },
      { label: "已解冻", value: "thawed" },
    ],
  },
]);

/** 列表物料显示：优先取冗余编码名称，回退导航对象 */
function materialLabel(row) {
  const code = row.materialCode || row.material?.materialCode;
  const name = row.materialName || row.material?.materialName;
  if (!code && !name) return "-";
  return code && name ? `${code} - ${name}` : code || name;
}

/** 查询列表 */
function getList() {
  loading.value = true;
  listFreeze(queryParams.value).then((res) => {
    freezeList.value = res.rows || [];
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
    freezeStatus: undefined,
    pageNum: 1,
    pageSize: 10,
  };
  handleQuery();
}

/** 多选框 */
function handleSelectionChange(selection) {
  selectedRows.value = selection;
  ids.value = selection.map((item) => item.freezeId);
  single.value = selection.length !== 1;
  multiple.value = !selection.length;
}

/** 表单重置 */
function reset() {
  form.value = {
    freezeId: undefined,
    freezeNo: undefined,
    warehouseId: undefined,
    materialId: undefined,
    materialCode: "",
    materialName: "",
    batchNo: undefined,
    zoneId: undefined,
    locationId: undefined,
    locCascader: [],
    stockId: undefined,
    stockLabel: undefined,
    stockAvailable: undefined,
    freezeQuantity: undefined,
    freezeType: undefined,
    freezeReason: undefined,
    freezeStatus: undefined,
    // 执行/解冻时由后端回填，编辑时原样带回避免被清空
    freezeUserId: undefined,
    freezeUserName: undefined,
    freezeTime: undefined,
    thawUserId: undefined,
    thawUserName: undefined,
    thawTime: undefined,
    thawReason: undefined,
  };
  viewMode.value = false;
  if (freezeRef.value) freezeRef.value.resetFields();
}

/** 新增 */
function handleAdd() {
  reset();
  open.value = true;
  title.value = "添加库存冻结";
}

/** 修改 */
function handleUpdate(row) {
  reset();
  const id = row?.freezeId || ids.value[0];
  getFreeze(id).then((res) => fillForm(res.data || {}));
}

/** 查看明细：已冻结/已解冻后只读打开 */
function handleView(row) {
  if (!row?.freezeId) return;
  reset();
  viewMode.value = true;
  getFreeze(row.freezeId).then((res) => fillForm(res.data || {}));
}

/** 填充表单 */
function fillForm(data) {
  form.value = {
    freezeId: data.freezeId,
    freezeNo: data.freezeNo,
    warehouseId: data.warehouseId,
    materialId: data.materialId,
    materialCode: data.materialCode || data.material?.materialCode || "",
    materialName: data.materialName || data.material?.materialName || "",
    batchNo: data.batchNo,
    zoneId: data.zoneId,
    locationId: data.locationId,
    locCascader: data.locationId ? [data.zoneId, data.locationId].filter(Boolean) : [],
    stockId: data.stockId,
    freezeQuantity: data.freezeQuantity,
    freezeType: data.freezeType,
    freezeReason: data.freezeReason,
    freezeStatus: data.freezeStatus,
    freezeUserId: data.freezeUserId,
    freezeUserName: data.freezeUserName,
    freezeTime: data.freezeTime,
    thawUserId: data.thawUserId,
    thawUserName: data.thawUserName,
    thawTime: data.thawTime,
    thawReason: data.thawReason,
  };
  stockOptions.value = [];
  // 回显库存行：拉取该库位的库存行选项，编辑模式下拉才能正确显示已选行
  if (data.locationId) {
    stockLoading.value = true;
    listStock({ pageNum: 1, pageSize: 9999, params: { locationId: data.locationId } })
      .then((res) => {
        stockOptions.value = makeStockOptions(res.rows);
        const matched = stockOptions.value.find((o) => o.stockId === data.stockId);
        // 查看模式只读文本显示完整行描述
        if (matched) {
          form.value.stockLabel = matched.labelShort;
          form.value.stockAvailable = matched.available;
        } else if (data.stockId) {
          form.value.stockLabel = "该库存行已不存在";
        }
      })
      .finally(() => {
        stockLoading.value = false;
      });
  }
  open.value = true;
  title.value = viewMode.value ? "冻结单明细" : "修改库存冻结";
}

/** 库位选择：切换库位后拉取该库位的库存行 */
function onLocation(val) {
  form.value.zoneId = val?.[0];
  form.value.locationId = val?.[1];
  form.value.stockId = undefined;
  form.value.stockLabel = undefined;
  form.value.stockAvailable = undefined;
  stockOptions.value = [];
  form.value.materialId = undefined;
  form.value.materialCode = "";
  form.value.materialName = "";
  form.value.batchNo = undefined;
  if (form.value.locationId) loadStockOptions();
}

/** 组装库存行下拉选项（同物料同批次可能多条码行） */
function makeStockOptions(rows) {
  return (rows || []).map((s) => {
    const available = (Number(s.stockQuantity) || 0) - (Number(s.freezeQuantity) || 0) - (Number(s.lockQuantity) || 0);
    return {
      value: String(s.stockId),
      label: `${s.materialCode || s.material?.materialCode} / 批次${s.batchNo || "无"}${s.barcode ? " / " + s.barcode : ""} / 可用 ${available}`,
      labelShort: `${s.materialCode || s.material?.materialCode} / 批次${s.batchNo || "无"}${s.barcode ? " / " + s.barcode : ""} / 库存 ${s.stockQuantity}`,
      available,
      stockId: s.stockId,
      materialId: s.materialId,
      materialCode: s.materialCode || s.material?.materialCode,
      materialName: s.materialName || s.material?.materialName,
      batchNo: s.batchNo || "",
    };
  });
}

/** 拉取库位下全部库存行 */
function loadStockOptions() {
  stockLoading.value = true;
  listStock({ pageNum: 1, pageSize: 9999, params: { locationId: form.value.locationId } })
    .then((res) => {
      stockOptions.value = makeStockOptions(res.rows);
    })
    .finally(() => {
      stockLoading.value = false;
    });
}

/** 库存行选择：锁定物料/批次，带出可用量并默认全量冻结 */
function onStockPick(val) {
  const opt = stockOptions.value.find((o) => o.value === val);
  if (!opt) return;
  form.value.stockId = opt.stockId;
  form.value.stockAvailable = opt.available;
  form.value.materialId = opt.materialId;
  form.value.materialCode = opt.materialCode;
  form.value.materialName = opt.materialName;
  form.value.batchNo = opt.batchNo;
  form.value.freezeQuantity = opt.available > 0 ? opt.available : undefined;
}

/** 仓库变更：清空库位（避免跨仓库） */
function onWarehouseChange() {
  form.value.locCascader = [];
  form.value.zoneId = undefined;
  form.value.locationId = undefined;
  form.value.stockId = undefined;
  form.value.stockLabel = undefined;
  form.value.stockAvailable = undefined;
  stockOptions.value = [];
  form.value.materialId = undefined;
  form.value.materialCode = "";
  form.value.materialName = "";
  form.value.batchNo = undefined;
}

/** 提交 */
function submitForm() {
  freezeRef.value.validate((valid) => {
    if (!valid) return;
    if (!form.value.locationId) {
      ElMessage.warning("请选择库位");
      return;
    }
    if (!form.value.stockId) {
      ElMessage.warning("请选择库存行（同物料同批次可能有多条码，必须精确到行）");
      return;
    }
    if (!(form.value.freezeQuantity > 0)) {
      ElMessage.warning("冻结数量必须大于 0");
      return;
    }
    if (form.value.stockAvailable != null && form.value.freezeQuantity > form.value.stockAvailable) {
      ElMessage.warning(`冻结数量超过该库存行可用量（可用 ${form.value.stockAvailable}）`);
      return;
    }

    const payload = {
      freezeId: form.value.freezeId,
      freezeNo: form.value.freezeNo,
      warehouseId: form.value.warehouseId,
      materialId: form.value.materialId,
      materialCode: form.value.materialCode,
      materialName: form.value.materialName,
      batchNo: form.value.batchNo,
      zoneId: form.value.zoneId,
      locationId: form.value.locationId,
      stockId: form.value.stockId,
      freezeQuantity: form.value.freezeQuantity,
      freezeType: form.value.freezeType,
      freezeReason: form.value.freezeReason,
      freezeStatus: form.value.freezeStatus,
      freezeUserId: form.value.freezeUserId,
      freezeUserName: form.value.freezeUserName,
      freezeTime: form.value.freezeTime,
      thawUserId: form.value.thawUserId,
      thawUserName: form.value.thawUserName,
      thawTime: form.value.thawTime,
      thawReason: form.value.thawReason,
    };

    const api = form.value.freezeId ? updateFreeze : addFreeze;
    api(payload).then(() => {
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

/** 执行冻结 */
function handleExecute(row) {
  ElMessageBox.confirm(
    `执行冻结单「${row.freezeNo}」将占用该库位的可用库存（库存总量不变），执行后不可修改。是否继续？`,
    "冻结确认",
    { confirmButtonText: "确定冻结", cancelButtonText: "取消", type: "warning" }
  )
    .then(() => executeFreeze(row.freezeId))
    .then(() => {
      ElMessage.success("冻结成功，可用库存已扣减");
      getList();
    });
}

/** 解冻 */
function handleThaw(row) {
  ElMessageBox.prompt("请输入解冻原因", "解冻确认", {
    confirmButtonText: "确定解冻",
    cancelButtonText: "取消",
    inputPlaceholder: "解冻原因（选填）",
    inputType: "textarea",
  })
    .then(({ value }) => thawFreeze(row.freezeId, value))
    .then(() => {
      ElMessage.success("已解冻，可用库存已回补");
      getList();
    });
}

/** 删除 */
function handleDelete(row) {
  const delIds = row.freezeId || ids.value.join(",");
  ElMessageBox.confirm(`是否确认删除冻结单编号为"${delIds}"的数据项？`, "警告", {
    confirmButtonText: "确定",
    cancelButtonText: "取消",
    type: "warning",
  })
    .then(() => delFreeze(delIds))
    .then(() => {
      getList();
      ElMessage.success("删除成功");
    });
}

getList();
</script>

<style scoped>
.form-tip {
  margin-top: 4px;
  font-size: 12px;
}
.text-muted {
  color: #909399;
}
</style>
