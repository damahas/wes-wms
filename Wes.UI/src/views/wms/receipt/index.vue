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
        <el-button type="primary" plain icon="Plus" @click="handleAdd" v-hasPermi="['wms:receipt:add']">新增</el-button>
      </el-col>
      <el-col :span="1.5">
        <el-button v-if="canEdit(selectedRows[0] || {})" type="success" plain icon="Edit" :disabled="single" @click="handleUpdate" v-hasPermi="['wms:receipt:edit']">修改</el-button>
        <el-button v-else type="info" plain icon="View" :disabled="single" @click="handleView(selectedRows[0])">明细</el-button>
      </el-col>
      <el-col :span="1.5">
        <el-button type="danger" plain icon="Delete" :disabled="multiple || selectedRows.some((r) => !canEdit(r))" @click="handleDelete" v-hasPermi="['wms:receipt:remove']">删除</el-button>
      </el-col>
        <right-toolbar
          v-model:showSearch="showSearch"
          @queryTable="getList"
        ></right-toolbar>
    </el-row>

    <!-- 列表 -->
    <el-table v-loading="loading" :data="receiptList" @selection-change="handleSelectionChange">
      <el-table-column type="selection" width="55" align="center" />
      <el-table-column label="入库单号" align="center" prop="receiptNo" :show-overflow-tooltip="true" />
      <el-table-column label="入库类型" align="center" :show-overflow-tooltip="true">
        <template #default="scope">
          {{ receiptTypeMap[scope.row.receiptType] || "-" }}
        </template>
      </el-table-column>
      <el-table-column label="仓库" align="center" :show-overflow-tooltip="true">
        <template #default="scope">
          {{ scope.row.warehouse?.warehouseName || "-" }}
        </template>
      </el-table-column>
      <el-table-column label="总数量" align="center" prop="totalQuantity" />
      <el-table-column label="状态" align="center" width="100">
        <template #default="scope">
          <el-tag :type="receiptStatusTag(scope.row.receiptStatus)">
            {{ receiptStatusMap[scope.row.receiptStatus] || "-" }}
          </el-tag>
        </template>
      </el-table-column>
      <el-table-column label="创建时间" align="center" prop="createTime" min-width="170" />
      <el-table-column label="操作" align="center" width="220" class-name="small-padding fixed-width">
        <template #default="scope">
          <el-button link type="warning" icon="Printer" @click="handlePrintLabel(scope.row)">标签</el-button>
          <el-button v-if="canEdit(scope.row)" link type="primary" icon="Edit" @click="handleUpdate(scope.row)" v-hasPermi="['wms:receipt:edit']">修改</el-button>
          <el-button v-else link type="info" icon="View" @click="handleView(scope.row)">明细</el-button>
          <el-button v-if="canEdit(scope.row)" link type="danger" icon="Delete" @click="handleDelete(scope.row)" v-hasPermi="['wms:receipt:remove']">删除</el-button>
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
          <span class="section-title">基本信息</span>
          <span class="section-extra">
            {{ form.receiptNo ? "单号 " + form.receiptNo : "单号保存后自动生成" }}
          </span>
        </div>
        <el-row :gutter="16">
          <el-col :span="6">
            <el-form-item label="入库类型" prop="receiptType">
              <el-select v-model="form.receiptType" placeholder="请选择入库类型" clearable style="width:100%" :disabled="!!form.receiptId" @change="onReceiptTypeChange">
                <el-option label="采购入库" value="purchase" />
                <el-option label="生产入库" value="produce" />
                <el-option label="退货入库" value="return_in" />
                <el-option label="其他入库" value="other" />
              </el-select>
            </el-form-item>
          </el-col>
          <el-col :span="6">
            <el-form-item label="仓库" prop="warehouseId">
              <WarehouseSelect v-model="form.warehouseId" placeholder="请选择仓库" :disabled="viewMode" @change="onWarehouseChange" />
            </el-form-item>
          </el-col>
          <el-col :span="6">
            <el-form-item label="入库日期" prop="receiptDate">
              <el-date-picker v-model="form.receiptDate" type="date" value-format="YYYY-MM-DD"
                placeholder="请选择入库日期" style="width:100%" />
            </el-form-item>
          </el-col>
          <el-col :span="6">
            <el-form-item v-if="form.receiptType === 'purchase'" label="收货单" prop="sourceId">
              <ReceivingSelect v-model="form.sourceId" placeholder="请选择收货单" :disabled="!!form.receiptId" @change="onReceivingChange" />
            </el-form-item>
            <el-form-item v-else-if="form.receiptType === 'produce'" label="生产单号" prop="sourceId">
              <el-input v-model="form.sourceId" placeholder="请输入生产单号" maxlength="50" />
            </el-form-item>
            <el-form-item v-else-if="form.receiptType === 'return_in'" label="退货单号" prop="sourceId">
              <el-input v-model="form.sourceId" placeholder="请输入退货单号" maxlength="50" />
            </el-form-item>
            <el-form-item v-else-if="form.receiptType === 'other'" label="来源单号" prop="sourceId">
              <el-input v-model="form.sourceId" placeholder="请输入来源单号" maxlength="50" />
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="16">
          <el-col :span="24">
            <el-form-item label="备注" prop="remark">
              <el-input v-model="form.remark" type="textarea" :autosize="{ minRows: 1, maxRows: 3 }"
                placeholder="请输入备注（选填）" maxlength="500" />
            </el-form-item>
          </el-col>
        </el-row>
      </el-form>

      <!-- 入库明细 -->
      <div class="detail-section">
        <!-- 入库明细 -->
        <div class="section-head">
          <span class="section-title">入库明细</span>
          <span class="section-badge">{{ detailList.length }}</span>
          <el-button class="section-extra detail-add-btn" type="primary" plain icon="Plus"
            @click="openMaterialDialog" v-if="!form.receiptId && !viewMode">添加明细</el-button>
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
                        : "（未选择物料）"
                    }}
                  </span>
                  <span v-if="row.spec" class="item-spec">{{ row.spec }}</span>
                </span>
                <span class="item-qty">{{ row.receiptQuantity || 0 }}{{ row.unit || "" }}</span>
                <span class="status-dot" :class="'dot-' + (row.detailStatus || 'pending')" />
              </div>
              <el-empty v-if="!detailList.length" description="暂无明细" :image-size="60" />
            </div>
          </el-col>

          <!-- 右：Tab 分区，一屏只显示一块 -->
          <el-col :span="17" class="pane-divider">
            <div v-if="currentDetailRow" class="detail-pane">
              <el-tabs v-model="detailTab" class="pane-tabs">
                <!-- 明细信息 -->
                <el-tab-pane name="info">
                  <template #label>明细信息</template>
                  <div class="pane-header">
                    <span class="pane-title">
                      明细 #{{ currentDetailIndex + 1 }}：
                      <span v-if="currentDetailRow.materialCode" class="material-code">{{ currentDetailRow.materialCode }}</span>
                      {{ currentDetailRow.materialName || "未选择物料" }}
                      <span v-if="currentDetailRow.spec" class="text-muted">（{{ currentDetailRow.spec }}）</span>
                    </span>
                    <span class="pane-ops">
                      <el-tag v-if="currentDetailRow.detailStatus === 'shelved'" type="success" size="small">
                        已上架，只读
                      </el-tag>
                      <el-button
                        v-if="!viewMode && currentDetailRow.detailId && currentDetailRow.detailStatus !== 'shelved'"
                        size="small" type="primary" @click="openShelve(currentDetailRow)"
                      >上架</el-button>
                      <el-button
                        v-if="!form.receiptId && !viewMode" size="small" type="danger" plain icon="Delete"
                        @click="removeDetailRow(currentDetailIndex)"
                      >删除</el-button>
                    </span>
                  </div>
                  <el-form :model="currentDetailRow" label-width="84px" class="nowrap-labels" :disabled="viewMode || currentDetailRow.detailStatus === 'shelved'">
                    <el-row :gutter="16">
                      <el-col :span="12">
                        <el-form-item label="数量">
                          <el-input-number controls-position="right" v-model="currentDetailRow.receiptQuantity" :min="0" style="width:100%" />
                        </el-form-item>
                      </el-col>
                      <el-col :span="12">
                        <el-form-item label="单位">
                          <el-select v-model="currentDetailRow.unit" placeholder="单位" filterable style="width:100%">
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
                        <el-form-item label="库位">
                          <!-- 仓库由单据固定，这里只选库区/库位 -->
                          <LocationCascader
                            v-model="currentDetailRow.locCascader"
                            :warehouse-id="form.warehouseId"
                            :disabled="viewMode || !form.warehouseId"
                            :placeholder="form.warehouseId ? '请选择库区/库位' : '请先选择仓库'"
                            @update:model-value="(val) => (currentDetailRow.locationId = val?.[1])"
                          />
                        </el-form-item>
                      </el-col>
                    </el-row>
                    <!-- 批次 / 日期 / 供应商：纯分组标题，无折叠交互（避免误点） -->
                    <div class="detail-group">
                      <div class="group-title">批次 / 日期 / 供应商</div>
                      <el-row :gutter="16">
                        <el-col :span="12">
                          <el-form-item label="批次号">
                            <el-input v-model="currentDetailRow.batchNo" placeholder="批次号" />
                          </el-form-item>
                        </el-col>
                        <el-col :span="12">
                          <el-form-item label="供应商">
                            <PartnerSelect v-model="currentDetailRow.supplierId" partner-type="supplier" placeholder="请选择供应商" :disabled="viewMode" />
                          </el-form-item>
                        </el-col>
                        <el-col :span="12">
                          <el-form-item label="生产日期">
                            <el-date-picker v-model="currentDetailRow.materialionDate" type="date" value-format="YYYY-MM-DD"
                              placeholder="生产日期" style="width:100%" />
                          </el-form-item>
                        </el-col>
                        <el-col :span="12">
                          <el-form-item label="失效日期">
                            <el-date-picker v-model="currentDetailRow.expireDate" type="date" value-format="YYYY-MM-DD"
                              placeholder="失效日期" style="width:100%" />
                          </el-form-item>
                        </el-col>
                      </el-row>
                    </div>
                  </el-form>
                </el-tab-pane>

                <!-- 条码 -->
                <el-tab-pane name="barcode">
                  <template #label>
                    条码<span class="tab-badge">{{ barcodeList.length }}</span>
                  </template>
                  <template v-if="currentDetailRow.detailId">
                    <div class="pane-header">
                      <span class="text-muted">
                        合计 {{ barcodeTotal }} / 明细数量 {{ currentDetailQty
                        }}<template v-if="!viewMode">　拆分：均分填份数（3），不等分填 10,5</template>
                      </span>
                      <el-button v-if="!viewMode" size="small" type="primary" :disabled="selectedBarcodes.length < 2" @click="doMergeBarcodes">
                        合并所选
                      </el-button>
                    </div>
                    <el-table :data="barcodeList" border max-height="400" @selection-change="(v) => (selectedBarcodes = v)">
                      <el-table-column v-if="!viewMode" type="selection" width="45" :selectable="(row) => row.status !== 'shelved'" />
                      <el-table-column label="序号" prop="seqNo" width="60" align="center" />
                      <el-table-column label="条码" prop="barcode" min-width="160" />
                      <el-table-column label="数量" prop="quantity" width="80" align="center" />
                      <el-table-column label="状态" width="90" align="center">
                        <template #default="scope">
                          <el-tag :type="scope.row.status === 'shelved' ? 'success' : 'info'" size="small">
                            {{ scope.row.status === "shelved" ? "已上架" : "待上架" }}
                          </el-tag>
                        </template>
                      </el-table-column>
                      <el-table-column v-if="!viewMode" label="操作" width="80" align="center">
                        <template #default="scope">
                          <el-button link type="primary" :disabled="scope.row.status === 'shelved'"
                            @click="splitBarcodeRow(scope.row)">拆分</el-button>
                        </template>
                      </el-table-column>
                    </el-table>
                  </template>
                  <el-empty v-else description="保存入库单后自动生成条码" :image-size="60" />
                </el-tab-pane>
              </el-tabs>
            </div>
            <el-empty v-else :description="viewMode ? '在左侧点击一条明细查看' : '在左侧点击一条明细进行编辑'" :image-size="60" />
          </el-col>
        </el-row>
      </div>

      <template #footer>
        <div class="dialog-footer">
          <el-button v-if="!viewMode" type="primary" @click="submitForm">确 定</el-button>
          <el-button @click="cancel">{{ viewMode ? "关 闭" : "取 消" }}</el-button>
        </div>
      </template>
    </el-dialog>

    <!-- 明细上架 -->
    <el-dialog title="明细上架" v-model="shelveOpen" width="460px" append-to-body>
      <el-form :model="shelveForm" label-width="90px">
        <el-form-item label="待上架条码">
          <el-input :model-value="`${shelveForm.pendingBarcodeCount || 0} 个（按序号依次上架）`" readonly />
        </el-form-item>
        <el-form-item label="物料">
          <el-input :model-value="shelveForm.materialLabel" readonly />
        </el-form-item>
        <el-form-item label="待上架数量">
          <el-input :model-value="String(shelveForm.remainQty)" readonly />
        </el-form-item>
        <el-form-item label="上架条码数">
          <el-input-number v-model="shelveForm.quantity" :min="1" :max="shelveForm.pendingBarcodeCount || shelveForm.remainQty || 1"
            controls-position="right" style="width:100%" />
        </el-form-item>
        <el-form-item label="上架库位">
          <!-- 仓库由单据固定，只选库区/库位 -->
          <LocationCascader
            v-model="shelveForm.locCascader"
            :warehouse-id="form.warehouseId"
            @update:model-value="(val) => (shelveForm.locationId = val?.[1])"
          />
        </el-form-item>
      </el-form>
      <template #footer>
        <el-button type="primary" @click="submitShelve">确 定</el-button>
        <el-button @click="shelveOpen = false">取 消</el-button>
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

