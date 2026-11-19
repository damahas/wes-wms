<template>
  <el-drawer v-model="visible" size="86%" destroy-on-close @opened="init">
    <template #header>
      <div class="ld-header">
        <span>{{ warehouse.warehouseName || "仓库" }} - 平面布局</span>
        <el-tag v-if="layoutId" :type="status === '1' ? 'success' : 'info'" size="small">
          {{ status === "1" ? "已发布" : "草稿" }}
        </el-tag>
      </div>
    </template>

    <div class="ld-wrap" v-loading="loading">
      <!-- 工具栏 -->
      <div class="ld-toolbar">
        <el-radio-group v-model="mode" size="small">
          <el-radio-button value="edit">编辑</el-radio-button>
          <el-radio-button value="preview">预览</el-radio-button>
        </el-radio-group>

        <template v-if="mode === 'edit'">
          <el-divider direction="vertical" />
          <el-radio-group v-model="tool" size="small">
            <el-radio-button value="select">选择</el-radio-button>
            <el-radio-button value="unit">储位</el-radio-button>
            <el-radio-button value="label">标注</el-radio-button>
            <el-radio-button value="region">分区</el-radio-button>
          </el-radio-group>
          <el-divider direction="vertical" />
          <span class="ld-grid-label">行</span>
          <el-input-number v-model="grid.rows" size="small" :min="5" :max="100" controls-position="right" style="width: 90px" />
          <span class="ld-grid-label">列</span>
          <el-input-number v-model="grid.cols" size="small" :min="5" :max="100" controls-position="right" style="width: 90px" />
        </template>

        <div class="ld-toolbar-right">
          <span v-if="mode === 'edit' && childTarget" class="ld-child-hint">
            正在为「{{ childTarget.name }}」添加子块（储位/标注），Esc 取消
          </span>
          <span v-else-if="mode === 'edit' && appendModeId" class="ld-child-hint">
            正在为「{{ appendBlockName }}」追加拼接段（L/U 形），Esc 取消
          </span>
          <span v-else-if="mode === 'edit' && tool !== 'select'" class="ld-hint">
            块内起笔拖拽=拼接，空白起笔=新增；从块外补画可右键块「追加拼接段」
          </span>
          <template v-if="mode === 'edit'">
            <el-button type="primary" size="small" icon="Check" @click="handleSave">保 存</el-button>
            <el-button size="small" icon="Promotion" :disabled="!layoutId" @click="handlePublish">发 布</el-button>
          </template>
        </div>
      </div>

      <!-- 画布 + 属性面板 -->
      <div class="ld-body">
        <div
          ref="canvasRef"
          class="ld-canvas"
          :class="{ preview: mode === 'preview' }"
          @mousedown="onMouseDown"
          @mousemove="onMouseMove"
          @mouseup="onMouseUp"
          @mouseleave="onMouseLeave"
          @click="onPreviewClick"
          @contextmenu.prevent
        >
          <svg :width="svgWidth" :height="svgHeight">
            <rect class="ld-bg" :x="PAD" :y="PAD" :width="grid.cols * CELL" :height="grid.rows * CELL" />
            <path class="ld-grid-line" :d="gridPath" />
            <defs>
              <!-- 分区填充挖洞：子块占用的格子不涂分区背景，避免混色 -->
              <mask v-for="m in regionMasks" :key="m.id" :id="m.id">
                <rect x="0" y="0" :width="svgWidth" :height="svgHeight" fill="white" />
                <rect
                  v-for="(rc, i) in m.holes"
                  :key="i"
                  :x="PAD + (rc.c1 - 1) * CELL"
                  :y="PAD + (rc.r1 - 1) * CELL"
                  :width="(rc.c2 - rc.c1 + 1) * CELL"
                  :height="(rc.r2 - rc.r1 + 1) * CELL"
                  fill="black"
                />
              </mask>
            </defs>
            <g
              v-for="item in renderItems"
              :key="item.block.id"
              class="ld-block"
              :class="{ selected: selectedId === item.block.id, draggable: mode === 'edit' }"
              @mouseenter="onBlockEnter($event, item.block)"
              @mouseleave="onBlockLeave"
              @contextmenu.stop.prevent="onContextMenu($event, item.block)"
            >
              <!-- 填充（分区为浅背景色，无描边） -->
              <rect
                v-for="(rc, i) in offsetRects(item.block)"
                :key="`f${i}`"
                :x="PAD + (rc.c1 - 1) * CELL"
                :y="PAD + (rc.r1 - 1) * CELL"
                :width="(rc.c2 - rc.c1 + 1) * CELL"
                :height="(rc.r2 - rc.r1 + 1) * CELL"
                :fill="blockColor(item.block)"
                :fill-opacity="fillOpacity(item.block)"
                :mask="regionMask(item.block)"
              />
              <!-- 外边界描边（相邻格子交界处不画线，整体呈现为一个形状） -->
              <path
                :d="boundaryPath(item.block)"
                fill="none"
                :stroke="selectedId === item.block.id ? '#FF7A45' : blockColor(item.block)"
                :stroke-width="selectedId === item.block.id ? 2 : 1.2"
              />
              <text
                v-if="showText(item.block)"
                class="ld-block-text"
                :x="textX(item.block)"
                :y="textY(item.block)"
              >{{ item.block.name }}</text>
            </g>
            <rect
              v-if="rubber"
              class="ld-rubber"
              :class="`rubber-${tool}`"
              :x="PAD + (Math.min(rubber.c1, rubber.c2) - 1) * CELL"
              :y="PAD + (Math.min(rubber.r1, rubber.r2) - 1) * CELL"
              :width="(Math.abs(rubber.c2 - rubber.c1) + 1) * CELL"
              :height="(Math.abs(rubber.r2 - rubber.r1) + 1) * CELL"
            />
          </svg>

          <!-- 悬浮提示（预览模式） -->
          <div
            v-if="mode === 'preview' && hover.show"
            class="ld-tooltip"
            :style="{ left: hover.x + 14 + 'px', top: hover.y + 14 + 'px' }"
          >
            <div class="ld-tip-name">{{ hover.block?.name }}</div>
            <div class="ld-tip-row">层级：{{ derivedLabel(hover.block) }}</div>
            <div v-if="hover.block?.refCode" class="ld-tip-row">绑定：{{ hover.block.refCode }} {{ hover.block.refName }}</div>
            <div v-if="hover.block?.children?.length" class="ld-tip-row">子块：{{ hover.block.children.length }} 个</div>
          </div>

          <!-- 右键菜单 -->
          <div
            v-if="menu.show"
            class="ld-menu"
            :style="{ left: menu.x + 'px', top: menu.y + 'px' }"
          >
            <div class="ld-menu-title">{{ menu.block?.name }}</div>
            <div class="ld-menu-item" @click="renameBlock">重命名</div>
            <div v-if="menu.block?.type !== 'label'" class="ld-menu-item" @click="appendSegment">追加拼接段（L/U 形）</div>
            <div v-if="menu.block?.type === 'region'" class="ld-menu-item" @click="addChild">添加储位</div>
            <div
              v-if="menu.block?.type === 'region'"
              class="ld-menu-item"
              :class="{ disabled: !menu.block?.children?.length }"
              @click="mergeChildren"
            >清空子块</div>
            <div class="ld-menu-item danger" @click="removeBlock">删除</div>
          </div>
        </div>

        <!-- 属性面板 -->
        <div v-if="mode === 'edit'" class="ld-props">
          <template v-if="selected">
            <div class="ld-props-title">
              属性
              <el-tag size="small" :type="selected.type === 'region' ? 'primary' : selected.type === 'label' ? 'warning' : 'success'">
                {{ derivedLabel(selected) }}
              </el-tag>
            </div>
            <el-form label-width="60px" size="small" @submit.prevent>
              <el-form-item label="名称">
                <el-input v-model="selected.name" maxlength="50" />
              </el-form-item>
              <el-form-item v-if="selected.type !== 'label'" label="绑定">
                <el-select v-model="selected.refType" placeholder="不绑定" clearable style="width: 100%" @change="onRefTypeChange">
                  <el-option label="库区" value="zone" />
                  <el-option label="库位" value="location" />
                </el-select>
              </el-form-item>
              <el-form-item v-if="selected.type !== 'label' && selected.refType" label="实体">
                <el-select
                  :model-value="selected.refId ? String(selected.refId) : ''"
                  filterable
                  placeholder="请选择"
                  style="width: 100%"
                  @change="onRefChange"
                >
                  <el-option
                    v-for="op in refOptions"
                    :key="op.value"
                    :label="op.label"
                    :value="op.value"
                  />
                </el-select>
              </el-form-item>
              <el-form-item label="颜色">
                <el-color-picker v-model="selected.color" />
                <span v-if="selected.type === 'region'" class="ld-color-tip">分区内空白处填充浅色</span>
              </el-form-item>
              <el-form-item v-if="selected.children?.length" label="子块">
                <span>{{ selected.children.length }} 个</span>
              </el-form-item>
              <el-form-item v-if="selected.rects.length > 1" label="形状">
                <span>{{ selected.rects.length }} 个矩形拼接</span>
              </el-form-item>
            </el-form>
            <el-button type="danger" size="small" plain icon="Delete" @click="removeSelected">删除该块</el-button>
          </template>
          <el-empty v-else description="点击画布中的块查看属性" :image-size="80" />
        </div>
      </div>

      <!-- 图例 -->
      <div class="ld-legend">
        <span class="ld-legend-item"><i class="dot" style="background: rgba(64, 158, 255, .3)" />分区（容器）</span>
        <span class="ld-legend-item"><i class="dot" style="background: rgba(103, 194, 58, .55)" />储位</span>
        <span class="ld-legend-item"><i class="dot" style="background: rgba(230, 162, 60, .55)" />标注</span>
        <span class="ld-legend-stat">
          共 {{ countBlocks().total }} 块：分区 {{ countBlocks().regions }} / 储位 {{ countBlocks().units }} / 标注 {{ countBlocks().labels }}
        </span>
      </div>
    </div>

    <!-- 储位库存明细（预览模式点击储位） -->
    <el-dialog
      v-model="stockDlg.show"
      :title="stockDlg.block ? `${stockDlg.block.name}${stockDlg.block.refCode ? '（' + stockDlg.block.refCode + '）' : ''} 库存明细` : '库存明细'"
      width="880px"
      append-to-body
    >
      <el-table v-loading="stockDlg.loading" :data="stockDlg.rows" max-height="420">
        <el-table-column label="物料编码" prop="materialCode" width="120" align="center" show-overflow-tooltip />
        <el-table-column label="物料名称" prop="materialName" min-width="150" align="center" show-overflow-tooltip />
        <el-table-column label="批次" prop="batchNo" width="110" align="center">
          <template #default="s">{{ s.row.batchNo || "-" }}</template>
        </el-table-column>
        <el-table-column label="库存" prop="stockQuantity" width="80" align="center" />
        <el-table-column label="冻结" prop="freezeQuantity" width="80" align="center">
          <template #default="s">
            <span :style="{ color: (s.row.freezeQuantity ?? 0) > 0 ? 'var(--el-color-danger)' : '' }">{{ s.row.freezeQuantity ?? 0 }}</span>
          </template>
        </el-table-column>
        <el-table-column label="锁定" prop="lockQuantity" width="80" align="center">
          <template #default="s">
            <span :style="{ color: (s.row.lockQuantity ?? 0) > 0 ? 'var(--el-color-warning)' : '' }">{{ s.row.lockQuantity ?? 0 }}</span>
          </template>
        </el-table-column>
        <el-table-column label="可用" width="90" align="center">
          <template #default="s">
            <span style="color: var(--el-color-success)">{{ (s.row.stockQuantity ?? 0) - (s.row.freezeQuantity ?? 0) - (s.row.lockQuantity ?? 0) }}</span>
          </template>
        </el-table-column>
        <el-table-column label="有效期至" width="110" align="center">
          <template #default="s">{{ s.row.expireDate ? String(s.row.expireDate).slice(0, 10) : "-" }}</template>
        </el-table-column>
        <template #empty>该库位暂无库存</template>
      </el-table>
    </el-dialog>
  </el-drawer>
