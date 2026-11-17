<template>
  <div class="tags-view-container">
    <el-icon v-show="canLeft" class="scroll-btn" @click="scrollByStep(-1)">
      <ArrowLeft />
    </el-icon>
    <div class="tags-view-scroll" ref="scrollRef" @scroll="updateArrows">
      <span
        v-for="tag in visitedViews"
        :key="tag.fullPath"
        class="tags-view-item"
        :class="{ active: isActive(tag), affix: tag.affix }"
        @click="handleClick(tag)"
        @contextmenu.prevent="openMenu(tag, $event)"
      >
        <span class="tag-dot" :class="{ active: isActive(tag) }"></span>
        <span class="tag-title">{{ tag.title }}</span>
        <el-icon v-if="!tag.affix" class="tag-close" @click.stop="closeSelectedTag(tag)">
          <Close />
        </el-icon>
      </span>
    </div>
    <el-icon v-show="canRight" class="scroll-btn" @click="scrollByStep(1)">
      <ArrowRight />
    </el-icon>
    <el-dropdown
      v-show="hasOverflow"
      trigger="click"
      popper-class="tags-view-dropdown"
      @command="handleDropdown"
    >
      <span class="dropdown-trigger" :title="t('tagsView.more')">
        <el-icon><MoreFilled /></el-icon>
      </span>
      <template #dropdown>
        <el-dropdown-menu>
          <el-dropdown-item
            v-for="tag in visitedViews"
            :key="tag.fullPath"
            :command="tag.fullPath"
            :class="{ 'is-active': isActive(tag) }"
          >
            <span class="dropdown-dot" :class="{ active: isActive(tag) }"></span>
            <span class="dropdown-title">{{ tag.title }}</span>
          </el-dropdown-item>
        </el-dropdown-menu>
      </template>
    </el-dropdown>
    <ul
      v-show="menuVisible"
      class="contextmenu"
      :style="{ left: menuLeft + 'px', top: menuTop + 'px' }"
    >
      <li @click="refreshSelectedTag(selectedTag)">{{ t("tagsView.refresh") }}</li>
      <li @click="closeSelectedTag(selectedTag)">{{ t("tagsView.close") }}</li>
      <li @click="closeOthersTags(selectedTag)">{{ t("tagsView.closeOthers") }}</li>
      <li @click="closeAllTags">{{ t("tagsView.closeAll") }}</li>
    </ul>
  </div>
</template>

<script setup>
import { computed, nextTick, onBeforeUnmount, onMounted, ref, watch } from "vue";
import { useRoute, useRouter } from "vue-router";
import { useStore } from "vuex";
import { useI18n } from "vue-i18n";
import { ArrowLeft, ArrowRight, Close, MoreFilled } from "@element-plus/icons-vue";

const route = useRoute();
const router = useRouter();
const store = useStore();
const { t } = useI18n();

const visitedViews = computed(() => store.state.tagsView.visitedViews);

const menuVisible = ref(false);
const menuLeft = ref(0);
const menuTop = ref(0);
const selectedTag = ref(null);

// 溢出时的滚动控制
const scrollRef = ref(null);
const canLeft = ref(false);
const canRight = ref(false);
const hasOverflow = ref(false);

const isActive = (tag) => tag.path === route.path;

function buildView(r) {
  return {
    name: r.name,
    path: r.path,
    fullPath: r.fullPath,
    title: r.path === "/home" ? t("menu.home") : r.meta?.title || r.name || r.path,
    noCache: !!r.meta?.noCache,
    affix: r.path === "/home",
  };
}

function addTag() {
  // 登录页与 404 不进标签页
  if (route.path === "/login" || route.path === "/404") return;
  store.dispatch("tagsView/addView", buildView(route));
}

// 以 fullPath 监听：路径或查询参数变化都会登记/激活对应标签
watch(() => route.fullPath, addTag, { immediate: true });

