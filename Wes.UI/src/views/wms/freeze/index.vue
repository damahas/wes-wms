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
          <el-button type="primary" plain icon="Plus" @click="handleAdd" v-hasPermi="['wms:freeze:add']">{{ t('common.add') }}</el-button>
        </el-col>
        <el-col :span="1.5">
          <el-button v-if="canEdit(selectedRows[0] || {})" type="success" plain icon="Edit" :disabled="single" @click="handleUpdate" v-hasPermi="['wms:freeze:edit']">{{ t('common.edit') }}</el-button>
          <el-button v-else type="info" plain icon="View" :disabled="single" @click="handleView(selectedRows[0])">{{ t('wms.receipt.details') }}</el-button>
        </el-col>
        <el-col :span="1.5">
          <el-button type="danger" plain icon="Delete" :disabled="multiple || selectedRows.some((r) => !canEdit(r))" @click="handleDelete" v-hasPermi="['wms:freeze:remove']">{{ t('common.delete') }}</el-button>
        </el-col>
        <right-toolbar
          v-model:showSearch="showSearch"
          @queryTable="getList"
        ></right-toolbar>
      </el-row>

      <!-- 列表 -->
      <el-table v-loading="loading" :data="freezeList" @selection-change="handleSelectionChange">
        <el-table-column type="selection" width="55" align="center" />
        <el-table-column :label="t('wms.freeze.freezeNo')" align="center" prop="freezeNo" :show-overflow-tooltip="true" />
        <el-table-column :label="t('wms.freeze.freezeType')" align="center" width="100">
          <template #default="scope">
            {{ freezeTypeLabel(scope.row.freezeType) }}
          </template>
        </el-table-column>
        <el-table-column :label="t('wms.common.warehouse')" align="center" :show-overflow-tooltip="true">
          <template #default="scope">
            {{ scope.row.warehouse?.warehouseName || scope.row.warehouseName || "-" }}
          </template>
        </el-table-column>
        <el-table-column :label="t('wms.common.zone')" align="center" :show-overflow-tooltip="true">
          <template #default="scope">
            {{ scope.row.zone?.zoneName || scope.row.zoneCode || "-" }}
          </template>
        </el-table-column>
        <el-table-column :label="t('wms.common.location')" align="center" width="120" :show-overflow-tooltip="true">
          <template #default="scope">
            {{ scope.row.location?.locationCode || scope.row.locationCode || "-" }}
          </template>
        </el-table-column>
        <el-table-column :label="t('wms.material.title')" align="center" min-width="170" :show-overflow-tooltip="true">
          <template #default="scope">
            {{ materialLabel(scope.row) }}
          </template>
        </el-table-column>
        <el-table-column :label="t('wms.receiving.batchNo')" align="center" prop="batchNo" width="110" :show-overflow-tooltip="true">
          <template #default="scope">
            {{ scope.row.batchNo || "-" }}
          </template>
        </el-table-column>
        <el-table-column :label="t('wms.freeze.freezeQuantity')" align="center" prop="freezeQuantity" width="90" />
        <el-table-column :label="t('common.status')" align="center" width="100">
          <template #default="scope">
            <el-tag :type="freezeStatusTag(scope.row.freezeStatus)">
              {{ freezeStatusLabel(scope.row.freezeStatus) }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column :label="t('common.createTime')" align="center" prop="createTime" min-width="170" />
        <el-table-column :label="t('common.actions')" align="center" width="220" class-name="small-padding fixed-width">
          <template #default="scope">
            <el-button v-if="canEdit(scope.row)" link type="success" icon="Promotion" @click="handleExecute(scope.row)" v-hasPermi="['wms:freeze:edit']">{{ t('wms.freeze.freeze') }}</el-button>
            <el-button v-if="scope.row.freezeStatus === 'frozen'" link type="warning" icon="Sunny" @click="handleThaw(scope.row)" v-hasPermi="['wms:freeze:edit']">{{ t('wms.freeze.thaw') }}</el-button>
            <el-button v-if="canEdit(scope.row)" link type="primary" icon="Edit" @click="handleUpdate(scope.row)" v-hasPermi="['wms:freeze:edit']">{{ t('common.edit') }}</el-button>
            <el-button v-else link type="info" icon="View" @click="handleView(scope.row)">{{ t('wms.receipt.details') }}</el-button>
            <el-button v-if="canEdit(scope.row)" link type="danger" icon="Delete" @click="handleDelete(scope.row)" v-hasPermi="['wms:freeze:remove']">{{ t('common.delete') }}</el-button>
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
              <el-form-item :label="t('wms.freeze.freezeNo')">
                <el-input v-model="form.freezeNo" :placeholder="t('wms.receipt.receiptNoAuto')" readonly />
              </el-form-item>
            </el-col>
            <el-col :span="12">
              <el-form-item :label="t('wms.freeze.freezeType')" prop="freezeType">
                <el-select v-model="form.freezeType" :placeholder="t('wms.freeze.ph.selectType')" style="width:100%">
                  <el-option :label="t('wms.freeze.typeQuality')" value="quality" />
                  <el-option :label="t('wms.freeze.typeLegal')" value="legal" />
                  <el-option :label="t('wms.freeze.typeReserve')" value="reserve" />
                  <el-option :label="t('wms.issueOrder.typeOther')" value="other" />
                </el-select>
              </el-form-item>
            </el-col>
            <el-col :span="12">
              <el-form-item :label="t('wms.common.warehouse')" prop="warehouseId">
                <WarehouseSelect v-model="form.warehouseId" :placeholder="t('wms.warehouse.ph.selectWarehouse')" :disabled="viewMode" @change="onWarehouseChange" />
              </el-form-item>
            </el-col>
            <el-col :span="12">
              <el-form-item :label="t('wms.common.location')" prop="locationId">
                <LocationCascader
                  v-model="form.locCascader"
                  :warehouse-id="form.warehouseId"
                  :disabled="!form.warehouseId || viewMode"
                  :placeholder="form.warehouseId ? t('wms.location.ph.selectZoneOrLocation') : t('wms.move.ph.selectWarehouseFirst')"
                  @update:model-value="onLocation"
                />
              </el-form-item>
            </el-col>
            <el-col :span="24">
              <el-form-item :label="t('wms.adjust.stockRow')" prop="stockId">
                <el-select
                  v-if="!viewMode"
                  v-model="form.stockId"
                  :loading="stockLoading"
                  :placeholder="form.locationId ? t('wms.freeze.ph.pickStockRow') : t('wms.adjust.ph.selectLocationFirst')"
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
              <el-form-item :label="t('wms.material.title')">
                <span>{{ form.materialCode }} - {{ form.materialName || "-" }}</span>
              </el-form-item>
            </el-col>
            <el-col :span="12">
              <el-form-item :label="t('wms.receiving.batchNo')">
                <span>{{ form.batchNo || t('wms.move.noBatch') }}</span>
              </el-form-item>
            </el-col>
            <el-col :span="12">
              <el-form-item :label="t('wms.freeze.freezeQuantity')" prop="freezeQuantity">
                <el-input-number v-model="form.freezeQuantity" :min="0" :controls="false"
                  :placeholder="form.stockAvailable != null ? t('wms.freeze.ph.freezable', { qty: form.stockAvailable }) : t('wms.freeze.freezeQuantity')"
                  style="width:100%" />
              </el-form-item>
            </el-col>
            <el-col :span="24">
              <el-form-item :label="t('wms.freeze.freezeReason')" prop="freezeReason">
                <el-input v-model="form.freezeReason" type="textarea" :autosize="{ minRows: 2, maxRows: 4 }"
                  :placeholder="t('wms.freeze.ph.inputReason')" maxlength="255" />
              </el-form-item>
            </el-col>
          </el-row>

          <div class="form-tip text-muted">
            {{ t('wms.freeze.formTip') }}
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

<script setup name="WmsFreeze">
import { ref, reactive, toRefs, computed } from "vue";
import { ElMessage, ElMessageBox } from "element-plus";
import { useI18n } from "vue-i18n";
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

const { t } = useI18n();
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
function freezeTypeLabel(v) {
  const map = {
    quality: t('wms.freeze.typeQuality'),
    legal: t('wms.freeze.typeLegal'),
    reserve: t('wms.freeze.typeReserve'),
    other: t('wms.issueOrder.typeOther'),
  };
  return map[v] || "-";
}

// 冻结状态：待冻结 → 已冻结 → 已解冻
function freezeStatusLabel(s) {
  const map = { pending: t('wms.freeze.statusPending'), frozen: t('wms.freeze.statusFrozen'), thawed: t('wms.freeze.statusThawed') };
  return map[s] || "-";
}
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
    warehouseId: [{ required: true, message: () => t('wms.receipt.rules.warehouseRequired'), trigger: "change" }],
    materialId: [{ required: true, message: () => t('wms.issueOrder.selectMaterialRequired'), trigger: "change" }],
    locationId: [{ required: true, message: () => t('wms.freeze.ph.selectLocation'), trigger: "change" }],
    freezeQuantity: [{ required: true, message: () => t('wms.freeze.ph.inputFreezeQty'), trigger: "blur" }],
    freezeType: [{ required: true, message: () => t('wms.freeze.ph.selectType'), trigger: "change" }],
  },
});

