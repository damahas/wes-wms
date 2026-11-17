<template>
  <div class="login-container">
    <div class="login-panel">
      <el-dropdown class="lang-dropdown" @command="handleLangChange">
        <span class="lang-text">
          <i class="fa fa-language" style="font-size: 12px"></i>
          <span>{{ currentLangDisplay }}</span>
          <i class="fa-solid fa-angle-down"></i>
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

      <div class="login-title">{{ t("login.title") }}</div>

      <!-- 账号密码：始终显示，两步验证由弹窗接管 -->
        <div class="form-item">
          <div class="form-label">
            <span class="required">*</span>
            {{ t("login.userName") }}
          </div>
          <el-input
            v-model="loginForm.userName"
            class="form-input"
            :placeholder="t('login.userNamePlaceholder')"
            @change="isUserNameError = !loginForm.userName"
          />
          <div class="form-error" v-if="isUserNameError">
            {{ t("login.userNamePlaceholder") }}
          </div>
        </div>

        <div class="form-item">
          <div class="form-label">
            <span class="required">*</span>
            {{ t("login.password") }}
          </div>
          <el-input
            v-model="loginForm.password"
            class="form-input"
            :placeholder="t('login.passwordPlaceholder')"
            type="password"
            @change="isPasswordError = !loginForm.password"
            @keyup.enter="handleLogin"
          />
          <div class="form-error" v-if="isPasswordError">
            {{ t("login.passwordPlaceholder") }}
          </div>
        </div>

        <div class="form-item" style="margin-top: 50px" v-if="captchaEnabled">
          <slider-code
            ref="sliderCodeRef"
            style="width: 100%"
            @handleOk="handleSliderSuccess"
          ></slider-code>
        </div>

        <div
          class="login-btn"
          :class="{ 'login-btn-no-captcha': !captchaEnabled }"
          @click="handleLogin"
        >
          {{ t("login.login") }}
        </div>

    <!-- 两步验证：登录第二步 / 首次绑定，统一弹窗承载 -->
    <el-dialog
      v-model="twoFactorVisible"
      :title="t('login.twoFactor.title')"
      width="440px"
      align-center
      :close-on-click-modal="false"
      @closed="handleTwoFactorClosed"
    >
      <!-- 第二步：输入验证码 -->
      <div class="tfa-block" v-if="step === 'verify'">
        <div class="form-tip">
          <template v-if="tfaMode === 'email'">
            {{ t("login.twoFactor.emailSent") }}
            <b>{{ emailTarget }}</b>
          </template>
          <template v-else>
            {{ t("login.twoFactor.totpHint") }}
          </template>
        </div>
        <div class="form-label">
          <span class="required">*</span>
          {{ t("login.twoFactor.code") }}
        </div>
        <el-input
          v-model="verifyForm.code"
          class="form-input"
          :placeholder="t('login.twoFactor.codePlaceholder')"
          maxlength="12"
          @keyup.enter="handleVerify"
        />
        <div class="link-row" v-if="tfaMode === 'email'">
          <span @click="handleResend">{{ t("login.twoFactor.resend") }}</span>
        </div>
      </div>

      <!-- 首次登录：绑定两步验证 -->
      <div v-else>
        <div class="tfa-leading-tip">{{ t("login.twoFactor.bindTip") }}</div>

        <!-- 二维码独立居中展示：放进 el-form-item 会被表单行高撑出多余空白 -->
        <div class="qr-wrap" v-if="tfaMode === 'totp'">
          <img v-if="qrDataUrl" :src="qrDataUrl" class="qr-img" :alt="t('login.twoFactor.scanTip')" />
          <el-skeleton v-else class="qr-img" animated />
          <div class="qr-tip">{{ t("login.twoFactor.scanTip") }}</div>
        </div>

        <div class="form-item">
          <div class="form-label">
            <span class="required">*</span>
            {{ tfaMode === 'email' ? t("login.twoFactor.emailCode") : t("login.twoFactor.totpCode") }}
          </div>
          <el-input
            v-model="verifyForm.code"
            class="form-input"
            :placeholder="t('login.twoFactor.codePlaceholder')"
            maxlength="12"
            @keyup.enter="handleBind"
          />
          <div class="link-row" v-if="tfaMode === 'email'">
            <span @click="handleResend">{{ t("login.twoFactor.resend") }}</span>
          </div>
        </div>
      </div>

      <template #footer>
        <el-button plain @click="twoFactorVisible = false">
          {{ t("login.twoFactor.back") }}
        </el-button>
        <el-button type="primary" @click="step === 'verify' ? handleVerify() : handleBind()">
          {{ step === 'verify' ? t("login.login") : t("login.twoFactor.bindAndLogin") }}
        </el-button>
      </template>
    </el-dialog>
    </div>

  </div>
