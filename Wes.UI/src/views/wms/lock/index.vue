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
          <el-button type="primary" plain icon="Plus" @click="handleAdd" v-hasPermi="['wms:lock:add']">{{ t('common.add') }}</el-button>
        </el-col>
        <el-col :span="1.5">
          <el-button v-if="canEdit(selectedRows[0] || {})" type="success" plain icon="Edit" :disabled="single" @click="handleUpdate" v-hasPermi="['wms:lock:edit']">{{ t('common.edit') }}</el-button>
          <el-button v-else type="info" plain icon="View" :disabled="single" @click="handleView(selectedRows[0])">{{ t('wms.receipt.details') }}</el-button>
        </el-col>
        <el-col :span="1.5">
          <el-button type="danger" plain icon="Delete" :disabled="multiple || selectedRows.some((r) => !canEdit(r))" @click="handleDelete" v-hasPermi="['wms:lock:remove']">{{ t('common.delete') }}</el-button>
        </el-col>
        <right-toolbar
          v-model:showSearch="showSearch"
          @queryTable="getList"
        ></right-toolbar>
      </el-row>

      <!-- 列表 -->
      <el-table v-loading="loading" :data="lockList" @selection-change="handleSelectionChange">
        <el-table-column type="selection" width="55" align="center" />
        <el-table-column :label="t('wms.lock.lockNo')" align="center" prop="lockId" width="170" :show-overflow-tooltip="true" />
        <el-table-column :label="t('wms.lock.lockType')" align="center" width="110">
          <template #default="scope">
            {{ lockTypeLabel(scope.row.lockType) }}
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
            {{ scope.row.location?.locationCode || scope.row.locationId || "-" }}
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
        <el-table-column :label="t('wms.lock.lockQuantity')" align="center" prop="lockQuantity" width="90" />
        <el-table-column :label="t('wms.lock.usedQuantity')" align="center" prop="usedQuantity" width="80">
          <template #default="scope">
            {{ scope.row.usedQuantity ?? 0 }}
          </template>
        </el-table-column>
        <el-table-column :label="t('common.status')" align="center" width="100">
          <template #default="scope">
            <el-tag :type="lockStatusTag(scope.row.lockStatus)">
              {{ lockStatusLabel(scope.row.lockStatus) }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column :label="t('common.createTime')" align="center" prop="createTime" min-width="170" />
        <el-table-column :label="t('common.actions')" align="center" width="220" class-name="small-padding fixed-width">
          <template #default="scope">
            <el-button v-if="canEdit(scope.row)" link type="success" icon="Promotion" @click="handleExecute(scope.row)" v-hasPermi="['wms:lock:edit']">{{ t('wms.lock.lock') }}</el-button>
            <el-button v-if="scope.row.lockStatus === 'locked' || scope.row.lockStatus === 'partial'" link type="warning" icon="Unlock" @click="handleRelease(scope.row)" v-hasPermi="['wms:lock:edit']">{{ t('wms.lock.release') }}</el-button>
            <el-button v-if="canEdit(scope.row)" link type="primary" icon="Edit" @click="handleUpdate(scope.row)" v-hasPermi="['wms:lock:edit']">{{ t('common.edit') }}</el-button>
            <el-button v-else link type="info" icon="View" @click="handleView(scope.row)">{{ t('wms.receipt.details') }}</el-button>
            <el-button v-if="canEdit(scope.row)" link type="danger" icon="Delete" @click="handleDelete(scope.row)" v-hasPermi="['wms:lock:remove']">{{ t('common.delete') }}</el-button>
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
              <el-form-item :label="t('wms.lock.lockType')" prop="lockType">
                <el-select v-model="form.lockType" :placeholder="t('wms.lock.ph.selectType')" style="width:100%">
                  <el-option :label="t('wms.lock.typeOrder')" value="order" />
                  <el-option :label="t('wms.lock.typeProduce')" value="produce" />
                  <el-option :label="t('wms.lock.typeExhibit')" value="exhibit" />
                  <el-option :label="t('wms.lock.typeManual')" value="manual" />
                </el-select>
              </el-form-item>
            </el-col>
            <el-col :span="12">
              <el-form-item :label="t('wms.lock.sourceNo')">
                <el-input v-model="form.sourceNo" :placeholder="t('wms.lock.ph.sourceNo')" maxlength="50" />
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
                  :placeholder="form.locationId ? t('wms.lock.ph.pickStockRow') : t('wms.adjust.ph.selectLocationFirst')"
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
              <el-form-item :label="t('wms.lock.lockQuantity')" prop="lockQuantity">
                <el-input-number v-model="form.lockQuantity" :min="0" :controls="false"
                  :placeholder="form.stockAvailable != null ? t('wms.lock.ph.lockable', { qty: form.stockAvailable }) : t('wms.lock.lockQuantity')"
                  style="width:100%" />
              </el-form-item>
            </el-col>
            <el-col :span="12">
              <el-form-item :label="t('wms.lock.expireTime')">
                <el-date-picker v-model="form.expireTime" type="datetime" value-format="YYYY-MM-DD HH:mm:ss"
                  :placeholder="t('wms.lock.ph.expireTime')" style="width:100%" />
              </el-form-item>
            </el-col>
          </el-row>

          <div class="form-tip text-muted">
            {{ t('wms.lock.formTip') }}
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

<script setup name="WmsLock">
import { ref, reactive, toRefs, computed } from "vue";
import { ElMessage, ElMessageBox } from "element-plus";
import { useI18n } from "vue-i18n";
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

const { t } = useI18n();
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
const lockTypeMap = computed(() => ({
  order: t("wms.lock.typeOrder"),
  produce: t("wms.lock.typeProduce"),
  exhibit: t("wms.lock.typeExhibit"),
  manual: t("wms.lock.typeManual"),
}));

// 锁定状态：待锁定 → 已锁定 / 部分释放 → 已释放
const lockStatusMap = computed(() => ({
  pending: t("wms.lock.statusPending"),
  locked: t("wms.lock.statusLocked"),
  partial: t("wms.lock.statusPartial"),
  released: t("wms.lock.statusReleased"),
}));
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
    warehouseId: [{ required: true, message: () => t('wms.lock.rules.warehouseRequired'), trigger: "change" }],
    materialId: [{ required: true, message: () => t('wms.lock.rules.materialRequired'), trigger: "change" }],
    locationId: [{ required: true, message: () => t('wms.lock.rules.locationRequired'), trigger: "change" }],
    lockQuantity: [{ required: true, message: () => t('wms.lock.rules.quantityRequired'), trigger: "blur" }],
    lockType: [{ required: true, message: () => t('wms.lock.rules.typeRequired'), trigger: "change" }],
  },
});

