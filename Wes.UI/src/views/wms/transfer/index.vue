<template>
  <div class="app-container">
    <div class="main-panel p16">
      <query-form :config="queryConfig" v-model:visible="showSearch" v-model="queryParams" @query="handleQuery" @reset="resetQuery" />

      <el-row :gutter="10" class="mb8">
        <el-col :span="1.5">
          <el-button type="primary" plain icon="Plus" @click="handleAdd" v-hasPermi="['wms:transfer:add']">{{ t('common.add') }}</el-button>
        </el-col>
        <el-col :span="1.5">
          <el-button v-if="canEdit(selectedRows[0] || {})" type="success" plain icon="Edit" :disabled="single" @click="handleUpdate" v-hasPermi="['wms:transfer:edit']">{{ t('common.edit') }}</el-button>
          <el-button v-else type="info" plain icon="View" :disabled="single" @click="handleView(selectedRows[0])">{{ t('wms.receipt.details') }}</el-button>
        </el-col>
        <el-col :span="1.5">
          <el-button type="warning" plain icon="Upload" :disabled="single || !canOut(selectedRows[0])" @click="handleOut" v-hasPermi="['wms:transfer:out']">{{ t('wms.transfer.out') }}</el-button>
        </el-col>
        <el-col :span="1.5">
          <el-button type="success" plain icon="Download" :disabled="single || !canIn(selectedRows[0])" @click="handleIn" v-hasPermi="['wms:transfer:in']">{{ t('wms.transfer.in') }}</el-button>
        </el-col>
        <el-col :span="1.5">
          <el-button type="danger" plain icon="Delete" :disabled="multiple || selectedRows.some((r) => !canEdit(r))" @click="handleDelete" v-hasPermi="['wms:transfer:remove']">{{ t('common.delete') }}</el-button>
        </el-col>
        <right-toolbar v-model:showSearch="showSearch" @queryTable="getList"></right-toolbar>
      </el-row>

      <el-table v-loading="loading" :data="transferList" @selection-change="handleSelectionChange">
        <el-table-column type="selection" width="55" align="center" />
        <el-table-column :label="t('wms.transfer.transferNo')" align="center" prop="transferNo" :show-overflow-tooltip="true" />
        <el-table-column :label="t('wms.transfer.fromWarehouse')" align="center" min-width="120" :show-overflow-tooltip="true">
          <template #default="scope">
            {{ scope.row.fromWarehouse?.warehouseName || "-" }}
          </template>
        </el-table-column>
        <el-table-column :label="t('wms.transfer.toWarehouse')" align="center" min-width="120" :show-overflow-tooltip="true">
          <template #default="scope">
            {{ scope.row.toWarehouse?.warehouseName || "-" }}
          </template>
        </el-table-column>
        <el-table-column :label="t('wms.transfer.total')" align="center" prop="totalQuantity" width="80" />
        <el-table-column :label="t('wms.transfer.outQty')" align="center" prop="outQuantity" width="80" />
        <el-table-column :label="t('wms.transfer.inQty')" align="center" prop="inQuantity" width="80" />
        <el-table-column :label="t('wms.transfer.inTransit')" align="center" width="80">
          <template #default="scope">
            <span :class="{ 'text-warn': (scope.row.inTransitQuantity ?? 0) > 0 }">
              {{ scope.row.inTransitQuantity ?? 0 }}
            </span>
          </template>
        </el-table-column>
        <el-table-column :label="t('common.status')" align="center" width="100">
          <template #default="scope">
            <el-tag :type="statusTag(scope.row.transferStatus)">
              {{ statusLabel(scope.row.transferStatus) }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column :label="t('wms.transfer.outByTime')" align="center" min-width="170">
          <template #default="scope">
            {{ scope.row.outBy ? scope.row.outBy + " " + (scope.row.outTime || "").slice(5, 16) : "-" }}
          </template>
        </el-table-column>
        <el-table-column :label="t('wms.transfer.inByTime')" align="center" min-width="170">
          <template #default="scope">
            {{ scope.row.inBy ? scope.row.inBy + " " + (scope.row.inTime || "").slice(5, 16) : "-" }}
          </template>
        </el-table-column>
        <el-table-column :label="t('common.actions')" align="center" width="150" class-name="small-padding fixed-width">
          <template #default="scope">
            <el-button v-if="canOut(scope.row)" link type="warning" icon="Upload" @click="handleOut(scope.row)" v-hasPermi="['wms:transfer:out']">{{ t('wms.transfer.out') }}</el-button>
            <el-button v-if="canIn(scope.row)" link type="success" icon="Download" @click="handleIn(scope.row)" v-hasPermi="['wms:transfer:in']">{{ t('wms.transfer.in') }}</el-button>
            <el-button v-if="canEdit(scope.row)" link type="danger" icon="Delete" @click="handleDelete(scope.row)" v-hasPermi="['wms:transfer:remove']">{{ t('common.delete') }}</el-button>
          </template>
        </el-table-column>
      </el-table>

      <pagination v-show="total > 0" :total="total" v-model:page="queryParams.pageNum" v-model:limit="queryParams.pageSize" @pagination="getList" />

      <!-- 新增/修改 -->
      <el-dialog :title="title" v-model="open" width="1040px" append-to-body>
        <el-form ref="transferRef" :model="form" :rules="rules" label-width="92px" class="nowrap-labels" :disabled="viewMode">
          <el-row :gutter="16">
            <el-col :span="8">
              <el-form-item :label="t('wms.transfer.transferNo')">
                <el-input v-model="form.transferNo" :placeholder="t('wms.receipt.receiptNoAuto')" readonly />
              </el-form-item>
            </el-col>
            <el-col :span="8">
              <el-form-item :label="t('wms.transfer.fromWarehouse')" prop="fromWarehouseId">
                <WarehouseSelect v-model="form.fromWarehouseId" :placeholder="t('wms.transfer.ph.selectFromWarehouse')" @change="onWarehouseChange" />
              </el-form-item>
            </el-col>
            <el-col :span="8">
              <el-form-item :label="t('wms.transfer.toWarehouse')" prop="toWarehouseId">
                <WarehouseSelect v-model="form.toWarehouseId" :placeholder="t('wms.transfer.ph.selectToWarehouse')" />
              </el-form-item>
            </el-col>
            <el-col :span="8">
              <el-form-item :label="t('wms.transfer.transferType')">
                <el-select v-model="form.transferType" :placeholder="t('wms.common.pleaseSelect')" style="width: 100%">
                  <el-option :label="t('wms.transfer.typeNormal')" value="normal" />
                  <el-option :label="t('wms.transfer.typeUrgent')" value="urgent" />
                </el-select>
              </el-form-item>
            </el-col>
            <el-col :span="16">
              <el-form-item :label="t('wms.transfer.transferReason')">
                <el-input v-model="form.transferReason" :placeholder="t('wms.transfer.ph.reasonExample')" maxlength="255" />
              </el-form-item>
            </el-col>
            <el-col :span="24">
              <el-form-item :label="t('common.remark')">
                <el-input v-model="form.remark" type="textarea" :autosize="{ minRows: 2, maxRows: 4 }" :placeholder="t('common.input')" maxlength="500" />
              </el-form-item>
            </el-col>
          </el-row>
        </el-form>

        <el-row :gutter="10" class="mb8">
          <el-col :span="2">
            <el-button v-if="!form.transferId && !viewMode" class="detail-add-btn" type="primary" plain icon="Plus" @click="openStockDialog">{{ t('wms.receiving.addDetail') }}</el-button>
          </el-col>
          <span class="text-muted">{{ t('wms.transfer.totalHint', { total: totalQty }) }}</span>
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
          <el-table-column :label="t('wms.transfer.stockCol')" align="center" min-width="320">
            <template #default="scope">
              <span>{{ scope.row.stockLabel }}</span>
            </template>
          </el-table-column>
          <el-table-column :label="t('wms.transfer.transferQty')" align="center" width="110">
            <template #default="scope">
              <span :title="t('wms.move.quantityTip')">
                {{ scope.row.transferQuantity }}
              </span>
            </template>
          </el-table-column>
          <el-table-column :label="t('common.remark')" align="center" min-width="120">
            <template #default="scope">
              <el-input v-model="scope.row.remark" size="small" :placeholder="t('common.remark')" maxlength="255" />
            </template>
          </el-table-column>
          <el-table-column :label="t('common.actions')" align="center" width="60">
            <template #default="scope">
              <el-button link type="danger" icon="Delete" @click="removeDetail(scope.$index)"></el-button>
            </template>
          </el-table-column>
        </el-table>

        <template #footer>
          <div class="dialog-footer">
            <el-button v-if="!viewMode" type="primary" @click="submitForm">{{ t('common.submit') }}</el-button>
            <el-button @click="cancel">{{ viewMode ? t('common.close') : t('common.cancel') }}</el-button>
          </div>
        </template>
      </el-dialog>

      <!-- 移入收货 -->
      <el-dialog :title="t('wms.transfer.inDialogTitle')" v-model="inOpen" width="900px" append-to-body>
        <el-alert type="info" show-icon :closable="false" class="mb8">
          {{ t('wms.transfer.inDialogAlert', { no: inForm.transferNo }) }}
        </el-alert>
        <el-table :data="inForm.items" border size="small" max-height="420">
          <el-table-column :label="t('wms.material.title')" align="center" min-width="180" :show-overflow-tooltip="true">
            <template #default="scope">
              {{ scope.row.materialCode }} {{ scope.row.materialName }}
            </template>
          </el-table-column>
          <el-table-column :label="t('wms.receiving.batchNo')" align="center" prop="batchNo" width="110" />
          <el-table-column :label="t('wms.receipt.barcode')" align="center" width="130" :show-overflow-tooltip="true">
            <template #default="scope">
              {{ scope.row.barcode || "-" }}
            </template>
          </el-table-column>
          <el-table-column :label="t('wms.transfer.inTransit')" align="center" width="80">
            <template #default="scope">
              <span class="text-warn">{{ scope.row.transitQty }}</span>
            </template>
          </el-table-column>
          <el-table-column :label="t('wms.transfer.receivedQty')" align="center" width="130">
            <template #default="scope">
              <el-input-number v-model="scope.row.quantity" :min="0" :max="scope.row.transitQty"
                :precision="3" :controls="false" size="small" style="width: 100%" />
            </template>
          </el-table-column>
          <el-table-column :label="t('wms.transfer.receiveLocation')" align="center" width="220">
            <template #default="scope">
              <LocationCascader
                v-model="scope.row.toLocCascader"
                :warehouse-id="inForm.toWarehouseId"
                :placeholder="inForm.toWarehouseId ? t('wms.move.ph.zoneOrLocation') : t('wms.move.ph.selectWarehouseFirst')"
                @update:model-value="(val) => onInLocation(val, scope.$index)" />
            </template>
          </el-table-column>
        </el-table>
        <template #footer>
          <div class="dialog-footer">
            <el-button type="primary" @click="submitIn">{{ t('wms.transfer.confirmReceive') }}</el-button>
            <el-button @click="inOpen = false">{{ t('common.cancel') }}</el-button>
          </div>
        </template>
      </el-dialog>
    </div>
  </div>
</template>

<script setup name="WmsTransfer">
import { ref, reactive, toRefs, computed } from "vue";
import { ElMessage, ElMessageBox } from "element-plus";
import { useI18n } from "vue-i18n";
import {
  listTransfer, getTransfer, addTransfer, updateTransfer, delTransfer, outTransfer, inTransfer,
} from "@/api/wms/transfer";
import QueryForm from "@/components/QueryForm/index.vue";
import RightToolbar from "@/components/RightToolbar/index.vue";
import WarehouseSelect from "@/views/wms/warehouse/input.vue";
import SelectStock from "@/views/wms/stock/select.vue";
import LocationCascader from "@/views/wms/location/cascader.vue";

const { t } = useI18n();
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
function statusLabel(s) {
  const map = {
    pending: t('wms.transfer.statusPending'),
    in_transit: t('wms.transfer.statusInTransit'),
    completed: t('wms.returnOrder.statusCompleted'),
    cancelled: t('wms.receiving.statusCancelled'),
  };
  return map[s] || "-";
}
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
    fromWarehouseId: [{ required: true, message: () => t('wms.transfer.ph.selectFromWarehouse'), trigger: "change" }],
    toWarehouseId: [{ required: true, message: () => t('wms.transfer.ph.selectToWarehouse'), trigger: "change" }],
  },
});