</template>

<script setup name="WmsLayoutDesigner">
import { ref, reactive, computed, watch, onMounted, onUnmounted } from "vue";
import { ElMessage, ElMessageBox } from "element-plus";
import { getWarehouseLayout, saveWarehouseLayout, publishWarehouseLayout } from "@/api/wms/warehouseLayout";
import { listZone } from "@/api/wms/zone";
import { listLocation } from "@/api/wms/location";
import { listStock } from "@/api/wms/stock";

const props = defineProps({
  modelValue: Boolean,
  warehouse: { type: Object, default: () => ({}) }
});
const emit = defineEmits(["update:modelValue"]);

const CELL = 26;
const PAD = 20;
const DEFAULT_COLOR = { region: "#409EFF", unit: "#67C23A", label: "#E6A23C" };

const visible = computed({
  get: () => props.modelValue,
  set: (v) => emit("update:modelValue", v)
});

const loading = ref(false);
const layoutId = ref(0);
const status = ref("0");
const mode = ref("edit");
const tool = ref("select");
const grid = reactive({ rows: 25, cols: 40 });
const blocks = ref([]);
const selectedId = ref(null);
const childTargetId = ref(null);
const canvasRef = ref(null);

const zoneOptions = ref([]);
const locationOptions = ref([]);

