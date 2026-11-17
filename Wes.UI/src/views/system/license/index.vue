<template>
  <div class="app-container">
    <el-card class="box-card">
      <template #header>
        <div class="clearfix">
          <span>{{ t('license.title') }}</span>
        </div>
      </template>
      <el-form ref="formRef" :model="form" :rules="rules" label-width="auto">
        <el-form-item :label="t('license.platformSystem')">{{ form.platformOS }}</el-form-item>
        <el-form-item :label="t('license.platformCode')">{{ form.platformCode }}</el-form-item>
        <el-form-item :label="t('license.licenseKey')" prop="licenseCode">
          <el-input
            type="textarea"
            :rows="5"
            :placeholder="t('license.licenseKeyPlaceholder')"
            v-model="form.licenseCode"
          ></el-input>
        </el-form-item>
        <el-row>
          <el-col :span="1" :offset="22">
            <el-button type="primary" @click="handleSave">{{ t('license.activate') }}</el-button>
          </el-col>
        </el-row>
      </el-form>
    </el-card>
    <el-card class="box-card" style="margin-top: 8px" v-if="form.licenseModel">
      <template #header>
        <div class="clearfix">
          <span>{{ t('license.activationInfo') }}</span>
        </div>
      </template>
      <el-form :model="form" label-width="auto">
        <el-form-item :label="t('license.licenseVersion')">
          {{ licenseVersionText }}
        </el-form-item>
        <el-form-item :label="t('license.activationTime')">
          {{ form.licenseModel.activateTime }}
        </el-form-item>
        <el-form-item :label="t('license.expireTime')">
          {{ form.licenseModel.expireTime }}
        </el-form-item>
      </el-form>
    </el-card>
  </div>
</template>

<script setup>
import { ref, reactive, computed } from 'vue'
import { useI18n } from 'vue-i18n'
import { getLicense, saveLicense } from '@/api/system/license'

const { t } = useI18n()

// 许可证版本：trial 试用版 / enterpriseTrial 企业试运行版 / enterprise 企业正式版
const licenseTypeMap = {
  trial: 'license.trial',
  enterprisetrial: 'license.enterpriseTrial',
  trialrun: 'license.enterpriseTrial',
  enterprisetrialrun: 'license.enterpriseTrial',
  enterprise: 'license.enterprise'
}

const licenseVersionText = computed(() => {
  const type = String(form.licenseModel?.licenseType || '')
    .toLowerCase()
    .replace(/[_-]/g, '')
  return t(licenseTypeMap[type] || 'license.trial')
})

const formRef = ref(null)
const form = reactive({
  platformOS: '',
  platformCode: '',
  licenseCode: '',
  licenseModel: null
})

const rules = {
  licenseCode: [{ required: true, message: 'license.licenseKeyPlaceholder', trigger: 'focus' }]
}

function handleGet() {
  getLicense().then((response) => {
    Object.assign(form, response.data)
  })
}

function handleSave() {
  formRef.value.validate((valid) => {
    if (valid) {
      saveLicense(form).then(() => {
        handleGet()
      })
    }
  })
}

handleGet()
</script>
