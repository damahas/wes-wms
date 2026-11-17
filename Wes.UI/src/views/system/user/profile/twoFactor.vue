<template>
  <div class="two-factor">
    <el-alert
      v-if="!status.bound"
      :title="t('user.twoFactor.notBound')"
      type="warning"
      :closable="false"
      show-icon
      class="mb-tip"
    />

    <el-descriptions :column="1" border class="mb-tip">
      <el-descriptions-item :label="t('user.twoFactor.bindStatus')">
        <el-tag :type="status.bound ? 'success' : 'warning'">
          {{ status.bound ? t('user.twoFactor.bound') : t('user.twoFactor.unbound') }}
        </el-tag>
      </el-descriptions-item>
      <el-descriptions-item :label="t('user.twoFactor.method')" v-if="status.bound">
        {{ methodLabel(status.mode) }}
      </el-descriptions-item>
      <el-descriptions-item :label="t('user.twoFactor.bindTime')" v-if="status.bound">
        {{ status.bindTime || '-' }}
      </el-descriptions-item>

    </el-descriptions>

    <!-- 绑定 -->
    <template v-if="!status.bound">
      <el-button type="primary" @click="openBindDialog">{{ t('user.twoFactor.bind') }}</el-button>
    </template>

    <!-- 绑定弹窗 -->
    <el-dialog
      v-model="bindVisible"
      :title="t('user.twoFactor.bind')"
      width="460px"
      :close-on-click-modal="false"
      @closed="code = ''"
    >
      <!-- 二维码独立居中展示：不放 el-form-item 行内，避免表单行高撑出多余空白 -->
      <div class="qr-wrap" v-if="mode === 'totp'">
        <img v-if="qrDataUrl" :src="qrDataUrl" class="qr-img" :alt="t('user.twoFactor.scanTip')" />
        <el-skeleton v-else class="qr-img" animated />
        <div class="qr-tip">{{ t('user.twoFactor.scanTip') }}</div>
      </div>

      <el-form label-width="110px">
        <el-form-item :label="t('user.twoFactor.code')">
          <el-input
            v-model="code"
            maxlength="12"
            style="width: 220px"
            :placeholder="mode === 'email' ? t('user.twoFactor.emailCodePlaceholder') : t('user.twoFactor.totpCodePlaceholder')"
          />
          <el-button v-if="mode === 'email'" style="margin-left: 8px" @click="handleSendCode">
            {{ t('user.twoFactor.sendCode') }}
          </el-button>
        </el-form-item>
      </el-form>

      <template #footer>
        <el-button v-if="mode === 'totp'" @click="handlePrepare">{{ t('user.twoFactor.regenerateSecret') }}</el-button>
        <el-button @click="bindVisible = false">{{ t('common.cancel') }}</el-button>
        <el-button type="primary" @click="handleBind">{{ t('user.twoFactor.bind') }}</el-button>
      </template>
    </el-dialog>

    <!-- 解绑 -->
    <template v-if="status.bound">
      <el-button type="danger" plain @click="unbindVisible = true">{{ t('user.twoFactor.unbind') }}</el-button>
    </template>

    <!-- 解绑确认 -->
    <el-dialog v-model="unbindVisible" :title="t('user.twoFactor.unbind')" width="420px">
      <el-form label-width="110px">
        <el-form-item :label="t('user.twoFactor.password')">
          <el-input v-model="unbindForm.password" type="password" show-password />
        </el-form-item>
        <el-form-item :label="t('user.twoFactor.code')">
          <el-input v-model="unbindForm.code" maxlength="12" />
        </el-form-item>
      </el-form>
      <template #footer>
        <el-button @click="unbindVisible = false">{{ t('common.cancel') }}</el-button>
        <el-button type="danger" @click="handleUnbind">{{ t('common.confirm') }}</el-button>
      </template>
    </el-dialog>

  </div>
</template>

<script setup>
import { ref, reactive, onMounted, onBeforeUnmount, computed } from "vue";
import { useI18n } from "vue-i18n";
import { ElMessage } from "element-plus";
import QRCode from "qrcode";
import {
  getTwoFactorStatus,
  prepareTwoFactor,
  confirmTwoFactor,
  unbindTwoFactor,
} from "@/api/system/user";
import { isTwoFactorOn } from "@/api/login";
import { encryptPassword } from "@/utils/rsa";

const { t } = useI18n();

const status = ref({ bound: false, mode: "", bindTime: "" });
const config = ref({ enabled: false, mode: "email" });
// 验证方式由系统参数配置决定，用户不可自选
const mode = computed(() => (config.value.mode === "totp" ? "totp" : "email"));
const code = ref("");
const qrDataUrl = ref("");
const unbindVisible = ref(false);
const bindVisible = ref(false);
const unbindForm = reactive({ password: "", code: "" });

const methodLabel = (val) =>
  val === "totp" ? t('user.twoFactor.totp') : t('user.twoFactor.email');

onMounted(async () => {
  try {
    const cfg = await isTwoFactorOn();
    config.value = { enabled: false, mode: "email", ...(cfg.data || {}) };
  } catch {
    /* 忽略 */
  }
  await loadStatus();
  // 绑定改为按需发起，进入页面不再自动发送验证码
});

const loadStatus = async () => {
  try {
    const res = await getTwoFactorStatus();
    status.value = res.data || {};
  } catch {
    /* 忽略 */
  }
};

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

const handlePrepare = async () => {
  if (status.value.bound) return;
  try {
    const res = await prepareTwoFactor();
    const data = res.data || {};
    if (data.enabled === false) {
      ElMessage.warning(data.msg || t('request.requestFailed'));
      return;
    }
    qrDataUrl.value = "";
    if (mode.value === "totp") {
      await renderQrCode(data.otpauthUri);
    }
  } catch {
    /* 忽略 */
  }
};

const handleSendCode = async () => {
  await handlePrepare();
};

/** 打开绑定弹窗时才生成密钥 / 发送验证码 */
const openBindDialog = async () => {
  qrDataUrl.value = "";
  code.value = "";
  bindVisible.value = true;
  await handlePrepare();
};

const handleBind = async () => {
  if (!code.value) {
    ElMessage.warning(t('user.twoFactor.codeRequired'));
    return;
  }
  try {
    const res = await confirmTwoFactor({ mode: mode.value, code: code.value });
    code.value = "";
    bindVisible.value = false;
    await loadStatus();
    ElMessage.success(res.msg);
  } catch {
    /* 忽略 */
  }
};

const handleUnbind = async () => {
  try {
    const res = await unbindTwoFactor({
      ...unbindForm,
      password: encryptPassword(unbindForm.password),
    });
    ElMessage.success(res.msg);
    unbindVisible.value = false;
    unbindForm.password = "";
    unbindForm.code = "";
    await loadStatus();
    // 重新绑定走弹窗，这里不再预先生成密钥 / 发送验证码
  } catch {
    /* 忽略 */
  }
};

</script>

<style scoped>
.two-factor {
  font-size: 14px;
}

.mb-tip {
  margin-bottom: 16px;
}

/* 二维码居中容器：独立承载，避免 el-form-item 行高带来额外空白 */
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
  border-radius: 4px;
  background-color: #ffffff;
}

.qr-tip {
  margin-top: 10px;
  font-size: 14px;
  color: var(--text-secondary, #909399);
}

</style>
