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
          <el-button type="primary" plain icon="Plus" @click="handleAdd" v-hasPermi="['wms:move:add']">新增</el-button>
        </el-col>
        <el-col :span="1.5">
          <el-button v-if="canEdit(selectedRows[0] || {})" type="success" plain icon="Edit" :disabled="single" @click="handleUpdate" v-hasPermi="['wms:move:edit']">修改</el-button>
          <el-button v-else type="info" plain icon="View" :disabled="single" @click="handleView(selectedRows[0])">明细</el-button>
        </el-col>
        <el-col :span="1.5">
          <el-button type="danger" plain icon="Delete" :disabled="multiple || selectedRows.some((r) => !canEdit(r))" @click="handleDelete" v-hasPermi="['wms:move:remove']">删除</el-button>
        </el-col>
        <right-toolbar
          v-model:showSearch="showSearch"
          @queryTable="getList"
        ></right-toolbar>
      </el-row>

      <!-- 列表 -->
      <el-table v-loading="loading" :data="moveList" @selection-change="handleSelectionChange">
        <el-table-column type="selection" width="55" align="center" />
        <el-table-column label="移库单号" align="center" prop="moveNo" :show-overflow-tooltip="true" />
        <el-table-column label="移库类型" align="center" width="110" :show-overflow-tooltip="true">
          <template #default="scope">
            {{ moveTypeMap[scope.row.moveType] || "-" }}
          </template>
        </el-table-column>
        <el-table-column label="仓库" align="center" :show-overflow-tooltip="true">
          <template #default="scope">
            {{ scope.row.warehouse?.warehouseName || "-" }}
          </template>
        </el-table-column>
        <el-table-column label="总数量" align="center" prop="totalQuantity" width="90" />
        <el-table-column label="状态" align="center" width="100">
          <template #default="scope">
            <el-tag :type="moveStatusTag(scope.row.moveStatus)">
              {{ moveStatusMap[scope.row.moveStatus] || "-" }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column label="创建时间" align="center" prop="createTime" min-width="170" />
        <el-table-column label="操作" align="center" width="220" class-name="small-padding fixed-width">
          <template #default="scope">
            <el-button v-if="canEdit(scope.row)" link type="success" icon="Promotion" @click="handleExecute(scope.row)" v-hasPermi="['wms:move:edit']">执行</el-button>
            <el-button v-if="canEdit(scope.row)" link type="primary" icon="Edit" @click="handleUpdate(scope.row)" v-hasPermi="['wms:move:edit']">修改</el-button>
            <el-button v-else link type="info" icon="View" @click="handleView(scope.row)">明细</el-button>
            <el-button v-if="canEdit(scope.row)" link type="danger" icon="Delete" @click="handleDelete(scope.row)" v-hasPermi="['wms:move:remove']">删除</el-button>
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
              <el-form-item label="移库单号">
                <el-input v-model="form.moveNo" placeholder="保存后自动生成" readonly />
              </el-form-item>
            </el-col>
            <el-col :span="8">
              <el-form-item label="移库类型" prop="moveType">
                <el-select v-model="form.moveType" placeholder="请选择移库类型" style="width:100%">
                  <el-option v-for="(label, value) in moveTypeMap" :key="value" :label="label" :value="value" />
                </el-select>
              </el-form-item>
            </el-col>
            <el-col :span="8">
              <el-form-item label="仓库" prop="warehouseId">
                <WarehouseSelect v-model="form.warehouseId" placeholder="请选择仓库" :disabled="viewMode" @change="onWarehouseChange" />
              </el-form-item>
            </el-col>
            <el-col :span="24">
              <el-form-item label="移库原因" prop="moveReason">
                <el-input v-model="form.moveReason" type="textarea" :autosize="{ minRows: 1, maxRows: 3 }"
                  placeholder="请说明移库原因（选填）" maxlength="255" />
              </el-form-item>
            </el-col>
          </el-row>

          <!-- 移库明细：库存行一律从弹窗多选进来，行内不可再换，要改就删行重挑 -->
          <div class="detail-head">
            <span class="detail-title">移库明细</span>
            <el-button v-if="!viewMode" class="detail-add-btn" type="primary" plain icon="Plus" @click="openStockDialog">添加明细</el-button>
            <SelectStock
              ref="stockSelectRef"
              :is-single="false"
              :warehouse-id="form.warehouseId"
              @handle-data="onStockSelected"
              v-show="false"
            />
          </div>

          <el-table :data="details" border size="small" empty-text="请点击「添加明细」从本仓的库存中挑选">
            <el-table-column label="库存（物料 / 批次 / 移出库位）" min-width="300">
              <template #default="scope">
                <span>{{ scope.row.stockLabel }}</span>
              </template>
            </el-table-column>
            <el-table-column label="移入库位" width="200">
              <template #default="scope">
                <LocationCascader
                  v-model="scope.row.toLocCascader"
                  :warehouse-id="form.warehouseId"
                  :disabled="!form.warehouseId || viewMode"
                  :placeholder="form.warehouseId ? '库区/库位' : '请先选仓库'"
                  @update:model-value="(val) => onToLocation(val, scope.$index)"
                />
              </template>
            </el-table-column>
            <el-table-column label="数量" width="110">
              <template #default="scope">
                <span title="一条库存一个条码，数量跟随库存行，改了就跟标签对不上">
                  {{ scope.row.moveQuantity }}
                </span>
              </template>
            </el-table-column>
            <el-table-column label="操作" width="70" align="center">
              <template #default="scope">
                <el-button v-if="!viewMode" link type="danger" icon="Delete" @click="removeDetail(scope.$index)" />
              </template>
            </el-table-column>
          </el-table>

          <div class="detail-tip text-muted">
            直接挑本仓的库存行（默认只显示可用的：扣掉冻结与锁定），移出库位与数量都由库存带出、不可改（一条库存一个条码，拆开会跟标签对不上）；移入库位必须在同一仓库且不能相同，跨仓请用调拨单。
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

<script setup name="WmsMove">
import { ref, reactive, toRefs, computed } from "vue";
import { ElMessage, ElMessageBox } from "element-plus";
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
const moveTypeMap = {
  location_adjust: "库位调整",
  zone_adjust: "库区调整",
  replenish: "补货移库",
  check_adjust: "盘点调整",
  other: "其他",
};

// 移库状态：待移库 → 已完成
const moveStatusMap = { pending: "待移库", completed: "已完成" };
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
    moveType: [{ required: true, message: "请选择移库类型", trigger: "change" }],
    warehouseId: [{ required: true, message: "请选择仓库", trigger: "change" }],
  },
});