const rubber = ref(null);
const appendTargetId = ref(null);
/** 显式拼接模式：右键「追加拼接段」进入，之后任意拖拽均并入该块 */
const appendModeId = ref(null);
const appendBlockName = computed(() => findBlock(blocks.value, appendModeId.value)?.name || "");
const drag = ref(null);
const menu = reactive({ show: false, x: 0, y: 0, block: null });
const hover = reactive({ show: false, x: 0, y: 0, block: null });

const svgWidth = computed(() => grid.cols * CELL + PAD * 2);
const svgHeight = computed(() => grid.rows * CELL + PAD * 2);

const selected = computed(() => findBlock(blocks.value, selectedId.value));
const childTarget = computed(() => findBlock(blocks.value, childTargetId.value));

const gridPath = computed(() => {
  const v = [];
  const h = [];
  for (let c = 0; c <= grid.cols; c++) v.push(`M${PAD + c * CELL} ${PAD}V${PAD + grid.rows * CELL}`);
  for (let r = 0; r <= grid.rows; r++) h.push(`M${PAD} ${PAD + r * CELL}H${PAD + grid.cols * CELL}`);
  return [...v, ...h].join(" ");
});

/** 深度优先展开（父块先渲染，子块叠在上面） */
const renderItems = computed(() => {
  // 分区是容器/背景层，永远排在最前（画在底层），储位/标注盖在其上
  const regions = [];
  const rest = [];
  const walk = (list, depth) => {
    (list || []).forEach((b) => {
      (b.type === "region" ? regions : rest).push({ block: b, depth });
      walk(b.children, depth + 1);
    });
  };
  walk(blocks.value, 0);
  return [...regions, ...rest];
});

function findBlock(list, id) {
  for (const b of list || []) {
    if (b.id === id) return b;
    const found = findBlock(b.children, id);
    if (found) return found;
  }
  return null;
}

function findParent(list, id, parent = null) {
  for (const b of list || []) {
    if (b.id === id) return parent;
    const found = findParent(b.children, id, b);
    if (found) return found;
  }
  return null;
}

function removeById(list, id) {
  const idx = (list || []).findIndex((b) => b.id === id);
  if (idx >= 0) {
    list.splice(idx, 1);
    return true;
  }
  return (list || []).some((b) => removeById(b.children, id));
}

function rectsIntersect(a, b) {
  return a.r1 <= b.r2 && a.r2 >= b.r1 && a.c1 <= b.c2 && a.c2 >= b.c1;
}

function unionBbox(block) {
  const rects = block.rects || [];
  return {
    r1: Math.min(...rects.map((x) => x.r1)),
    r2: Math.max(...rects.map((x) => x.r2)),
    c1: Math.min(...rects.map((x) => x.c1)),
    c2: Math.max(...rects.map((x) => x.c2))
  };
}

function normRect(r) {
  return {
    r1: Math.min(r.r1, r.r2),
    r2: Math.max(r.r1, r.r2),
    c1: Math.min(r.c1, r.c2),
    c2: Math.max(r.c1, r.c2)
  };
}

