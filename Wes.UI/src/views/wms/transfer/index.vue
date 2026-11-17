<template>
  <div class="app-container">
    <div class="main-panel p16">
      <query-form :config="queryConfig" v-model:visible="showSearch" v-model="queryParams" @query="handleQuery" @reset="resetQuery" />

      <el-row :gutter="10" class="mb8">
        <el-col :span="1.5">
          <el-button type="primary" plain icon="Plus" @click="handleAdd" v-hasPermi="['wms:transfer:add']">新增</el-button>
        </el-col>
        <el-col :span="1.5">
          <el-button v-if="canEdit(selectedRows[0] || {})" type="success" plain icon="Edit" :disabled="single" @click="handleUpdate" v-hasPermi="['wms:transfer:edit']">修改</el-button>
          <el-button v-else type="info" plain icon="View" :disabled="single" @click="handleView(selectedRows[0])">明细</el-button>
        </el-col>
        <el-col :span="1.5">
          <el-button type="warning" plain icon="Upload" :disabled="single || !canOut(selectedRows[0])" @click="handleOut" v-hasPermi="['wms:transfer:out']">移出</el-button>
        </el-col>
        <el-col :span="1.5">
          <el-button type="success" plain icon="Download" :disabled="single || !canIn(selectedRows[0])" @click="handleIn" v-hasPermi="['wms:transfer:in']">移入</el-button>
        </el-col>
        <el-col :span="1.5">
          <el-button type="danger" plain icon="Delete" :disabled="multiple || selectedRows.some((r) => !canEdit(r))" @click="handleDelete" v-hasPermi="['wms:transfer:remove']">删除</el-button>
        </el-col>
        <right-toolbar v-model:showSearch="showSearch" @queryTable="getList"></right-toolbar>
      </el-row>

      <el-table v-loading="loading" :data="transferList" @selection-change="handleSelectionChange">
        <el-table-column type="selection" width="55" align="center" />
        <el-table-column label="调拨单号" align="center" prop="transferNo" :show-overflow-tooltip="true" />
        <el-table-column label="调出仓库" align="center" min-width="120" :show-overflow-tooltip="true">
          <template #default="scope">
            {{ scope.row.fromWarehouse?.warehouseName || "-" }}
          </template>
        </el-table-column>
        <el-table-column label="调入仓库" align="center" min-width="120" :show-overflow-tooltip="true">
          <template #default="scope">
            {{ scope.row.toWarehouse?.warehouseName || "-" }}
          </template>
        </el-table-column>
        <el-table-column label="总数" align="center" prop="totalQuantity" width="80" />
        <el-table-column label="已出库" align="center" prop="outQuantity" width="80" />
        <el-table-column label="已入库" align="center" prop="inQuantity" width="80" />
        <el-table-column label="在途" align="center" width="80">
          <template #default="scope">
            <span :class="{ 'text-warn': (scope.row.inTransitQuantity ?? 0) > 0 }">
              {{ scope.row.inTransitQuantity ?? 0 }}
            </span>
          </template>
        </el-table-column>
        <el-table-column label="状态" align="center" width="100">
          <template #default="scope">
            <el-tag :type="statusTag(scope.row.transferStatus)">
              {{ statusMap[scope.row.transferStatus] || "-" }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column label="移出人/时间" align="center" min-width="170">
          <template #default="scope">
            {{ scope.row.outBy ? scope.row.outBy + " " + (scope.row.outTime || "").slice(5, 16) : "-" }}
          </template>
        </el-table-column>
        <el-table-column label="移入人/时间" align="center" min-width="170">
          <template #default="scope">
            {{ scope.row.inBy ? scope.row.inBy + " " + (scope.row.inTime || "").slice(5, 16) : "-" }}
          </template>
        </el-table-column>
        <el-table-column label="操作" align="center" width="150" class-name="small-padding fixed-width">
          <template #default="scope">
            <el-button v-if="canOut(scope.row)" link type="warning" icon="Upload" @click="handleOut(scope.row)" v-hasPermi="['wms:transfer:out']">移出</el-button>
            <el-button v-if="canIn(scope.row)" link type="success" icon="Download" @click="handleIn(scope.row)" v-hasPermi="['wms:transfer:in']">移入</el-button>
            <el-button v-if="canEdit(scope.row)" link type="danger" icon="Delete" @click="handleDelete(scope.row)" v-hasPermi="['wms:transfer:remove']">删除</el-button>
          </template>
        </el-table-column>
      </el-table>

      <pagination v-show="total > 0" :total="total" v-model:page="queryParams.pageNum" v-model:limit="queryParams.pageSize" @pagination="getList" />

      <!-- 新增/修改 -->
      <el-dialog :title="title" v-model="open" width="1040px" append-to-body>
        <el-form ref="transferRef" :model="form" :rules="rules" label-width="92px" class="nowrap-labels" :disabled="viewMode">
          <el-row :gutter="16">
            <el-col :span="8">
              <el-form-item label="调拨单号">
                <el-input v-model="form.transferNo" placeholder="保存后自动生成" readonly />
              </el-form-item>
            </el-col>
            <el-col :span="8">
              <el-form-item label="调出仓库" prop="fromWarehouseId">
                <WarehouseSelect v-model="form.fromWarehouseId" placeholder="请选择调出仓库" @change="onWarehouseChange" />
              </el-form-item>
            </el-col>
            <el-col :span="8">
              <el-form-item label="调入仓库" prop="toWarehouseId">
                <WarehouseSelect v-model="form.toWarehouseId" placeholder="请选择调入仓库" />
              </el-form-item>
            </el-col>
            <el-col :span="8">
              <el-form-item label="调拨类型">
                <el-select v-model="form.transferType" placeholder="请选择类型" style="width: 100%">
                  <el-option label="普通调拨" value="normal" />
                  <el-option label="紧急调拨" value="urgent" />
                </el-select>
              </el-form-item>
            </el-col>
            <el-col :span="16">
              <el-form-item label="调拨原因">
                <el-input v-model="form.transferReason" placeholder="如：产线缺料，从主仓调拨" maxlength="255" />
              </el-form-item>
            </el-col>
            <el-col :span="24">
              <el-form-item label="备注">
                <el-input v-model="form.remark" type="textarea" :autosize="{ minRows: 2, maxRows: 4 }" placeholder="请输入备注" maxlength="500" />
              </el-form-item>
            </el-col>
          </el-row>
        </el-form>

        <el-row :gutter="10" class="mb8">
          <el-col :span="2">
            <el-button v-if="!form.transferId && !viewMode" class="detail-add-btn" type="primary" plain icon="Plus" @click="openStockDialog">添加明细</el-button>
          </el-col>
          <span class="text-muted">合计 {{ totalQty }}；数量跟随库存行不可改（一条库存一个条码），收货库位不用在这里填，货到了由调入仓现场定</span>
        </el-row>

        <!-- 库存行一律从弹窗多选进来，行内不可再换，要改就删行重挑 -->
        <SelectStock
          ref="stockSelectRef"
          :is-single="false"
          :warehouse-id="form.fromWarehouseId"
          @handle-data="onStockSelected"
          v-show="false"
        />

        <el-table :data="form.details" border size="small" max-height="360">
          <el-table-column label="库存（物料 / 批次 / 调出库位）" align="center" min-width="320">
            <template #default="scope">
              <span>{{ scope.row.stockLabel }}</span>
            </template>
          </el-table-column>
          <el-table-column label="调拨数量" align="center" width="110">
            <template #default="scope">
              <span title="一条库存一个条码，数量跟随库存行，改了就跟标签对不上">
                {{ scope.row.transferQuantity }}
              </span>
            </template>
          </el-table-column>
          <el-table-column label="备注" align="center" min-width="120">
            <template #default="scope">
              <el-input v-model="scope.row.remark" size="small" placeholder="备注" maxlength="255" />
            </template>
          </el-table-column>
          <el-table-column label="操作" align="center" width="60">
            <template #default="scope">
              <el-button link type="danger" icon="Delete" @click="removeDetail(scope.$index)"></el-button>
            </template>
          </el-table-column>
        </el-table>

        <template #footer>
          <div class="dialog-footer">
            <el-button v-if="!viewMode" type="primary" @click="submitForm">确 定</el-button>
            <el-button @click="cancel">{{ viewMode ? "关 闭" : "取 消" }}</el-button>
          </div>
        </template>
      </el-dialog>

      <!-- 移入收货 -->
      <el-dialog title="调拨收货" v-model="inOpen" width="900px" append-to-body>
        <el-alert type="info" show-icon :closable="false" class="mb8">
          单号 {{ inForm.transferNo }}　按实际到货数量填写；少收的部分会继续记为在途，不会凭空消失
        </el-alert>
        <el-table :data="inForm.items" border size="small" max-height="420">
          <el-table-column label="物料" align="center" min-width="180" :show-overflow-tooltip="true">
            <template #default="scope">
              {{ scope.row.materialCode }} {{ scope.row.materialName }}
            </template>
          </el-table-column>
          <el-table-column label="批次" align="center" prop="batchNo" width="110" />
          <el-table-column label="条码" align="center" width="130" :show-overflow-tooltip="true">
            <template #default="scope">
              {{ scope.row.barcode || "-" }}
            </template>
          </el-table-column>
          <el-table-column label="在途" align="center" width="80">
            <template #default="scope">
              <span class="text-warn">{{ scope.row.transitQty }}</span>
            </template>
          </el-table-column>
          <el-table-column label="实收数量" align="center" width="130">
            <template #default="scope">
              <el-input-number v-model="scope.row.quantity" :min="0" :max="scope.row.transitQty"
                :precision="3" :controls="false" size="small" style="width: 100%" />
            </template>
          </el-table-column>
          <el-table-column label="收货库位" align="center" width="220">
            <template #default="scope">
              <LocationCascader
                v-model="scope.row.toLocCascader"
                :warehouse-id="inForm.toWarehouseId"
                :placeholder="inForm.toWarehouseId ? '库区/库位' : '请先选仓库'"
                @update:model-value="(val) => onInLocation(val, scope.$index)" />
            </template>
          </el-table-column>
        </el-table>
        <template #footer>
          <div class="dialog-footer">
            <el-button type="primary" @click="submitIn">确认收货</el-button>
            <el-button @click="inOpen = false">取 消</el-button>
          </div>
        </template>
      </el-dialog>
    </div>
  </div>
</template>

<script setup name="WmsTransfer">
import { ref, reactive, toRefs, computed } from "vue";
import { ElMessage, ElMessageBox } from "element-plus";
import {
  listTransfer, getTransfer, addTransfer, updateTransfer, delTransfer, outTransfer, inTransfer,
} from "@/api/wms/transfer";
import QueryForm from "@/components/QueryForm/index.vue";
import RightToolbar from "@/components/RightToolbar/index.vue";
import WarehouseSelect from "@/views/wms/warehouse/input.vue";
import SelectStock from "@/views/wms/stock/select.vue";
import LocationCascader from "@/views/wms/location/cascader.vue";

const transferRef = ref(null);
const transferList = ref([]);
const open = ref(false);
const loading = ref(false);
const ids = ref([]);
const single = ref(true);
const multiple = ref(true);
const total = ref(0);
const title = ref("");
const viewMode = ref(false);
const selectedRows = ref([]);
/** 库存多选弹窗（明细行一律从这里进来） */
const stockSelectRef = ref(null);
const showSearch = ref(true);

// 收货弹窗
const inOpen = ref(false);
const inForm = ref({ transferId: undefined, transferNo: "", toWarehouseId: undefined, items: [] });

// 待调拨 → 移出 → 在途 → 移入 → 已完成
const statusMap = {
  pending: "待调拨",
  in_transit: "在途",
  completed: "已完成",
  cancelled: "已取消",
};
const statusTag = (s) =>
  s === "pending" ? "info" : s === "in_transit" ? "warning" : s === "completed" ? "success" : "danger";

/** 只有待调拨的单据能改能删 */
function canEdit(row) {
  return (row ?? {}).transferStatus === "pending";
}
/** 待调拨才能移出 */
function canOut(row) {
  return (row ?? {}).transferStatus === "pending";
}
/** 在途才能收货 */
function canIn(row) {
  return (row ?? {}).transferStatus === "in_transit";
}

const data = reactive({
  form: {},
  queryParams: {
    pageNum: 1,
    pageSize: 10,
    keywords: undefined,
    transferStatus: undefined,
  },
  rules: {
    fromWarehouseId: [{ required: true, message: "请选择调出仓库", trigger: "change" }],
    toWarehouseId: [{ required: true, message: "请选择调入仓库", trigger: "change" }],
  },
});

const { form, queryParams, rules } = toRefs(data);

const queryConfig = computed(() => [
  { label: "关键字", prop: "keywords", type: "input", placeholder: "调拨单号" },
  {
    label: "状态",
    prop: "transferStatus",
    type: "select",
    placeholder: "请选择状态",
    options: [
      { label: "待调拨", value: "pending" },
      { label: "在途", value: "in_transit" },
      { label: "已完成", value: "completed" },
      { label: "已取消", value: "cancelled" },
    ],
  },
]);

const totalQty = computed(() =>
  (form.value.details || []).reduce((s, d) => s + Number(d.transferQuantity || 0), 0)
);

function getList() {
  loading.value = true;
  listTransfer(queryParams.value).then((res) => {
    transferList.value = res.rows || [];
    total.value = res.total || 0;
    loading.value = false;
  });
}

/** 标签页重新激活时由布局回调（是否刷新由页面决定） */
function tabRefresh() {
  getList();
}

function handleQuery() {
  queryParams.value.pageNum = 1;
  getList();
}

function resetQuery() {
  queryParams.value = { pageNum: 1, pageSize: 10, keywords: undefined, transferStatus: undefined };
  handleQuery();
}

function handleSelectionChange(selection) {
  selectedRows.value = selection;
  ids.value = selection.map((item) => item.transferId);
  single.value = selection.length !== 1;
  multiple.value = !selection.length;
}

function reset() {
  form.value = {
    transferId: undefined,
    transferNo: undefined,
    fromWarehouseId: undefined,
    toWarehouseId: undefined,
    transferType: "normal",
    transferReason: undefined,
    remark: undefined,
    transferStatus: undefined,
    details: [],
  };
  viewMode.value = false;
  if (transferRef.value) transferRef.value.resetFields();
}

function handleAdd() {
  reset();
  open.value = true;
  title.value = "添加调拨单";
}

function handleUpdate(row) {
  reset();
  const id = row?.transferId || ids.value[0];
  getTransfer(id).then((res) => fillForm(res.data || {}));
}

function handleView(row) {
  if (!row?.transferId) return;
  reset();
  viewMode.value = true;
  getTransfer(row.transferId).then((res) => fillForm(res.data || {}));
  title.value = "调拨单明细";
}

function fillForm(d) {
  form.value = {
    transferId: d.transferId,
    transferNo: d.transferNo,
    fromWarehouseId: d.fromWarehouseId,
    toWarehouseId: d.toWarehouseId,
    transferType: d.transferType || "normal",
    transferReason: d.transferReason,
    remark: d.remark,
    transferStatus: d.transferStatus,
    details: (d.details || []).map((x) => ({
      detailId: x.detailId,
      // 明细没存库存行 id，编辑时用文本把当前这行显示出来（真值仍在下面几个字段里）
      stockId: undefined,
      stockLabel: [x.materialCode, x.materialName, x.batchNo ? "批次" + x.batchNo : "无批次", x.fromLocationCode]
        .filter(Boolean).join(" / "),
      materialId: x.materialId,
      materialCode: x.materialCode,
      materialName: x.materialName,
      batchNo: x.batchNo,
      transferQuantity: x.transferQuantity,
      fromZoneId: x.fromZoneId,
      fromLocationId: x.fromLocationId,
      remark: x.remark,
    })),
  };
  open.value = true;
  title.value = "修改调拨单";
}

/**
 * 调出仓变更：换了仓，已挑的库存行必然不属于新仓，而库存列行内不可改，
 * 留着只会让人以为改改就行——直接整表作废，让人重新挑。
 */
function onWarehouseChange() {
  if ((form.value.details || []).length === 0) return;
  form.value.details = [];
  ElMessage.warning("调出仓库已变更，明细已清空，请重新挑选库存");
}



/**
 * 打开库存多选弹窗。
 * 调拨搬的是调出仓里具体的一行库存，一次挑多行直接生成明细，
 * 比"插个空行再逐行选"顺手，也不会出现空行里库存点不动的死角。
 */
function openStockDialog() {
  if (!form.value.fromWarehouseId) {
    ElMessage.warning("请先选择调出仓库，再挑选要调的库存");
    return;
  }
  stockSelectRef.value?.open();
}

/** 多选库存 → 批量生成明细行；同一行库存已在明细里则跳过，避免同一批货调两次 */
function onStockSelected(items) {
  if (!items || items.length === 0) return;
  if (!form.value.details) form.value.details = [];
  let skipped = 0;
  for (const stock of items) {
    if (form.value.details.some((d) => d.stockId === stock.stockId)) {
      skipped++;
      continue;
    }
    const available = (stock.stockQuantity ?? 0) - (stock.freezeQuantity ?? 0) - (stock.lockQuantity ?? 0);
    form.value.details.push({
      detailId: undefined,
      stockId: stock.stockId,
      stockLabel: `${stock.materialCode || ""} ${stock.materialName || ""} / ${stock.batchNo || "无批次"} / ${stock.locationCode || ""}`,
      materialId: stock.materialId,
      materialCode: stock.materialCode || "",
      materialName: stock.materialName || "",
      batchNo: stock.batchNo,
      transferQuantity: available > 0 ? available : undefined,
      fromZoneId: stock.zoneId,
      fromLocationId: stock.locationId,
      barcode: stock.barcode || "",
      remark: undefined,
    });
  }
  if (skipped > 0) ElMessage.info(`有 ${skipped} 行库存已在明细中，已跳过`);
}

function removeDetail(index) {
  form.value.details.splice(index, 1);
}

function cancel() {
  open.value = false;
  reset();
}

function submitForm() {
  transferRef.value.validate((valid) => {
    if (!valid) return;

    if (form.value.fromWarehouseId === form.value.toWarehouseId) {
      ElMessage.warning("调出与调入仓库相同，库内搬货请使用移库单！");
      return;
    }

    const details = form.value.details || [];
    if (details.length === 0) {
      ElMessage.warning("请至少添加一条调拨明细！");
      return;
    }
    for (const d of details) {
      if (!d.materialId) {
        ElMessage.warning("请选择调拨物料！");
        return;
      }
      if (!(d.transferQuantity > 0)) {
        ElMessage.warning("调拨数量必须大于0！");
        return;
      }
      if (!d.fromLocationId) {
        ElMessage.warning("请选择调出库位！");
        return;
      }
    }

    const payload = {
      transferId: form.value.transferId,
      transferNo: form.value.transferNo,
      fromWarehouseId: form.value.fromWarehouseId,
      toWarehouseId: form.value.toWarehouseId,
      transferType: form.value.transferType,
      transferReason: form.value.transferReason,
      remark: form.value.remark,
      transferStatus: form.value.transferStatus,
      details: details.map((x) => ({
        detailId: x.detailId,
        materialId: x.materialId,
        batchNo: x.batchNo,
        fromZoneId: x.fromZoneId,
        fromLocationId: x.fromLocationId,
        transferQuantity: x.transferQuantity,
        remark: x.remark,
      })),
    };

    const api = form.value.transferId ? updateTransfer : addTransfer;
    api(payload).then(() => {
      ElMessage.success("保存成功");
      open.value = false;
      getList();
    });
  });
}

/** 移出 */
function handleOut(row) {
  const target = row?.transferId ? row : selectedRows.value[0];
  if (!target) return;
  ElMessageBox.confirm(
    `确认对调拨单「${target.transferNo}」执行移出？将扣减调出仓的库存，货物进入在途状态，等待调入仓收货。`,
    "移出确认",
    { confirmButtonText: "确定移出", cancelButtonText: "取消", type: "warning" }
  )
    .then(() => outTransfer(target.transferId))
    .then((res) => {
      ElMessage.success(res.msg || "移出成功");
      getList();
    });
}

/** 打开收货弹窗：只带在途数量大于 0 的明细 */
function handleIn(row) {
  const target = row?.transferId ? row : selectedRows.value[0];
  if (!target) return;
  getTransfer(target.transferId).then((res) => {
    const d = res.data || {};
    inForm.value = {
      transferId: d.transferId,
      transferNo: d.transferNo,
      toWarehouseId: d.toWarehouseId,
      items: (d.details || [])
        .filter((x) => (x.inTransitQuantity ?? 0) > 0)
        .map((x) => ({
          detailId: x.detailId,
          materialCode: x.materialCode,
          materialName: x.materialName,
          batchNo: x.batchNo,
          barcode: x.barcode,
          transitQty: x.inTransitQuantity ?? 0,
          quantity: x.inTransitQuantity ?? 0,
          toLocCascader: [],
          toZoneId: undefined,
          toLocationId: undefined,
        })),
    };
    inOpen.value = true;
  });
}

function onInLocation(val, index) {
  const row = inForm.value.items[index];
  row.toZoneId = val?.[0];
  row.toLocationId = val?.[1];
}

/** 提交收货 */
function submitIn() {
  const items = inForm.value.items.filter((i) => i.quantity > 0);
  if (items.length === 0) {
    ElMessage.warning("请填写实收数量！");
    return;
  }
  for (const i of items) {
    if (!i.toLocationId) {
      ElMessage.warning(`请选择【${i.materialName}】的收货库位！`);
      return;
    }
    if (i.quantity > i.transitQty) {
      ElMessage.warning(`【${i.materialName}】实收不能超过在途数量！`);
      return;
    }
  }

  inTransfer({
    transferId: inForm.value.transferId,
    items: items.map((i) => ({
      detailId: i.detailId,
      quantity: i.quantity,
      toLocationId: i.toLocationId,
    })),
  }).then((res) => {
    ElMessage.success(res.msg || "收货成功");
    inOpen.value = false;
    getList();
  });
}

function handleDelete(row) {
  const delIds = row.transferId || ids.value.join(",");
  ElMessageBox.confirm(`是否确认删除调拨单编号为"${delIds}"的数据项？`, "警告", {
    confirmButtonText: "确定",
    cancelButtonText: "取消",
    type: "warning",
  })
    .then(() => delTransfer(delIds))
    .then(() => {
      getList();
      ElMessage.success("删除成功");
    });
}

getList();
</script>

<style scoped>
.text-muted {
  color: #909399;
  font-size: 12px;
}
.text-warn {
  color: var(--el-color-warning);
}
.mb8 {
  margin-bottom: 8px;
}
</style>
