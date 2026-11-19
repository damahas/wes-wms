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
          <el-button type="primary" plain icon="Plus" @click="handleAdd" v-hasPermi="['wms:stockWarningConfig:add']">{{ t('common.add') }}</el-button>
        </el-col>
        <el-col :span="1.5">
          <el-button type="success" plain icon="Edit" :disabled="single" @click="handleUpdate" v-hasPermi="['wms:stockWarningConfig:edit']">{{ t('common.edit') }}</el-button>
        </el-col>
        <el-col :span="1.5">
          <el-button type="danger" plain icon="Delete" :disabled="multiple" @click="handleDelete" v-hasPermi="['wms:stockWarningConfig:remove']">{{ t('common.delete') }}</el-button>
        </el-col>
        <right-toolbar
          v-model:showSearch="showSearch"
          @queryTable="getList"
          :hint="t('wms.stockWarningConfig.hint')"
        />
      </el-row>

      <!-- 列表 -->
      <el-table v-loading="loading" :data="configList" @selection-change="handleSelectionChange">
        <el-table-column type="selection" width="55" align="center" />
        <el-table-column :label="t('wms.stockWarningConfig.warehouse')" align="center" :show-overflow-tooltip="true">
          <template #default="scope">{{ scope.row.warehouse?.warehouseName || "-" }}</template>
        </el-table-column>
        <el-table-column :label="t('wms.stockWarningConfig.zone')" align="center" :show-overflow-tooltip="true">
          <template #default="scope">{{ scope.row.zone?.zoneName || t('wms.stockWarningConfig.wholeWarehouse') }}</template>
        </el-table-column>
        <el-table-column :label="t('wms.stockWarningConfig.location')" align="center" width="110" :show-overflow-tooltip="true">
          <template #default="scope">
            {{ scope.row.location?.locationCode || (scope.row.zoneId ? t('wms.stockWarningConfig.wholeZone') : t('wms.stockWarningConfig.wholeWarehouseScope')) }}
          </template>
        </el-table-column>
        <el-table-column :label="t('wms.stockWarningConfig.material')" align="center" min-width="150" :show-overflow-tooltip="true">
          <template #default="scope">
            <template v-if="scope.row.material">
              {{ scope.row.material.materialCode }} - {{ scope.row.material.materialName }}
            </template>
            <span v-else class="text-muted">{{ t('wms.stockWarningConfig.allMaterials') }}</span>
          </template>
        </el-table-column>
        <el-table-column :label="t('wms.stockWarningConfig.warningType')" align="center" width="120">
          <template #default="scope">
            <el-tag size="small" :type="scope.row.warningType === 'min_stock' ? 'danger' : 'warning'">
              {{ warningTypeMap[scope.row.warningType] || "-" }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column :label="t('wms.stockWarningConfig.thresholdValue')" align="center" prop="thresholdValue" width="90" />
        <el-table-column :label="t('wms.stockWarningConfig.warningLevel')" align="center" width="80">
          <template #default="scope">
            <el-tag size="small" :type="levelTag(scope.row.warningLevel)">
              {{ levelMap[scope.row.warningLevel] || "-" }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column :label="t('wms.stockWarningConfig.notifyUsers')" align="center" min-width="130" :show-overflow-tooltip="true">
          <template #default="scope">{{ scope.row.notifyUserNames || "-" }}</template>
        </el-table-column>
        <el-table-column :label="t('wms.stockWarningConfig.enabled')" align="center" width="80">
          <template #default="scope">
            <el-tag size="small" :type="scope.row.enabled === 1 ? 'success' : 'info'">
              {{ scope.row.enabled === 1 ? t('common.enable') : t('common.disable') }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column :label="t('common.remark')" align="center" prop="remark" :show-overflow-tooltip="true" />
        <el-table-column :label="t('common.actions')" align="center" width="150" class-name="small-padding fixed-width">
          <template #default="scope">
            <el-button link type="primary" icon="Edit" @click="handleUpdate(scope.row)" v-hasPermi="['wms:stockWarningConfig:edit']">{{ t('common.edit') }}</el-button>
            <el-button link type="danger" icon="Delete" @click="handleDelete(scope.row)" v-hasPermi="['wms:stockWarningConfig:remove']">{{ t('common.delete') }}</el-button>
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
              <el-form-item :label="t('wms.stockWarningConfig.warehouse')" prop="warehouseId">
                <WarehouseSelect v-model="form.warehouseId" :placeholder="t('wms.stockWarningConfig.rules.warehouseRequired')" @change="onWarehouseChange" />
              </el-form-item>
            </el-col>
            <el-col :span="12">
              <el-form-item :label="t('wms.stockWarningConfig.material')" prop="materialId">
                <MaterialSelect
                  v-model="form.materialId"
                  :label="form.materialCode && form.materialName ? form.materialCode + ' - ' + form.materialName : ''"
                  :placeholder="t('wms.stockWarningConfig.materialPlaceholder')"
                  @change="onMaterialChange"
                />
              </el-form-item>
            </el-col>
            <el-col :span="24">
              <el-form-item :label="t('wms.stockWarningConfig.rangeLabel')">
                <LocationCascader
                  v-model="form.locCascader"
                  :warehouse-id="form.warehouseId"
                  :any-level="true"
                  :disabled="!form.warehouseId"
                  :placeholder="form.warehouseId ? t('wms.stockWarningConfig.rangePlaceholder') : t('wms.stockWarningConfig.rangePlaceholderNeedWarehouse')"
                  @update:model-value="onLocCascader"
                />
              </el-form-item>
            </el-col>
            <el-col :span="12">
              <el-form-item :label="t('wms.stockWarningConfig.warningType')" prop="warningType">
                <el-select v-model="form.warningType" :placeholder="t('wms.stockWarningConfig.rules.typeRequired')" style="width:100%">
                  <el-option :label="t('wms.stockWarningConfig.typeMinOption')" value="min_stock" />
                  <el-option :label="t('wms.stockWarningConfig.typeMaxOption')" value="max_stock" />
                </el-select>
              </el-form-item>
            </el-col>
            <el-col :span="12">
              <el-form-item :label="t('wms.stockWarningConfig.thresholdValue')" prop="thresholdValue">
                <el-input-number v-model="form.thresholdValue" :min="0" :precision="3" :controls="false"
                  :placeholder="t('wms.stockWarningConfig.thresholdPlaceholder')" style="width:100%" />
              </el-form-item>
            </el-col>
            <el-col :span="12">
              <el-form-item :label="t('wms.stockWarningConfig.warningLevel')" prop="warningLevel">
                <el-select v-model="form.warningLevel" :placeholder="t('wms.stockWarningConfig.rules.levelRequired')" style="width:100%">
                  <el-option :label="t('wms.stockWarningConfig.levelNormal')" value="normal" />
                  <el-option :label="t('wms.stockWarningConfig.levelUrgent')" value="urgent" />
                  <el-option :label="t('wms.stockWarningConfig.levelCritical')" value="critical" />
                </el-select>
              </el-form-item>
            </el-col>
            <el-col :span="12">
              <el-form-item :label="t('wms.stockWarningConfig.enabled')">
                <el-switch v-model="form.enabled" :active-value="1" :inactive-value="0" />
              </el-form-item>
            </el-col>
            <el-col :span="24">
              <el-form-item :label="t('wms.stockWarningConfig.notifyUsers')" prop="notifyUserIds">
                <div class="user-picker">
                  <el-button icon="Plus" @click="openUserSelect">{{ t('wms.stockWarningConfig.selectUsers') }}</el-button>
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
              <el-form-item :label="t('common.remark')">
                <el-input v-model="form.remark" maxlength="255" />
              </el-form-item>
            </el-col>
          </el-row>

          <div class="form-tip text-muted">
            {{ t('wms.stockWarningConfig.formTip') }}
          </div>
        </el-form>
        <template #footer>
          <div class="dialog-footer">
            <el-button type="primary" @click="submitForm">{{ t('common.submit') }}</el-button>
            <el-button @click="cancel">{{ t('common.cancel') }}</el-button>
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
import { useI18n } from "vue-i18n";
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

const { t } = useI18n();
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

const warningTypeMap = computed(() => ({ min_stock: t("wms.stockWarningConfig.typeMinStock"), max_stock: t("wms.stockWarningConfig.typeMaxStock") }));
const levelMap = computed(() => ({
  normal: t("wms.stockWarningConfig.levelNormal"),
  urgent: t("wms.stockWarningConfig.levelUrgent"),
  critical: t("wms.stockWarningConfig.levelCritical"),
}));
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
    warehouseId: [{ required: true, message: () => t('wms.stockWarningConfig.rules.warehouseRequired'), trigger: "change" }],
    warningType: [{ required: true, message: () => t('wms.stockWarningConfig.rules.typeRequired'), trigger: "change" }],
    thresholdValue: [{ required: true, message: () => t('wms.stockWarningConfig.rules.thresholdRequired'), trigger: "blur" }],
    warningLevel: [{ required: true, message: () => t('wms.stockWarningConfig.rules.levelRequired'), trigger: "change" }],
  },
});