/** 矩形集合覆盖的格子集合 */
function rectCellSet(rects) {
  const cells = new Set();
  (rects || []).forEach((rc) => {
    for (let r = rc.r1; r <= rc.r2; r++) {
      for (let c = rc.c1; c <= rc.c2; c++) cells.add(`${r},${c}`);
    }
  });
  return cells;
}

/** 矩形是否完全被已有格子覆盖 */
function rectFullyCovered(rect, cells) {
  for (let r = rect.r1; r <= rect.r2; r++) {
    for (let c = rect.c1; c <= rect.c2; c++) {
      if (!cells.has(`${r},${c}`)) return false;
    }
  }
  return true;
}

/** 将矩形集合规范化为无重叠的最小矩形分解（按行贪心合并），避免重叠格子双重着色 */
function normalizeRects(rects) {
  const rowCells = new Map();
  (rects || []).forEach((rc) => {
    for (let r = rc.r1; r <= rc.r2; r++) {
      if (!rowCells.has(r)) rowCells.set(r, new Set());
      for (let c = rc.c1; c <= rc.c2; c++) rowCells.get(r).add(c);
    }
  });
  const rows = [...rowCells.keys()].sort((a, b) => a - b);
  const consumed = new Set();
  const out = [];
  for (const r of rows) {
    const cs = [...rowCells.get(r)].sort((a, b) => a - b).filter((c) => !consumed.has(`${r},${c}`));
    let i = 0;
    while (i < cs.length) {
      const c0 = cs[i];
      let cEnd = c0;
      while (cs.includes(cEnd + 1)) cEnd++;
      let rEnd = r;
      while (rowCells.has(rEnd + 1)) {
        const next = rowCells.get(rEnd + 1);
        let ok = true;
        for (let c = c0; c <= cEnd; c++) {
          if (!next.has(c) || consumed.has(`${rEnd + 1},${c}`)) {
            ok = false;
            break;
          }
        }
        if (!ok) break;
        rEnd++;
      }
      out.push({ r1: r, r2: rEnd, c1: c0, c2: cEnd });
      for (let rr = r; rr <= rEnd; rr++) {
        for (let c = c0; c <= cEnd; c++) consumed.add(`${rr},${c}`);
      }
      i += cEnd - c0 + 1;
    }
  }
  return out;
}

function rectInside(outer, inner) {
  return inner.r1 >= outer.r1 && inner.r2 <= outer.r2 && inner.c1 >= outer.c1 && inner.c2 <= outer.c2;
}

/**
 * 收集参与碰撞检测的矩形：跳过 skipId 及其子树；分区块作为标注层不参与碰撞（允许被覆盖/重叠）
 */
function conflictRects(skipId) {
  const rects = [];
  const walk = (list) => {
    (list || []).forEach((b) => {
      if (b.id === skipId) return;
      if (b.type !== "region") rects.push(...(b.rects || []));
      walk(b.children);
    });
  };
  walk(blocks.value);
  return rects;
}

function overlapAny(rects, others) {
  return rects.some((n) => others.some((rc) => rectsIntersect(n, rc)));
}

function posToCell(evt) {
  const rect = canvasRef.value.getBoundingClientRect();
  const x = evt.clientX - rect.left - PAD;
  const y = evt.clientY - rect.top - PAD;
  const c = Math.max(1, Math.min(grid.cols, Math.floor(x / CELL) + 1));
  const r = Math.max(1, Math.min(grid.rows, Math.floor(y / CELL) + 1));
  return { r, c };
}

function hitBlock(r, c) {
  // 分区是容器/标注层，永远不挡点击：命中多个块时优先返回储位/标注
  const items = [...renderItems.value].reverse();
  let region = null;
  for (const { block } of items) {
    if (!(block.rects || []).some((rc) => r >= rc.r1 && r <= rc.r2 && c >= rc.c1 && c <= rc.c2)) continue;
    if (block.type !== "region") return block;
    if (!region) region = block;
  }
  return region;
}

let blockSeq = Date.now() % 1000000;
function nextId() {
  return `b${blockSeq++}`;
}

function createBlock(type, rect) {
  const counts = countBlocks();
  const nameMap = { unit: "储位", region: "分区", label: "标注" };
  const statKey = type === "unit" ? "units" : type === "region" ? "regions" : "labels";
  return {
    id: nextId(),
    type,
    name: `${nameMap[type]}${counts[statKey] + 1}`,
    rects: [rect],
    refType: null,
    refId: null,
    refCode: "",
    refName: "",
    color: DEFAULT_COLOR[type],
    children: []
  };
}

function onMouseDown(evt) {
  if (evt.button !== 0 || mode.value !== "edit") return;
  menu.show = false;
  const { r, c } = posToCell(evt);

  if (tool.value === "select") {
    const b = hitBlock(r, c);
    if (b) {
      selectedId.value = b.id;
      drag.value = {
        blockId: b.id,
        startR: r,
        startC: c,
        origRects: b.rects.map((x) => ({ ...x })),
        dx: 0,
        dy: 0
      };
    } else {
      selectedId.value = null;
    }
    return;
  }

  if (tool.value === "label") {
    const parent = childTarget.value;
    const rect = { r1: r, r2: r, c1: c, c2: c };
    if (parent && !rectInside(unionBbox(parent), rect)) {
      ElMessage.warning("子块必须落在父块范围内");
      return;
    }
    if (overlapAny([rect], conflictRects(null))) {
      // 已被占用：选中该块并在右侧显示属性面板，便于编辑
      const hit = hitBlock(r, c);
      if (hit) selectedId.value = hit.id;
      return;
    }
    (parent ? parent.children : blocks.value).push(createBlock("label", rect));
    return;
  }

  const hit = hitBlock(r, c);
  if (appendModeId.value) {
    // 显式拼接模式：任意位置起笔都并入指定块
    appendTargetId.value = appendModeId.value;
  } else if (hit && hit.type === tool.value && !childTarget.value) {
    appendTargetId.value = hit.id;
  } else {
    appendTargetId.value = null;
  }
  rubber.value = { r1: r, c1: c, r2: r, c2: c };
}

