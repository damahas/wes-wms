<template>
  <el-header class="header">
    <div class="header-left">
      <div class="header-logo">
        <img src="/images/logo.png" alt="WES" class="header-logo-img" />
      </div>
      <span class="header-title">WES管理系统</span>
    </div>
    <TagsView class="header-tags" />
    <div class="header-right">
      <span class="header-icon-btn ai-trigger" @click="openAi" :title="t('ai.title')">
        <span class="ai-trigger__text">AI</span>
      </span>

      <a
        class="header-icon-btn github-link"
        href="https://github.com/damahas/wes-admin"
        target="_blank"
        title="GitHub"
      >
        <i class="fa-brands fa-github"></i>
      </a>

      <Message />

      <span class="header-icon-btn theme-icon" @click="handleDarkChange(!isDark)">
        <i class="fa fa-sun" v-if="isDark"></i>
        <i class="fa fa-moon" v-else></i>
      </span>

      <el-dropdown @command="handleLangChange">
        <span class="header-icon-btn">
          <i class="fa fa-globe"></i>
        </span>
        <template #dropdown>
          <el-dropdown-menu>
            <el-dropdown-item
              v-for="item in langList"
              :key="item.langCode"
              :command="item.langCode"
            >
              {{ item.langName }}
            </el-dropdown-item>
          </el-dropdown-menu>
        </template>
      </el-dropdown>

      <el-dropdown @command="handleCommand">
        <span class="user-info">
          {{ userInfo?.userName || t("common.user") }}
          <el-icon class="el-icon--right"><arrow-down /></el-icon>
        </span>
        <template #dropdown>
          <el-dropdown-menu>
            <el-dropdown-item command="logout">{{ t("common.logout") }}</el-dropdown-item>
          </el-dropdown-menu>
        </template>
      </el-dropdown>
    </div>
  </el-header>
</template>

<script setup>
import { computed, inject } from "vue";
import { useRouter } from "vue-router";
import { useStore } from "vuex";
import { useI18n } from "vue-i18n";
import { ArrowDown } from "@element-plus/icons-vue";
import Message from "@/layout/components/Message";
import TagsView from "@/layout/components/TagsView/index.vue";

const router = useRouter();
const store = useStore();
const { t, locale } = useI18n();

const isDark = computed(() => store.getters["system/isDark"]);
const currentLang = computed(() => store.getters["system/locale"]);
const langList = computed(() => store.getters["system/langList"]);
const userInfo = computed(() => store.getters["user/userInfo"]);

const openAi = inject("openAi");

const handleDarkChange = (value) => {
  store.dispatch("system/setDark", value);
};

const handleLangChange = (lang) => {
  store.dispatch("system/setLocale", lang);
  locale.value = lang;
  location.reload();
};

const handleCommand = (command) => {
  if (command === "logout") {
    store.dispatch("user/logout");
    router.push("/login");
  }
};
</script>

<style scoped>
.header {
  height: 48px;
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 0 20px;
  border: none;
  box-shadow: none;
  background-color: var(--bg-header);
  border-bottom: 1px solid var(--border-color);
  flex-shrink: 0;
}

.header-left {
  display: flex;
  align-items: center;
  gap: 10px;
  flex-shrink: 0;
}

/* 顶栏中间的标签页区域 */
.header-tags {
  flex: 1;
  min-width: 0;
  margin: 0 16px;
}

.header-logo {
  display: flex;
  align-items: center;
  justify-content: center;
}

.header-logo-img {
  height: 28px;
  width: auto;
  display: block;
}

.header-title {
  font-size: 18px;
  font-weight: 500;
  color: var(--text-title);
  letter-spacing: 0.01em;
}

.header-right {
  display: flex;
  align-items: center;
  gap: 8px;
}

.header-icon-btn {
  display: flex;
  align-items: center;
  justify-content: center;
  width: 32px;
  height: 32px;
  border-radius: 50%;
  cursor: pointer;
  color: var(--text-secondary);
  font-size: 15px;
  transition: background-color 0.2s;
}

.header-icon-btn:hover {
  background-color: var(--bg-hover);
}

.github-link {
  text-decoration: none;
  font-size: 17px;
}

.ai-trigger {
  color: var(--text-secondary);
}
.ai-trigger:hover {
  color: var(--theme-color);
  background-color: var(--theme-color-light);
}
.ai-trigger__text {
  font-size: 17px;
  font-weight: 700;
  letter-spacing: 0.5px;
  font-style: normal;
}

.user-info {
  display: flex;
  align-items: center;
  gap: 8px;
  padding-left: 6px;
  cursor: pointer;
  color: var(--text-primary);
}


</style>