const { form, queryParams, rules } = toRefs(data);

const queryConfig = computed(() => [
  {
    label: t('wms.common.keywords'),
    prop: "keywords",
    type: "input",
    placeholder: t('wms.lock.ph.keywords'),
  },
  {
    label: t('common.status'),
    prop: "lockStatus",
    type: "select",
    placeholder: t('wms.warehouse.ph.selectStatus'),
    options: [
      { label: t('wms.lock.statusPending'), value: "pending" },
      { label: t('wms.lock.statusLocked'), value: "locked" },
      { label: t('wms.lock.statusPartial'), value: "partial" },
      { label: t('wms.lock.statusReleased'), value: "released" },
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
  title.value = t("wms.lock.addTitle");
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
          form.value.stockLabel = t("wms.lock.stockRowGone");
        }
      })
      .finally(() => {
        stockLoading.value = false;
      });
  }
  open.value = true;
  title.value = viewMode.value ? t("wms.lock.viewTitle") : t("wms.lock.editTitle");
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
      label: `${s.materialCode || s.material?.materialCode} / ${t("wms.receiving.batchNo")}${s.batchNo || t("wms.move.noBatch")}${s.barcode ? " / " + s.barcode : ""} / ${t("wms.stock.available")} ${available}`,
      labelShort: `${s.materialCode || s.material?.materialCode} / ${t("wms.receiving.batchNo")}${s.batchNo || t("wms.move.noBatch")}${s.barcode ? " / " + s.barcode : ""} / ${t("wms.stock.stockQuantity")} ${s.stockQuantity}`,
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
      ElMessage.warning(t("wms.lock.rules.locationRequired"));
      return;
    }
    if (!form.value.stockId) {
      ElMessage.warning(t("wms.lock.pickStockRowWarn"));
      return;
    }
    if (!(form.value.lockQuantity > 0)) {
      ElMessage.warning(t("wms.lock.qtyRequired"));
      return;
    }
    if (form.value.stockAvailable != null && form.value.lockQuantity > form.value.stockAvailable) {
      ElMessage.warning(t("wms.lock.qtyExceedWarn", { qty: form.value.stockAvailable }));
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
      ElMessage.success(t("wms.common.savedSuccess"));
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
    t("wms.lock.executeConfirmMsg", { no: row.lockId }),
    t("wms.lock.executeConfirmTitle"),
    { confirmButtonText: t("wms.lock.executeConfirmBtn"), cancelButtonText: t("common.cancel"), type: "warning" }
  )
    .then(() => executeLock(row.lockId))
    .then(() => {
      ElMessage.success(t("wms.lock.executeSuccess"));
      getList();
    });
}

/** 释放锁定 */
function handleRelease(row) {
  ElMessageBox.confirm(
    t("wms.lock.releaseConfirmMsg", { no: row.lockId }),
    t("wms.lock.releaseConfirmTitle"),
    { confirmButtonText: t("wms.lock.releaseConfirmBtn"), cancelButtonText: t("common.cancel"), type: "warning" }
  )
    .then(() => releaseLock(row.lockId))
    .then(() => {
      ElMessage.success(t("wms.lock.releaseSuccess"));
      getList();
    });
}

/** 删除 */
function handleDelete(row) {
  const delIds = row.lockId || ids.value.join(",");
  ElMessageBox.confirm(t("wms.lock.deleteConfirm", { id: delIds }), t("wms.common.warning"), {
    confirmButtonText: t("common.confirm"),
    cancelButtonText: t("common.cancel"),
    type: "warning",
  })
    .then(() => delLock(delIds))
    .then(() => {
      getList();
      ElMessage.success(t("wms.common.deleteSuccess"));
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
