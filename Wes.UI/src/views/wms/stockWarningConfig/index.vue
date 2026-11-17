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
          <el-button type="primary" plain icon="Plus" @click="handleAdd" v-hasPermi="['wms:stockWarningConfig:add']">新增</el-button>
        </el-col>
        <el-col :span="1.5">
          <el-button type="success" plain icon="Edit" :disabled="single" @click="handleUpdate" v-hasPermi="['wms:stockWarningConfig:edit']">修改</el-button>
        </el-col>
        <el-col :span="1.5">
          <el-button type="danger" plain icon="Delete" :disabled="multiple" @click="handleDelete" v-hasPermi="['wms:stockWarningConfig:remove']">删除</el-button>
        </el-col>
        <right-toolbar
          v-model:showSearch="showSearch"
          @queryTable="getList"
          hint="一条配置就是一次考核：范围按 仓库 → 库区 → 库位 逐级收窄（只选仓库=全仓，选到库区=按库区，选到库位=按库位），物料不选=范围内全部物料一起考核。
定时任务每天按配置自动检查库存，也可在「库存预警」页面手动立即执行；触发后给处理人发站内信，点击站内信可跳到预警页面处理。"
        />
      </el-row>

      <!-- 列表 -->
      <el-table v-loading="loading" :data="configList" @selection-change="handleSelectionChange">
        <el-table-column type="selection" width="55" align="center" />
        <el-table-column label="仓库" align="center" :show-overflow-tooltip="true">
          <template #default="scope">{{ scope.row.warehouse?.warehouseName || "-" }}</template>
        </el-table-column>
        <el-table-column label="库区" align="center" :show-overflow-tooltip="true">
          <template #default="scope">{{ scope.row.zone?.zoneName || "全仓" }}</template>
        </el-table-column>
        <el-table-column label="库位" align="center" width="110" :show-overflow-tooltip="true">
          <template #default="scope">
            {{ scope.row.location?.locationCode || (scope.row.zoneId ? "整个库区" : "整个仓库") }}
          </template>
        </el-table-column>
        <el-table-column label="物料" align="center" min-width="150" :show-overflow-tooltip="true">
          <template #default="scope">
            <template v-if="scope.row.material">
              {{ scope.row.material.materialCode }} - {{ scope.row.material.materialName }}
            </template>
            <span v-else class="text-muted">全部物料</span>
          </template>
        </el-table-column>
        <el-table-column label="预警类型" align="center" width="120">
          <template #default="scope">
            <el-tag size="small" :type="scope.row.warningType === 'min_stock' ? 'danger' : 'warning'">
              {{ warningTypeMap[scope.row.warningType] || "-" }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column label="阈值" align="center" prop="thresholdValue" width="90" />
        <el-table-column label="级别" align="center" width="80">
          <template #default="scope">
            <el-tag size="small" :type="levelTag(scope.row.warningLevel)">
              {{ levelMap[scope.row.warningLevel] || "-" }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column label="处理人" align="center" min-width="130" :show-overflow-tooltip="true">
          <template #default="scope">{{ scope.row.notifyUserNames || "-" }}</template>
        </el-table-column>
        <el-table-column label="启用" align="center" width="80">
          <template #default="scope">
            <el-tag size="small" :type="scope.row.enabled === 1 ? 'success' : 'info'">
              {{ scope.row.enabled === 1 ? "启用" : "停用" }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column label="备注" align="center" prop="remark" :show-overflow-tooltip="true" />
        <el-table-column label="操作" align="center" width="150" class-name="small-padding fixed-width">
          <template #default="scope">
            <el-button link type="primary" icon="Edit" @click="handleUpdate(scope.row)" v-hasPermi="['wms:stockWarningConfig:edit']">修改</el-button>
            <el-button link type="danger" icon="Delete" @click="handleDelete(scope.row)" v-hasPermi="['wms:stockWarningConfig:remove']">删除</el-button>
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
      <el-dialog :title="title" v-model="open" width="760px" append-to-body>
        <el-form ref="configRef" :model="form" :rules="rules" label-width="88px" class="nowrap-labels">
          <el-row :gutter="16">
            <el-col :span="12">
              <el-form-item label="仓库" prop="warehouseId">
                <WarehouseSelect v-model="form.warehouseId" placeholder="请选择仓库" @change="onWarehouseChange" />
              </el-form-item>
            </el-col>
            <el-col :span="12">
              <el-form-item label="物料" prop="materialId">
                <MaterialSelect
                  v-model="form.materialId"
                  :label="form.materialCode && form.materialName ? form.materialCode + ' - ' + form.materialName : ''"
                  placeholder="不选 = 范围内全部物料"
                  @change="onMaterialChange"
                />
              </el-form-item>
            </el-col>
            <el-col :span="24">
              <el-form-item label="预警范围">
                <LocationCascader
                  v-model="form.locCascader"
                  :warehouse-id="form.warehouseId"
                  :any-level="true"
                  :disabled="!form.warehouseId"
                  :placeholder="form.warehouseId ? '可只选库区，也可选到库位；不选 = 整个仓库' : '请先选择仓库'"
                  @update:model-value="onLocCascader"
                />
              </el-form-item>
            </el-col>
            <el-col :span="12">
              <el-form-item label="预警类型" prop="warningType">
                <el-select v-model="form.warningType" placeholder="请选择预警类型" style="width:100%">
                  <el-option label="低于下限（缺货）" value="min_stock" />
                  <el-option label="高于上限（积压）" value="max_stock" />
                </el-select>
              </el-form-item>
            </el-col>
            <el-col :span="12">
              <el-form-item label="阈值" prop="thresholdValue">
                <el-input-number v-model="form.thresholdValue" :min="0" :precision="3" :controls="false"
                  placeholder="触发预警的数量界限" style="width:100%" />
              </el-form-item>
            </el-col>
            <el-col :span="12">
              <el-form-item label="预警级别" prop="warningLevel">
                <el-select v-model="form.warningLevel" placeholder="请选择预警级别" style="width:100%">
                  <el-option label="一般" value="normal" />
                  <el-option label="紧急" value="urgent" />
                  <el-option label="危机" value="critical" />
                </el-select>
              </el-form-item>
            </el-col>
            <el-col :span="12">
              <el-form-item label="是否启用">
                <el-switch v-model="form.enabled" :active-value="1" :inactive-value="0" />
              </el-form-item>
            </el-col>
            <el-col :span="24">
              <el-form-item label="处理人" prop="notifyUserIds">
                <div class="user-picker">
                  <el-button icon="Plus" @click="openUserSelect">选择人员</el-button>
                  <el-tag
                    v-for="u in form.notifyUsers"
                    :key="u.userId"
                    closable
                    type="info"
                    @close="removeUser(u)"
                    class="user-tag"
                  >{{ u.userName }}</el-tag>
                </div>
              </el-form-item>
            </el-col>
            <el-col :span="24">
              <el-form-item label="备注">
                <el-input v-model="form.remark" placeholder="选填" maxlength="255" />
              </el-form-item>
            </el-col>
          </el-row>

          <div class="form-tip text-muted">
            预警触发后，系统会给这里选中的处理人每人发一条站内信，点击站内信直接跳到「库存预警」页面处理。
          </div>
        </el-form>
        <template #footer>
          <div class="dialog-footer">
            <el-button type="primary" @click="submitForm">确 定</el-button>
            <el-button @click="cancel">取 消</el-button>
          </div>
        </template>
      </el-dialog>

      <SelectUser ref="userSelectRef" :is-single="false" @handle-data="onUsersPicked" />
    </div>
  </div>
</template>

<script setup name="WmsStockWarningConfig">
import { ref, reactive, toRefs, computed } from "vue";
import { ElMessage, ElMessageBox } from "element-plus";
import {
  listStockWarningConfig,
  getStockWarningConfig,
  addStockWarningConfig,
  updateStockWarningConfig,
  delStockWarningConfig,
} from "@/api/wms/stockWarningConfig";
import QueryForm from "@/components/QueryForm/index.vue";
import RightToolbar from "@/components/RightToolbar/index.vue";
import WarehouseSelect from "@/views/wms/warehouse/input.vue";
import MaterialSelect from "@/views/wms/material/input.vue";
import LocationCascader from "@/views/wms/location/cascader.vue";
import SelectUser from "@/views/system/user/select.vue";

const configRef = ref(null);
const userSelectRef = ref(null);
const configList = ref([]);
const open = ref(false);
const loading = ref(true);
const ids = ref([]);
const single = ref(true);
const multiple = ref(true);
const total = ref(0);
const title = ref("");

const showSearch = ref(true);

const warningTypeMap = { min_stock: "低于下限", max_stock: "高于上限" };
const levelMap = { normal: "一般", urgent: "紧急", critical: "危机" };
const levelTag = (l) => (l === "critical" ? "danger" : l === "urgent" ? "warning" : "info");

const data = reactive({
  form: {},
  queryParams: {
    pageNum: 1,
    pageSize: 10,
    keywords: undefined,
    warehouseId: undefined,
    enabled: undefined,
  },
  rules: {
    warehouseId: [{ required: true, message: "请选择仓库", trigger: "change" }],
    warningType: [{ required: true, message: "请选择预警类型", trigger: "change" }],
    thresholdValue: [{ required: true, message: "请输入阈值", trigger: "blur" }],
    warningLevel: [{ required: true, message: "请选择预警级别", trigger: "change" }],
  },
});

const { form, queryParams, rules } = toRefs(data);

const queryConfig = computed(() => [
  { label: "备注", prop: "keywords", type: "input", placeholder: "请输入备注关键字" },
  {
    label: "启用状态",
    prop: "enabled",
    type: "select",
    placeholder: "请选择启用状态",
    options: [
      { label: "启用", value: 1 },
      { label: "停用", value: 0 },
    ],
  },
]);

/** 查询列表 */
function getList() {
  loading.value = true;
  listStockWarningConfig(queryParams.value).then((res) => {
    configList.value = res.rows || [];
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
  queryParams.value = { keywords: undefined, warehouseId: undefined, enabled: undefined, pageNum: 1, pageSize: 10 };
  handleQuery();
}

function handleSelectionChange(selection) {
  ids.value = selection.map((item) => item.configId);
  single.value = selection.length !== 1;
  multiple.value = !selection.length;
}

/** 表单重置 */
function reset() {
  form.value = {
    configId: undefined,
    warehouseId: undefined,
    zoneId: undefined,
    locationId: undefined,
    locCascader: [],
    materialId: undefined,
    materialCode: "",
    materialName: "",
    warningType: "min_stock",
    thresholdValue: undefined,
    warningLevel: "normal",
    notifyUserIds: "",
    notifyUserNames: "",
    notifyUsers: [],
    enabled: 1,
    remark: undefined,
  };
  if (configRef.value) configRef.value.resetFields();
}

function handleAdd() {
  reset();
  open.value = true;
  title.value = "添加预警配置";
}

function handleUpdate(row) {
  reset();
  const id = row?.configId || ids.value[0];
  getStockWarningConfig(id).then((res) => fillForm(res.data || {}));
}

/** 填充表单：id 串 + 姓名串 zip 回人员标签 */
function fillForm(d) {
  const idList = (d.notifyUserIds || "").split(",").map((s) => s.trim()).filter(Boolean);
  const nameList = (d.notifyUserNames || "").split("、").filter(Boolean);
  form.value = {
    configId: d.configId,
    warehouseId: d.warehouseId,
    zoneId: d.zoneId,
    locationId: d.locationId,
    locCascader: d.zoneId ? [d.zoneId, d.locationId].filter(Boolean) : [],
    materialId: d.materialId,
    materialCode: d.material?.materialCode || "",
    materialName: d.material?.materialName || "",
    warningType: d.warningType,
    thresholdValue: d.thresholdValue,
    warningLevel: d.warningLevel || "normal",
    notifyUserIds: d.notifyUserIds || "",
    notifyUserNames: d.notifyUserNames || "",
    notifyUsers: idList.map((id, i) => ({ userId: id, userName: nameList[i] || `用户${id}` })),
    enabled: d.enabled ?? 1,
    remark: d.remark,
  };
  open.value = true;
  title.value = "修改预警配置";
}

/** 仓库变更：清空范围 */
function onWarehouseChange() {
  form.value.locCascader = [];
  form.value.zoneId = undefined;
  form.value.locationId = undefined;
}

/** 范围选择：checkStrictly 模式下可能只选到库区 */
function onLocCascader(val) {
  form.value.zoneId = val?.[0];
  form.value.locationId = val?.length > 1 ? val[1] : undefined;
}

/** 物料选择 */
function onMaterialChange(material) {
  if (!material) {
    form.value.materialId = undefined;
    form.value.materialCode = "";
    form.value.materialName = "";
    return;
  }
  form.value.materialCode = material.materialCode || "";
  form.value.materialName = material.materialName || "";
}

/** 处理人多选 */
function openUserSelect() {
  userSelectRef.value.open();
}

function onUsersPicked(users) {
  const merged = [...(form.value.notifyUsers || [])];
  (users || []).forEach((u) => {
    if (!merged.some((p) => p.userId == u.userId)) {
      merged.push({ userId: u.userId, userName: u.userName || u.account });
    }
  });
  form.value.notifyUsers = merged;
}

function removeUser(u) {
  form.value.notifyUsers = (form.value.notifyUsers || []).filter((p) => p.userId != u.userId);
}

/** 提交 */
function submitForm() {
  configRef.value.validate((valid) => {
    if (!valid) return;
    if (!(form.value.thresholdValue > 0)) {
      ElMessage.warning("阈值必须大于 0");
      return;
    }
    if (!form.value.notifyUsers || form.value.notifyUsers.length === 0) {
      ElMessage.warning("请至少选择一名处理人");
      return;
    }

    const payload = {
      configId: form.value.configId,
      warehouseId: form.value.warehouseId,
      zoneId: form.value.zoneId,
      locationId: form.value.locationId,
      materialId: form.value.materialId || undefined,
      warningType: form.value.warningType,
      thresholdValue: form.value.thresholdValue,
      warningLevel: form.value.warningLevel,
      notifyUserIds: form.value.notifyUsers.map((u) => u.userId).join(","),
      enabled: form.value.enabled,
      remark: form.value.remark,
    };

    const req = form.value.configId ? updateStockWarningConfig(payload) : addStockWarningConfig(payload);
    req.then((res) => {
      if (res.code === 200) {
        ElMessage.success("保存成功");
        open.value = false;
        getList();
      } else {
        ElMessage.error(res.msg || "保存失败");
      }
    });
  });
}

function cancel() {
  open.value = false;
  reset();
}

function handleDelete(row) {
  const delIds = row?.configId ? [row.configId] : ids.value;
  if (!delIds || delIds.length === 0) return;
  ElMessageBox.confirm(`确认删除选中的 ${delIds.length} 条预警配置吗？`, "提示", { type: "warning" })
    .then(() => delStockWarningConfig(delIds.join(",")))
    .then((res) => {
      if (res.code === 200) {
        ElMessage.success("删除成功");
        getList();
      } else {
        ElMessage.error(res.msg || "删除失败");
      }
    })
    .catch(() => {});
}

getList();
</script>

<style scoped>
.user-picker {
  display: flex;
  align-items: center;
  flex-wrap: wrap;
  gap: 6px;
}
</style>
