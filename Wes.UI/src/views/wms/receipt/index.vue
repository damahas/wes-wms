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
        <el-button type="primary" plain icon="Plus" @click="handleAdd" v-hasPermi="['wms:receipt:add']">{{ t('common.add') }}</el-button>
      </el-col>
      <el-col :span="1.5">
        <el-button v-if="canEdit(selectedRows[0] || {})" type="success" plain icon="Edit" :disabled="single" @click="handleUpdate" v-hasPermi="['wms:receipt:edit']">{{ t('common.edit') }}</el-button>
        <el-button v-else type="info" plain icon="View" :disabled="single" @click="handleView(selectedRows[0])">{{ t('wms.receipt.details') }}</el-button>
      </el-col>
      <el-col :span="1.5">
        <el-button type="danger" plain icon="Delete" :disabled="multiple || selectedRows.some((r) => !canEdit(r))" @click="handleDelete" v-hasPermi="['wms:receipt:remove']">{{ t('common.delete') }}</el-button>
      </el-col>
        <right-toolbar
          v-model:showSearch="showSearch"
          @queryTable="getList"
        ></right-toolbar>
    </el-row>

    <!-- 列表 -->
    <el-table v-loading="loading" :data="receiptList" @selection-change="handleSelectionChange">
      <el-table-column type="selection" width="55" align="center" />
      <el-table-column :label="t('wms.receipt.receiptNo')" align="center" prop="receiptNo" :show-overflow-tooltip="true" />
      <el-table-column :label="t('wms.receipt.receiptType')" align="center" :show-overflow-tooltip="true">
        <template #default="scope">
          {{ receiptTypeLabel(scope.row.receiptType) }}
        </template>
      </el-table-column>
      <el-table-column :label="t('wms.common.warehouse')" align="center" :show-overflow-tooltip="true">
        <template #default="scope">
          {{ scope.row.warehouse?.warehouseName || "-" }}
        </template>
      </el-table-column>
      <el-table-column :label="t('wms.receipt.totalQuantity')" align="center" prop="totalQuantity" />
      <el-table-column :label="t('common.status')" align="center" width="100">
        <template #default="scope">
          <el-tag :type="receiptStatusTag(scope.row.receiptStatus)">
            {{ receiptStatusLabel(scope.row.receiptStatus) }}
          </el-tag>
        </template>
      </el-table-column>
      <el-table-column :label="t('common.createTime')" align="center" prop="createTime" min-width="170" />
      <el-table-column :label="t('common.actions')" align="center" width="220" class-name="small-padding fixed-width">
        <template #default="scope">
          <el-button link type="warning" icon="Printer" @click="handlePrintLabel(scope.row)">{{ t('wms.receipt.label') }}</el-button>
          <el-button v-if="canEdit(scope.row)" link type="primary" icon="Edit" @click="handleUpdate(scope.row)" v-hasPermi="['wms:receipt:edit']">{{ t('common.edit') }}</el-button>
          <el-button v-else link type="info" icon="View" @click="handleView(scope.row)">{{ t('wms.receipt.details') }}</el-button>
          <el-button v-if="canEdit(scope.row)" link type="danger" icon="Delete" @click="handleDelete(scope.row)" v-hasPermi="['wms:receipt:remove']">{{ t('common.delete') }}</el-button>
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
    <el-dialog :title="title" v-model="open" width="90%" top="4vh" append-to-body>
      <el-form ref="receiptRef" :model="form" :rules="rules" label-width="88px" class="nowrap-labels" :disabled="viewMode">
        <!-- 基本信息 -->
        <div class="section-head">
          <span class="section-title">{{ t('wms.material.tabBasic') }}</span>
          <span class="section-extra">
            {{ form.receiptNo ? t('wms.receipt.receiptNoPrefix') + ' ' + form.receiptNo : t('wms.receipt.receiptNoAuto') }}
          </span>
        </div>
        <el-row :gutter="16">
          <el-col :span="6">
            <el-form-item :label="t('wms.receipt.receiptType')" prop="receiptType">
              <el-select v-model="form.receiptType" :placeholder="t('wms.receipt.ph.selectType')" clearable style="width:100%" :disabled="!!form.receiptId" @change="onReceiptTypeChange">
                <el-option :label="t('wms.receipt.typePurchase')" value="purchase" />
                <el-option :label="t('wms.receipt.typeProduce')" value="produce" />
                <el-option :label="t('wms.receipt.typeReturnIn')" value="return_in" />
                <el-option :label="t('wms.receipt.typeOther')" value="other" />
              </el-select>
            </el-form-item>
          </el-col>
          <el-col :span="6">
            <el-form-item :label="t('wms.common.warehouse')" prop="warehouseId">
              <WarehouseSelect v-model="form.warehouseId" :placeholder="t('wms.warehouse.ph.selectWarehouse')" :disabled="viewMode" @change="onWarehouseChange" />
            </el-form-item>
          </el-col>
          <el-col :span="6">
            <el-form-item :label="t('wms.receipt.receiptDate')" prop="receiptDate">
              <el-date-picker v-model="form.receiptDate" type="date" value-format="YYYY-MM-DD"
                :placeholder="t('wms.receipt.ph.selectDate')" style="width:100%" />
            </el-form-item>
          </el-col>
          <el-col :span="6">
            <el-form-item v-if="form.receiptType === 'purchase'" :label="t('wms.receipt.sourceReceiving')" prop="sourceId">
              <ReceivingSelect v-model="form.sourceId" :placeholder="t('wms.receiving.ph.selectReceiving')" :disabled="!!form.receiptId" @change="onReceivingChange" />
            </el-form-item>
            <el-form-item v-else-if="form.receiptType === 'produce'" :label="t('wms.receipt.sourceProduce')" prop="sourceId">
              <el-input v-model="form.sourceId" :placeholder="t('wms.receipt.ph.inputProduceNo')" maxlength="50" />
            </el-form-item>
            <el-form-item v-else-if="form.receiptType === 'return_in'" :label="t('wms.receipt.sourceReturn')" prop="sourceId">
              <el-input v-model="form.sourceId" :placeholder="t('wms.receipt.ph.inputReturnNo')" maxlength="50" />
            </el-form-item>
            <el-form-item v-else-if="form.receiptType === 'other'" :label="t('wms.receipt.sourceOther')" prop="sourceId">
              <el-input v-model="form.sourceId" :placeholder="t('wms.receipt.ph.inputSourceNo')" maxlength="50" />
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="16">
          <el-col :span="24">
            <el-form-item :label="t('common.remark')" prop="remark">
              <el-input v-model="form.remark" type="textarea" :autosize="{ minRows: 1, maxRows: 3 }"
                :placeholder="t('wms.receipt.ph.inputRemarkOptional')" maxlength="500" />
            </el-form-item>
          </el-col>
        </el-row>
      </el-form>

      <!-- 入库明细 -->
      <div class="detail-section">
        <!-- 入库明细 -->
        <div class="section-head">
          <span class="section-title">{{ t('wms.receipt.detailTitle') }}</span>
          <span class="section-badge">{{ detailList.length }}</span>
          <el-button class="section-extra detail-add-btn" type="primary" plain icon="Plus"
            @click="openMaterialDialog" v-if="!form.receiptId && !viewMode">{{ t('wms.receiving.addDetail') }}</el-button>
        </div>
        <el-row :gutter="16">
          <!-- 左：明细列表（一行一条，状态看色点，详情悬停显示） -->
          <el-col :span="7">
            <div class="detail-list">
              <div
                v-for="(row, index) in detailList"
                :key="index"
                class="detail-list-item"
                :class="{ active: row === currentDetailRow }"
                :title="itemTip(row)"
                @click="onDetailRowClick(row)"
              >
                <span class="item-seq">{{ index + 1 }}</span>
                <span class="item-title">
                  <span class="item-name">
                    {{
                      row.materialName
                        ? (row.materialCode ? row.materialCode + " - " + row.materialName : row.materialName)
                        : t('wms.receipt.noMaterial')
                    }}
                  </span>
                  <span v-if="row.spec" class="item-spec">{{ row.spec }}</span>
                </span>
                <span class="item-qty">{{ row.receiptQuantity || 0 }}{{ row.unit || "" }}</span>
                <span class="status-dot" :class="'dot-' + (row.detailStatus || 'pending')" />
              </div>
              <el-empty v-if="!detailList.length" :description="t('wms.receipt.noDetail')" :image-size="60" />
            </div>
          </el-col>

          <!-- 右：Tab 分区，一屏只显示一块 -->
          <el-col :span="17" class="pane-divider">
            <div v-if="currentDetailRow" class="detail-pane">
              <el-tabs v-model="detailTab" class="pane-tabs">
                <!-- 明细信息 -->
                <el-tab-pane name="info">
                  <template #label>{{ t('wms.receipt.tabInfo') }}</template>
                  <div class="pane-header">
                    <span class="pane-title">
                      {{ t('wms.receipt.detail') }} #{{ currentDetailIndex + 1 }}：
                      <span v-if="currentDetailRow.materialCode" class="material-code">{{ currentDetailRow.materialCode }}</span>
                      {{ currentDetailRow.materialName || t('wms.receipt.unselectedMaterial') }}
                      <span v-if="currentDetailRow.spec" class="text-muted">（{{ currentDetailRow.spec }}）</span>
                    </span>
                    <span class="pane-ops">
                      <el-tag v-if="currentDetailRow.detailStatus === 'shelved'" type="success" size="small">
                        {{ t('wms.receipt.shelvedReadonly') }}
                      </el-tag>
                      <el-button
                        v-if="!viewMode && currentDetailRow.detailId && currentDetailRow.detailStatus !== 'shelved'"
                        size="small" type="primary" @click="openShelve(currentDetailRow)"
                      >{{ t('wms.receipt.shelve') }}</el-button>
                      <el-button
                        v-if="!form.receiptId && !viewMode" size="small" type="danger" plain icon="Delete"
                        @click="removeDetailRow(currentDetailIndex)"
                      >{{ t('common.delete') }}</el-button>
                    </span>
                  </div>
                  <el-form :model="currentDetailRow" label-width="84px" class="nowrap-labels" :disabled="viewMode || currentDetailRow.detailStatus === 'shelved'">
                    <el-row :gutter="16">
                      <el-col :span="12">
                        <el-form-item :label="t('wms.receipt.quantity')">
                          <el-input-number controls-position="right" v-model="currentDetailRow.receiptQuantity" :min="0" style="width:100%" />
                        </el-form-item>
                      </el-col>
                      <el-col :span="12">
                        <el-form-item :label="t('wms.receiving.unit')">
                          <el-select v-model="currentDetailRow.unit" :placeholder="t('wms.receiving.unit')" filterable style="width:100%">
                            <el-option
                              v-for="opt in unitOptions(currentDetailRow)"
                              :key="opt.specId"
                              :label="opt.label"
                              :value="opt.value"
                            />
                          </el-select>
                        </el-form-item>
                      </el-col>
                      <el-col :span="24">
                        <el-form-item :label="t('wms.common.location')">
                          <!-- 仓库由单据固定，这里只选库区/库位 -->
                          <LocationCascader
                            v-model="currentDetailRow.locCascader"
                            :warehouse-id="form.warehouseId"
                            :disabled="viewMode || !form.warehouseId"
                            :placeholder="form.warehouseId ? t('wms.location.ph.selectZoneOrLocation') : t('wms.receipt.ph.selectWarehouseFirst')"
                            @update:model-value="(val) => (currentDetailRow.locationId = val?.[1])"
                          />
                        </el-form-item>
                      </el-col>
                    </el-row>
                    <!-- 批次 / 日期 / 供应商：纯分组标题，无折叠交互（避免误点） -->
                    <div class="detail-group">
                      <div class="group-title">{{ t('wms.receipt.groupBatch') }}</div>
                      <el-row :gutter="16">
                        <el-col :span="12">
                          <el-form-item :label="t('wms.receiving.batchNo')">
                            <el-input v-model="currentDetailRow.batchNo" :placeholder="t('wms.receiving.batchNo')" />
                          </el-form-item>
                        </el-col>
                        <el-col :span="12">
                          <el-form-item :label="t('wms.material.supplier')">
                            <PartnerSelect v-model="currentDetailRow.supplierId" partner-type="supplier" :placeholder="t('wms.material.ph.clickSupplier')" :disabled="viewMode" />
                          </el-form-item>
                        </el-col>
                        <el-col :span="12">
                          <el-form-item :label="t('wms.receipt.productionDate')">
                            <el-date-picker v-model="currentDetailRow.materialionDate" type="date" value-format="YYYY-MM-DD"
                              :placeholder="t('wms.receipt.productionDate')" style="width:100%" />
                          </el-form-item>
                        </el-col>
                        <el-col :span="12">
                          <el-form-item :label="t('wms.receipt.expireDate')">
                            <el-date-picker v-model="currentDetailRow.expireDate" type="date" value-format="YYYY-MM-DD"
                              :placeholder="t('wms.receipt.expireDate')" style="width:100%" />
                          </el-form-item>
                        </el-col>
                      </el-row>
                    </div>
                  </el-form>
                </el-tab-pane>

                <!-- 条码 -->
                <el-tab-pane name="barcode">
                  <template #label>
                    {{ t('wms.receipt.barcode') }}<span class="tab-badge">{{ barcodeList.length }}</span>
                  </template>
                  <template v-if="currentDetailRow.detailId">
                    <div class="pane-header">
                      <span class="text-muted">
                        {{ t('wms.receipt.barcodeSummary', { total: barcodeTotal, qty: currentDetailQty }) }}<template v-if="!viewMode">　{{ t('wms.receipt.splitHint') }}</template>
                      </span>
                      <el-button v-if="!viewMode" size="small" type="primary" :disabled="selectedBarcodes.length < 2" @click="doMergeBarcodes">
                        {{ t('wms.receipt.mergeSelected') }}
                      </el-button>
                    </div>
                    <el-table :data="barcodeList" border max-height="400" @selection-change="(v) => (selectedBarcodes = v)">
                      <el-table-column v-if="!viewMode" type="selection" width="45" :selectable="(row) => row.status !== 'shelved'" />
                      <el-table-column :label="t('wms.common.index')" prop="seqNo" width="60" align="center" />
                      <el-table-column :label="t('wms.receipt.barcode')" prop="barcode" min-width="160" />
                      <el-table-column :label="t('wms.receipt.quantity')" prop="quantity" width="80" align="center" />
                      <el-table-column :label="t('common.status')" width="90" align="center">
                        <template #default="scope">
                          <el-tag :type="scope.row.status === 'shelved' ? 'success' : 'info'" size="small">
                            {{ scope.row.status === "shelved" ? t('wms.receipt.statusShelved') : t('wms.receipt.statusPendingShelve') }}
                          </el-tag>
                        </template>
                      </el-table-column>
                      <el-table-column v-if="!viewMode" :label="t('common.actions')" width="80" align="center">
                        <template #default="scope">
                          <el-button link type="primary" :disabled="scope.row.status === 'shelved'"
                            @click="splitBarcodeRow(scope.row)">{{ t('wms.receipt.split') }}</el-button>
                        </template>
                      </el-table-column>
                    </el-table>
                  </template>
                  <el-empty v-else :description="t('wms.receipt.barcodeEmpty')" :image-size="60" />
                </el-tab-pane>
              </el-tabs>
            </div>
            <el-empty v-else :description="viewMode ? t('wms.receipt.clickToView') : t('wms.receipt.clickToEdit')" :image-size="60" />
          </el-col>
        </el-row>
      </div>

      <template #footer>
        <div class="dialog-footer">
          <el-button v-if="!viewMode" type="primary" @click="submitForm">{{ t('common.submit') }}</el-button>
          <el-button @click="cancel">{{ viewMode ? t('common.close') : t('common.cancel') }}</el-button>
        </div>
      </template>
    </el-dialog>

    <!-- 明细上架 -->
    <el-dialog :title="t('wms.receipt.shelveTitle')" v-model="shelveOpen" width="460px" append-to-body>
      <el-form :model="shelveForm" label-width="90px">
        <el-form-item :label="t('wms.receipt.pendingBarcodeCount')">
          <el-input :model-value="t('wms.receipt.pendingBarcodeValue', { count: shelveForm.pendingBarcodeCount || 0 })" readonly />
        </el-form-item>
        <el-form-item :label="t('wms.material.title')">
          <el-input :model-value="shelveForm.materialLabel" readonly />
        </el-form-item>
        <el-form-item :label="t('wms.receipt.pendingQty')">
          <el-input :model-value="String(shelveForm.remainQty)" readonly />
        </el-form-item>
        <el-form-item :label="t('wms.receipt.shelveBarcodeCount')">
          <el-input-number v-model="shelveForm.quantity" :min="1" :max="shelveForm.pendingBarcodeCount || shelveForm.remainQty || 1"
            controls-position="right" style="width:100%" />
        </el-form-item>
        <el-form-item :label="t('wms.receipt.shelveLocation')">
          <!-- 仓库由单据固定，只选库区/库位 -->
          <LocationCascader
            v-model="shelveForm.locCascader"
            :warehouse-id="form.warehouseId"
            @update:model-value="(val) => (shelveForm.locationId = val?.[1])"
          />
        </el-form-item>
      </el-form>
      <template #footer>
        <el-button type="primary" @click="submitShelve">{{ t('common.submit') }}</el-button>
        <el-button @click="shelveOpen = false">{{ t('common.cancel') }}</el-button>
      </template>
    </el-dialog>

    <!-- 隐藏的物料多选组件，通过 ref 调用 open() 弹出 -->
    <MaterialMultiSelect
      ref="materialMultiSelectRef"
      :multiple="true"
      @change="onMaterialMultiSelect"
      v-show="false"
    />

    </div>
  </div>