const { form, queryParams, rules } = toRefs(data);

const queryConfig = computed(() => [
  {
    label: "关键字",
    prop: "keywords",
    type: "input",
    placeholder: "请输入移库单号",
  },
  {
    label: "状态",
    prop: "moveStatus",
    type: "select",
    placeholder: "请选择状态",
    options: [
      { label: "待移库", value: "pending" },
      { label: "已完成", value: "completed" },
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
  title.value = "添加移库单";
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
    stockLabel: [d.materialCode, d.materialName, d.batchNo ? "批次" + d.batchNo : "无批次", d.fromLocationCode]
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
  title.value = viewMode.value ? "移库单明细" : "修改移库单";
}

/**
 * 打开库存多选弹窗。
 * 原来点「添加明细」只是插一行空的，空行里仓库还没选、库存也点不动，等于先把人卡住；
 * 改成弹窗里一次挑多行，选完直接成型。
 */
function openStockDialog() {
  if (!form.value.warehouseId) {
    ElMessage.warning("请先选择仓库，再挑选要搬的库存");
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
      stockLabel: `${stock.materialCode || ""} ${stock.materialName || ""} / ${stock.batchNo || "无批次"} / ${stock.locationCode || ""}`,
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
  if (skipped > 0) ElMessage.info(`有 ${skipped} 行库存已在明细中，已跳过`);
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
  ElMessage.warning("仓库已变更，明细已清空，请重新挑选库存");
}

/** 提交 */
function submitForm() {
  moveRef.value.validate((valid) => {
    if (!valid) return;
    if (!details.value.length) {
      ElMessage.warning("请至少添加一条移库明细");
      return;
    }
    for (let i = 0; i < details.value.length; i++) {
      const d = details.value[i];
      if (!d.materialId) {
        ElMessage.warning(`第 ${i + 1} 行请选择物料`);
        return;
      }
      if (!d.fromLocationId) {
        ElMessage.warning(`第 ${i + 1} 行请选择移出库位`);
        return;
      }
      if (!d.toLocationId) {
        ElMessage.warning(`第 ${i + 1} 行请选择移入库位`);
        return;
      }
      if (d.fromLocationId === d.toLocationId) {
        ElMessage.warning(`第 ${i + 1} 行移出库位与移入库位不能相同`);
        return;
      }
      if (!(d.moveQuantity > 0)) {
        ElMessage.warning(`第 ${i + 1} 行移库数量必须大于 0`);
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

/** 执行移库 */
function handleExecute(row) {
  ElMessageBox.confirm(
    `执行移库单「${row.moveNo}」将立即从移出库位扣减、向移入库位增加，并写入库存流水，执行后不可修改和删除。是否继续？`,
    "执行确认",
    { confirmButtonText: "确定执行", cancelButtonText: "取消", type: "warning" }
  )
    .then(() => executeMove(row.moveId))
    .then(() => {
      ElMessage.success("执行成功，库存已更新");
      getList();
    });
}

/** 删除 */
function handleDelete(row) {
  const delIds = row.moveId || ids.value.join(",");
  ElMessageBox.confirm(`是否确认删除移库单编号为"${delIds}"的数据项？`, "警告", {
    confirmButtonText: "确定",
    cancelButtonText: "取消",
    type: "warning",
  })
    .then(() => delMove(delIds))
    .then(() => {
      getList();
      ElMessage.success("删除成功");
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
