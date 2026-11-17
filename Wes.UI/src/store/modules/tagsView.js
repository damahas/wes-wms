/**
 * 标签页导航（tags-view）状态
 *
 * visitedViews：已打开的标签页列表（以 fullPath 去重，重复打开则激活已有标签）
 * cachedViews：keep-alive 缓存的组件名列表（即路由 name，配合布局中的包装组件匹配）
 *
 * 缓存策略：所有已打开页面默认缓存（保留查询条件、页码、表单内容等状态）；
 * 重新激活时布局会调用页面定义的 tabRefresh() 方法（如刷新列表数据），是否刷新由页面自己决定；
 * 菜单配置了"不缓存"（meta.noCache）→ 不缓存。
 */
import { nextTick } from "vue";

export default {
  namespaced: true,
  state: {
    visitedViews: [],
    cachedViews: [],
    // 每个标签页的手动刷新计数器（key: fullPath），用于强制重新渲染当前页
    refreshCounters: {},
  },
  mutations: {
    ADD_VISITED_VIEW(state, view) {
      if (state.visitedViews.some((v) => v.fullPath === view.fullPath)) return;
      state.visitedViews.push(view);
      // 固定标签（首页）始终排在最前，其余保持打开顺序（sort 为稳定排序）
      state.visitedViews.sort((a, b) => (b.affix ? 1 : 0) - (a.affix ? 1 : 0));
    },
    ADD_CACHED_VIEW(state, view) {
      if (!view.name || view.noCache) return;
      if (!state.cachedViews.includes(view.name)) {
        state.cachedViews.push(view.name);
      }
    },
    DEL_VISITED_VIEW(state, view) {
      const index = state.visitedViews.findIndex((v) => v.fullPath === view.fullPath);
      if (index > -1) state.visitedViews.splice(index, 1);
    },
    DEL_CACHED_VIEW(state, view) {
      if (!view.name) return;
      const index = state.cachedViews.indexOf(view.name);
      if (index > -1) state.cachedViews.splice(index, 1);
    },
    DEL_OTHERS_VIEWS(state, view) {
      state.visitedViews = state.visitedViews.filter(
        (v) => v.affix || v.fullPath === view.fullPath
      );
      const keepNames = state.visitedViews.map((v) => v.name);
      state.cachedViews = state.cachedViews.filter((name) => keepNames.includes(name));
    },
    DEL_ALL_VIEWS(state) {
      state.visitedViews = state.visitedViews.filter((v) => v.affix);
      const keepNames = state.visitedViews.map((v) => v.name);
      state.cachedViews = state.cachedViews.filter((name) => keepNames.includes(name));
    },
    UPDATE_VISITED_VIEW(state, view) {
      const target = state.visitedViews.find((v) => v.fullPath === view.fullPath);
      if (target && view.title) target.title = view.title;
    },
    REFRESH_VIEW(state, fullPath) {
      state.refreshCounters = {
        ...state.refreshCounters,
        [fullPath]: (state.refreshCounters[fullPath] || 0) + 1,
      };
    },
  },
  actions: {
    addView({ commit }, view) {
      commit("ADD_VISITED_VIEW", view);
      commit("ADD_CACHED_VIEW", view);
    },
    delView({ commit }, view) {
      commit("DEL_VISITED_VIEW", view);
      commit("DEL_CACHED_VIEW", view);
    },
    delCachedView({ commit }, view) {
      commit("DEL_CACHED_VIEW", view);
    },
    addCachedView({ commit }, view) {
      commit("ADD_CACHED_VIEW", view);
    },
    delOthersViews({ commit }, view) {
      commit("DEL_OTHERS_VIEWS", view);
    },
    delAllViews({ commit }) {
      commit("DEL_ALL_VIEWS");
    },
    updateView({ commit }, view) {
      commit("UPDATE_VISITED_VIEW", view);
    },
    /** 手动刷新标签页：先摘除缓存销毁旧实例，渲染完成后再恢复缓存并递增计数器强制重渲染 */
    async refreshView({ commit, dispatch }, view) {
      await dispatch("delCachedView", view);
      await nextTick();
      await dispatch("addCachedView", view);
      commit("REFRESH_VIEW", view.fullPath);
    },
  },
};