</template>

<script setup>
import { reactive, ref, computed, onMounted, onBeforeUnmount } from "vue";
import { useRouter, useRoute } from "vue-router";
import { useStore } from "vuex";
import { useI18n } from "vue-i18n";
import { ElMessage } from "element-plus";
import QRCode from "qrcode";
import { encryptPassword } from "@/utils/rsa";
import SliderCode from "@/components/SliderCode";
import {
  login,
  isCaptchaOn,
  isTwoFactorOn,
  verifyTwoFactor,
  sendTwoFactorEmailCode,
  prepareLoginBind,
  bindAndLogin,
} from "@/api/login";

const router = useRouter();
const route = useRoute();
const store = useStore();
const { t, locale } = useI18n();

const loginForm = reactive({
  userName: "",
  password: "",
  code: "",
  positionX: 0,
});
const sliderCodeRef = ref();

const isUserNameError = ref(false);
const isPasswordError = ref(false);
const captchaEnabled = ref(false);

// 两步验证
const config = ref({ enabled: false, mode: "email", required: true });
const step = ref("password"); // password | verify | bind
// 两步验证走弹窗，弹窗关闭即视为放弃本次第二因素校验
const twoFactorVisible = ref(false);
const challengeToken = ref("");
// 验证方式由系统参数配置决定，用户不可自选
const tfaMode = computed(() => (config.value.mode === "totp" ? "totp" : "email"));
const emailTarget = ref("");
const qrDataUrl = ref("");
const verifyForm = reactive({ code: "" });

const pendingToken = ref("");

onMounted(async () => {
  try {
    const res = await isCaptchaOn();
    captchaEnabled.value = res.data;
  } catch {
    captchaEnabled.value = false;
  }
  try {
    const res = await isTwoFactorOn();
    config.value = { ...config.value, ...(res.data || {}) };
  } catch {
    config.value = { enabled: false, mode: "email" };
  }
});

const langList = computed(() => store.getters["system/langList"]);
const currentLangDisplay = computed(() => {
  const lang = store.getters["system/locale"] || "en";
  return langList.value.find((p) => p.langCode === lang)?.langName;
});

const handleLangChange = (lang) => {
  store.dispatch("system/setLocale", lang);
  locale.value = lang;
};

const handleSliderSuccess = (data) => {
  if (data) {
    loginForm.code = data.code;
    loginForm.positionX = data.positionX;
    if (loginForm.userName && loginForm.password) {
      handleLogin();
    } else {
      ElMessage.success(t('request.success') || '验证成功');
    }
    return;
  }
  loginForm.code = "";
  loginForm.positionX = 0;
};

const finishLogin = () => {
  ElMessage.success(t("login.success"));
  store.dispatch("user/setAccessToken", pendingToken.value);
  router.push(route.query.redirect || "/");
};

/** 进入第二步/绑定步骤 */
const enterStep = async (nextStep, res) => {
  challengeToken.value = res.challengeToken;
  verifyForm.code = "";
  emailTarget.value = res.email || "";
  step.value = nextStep;
  twoFactorVisible.value = true;

  if (nextStep === "verify") {
    if (tfaMode.value === "email") await handleSendCode();
    return;
  }

  await prepareBind();
};

