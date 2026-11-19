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

      <el-row :gutter="10" class="mb8">
        <right-toolbar
          v-model:showSearch="showSearch"
          @queryTable="getList"
        ></right-toolbar>
      </el-row>

      <el-table v-loading="loading" :data="regionList" :default-expand-all="false">
        <el-table-column :label="t('wms.common.code')" align="center" prop="code" width="120" />
        <el-table-column :label="t('wms.common.name')" align="center" prop="name" :show-overflow-tooltip="true" />
        <el-table-column :label="t('wms.region.parentCode')" align="center" prop="parentCode" width="120" />
        <el-table-column :label="t('wms.region.level')" align="center" width="100">
          <template #default="scope">
            <el-tag :type="levelType(scope.row.code)">{{ levelText(scope.row.code) }}</el-tag>
          </template>
        </el-table-column>
        <el-table-column :label="t('common.lastUpdateTime')" align="center" prop="updateDate" min-width="170">
          <template #default="scope">
            <span>{{ formatTime(scope.row.updateDate) }}</span>
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
    </div>
  </div>
</template>

<script setup name="WmsRegion">
import { ref, reactive, toRefs, computed } from "vue";
import { useI18n } from "vue-i18n";
import QueryForm from "@/components/QueryForm/index.vue";
import RightToolbar from "@/components/RightToolbar/index.vue";
import { listRegion } from "@/api/wms/region";

const { t } = useI18n();

const regionList = ref([]);
const loading = ref(true);
const showSearch = ref(true);
const total = ref(0);

const queryConfig = computed(() => [
  {
    label: t('wms.common.code'),
    prop: "code",
    type: "input",
    placeholder: t('wms.region.ph.inputCode'),
  },
  {
    label: t('wms.common.name'),
    prop: "name",
    type: "input",
    placeholder: t('wms.region.ph.inputName'),
  },
]);

const data = reactive({
  queryParams: {
    code: undefined,
    name: undefined,
    pageNum: 1,
    pageSize: 10,
  },
});

const { queryParams } = toRefs(data);

/** 层级标识：2位=省，4位=市，6位=区 */
function levelText(code) {
  if (!code) return "-";
  if (code.length === 2) return t('wms.region.province');
  if (code.length === 4) return t('wms.region.city');
  if (code.length === 6) return t('wms.region.district');
  return "-";
}
function levelType(code) {
  if (!code) return "info";
  if (code.length === 2) return "danger";
  if (code.length === 4) return "warning";
  return "success";
}

/** 查询列表 */
function getList() {
  loading.value = true;
  listRegion(queryParams.value).then((res) => {
    regionList.value = res.rows || [];
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
    code: undefined,
    name: undefined,
    pageNum: 1,
    pageSize: 10,
  };
  handleQuery();
}

/** 时间格式化 */
function formatTime(time) {
  if (!time) return "-";
  const d = new Date(time);
  const pad = (n) => String(n).padStart(2, "0");
  return `${d.getFullYear()}-${pad(d.getMonth() + 1)}-${pad(d.getDate())} ${pad(d.getHours())}:${pad(d.getMinutes())}:${pad(d.getSeconds())}`;
}

getList();
</script>
