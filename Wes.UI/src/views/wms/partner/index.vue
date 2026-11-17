<template>
  <div class="app-container">
    <div class="main-panel p16">
      <query-form
        :config="queryConfig"
        v-model:visible="showSearch"
        v-model="queryParams.params"
        @query="handleQuery"
        @reset="resetQuery"
      />

    <!-- 操作按钮 -->
    <el-row :gutter="10" class="mb8">
      <el-col :span="1.5">
        <el-button type="primary" plain icon="Plus" @click="handleAdd" v-hasPermi="[perm('add')]">新增</el-button>
      </el-col>
      <el-col :span="1.5">
        <el-button type="success" plain icon="Edit" :disabled="single" @click="handleUpdate" v-hasPermi="[perm('edit')]">修改</el-button>
      </el-col>
      <el-col :span="1.5">
        <el-button type="danger" plain icon="Delete" :disabled="multiple" @click="handleDelete" v-hasPermi="[perm('remove')]">删除</el-button>
      </el-col>
        <right-toolbar
          v-model:showSearch="showSearch"
          @queryTable="getList"
        ></right-toolbar>
    </el-row>

    <!-- 列表 -->
    <el-table v-loading="loading" :data="partnerList" @selection-change="handleSelectionChange">
      <el-table-column type="selection" width="55" align="center" />
      <el-table-column :label="typeName + '编码'" align="center" prop="partnerCode" :show-overflow-tooltip="true" />
      <el-table-column :label="typeName + '名称'" align="center" prop="partnerName" :show-overflow-tooltip="true" />

      <el-table-column label="省份" align="center" prop="province" :show-overflow-tooltip="true" />
      <el-table-column label="状态" align="center" prop="status" width="80">
        <template #default="scope">
          <el-switch v-model="scope.row.status" active-value="0" inactive-value="1" @change="handleStatusChange(scope.row)" />
        </template>
      </el-table-column>
      <el-table-column label="创建时间" align="center" prop="createTime" min-width="170" />
      <el-table-column label="操作" align="center" width="150" class-name="small-padding fixed-width">
        <template #default="scope">
          <el-button link type="primary" icon="Edit" @click="handleUpdate(scope.row)" v-hasPermi="[perm('edit')]">修改</el-button>
          <el-button link type="danger" icon="Delete" @click="handleDelete(scope.row)" v-hasPermi="[perm('remove')]">删除</el-button>
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
    <el-dialog :title="title" v-model="open" width="900px" append-to-body>
      <el-tabs v-model="activeTab">
        <!-- 基本信息 -->
        <el-tab-pane label="基本信息" name="base">
          <el-form ref="partnerRef" :model="form" :rules="rules" label-width="auto">
            <el-form-item :label="typeName + '编码'" prop="partnerCode">
              <el-input v-model="form.partnerCode" :placeholder="'请输入' + typeName + '编码'" maxlength="50" />
            </el-form-item>
            <el-form-item :label="typeName + '名称'" prop="partnerName">
              <el-input v-model="form.partnerName" :placeholder="'请输入' + typeName + '名称'" maxlength="100" />
            </el-form-item>
            <el-form-item label="类型" prop="partnerType">
              <el-radio-group v-model="form.partnerType" :disabled="!!form.partnerId">
                <el-radio value="supplier">供应商</el-radio>
                <el-radio value="customer">客户</el-radio>
              </el-radio-group>
            </el-form-item>
            <el-form-item label="省/市/区">
              <el-cascader :options="regionOptions" v-model="regionArr" placeholder="请选择省/市/区" style="width: 100%" />
            </el-form-item>
            <el-form-item label="详细地址" prop="address">
              <el-input v-model="form.address" type="textarea" placeholder="请输入详细地址" maxlength="255" />
            </el-form-item>
            <el-form-item label="合作开始日期" prop="cooperateStartDate">
              <el-date-picker v-model="form.cooperateStartDate" type="date" value-format="YYYY-MM-DD" placeholder="请输入合作开始日期" style="width: 100%" />
            </el-form-item>
            <el-form-item label="状态" prop="status">
              <el-radio-group v-model="form.status">
                <el-radio value="0">启用</el-radio>
                <el-radio value="1">停用</el-radio>
              </el-radio-group>
            </el-form-item>
            <el-form-item label="备注" prop="remark">
              <el-input v-model="form.remark" type="textarea" placeholder="请输入备注" maxlength="500" />
            </el-form-item>
          </el-form>
        </el-tab-pane>

        <!-- 联系人 -->
        <el-tab-pane label="联系人" name="contact">
          <el-row :gutter="10" class="mb8">
            <el-col :span="1.5">
              <el-button type="primary" plain icon="Plus" @click="handleContactAdd">新增联系人</el-button>
            </el-col>
            <el-col :span="1.5">
              <el-button type="danger" plain icon="Delete" :disabled="contactMultiple" @click="handleContactDelete">删除选中</el-button>
            </el-col>
          </el-row>
          <el-table :data="contactList" @selection-change="handleContactSelectionChange" table-layout="fixed">
            <el-table-column type="selection" width="55" align="center" />
            <el-table-column label="姓名" align="center">
              <template #default="scope">
                <el-input v-model="scope.row.contactName" placeholder="姓名" />
              </template>
            </el-table-column>
            <el-table-column label="职务" align="center">
              <template #default="scope">
                <el-input v-model="scope.row.title" placeholder="职务" />
              </template>
            </el-table-column>
            <el-table-column label="电话" align="center">
              <template #default="scope">
                <el-input v-model="scope.row.phone" placeholder="电话" />
              </template>
            </el-table-column>
            <el-table-column label="邮箱" align="center" width="220">
              <template #default="scope">
                <el-input v-model="scope.row.email" placeholder="邮箱" />
              </template>
            </el-table-column>
            <el-table-column label="默认" align="center" width="80">
              <template #default="scope">
                <el-switch v-model="scope.row.isDefault" active-value="1" inactive-value="0" />
              </template>
            </el-table-column>
          </el-table>
        </el-tab-pane>

        <!-- 收货地址 -->
        <el-tab-pane label="收货地址" name="address">
          <el-row :gutter="10" class="mb8">
            <el-col :span="1.5">
              <el-button type="primary" plain icon="Plus" @click="handleAddressAdd">新增地址</el-button>
            </el-col>
            <el-col :span="1.5">
              <el-button type="danger" plain icon="Delete" :disabled="addressMultiple" @click="handleAddressDelete">删除选中</el-button>
            </el-col>
          </el-row>
          <el-table :data="addressList" @selection-change="handleAddressSelectionChange">
            <el-table-column type="selection" width="55" align="center" />
            <el-table-column label="省/市/区" align="center" min-width="220">
              <template #default="scope">
                <el-cascader :options="regionOptions" v-model="scope.row._region" placeholder="请选择" style="width: 100%" />
              </template>
            </el-table-column>
            <el-table-column label="详细地址" align="center" min-width="200">
              <template #default="scope">
                <el-input v-model="scope.row.address" placeholder="详细地址" />
              </template>
            </el-table-column>
            <el-table-column label="收货人" align="center" width="120">
              <template #default="scope">
                <el-input v-model="scope.row.contactName" placeholder="收货人" />
              </template>
            </el-table-column>
            <el-table-column label="电话" align="center" width="140">
              <template #default="scope">
                <el-input v-model="scope.row.contactPhone" placeholder="电话" />
              </template>
            </el-table-column>
            <el-table-column label="默认" align="center" width="80">
              <template #default="scope">
                <el-switch v-model="scope.row.isDefault" active-value="1" inactive-value="0" />
              </template>
            </el-table-column>
          </el-table>
        </el-tab-pane>
      </el-tabs>

      <template #footer>
        <div class="dialog-footer">
          <el-button type="primary" @click="submitForm">确 定</el-button>
          <el-button @click="cancel">取 消</el-button>
        </div>
      </template>
    </el-dialog>
    </div>
  </div>