/** 绑定准备：验证器方式取密钥，邮件方式发验证码 */
const prepareBind = async () => {
  if (tfaMode.value !== "totp") {
    await handleSendCode();
    return;
  }
  try {
    const res = await prepareLoginBind({ challengeToken: challengeToken.value, mode: tfaMode.value });
    await renderQrCode(res.otpauthUri);
  } catch (e) {
    qrDataUrl.value = "";
  }
};

/** 生成绑定二维码 */
const renderQrCode = async (text) => {
  if (!text) {
    qrDataUrl.value = "";
    return;
  }
  try {
    qrDataUrl.value = await QRCode.toDataURL(text, {
      errorCorrectionLevel: "M",
      margin: 1,
      width: 180,
      color: { dark: "#000000ff", light: "#ffffffff" },
    });
  } catch {
    qrDataUrl.value = "";
  }
};

/** 发送/重发邮箱验证码 */
const handleSendCode = async () => {
  try {
    await sendTwoFactorEmailCode({ challengeToken: challengeToken.value, mode: "email" });
    ElMessage.success(t("login.twoFactor.codeSent"));
  } catch (e) {
    /* 错误提示由拦截器统一处理 */
  }
};

const handleResend = () => {
  handleSendCode();
};

const handleVerify = async () => {
  if (!verifyForm.code) {
    ElMessage.warning(t("login.twoFactor.codePlaceholder"));
    return;
  }
  try {
    const res = await verifyTwoFactor({
      challengeToken: challengeToken.value,
      mode: tfaMode.value,
      code: verifyForm.code,
    });
    // 密钥异常或未绑定：转入绑定流程
    if (res?.code === 332) {
      await enterStep("bind", res);
      return;
    }
    // 验证失败：票据已作废，回到登录第一步
    if (res?.code === 333) {
      ElMessage.error(res.msg || t("login.twoFactor.needRelogin"));
      backToPassword();
      return;
    }
    completeLogin(res);
  } catch (e) {
    /* 错误提示由拦截器统一处理 */
  }
};

const handleBind = async () => {
  if (!verifyForm.code) {
    ElMessage.warning(t("login.twoFactor.codePlaceholder"));
    return;
  }
  try {
    const res = await bindAndLogin({
      challengeToken: challengeToken.value,
      mode: tfaMode.value,
      code: verifyForm.code,
    });
    // 绑定校验失败：票据已作废，回到登录第一步
    if (res?.code === 333) {
      ElMessage.error(res.msg || t("login.twoFactor.needRelogin"));
      backToPassword();
      return;
    }
    completeLogin(res);
  } catch (e) {
    /* 错误提示由拦截器统一处理 */
  }
};

const completeLogin = (res) => {
  if (!res || res.code !== 200) return;
  pendingToken.value = res.token;
  twoFactorVisible.value = false;
  finishLogin();
};

const backToPassword = () => {
  twoFactorVisible.value = false;
  step.value = "password";
  verifyForm.code = "";
};

/** 弹窗关闭（关闭按钮 / ESC）：清理中间票据状态回到登录第一步 */
const handleTwoFactorClosed = () => {
  if (step.value !== "password") {
    backToPassword();
  }
};

const handleLogin = async () => {
  if (!loginForm.userName) {
    isUserNameError.value = true;
  }
  if (!loginForm.password) {
    isPasswordError.value = true;
  }
  if (!loginForm.userName || !loginForm.password) {
    return;
  }

  isUserNameError.value = false;
  isPasswordError.value = false;

  let encryptedPassword;
  try {
    encryptedPassword = encryptPassword(loginForm.password);
  } catch (e) {
    ElMessage.error(e.message || t("login.passwordEncryptFailed"));
    return;
  }

  try {
    const res = await login({ ...loginForm, password: encryptedPassword });
    if (!res) return;
    if (res.code === 200) {
      pendingToken.value = res.token;
      finishLogin();
      return;
    }
    if (res.code === 331) {
      await enterStep("verify", res);
      return;
    }
    if (res.code === 332) {
      await enterStep("bind", res);
      return;
    }
  } catch (e) {
    // 重新获取验证码
    sliderCodeRef.value?.reset();
  }
};
</script>

