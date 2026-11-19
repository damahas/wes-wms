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
          <el-button type="primary" plain icon="Plus" @click="handleAdd" v-hasPermi="['wms:adjust:add']">{{ t('common.add') }}</el-button>
        </el-col>
        <el-col :span="1.5">
          <el-button type="success" plain icon="Edit" :disabled="single" @click="handleUpdate" v-hasPermi="['wms:adjust:edit']">{{ t('common.edit') }}</el-button>
        </el-col>
        <el-col :span="1.5">
          <el-button type="danger" plain icon="Delete" :disabled="multiple" @click="handleDelete" v-hasPermi="['wms:adjust:remove']">{{ t('common.delete') }}</el-button>
        </el-col>
        <right-toolbar
          v-model:showSearch="showSearch"
          :hint="t('wms.adjust.hint')"
          @queryTable="getList"
        ></right-toolbar>
      </el-row>

      <!-- 列表 -->
      <el-table v-loading="loading" :data="adjustList" @selection-change="handleSelectionChange">
        <el-table-column type="selection" width="55" align="center" :selectable="(row) => row.adjustStatus !== 'completed'" />
        <el-table-column :label="t('wms.adjust.adjustNo')" align="center" prop="adjustNo" width="180" :show-overflow-tooltip="true" />
        <el-table-column :label="t('wms.adjust.adjustType')" align="center" width="110">
          <template #default="scope">
            {{ adjustTypeLabel(scope.row.adjustType) }}
          </template>
        </el-table-column>
        <el-table-column :label="t('wms.common.warehouse')" align="center" :show-overflow-tooltip="true">
          <template #default="scope">{{ scope.row.warehouse?.warehouseName || "-" }}</template>
        </el-table-column>
        <el-table-column :label="t('wms.adjust.totalQuantity')" align="center" prop="totalQuantity" width="110">
          <template #default="scope">{{ scope.row.totalQuantity ?? "-" }}</template>
        </el-table-column>
        <el-table-column :label="t('common.status')" align="center" width="100">
          <template #default="scope">
            <el-tag :type="scope.row.adjustStatus === 'completed' ? 'success' : 'warning'" size="small">
              {{ scope.row.adjustStatus === "completed" ? t('wms.adjust.executed') : t('wms.adjust.pendingExecute') }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column :label="t('wms.adjust.adjustReason')" align="center" prop="adjustReason" :show-overflow-tooltip="true" />
        <el-table-column :label="t('common.createTime')" align="center" prop="createTime" width="170" />
        <el-table-column :label="t('common.actions')" align="center" width="190" class-name="small-padding fixed-width">
          <template #default="scope">
            <el-button
              v-if="scope.row.adjustStatus !== 'completed'"
              link type="primary" icon="Check" @click="handleExecute(scope.row)"
              v-hasPermi="['wms:adjust:edit']"
            >{{ t('wms.move.execute') }}</el-button>
            <el-button
              v-if="scope.row.adjustStatus !== 'completed'"
              link type="primary" icon="Edit" @click="handleUpdate(scope.row)"
              v-hasPermi="['wms:adjust:edit']"
            >{{ t('common.edit') }}</el-button>
            <el-button
              v-if="scope.row.adjustStatus !== 'completed'"
              link type="danger" icon="Delete" @click="handleDelete(scope.row)"
              v-hasPermi="['wms:adjust:remove']"
            >{{ t('common.delete') }}</el-button>
            <span v-if="scope.row.adjustStatus === 'completed'" class="text-muted">{{ t('wms.adjust.executed') }}</span>
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
      <el-dialog :title="title" v-model="open" width="90%" append-to-body>
        <el-form ref="adjustRef" :model="form" :rules="rules" label-width="90px" class="nowrap-labels">
          <el-row :gutter="16">
            <el-col :span="8">
              <el-form-item :label="t('wms.adjust.adjustNo')">
                <el-input v-model="form.adjustNo" :placeholder="t('wms.receipt.receiptNoAuto')" readonly />
              </el-form-item>
            </el-col>
            <el-col :span="8">
              <el-form-item :label="t('wms.adjust.adjustType')" prop="adjustType">
                <el-select v-model="form.adjustType" :placeholder="t('wms.adjust.ph.selectType')" style="width:100%">
                  <el-option :label="t('wms.adjust.typeCheckProfit')" value="check_profit" />
                  <el-option :label="t('wms.adjust.typeCheckLoss')" value="check_loss" />
                  <el-option :label="t('wms.adjust.typeDamage')" value="damage" />
                  <el-option :label="t('wms.adjust.typeOverflow')" value="overflow" />
                  <el-option :label="t('wms.adjust.typeValidity')" value="validity" />
                  <el-option :label="t('wms.issueOrder.typeOther')" value="other" />
                </el-select>
              </el-form-item>
            </el-col>
            <el-col :span="8">
              <el-form-item :label="t('wms.common.warehouse')" prop="warehouseId">
                <WarehouseSelect v-model="form.warehouseId" :placeholder="t('wms.warehouse.ph.selectWarehouse')" @change="onWarehouseChange" />
              </el-form-item>
            </el-col>
            <el-col :span="24">
              <el-form-item :label="t('wms.adjust.adjustReason')" prop="adjustReason">
                <el-input v-model="form.adjustReason" type="textarea" :autosize="{ minRows: 1, maxRows: 3 }"
                  :placeholder="t('wms.adjust.ph.reasonExample')" maxlength="255" />
              </el-form-item>
            </el-col>
          </el-row>

          <!-- 调整明细 -->
          <div class="detail-head">
            <span class="detail-title">{{ t('wms.adjust.detailTitle') }}</span>
            <el-button class="detail-add-btn" type="primary" plain icon="Plus" @click="addDetail">{{ t('wms.receiving.addDetail') }}</el-button>
          </div>

          <el-table :data="details" border size="small" :empty-text="t('wms.adjust.emptyDetail')">
            <el-table-column :label="t('wms.common.location')" width="200">
              <template #default="scope">
                <LocationCascader
                  v-model="scope.row.locCascader"
                  :warehouse-id="form.warehouseId"
                  :disabled="!form.warehouseId"
                  :placeholder="form.warehouseId ? t('wms.move.ph.zoneOrLocation') : t('wms.move.ph.selectWarehouseFirst')"
                  @update:model-value="(val) => onDetailLocation(val, scope.$index)"
                />
              </template>
            </el-table-column>
            <el-table-column :label="t('wms.adjust.stockRow')" min-width="230">
              <template #default="scope">
                <el-select
                  v-model="scope.row.stockPick"
                  :loading="scope.row._loadingStock"
                  :placeholder="scope.row.locationId ? t('wms.adjust.ph.pickStockRow') : t('wms.adjust.ph.selectLocationFirst')"
                  :disabled="!scope.row.locationId"
                  style="width:100%"
                  @change="(v) => onStockPick(v, scope.$index)"
                >
                  <el-option :label="t('wms.adjust.newStockOption')" value="new" />
                  <el-option
                    v-for="opt in scope.row._stockOptions || []"
                    :key="opt.value"
                    :label="opt.label"
                    :value="opt.value"
                  />
                </el-select>
              </template>
            </el-table-column>
            <el-table-column :label="t('wms.material.title')" min-width="180">
              <template #default="scope">
                <MaterialSelect
                  v-if="scope.row._isNew"
                  v-model="scope.row.materialId"
                  :label="scope.row.materialCode && scope.row.materialName ? scope.row.materialCode + ' - ' + scope.row.materialName : ''"
                  :placeholder="t('wms.material.ph.selectMaterial')"
                  @change="onMaterialChange($event, scope.$index)"
                />
                <span v-else class="text-muted">{{ scope.row.materialCode }} - {{ scope.row.materialName }}</span>
              </template>
            </el-table-column>
            <el-table-column :label="t('wms.receiving.batchNo')" width="120">
              <template #default="scope">
                <el-input v-if="scope.row._isNew" v-model="scope.row.batchNo" :placeholder="t('wms.receiving.batchNo')" maxlength="50" />
                <span v-else class="text-muted">{{ scope.row.batchNo || t('wms.move.noBatch') }}</span>
              </template>
            </el-table-column>
            <el-table-column :label="t('wms.adjust.before')" width="80" align="center">
              <template #default="scope">
                <span class="text-muted">{{ scope.row.stockId || scope.row.detailId ? (scope.row.beforeQuantity ?? 0) : "—" }}</span>
              </template>
            </el-table-column>
            <el-table-column :label="t('wms.adjust.adjustQuantity')" width="120">
              <template #default="scope">
                <el-input-number v-model="scope.row.adjustQuantity" :controls="false"
                  :placeholder="t('wms.adjust.ph.positiveNeg')" style="width:100%" />
              </template>
            </el-table-column>
            <el-table-column :label="t('wms.adjust.after')" width="80" align="center">
              <template #default="scope">
                <span :class="previewClass(scope.row)">{{ previewAfter(scope.row) }}</span>
              </template>
            </el-table-column>
            <el-table-column :label="t('common.actions')" width="70" align="center">
              <template #default="scope">
                <el-button link type="danger" icon="Delete" @click="removeDetail(scope.$index)" />
              </template>
            </el-table-column>
          </el-table>

          <div class="detail-tip text-muted">
            {{ t('wms.adjust.detailTip') }}
          </div>
        </el-form>
        <template #footer>
          <div class="dialog-footer">
            <el-button type="primary" @click="submitForm">{{ t('common.submit') }}</el-button>
            <el-button @click="cancel">{{ t('common.cancel') }}</el-button>
          </div>
        </template>
      </el-dialog>
    </div>
  </div>
</template>

<script setup name="WmsAdjust">
import { ref, reactive, toRefs, computed } from "vue";
import { ElMessage, ElMessageBox } from "element-plus";
import { useI18n } from "vue-i18n";
import {
  listAdjust,
  getAdjust,
  addAdjust,
  updateAdjust,
  delAdjust,
  executeAdjust,
} from "@/api/wms/adjust";
import { listStock } from "@/api/wms/stock";
import QueryForm from "@/components/QueryForm/index.vue";
import RightToolbar from "@/components/RightToolbar/index.vue";
import WarehouseSelect from "@/views/wms/warehouse/input.vue";
import MaterialSelect from "@/views/wms/material/input.vue";
import LocationCascader from "@/views/wms/location/cascader.vue";

const { t } = useI18n();
const adjustRef = ref(null);
const adjustList = ref([]);
const open = ref(false);
const loading = ref(true);
const ids = ref([]);
const single = ref(true);
const multiple = ref(true);
const total = ref(0);
const title = ref("");

const showSearch = ref(true);

/** 调整类型字典 */
function adjustTypeLabel(v) {
  const map = {
    check_profit: t('wms.adjust.typeCheckProfit'),
    check_loss: t('wms.adjust.typeCheckLoss'),
    damage: t('wms.adjust.typeDamage'),
    overflow: t('wms.adjust.typeOverflow'),
    validity: t('wms.adjust.typeValidity'),
    other: t('wms.issueOrder.typeOther'),
  };
  return map[v] || v || "-";
}

const data = reactive({
  form: {},
  details: [],
  queryParams: {
    pageNum: 1,
    pageSize: 10,
    keywords: undefined,
  },
  rules: {
    adjustType: [{ required: true, message: () => t('wms.adjust.ph.selectType'), trigger: "change" }],
    warehouseId: [{ required: true, message: () => t('wms.receipt.rules.warehouseRequired'), trigger: "change" }],
  },
});

const { form, details, queryParams, rules } = toRefs(data);

const queryConfig = computed(() => [
  {
    label: t('wms.common.keywords'),
    prop: "keywords",
    type: "input",
    placeholder: t('wms.adjust.ph.inputAdjustNo'),
  },
]);

/** 查询列表 */
function getList() {
  loading.value = true;
  listAdjust(queryParams.value).then((res) => {
    adjustList.value = res.rows || [];
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

function handleSelectionChange(selection) {
  ids.value = selection.map((item) => item.adjustId);
  single.value = selection.length !== 1;
  multiple.value = !selection.length;
}

/** 表单重置 */
function reset() {
  form.value = {
    adjustId: undefined,
    adjustNo: undefined,
    adjustType: undefined,
    warehouseId: undefined,
    adjustReason: undefined,
    adjustStatus: undefined,
    remark: undefined,
  };
  details.value = [];
  if (adjustRef.value) adjustRef.value.resetFields();
}

/** 新增 */
function handleAdd() {
  reset();
  open.value = true;
  title.value = t('wms.adjust.addTitle');
}

/** 修改 */
function handleUpdate(row) {
  reset();
  const id = row.adjustId || ids.value[0];
  getAdjust(id).then((res) => {
    const data = res.data || {};
    form.value = data;
    details.value = (data.details || []).map((d) => ({
      ...d,
      // 库位级联回显：[库区, 库位]
      locCascader: d.locationId ? [d.zoneId || d.location?.zoneId, d.locationId].filter(Boolean) : [],
      stockPick: undefined,
      _stockOptions: [],
      _isNew: false,
    }));
    // 每行拉库存行选项并回填：有 stockId 直接选中；老明细按物料+批次匹配；匹配不到视为盘盈新增
    details.value.forEach((d, i) => {
      if (!d.locationId) return;
      d._loadingStock = true;
      listStock({ pageNum: 1, pageSize: 9999, params: { locationId: d.locationId } })
        .then((res2) => {
          d._stockOptions = (res2.rows || []).map((s) => ({
            value: String(s.stockId),
            label: `${s.materialCode || s.material?.materialCode} / ${t('wms.receiving.batchNo')}${s.batchNo || t('wms.move.noBatch')}${s.barcode ? " / " + s.barcode : ""} / ${t('wms.stock.stock')} ${s.stockQuantity}`,
            stockId: s.stockId,
            materialId: s.materialId,
            materialCode: s.materialCode || s.material?.materialCode,
            materialName: s.materialName || s.material?.materialName,
            batchNo: s.batchNo || "",
            quantity: Number(s.stockQuantity) || 0,
          }));
          const matched = d.stockId
            ? d._stockOptions.find((o) => o.stockId === d.stockId)
            : d._stockOptions.find(
                (o) => o.materialId === d.materialId && (o.batchNo || "") === (d.batchNo || "")
              );
          if (matched) {
            d.stockPick = matched.value;
            d.stockId = matched.stockId;
            d._isNew = false;
            d.beforeQuantity = matched.quantity;
          } else {
            d.stockPick = "new";
            d._isNew = true;
          }
        })
        .finally(() => {
          d._loadingStock = false;
        });
    });
    open.value = true;
    title.value = t('wms.adjust.editTitle');
  });
}

/** 添加明细行 */
function addDetail() {
  details.value.push({
    detailId: 0,
    stockId: undefined,
    stockPick: undefined,
    _stockOptions: [],
    _isNew: false,
    materialId: undefined,
    materialCode: "",
    materialName: "",
    batchNo: "",
    zoneId: undefined,
    locationId: undefined,
    locCascader: [],
    beforeQuantity: 0,
    adjustQuantity: undefined,
    afterQuantity: 0,
  });
}

function removeDetail(index) {
  details.value.splice(index, 1);
}

/** 物料选择：带出编码名称供展示 */
function onMaterialChange(data, index) {
  if (!data) return;
  const row = details.value[index];
  row.materialCode = data.materialCode || "";
  row.materialName = data.materialName || "";
}

/** 库位选择：切换库位后拉取该库位的库存行，供精确调整 */
function onDetailLocation(val, index) {
  const row = details.value[index];
  row.zoneId = val?.[0];
  row.locationId = val?.[1];
  // 库位变了，之前的库存行选择全部作废
  row.stockId = undefined;
  row.stockPick = undefined;
  row._stockOptions = [];
  row._isNew = false;
  row.materialId = undefined;
  row.materialCode = undefined;
  row.materialName = undefined;
  row.batchNo = undefined;
  row.beforeQuantity = 0;
  row.adjustQuantity = undefined;
  if (row.locationId) loadStockOptions(row);
}

/** 拉取某行库位下的全部库存行（同物料同批次可能多条码行，必须让用户挑） */
function loadStockOptions(row) {
  row._loadingStock = true;
  listStock({ pageNum: 1, pageSize: 9999, params: { locationId: row.locationId } })
    .then((res) => {
      row._stockOptions = (res.rows || []).map((s) => ({
        value: String(s.stockId),
        label: `${s.materialCode || s.material?.materialCode} / ${t('wms.receiving.batchNo')}${s.batchNo || t('wms.move.noBatch')}${s.barcode ? " / " + s.barcode : ""} / ${t('wms.stock.stock')} ${s.stockQuantity}`,
        stockId: s.stockId,
        materialId: s.materialId,
        materialCode: s.materialCode || s.material?.materialCode,
        materialName: s.materialName || s.material?.materialName,
        batchNo: s.batchNo || "",
        quantity: Number(s.stockQuantity) || 0,
      }));
    })
    .finally(() => {
      row._loadingStock = false;
    });
}

/** 库存行选择：选已有行 → 锁定物料/批次并带出当前量；选 new → 切盘盈新增模式 */
function onStockPick(val, index) {
  const row = details.value[index];
  row.adjustQuantity = undefined;
  if (val === "new") {
    row.stockId = undefined;
    row._isNew = true;
    row.materialId = undefined;
    row.materialCode = undefined;
    row.materialName = undefined;
    row.batchNo = undefined;
    row.beforeQuantity = 0;
    return;
  }
  const opt = (row._stockOptions || []).find((o) => o.value === val);
  if (!opt) return;
  row.stockId = opt.stockId;
  row._isNew = false;
  row.materialId = opt.materialId;
  row.materialCode = opt.materialCode;
  row.materialName = opt.materialName;
  row.batchNo = opt.batchNo;
  row.beforeQuantity = opt.quantity;
}

/** 调整后预览 */
function previewAfter(row) {
  if (row.stockId && row.beforeQuantity != null) {
    const after = (row.beforeQuantity || 0) + (Number(row.adjustQuantity) || 0);
    return after;
  }
  if (row._isNew && Number(row.adjustQuantity) > 0) return row.adjustQuantity;
  return "—";
}

function previewClass(row) {
  const after = previewAfter(row);
  if (typeof after !== "number") return "text-muted";
  return after === 0 ? "dir-out" : "text-muted";
}

/** 仓库变更：清空明细库位（避免跨仓库） */
function onWarehouseChange() {
  details.value.forEach((d) => {
    d.locCascader = [];
    d.zoneId = undefined;
    d.locationId = undefined;
    d.stockId = undefined;
    d.stockPick = undefined;
    d._stockOptions = [];
    d._isNew = false;
    d.materialId = undefined;
    d.materialCode = undefined;
    d.materialName = undefined;
    d.batchNo = undefined;
    d.beforeQuantity = 0;
    d.adjustQuantity = undefined;
  });
}

/** 提交 */
function submitForm() {
  adjustRef.value.validate((valid) => {
    if (!valid) return;
    if (!details.value.length) {
      ElMessage.warning(t('wms.adjust.selectAtLeastOneDetail'));
      return;
    }
    for (let i = 0; i < details.value.length; i++) {
      const d = details.value[i];
      if (!d.locationId) {
        ElMessage.warning(t('wms.adjust.rowLocationRequired', { no: i + 1 }));
        return;
      }
      if (!d.stockId && !d._isNew) {
        ElMessage.warning(t('wms.adjust.rowStockRequired', { no: i + 1 }));
        return;
      }
      if (d._isNew && !d.materialId) {
        ElMessage.warning(t('wms.adjust.rowNewMaterialRequired', { no: i + 1 }));
        return;
      }
      if (!d.adjustQuantity) {
        ElMessage.warning(t('wms.adjust.rowQtyZero', { no: i + 1 }));
        return;
      }
      if (d._isNew && Number(d.adjustQuantity) <= 0) {
        ElMessage.warning(t('wms.adjust.rowNewQtyRequired', { no: i + 1 }));
        return;
      }
      if (d.stockId && (d.beforeQuantity || 0) + Number(d.adjustQuantity) < 0) {
        ElMessage.warning(t('wms.adjust.rowNegativeWarn', { no: i + 1, qty: d.beforeQuantity }));
        return;
      }
    }

    const payload = {
      ...form.value,
      totalQuantity: details.value.reduce((s, d) => s + (Number(d.adjustQuantity) || 0), 0),
      details: details.value.map((d) => ({
        detailId: d.detailId || 0,
        stockId: d.stockId || undefined,
        materialId: d.materialId,
        batchNo: d.batchNo,
        zoneId: d.zoneId,
        locationId: d.locationId,
        adjustQuantity: d.adjustQuantity,
        beforeQuantity: d.beforeQuantity || 0,
        afterQuantity: d.afterQuantity || 0,
      })),
    };

    const api = form.value.adjustId ? updateAdjust : addAdjust;
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

/** 执行调整单 */
function handleExecute(row) {
  ElMessageBox.confirm(
    t('wms.move.executeConfirmMsg', { no: row.adjustNo }),
    t('wms.move.executeConfirmTitle'),
    { confirmButtonText: t('wms.move.executeConfirmBtn'), cancelButtonText: t('common.cancel'), type: "warning" }
  )
    .then(() => executeAdjust(row.adjustId))
    .then(() => {
      ElMessage.success(t('wms.move.executeSuccess'));
      getList();
    });
}

/** 删除 */
function handleDelete(row) {
  const delIds = row.adjustId || ids.value.join(",");
  ElMessageBox.confirm(t('wms.common.deleteConfirm', { id: delIds }), t('wms.common.warning'), {
    confirmButtonText: t('common.confirm'),
    cancelButtonText: t('common.cancel'),
    type: "warning",
  })
    .then(() => delAdjust(delIds))
    .then(() => {
      getList();
      ElMessage.success(t('wms.common.deleteSuccess'));
    });
}

getList();
</script>

<style scoped>
.nowrap-labels :deep(.el-form-item__label) {
  white-space: nowrap;
}
.detail-head {
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: 8px 0;
  margin-top: 4px;
  border-top: 1px solid #ebeef5;
}
.detail-title {
  font-size: 14px;
  font-weight: 600;
  color: #303133;
}
.detail-tip {
  margin-top: 8px;
  font-size: 12px;
}
</style>