const { form, queryParams, rules } = toRefs(data);

const queryConfig = computed(() => [
  { label: t('wms.common.keywords'), prop: "keywords", type: "input", placeholder: t('wms.transfer.ph.inputTransferNo') },
  {
    label: t('common.status'),
    prop: "transferStatus",
    type: "select",
    placeholder: t('wms.warehouse.ph.selectStatus'),
    options: [
      { label: t('wms.transfer.statusPending'), value: "pending" },
      { label: t('wms.transfer.statusInTransit'), value: "in_transit" },
      { label: t('wms.returnOrder.statusCompleted'), value: "completed" },
      { label: t('wms.receiving.statusCancelled'), value: "cancelled" },
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
  title.value = t('wms.transfer.addTitle');
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
  title.value = t('wms.transfer.viewTitle');
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
      stockLabel: [x.materialCode, x.materialName, x.batchNo ? `${t('wms.receiving.batchNo')}${x.batchNo}` : t('wms.move.noBatch'), x.fromLocationCode]
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
  title.value = t('wms.transfer.editTitle');
}

/**
 * 调出仓变更：换了仓，已挑的库存行必然不属于新仓，而库存列行内不可改，
 * 留着只会让人以为改改就行——直接整表作废，让人重新挑。
 */
function onWarehouseChange() {
  if ((form.value.details || []).length === 0) return;
  form.value.details = [];
  ElMessage.warning(t('wms.transfer.fromWarehouseChanged'));
}



/**
 * 打开库存多选弹窗。
 * 调拨搬的是调出仓里具体的一行库存，一次挑多行直接生成明细，
 * 比"插个空行再逐行选"顺手，也不会出现空行里库存点不动的死角。
 */
function openStockDialog() {
  if (!form.value.fromWarehouseId) {
    ElMessage.warning(t('wms.transfer.selectFromWarehouseFirst'));
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
      stockLabel: `${stock.materialCode || ""} ${stock.materialName || ""} / ${stock.batchNo || t('wms.move.noBatch')} / ${stock.locationCode || ""}`,
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
  if (skipped > 0) ElMessage.info(t('wms.move.skippedRows', { count: skipped }));
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
      ElMessage.warning(t('wms.transfer.sameWarehouseWarn'));
      return;
    }

    const details = form.value.details || [];
    if (details.length === 0) {
      ElMessage.warning(t('wms.transfer.selectAtLeastOneDetail'));
      return;
    }
    for (const d of details) {
      if (!d.materialId) {
        ElMessage.warning(t('wms.transfer.selectMaterialRequired'));
        return;
      }
      if (!(d.transferQuantity > 0)) {
        ElMessage.warning(t('wms.transfer.qtyRequired'));
        return;
      }
      if (!d.fromLocationId) {
        ElMessage.warning(t('wms.transfer.fromLocationRequired'));
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
      ElMessage.success(t('wms.common.savedSuccess'));
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
    t('wms.transfer.outConfirmMsg', { no: target.transferNo }),
    t('wms.transfer.outConfirmTitle'),
    { confirmButtonText: t('wms.transfer.outConfirmBtn'), cancelButtonText: t('common.cancel'), type: "warning" }
  )
    .then(() => outTransfer(target.transferId))
    .then((res) => {
      ElMessage.success(res.msg || t('wms.transfer.outSuccess'));
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
    ElMessage.warning(t('wms.transfer.inputReceivedQty'));
    return;
  }
  for (const i of items) {
    if (!i.toLocationId) {
      ElMessage.warning(t('wms.transfer.selectReceiveLocation', { name: i.materialName }));
      return;
    }
    if (i.quantity > i.transitQty) {
      ElMessage.warning(t('wms.transfer.receivedExceedTransit', { name: i.materialName }));
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
    ElMessage.success(res.msg || t('wms.transfer.receiveSuccess'));
    inOpen.value = false;
    getList();
  });
}

function handleDelete(row) {
  const delIds = row.transferId || ids.value.join(",");
  ElMessageBox.confirm(t('wms.transfer.deleteConfirm', { id: delIds }), t('wms.common.warning'), {
    confirmButtonText: t('common.confirm'),
    cancelButtonText: t('common.cancel'),
    type: "warning",
  })
    .then(() => delTransfer(delIds))
    .then(() => {
      getList();
      ElMessage.success(t('wms.common.deleteSuccess'));
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