<style scoped>
.login-container {
  width: 100%;
  height: 100vh;
  background-size: 100% 100%;
  background-repeat: no-repeat;
  background-position: center;
  background-image: url(/images/login/login-bg.jpg);
  display: flex;
  align-items: center;
  justify-content: flex-end;
  position: relative;
}

.login-logo {
  position: absolute;
  top: 48px;
  left: 63px;
}

.login-panel {
  position: relative;
  width: 480px;
  height: auto;
  min-height: 580px;
  border-radius: 12px;
  background-color: var(--bg-card);
  margin-right: 145px;
  padding: 0 69px 30px;
  box-shadow: 0 2px 10px var(--shadow-color);
}

.lang-dropdown {
  position: absolute;
  top: 29px;
  right: 27px;
}

.lang-text {
  color: var(--text-secondary);
  display: flex;
  align-items: center;
  gap: 3px;
  cursor: pointer;
}

.lang-text span {
  font-size: 14px;
  margin-left: 3px;
  margin-right: 9px;
}

.login-title {
  font-size: 24px;
  font-weight: 700;
  color: var(--login-title-color);
  text-align: center;
  margin-top: 103px;
}

.form-item {
  margin-top: 30px;
  position: relative;
}

.form-item:first-of-type {
  margin-top: 41px;
}

/* 弹窗内首屏：沿用登录面板的 .form-item 会带上来 30/41px 顶部留白，这里用独立类避开 */
.tfa-block {
  position: relative;
}

.tfa-leading-tip {
  font-size: 14px;
  color: var(--text-secondary);
  line-height: 22px;
  margin-bottom: 14px;
}

.form-label {
  font-size: 16px;
  font-weight: 500;
  color: var(--login-label-color);
}

.required {
  color: var(--color-danger);
  margin-right: 3px;
}

.form-input {
  margin-top: 15px;
}

.form-error {
  color: var(--color-danger);
  font-size: 12px;
  margin-top: 3px;
}

.form-tip {
  font-size: 14px;
  color: var(--text-secondary);
  line-height: 22px;
  margin-bottom: 6px;
}

.link-row {
  margin-top: 8px;
  font-size: 14px;
  color: var(--login-btn-bg);
  cursor: pointer;
}

.link-row .disabled {
  color: var(--text-secondary);
  cursor: not-allowed;
}

/* 二维码居中容器：独立承载，避免表单行高带来额外空白 */
.qr-wrap {
  display: flex;
  flex-direction: column;
  align-items: center;
  margin-bottom: 18px;
}

.qr-img {
  width: 180px;
  height: 180px;
  display: block;
  padding: 6px;
  border-radius: 6px;
  background-color: #ffffff;
}

.qr-tip {
  margin-top: 10px;
  font-size: 14px;
  color: var(--text-secondary);
}

.login-btn {
  background: var(--login-btn-bg);
  border-radius: 7.28px;
  color: white;
  font-size: 18px;
  font-weight: 400;
  margin-top: 59px;
  height: 36px;
  width: 341px;
  display: flex;
  align-items: center;
  justify-content: center;
  cursor: pointer;
}

.login-btn-no-captcha {
  margin-top: 100px;
}

.login-btn:hover {
  background: var(--login-btn-hover-bg);
}

.login-btn:active {
  background: var(--login-btn-active-bg);
}
</style>

<style>
html.dark .login-container {
  background-image: url(/images/login/login-bg-b.jpg);
}
</style>
