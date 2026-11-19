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
        <el-button type="primary" plain icon="Plus" @click="handleAdd" v-hasPermi="['wms:locationLock:add']">{{ t('common.add') }}</el-button>
      </el-col>
      <el-col :span="1.5">
        <el-button type="success" plain icon="Edit" :disabled="single" @click="handleUpdate" v-hasPermi="['wms:locationLock:edit']">{{ t('common.edit') }}</el-button>
      </el-col>
      <el-col :span="1.5">
        <el-button type="danger" plain icon="Delete" :disabled="multiple" @click="handleDelete" v-hasPermi="['wms:locationLock:remove']">{{ t('common.delete') }}</el-button>
      </el-col>
        <right-toolbar
          v-model:showSearch="showSearch"
          @queryTable="getList"
        ></right-toolbar>
    </el-row>

    <!-- 列表 -->
    <el-table v-loading="loading" :data="locationLockList" @selection-change="handleSelectionChange">
      <el-table-column type="selection" width="55" align="center" />
      <el-table-column :label="t('wms.locationLock.lockType')" align="center" prop="lockType" :show-overflow-tooltip="true">
        <template #default="scope">
          <span>{{ getLockTypeLabel(scope.row.lockType) }}</span>
        </template>
      </el-table-column>
      <el-table-column :label="t('wms.locationLock.lockReason')" align="center" prop="lockReason" :show-overflow-tooltip="true" />
      <el-table-column :label="t('common.createTime')" align="center" prop="createTime" min-width="170" />
      <el-table-column :label="t('common.actions')" align="center" width="150" class-name="small-padding fixed-width">
        <template #default="scope">
          <el-button link type="primary" icon="Edit" @click="handleUpdate(scope.row)" v-hasPermi="['wms:locationLock:edit']">{{ t('common.edit') }}</el-button>
          <el-button link type="danger" icon="Delete" @click="handleDelete(scope.row)" v-hasPermi="['wms:locationLock:remove']">{{ t('common.delete') }}</el-button>
        </template>
      </el-table-column>
      <el-table-column :label="t('wms.common.warehouse')" align="center" :show-overflow-tooltip="true">
        <template #default="scope">
          {{ scope.row.warehouse?.warehouseName || "-" }}
        </template>
      </el-table-column>
      <el-table-column :label="t('wms.locationLock.zoneCode')" align="center" :show-overflow-tooltip="true">
        <template #default="scope">
          {{ scope.row.zone?.zoneCode || "-" }}
        </template>
      </el-table-column>
      <el-table-column :label="t('wms.common.zone')" align="center" :show-overflow-tooltip="true">
        <template #default="scope">
          {{ scope.row.zone?.zoneName || "-" }}
        </template>
      </el-table-column>
      <el-table-column :label="t('wms.locationLock.locationCode')" align="center" :show-overflow-tooltip="true">
        <template #default="scope">
          {{ scope.row.location?.locationCode || "-" }}
        </template>
      </el-table-column>
      <el-table-column :label="t('wms.common.location')" align="center" :show-overflow-tooltip="true">
        <template #default="scope">
          {{ scope.row.location?.locationName || "-" }}
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
    <el-dialog :title="title" v-model="open" width="700px" append-to-body>
      <el-form ref="locationLockRef" :model="form" :rules="rules" label-width="auto">
        <el-form-item :label="t('wms.common.warehouse')" prop="warehouseId">
          <el-select v-model="form.warehouseId" :placeholder="t('wms.locationLock.ph.selectWarehouse')" @change="onWarehouseChange" style="width: 100%">
            <el-option v-for="w in warehouseOptions" :key="w.warehouseId" :label="w.warehouseName" :value="w.warehouseId" />
          </el-select>
        </el-form-item>
        <el-form-item :label="t('wms.common.zone')" prop="zoneId">
          <el-select v-model="form.zoneId" :placeholder="t('wms.locationLock.ph.selectZone')" @change="onZoneChange" style="width: 100%">
            <el-option v-for="z in zoneOptions" :key="z.zoneId" :label="z.zoneName" :value="z.zoneId" />
          </el-select>
        </el-form-item>
        <el-form-item :label="t('wms.common.location')" prop="locationId">
          <el-select v-model="form.locationId" :placeholder="t('wms.locationLock.ph.selectLocation')" style="width: 100%">
            <el-option v-for="l in locationOptions" :key="l.locationId" :label="l.locationCode" :value="l.locationId" />
          </el-select>
        </el-form-item>
        <el-form-item :label="t('wms.locationLock.lockType')" prop="lockType">
          <el-select v-model="form.lockType" :placeholder="t('wms.locationLock.ph.selectType')" style="width: 100%">
            <el-option v-for="d in lockTypeDict" :key="d.dictValue" :label="d.dictLabel" :value="d.dictValue" />
          </el-select>
        </el-form-item>
        <el-form-item :label="t('wms.locationLock.lockReason')" prop="lockReason">
          <el-input v-model="form.lockReason" type="textarea" :placeholder="t('wms.locationLock.ph.inputReason')" maxlength="255" />
        </el-form-item>
        <el-form-item :label="t('wms.locationLock.lockUser')" prop="lockUserId">
          <UserInput v-model="form.lockUserId" style="width:100%" :placeholder="t('wms.locationLock.ph.selectLockUser')" />
        </el-form-item>
        <el-form-item :label="t('wms.locationLock.lockTime')" prop="lockTime">
          <el-date-picker v-model="form.lockTime" type="datetime" value-format="YYYY-MM-DD HH:mm:ss" :placeholder="t('wms.locationLock.ph.lockTime')" style="width: 100%" />
        </el-form-item>
        <el-form-item :label="t('wms.locationLock.unlockUser')" prop="unlockUserId">
          <UserInput v-model="form.unlockUserId" style="width:100%" :placeholder="t('wms.locationLock.ph.selectUnlockUser')" />
        </el-form-item>
        <el-form-item :label="t('wms.locationLock.unlockTime')" prop="unlockTime">
          <el-date-picker v-model="form.unlockTime" type="datetime" value-format="YYYY-MM-DD HH:mm:ss" :placeholder="t('wms.locationLock.ph.unlockTime')" style="width: 100%" />
        </el-form-item>
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