function onMouseMove(evt) {
  const { r, c } = posToCell(evt);
  if (rubber.value) {
    rubber.value.r2 = r;
    rubber.value.c2 = c;
  }
  if (drag.value) {
    drag.value.dx = c - drag.value.startC;
    drag.value.dy = r - drag.value.startR;
  }
}

function onMouseUp() {
  if (rubber.value) finishDraw();
  if (drag.value) finishMove();
}

function onMouseLeave() {
  if (rubber.value) finishDraw();
  if (drag.value) finishMove();
}

function finishDraw() {
  const rect = normRect(rubber.value);
  rubber.value = null;
  const target = appendTargetId.value ? findBlock(blocks.value, appendTargetId.value) : null;
  appendTargetId.value = null;
  appendModeId.value = null; // 拼接模式一次性消费，本次拖拽结束后退出

  // 单击已有块：直接选中并在右侧显示属性面板，便于编辑（拼接模式下除外，单击=追加 1 格）
  if (!appendTargetId.value && rect.r1 === rect.r2 && rect.c1 === rect.c2) {
    const hit = hitBlock(rect.r1, rect.c1);
    if (hit) {
      selectedId.value = hit.id;
      return;
    }
  }

  if (target) {
    // 追加矩形到已有块（拼接 L/U 形）
    const parent = findParent(blocks.value, target.id);
    if (parent && !rectInside(unionBbox(parent), rect)) {
      ElMessage.warning("超出所在分区范围，已取消");
      return;
    }
    if (rectFullyCovered(rect, rectCellSet(target.rects))) {
      // 拖拽范围完全在块内：同样视为选中该块
      selectedId.value = target.id;
      return;
    }
    if (target.type !== "region" && overlapAny([rect], conflictRects(target.id))) {
      ElMessage.warning("与相邻块重叠，已取消");
      return;
    }
    // 规范化矩形：消除与自身的重叠，保证填充不叠加
    target.rects = normalizeRects([...target.rects, rect]);
    return;
  }

  const parent = childTarget.value;
  const parentBbox = parent ? unionBbox(parent) : null;
  if (parent && !rectInside(parentBbox, rect)) {
    ElMessage.warning("子块必须落在父块范围内");
    return;
  }
  // 分区是标注层，允许与其他块重叠；储位/标注之间不允许
  if (tool.value !== "region" && overlapAny([rect], conflictRects(null))) {
    ElMessage.warning("与已有块重叠，请重新绘制");
    return;
  }
  const list = parent ? parent.children : blocks.value;
  const block = createBlock(tool.value, rect);
  list.push(block);
  selectedId.value = block.id;
}

function finishMove() {
  const { blockId, origRects, dx, dy } = drag.value;
  drag.value = null;
  if (!dx && !dy) return;
  const block = findBlock(blocks.value, blockId);
  if (!block) return;

  const minR = Math.min(...origRects.map((x) => x.r1));
  const maxR = Math.max(...origRects.map((x) => x.r2));
  const minC = Math.min(...origRects.map((x) => x.c1));
  const maxC = Math.max(...origRects.map((x) => x.c2));
  let ndy = Math.max(dy, 1 - minR);
  ndy = Math.min(ndy, grid.rows - maxR);
  let ndx = Math.max(dx, 1 - minC);
  ndx = Math.min(ndx, grid.cols - maxC);
  const parent = findParent(blocks.value, blockId);
  if (parent) {
    const pb = unionBbox(parent);
    ndy = Math.max(ndy, pb.r1 - minR);
    ndy = Math.min(ndy, pb.r2 - maxR);
    ndx = Math.max(ndx, pb.c1 - minC);
    ndx = Math.min(ndx, pb.c2 - maxC);
  }
  const moved = origRects.map((x) => ({
    r1: x.r1 + ndy,
    r2: x.r2 + ndy,
    c1: x.c1 + ndx,
    c2: x.c2 + ndx
  }));
  if (block.type !== "region" && overlapAny(moved, conflictRects(blockId))) {
    ElMessage.warning("目标位置与其他块重叠，已回退");
    return;
  }
  block.rects = moved;
}

function offsetRects(block) {
  if (drag.value && drag.value.blockId === block.id) {
    const { dx, dy } = drag.value;
    return block.rects.map((x) => ({ r1: x.r1 + dy, r2: x.r2 + dy, c1: x.c1 + dx, c2: x.c2 + dx }));
  }
  return block.rects;
}

function showText(block) {
  // 与 textX/textY 同源（offsetRects），rects 为空/缺字段时坐标为 NaN，直接不渲染
  const b = unionBbox(offsetRects(block));
  if (!Number.isFinite(b.r1) || !Number.isFinite(b.c1) || !Number.isFinite(b.r2) || !Number.isFinite(b.c2)) return false;
  if (block.type === "label") return true;
  return b.r2 - b.r1 >= 1 && b.c2 - b.c1 >= 2;
}

