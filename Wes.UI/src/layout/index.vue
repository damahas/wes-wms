<template>
  <div class="layout-container">
    <Header />
    <div class="layout-body">
      <Sidebar :is-collapse="isCollapse" @toggle-collapse="toggleCollapse" />
      <el-main class="main">
        <router-view v-slot="{ Component, route: viewRoute }">
          <keep-alive :include="cachedViews">
            <component :is="wrapView(viewRoute, Component)" :key="viewKey(viewRoute)" :view="Component" />
          </keep-alive>
        </router-view>
      </el-main>
    </div>
  </div>
</template>

<script setup>
import { computed, defineComponent, h, markRaw, onActivated, ref } from 'vue'
import { useStore } from 'vuex'
import Header from './components/Header'
import Sidebar from './components/Sidebar'

const store = useStore()

const isCollapse = ref(false)

const toggleCollapse = () => {
  isCollapse.value = !isCollapse.value
}

/**
 * 调用页面自定义的 tabRefresh() 方法（激活标签页时触发，是否刷新由页面自己决定）。
 * 两种定义方式都支持：
 *   1. <script setup> 顶层直接定义 function tabRefresh() {...}
 *   2. defineExpose({ tabRefresh })
 */
function callTabRefresh(inner) {
  if (!inner) return
  if (typeof inner.tabRefresh === 'function') {
    inner.tabRefresh()
    return
  }
  const inst = inner.$
  const fn = inst?.setupState?.tabRefresh
  if (typeof fn === 'function') fn()
}

// keep-alive 的 include 按组件名匹配，而路由 name（来自菜单配置）与视图组件名不一致，
// 因此用包装组件把路由 name 注入为组件名，实现"标签页关闭/刷新时精确销毁缓存"。
// 包装组件同时负责：标签页重新激活时回调页面 tabRefresh()。
const wrapperCache = new Map()

function wrapView(route, Component) {
  if (!Component) return Component
  if (!route?.name) return markRaw(Component)
  let wrapper = wrapperCache.get(route.name)
  if (!wrapper) {
    wrapper = defineComponent({
      name: route.name,
      props: { view: { type: [Object, Function], required: true } },
      setup(props) {
        const innerRef = ref(null)
        let firstActivation = true
        onActivated(() => {
          // 首次挂载后的 activated 跳过：页面刚创建，数据本来就是新的
          if (firstActivation) {
            firstActivation = false
            return
          }
          callTabRefresh(innerRef.value)
        })
        return () => h(props.view, { ref: innerRef })
      },
    })
    wrapperCache.set(route.name, wrapper)
  }
  return wrapper
}

// key = fullPath + 刷新计数：手动刷新时递增计数器强制重渲染，其他情况 fullPath 保证参数不同的页面独立缓存
const refreshCounters = computed(() => store.state.tagsView.refreshCounters)
const viewKey = (route) =>
  route.fullPath + (refreshCounters.value[route.fullPath] ? `:r${refreshCounters.value[route.fullPath]}` : "")

const cachedViews = computed(() => store.state.tagsView.cachedViews)
</script>

<style scoped>
.layout-container {
  display: flex;
  flex-direction: column;
  width: 100%;
  height: 100vh;
}

.layout-body {
  flex: 1;
  display: flex;
  overflow: hidden;
}

.main {
  flex: 1;
  padding: 10px;
  overflow-y: auto;
  box-sizing: border-box;
  background-color: var(--bg-primary);
}
</style>
