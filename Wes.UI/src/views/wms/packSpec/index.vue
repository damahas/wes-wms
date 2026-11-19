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
        <el-button type="primary" plain icon="Plus" @click="handleAdd" v-hasPermi="['wms:packSpec:add']">{{ t('wms.packSpec.addBtn') }}</el-button>
      </el-col>
        <right-toolbar
          v-model:showSearch="showSearch"
          @queryTable="getList"
        ></right-toolbar>
    </el-row>

    <!-- 列表：仅展示顶层包装规格 -->
    <el-table v-loading="loading" :data="packSpecList" @selection-change="handleSelectionChange">
      <el-table-column type="selection" width="55" align="center" />
      <el-table-column :label="t('wms.packSpec.levelName')" align="center" prop="levelName" :show-overflow-tooltip="true" />
      <el-table-column :label="t('wms.packSpec.uom')" align="center" prop="uom" :show-overflow-tooltip="true" />
      <el-table-column :label="t('wms.packSpec.levelCount')" align="center" prop="levelCount">
        <template #default="scope">{{ scope.row.levelCount ?? '—' }}</template>
      </el-table-column>
      <el-table-column :label="t('wms.packSpec.isContinuous')" align="center" prop="isContinuous">
        <template #default="scope">
          <el-tag :type="scope.row.isContinuous === 1 ? 'warning' : 'info'" size="small">
            {{ scope.row.isContinuous === 1 ? t('wms.packSpec.continuous') : t('wms.packSpec.discrete') }}
          </el-tag>
        </template>
      </el-table-column>
      <el-table-column :label="t('wms.material.basicUnit')" align="center" prop="isBase">
        <template #default="scope">
          <el-tag v-if="scope.row.isBase === 1" type="success" size="small">{{ t('wms.packSpec.base') }}</el-tag>
        </template>
      </el-table-column>
      <el-table-column :label="t('common.createTime')" align="center" prop="createTime" min-width="170" />
      <el-table-column :label="t('common.actions')" align="center" width="200" class-name="small-padding fixed-width">
        <template #default="scope">
          <el-button link type="primary" icon="Edit" @click="handleUpdate(scope.row)" v-hasPermi="['wms:packSpec:edit']">{{ t('wms.packSpec.manageLevels') }}</el-button>
          <el-button link type="danger" icon="Delete" @click="handleDelete(scope.row)" v-hasPermi="['wms:packSpec:remove']">{{ t('common.delete') }}</el-button>
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

    <!-- 层级管理弹窗 -->
    <el-dialog :title="title" v-model="open" width="900px" append-to-body @closed="onDialogClosed">
      <el-row :gutter="16">
        <!-- 左：层级树 -->
        <el-col :span="11">
          <div class="tree-panel">
            <div class="tree-panel__header">
              <span>{{ t('wms.packSpec.treeHeader') }}（{{ dialogMode === 'create' ? t('wms.packSpec.treeCreateHint') : t('wms.packSpec.treeEditHint') }}）</span>
            </div>
            <div class="tree-panel__body">
              <el-tree
                v-if="treeData.length"
                :data="treeData"
                :props="{ label: 'label', children: 'children' }"
                default-expand-all
                :expand-on-click-node="false"
                node-key="specId"
                highlight-current
                ref="treeRef"
                @node-click="onNodeClick"
              >
                <template #default="{ data }">
                  <span class="level-node">
                    <span class="level-node__name">{{ data.levelName }}</span>
                    <span class="level-node__meta">
                      {{ data.uom ? data.uom : '' }}
                      <template v-if="data.qtyPerParent != null && data.qtyPerParent !== ''"> · {{ data.qtyPerParent }}/{{ t('wms.packSpec.upper') }}</template>
                      <template v-if="data.isBase === 1"> · {{ t('wms.packSpec.base') }}</template>
                    </span>
                  </span>
                </template>
              </el-tree>
              <el-empty v-else :description="t('wms.packSpec.treeEmpty')" :image-size="80" />
            </div>
            <div class="tree-panel__footer" v-if="selectedNode && dialogMode === 'edit'">
              <el-button type="primary" link icon="Plus" @click="addChild(selectedNode)">{{ t('wms.packSpec.addChild') }}</el-button>
              <el-button type="danger" link icon="Delete" @click="deleteNode(selectedNode)">{{ t('wms.packSpec.deleteLevel') }}</el-button>
            </div>
          </div>
        </el-col>

        <!-- 右：节点表单 -->
        <el-col :span="13">
          <el-form ref="nodeRef" :model="nodeForm" :rules="nodeRules" label-width="auto">
            <el-alert
              v-if="dialogMode === 'create'"
              type="info"
              :closable="false"
              :title="t('wms.packSpec.firstStepAlert')"
              class="mb12"
            />
            <el-form-item :label="t('wms.packSpec.parentLevel')">
              <el-input :model-value="nodeForm.parentName || t('wms.packSpec.topNoParent')" readonly :placeholder="t('wms.packSpec.top')" />
            </el-form-item>
            <el-row :gutter="16">
              <el-col :span="12">
                <el-form-item :label="t('wms.packSpec.levelName')" prop="levelName">
                  <el-input v-model="nodeForm.levelName" :placeholder="t('wms.packSpec.ph.levelNameExample')" maxlength="50" />
                </el-form-item>
              </el-col>
              <el-col :span="12">
                <el-form-item :label="t('wms.packSpec.uom')" prop="uom">
                  <el-select v-model="nodeForm.uom" :placeholder="t('wms.common.pleaseSelect')" clearable filterable style="width: 100%">
                    <el-option v-for="dict in wms_sys_unit_type" :key="dict.value" :label="dict.label" :value="dict.value" />
                  </el-select>
                </el-form-item>
              </el-col>
            </el-row>
            <el-row :gutter="16">
              <el-col :span="12">
                <el-form-item :label="t('wms.packSpec.qtyPerParent')" prop="qtyPerParent">
                  <el-input-number controls-position="right" v-model="nodeForm.qtyPerParent" :min="0" :precision="3" :controls="false" style="width: 100%" />
                </el-form-item>
              </el-col>
              <el-col :span="12">
                <el-form-item :label="t('wms.packSpec.sortOrder')" prop="sortOrder">
                  <el-input-number controls-position="right" v-model="nodeForm.sortOrder" :min="0" :precision="0" :controls="false" style="width: 100%" />
                </el-form-item>
              </el-col>
            </el-row>
            <el-row :gutter="16">
              <el-col :span="12">
                <el-form-item :label="t('wms.packSpec.isContinuous')" prop="isContinuous" class="compact-switch">
                  <el-switch
                    v-model="nodeForm.isContinuous"
                    :active-value="1"
                    :inactive-value="0"
                    inline-prompt
                    :width="58"
                    :active-text="t('wms.packSpec.continuous')"
                    :inactive-text="t('wms.packSpec.discrete')"
                  />
                </el-form-item>
              </el-col>
              <el-col :span="12">
                <el-form-item :label="t('wms.packSpec.isBaseLabel')" prop="isBase" class="compact-switch">
                  <el-switch
                    v-model="nodeForm.isBase"
                    :active-value="1"
                    :inactive-value="0"
                    inline-prompt
                    :width="64"
                    :active-text="t('wms.packSpec.base')"
                    :inactive-text="t('wms.packSpec.notBase')"
                  />
                </el-form-item>
              </el-col>
            </el-row>
            <el-form-item :label="t('wms.packSpec.effectiveDate')" prop="effectiveDate">
              <el-date-picker v-model="nodeForm.effectiveDate" type="datetime" value-format="YYYY-MM-DD HH:mm:ss" :placeholder="t('wms.packSpec.ph.effectiveHint')" style="width: 100%" />
            </el-form-item>
            <el-form-item>
              <el-button type="primary" @click="saveNode">{{ t('wms.packSpec.saveNode') }}</el-button>
              <el-button v-if="dialogMode === 'edit' && nodeForm.specId" @click="resetNodeForm">{{ t('common.reset') }}</el-button>
            </el-form-item>
          </el-form>
        </el-col>
      </el-row>
    </el-dialog>
  </div>