</template>

<script setup name="WmsPartner">
import { ref, reactive, toRefs, computed, watch, onMounted } from "vue";
import { useRoute } from "vue-router";
import { ElMessage, ElMessageBox } from "element-plus";
import {
  listPartner,
  getPartner,
  addPartner,
  updatePartner,
  delPartner,
  changePartnerStatus,
} from "@/api/wms/partner";
import { listPartnerAddress, addPartnerAddress, updatePartnerAddress, delPartnerAddress } from "@/api/wms/partnerAddress";
import { listPartnerContact, addPartnerContact, updatePartnerContact, delPartnerContact } from "@/api/wms/partnerContact";
import { treeRegion } from "@/api/wms/region";
import QueryForm from "@/components/QueryForm/index.vue";
import RightToolbar from "@/components/RightToolbar/index.vue";

const route = useRoute();

// 根据路由判断页面类型：/supplier → 供应商，/customer → 客户
const pageType = computed(() => {
  const p = route.path;
  if (p.includes("/customer")) return "customer";
  return "supplier";
});
const isSupplier = computed(() => pageType.value === "supplier");
const typeName = computed(() => (isSupplier.value ? "供应商" : "客户"));
const typeCode = computed(() => (isSupplier.value ? "supplier" : "customer"));

const partnerRef = ref(null);
const partnerList = ref([]);
const open = ref(false);
const loading = ref(true);
const ids = ref([]);
const single = ref(true);
const multiple = ref(true);
const total = ref(0);
const title = ref("");
const activeTab = ref("base");
const regionOptions = ref([]);
const regionArr = ref([]);

