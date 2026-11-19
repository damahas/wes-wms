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
          <el-button type="primary" plain icon="Plus" @click="handleAdd" v-hasPermi="['wms:move:add']">{{ t('common.add') }}</el-button>
        </el-col>
        <el-col :span="1.5">
          <el-button v-if="canEdit(selectedRows[0] || {})" type="success" plain icon="Edit" :disabled="single" @click="handleUpdate" v-hasPermi="['wms:move:edit']">{{ t('common.edit') }}</el-button>
          <el-button v-else type="info" plain icon="View" :disabled="single" @click="handleView(selectedRows[0])">{{ t('wms.receipt.details') }}</el-button>
        </el-col>
        <el-col :span="1.5">
          <el-button type="danger" plain icon="Delete" :disabled="multiple || selectedRows.some((r) => !canEdit(r))" @click="handleDelete" v-hasPermi="['wms:move:remove']">{{ t('common.delete') }}</el-button>
        </el-col>
        <right-toolbar
          v-model:showSearch="showSearch"
          @queryTable="getList"
        ></right-toolbar>
      </el-row>

      <!-- 列表 -->
      <el-table v-loading="loading" :data="moveList" @selection-change="handleSelectionChange">
        <el-table-column type="selection" width="55" align="center" />
        <el-table-column :label="t('wms.move.moveNo')" align="center" prop="moveNo" :show-overflow-tooltip="true" />
        <el-table-column :label="t('wms.move.moveType')" align="center" width="110" :show-overflow-tooltip="true">
          <template #default="scope">
            {{ moveTypeLabel(scope.row.moveType) }}
          </template>
        </el-table-column>
        <el-table-column :label="t('wms.common.warehouse')" align="center" :show-overflow-tooltip="true">
          <template #default="scope">
            {{ scope.row.warehouse?.warehouseName || "-" }}
          </template>
        </el-table-column>
        <el-table-column :label="t('wms.receipt.totalQuantity')" align="center" prop="totalQuantity" width="90" />
        <el-table-column :label="t('common.status')" align="center" width="100">
          <template #default="scope">
            <el-tag :type="moveStatusTag(scope.row.moveStatus)">
              {{ moveStatusLabel(scope.row.moveStatus) }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column :label="t('common.createTime')" align="center" prop="createTime" min-width="170" />
        <el-table-column :label="t('common.actions')" align="center" width="220" class-name="small-padding fixed-width">
          <template #default="scope">
            <el-button v-if="canEdit(scope.row)" link type="success" icon="Promotion" @click="handleExecute(scope.row)" v-hasPermi="['wms:move:edit']">{{ t('wms.move.execute') }}</el-button>
            <el-button v-if="canEdit(scope.row)" link type="primary" icon="Edit" @click="handleUpdate(scope.row)" v-hasPermi="['wms:move:edit']">{{ t('common.edit') }}</el-button>
            <el-button v-else link type="info" icon="View" @click="handleView(scope.row)">{{ t('wms.receipt.details') }}</el-button>
            <el-button v-if="canEdit(scope.row)" link type="danger" icon="Delete" @click="handleDelete(scope.row)" v-hasPermi="['wms:move:remove']">{{ t('common.delete') }}</el-button>
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
      <el-dialog :title="title" v-model="open" width="1100px" top="4vh" append-to-body>
        <el-form ref="moveRef" :model="form" :rules="rules" label-width="88px" class="nowrap-labels" :disabled="viewMode">
          <el-row :gutter="16">
            <el-col :span="8">
              <el-form-item :label="t('wms.move.moveNo')">
                <el-input v-model="form.moveNo" :placeholder="t('wms.receipt.receiptNoAuto')" readonly />
              </el-form-item>
            </el-col>
            <el-col :span="8">
              <el-form-item :label="t('wms.move.moveType')" prop="moveType">
                <el-select v-model="form.moveType" :placeholder="t('wms.move.ph.selectType')" style="width:100%">
                  <el-option :label="t('wms.move.typeLocationAdjust')" value="location_adjust" />
                  <el-option :label="t('wms.move.typeZoneAdjust')" value="zone_adjust" />
                  <el-option :label="t('wms.move.typeReplenish')" value="replenish" />
                  <el-option :label="t('wms.move.typeCheckAdjust')" value="check_adjust" />
                  <el-option :label="t('wms.issueOrder.typeOther')" value="other" />
                </el-select>
              </el-form-item>
            </el-col>
            <el-col :span="8">
              <el-form-item :label="t('wms.common.warehouse')" prop="warehouseId">
                <WarehouseSelect v-model="form.warehouseId" :placeholder="t('wms.warehouse.ph.selectWarehouse')" :disabled="viewMode" @change="onWarehouseChange" />
              </el-form-item>
            </el-col>
            <el-col :span="24">
              <el-form-item :label="t('wms.move.moveReason')" prop="moveReason">
                <el-input v-model="form.moveReason" type="textarea" :autosize="{ minRows: 1, maxRows: 3 }"
                  :placeholder="t('wms.move.ph.inputReason')" maxlength="255" />
              </el-form-item>
            </el-col>
          </el-row>

          <!-- 移库明细：库存行一律从弹窗多选进来，行内不可再换，要改就删行重挑 -->
          <div class="detail-head">
            <span class="detail-title">{{ t('wms.move.detailTitle') }}</span>
            <el-button v-if="!viewMode" class="detail-add-btn" type="primary" plain icon="Plus" @click="openStockDialog">{{ t('wms.receiving.addDetail') }}</el-button>
            <SelectStock
              ref="stockSelectRef"
              :is-single="false"
              :warehouse-id="form.warehouseId"
              @handle-data="onStockSelected"
              v-show="false"
            />
          </div>

          <el-table :data="details" border size="small" :empty-text="t('wms.move.emptyDetail')">
            <el-table-column :label="t('wms.move.stockCol')" min-width="300">
              <template #default="scope">
                <span>{{ scope.row.stockLabel }}</span>
              </template>
            </el-table-column>
            <el-table-column :label="t('wms.move.toLocation')" width="200">
              <template #default="scope">
                <LocationCascader
                  v-model="scope.row.toLocCascader"
                  :warehouse-id="form.warehouseId"
                  :disabled="!form.warehouseId || viewMode"
                  :placeholder="form.warehouseId ? t('wms.move.ph.zoneOrLocation') : t('wms.move.ph.selectWarehouseFirst')"
                  @update:model-value="(val) => onToLocation(val, scope.$index)"
                />
              </template>
            </el-table-column>
            <el-table-column :label="t('wms.receipt.quantity')" width="110">
              <template #default="scope">
                <span :title="t('wms.move.quantityTip')">
                  {{ scope.row.moveQuantity }}
                </span>
              </template>
            </el-table-column>
            <el-table-column :label="t('common.actions')" width="70" align="center">
              <template #default="scope">
                <el-button v-if="!viewMode" link type="danger" icon="Delete" @click="removeDetail(scope.$index)" />
              </template>
            </el-table-column>
          </el-table>

          <div class="detail-tip text-muted">
            {{ t('wms.move.detailTip') }}
          </div>
        </el-form>
        <template #footer>
          <div class="dialog-footer">
            <el-button v-if="!viewMode" type="primary" @click="submitForm">{{ t('common.submit') }}</el-button>
            <el-button @click="cancel">{{ viewMode ? t('common.close') : t('common.cancel') }}</el-button>
          </div>
        </template>
      </el-dialog>
    </div>
  </div>
</template>

<script setup name="WmsMove">
import { ref, reactive, toRefs, computed } from "vue";
import { ElMessage, ElMessageBox } from "element-plus";
import { useI18n } from "vue-i18n";
import {
  listMove,
  getMove,
  addMove,
  updateMove,
  delMove,
  executeMove,
} from "@/api/wms/move";
import QueryForm from "@/components/QueryForm/index.vue";
import RightToolbar from "@/components/RightToolbar/index.vue";
import WarehouseSelect from "@/views/wms/warehouse/input.vue";
import SelectStock from "@/views/wms/stock/select.vue";
import LocationCascader from "@/views/wms/location/cascader.vue";

const { t } = useI18n();
const moveRef = ref(null);
const moveList = ref([]);
const open = ref(false);
const loading = ref(true);
const ids = ref([]);
const single = ref(true);
const multiple = ref(true);
const total = ref(0);
const title = ref("");
/** 只读查看模式：单据已执行后只用于查看明细 */
const viewMode = ref(false);
/** 列表当前勾选的行 */
const selectedRows = ref([]);
/** 库存多选弹窗（明细行一律从这里进来） */
const stockSelectRef = ref(null);

const showSearch = ref(true);

// 移库类型：库位调整 / 库区调整 / 补货移库 / 盘点调整 / 其他
function moveTypeLabel(v) {
  const map = {
    location_adjust: t('wms.move.typeLocationAdjust'),
    zone_adjust: t('wms.move.typeZoneAdjust'),
    replenish: t('wms.move.typeReplenish'),
    check_adjust: t('wms.move.typeCheckAdjust'),
    other: t('wms.issueOrder.typeOther'),
  };
  return map[v] || "-";
}

// 移库状态：待移库 → 已完成
function moveStatusLabel(s) {
  const map = { pending: t('wms.move.statusPending'), completed: t('wms.issueOrder.statusIssued') };
  return map[s] || "-";
}
const moveStatusTag = (s) => (s === "completed" ? "success" : "info");

/** 已执行（已完成）的移库单不能再修改或删除 */
function canEdit(row) {
  return row.moveStatus !== "completed";
}

const details = ref([]);

const data = reactive({
  form: {},
  queryParams: {
    pageNum: 1,
    pageSize: 10,
    keywords: undefined,
    moveStatus: undefined,
  },
  rules: {
    moveType: [{ required: true, message: () => t('wms.move.rules.typeRequired'), trigger: "change" }],
    warehouseId: [{ required: true, message: () => t('wms.receipt.rules.warehouseRequired'), trigger: "change" }],
  },
});

const { form, queryParams, rules } = toRefs(data);

const queryConfig = computed(() => [
  {
    label: t('wms.common.keywords'),
    prop: "keywords",
    type: "input",
    placeholder: t('wms.move.ph.inputMoveNo'),
  },
  {
    label: t('common.status'),
    prop: "moveStatus",
    type: "select",
    placeholder: t('wms.warehouse.ph.selectStatus'),
    options: [
      { label: t('wms.move.statusPending'), value: "pending" },
      { label: t('wms.issueOrder.statusIssued'), value: "completed" },
    ],
  },
]);

/** 查询列表 */
function getList() {
  loading.value = true;
  listMove(queryParams.value).then((res) => {
    moveList.value = res.rows || [];
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
    moveStatus: undefined,
    pageNum: 1,
    pageSize: 10,
  };
  handleQuery();
}

/** 多选框 */
function handleSelectionChange(selection) {
  selectedRows.value = selection;
  ids.value = selection.map((item) => item.moveId);
  single.value = selection.length !== 1;
  multiple.value = !selection.length;
}

/** 表单重置 */
function reset() {
  form.value = {
    moveId: undefined,
    moveNo: undefined,
    moveType: undefined,
    moveReason: undefined,
    warehouseId: undefined,
    totalQuantity: undefined,
    moveStatus: undefined,
    remark: undefined,
  };
  details.value = [];
  viewMode.value = false;
  if (moveRef.value) moveRef.value.resetFields();
}

/** 新增 */
function handleAdd() {
  reset();
  open.value = true;
  title.value = t('wms.move.addTitle');
}

/** 修改 */
function handleUpdate(row) {
  reset();
  const id = row?.moveId || ids.value[0];
  getMove(id).then((res) => fillForm(res.data || {}));
}

/** 查看明细：已执行后只读打开 */
function handleView(row) {
  if (!row?.moveId) return;
  reset();
  viewMode.value = true;
  getMove(row.moveId).then((res) => fillForm(res.data || {}));
}

/** 填充表单 */
function fillForm(data) {
  form.value = {
    moveId: data.moveId,
    moveNo: data.moveNo,
    moveType: data.moveType,
    moveReason: data.moveReason,
    warehouseId: data.warehouseId,
    totalQuantity: data.totalQuantity,
    moveStatus: data.moveStatus,
    remark: data.remark,
  };
  details.value = (data.details || []).map((d) => ({
    detailId: d.detailId || 0,
    // 明细没存库存行 id，编辑时用文把当前这行显示出来（真值仍在下面几个字段里）
    stockId: undefined,
    stockLabel: [d.materialCode, d.materialName, d.batchNo ? `${t('wms.receiving.batchNo')}${d.batchNo}` : t('wms.move.noBatch'), d.fromLocationCode]
      .filter(Boolean).join(" / "),
    materialId: d.materialId,
    materialCode: d.materialCode || d.material?.materialCode || "",
    materialName: d.materialName || d.material?.materialName || "",
    batchNo: d.batchNo || "",
    fromZoneId: d.fromZoneId,
    fromLocationId: d.fromLocationId,
    toZoneId: d.toZoneId,
    toLocationId: d.toLocationId,
    toLocCascader: d.toLocationId ? [d.toZoneId, d.toLocationId].filter(Boolean) : [],
    moveQuantity: d.moveQuantity,
  }));
  open.value = true;
  title.value = viewMode.value ? t('wms.move.viewTitle') : t('wms.move.editTitle');
}

/**
 * 打开库存多选弹窗。
 * 原来点「添加明细」只是插一行空的，空行里仓库还没选、库存也点不动，等于先把人卡住；
 * 改成弹窗里一次挑多行，选完直接成型。
 */
function openStockDialog() {
  if (!form.value.warehouseId) {
    ElMessage.warning(t('wms.move.selectWarehouseFirst'));
    return;
  }
  stockSelectRef.value?.open();
}

/** 多选库存 → 批量生成明细行；同一行库存已在明细里则跳过，避免搬两次 */
function onStockSelected(items) {
  if (!items || items.length === 0) return;
  let skipped = 0;
  for (const stock of items) {
    if (details.value.some((d) => d.stockId === stock.stockId)) {
      skipped++;
      continue;
    }
    const available = (stock.stockQuantity ?? 0) - (stock.freezeQuantity ?? 0) - (stock.lockQuantity ?? 0);
    details.value.push({
      detailId: 0,
      stockId: stock.stockId,
      stockLabel: `${stock.materialCode || ""} ${stock.materialName || ""} / ${stock.batchNo || t('wms.move.noBatch')} / ${stock.locationCode || ""}`,
      materialId: stock.materialId,
      materialCode: stock.materialCode || "",
      materialName: stock.materialName || "",
      batchNo: stock.batchNo || "",
      fromZoneId: stock.zoneId,
      fromLocationId: stock.locationId,
      toZoneId: undefined,
      toLocationId: undefined,
      toLocCascader: [],
      barcode: stock.barcode || "",
      moveQuantity: Math.floor(available) > 0 ? Math.floor(available) : undefined,
    });
  }
  if (skipped > 0) ElMessage.info(t('wms.move.skippedRows', { count: skipped }));
}

function removeDetail(index) {
  details.value.splice(index, 1);
}



/** 移入库位（两级：库区 → 库位） */
function onToLocation(val, index) {
  const row = details.value[index];
  row.toZoneId = val?.[0];
  row.toLocationId = val?.[1];
}

/**
 * 仓库变更：换了仓，已挑的库存行必然不属于新仓，而且库存列行内不可改，
 * 留着只会让人以为改改就行——直接整表作废，让人重新挑。
 */
function onWarehouseChange() {
  if (details.value.length === 0) return;
  details.value = [];
  ElMessage.warning(t('wms.move.warehouseChanged'));
}

/** 提交 */
function submitForm() {
  moveRef.value.validate((valid) => {
    if (!valid) return;
    if (!details.value.length) {
      ElMessage.warning(t('wms.move.selectAtLeastOneDetail'));
      return;
    }
    for (let i = 0; i < details.value.length; i++) {
      const d = details.value[i];
      if (!d.materialId) {
        ElMessage.warning(t('wms.receipt.rowMaterialRequired', { no: i + 1 }));
        return;
      }
      if (!d.fromLocationId) {
        ElMessage.warning(t('wms.move.rowFromLocationRequired', { no: i + 1 }));
        return;
      }
      if (!d.toLocationId) {
        ElMessage.warning(t('wms.move.rowToLocationRequired', { no: i + 1 }));
        return;
      }
      if (d.fromLocationId === d.toLocationId) {
        ElMessage.warning(t('wms.move.rowSameLocation', { no: i + 1 }));
        return;
      }
      if (!(d.moveQuantity > 0)) {
        ElMessage.warning(t('wms.move.rowQtyRequired', { no: i + 1 }));
        return;
      }
    }

    const payload = {
      ...form.value,
      details: details.value.map((d) => ({
        detailId: d.detailId || 0,
        materialId: d.materialId,
        materialCode: d.materialCode,
        materialName: d.materialName,
        batchNo: d.batchNo,
        fromZoneId: d.fromZoneId,
        fromLocationId: d.fromLocationId,
        toZoneId: d.toZoneId,
        toLocationId: d.toLocationId,
        moveQuantity: d.moveQuantity,
      })),
    };

    const api = form.value.moveId ? updateMove : addMove;
    api(payload).then(() => {
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

/** 执行移库 */
function handleExecute(row) {
  ElMessageBox.confirm(
    t('wms.move.executeConfirmMsg', { no: row.moveNo }),
    t('wms.move.executeConfirmTitle'),
    { confirmButtonText: t('wms.move.executeConfirmBtn'), cancelButtonText: t('common.cancel'), type: "warning" }
  )
    .then(() => executeMove(row.moveId))
    .then(() => {
      ElMessage.success(t('wms.move.executeSuccess'));
      getList();
    });
}

/** 删除 */
function handleDelete(row) {
  const delIds = row.moveId || ids.value.join(",");
  ElMessageBox.confirm(t('wms.move.deleteConfirm', { id: delIds }), t('wms.common.warning'), {
    confirmButtonText: t('common.confirm'),
    cancelButtonText: t('common.cancel'),
    type: "warning",
  })
    .then(() => delMove(delIds))
    .then(() => {
      getList();
      ElMessage.success(t('wms.common.deleteSuccess'));
    });
}

getList();
</script>

<style scoped>
.detail-head {
  display: flex;
  align-items: center;
  justify-content: space-between;
  margin: 6px 0 10px;
}
.detail-title {
  position: relative;
  padding-left: 10px;
  font-size: 14px;
  font-weight: 600;
  color: #303133;
}
.detail-title::before {
  content: "";
  position: absolute;
  left: 0;
  top: 2px;
  bottom: 2px;
  width: 3px;
  border-radius: 2px;
  background: var(--el-color-primary);
}
.detail-tip {
  margin-top: 10px;
  font-size: 12px;
}
.text-muted {
  color: #909399;
}
</style>