</template>

<script setup name="WmsPackSpec">
import { ref, reactive, toRefs, computed } from "vue";
import { ElMessage, ElMessageBox } from "element-plus";
import { useI18n } from "vue-i18n";
import {
  listPackSpec,
  addPackSpec,
  updatePackSpec,
  delPackSpec,
  listPackSpecTree,
} from "@/api/wms/packSpec";
import { getDict, handleTree } from "@/utils";
import QueryForm from "@/components/QueryForm/index.vue";
import RightToolbar from "@/components/RightToolbar/index.vue";

const { t } = useI18n();
const { wms_sys_unit_type } = getDict("wms_sys_unit_type");

const packSpecList = ref([]);
const open = ref(false);
const loading = ref(true);
const ids = ref([]);
const single = ref(true);
const multiple = ref(true);
const total = ref(0);
const title = ref("");

const showSearch = ref(true);

// 弹窗模式：create=新建整套（先建顶层）；edit=管理已有整套层级树
const dialogMode = ref("edit");
const currentRootId = ref(undefined);
const treeData = ref([]);
const selectedNode = ref(null);
const treeRef = ref(null);

const data = reactive({
  queryParams: {
    pageNum: 1,
    pageSize: 10,
    params: {
      keywords: undefined,
    },
  },
});

