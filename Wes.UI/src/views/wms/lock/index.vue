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
          <el-button type="primary" plain icon="Plus" @click="handleAdd" v-hasPermi="['wms:lock:add']">新增</el-button>
        </el-col>
        <el-col :span="1.5">
          <el-button v-if="canEdit(selectedRows[0] || {})" type="success" plain icon="Edit" :disabled="single" @click="handleUpdate" v-hasPermi="['wms:lock:edit']">修改</el-button>
          <el-button v-else type="info" plain icon="View" :disabled="single" @click="handleView(selectedRows[0])">明细</el-button>
        </el-col>
        <el-col :span="1.5">
          <el-button type="danger" plain icon="Delete" :disabled="multiple || selectedRows.some((r) => !canEdit(r))" @click="handleDelete" v-hasPermi="['wms:lock:remove']">删除</el-button>
        </el-col>
        <right-toolbar
          v-model:showSearch="showSearch"
          @queryTable="getList"
        ></right-toolbar>
      </el-row>

      <!-- 列表 -->
      <el-table v-loading="loading" :data="lockList" @selection-change="handleSelectionChange">
        <el-table-column type="selection" width="55" align="center" />
        <el-table-column label="锁定编号" align="center" prop="lockId" width="170" :show-overflow-tooltip="true" />
        <el-table-column label="锁定类型" align="center" width="110">
          <template #default="scope">
            {{ lockTypeMap[scope.row.lockType] || "-" }}
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
            {{ scope.row.location?.locationCode || scope.row.locationId || "-" }}
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
        <el-table-column label="锁定数量" align="center" prop="lockQuantity" width="90" />
        <el-table-column label="已使用" align="center" prop="usedQuantity" width="80">
          <template #default="scope">
            {{ scope.row.usedQuantity ?? 0 }}
          </template>
        </el-table-column>
        <el-table-column label="状态" align="center" width="100">
          <template #default="scope">
            <el-tag :type="lockStatusTag(scope.row.lockStatus)">
              {{ lockStatusMap[scope.row.lockStatus] || "-" }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column label="创建时间" align="center" prop="createTime" min-width="170" />
        <el-table-column label="操作" align="center" width="220" class-name="small-padding fixed-width">
          <template #default="scope">
            <el-button v-if="canEdit(scope.row)" link type="success" icon="Promotion" @click="handleExecute(scope.row)" v-hasPermi="['wms:lock:edit']">锁定</el-button>
            <el-button v-if="scope.row.lockStatus === 'locked' || scope.row.lockStatus === 'partial'" link type="warning" icon="Unlock" @click="handleRelease(scope.row)" v-hasPermi="['wms:lock:edit']">释放</el-button>
            <el-button v-if="canEdit(scope.row)" link type="primary" icon="Edit" @click="handleUpdate(scope.row)" v-hasPermi="['wms:lock:edit']">修改</el-button>
            <el-button v-else link type="info" icon="View" @click="handleView(scope.row)">明细</el-button>
            <el-button v-if="canEdit(scope.row)" link type="danger" icon="Delete" @click="handleDelete(scope.row)" v-hasPermi="['wms:lock:remove']">删除</el-button>
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
        <el-form ref="lockRef" :model="form" :rules="rules" label-width="88px" class="nowrap-labels" :disabled="viewMode">
          <el-row :gutter="16">
            <el-col :span="12">
              <el-form-item label="锁定类型" prop="lockType">
                <el-select v-model="form.lockType" placeholder="请选择锁定类型" style="width:100%">
                  <el-option v-for="(label, value) in lockTypeMap" :key="value" :label="label" :value="value" />
                </el-select>
              </el-form-item>
            </el-col>
            <el-col :span="12">
              <el-form-item label="来源单据号">
                <el-input v-model="form.sourceNo" placeholder="来源单据号（选填）" maxlength="50" />
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
                  :placeholder="form.locationId ? '选择要锁定的库存行（同物料同批次可能多条码）' : '先选库位'"
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
              <el-form-item label="锁定数量" prop="lockQuantity">
                <el-input-number v-model="form.lockQuantity" :min="0" :controls="false"
                  :placeholder="form.stockAvailable != null ? `可锁定 ${form.stockAvailable}` : '锁定数量'"
                  style="width:100%" />
              </el-form-item>
            </el-col>
            <el-col :span="12">
              <el-form-item label="过期时间">
                <el-date-picker v-model="form.expireTime" type="datetime" value-format="YYYY-MM-DD HH:mm:ss"
                  placeholder="过期时间（选填）" style="width:100%" />
              </el-form-item>
            </el-col>
          </el-row>

          <div class="form-tip text-muted">
            精确到库存行：同物料同批次多条码时必须指定锁哪一行。执行锁定后该行可用库存减少、锁定数量增加，库存总量不变；
            出库时优先消耗锁定量并自动核销锁定单（全部消耗自动转「已完成」）；释放后按未使用数量回补。
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

<script setup name="WmsLock">
import { ref, reactive, toRefs, computed } from "vue";
import { ElMessage, ElMessageBox } from "element-plus";
import {
  listLock,
  getLock,
  addLock,
  updateLock,
  delLock,
  executeLock,
  releaseLock,
} from "@/api/wms/lock";
import { listStock } from "@/api/wms/stock";
import QueryForm from "@/components/QueryForm/index.vue";
import RightToolbar from "@/components/RightToolbar/index.vue";
import WarehouseSelect from "@/views/wms/warehouse/input.vue";
import LocationCascader from "@/views/wms/location/cascader.vue";

const lockRef = ref(null);
const lockList = ref([]);
const stockOptions = ref([]);
const stockLoading = ref(false);
const open = ref(false);
const loading = ref(true);
const ids = ref([]);
const single = ref(true);
const multiple = ref(true);
const total = ref(0);
const title = ref("");
/** 只读查看模式：已锁定/已释放后只用于查看 */
const viewMode = ref(false);
/** 列表当前勾选的行 */
const selectedRows = ref([]);

const showSearch = ref(true);

// 锁定类型
const lockTypeMap = {
  order: "订单预留",
  produce: "生产预留",
  exhibit: "展会预留",
  manual: "手动锁定",
};

// 锁定状态：待锁定 → 已锁定 / 部分释放 → 已释放
const lockStatusMap = { pending: "待锁定", locked: "已锁定", partial: "部分释放", released: "已释放" };
const lockStatusTag = (s) => (s === "locked" || s === "partial" ? "warning" : s === "released" ? "success" : "info");

/** 已开始锁定（已锁定/部分释放/已释放）的单据不能再修改或删除 */
function canEdit(row) {
  return row.lockStatus !== "locked" && row.lockStatus !== "partial" && row.lockStatus !== "released";
}

const data = reactive({
  form: {},
  queryParams: {
    pageNum: 1,
    pageSize: 10,
    keywords: undefined,
    lockStatus: undefined,
  },
  rules: {
    warehouseId: [{ required: true, message: "请选择仓库", trigger: "change" }],
    materialId: [{ required: true, message: "请选择物料", trigger: "change" }],
    locationId: [{ required: true, message: "请选择库位", trigger: "change" }],
    lockQuantity: [{ required: true, message: "请输入锁定数量", trigger: "blur" }],
    lockType: [{ required: true, message: "请选择锁定类型", trigger: "change" }],
  },
});

const { form, queryParams, rules } = toRefs(data);

const queryConfig = computed(() => [
  {
    label: "关键字",
    prop: "keywords",
    type: "input",
    placeholder: "请输入物料编码/名称",
  },
  {
    label: "状态",
    prop: "lockStatus",
    type: "select",
    placeholder: "请选择状态",
    options: [
      { label: "待锁定", value: "pending" },
      { label: "已锁定", value: "locked" },
      { label: "部分释放", value: "partial" },
      { label: "已释放", value: "released" },
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
  listLock(queryParams.value).then((res) => {
    lockList.value = res.rows || [];
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
    lockStatus: undefined,
    pageNum: 1,
    pageSize: 10,
  };
  handleQuery();
}

/** 多选框 */
function handleSelectionChange(selection) {
  selectedRows.value = selection;
  ids.value = selection.map((item) => item.lockId);
  single.value = selection.length !== 1;
  multiple.value = !selection.length;
}

/** 表单重置 */
function reset() {
  form.value = {
    lockId: undefined,
    lockType: undefined,
    sourceType: undefined,
    sourceId: undefined,
    sourceNo: undefined,
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
    lockQuantity: undefined,
    usedQuantity: undefined,
    lockStatus: undefined,
    // 执行锁定时由后端回填，编辑时原样带回避免被清空
    lockUserId: undefined,
    lockUserName: undefined,
    lockTime: undefined,
    expireTime: undefined,
  };
  viewMode.value = false;
  if (lockRef.value) lockRef.value.resetFields();
}

/** 新增 */
function handleAdd() {
  reset();
  open.value = true;
  title.value = "添加库存锁定";
}

/** 修改 */
function handleUpdate(row) {
  reset();
  const id = row?.lockId || ids.value[0];
  getLock(id).then((res) => fillForm(res.data || {}));
}

/** 查看明细：已锁定/已释放后只读打开 */
function handleView(row) {
  if (!row?.lockId) return;
  reset();
  viewMode.value = true;
  getLock(row.lockId).then((res) => fillForm(res.data || {}));
}

/** 填充表单 */
function fillForm(data) {
  form.value = {
    lockId: data.lockId,
    lockType: data.lockType,
    sourceType: data.sourceType,
    sourceId: data.sourceId,
    sourceNo: data.sourceNo,
    warehouseId: data.warehouseId,
    materialId: data.materialId,
    materialCode: data.materialCode || data.material?.materialCode || "",
    materialName: data.materialName || data.material?.materialName || "",
    batchNo: data.batchNo,
    zoneId: data.zoneId,
    locationId: data.locationId,
    locCascader: data.locationId ? [data.zoneId, data.locationId].filter(Boolean) : [],
    stockId: data.stockId,
    lockQuantity: data.lockQuantity,
    usedQuantity: data.usedQuantity,
    lockStatus: data.lockStatus,
    lockUserId: data.lockUserId,
    lockUserName: data.lockUserName,
    lockTime: data.lockTime,
    expireTime: data.expireTime ? String(data.expireTime).slice(0, 19) : undefined,
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
  title.value = viewMode.value ? "锁定单明细" : "修改库存锁定";
}

/** 库位选择（两级：库区 → 库位） */
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

/** 库存行选择：锁定物料/批次，带出可用量并默认全量锁定 */
function onStockPick(val) {
  const opt = stockOptions.value.find((o) => o.value === val);
  if (!opt) return;
  form.value.stockId = opt.stockId;
  form.value.stockAvailable = opt.available;
  form.value.materialId = opt.materialId;
  form.value.materialCode = opt.materialCode;
  form.value.materialName = opt.materialName;
  form.value.batchNo = opt.batchNo;
  form.value.lockQuantity = opt.available > 0 ? opt.available : undefined;
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
  lockRef.value.validate((valid) => {
    if (!valid) return;
    if (!form.value.locationId) {
      ElMessage.warning("请选择库位");
      return;
    }
    if (!form.value.stockId) {
      ElMessage.warning("请选择库存行（同物料同批次可能有多条码，必须精确到行）");
      return;
    }
    if (!(form.value.lockQuantity > 0)) {
      ElMessage.warning("锁定数量必须大于 0");
      return;
    }
    if (form.value.stockAvailable != null && form.value.lockQuantity > form.value.stockAvailable) {
      ElMessage.warning(`锁定数量超过该库存行可用量（可用 ${form.value.stockAvailable}）`);
      return;
    }

    const payload = {
      lockId: form.value.lockId,
      lockType: form.value.lockType,
      sourceType: form.value.sourceType,
      sourceId: form.value.sourceId,
      sourceNo: form.value.sourceNo,
      warehouseId: form.value.warehouseId,
      materialId: form.value.materialId,
      materialCode: form.value.materialCode,
      materialName: form.value.materialName,
      batchNo: form.value.batchNo,
      zoneId: form.value.zoneId,
      locationId: form.value.locationId,
      stockId: form.value.stockId,
      lockQuantity: form.value.lockQuantity,
      usedQuantity: form.value.usedQuantity,
      lockStatus: form.value.lockStatus,
      lockUserId: form.value.lockUserId,
      lockUserName: form.value.lockUserName,
      lockTime: form.value.lockTime,
      expireTime: form.value.expireTime,
    };

    const api = form.value.lockId ? updateLock : addLock;
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

/** 执行锁定 */
function handleExecute(row) {
  ElMessageBox.confirm(
    `执行锁定单「${row.lockId}」将占用该库位的可用库存（库存总量不变），执行后不可修改。是否继续？`,
    "锁定确认",
    { confirmButtonText: "确定锁定", cancelButtonText: "取消", type: "warning" }
  )
    .then(() => executeLock(row.lockId))
    .then(() => {
      ElMessage.success("锁定成功，可用库存已扣减");
      getList();
    });
}

/** 释放锁定 */
function handleRelease(row) {
  ElMessageBox.confirm(
    `释放锁定单「${row.lockId}」将回补该库位的可用库存（已消耗部分不退回）。是否继续？`,
    "释放确认",
    { confirmButtonText: "确定释放", cancelButtonText: "取消", type: "warning" }
  )
    .then(() => releaseLock(row.lockId))
    .then(() => {
      ElMessage.success("已释放，可用库存已回补");
      getList();
    });
}

/** 删除 */
function handleDelete(row) {
  const delIds = row.lockId || ids.value.join(",");
  ElMessageBox.confirm(`是否确认删除锁定单编号为"${delIds}"的数据项？`, "警告", {
    confirmButtonText: "确定",
    cancelButtonText: "取消",
    type: "warning",
  })
    .then(() => delLock(delIds))
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