<script setup name="WmsLocationLock">
import { ref, reactive, toRefs, computed, onMounted } from "vue";
import { ElMessage, ElMessageBox } from "element-plus";
import { useI18n } from "vue-i18n";
import UserInput from "@/views/system/user/input.vue";
import { getDicts } from "@/api/system/dict";
import { listWarehouse } from "@/api/wms/warehouse";
import { listZone } from "@/api/wms/zone";
import { listLocation } from "@/api/wms/location";
import {
  listLocationLock,
  getLocationLock,
  addLocationLock,
  updateLocationLock,
  delLocationLock,
} from "@/api/wms/locationLock";
import QueryForm from "@/components/QueryForm/index.vue";
import RightToolbar from "@/components/RightToolbar/index.vue";

const { t } = useI18n();
const locationLockRef = ref(null);
const locationLockList = ref([]);
const open = ref(false);
const loading = ref(true);
const ids = ref([]);
const single = ref(true);
const multiple = ref(true);
const total = ref(0);
const title = ref("");

const showSearch = ref(true);

// 下拉选项
const warehouseOptions = ref([]);
const zoneOptions = ref([]);
const locationOptions = ref([]);
const lockTypeDict = ref([]);

const data = reactive({
  form: {},
  queryParams: {
    pageNum: 1,
    pageSize: 10,
    keywords: undefined,
  },
});

const { form, queryParams } = toRefs(data);

const rules = computed(() => ({
  warehouseId: [{ required: true, message: t('wms.locationLock.rules.warehouseRequired'), trigger: "change" }],
  zoneId: [{ required: true, message: t('wms.locationLock.rules.zoneRequired'), trigger: "change" }],
  locationId: [{ required: true, message: t('wms.locationLock.rules.locationRequired'), trigger: "change" }],
  lockType: [{ required: true, message: t('wms.locationLock.rules.typeRequired'), trigger: "change" }],
  lockReason: [{ required: true, message: t('wms.locationLock.rules.reasonRequired'), trigger: "blur" }],
}));