</template>

<script setup name="WmsReceipt">
import { ref, reactive, toRefs, computed, watch } from "vue";
import { ElMessage, ElMessageBox } from "element-plus";
import { useI18n } from "vue-i18n";
import {
  listReceipt,
  getReceipt,
  addReceipt,
  updateReceipt,
  delReceipt,
  shelveReceiptDetail,
  getReceiptBarcodes,
  splitReceiptBarcode,
  mergeReceiptBarcodes,
} from "@/api/wms/receipt";
import { getReceivingDetailList } from "@/api/wms/receiving";
import { getMaterial } from "@/api/wms/material";
import { listPackSpecTree } from "@/api/wms/packSpec";
import QueryForm from "@/components/QueryForm/index.vue";
import RightToolbar from "@/components/RightToolbar/index.vue";
import WarehouseSelect from "@/views/wms/warehouse/input.vue";
import PartnerSelect from "@/views/wms/partner/input.vue";
import ReceivingSelect from "@/views/wms/receiving/input.vue";
import LocationCascader from "@/views/wms/location/cascader.vue";
import MaterialMultiSelect from "@/views/wms/material/select.vue";

const { t } = useI18n();
const receiptRef = ref(null);
const receiptList = ref([]);
const open = ref(false);
const loading = ref(true);
const ids = ref([]);
const single = ref(true);
const multiple = ref(true);
const total = ref(0);
const title = ref("");
/** 只读查看模式：单据已开始上架时，弹窗只用于查看明细 */
const viewMode = ref(false);
/** 列表当前勾选的行（顶部按钮据此在 修改 / 明细 之间切换） */
const selectedRows = ref([]);