const { queryParams } = toRefs(data);

const queryConfig = computed(() => [
  {
    label: t('wms.common.keywords'),
    prop: "keywords",
    type: "input",
    placeholder: t('wms.packSpec.ph.keywords'),
  },
]);

/** 查询顶层包装规格列表 */
function getList() {
  loading.value = true;
  listPackSpec(queryParams.value).then((res) => {
    packSpecList.value = (res.rows || []).map((r) => ({ ...r, levelCount: undefined }));
    // 取每套层级数量（顶层 + 子层）
    packSpecList.value.forEach((top) => {
      listPackSpecTree(top.specId).then((t) => {
        top.levelCount = (t.rows || []).length;
      });
    });
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
  queryParams.value.params = { keywords: undefined };
  handleQuery();
}

/** 加载某 root 下整套层级树（统一 specId/parentId 为字符串便于建树） */
function loadTree(rootId) {
  currentRootId.value = rootId;
  return listPackSpecTree(rootId).then((res) => {
    const rows = (res.rows || []).map((n) => ({
      ...n,
      specId: String(n.specId),
      parentId: n.parentId != null ? String(n.parentId) : null,
      label: `${n.levelName}${n.isBase === 1 ? `（${t('wms.packSpec.base')}）` : ""}`,
    }));
    treeData.value = handleTree(rows, "specId", "parentId");
  });
}

/** 新增：先建顶层 */
function handleAdd() {
  resetNodeForm();
  dialogMode.value = "create";
  treeData.value = [];
  selectedNode.value = null;
  open.value = true;
  title.value = t('wms.packSpec.addTitle');
}

/** 管理层级：加载整棵树 */
function handleUpdate(row) {
  resetNodeForm();
  dialogMode.value = "edit";
  open.value = true;
  title.value = t('wms.packSpec.manageTitle');
  loadTree(row.specId);
}

/** 节点表单（左侧树操作的目标） */
const nodeForm = reactive({
  specId: undefined,
  rootId: undefined,
  parentId: undefined,
  parentName: "",
  levelName: undefined,
  uom: undefined,
  qtyPerParent: undefined,
  isContinuous: 0,
  isBase: 0,
  sortOrder: undefined,
  effectiveDate: undefined,
});
const nodeRef = ref(null);
const nodeRules = {
  levelName: [{ required: true, message: () => t('wms.packSpec.rules.levelNameRequired'), trigger: "blur" }],
};

function resetNodeForm() {
  nodeForm.specId = undefined;
  nodeForm.rootId = undefined;
  nodeForm.parentId = undefined;
  nodeForm.parentName = "";
  nodeForm.levelName = undefined;
  nodeForm.uom = undefined;
  nodeForm.qtyPerParent = undefined;
  nodeForm.isContinuous = 0;
  nodeForm.isBase = 0;
  nodeForm.sortOrder = undefined;
  nodeForm.effectiveDate = undefined;
  if (nodeRef.value) nodeRef.value.resetFields();
}

/** 在树中按 specId 查找节点名称（用于回显上级层级名） */
function findNodeName(list, id) {
  for (const n of list) {
    if (String(n.specId) === String(id)) return n.levelName;
    if (n.children && n.children.length) {
      const r = findNodeName(n.children, id);
      if (r) return r;
    }
  }
  return "";
}

/** 点击树节点：选中并直接载入右侧表单编辑（edit 模式下） */
function onNodeClick(node) {
  selectedNode.value = node;
  if (dialogMode.value === "edit") {
    editNode(node);
  }
}

/** 添加子级：以当前节点为父 */
function addChild(node) {
  resetNodeForm();
  nodeForm.parentId = node.specId;
  nodeForm.rootId = currentRootId.value;
  nodeForm.parentName = node.levelName;
}

/** 编辑本层：把节点载入表单 */
function editNode(node) {
  resetNodeForm();
  nodeForm.specId = node.specId;
  nodeForm.rootId = currentRootId.value;
  nodeForm.parentId = node.parentId;
  nodeForm.parentName = node.parentId ? findNodeName(treeData.value, node.parentId) : t('wms.packSpec.topNoParent');
  nodeForm.levelName = node.levelName;
  nodeForm.uom = node.uom;
  nodeForm.qtyPerParent = node.qtyPerParent;
  nodeForm.isContinuous = node.isContinuous ?? 0;
  nodeForm.isBase = node.isBase ?? 0;
  nodeForm.sortOrder = node.sortOrder;
  nodeForm.effectiveDate = node.effectiveDate;
}

/** 保存节点：新增或更新 */
function saveNode() {
  nodeRef.value.validate((valid) => {
    if (!valid) return;
    const payload = {
      specId: nodeForm.specId ? Number(nodeForm.specId) : 0,
      rootId: nodeForm.rootId ? Number(nodeForm.rootId) : null,
      parentId: nodeForm.parentId ? Number(nodeForm.parentId) : null,
      levelName: nodeForm.levelName,
      uom: nodeForm.uom,
      qtyPerParent: nodeForm.qtyPerParent,
      isContinuous: nodeForm.isContinuous,
      isBase: nodeForm.isBase,
      sortOrder: nodeForm.sortOrder,
      effectiveDate: nodeForm.effectiveDate,
    };
    if (!nodeForm.specId) {
      // 新增：保存后若当前是顶层，自动进入树管理
      addPackSpec(payload).then((res) => {
        ElMessage.success(t('wms.common.savedSuccess'));
        const newId = res.data?.specId;
        if (newId) {
          dialogMode.value = "edit";
          currentRootId.value = newId;
          loadTree(newId);
        }
        getList();
      });
    } else {
      updatePackSpec(payload).then(() => {
        ElMessage.success(t('wms.common.savedSuccess'));
        loadTree(currentRootId.value);
        getList();
      });
    }
  });
}

/** 删除本层 */
function deleteNode(node) {
  ElMessageBox.confirm(t('wms.packSpec.deleteNodeConfirm', { name: node.levelName }), t('wms.common.warning'), {
    confirmButtonText: t('common.confirm'),
    cancelButtonText: t('common.cancel'),
    type: "warning",
  }).then(() => {
    delPackSpec(node.specId).then(() => {
      ElMessage.success(t('wms.common.deleteSuccess'));
      if (!node.parentId) {
        // 删的是顶层：关闭弹窗
        open.value = false;
      } else {
        loadTree(currentRootId.value);
      }
      getList();
    });
  });
}

function handleDelete(row) {
  const delIds = row.specId || ids.value.join(",");
  ElMessageBox.confirm(t('wms.packSpec.deleteTreeConfirm', { name: row.levelName }), t('wms.common.warning'), {
    confirmButtonText: t('common.confirm'),
    cancelButtonText: t('common.cancel'),
    type: "warning",
  })
    .then(() => delPackSpec(delIds))
    .then(() => {
      getList();
      ElMessage.success(t('wms.common.deleteSuccess'));
    });
}

function onDialogClosed() {
  resetNodeForm();
  selectedNode.value = null;
  treeData.value = [];
  currentRootId.value = undefined;
}

function handleSelectionChange(selection) {
  ids.value = selection.map((i) => i.specId);
  single.value = selection.length !== 1;
  multiple.value = !selection.length;
}

getList();
</script>

<style scoped>
.tree-panel {
  border: 1px solid var(--el-border-color);
  border-radius: 4px;
  height: 420px;
  display: flex;
  flex-direction: column;
}
.tree-panel__header {
  padding: 8px 12px;
  background: var(--el-fill-color-light);
  font-size: 13px;
  font-weight: 600;
  border-bottom: 1px solid var(--el-border-color);
}
.tree-panel__body {
  flex: 1;
  overflow: auto;
  padding: 8px 12px;
}
.tree-panel__footer {
  border-top: 1px solid var(--el-border-color);
  padding: 8px 12px;
  display: flex;
  gap: 12px;
}
.level-node {
  display: flex;
  align-items: center;
  gap: 8px;
}
.level-node__name {
  font-weight: 500;
}
.level-node__meta {
  font-size: 12px;
  color: var(--el-text-color-secondary);
}
/* 开关类表单项：文字内联显示，避免长文字换行错乱 */
.compact-switch :deep(.el-form-item__content) {
  white-space: nowrap;
  display: flex;
  align-items: center;
}
.compact-switch :deep(.el-switch) {
  flex-shrink: 0;
}
</style>