const queryConfig = computed(() => [
  {
    label: t('wms.common.keywords'),
    prop: "keywords",
    type: "input",
    placeholder: t('wms.locationLock.ph.keywords'),
  },
]);

/** 获取仓库列表 */
function fetchWarehouses() {
  listWarehouse({ pageSize: 0 }).then((res) => {
    warehouseOptions.value = res.rows || [];
  });
}

/** 获取库区列表（按仓库过滤） */
function fetchZones(warehouseId) {
  zoneOptions.value = [];
  if (!warehouseId) return;
  listZone({ pageSize: 0, params: { warehouseId } }).then((res) => {
    zoneOptions.value = res.rows || [];
  });
}

/** 获取库位列表（按仓库+库区过滤） */
function fetchLocations(warehouseId, zoneId) {
  locationOptions.value = [];
  if (!warehouseId || !zoneId) return;
  listLocation({ pageSize: 0, params: { warehouseId, zoneId } }).then((res) => {
    locationOptions.value = res.rows || [];
  });
}

/** 获取锁定类型字典 */
function fetchLockTypeDict() {
  getDicts("wms_lock_type").then((res) => {
    lockTypeDict.value = res.data || [];
  });
}

/** 根据 dictValue 获取 dictLabel */
function getLockTypeLabel(value) {
  const item = lockTypeDict.value.find((d) => d.dictValue === value);
  return item ? item.dictLabel : value;
}

/** 仓库变更 */
function onWarehouseChange(val) {
  form.value.zoneId = undefined;
  form.value.locationId = undefined;
  locationOptions.value = [];
  fetchZones(val);
}

/** 库区变更 */
function onZoneChange(val) {
  form.value.locationId = undefined;
  fetchLocations(form.value.warehouseId, val);
}

/** 查询列表 */
function getList() {
  loading.value = true;
  listLocationLock(queryParams.value).then((res) => {
    locationLockList.value = res.rows || [];
    total.value = res.total || 0;
    loading.value = false;
  });
}

/** 标签页重新激活时由布局回调（是否刷新由页面决定） */
function tabRefresh() {
  getList();
}

/** 多选 */
function handleSelectionChange(selection) {
  ids.value = selection.map((i) => i.lockId);
  single.value = selection.length !== 1;
  multiple.value = !selection.length;
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

/** 表单重置 */
function reset() {
  form.value = {
    lockId: undefined,
    warehouseId: undefined,
    zoneId: undefined,
    locationId: undefined,
    lockType: undefined,
    lockReason: undefined,
    lockUserId: undefined,
    lockTime: undefined,
    unlockUserId: undefined,
    unlockTime: undefined,
  };
  zoneOptions.value = [];
  locationOptions.value = [];
  if (locationLockRef.value) locationLockRef.value.resetFields();
}

/** 新增 */
function handleAdd() {
  reset();
  open.value = true;
  title.value = t("wms.locationLock.addTitle");
}

/** 修改 */
function handleUpdate(row) {
  reset();
  const id = row.lockId || ids.value[0];
  getLocationLock(id).then((res) => {
    const d = res.data || {};
    form.value = d;
    // 回显联动下拉：加载仓库→库区→库位
    if (d.warehouseId) {
      fetchZones(d.warehouseId);
      if (d.zoneId) {
        fetchLocations(d.warehouseId, d.zoneId);
      }
    }
    open.value = true;
    title.value = t("wms.locationLock.editTitle");
  });
}

/** 提交 */
function submitForm() {
  locationLockRef.value.validate((valid) => {
    if (!valid) return;
    const api = form.value.lockId ? updateLocationLock : addLocationLock;
    api(form.value).then(() => {
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

/** 删除 */
function handleDelete(row) {
  const delIds = row.lockId || ids.value.join(",");
  ElMessageBox.confirm(t("wms.locationLock.deleteConfirm"), t("wms.common.warning"), {
    confirmButtonText: t("common.confirm"),
    cancelButtonText: t("common.cancel"),
    type: "warning",
  })
    .then(() => delLocationLock(delIds))
    .then(() => {
      getList();
      ElMessage.success(t("wms.common.deleteSuccess"));
    });
}

onMounted(() => {
  fetchWarehouses();
  fetchLockTypeDict();
  getList();
});
</script>