function receiptTypeLabel(v) {
  const map = {
    purchase: t('wms.receipt.typePurchase'),
    produce: t('wms.receipt.typeProduce'),
    return_in: t('wms.receipt.typeReturnIn'),
    other: t('wms.receipt.typeOther'),
  };
  return map[v] || "-";
}

// 入库单状态：待入库 → 上架中（部分明细已上架）→ 已入库（全部明细上架完成）
function receiptStatusLabel(s) {
  const map = { pending_in: t('wms.receipt.statusPendingIn'), shelfing: t('wms.receipt.statusShelfing'), in_stock: t('wms.receipt.statusInStock') };
  return map[s] || "-";
}
const receiptStatusTag = (s) => (s === "in_stock" ? "success" : s === "shelfing" ? "warning" : "info");

// 已开始上架（上架中/已入库）的单据不能再修改或删除：
// 编辑会重建明细与条码，已上架的库存会失去来源记录
function canEdit(row) {
  return row.receiptStatus !== "in_stock" && row.receiptStatus !== "shelfing";
}

// 明细上架状态
function detailStatusLabel(s) {
  const map = { pending: t('wms.receipt.statusPendingShelve'), shelving: t('wms.receipt.statusShelfing'), shelved: t('wms.receipt.statusShelved') };
  return map[s] || "-";
}