function textX(block) {
  const b = unionBbox(offsetRects(block));
  return PAD + (b.c1 - 1 + (b.c2 - b.c1 + 1) / 2) * CELL;
}

function textY(block) {
  const b = unionBbox(offsetRects(block));
  return PAD + (b.r1 - 1 + (b.r2 - b.r1 + 1) / 2) * CELL + 4;
}

function blockColor(block) {
  return block.color || DEFAULT_COLOR[block.type] || "#409EFF";
}

function fillOpacity(block) {
  return block.type === "region" ? 0.3 : 0.55;
}

/** 分区挖洞 mask：所有与分区重叠的储位/标注格子（含子块与平级独立块）都不涂分区背景 */
const regionMasks = computed(() => {
  const solid = [];
  renderItems.value.forEach((it) => {
    if (it.block.type !== "region") solid.push(...(it.block.rects || []));
  });
  if (!solid.length) return [];
  return renderItems.value
    .filter((it) => it.block.type === "region")
    .map((it) => ({ id: `ldm-${it.block.id}`, holes: solid }));
});

function regionMask(block) {
  if (block.type !== "region") return undefined;
  return regionMasks.value.some((m) => m.id === `ldm-${block.id}`) ? `url(#ldm-${block.id})` : undefined;
}

/** 块的外边界描边路径：相邻格子交界处不画线，整体呈现为一个形状 */
function boundaryPath(block) {
  const rects = offsetRects(block) || [];
  const cells = new Set();
  rects.forEach((rc) => {
    for (let r = rc.r1; r <= rc.r2; r++) {
      for (let c = rc.c1; c <= rc.c2; c++) cells.add(`${r},${c}`);
    }
  });
  const has = (r, c) => cells.has(`${r},${c}`);
  const d = [];
  rects.forEach((rc) => {
    for (let c = rc.c1; c <= rc.c2; c++) {
      if (!has(rc.r1 - 1, c)) d.push(`M${PAD + (c - 1) * CELL} ${PAD + (rc.r1 - 1) * CELL}h${CELL}`);
      if (!has(rc.r2 + 1, c)) d.push(`M${PAD + (c - 1) * CELL} ${PAD + rc.r2 * CELL}h${CELL}`);
    }
    for (let r = rc.r1; r <= rc.r2; r++) {
      if (!has(r, rc.c1 - 1)) d.push(`M${PAD + (rc.c1 - 1) * CELL} ${PAD + (r - 1) * CELL}v${CELL}`);
      if (!has(r, rc.c2 + 1)) d.push(`M${PAD + rc.c2 * CELL} ${PAD + (r - 1) * CELL}v${CELL}`);
    }
  });
  return d.join(" ");
}

/** 业务语义（属性面板 badge / 预览提示）：分区 = 库区，储位 = 库位，标注 = 文字说明 */
function derivedLabel(block) {
  if (!block) return "";
  if (block.type === "region") return "库区";
  if (block.type === "label") return "标注";
  return "库位";
}

function countBlocks() {
  const stat = { total: 0, regions: 0, units: 0, labels: 0 };
  const walk = (list) => {
    (list || []).forEach((b) => {
      stat.total++;
      if (b.type === "region") stat.regions++;
      else if (b.type === "label") stat.labels++;
      else stat.units++;
      walk(b.children);
    });
  };
  walk(blocks.value);
  return stat;
}

function onBlockEnter(evt, block) {
  if (mode.value !== "preview") return;
  hover.show = true;
  hover.x = evt.clientX;
  hover.y = evt.clientY;
  hover.block = block;
}

function onBlockLeave() {
  hover.show = false;
  hover.block = null;
}

function onContextMenu(evt, block) {
  if (mode.value !== "edit") return;
  menu.show = true;
  const rect = canvasRef.value.getBoundingClientRect();
  menu.x = evt.clientX - rect.left;
  menu.y = evt.clientY - rect.top;
  menu.block = block;
  selectedId.value = block.id;
}

function renameBlock() {
  const block = menu.block;
  menu.show = false;
  ElMessageBox.prompt("请输入名称", "重命名", {
    inputValue: block.name,
    confirmButtonText: "确定",
    cancelButtonText: "取消"
  })
    .then(({ value }) => {
      block.name = value || block.name;
    })
    .catch(() => {});
}

/** 进入显式拼接模式：之后任意位置拖拽均并入该块，Esc 或完成一次拼接后退出 */
function appendSegment() {
  appendModeId.value = menu.block.id;
  if (tool.value === "select" || tool.value === "label") {
    tool.value = menu.block.type === "region" ? "region" : "unit";
  }
  menu.show = false;
  ElMessage.info(`从任意位置拖拽即可为「${menu.block.name}」追加拼接段，Esc 取消`);
}

function addChild() {
  childTargetId.value = menu.block.id;
  tool.value = "unit";
  menu.show = false;
  ElMessage.info("在分区内拖拽绘制储位，也可切换为标注工具，Esc 取消");
}

function mergeChildren() {
  const block = menu.block;
  menu.show = false;
  if (!block.children || !block.children.length) return;
  ElMessageBox.confirm(`清空后「${block.name}」的 ${block.children.length} 个子块将被删除，是否继续？`, "提示", {
    type: "warning"
  })
    .then(() => {
      block.children = [];
    })
    .catch(() => {});
}

