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
        <el-table-column label="编码" align="center" prop="code" width="120" />
        <el-table-column label="名称" align="center" prop="name" :show-overflow-tooltip="true" />
        <el-table-column label="父级编码" align="center" prop="parentCode" width="120" />
        <el-table-column label="层级" align="center" width="100">
          <template #default="scope">
            <el-tag :type="levelType(scope.row.code)">{{ levelText(scope.row.code) }}</el-tag>
          </template>
        </el-table-column>
        <el-table-column label="更新时间" align="center" prop="updateDate" min-width="170">
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
import QueryForm from "@/components/QueryForm/index.vue";
import RightToolbar from "@/components/RightToolbar/index.vue";
import { listRegion } from "@/api/wms/region";

const regionList = ref([]);
const loading = ref(true);
const showSearch = ref(true);
const total = ref(0);

const queryConfig = computed(() => [
  {
    label: "编码",
    prop: "code",
    type: "input",
    placeholder: "请输入行政区划编码",
  },
  {
    label: "名称",
    prop: "name",
    type: "input",
    placeholder: "请输入省/市/区名称",
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
  if (code.length === 2) return "省";
  if (code.length === 4) return "市";
  if (code.length === 6) return "区/县";
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