const receiptTypeMap = {
  purchase: "采购入库",
  produce: "生产入库",
  return_in: "退货入库",
  other: "其他入库",
};

// 入库单状态：待入库 → 上架中（部分明细已上架）→ 已入库（全部明细上架完成）
const receiptStatusMap = { pending_in: "待入库", shelfing: "上架中", in_stock: "已入库" };
const receiptStatusTag = (s) => (s === "in_stock" ? "success" : s === "shelfing" ? "warning" : "info");

// 已开始上架（上架中/已入库）的单据不能再修改或删除：
// 编辑会重建明细与条码，已上架的库存会失去来源记录
function canEdit(row) {
  return row.receiptStatus !== "in_stock" && row.receiptStatus !== "shelfing";
}

// 明细上架状态
const detailStatusMap = { pending: "待上架", shelving: "上架中", shelved: "已上架" };

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
  parts.push(detailStatusMap[row.detailStatus || "pending"]);
  parts.push(`已上架 ${row.shelfQuantity || 0}/${row.receiptQuantity || 0}`);
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
    receiptType: [{ required: true, message: "请选择入库类型", trigger: "change" }],
    warehouseId: [{ required: true, message: "请选择仓库", trigger: "change" }],
    receiptDate: [{ required: true, message: "请选择入库日期", trigger: "change" }],
    sourceId: [
      {
        // 采购入库必须关联收货单，其余类型来源单号可留空
        validator: (rule, value, callback) => {
          if (form.value.receiptType === "purchase" && !value) callback(new Error("请选择收货单"));
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
    label: "关键字",
    prop: "keywords",
    type: "input",
    placeholder: "请输入关键字",
  },
  {
    label: "状态",
    prop: "receiptStatus",
    type: "select",
    placeholder: "请选择状态",
    options: [
      { label: "待入库", value: "pending_in" },
      { label: "上架中", value: "shelfing" },
      { label: "已入库", value: "in_stock" },
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
  title.value = "添加入库单";
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
  title.value = viewMode.value ? "入库单明细" : "修改入库单";
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
      `原数量 ${row.quantity}。均分请填份数（如 3），不等分请填逗号分隔的数量（如 10,5）`,
      "拆分条码",
      { inputPattern: /^[\d.\s,]+$/, inputErrorMessage: "只能填数字，或逗号分隔的数字" }
    );
    const input = (value || "").trim();
    let quantities = [];
    if (input.includes(",")) {
      quantities = input.split(",").map((s) => Number(s.trim())).filter((n) => !isNaN(n) && n > 0);
    } else {
      const n = Number(input);
      if (!Number.isInteger(n) || n < 2) {
        ElMessage.warning("均分份数必须是大于 1 的整数");
        return;
      }
      const base = Math.floor((row.quantity || 0) / n);
      quantities = Array.from({ length: n }, (_, i) =>
        i === n - 1 ? Number(row.quantity) - base * (n - 1) : base
      );
    }
    if (quantities.length < 2) {
      ElMessage.warning("至少拆成 2 个条码");
      return;
    }
    await splitReceiptBarcode(form.value.receiptId, currentDetailRow.value.detailId, {
      barcodeId: row.barcodeId,
      quantities,
    });
    ElMessage.success("拆分成功");
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
    ElMessage.success("合并成功");
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
    ElMessage.warning("该明细已全部上架");
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
    ElMessage.warning("请选择上架库位");
    return;
  }
  if (!shelveForm.quantity || shelveForm.quantity <= 0) {
    ElMessage.warning("上架数量必须大于0");
    return;
  }
  const receiptId = shelveForm.receiptId;
  shelveReceiptDetail(receiptId, shelveForm.detailId, {
    locationId: shelveForm.locationId,
    quantity: shelveForm.quantity,
  }).then(() => {
    ElMessage.success("上架成功");
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
      ElMessage.warning("请至少添加一条入库明细");
      return;
    }
    for (let i = 0; i < detailList.value.length; i++) {
      const d = detailList.value[i];
      if (!d.materialId) {
        ElMessage.warning(`第 ${i + 1} 行：请选择物料`);
        return;
      }
      if (!d.receiptQuantity || d.receiptQuantity <= 0) {
        ElMessage.warning(`第 ${i + 1} 行：入库数量必须大于0`);
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
  const delIds = row.receiptId || ids.value.join(",");
  ElMessageBox.confirm(`是否确认删除编号为"${delIds}"的数据项？`, "警告", {
    confirmButtonText: "确定",
    cancelButtonText: "取消",
    type: "warning",
  })
    .then(() => delReceipt(delIds))
    .then(() => {
      getList();
      ElMessage.success("删除成功");
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
    ElMessage.error("加载收货单明细失败，请检查后端服务是否已重新编译！");
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
          label += "（基本单位）";
        } else if (!p.parentId) {
          label += "（顶层）";
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
    ElMessage.info("所选物料已在明细中");
    return;
  }
  onDetailRowClick(detailList.value[detailList.value.length - 1]);
  ElMessage.success(`已添加 ${added} 条明细`);
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
  ElMessage.info(`TODO：打印标签功能待实现，入库单号：${row.receiptNo}`);
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