function removeBlock() {
  const block = menu.block;
  menu.show = false;
  ElMessageBox.confirm(`确定删除「${block.name}」及其子块吗？`, "提示", { type: "warning" })
    .then(() => {
      removeById(blocks.value, block.id);
      if (selectedId.value === block.id) selectedId.value = null;
      if (childTargetId.value === block.id) childTargetId.value = null;
    })
    .catch(() => {});
}

function removeSelected() {
  if (!selected.value) return;
  const block = selected.value;
  ElMessageBox.confirm(`确定删除「${block.name}」及其子块吗？`, "提示", { type: "warning" })
    .then(() => {
      removeById(blocks.value, block.id);
      selectedId.value = null;
    })
    .catch(() => {});
}

function onRefTypeChange() {
  const block = selected.value;
  if (!block) return;
  block.refId = null;
  block.refCode = "";
  block.refName = "";
}

function onRefChange(val) {
  const block = selected.value;
  if (!block) return;
  const opt = (block.refType === "zone" ? zoneOptions.value : locationOptions.value).find((x) => String(x.id) === val);
  if (opt) {
    block.refId = opt.id;
    block.refCode = opt.code;
    block.refName = opt.name;
    // 绑定实体后块名自动同步为实体名，画布与业务实体一一对应
    block.name = opt.name || opt.code;
  }
}

const refOptions = computed(() =>
  (selected.value?.refType === "zone" ? zoneOptions.value : locationOptions.value).map((x) => ({
    value: String(x.id),
    label: `${x.code} ${x.name}`
  }))
);

/** 预览模式：点击储位查看其绑定库位的库存明细 */
const stockDlg = reactive({ show: false, loading: false, rows: [], block: null });

function onPreviewClick(evt) {
  if (mode.value !== "preview") return;
  const { r, c } = posToCell(evt);
  const hit = hitBlock(r, c);
  // 储位/分区均可点击；绑定库位按库位查，绑定库区按库区查其下所有库位的库存
  if (!hit || (hit.type !== "unit" && hit.type !== "region")) return;
  if (!hit.refId || (hit.refType !== "location" && hit.refType !== "zone")) {
    ElMessage.info(`「${hit.name}」未绑定库位/库区，暂无库存明细`);
    return;
  }
  stockDlg.block = hit;
  stockDlg.show = true;
  stockDlg.loading = true;
  stockDlg.rows = [];
  const params = hit.refType === "zone" ? { zoneId: hit.refId } : { locationId: hit.refId };
  listStock({ pageNum: 1, pageSize: 9999, params })
    .then((res) => {
      stockDlg.rows = res.rows || [];
    })
    .finally(() => {
      stockDlg.loading = false;
    });
}

/** 兼容旧数据：area→unit，丢弃走道 */
function migrateBlocks(list) {
  const out = [];
  (list || []).forEach((b) => {
    if (b.type === "aisle") return;
    // 修正 r1>r2 等脏矩形，剔除无效项；整块无有效矩形则丢弃（避免 NaN 渲染异常）
    const rects = normalizeRects((b.rects || []).map(normRect));
    if (!rects.length) return;
    out.push({
      ...b,
      type: b.type === "area" ? "unit" : b.type,
      rects,
      // 标注仅是文字说明，清除历史遗留的实体绑定
      ...(b.type === "label" ? { refType: null, refId: null, refCode: "", refName: "" } : {}),
      children: migrateBlocks(b.children)
    });
  });
  return out;
}

async function init() {
  loading.value = true;
  selectedId.value = null;
  childTargetId.value = null;
  menu.show = false;
  mode.value = "edit";
  tool.value = "select";
  try {
    const [layoutRes, zoneRes, locRes] = await Promise.all([
      getWarehouseLayout(props.warehouse.warehouseId),
      listZone({ pageNum: 1, pageSize: 0, warehouseId: props.warehouse.warehouseId }),
      listLocation({ pageNum: 1, pageSize: 0, warehouseId: props.warehouse.warehouseId })
    ]);
    const layout = layoutRes.data || {};
    layoutId.value = layout.layoutId || 0;
    status.value = layout.status || "0";
    if (layout.canvasJson) {
      const canvas = JSON.parse(layout.canvasJson);
      grid.rows = canvas.grid?.rows || 25;
      grid.cols = canvas.grid?.cols || 40;
      blocks.value = migrateBlocks(canvas.blocks || []);
    } else {
      grid.rows = 25;
      grid.cols = 40;
      blocks.value = [];
    }
    zoneOptions.value = (zoneRes.rows || []).map((z) => ({ id: z.zoneId, code: z.zoneCode, name: z.zoneName }));
    locationOptions.value = (locRes.rows || []).map((l) => ({ id: l.locationId, code: l.locationCode, name: l.locationName }));
    // 画布上没有任何块：直接进编辑模式引导绘制；已有内容则进预览模式（点击储位可看库存）
    mode.value = blocks.value.length ? "preview" : "edit";
  } catch (e) {
    // 接口失败由全局拦截器提示，保持空画布
  } finally {
    loading.value = false;
  }
}

async function handleSave() {
  try {
    const res = await saveWarehouseLayout({
      layoutId: layoutId.value,
      warehouseId: props.warehouse.warehouseId,
      canvasJson: JSON.stringify({ grid: { rows: grid.rows, cols: grid.cols }, blocks: blocks.value })
    });
    if (res.data) layoutId.value = res.data;
    await init();
  } catch (e) {
    // 请求失败由全局拦截器提示
  }
}

async function handlePublish() {
  await publishWarehouseLayout(layoutId.value);
  status.value = "1";
  ElMessage.success("发布成功");
}