const { form, queryParams, rules } = toRefs(data);

const queryConfig = computed(() => [
  { label: t('common.remark'), prop: "keywords", type: "input", placeholder: t('wms.stockWarningConfig.keywordsPlaceholder') },
  {
    label: t('wms.stockWarningConfig.enabledStatus'),
    prop: "enabled",
    type: "select",
    placeholder: t('wms.stockWarningConfig.enabledStatus'),
    options: [
      { label: t('common.enable'), value: 1 },
      { label: t('common.disable'), value: 0 },
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
  title.value = t("wms.stockWarningConfig.addTitle");
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
    notifyUsers: idList.map((id, i) => ({ userId: id, userName: nameList[i] || t("wms.stockWarningConfig.userFallback", { id }) })),
    enabled: d.enabled ?? 1,
    remark: d.remark,
  };
  open.value = true;
  title.value = t("wms.stockWarningConfig.editTitle");
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
      ElMessage.warning(t("wms.stockWarningConfig.thresholdPositive"));
      return;
    }
    if (!form.value.notifyUsers || form.value.notifyUsers.length === 0) {
      ElMessage.warning(t("wms.stockWarningConfig.notifyRequired"));
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
        ElMessage.success(t("wms.stockWarningConfig.savedSuccess"));
        open.value = false;
        getList();
      } else {
        ElMessage.error(res.msg || t("wms.stockWarningConfig.saveFailed"));
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
  ElMessageBox.confirm(t("wms.stockWarningConfig.deleteConfirm", { count: delIds.length }), t("wms.common.tip"), { type: "warning" })
    .then(() => delStockWarningConfig(delIds.join(",")))
    .then((res) => {
      if (res.code === 200) {
        ElMessage.success(t("wms.stockWarningConfig.deleteSuccess"));
        getList();
      } else {
        ElMessage.error(res.msg || t("wms.stockWarningConfig.deleteFailed"));
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