watch(
  () => route.fullPath,
  async () => {
    await nextTick();
    // 激活标签滚入可视区，并刷新箭头状态
    const container = scrollRef.value;
    const active = container?.querySelector(".tags-view-item.active");
    if (container && active) {
      const target =
        active.offsetLeft - container.clientWidth / 2 + active.offsetWidth / 2;
      container.scrollTo({ left: Math.max(target, 0), behavior: "smooth" });
    }
    updateArrows();
  }
);

// 标签数量变化（关闭/打开）后重新计算溢出状态
watch(() => visitedViews.value.length, () => nextTick(updateArrows));

function updateArrows() {
  const el = scrollRef.value;
  if (!el) return;
  hasOverflow.value = el.scrollWidth > el.clientWidth + 1;
  canLeft.value = el.scrollLeft > 1;
  canRight.value = el.scrollLeft + el.clientWidth < el.scrollWidth - 1;
}

function scrollByStep(direction) {
  scrollRef.value?.scrollBy({ left: direction * 240, behavior: "smooth" });
}

function handleDropdown(fullPath) {
  const tag = visitedViews.value.find((v) => v.fullPath === fullPath);
  if (tag && !isActive(tag)) {
    router.push(tag.fullPath);
  }
}

function handleClick(tag) {
  if (tag.path !== route.path) {
    router.push(tag.fullPath);
  }
}

async function closeSelectedTag(tag) {
  closeMenu();
  // 注意：局部变量不能命名为 isActive，否则遮蔽外层同名函数触发 TDZ 报错
  const wasActive = isActive(tag);
  store.dispatch("tagsView/delView", tag);
  if (wasActive) {
    const others = visitedViews.value.filter((v) => v.fullPath !== tag.fullPath);
    const next = others[others.length - 1];
    router.push(next ? next.fullPath : "/home");
  }
}

async function refreshSelectedTag(tag) {
  closeMenu();
  if (!tag) return;
  // 非当前页先激活，再刷新
  if (tag.path !== route.path) {
    await router.push(tag.fullPath);
    await nextTick();
  }
  await store.dispatch("tagsView/refreshView", tag);
}

function closeOthersTags(tag) {
  closeMenu();
  if (tag.path !== route.path) {
    router.push(tag.fullPath);
  }
  store.dispatch("tagsView/delOthersViews", tag);
}

function closeAllTags() {
  closeMenu();
  store.dispatch("tagsView/delAllViews");
  if (route.path !== "/home") router.push("/home");
}

function openMenu(tag, e) {
  selectedTag.value = tag;
  menuLeft.value = e.clientX;
  menuTop.value = e.clientY;
  menuVisible.value = true;
}

function closeMenu() {
  menuVisible.value = false;
}

watch(menuVisible, (visible) => {
  if (visible) {
    document.body.addEventListener("click", closeMenu);
  } else {
    document.body.removeEventListener("click", closeMenu);
  }
});

onMounted(() => {
  if (!visitedViews.value.some((v) => v.affix)) {
    store.dispatch("tagsView/addView", buildView({ path: "/home", fullPath: "/home", name: "Home", meta: {} }));
  }
  window.addEventListener("resize", updateArrows);
  nextTick(updateArrows);
});

onBeforeUnmount(() => {
  window.removeEventListener("resize", updateArrows);
  document.body.removeEventListener("click", closeMenu);
});
</script>

<style lang="scss" scoped>
/* 嵌入 Header 中间的标签页：透明容器 + 胶囊式标签，激活态用主题渐变 */
.tags-view-container {
  height: 100%;
  min-width: 0;
  display: flex;
  align-items: center;
  gap: 4px;
}

.tags-view-scroll {
  flex: 1;
  min-width: 0;
  display: flex;
  align-items: center;
  gap: 8px;
  overflow-x: auto;
  overflow-y: hidden;
  padding: 2px 0;

  /* 隐藏滚动条，用左右箭头代替 */
  &::-webkit-scrollbar {
    height: 0;
    display: none;
  }
  scrollbar-width: none;
  -ms-overflow-style: none;
}