function onKeydown(e) {
  if (!visible.value || mode.value !== "edit") return;
  if (e.key === "Escape") {
    childTargetId.value = null;
    appendModeId.value = null;
    appendTargetId.value = null;
    menu.show = false;
    selectedId.value = null;
    rubber.value = null;
    drag.value = null;
  }
  if ((e.key === "Delete" || e.key === "Backspace") && selected.value) {
    const tag = document.activeElement?.tagName;
    if (tag === "INPUT" || tag === "TEXTAREA") return;
    removeById(blocks.value, selected.value.id);
    selectedId.value = null;
  }
}

watch(mode, () => {
  menu.show = false;
  hover.show = false;
  selectedId.value = null;
  childTargetId.value = null;
  rubber.value = null;
  drag.value = null;
});

watch(tool, () => {
  appendTargetId.value = null;
  appendModeId.value = null;
});

onMounted(() => window.addEventListener("keydown", onKeydown));
onUnmounted(() => window.removeEventListener("keydown", onKeydown));
</script>

<style scoped>
.ld-header {
  display: flex;
  align-items: center;
  gap: 8px;
}
.ld-wrap {
  display: flex;
  flex-direction: column;
  height: 100%;
}
.ld-toolbar {
  display: flex;
  align-items: center;
  gap: 10px;
  padding-bottom: 10px;
  flex-wrap: wrap;
}
.ld-toolbar-right {
  margin-left: auto;
  display: flex;
  align-items: center;
  gap: 10px;
}
.ld-grid-label {
  font-size: 13px;
  color: var(--el-text-color-regular);
}
.ld-hint,
.ld-child-hint {
  font-size: 12px;
  color: var(--el-text-color-secondary);
}
.ld-child-hint {
  color: var(--el-color-primary);
}
.ld-body {
  flex: 1;
  display: flex;
  gap: 12px;
  overflow: hidden;
}
.ld-canvas {
  flex: 1;
  overflow: auto;
  position: relative;
  background: var(--el-fill-color-lighter);
  border-radius: 6px;
  padding: 8px;
}
.ld-canvas :deep(svg) {
  display: block;
}
.ld-bg {
  fill: var(--el-bg-color);
  stroke: var(--el-border-color);
}
.ld-grid-line {
  fill: none;
  stroke: var(--el-border-color-lighter);
  stroke-width: 1;
}
.ld-block {
  cursor: default;
}
.ld-canvas:not(.preview) .ld-block {
  cursor: pointer;
}
/* 预览模式：储位可点击查看库存明细 */
.ld-canvas.preview .ld-block {
  cursor: pointer;
}
/* 拖拽选择预览：浅色同类型背景（透明度）+ 边框 */
.ld-rubber {
  fill: rgba(103, 194, 58, 0.3);
  stroke: var(--el-color-success);
  stroke-width: 1.5;
}
.ld-rubber.rubber-region {
  fill: rgba(64, 158, 255, 0.3);
  stroke: var(--el-color-primary);
}
.ld-block-text {
  font-size: 12px;
  fill: var(--el-text-color-primary);
  text-anchor: middle;
  pointer-events: none;
  user-select: none;
}
.ld-tooltip {
  position: fixed;
  z-index: 10;
  background: var(--el-bg-color-overlay);
  border: 1px solid var(--el-border-color-light);
  border-radius: 4px;
  padding: 8px 10px;
  box-shadow: var(--el-box-shadow-light);
  pointer-events: none;
  font-size: 12px;
  min-width: 140px;
}
.ld-tip-name {
  font-weight: 600;
  margin-bottom: 4px;
}
.ld-tip-row {
  color: var(--el-text-color-regular);
  line-height: 1.8;
}
.ld-menu {
  position: absolute;
  z-index: 20;
  background: var(--el-bg-color-overlay);
  border: 1px solid var(--el-border-color-light);
  border-radius: 4px;
  box-shadow: var(--el-box-shadow-light);
  min-width: 130px;
  padding: 4px 0;
}
.ld-menu-title {
  padding: 6px 12px;
  font-size: 12px;
  color: var(--el-text-color-secondary);
  border-bottom: 1px solid var(--el-border-color-lighter);
  margin-bottom: 4px;
}
.ld-menu-item {
  padding: 7px 12px;
  font-size: 13px;
  cursor: pointer;
}
.ld-menu-item:hover {
  background: var(--el-fill-color-light);
}
.ld-menu-item.danger {
  color: var(--el-color-danger);
}
.ld-menu-item.disabled {
  color: var(--el-text-color-placeholder);
  cursor: not-allowed;
}
.ld-props {
  width: 270px;
  flex-shrink: 0;
  border-left: 1px solid var(--el-border-color-lighter);
  padding-left: 12px;
  overflow: auto;
}
.ld-props-title {
  display: flex;
  align-items: center;
  gap: 8px;
  font-weight: 600;
  margin-bottom: 12px;
}
.ld-legend {
  display: flex;
  align-items: center;
  gap: 16px;
  padding-top: 10px;
  font-size: 12px;
  color: var(--el-text-color-regular);
  flex-wrap: wrap;
}
.ld-legend-item {
  display: inline-flex;
  align-items: center;
  gap: 5px;
}
.ld-legend-item .dot {
  width: 12px;
  height: 12px;
  border-radius: 2px;
  display: inline-block;
}
.ld-legend-stat {
  margin-left: auto;
}
.ld-color-tip {
  font-size: 12px;
  color: var(--el-text-color-secondary);
  margin-left: 8px;
}
</style>