const showSearch = ref(true);

const detailList = ref([]);

/** 主子表：当前在右侧编辑区编辑的明细行 */
const currentDetailRow = ref(null);
const currentDetailIndex = ref(-1);

/** 右侧 Tab：info-明细信息 barcode-条码 */
const detailTab = ref("info");

/** 点击表格行 → 右侧编辑区切换到该行 */
function onDetailRowClick(row) {
  currentDetailRow.value = row;
  currentDetailIndex.value = detailList.value.indexOf(row);
}

/** 左侧列表悬停提示：把规格、状态、上架进度收进 tooltip，保持列表一行一条 */
function itemTip(row) {
  const parts = [];
  parts.push(detailStatusLabel(row.detailStatus || "pending"));
  parts.push(`${t('wms.receipt.shelvedProgress', { a: row.shelfQuantity || 0, b: row.receiptQuantity || 0 })}`);
  return parts.join("　");
}



// 全部包装规格（按需加载并缓存，用于按物料包装规格构建单位下拉项）
const allPackSpecs = ref([]);

/** 今天（yyyy-MM-dd） */
function today() {
  const d = new Date();
  const p = (n) => String(n).padStart(2, "0");
  return `${d.getFullYear()}-${p(d.getMonth() + 1)}-${p(d.getDate())}`;
}

const data = reactive({
  form: {},
  queryParams: {
    pageNum: 1,
    pageSize: 10,
    keywords: undefined,
    receiptStatus: undefined,
  },
  rules: {
    receiptType: [{ required: true, message: () => t('wms.receipt.rules.typeRequired'), trigger: "change" }],
    warehouseId: [{ required: true, message: () => t('wms.receipt.rules.warehouseRequired'), trigger: "change" }],
    receiptDate: [{ required: true, message: () => t('wms.receipt.rules.dateRequired'), trigger: "change" }],
    sourceId: [
      {
        // 采购入库必须关联收货单，其余类型来源单号可留空
        validator: (rule, value, callback) => {
          if (form.value.receiptType === "purchase" && !value) callback(new Error(t('wms.receipt.rules.receivingRequired')));
          else callback();
        },
        trigger: "change",
      },
    ],
  },
});