.scroll-btn {
  flex-shrink: 0;
  width: 22px;
  height: 22px;
  border-radius: 50%;
  color: var(--text-secondary);
  cursor: pointer;
  display: inline-flex;
  align-items: center;
  justify-content: center;
  transition: all 0.2s;

  &:hover {
    background-color: var(--bg-hover);
    color: var(--theme-color);
  }
}

.dropdown-trigger {
  flex-shrink: 0;
  width: 24px;
  height: 24px;
  border-radius: 6px;
  color: var(--text-secondary);
  cursor: pointer;
  display: inline-flex;
  align-items: center;
  justify-content: center;
  transition: all 0.2s;

  &:hover {
    background-color: var(--bg-hover);
    color: var(--theme-color);
  }
}

.tags-view-item {
  display: inline-flex;
  align-items: center;
  gap: 6px;
  height: 28px;
  padding: 0 12px;
  border: 1px solid transparent;
  border-radius: 999px;
  background-color: var(--border-color-lighter);
  color: var(--text-secondary);
  font-size: 13px;
  cursor: pointer;
  white-space: nowrap;
  user-select: none;
  transition: all 0.2s;

  .tag-dot {
    width: 6px;
    height: 6px;
    border-radius: 50%;
    background-color: var(--text-placeholder);
    flex-shrink: 0;
    transition: background-color 0.2s;
  }

  .tag-title {
    max-width: 160px;
    overflow: hidden;
    text-overflow: ellipsis;
  }

  .tag-close {
    width: 14px;
    height: 14px;
    font-size: 12px;
    margin-right: -4px;
    border-radius: 50%;
    flex-shrink: 0;
    opacity: 0.55;

    &:hover {
      background-color: rgba(255, 255, 255, 0.35);
      color: #fff;
      opacity: 1;
    }
  }

  &:hover {
    background-color: var(--bg-hover);
    color: var(--text-primary);

    .tag-close {
      opacity: 1;
    }
  }

  &.affix:hover {
    background-color: var(--border-color-lighter);
  }

  &.active {
    background-image: var(--gradient-morning-mist);
    border-color: transparent;
    color: #fff;
    font-weight: 500;
    box-shadow: 0 2px 8px var(--theme-color-light);

    .tag-dot.active {
      background-color: rgba(255, 255, 255, 0.85);
    }

    .tag-close {
      color: #fff;
      opacity: 0.8;

      &:hover {
        background-color: rgba(255, 255, 255, 0.3);
      }
    }
  }
}

.contextmenu {
  position: fixed;
  z-index: 3000;
  margin: 0;
  padding: 5px 0;
  list-style: none;
  background-color: var(--bg-card);
  border: 1px solid var(--border-color);
  border-radius: 8px;
  box-shadow: var(--shadow-card-hover);
  font-size: 13px;
  color: var(--text-primary);

  li {
    padding: 6px 16px;
    cursor: pointer;

    &:hover {
      background-color: var(--bg-hover);
      color: var(--theme-color);
    }
  }
}
</style>

<style lang="scss">
/* 「更多」下拉总览：el-dropdown 弹层 teleport 到 body，需全局样式 */
.tags-view-dropdown {
  max-width: 320px;

  .el-dropdown-menu__item {
    display: flex;
    align-items: center;
    gap: 8px;
    max-width: 320px;

    .dropdown-dot {
      width: 6px;
      height: 6px;
      border-radius: 50%;
      background-color: var(--border-color);
      flex-shrink: 0;
    }

    .dropdown-title {
      overflow: hidden;
      text-overflow: ellipsis;
      white-space: nowrap;
    }

    &.is-active {
      color: var(--theme-color);
      font-weight: 500;
      background-color: var(--bg-hover);

      .dropdown-dot.active {
        background-color: var(--theme-color);
      }
    }
  }
}
</style>
