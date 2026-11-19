<template>
  <el-cascader
    :key="warehouseKey"
    :model-value="modelValue"
    @update:model-value="$emit('update:modelValue', $event)"
    :props="cascaderProps"
    :placeholder="finalPlaceholder"
    :disabled="disabled"
    :clearable="clearable"
    :show-all-levels="true"
    style="width: 100%"
  />
</template>

<script setup>
import { computed } from "vue";
import { useI18n } from "vue-i18n";
import { listWarehouse } from "@/api/wms/warehouse";
import { listZone } from "@/api/wms/zone";
import { listLocation } from "@/api/wms/location";

const { t } = useI18n();

const props = defineProps({
  modelValue: { type: Array, default: () => [] },
  placeholder: { type: String, default: "" },
  disabled: { type: Boolean, default: false },
  clearable: { type: Boolean, default: true },
  /**
   * 固定仓库：传入后级联从「库区」开始（两级），用于单据已选仓库的场景。
   * 不传则保持 仓库 → 库区 → 库位 三级。
   */
  warehouseId: { type: [String, Number], default: undefined },
  /** 允许只选到任意一级（如只选库区不选库位），预警配置这类"范围"场景用 */
  anyLevel: { type: Boolean, default: false },
});

defineEmits(["update:modelValue"]);

/** 是否固定仓库（固定后级联只有库区、库位两级） */
const fixedWarehouse = computed(() => !!props.warehouseId);

/** key 绑定仓库：仓库变化时强制重建级联，丢弃已缓存的懒加载节点（否则层级不会从三级切到两级） */
const warehouseKey = computed(() => props.warehouseId ?? "none");

const finalPlaceholder = computed(
  () => props.placeholder || (fixedWarehouse.value ? t('wms.location.ph.selectZoneOrLocation') : t('wms.location.ph.selectWhZoneOrLocation'))
);

const cascaderProps = computed(() => ({
  lazy: true,
  checkStrictly: props.anyLevel,
  lazyLoad: async (node, resolve) => {
    const { level, value } = node;

    try {
      if (fixedWarehouse.value) {
        // 固定仓库：level0=库区，level1=库位
        if (level === 0) {
          const res = await listZone({ pageNum: 1, pageSize: 9999, params: { warehouseId: props.warehouseId } });
          resolve(
            (res.rows || []).map((z) => ({ value: z.zoneId, label: z.zoneName, leaf: false }))
          );
        } else if (level === 1) {
          const res = await listLocation({ pageNum: 1, pageSize: 9999, params: { zoneId: value } });
          resolve(
            (res.rows || []).map((l) => ({ value: l.locationId, label: l.locationName, leaf: true }))
          );
        }
        return;
      }

      // 未固定仓库：level0=仓库，level1=库区，level2=库位
      if (level === 0) {
        const res = await listWarehouse({ pageNum: 1, pageSize: 9999 });
        resolve(
          (res.rows || []).map((w) => ({ value: w.warehouseId, label: w.warehouseName, leaf: false }))
        );
      } else if (level === 1) {
        const res = await listZone({ pageNum: 1, pageSize: 9999, params: { warehouseId: value } });
        resolve(
          (res.rows || []).map((z) => ({ value: z.zoneId, label: z.zoneName, leaf: false }))
        );
      } else if (level === 2) {
        const res = await listLocation({ pageNum: 1, pageSize: 9999, params: { zoneId: value } });
        resolve(
          (res.rows || []).map((l) => ({ value: l.locationId, label: l.locationName, leaf: true }))
        );
      }
    } catch {
      resolve([]);
    }
  },
}));
</script>