const contactList = ref([]);
const contactIds = ref([]);
const contactMultiple = ref(true);
const addressList = ref([]);
const addressIds = ref([]);
const addressMultiple = ref(true);

const showSearch = ref(true);

const data = reactive({
  form: {},
  queryParams: {
    pageNum: 1,
    pageSize: 10,
    params: {
      keywords: undefined,
      status: undefined,
      partnerType: typeCode.value,
    },
  },
});

const { form, queryParams } = toRefs(data);

const rules = computed(() => ({
  partnerCode: [{ required: true, message: `${typeName.value}编码不能为空`, trigger: "blur" }],
  partnerName: [{ required: true, message: `${typeName.value}名称不能为空`, trigger: "blur" }],
}));

// 根据路由类型（supplier/customer）同步 partnerType 并刷新列表
// 菜单切换时 Vue 复用同一组件不会重新挂载，需手动触发查询
let isFirstLoad = true;
watch(typeCode, (val) => {
  queryParams.value.params.partnerType = val;
  if (!isFirstLoad) {
    handleQuery();
  }
  isFirstLoad = false;
}, { immediate: true });

const queryConfig = computed(() => [
  {
    label: "关键字",
    prop: "keywords",
    type: "input",
    placeholder: `请输入${typeName.value}编码/名称`,
  },
  {
    label: "状态",
    prop: "status",
    type: "select",
    placeholder: "请选择状态",
  },
]);

/** 权限标识生成 */
function perm(action) {
  return `wms:${typeCode.value}:${action}`;
}

/** 主表多选 */
function handleSelectionChange(selection) {
  ids.value = selection.map((i) => i.partnerId);
  single.value = selection.length !== 1;
  multiple.value = !selection.length;
}