const { form, queryParams, rules } = toRefs(data);

const queryConfig = computed(() => [
  {
    label: t('wms.common.keywords'),
    prop: "keywords",
    type: "input",
    placeholder: t('common.input'),
  },
  {
    label: t('common.status'),
    prop: "receiptStatus",
    type: "select",
    placeholder: t('wms.warehouse.ph.selectStatus'),
    options: [
      { label: t('wms.receipt.statusPendingIn'), value: "pending_in" },
      { label: t('wms.receipt.statusShelfing'), value: "shelfing" },
      { label: t('wms.receipt.statusInStock'), value: "in_stock" },
    ],
  },
]);

/** 查询列表 */
function getList() {
  loading.value = true;
  listReceipt(queryParams.value).then((res) => {
    receiptList.value = res.rows || [];
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
    receiptStatus: undefined,
    pageNum: 1,
    pageSize: 10,
  };
  handleQuery();
}

/** 多选框 */
function handleSelectionChange(selection) {
  selectedRows.value = selection;
  ids.value = selection.map((item) => item.receiptId);
  single.value = selection.length !== 1;
  multiple.value = !selection.length;
}

/** 表单重置 */
function reset() {
  form.value = {
    receiptId: undefined,
    receiptNo: undefined,
    receiptType: undefined,
    warehouseId: undefined,
    warehouse: undefined,
    sourceId: undefined,
    sourceNo: undefined,
    sourceType: undefined,
    receiptDate: today(),
    remark: undefined,
  };
  detailList.value = [];
  currentDetailRow.value = null;
  currentDetailIndex.value = -1;
  viewMode.value = false;
  if (receiptRef.value) receiptRef.value.resetFields();
}

/** 新增 */
function handleAdd() {
  reset();
  open.value = true;
  title.value = t('wms.receipt.addTitle');
}

/** 修改 */
function handleUpdate(row) {
  reset();
  const id = row?.receiptId || ids.value[0];
  getReceipt(id).then((res) => fillForm(res.data || {}));
}

/** 查看明细：单据已开始上架时不给编辑，只读打开 */
function handleView(row) {
  if (!row?.receiptId) return;
  reset();
  viewMode.value = true;
  getReceipt(row.receiptId).then((res) => fillForm(res.data || {}));
}

/** 填充表单（修改、上架后刷新共用） */
function fillForm(data) {
  form.value = {
    receiptId: data.receiptId,
    receiptNo: data.receiptNo,
    receiptType: data.receiptType,
    receiptStatus: data.receiptStatus,
    warehouseId: data.warehouseId,
    warehouse: data.warehouse,
    sourceId: data.sourceId,
    sourceNo: data.sourceNo,
    sourceType: data.sourceType,
    receiptDate: data.receiptDate ? String(data.receiptDate).slice(0, 10) : today(),
    remark: data.remark,
  };
  detailList.value = (data.details || []).map((d) => ({
    ...d,
    // 物料信息优先取后端补出的顶层字段，兼容物料导航对象
    materialCode: d.materialCode || d.material?.materialCode || "",
    materialName: d.materialName || d.material?.materialName || "",
    materialPackSpecId: d.materialPackSpecId || d.material?.packSpecId || null,
    materialUnit: d.materialUnit || d.unit || d.material?.unit || "",
    detailStatus: d.detailStatus || "pending",
    shelfQuantity: d.shelfQuantity || 0,
    // 库位级联回显：[库区, 库位]（仓库由单据固定）
    locCascader: d.locationId
      ? [d.location?.zoneId, d.locationId].filter(Boolean)
      : [],
  }));
  // 加载各物料包装规格，供单位下拉使用
  Promise.all(detailList.value.map((r) => ensurePackSpecLoaded(r.materialPackSpecId)));
  // 默认选中第一条明细到编辑区
  if (detailList.value.length) onDetailRowClick(detailList.value[0]);
  open.value = true;
  title.value = viewMode.value ? t('wms.receipt.viewTitle') : t('wms.receipt.editTitle');
}

/** 上架弹窗 */
const shelveOpen = ref(false);
const shelveForm = reactive({
  receiptId: null,
  detailId: null,
  pendingBarcodeCount: 0,
  materialLabel: "",
  remainQty: 0,
  quantity: 1,
  locationId: null,
  locCascader: [],
});

/** 条码：内嵌在右侧，随选中的明细自动加载 */
const barcodeList = ref([]);
const selectedBarcodes = ref([]);
const barcodeTotal = computed(() =>
  barcodeList.value.reduce((s, r) => s + (Number(r.quantity) || 0), 0)
);
const currentDetailQty = computed(() => currentDetailRow.value?.receiptQuantity || 0);

// 左侧切换明细行 → 右侧自动加载该明细的条码
watch(currentDetailRow, (row) => {
  selectedBarcodes.value = [];
  loadBarcodes(row);
});

async function loadBarcodes(row) {
  const detailId = (row || currentDetailRow.value)?.detailId;
  if (!detailId) {
    barcodeList.value = [];
    return;
  }
  const r = await getReceiptBarcodes(form.value.receiptId, detailId);
  barcodeList.value = r.rows || r.data || [];
}

/** 拆分：均分填份数（如 3），不等分填逗号分隔的数量（如 10,5） */
async function splitBarcodeRow(row) {
  try {
    const { value } = await ElMessageBox.prompt(
      t('wms.receipt.splitPrompt', { qty: row.quantity }),
      t('wms.receipt.splitTitle'),
      { inputPattern: /^[\d.\s,]+$/, inputErrorMessage: t('wms.receipt.splitInputError') }
    );
    const input = (value || "").trim();
    let quantities = [];
    if (input.includes(",")) {
      quantities = input.split(",").map((s) => Number(s.trim())).filter((n) => !isNaN(n) && n > 0);
    } else {
      const n = Number(input);
      if (!Number.isInteger(n) || n < 2) {
        ElMessage.warning(t('wms.receipt.splitCountWarn'));
        return;
      }
      const base = Math.floor((row.quantity || 0) / n);
      quantities = Array.from({ length: n }, (_, i) =>
        i === n - 1 ? Number(row.quantity) - base * (n - 1) : base
      );
    }
    if (quantities.length < 2) {
      ElMessage.warning(t('wms.receipt.splitMinWarn'));
      return;
    }
    await splitReceiptBarcode(form.value.receiptId, currentDetailRow.value.detailId, {
      barcodeId: row.barcodeId,
      quantities,
    });
    ElMessage.success(t('wms.receipt.splitSuccess'));
    await loadBarcodes();
    await refreshCurrentDetail();
  } catch (e) {
    /* 取消输入 */
  }
}

async function doMergeBarcodes() {
  if (selectedBarcodes.value.length < 2) return;
  try {
    await mergeReceiptBarcodes(form.value.receiptId, currentDetailRow.value.detailId, {
      barcodeIds: selectedBarcodes.value.map((r) => r.barcodeId),
    });
    ElMessage.success(t('wms.receipt.mergeSuccess'));
    selectedBarcodes.value = [];
    await loadBarcodes();
    await refreshCurrentDetail();
  } catch (e) {
    /* ignore */
  }
}

/** 拆/合后刷新该明细行的条码统计 */
async function refreshCurrentDetail() {
  const res = await getReceipt(form.value.receiptId);
  const data = res.data || res;
  const d = (data.details || []).find((x) => x.detailId === currentDetailRow.value?.detailId);
  if (d && currentDetailRow.value) {
    currentDetailRow.value.barcodeCount = d.barcodeCount || 0;
    currentDetailRow.value.shelvedBarcodeCount = d.shelvedBarcodeCount || 0;
  }
}

/** 打开上架弹窗 */
function openShelve(row) {
  const remain = (row.receiptQuantity || 0) - (row.shelfQuantity || 0);
  if (remain <= 0) {
    ElMessage.warning(t('wms.receipt.allShelved'));
    return;
  }
  const pendingBarcodes = Math.max((row.barcodeCount || 0) - (row.shelvedBarcodeCount || 0), 0);
  shelveForm.receiptId = form.value.receiptId;
  shelveForm.detailId = row.detailId;
  shelveForm.pendingBarcodeCount = pendingBarcodes;
  shelveForm.materialLabel = [row.materialCode, row.materialName].filter(Boolean).join(" - ");
  shelveForm.remainQty = remain;
  shelveForm.quantity = pendingBarcodes || remain;
  shelveForm.locationId = row.locationId || null;
  shelveForm.locCascader = row.locCascader || [];
  shelveOpen.value = true;
}

/** 提交上架 */
function submitShelve() {
  if (!shelveForm.locationId) {
    ElMessage.warning(t('wms.receipt.selectShelveLocation'));
    return;
  }
  if (!shelveForm.quantity || shelveForm.quantity <= 0) {
    ElMessage.warning(t('wms.receipt.shelveQtyWarn'));
    return;
  }
  const receiptId = shelveForm.receiptId;
  shelveReceiptDetail(receiptId, shelveForm.detailId, {
    locationId: shelveForm.locationId,
    quantity: shelveForm.quantity,
  }).then(() => {
    ElMessage.success(t('wms.receipt.shelveSuccess'));
    shelveOpen.value = false;
    // 刷新明细状态/条码/已上架数量与主表状态
    getReceipt(receiptId).then((res) => fillForm(res.data || {}));
    getList();
  });
}

/** 提交 */
function submitForm() {
  receiptRef.value.validate((valid) => {
    if (!valid) return;
    // 校验明细
    if (detailList.value.length === 0) {
      ElMessage.warning(t('wms.receipt.selectAtLeastOneDetail'));
      return;
    }
    for (let i = 0; i < detailList.value.length; i++) {
      const d = detailList.value[i];
      if (!d.materialId) {
        ElMessage.warning(t('wms.receipt.rowMaterialRequired', { no: i + 1 }));
        return;
      }
      if (!d.receiptQuantity || d.receiptQuantity <= 0) {
        ElMessage.warning(t('wms.receipt.rowQtyRequired', { no: i + 1 }));
        return;
      }
    }
    const api = form.value.receiptId ? updateReceipt : addReceipt;
    const submitData = { ...form.value };
    delete submitData.warehouse;
    // 汇总总数量
    submitData.totalQuantity = detailList.value.reduce((s, d) => s + (d.receiptQuantity || 0), 0);
    // 清理明细中的临时字段
    submitData.details = detailList.value.map((d) => {
      const { materialCode, materialName, supplier, ...rest } = d;
      return rest;
    });
    api(submitData).then(() => {
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

/** 删除 */
function handleDelete(row) {
  const delIds = row.receiptId || ids.value.join(",");
  ElMessageBox.confirm(t('wms.common.deleteConfirm', { id: delIds }), t('wms.common.warning'), {
    confirmButtonText: t('common.confirm'),
    cancelButtonText: t('common.cancel'),
    type: "warning",
  })
    .then(() => delReceipt(delIds))
    .then(() => {
      getList();
      ElMessage.success(t('wms.common.deleteSuccess'));
    });
}

function onWarehouseChange(data) {
  form.value.warehouse = data;
  // 仓库变了，明细与上架弹窗里的库区/库位不再是同一仓库下的，清空避免错库位
  detailList.value.forEach((d) => {
    d.locCascader = [];
    d.locationId = undefined;
  });
  shelveForm.locCascader = [];
  shelveForm.locationId = null;
}

function onReceiptTypeChange() {
  form.value.sourceId = undefined;
  form.value.sourceNo = undefined;
  form.value.sourceType = undefined;
  detailList.value = [];
}

/** 收货单选择回调：自动回填来源单据信息并带出明细 */
async function onReceivingChange(data) {
  if (!data) {
    form.value.sourceId = undefined;
    form.value.sourceNo = undefined;
    form.value.sourceType = undefined;
    return;
  }
  form.value.sourceType = "receiving";
  form.value.sourceNo = data.receivingNo;
  // 回填收货单明细到入库明细
  try {
    const res = await getReceivingDetailList(data.receivingId);
    const rows = res.rows || res.data || [];
    detailList.value = rows.map((d) => ({
      detailId: 0,
      materialId: d.materialId,
      materialCode: d.materialCode || d.material?.materialCode || "",
      materialName: d.materialName || d.material?.materialName || "",
      spec: d.spec || d.material?.spec || "",
      unit: d.unit || "",
      materialPackSpecId: d.materialPackSpecId || d.material?.packSpecId || null,
      materialUnit: d.materialUnit || d.unit || d.material?.unit || "",
      batchNo: d.batchNo || "",
      materialionDate: undefined,
      expireDate: undefined,
      receiptQuantity: Math.max(0, (d.pendingInboundQty || 0) - (d.stockInQuantity || 0)),
      locationId: undefined,
      supplierId: data.supplierId,
      supplier: data.supplier,
    }));
    // 加载各物料的包装规格，供单位下拉使用
    await Promise.all(detailList.value.map((r) => ensurePackSpecLoaded(r.materialPackSpecId)));
  } catch (e) {
    console.error("加载收货单明细失败：", e);
    ElMessage.error(t('wms.receipt.loadReceivingDetailFailed'));
  }
}

// ============ 包装规格/单位 辅助 ============
/** 按需加载指定 rootId 的包装规格层级树（缓存复用） */
async function ensurePackSpecLoaded(rootId) {
  if (!rootId) return;
  if (allPackSpecs.value.some((p) => p.rootId === rootId)) return;
  try {
    const r = await listPackSpecTree(rootId);
    const nodes = r.rows || r.data || [];
    allPackSpecs.value.push(...nodes);
  } catch { /* ignore */ }
}

/** 按层级排序（自顶向下） */
function sortTopDown(nodes) {
  const result = [];
  function walk(node) {
    result.push(node);
    nodes.filter((n) => n.parentId === node.specId).forEach(walk);
  }
  nodes.filter((n) => !n.parentId).forEach(walk);
  return result;
}

/** 计算某节点到基本单位的换算系数 */
function convFactor(node, allNodes) {
  if (node.isBase === 1) return 1;
  const child = allNodes.find((n) => n.parentId === node.specId);
  if (!child) return 1;
  return (child.qtyPerParent || 1) * convFactor(child, allNodes);
}

/** 某明细行的单位下拉项：取物料包装规格各层 uom，去重后自顶向下排序 */
function unitOptions(row) {
  const opts = [];
  const psId = row.materialPackSpecId;
  if (psId && allPackSpecs.value.length) {
    const set = allPackSpecs.value.filter((p) => p.rootId === psId && p.isDel !== 1);
    sortTopDown(set).forEach((p) => {
      if (p.uom && !opts.some((o) => o.value === p.uom)) {
        let label = p.uom;
        if (p.isBase === 1) {
          label += `（${t('wms.receiving.baseUnit')}）`;
        } else if (!p.parentId) {
          label += `（${t('wms.receiving.topLevel')}）`;
        } else {
          const factor = convFactor(p, set);
          const baseUom = set.find((n) => n.isBase === 1)?.uom || p.uom;
          label += `（1${p.uom}=${factor}${baseUom}）`;
        }
        opts.push({ specId: p.specId, value: p.uom, label });
      }
    });
  }
  if (row.materialUnit && !opts.some((o) => o.value === row.materialUnit)) {
    opts.push({ specId: "mu", value: row.materialUnit, label: row.materialUnit });
  }
  // 兜底：保证已存单位可选
  if (row.unit && !opts.some((o) => o.value === row.unit)) {
    opts.push({ specId: "cu", value: row.unit, label: row.unit });
  }
  return opts;
}

// ============ 物料多选添加明细 ============
const materialMultiSelectRef = ref(null);

/** 打开物料多选弹窗（复用 material/select.vue） */
function openMaterialDialog() {
  materialMultiSelectRef.value?.open();
}

/** 多选确认：按物料批量生成明细行，规格/单位/默认库位由物料带出 */
async function onMaterialMultiSelect(materials) {
  if (!materials || materials.length === 0) return;
  let added = 0;

  for (const mat of materials) {
    // 同一物料不重复建行
    if (detailList.value.some((d) => d.materialId === mat.materialId)) continue;

    const row = {
      detailId: 0,
      materialId: mat.materialId,
      materialCode: mat.materialCode || "",
      materialName: mat.materialName || "",
      materialPackSpecId: null,
      materialUnit: "",
      spec: mat.spec || "",
      unit: "",
      batchNo: "",
      materialionDate: undefined,
      expireDate: undefined,
      receiptQuantity: 1,
      shelfQuantity: 0,
      detailStatus: "pending",
      locCascader: undefined,
      locationId: undefined,
      supplierId: undefined,
    };

    try {
      const res = await getMaterial(mat.materialId);
      const full = res.data || res;
      row.materialPackSpecId = full.packSpecId || null;
      row.materialUnit = full.unit || "";
      if (full.defaultLocationId) {
        row.locationId = full.defaultLocationId;
        row.locCascader = [full.defaultZoneId, full.defaultLocationId].filter(Boolean);
      }
      await ensurePackSpecLoaded(row.materialPackSpecId);
      // 单位默认取包装规格最底层（通常是最小单位）
      row.unit = unitOptions(row)[0]?.value || full.unit || "";
    } catch (e) {
      /* 物料详情失败不影响添加，用户可后续手工补 */
    }

    detailList.value.push(row);
    added++;
  }

  if (added === 0) {
    ElMessage.info(t('wms.receipt.alreadyInDetail'));
    return;
  }
  onDetailRowClick(detailList.value[detailList.value.length - 1]);
  ElMessage.success(t('wms.receiving.addedDetails', { count: added }));
}

/** 删除明细行 */
function removeDetailRow(index) {
  const removed = detailList.value[index];
  detailList.value.splice(index, 1);
  if (currentDetailRow.value === removed) {
    currentDetailRow.value = detailList.value.length ? detailList.value[0] : null;
    currentDetailIndex.value = currentDetailRow.value ? 0 : -1;
  }
}

/** 打印标签 */
function handlePrintLabel(row) {
  ElMessage.info(t('wms.receipt.printLabelTodo', { no: row.receiptNo }));
}

getList();
</script>

<style scoped>
.detail-section {
  margin-top: 18px;
}
/* 分区头：左侧主色竖条 + 计数徽章 + 右侧扩展区 */
.section-head {
  display: flex;
  align-items: center;
  gap: 8px;
  padding-bottom: 8px;
  margin-bottom: 12px;
  border-bottom: 1px solid #ebeef5;
}
.section-title {
  position: relative;
  padding-left: 10px;
  font-size: 14px;
  font-weight: 600;
  color: #303133;
}
.section-title::before {
  content: "";
  position: absolute;
  left: 0;
  top: 2px;
  bottom: 2px;
  width: 3px;
  border-radius: 2px;
  background: var(--el-color-primary);
}
.section-badge {
  display: inline-flex;
  align-items: center;
  justify-content: center;
  min-width: 18px;
  height: 18px;
  padding: 0 5px;
  border-radius: 9px;
  background: var(--el-color-primary-light-9);
  color: var(--el-color-primary);
  font-size: 12px;
}
.section-extra {
  margin-left: auto;
  font-size: 12px;
  color: #909399;
}
/* 分区头右侧按钮：优先用全局 .detail-add-btn 的尺寸，这里只保留配色与右对齐 */
.section-head > .el-button.section-extra {
  height: 34px;
  padding: 0 16px;
  font-size: 14px;
  color: var(--el-color-primary);
}
/* 左右分栏之间的竖分隔线 */
.pane-divider {
  border-left: 1px solid #ebeef5;
  padding-left: 16px;
}
/* 左：明细列表 */
.detail-list {
  border: 1px solid #ebeef5;
  border-radius: 4px;
  max-height: 440px;
  overflow-y: auto;
}
.detail-list-item {
  display: flex;
  align-items: center;
  gap: 8px;
  padding: 11px 12px;
  border-bottom: 1px solid #f5f7fa;
  cursor: pointer;
}
.detail-list-item:last-child {
  border-bottom: none;
}
.detail-list-item:hover {
  background: #f5f7fa;
}
.detail-list-item.active {
  background: #ecf5ff;
  box-shadow: inset 3px 0 0 var(--el-color-primary);
}
.item-seq {
  flex: none;
  width: 18px;
  height: 18px;
  border-radius: 50%;
  background: #f0f2f5;
  color: #909399;
  font-size: 11px;
  display: flex;
  align-items: center;
  justify-content: center;
}
.detail-list-item.active .item-seq {
  background: var(--el-color-primary);
  color: #fff;
}
.item-title {
  flex: 1;
  min-width: 0;
  display: flex;
  align-items: baseline;
  gap: 6px;
  font-size: 13px;
  font-weight: 600;
}
.item-name {
  flex: 1 1 auto;
  min-width: 0;
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: nowrap;
}
.item-spec {
  flex: 0 0 auto;
  max-width: 40%;
  font-size: 12px;
  font-weight: 400;
  color: #909399;
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: nowrap;
}
.item-qty {
  flex: none;
  font-size: 12px;
  color: #606266;
  font-variant-numeric: tabular-nums;
}
.status-dot {
  flex: none;
  width: 6px;
  height: 6px;
  border-radius: 50%;
  background: #c0c4cc;
}
.status-dot.dot-shelving {
  background: #e6a23c;
}
.status-dot.dot-shelved {
  background: #67c23a;
}
/* 右：Tab 分区 */
.detail-pane {
  padding: 0 4px;
}
.pane-tabs :deep(.el-tabs__header) {
  margin: 0 0 12px;
}
.pane-tabs :deep(.el-tabs__item) {
  font-size: 13px;
}
.tab-badge {
  display: inline-block;
  margin-left: 4px;
  min-width: 16px;
  height: 16px;
  line-height: 16px;
  padding: 0 5px;
  border-radius: 8px;
  background: var(--el-color-primary-light-9);
  color: var(--el-color-primary);
  font-size: 11px;
  text-align: center;
}
.pane-ops {
  display: flex;
  align-items: center;
  gap: 8px;
}
/* 明细标题里的物料编码 */
.material-code {
  margin-right: 2px;
  font-family: Consolas, Monaco, monospace;
  font-size: 13px;
  color: #909399;
}
.pane-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  gap: 8px;
  margin-bottom: 8px;
}
.pane-title {
  font-size: 14px;
  font-weight: 600;
}
.detail-pane :deep(.el-form-item) {
  margin-bottom: 12px;
}
/* 表单标签禁止换行（4 字标签如「入库类型」在窄列下被折行） */
.nowrap-labels :deep(.el-form-item__label) {
  white-space: nowrap;
}
/* 右侧分组标题（纯文本，无折叠交互） */
.detail-group {
  margin-top: 4px;
}
.group-title {
  margin-bottom: 8px;
  font-size: 12px;
  color: #909399;
}
</style>