const { form, queryParams, rules } = toRefs(data);

const queryConfig = computed(() => [
  {
    label: t('wms.common.keywords'),
    prop: "keywords",
    type: "input",
    placeholder: t('wms.freeze.ph.inputFreezeNo'),
  },
  {
    label: t('common.status'),
    prop: "freezeStatus",
    type: "select",
    placeholder: t('wms.warehouse.ph.selectStatus'),
    options: [
      { label: t('wms.freeze.statusPending'), value: "pending" },
      { label: t('wms.freeze.statusFrozen'), value: "frozen" },
      { label: t('wms.freeze.statusThawed'), value: "thawed" },
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
  title.value = t('wms.freeze.addTitle');
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
          form.value.stockLabel = t('wms.freeze.stockRowGone');
        }
      })
      .finally(() => {
        stockLoading.value = false;
      });
  }
  open.value = true;
  title.value = viewMode.value ? t('wms.freeze.viewTitle') : t('wms.freeze.editTitle');
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
      label: `${s.materialCode || s.material?.materialCode} / ${t('wms.receiving.batchNo')}${s.batchNo || t('wms.move.noBatch')}${s.barcode ? " / " + s.barcode : ""} / ${t('wms.stock.available')} ${available}`,
      labelShort: `${s.materialCode || s.material?.materialCode} / ${t('wms.receiving.batchNo')}${s.batchNo || t('wms.move.noBatch')}${s.barcode ? " / " + s.barcode : ""} / ${t('wms.stock.stock')} ${s.stockQuantity}`,
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
      ElMessage.warning(t('wms.freeze.ph.selectLocation'));
      return;
    }
    if (!form.value.stockId) {
      ElMessage.warning(t('wms.freeze.pickStockRow'));
      return;
    }
    if (!(form.value.freezeQuantity > 0)) {
      ElMessage.warning(t('wms.freeze.qtyRequired'));
      return;
    }
    if (form.value.stockAvailable != null && form.value.freezeQuantity > form.value.stockAvailable) {
      ElMessage.warning(t('wms.freeze.qtyExceedWarn', { qty: form.value.stockAvailable }));
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

/** 执行冻结 */
function handleExecute(row) {
  ElMessageBox.confirm(
    t('wms.freeze.executeConfirmMsg', { no: row.freezeNo }),
    t('wms.freeze.executeConfirmTitle'),
    { confirmButtonText: t('wms.freeze.executeConfirmBtn'), cancelButtonText: t('common.cancel'), type: "warning" }
  )
    .then(() => executeFreeze(row.freezeId))
    .then(() => {
      ElMessage.success(t('wms.freeze.executeSuccess'));
      getList();
    });
}

/** 解冻 */
function handleThaw(row) {
  ElMessageBox.prompt(t('wms.freeze.thawReasonPrompt'), t('wms.freeze.thawConfirmTitle'), {
    confirmButtonText: t('wms.freeze.thawConfirmBtn'),
    cancelButtonText: t('common.cancel'),
    inputPlaceholder: t('wms.freeze.ph.thawReasonOptional'),
    inputType: "textarea",
  })
    .then(({ value }) => thawFreeze(row.freezeId, value))
    .then(() => {
      ElMessage.success(t('wms.freeze.thawSuccess'));
      getList();
    });
}

/** 删除 */
function handleDelete(row) {
  const delIds = row.freezeId || ids.value.join(",");
  ElMessageBox.confirm(t('wms.freeze.deleteConfirm', { id: delIds }), t('wms.common.warning'), {
    confirmButtonText: t('common.confirm'),
    cancelButtonText: t('common.cancel'),
    type: "warning",
  })
    .then(() => delFreeze(delIds))
    .then(() => {
      getList();
      ElMessage.success(t('wms.common.deleteSuccess'));
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