/** 查询列表 */
function getList() {
  loading.value = true;
  listPartner(queryParams.value).then((res) => {
    partnerList.value = res.rows || [];
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
  queryParams.value.params.keywords = undefined;
  queryParams.value.params.status = undefined;
  queryParams.value.params.partnerType = typeCode.value;
  queryParams.value.pageNum = 1;
  handleQuery();
}

/** 表单重置 */
function reset() {
  form.value = {
    partnerId: undefined,
    partnerCode: undefined,
    partnerName: undefined,
    partnerType: typeCode.value,
    province: undefined,
    city: undefined,
    district: undefined,
    address: undefined,
    cooperateStartDate: undefined,
    status: "0",
    remark: undefined,
  };
  regionArr.value = [];
  contactList.value = [];
  addressList.value = [];
  contactIds.value = [];
  addressIds.value = [];
  if (partnerRef.value) partnerRef.value.resetFields();
}

/** 新增 */
function handleAdd() {
  reset();
  open.value = true;
  title.value = `添加${typeName.value}`;
}

/** 修改 */
function handleUpdate(row) {
  reset();
  const id = row.partnerId || ids.value[0];
  getPartner(id).then((res) => {
    form.value = res.data || {};
    regionArr.value = [res.data?.province, res.data?.city, res.data?.district].filter(Boolean);
    // 加载联系人
    listPartnerContact({ pageNum: 1, pageSize: 1000, params: { partnerId: id } }).then((r) => {
      contactList.value = (r.rows || []).map((c) => ({ ...c, isDefault: c.isDefault || "0" }));
    });
    // 加载收货地址
    listPartnerAddress({ pageNum: 1, pageSize: 1000, params: { partnerId: id } }).then((r) => {
      addressList.value = (r.rows || []).map((a) => ({
        ...a,
        _region: [a.province, a.city, a.district].filter(Boolean),
        isDefault: a.isDefault || "0",
      }));
    });
    open.value = true;
    title.value = `修改${typeName.value}`;
  });
}

/** 联系人行操作 */
function handleContactAdd() {
  contactList.value.push({ partnerId: form.value.partnerId, contactName: "", title: "", phone: "", email: "", isDefault: "0" });
}
function handleContactSelectionChange(selection) {
  contactIds.value = selection.map((i) => i.contactId);
  contactMultiple.value = selection.length === 0;
}
function handleContactDelete() {
  if (contactIds.value.length === 0) return;
  const savedIds = contactIds.value.filter(Boolean);
  const unsavedIds = contactIds.value.filter((id) => !id);
  const doDelete = () => {
    // 已保存的：调 API 删除
    const task = savedIds.length > 0 ? delPartnerContact(savedIds.join(",")) : Promise.resolve();
    task.then(() => {
      // 已保存的通过 ID 过滤，未保存的（无 ID）直接全部移除
      if (unsavedIds.length > 0) {
        // 无 ID 的行全部移除（通过索引过滤掉 selection 中无 ID 的行）
        contactList.value = contactList.value.filter((i) => i.contactId);
      }
      if (savedIds.length > 0) {
        contactList.value = contactList.value.filter((i) => !savedIds.includes(i.contactId));
      }
      contactIds.value = [];
      ElMessage.success("删除成功");
    });
  };
  if (savedIds.length > 0) {
    ElMessageBox.confirm("是否确认删除选中的联系人？", "警告", {
      confirmButtonText: "确定",
      cancelButtonText: "取消",
      type: "warning",
    }).then(doDelete).catch(() => {});
  } else {
    doDelete();
  }
}

/** 地址行操作 */
function handleAddressAdd() {
  addressList.value.push({ partnerId: form.value.partnerId, _region: [], address: "", contactName: "", contactPhone: "", isDefault: "0" });
}
function handleAddressSelectionChange(selection) {
  addressIds.value = selection.map((i) => i.addressId);
  addressMultiple.value = selection.length === 0;
}
function handleAddressDelete() {
  if (addressIds.value.length === 0) return;
  const savedIds = addressIds.value.filter(Boolean);
  const unsavedIds = addressIds.value.filter((id) => !id);
  const doDelete = () => {
    // 已保存的：调 API 删除
    const task = savedIds.length > 0 ? delPartnerAddress(savedIds.join(",")) : Promise.resolve();
    task.then(() => {
      // 已保存的通过 ID 过滤，未保存的（无 ID）直接全部移除
      if (unsavedIds.length > 0) {
        addressList.value = addressList.value.filter((i) => i.addressId);
      }
      if (savedIds.length > 0) {
        addressList.value = addressList.value.filter((i) => !savedIds.includes(i.addressId));
      }
      addressIds.value = [];
      ElMessage.success("删除成功");
    });
  };
  if (savedIds.length > 0) {
    ElMessageBox.confirm("是否确认删除选中的收货地址？", "警告", {
      confirmButtonText: "确定",
      cancelButtonText: "取消",
      type: "warning",
    }).then(doDelete).catch(() => {});
  } else {
    doDelete();
  }
}

/** 提交 */
function submitForm() {
  partnerRef.value.validate((valid) => {
    if (!valid) {
      activeTab.value = "base";
      return;
    }
    form.value.province = regionArr.value?.[0];
    form.value.city = regionArr.value?.[1];
    form.value.district = regionArr.value?.[2];
    const api = form.value.partnerId ? updatePartner : addPartner;
    api(form.value).then(() => {
      const pid = form.value.partnerId;
      // 保存联系人
      const cTasks = contactList.value.map((c) => {
        c.partnerId = pid;
        return c.contactId ? updatePartnerContact(c) : addPartnerContact(c);
      });
      // 保存地址
      const aTasks = addressList.value.map((a) => {
        a.partnerId = pid;
        a.province = a._region?.[0];
        a.city = a._region?.[1];
        a.district = a._region?.[2];
        return a.addressId ? updatePartnerAddress(a) : addPartnerAddress(a);
      });
      Promise.all([...cTasks, ...aTasks]).then(() => {
        ElMessage.success("保存成功");
        open.value = false;
        getList();
      });
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
  const delIds = row.partnerId || ids.value.join(",");
  ElMessageBox.confirm(`是否确认删除选中的${typeName.value}数据项？`, "警告", {
    confirmButtonText: "确定",
    cancelButtonText: "取消",
    type: "warning",
  })
    .then(() => delPartner(delIds))
    .then(() => {
      getList();
      ElMessage.success("删除成功");
    });
}

/** 状态切换 */
function handleStatusChange(row) {
  const text = row.status === "0" ? "启用" : "停用";
  ElMessageBox.confirm(`确认要"${text}"该记录吗？`, "提示", {
    confirmButtonText: "确定",
    cancelButtonText: "取消",
    type: "warning",
  })
    .then(() => {
      changePartnerStatus({ partnerId: row.partnerId, status: row.status }).then(() => {
        ElMessage.success("操作成功");
      });
    })
    .catch(() => {
      row.status = row.status === "0" ? "1" : "0";
    });
}

onMounted(() => {
  treeRegion().then((res) => {
    regionOptions.value = res.data || res.rows || res || [];
  });
  getList();
});
</script>
