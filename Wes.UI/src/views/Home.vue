<template>
  <div class="home-container">
    <!-- 欢迎横幅 -->
    <div class="welcome-banner">
      <div class="banner-content">
        <div class="banner-text">
          <h1 class="banner-greeting">{{ t('home.welcomeUser', { greeting: greeting, name: userName }) }}</h1>
          <p class="banner-date">{{ dateText }}</p>
          <p class="banner-desc">{{ t('home.monitorStatus') }}</p>
        </div>
        <div class="banner-illustration">
          <i class="fa fa-warehouse"></i>
        </div>
      </div>
    </div>

    <!-- 第一行统计卡片：核心指标 -->
    <div class="stats-row">
      <div class="stat-card warehouses">
        <div class="stat-top">
          <div class="stat-icon-wrap">
            <i class="fa fa-building"></i>
          </div>
          <span class="stat-badge">{{ t('home.warehouse') }}</span>
        </div>
        <div class="stat-number">{{ dashboard.warehouseCount ?? '-' }}</div>
        <div class="stat-footer">
          <span class="stat-label">{{ t('home.totalWarehouses') }}</span>
        </div>
      </div>
      <div class="stat-card locations">
        <div class="stat-top">
          <div class="stat-icon-wrap">
            <i class="fa fa-cubes"></i>
          </div>
          <span class="stat-badge">{{ t('home.location') }}</span>
        </div>
        <div class="stat-number">{{ dashboard.locationCount ?? '-' }}</div>
        <div class="stat-footer">
          <span class="stat-label">{{ t('home.freeLocations', { free: dashboard.freeLocationCount ?? 0 }) }}</span>
        </div>
      </div>
      <div class="stat-card materials">
        <div class="stat-top">
          <div class="stat-icon-wrap">
            <i class="fa fa-boxes"></i>
          </div>
          <span class="stat-badge">{{ t('home.material') }}</span>
        </div>
        <div class="stat-number">{{ dashboard.materialCount ?? '-' }}</div>
        <div class="stat-footer">
          <span class="stat-label">{{ t('home.materialTypes') }}</span>
        </div>
      </div>
      <div class="stat-card stock">
        <div class="stat-top">
          <div class="stat-icon-wrap">
            <i class="fa fa-layer-group"></i>
          </div>
          <span class="stat-badge">{{ t('home.stock') }}</span>
        </div>
        <div class="stat-number">{{ formatQty(dashboard.totalStockQuantity) }}</div>
        <div class="stat-footer">
          <span class="stat-label">{{ t('home.totalStock') }}</span>
        </div>
      </div>
    </div>

    <!-- 第二行统计卡片：今日运营数据 -->
    <div class="stats-row">
      <div class="stat-card receipt">
        <div class="stat-top">
          <div class="stat-icon-wrap">
            <i class="fa fa-download"></i>
          </div>
          <span class="stat-badge">{{ t('home.todayReceipt') }}</span>
        </div>
        <div class="stat-number">{{ dashboard.todayReceiptCount ?? 0 }}</div>
        <div class="stat-footer">
          <span class="stat-label">{{ t('home.todayReceiptQty', { qty: formatQty(dashboard.todayReceiptQuantity) }) }}</span>
        </div>
      </div>
      <div class="stat-card delivery">
        <div class="stat-top">
          <div class="stat-icon-wrap">
            <i class="fa fa-upload"></i>
          </div>
          <span class="stat-badge">{{ t('home.todayDelivery') }}</span>
        </div>
        <div class="stat-number">{{ dashboard.todayDeliveryCount ?? 0 }}</div>
        <div class="stat-footer">
          <span class="stat-label">{{ t('home.todayDeliveryQty', { qty: formatQty(dashboard.todayDeliveryQuantity) }) }}</span>
        </div>
      </div>
      <div class="stat-card tasks">
        <div class="stat-top">
          <div class="stat-icon-wrap">
            <i class="fa fa-clipboard-list"></i>
          </div>
          <span class="stat-badge">{{ t('home.pendingTasks') }}</span>
        </div>
        <div class="stat-number">{{ dashboard.pendingShelfCount ?? 0 }}</div>
        <div class="stat-footer">
          <span class="stat-label">{{ t('home.taskDetail', { shelf: dashboard.pendingShelfCount ?? 0 }) }}</span>
        </div>
      </div>
      <div class="stat-card warning">
        <div class="stat-top">
          <div class="stat-icon-wrap">
            <i class="fa fa-triangle-exclamation"></i>
          </div>
          <span class="stat-badge">{{ t('home.warning') }}</span>
        </div>
        <div class="stat-number">{{ dashboard.stockWarningCount ?? 0 }}</div>
        <div class="stat-footer">
          <span class="stat-label">{{ t('home.stockWarning') }}</span>
        </div>
      </div>
    </div>

    <!-- 趋势图表 + 库位使用率 -->
    <div class="middle-row">
      <div class="chart-section">
        <h3 class="section-title">
          <i class="fa fa-chart-line"></i> {{ t('home.receiptDeliveryTrend') }}
        </h3>
        <div class="chart-wrapper">
          <div class="chart-legend">
            <span class="legend-item"><span class="legend-dot receipt-dot"></span>{{ t('home.receiptCount') }}</span>
            <span class="legend-item"><span class="legend-dot delivery-dot"></span>{{ t('home.deliveryCount') }}</span>
          </div>
          <div class="bar-chart">
            <div class="bar-row" v-for="(item, idx) in chartData" :key="idx">
              <span class="bar-label">{{ item.date }}</span>
              <div class="bar-area">
                <div class="bar-group">
                  <div
                    class="bar bar-receipt"
                    :style="{ width: barWidth(item.receiptCount, maxBarValue) }"
                    :title="t('home.receiptCount') + ': ' + item.receiptCount"
                  ></div>
                  <span class="bar-val">{{ item.receiptCount }}</span>
                </div>
                <div class="bar-group">
                  <div
                    class="bar bar-delivery"
                    :style="{ width: barWidth(item.deliveryCount, maxBarValue) }"
                    :title="t('home.deliveryCount') + ': ' + item.deliveryCount"
                  ></div>
                  <span class="bar-val">{{ item.deliveryCount }}</span>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
      <div class="usage-section">
        <h3 class="section-title">
          <i class="fa fa-gauge"></i> {{ t('home.locationUsage') }}
        </h3>
        <div class="usage-card">
          <div class="usage-ring">
            <svg viewBox="0 0 120 120" width="120" height="120">
              <circle cx="60" cy="60" r="52" fill="none" stroke="#f0f0f0" stroke-width="10" />
              <circle
                cx="60" cy="60" r="52" fill="none" :stroke="usageColor" stroke-width="10"
                stroke-linecap="round"
                :stroke-dasharray="usageDasharray"
                :stroke-dashoffset="0"
                transform="rotate(-90 60 60)"
                style="transition: stroke-dasharray 0.6s ease"
              />
            </svg>
            <div class="usage-text">
              <span class="usage-rate">{{ dashboard.locationUsageRate ?? 0 }}%</span>
              <span class="usage-label">{{ t('home.usageRate') }}</span>
            </div>
          </div>
          <div class="usage-info">
            <div class="usage-item">
              <span class="usage-dot green"></span>
              <span>{{ t('home.usedLocations', { count: (dashboard.locationCount ?? 0) - (dashboard.freeLocationCount ?? 0) }) }}</span>
            </div>
            <div class="usage-item">
              <span class="usage-dot gray"></span>
              <span>{{ t('home.freeLocationCount', { count: dashboard.freeLocationCount ?? 0 }) }}</span>
            </div>
            <div class="usage-item">
              <span class="usage-dot blue"></span>
              <span>{{ t('home.totalZones', { count: dashboard.zoneCount ?? 0 }) }}</span>
            </div>
            <div class="usage-item">
              <span class="usage-dot orange"></span>
              <span>{{ t('home.totalPartners', { count: dashboard.partnerCount ?? 0 }) }}</span>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- 底部三栏：待办事项 + 最近库存流水 + 快捷入口 -->
    <div class="bottom-row">
      <!-- 待办事项 -->
      <div class="info-card todo-card">
        <h3 class="section-title">
          <i class="fa fa-clipboard-check"></i> {{ t('home.todoTitle') }}
        </h3>
        <div class="todo-list">
          <div class="todo-item" @click="goTo('/out/issueOrder')">
            <span class="todo-name">{{ t('home.todoIssue') }}</span>
            <span class="todo-count warn">{{ dashboard.pendingIssueCount ?? 0 }}</span>
          </div>
          <div class="todo-item" @click="goTo('/in/receipt')">
            <span class="todo-name">{{ t('home.todoShelf') }}</span>
            <span class="todo-count warn">{{ dashboard.pendingShelfCount ?? 0 }}</span>
          </div>
          <div class="todo-item" @click="goTo('/stock/warning')">
            <span class="todo-name">{{ t('home.todoWarning') }}</span>
            <span class="todo-count" :class="{ danger: (dashboard.stockWarningCount ?? 0) > 0 }">{{ dashboard.stockWarningCount ?? 0 }}</span>
          </div>
        </div>
      </div>

      <!-- 最近库存流水 -->
      <div class="info-card record-card">
        <h3 class="section-title">
          <i class="fa fa-list-ul"></i> {{ t('home.recentRecords') }}
          <span class="section-link" @click="goTo('/stock/stockRecord')">{{ t('home.viewAll') }}</span>
        </h3>
        <div v-if="recentRecords.length" class="record-list">
          <div class="record-item" v-for="r in recentRecords" :key="r.recordNo">
            <span class="record-dir" :class="r.direction === 'out' ? 'out' : 'in'">
              {{ r.direction === 'out' ? '出' : '入' }}
            </span>
            <span class="record-name" :title="r.materialName">{{ r.materialName || '-' }}</span>
            <span class="record-qty" :class="r.direction === 'out' ? 'out' : 'in'">
              {{ r.direction === 'out' ? '-' : '+' }}{{ formatQty(r.quantity) }}
            </span>
            <span class="record-time">{{ formatTime(r.operateTime) }}</span>
          </div>
        </div>
        <div v-else class="empty-tip">{{ t('home.noRecords') }}</div>
      </div>

      <!-- 快捷入口 -->
      <div class="info-card shortcut-card">
        <h3 class="section-title">
          <i class="fa fa-bolt"></i> {{ t('home.shortcutTitle') }}
        </h3>
        <div class="shortcut-grid">
          <div class="shortcut-item" v-for="s in shortcuts" :key="s.path" @click="goTo(s.path)">
            <i :class="['fa', s.icon]"></i>
            <span>{{ t(s.label) }}</span>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue'
import { useI18n } from 'vue-i18n'
import { useRouter } from 'vue-router'
import { useStore } from 'vuex'
import { getDashboard } from '@/api/wms/dashboard'

const { t } = useI18n()
const router = useRouter()
const store = useStore()

const userName = computed(() => store.getters['user/userInfo']?.account || store.getters['user/userInfo']?.userName || '')

const shortcuts = [
  { path: '/out/issueOrder', icon: 'fa-upload', label: 'home.scIssue' },
  { path: '/in/receipt', icon: 'fa-download', label: 'home.scReceipt' },
  { path: '/stock/stock', icon: 'fa-layer-group', label: 'home.scStock' },
  { path: '/stock/stockRecord', icon: 'fa-list-ul', label: 'home.scRecord' },
  { path: '/inner/adjust', icon: 'fa-sliders', label: 'home.scAdjust' },
  { path: '/goods/material', icon: 'fa-boxes', label: 'home.scMaterial' },
]

const goTo = (path) => {
  router.push(path)
}

const formatTime = (val) => {
  if (!val) return '-'
  return String(val).replace('T', ' ').slice(5, 16)
}

const recentRecords = computed(() => dashboard.value.recentRecords || [])

const greeting = computed(() => {
  const h = new Date().getHours()
  if (h < 6) return t('home.lateNight')
  if (h < 9) return t('home.morning')
  if (h < 12) return t('home.forenoon')
  if (h < 14) return t('home.noon')
  if (h < 18) return t('home.afternoon')
  return t('home.evening')
})

const dateText = computed(() => {
  const now = new Date()
  const weekdays = [t('home.sunday'), t('home.monday'), t('home.tuesday'), t('home.wednesday'), t('home.thursday'), t('home.friday'), t('home.saturday')]
  return t('home.dateFormat', {
    year: now.getFullYear(),
    month: now.getMonth() + 1,
    day: now.getDate(),
    weekday: weekdays[now.getDay()]
  })
})

const dashboard = ref({})

const loadDashboard = async () => {
  try {
    const res = await getDashboard()
    if (res.code === 200) {
      dashboard.value = res.data || {}
    }
  } catch {
    // 加载失败保持默认空值
  }
}

onMounted(() => {
  loadDashboard()
})

const formatQty = (val) => {
  if (val == null) return '-'
  return Number(val).toLocaleString()
}

// 图表数据
const chartData = computed(() => {
  const receipt = dashboard.value.receiptTrend || []
  const delivery = dashboard.value.deliveryTrend || []
  const maxLen = Math.max(receipt.length, delivery.length)
  const result = []
  for (let i = 0; i < maxLen; i++) {
    result.push({
      date: receipt[i]?.date || delivery[i]?.date || '',
      receiptCount: receipt[i]?.count || 0,
      deliveryCount: delivery[i]?.count || 0
    })
  }
  return result
})

const maxBarValue = computed(() => {
  let max = 1
  chartData.value.forEach(item => {
    max = Math.max(max, item.receiptCount, item.deliveryCount)
  })
  return max
})

const barWidth = (val, max) => {
  if (!max || max === 0) return '0%'
  return Math.round((val / max) * 100) + '%'
}

// 库位使用率环
const usageRate = computed(() => dashboard.value.locationUsageRate ?? 0)
const circumference = 2 * Math.PI * 52
const usageDasharray = computed(() => {
  const pct = Math.min(usageRate.value, 100)
  return (pct / 100 * circumference) + ' ' + circumference
})

const usageColor = computed(() => {
  const rate = usageRate.value
  if (rate >= 80) return '#F56C6C'
  if (rate >= 60) return '#E6A23C'
  return '#6BA368'
})
</script>

<style scoped>
.home-container {
  padding: 24px;
  max-width: 1400px;
  margin: 0 auto;
}

/* ========== 欢迎横幅 ========== */
.welcome-banner {
  background: linear-gradient(135deg, #8bc48a 0%, #7ab878 50%, #6ba368 100%);
  border-radius: 12px;
  padding: 28px 32px;
  margin-bottom: 24px;
  position: relative;
  overflow: hidden;
}
.welcome-banner::after {
  content: '';
  position: absolute;
  right: -40px;
  top: -40px;
  width: 200px;
  height: 200px;
  border-radius: 50%;
  background: rgba(255,255,255,.06);
}
.welcome-banner::before {
  content: '';
  position: absolute;
  right: 80px;
  bottom: -60px;
  width: 160px;
  height: 160px;
  border-radius: 50%;
  background: rgba(255,255,255,.04);
}
.banner-content {
  display: flex;
  align-items: center;
  justify-content: space-between;
  position: relative;
  z-index: 1;
}
.banner-text {
  color: #fff;
}
.banner-greeting {
  font-size: 24px;
  font-weight: 700;
  margin: 0 0 8px;
}
.banner-date {
  font-size: 14px;
  opacity: .85;
  margin: 0 0 6px;
}
.banner-desc {
  font-size: 13px;
  opacity: .7;
  margin: 0;
}
.banner-illustration {
  font-size: 72px;
  color: rgba(255,255,255,.15);
  line-height: 1;
}

/* ========== 统计卡片 ========== */
.stats-row {
  display: grid;
  grid-template-columns: repeat(4, 1fr);
  gap: 16px;
  margin-bottom: 16px;
}
.stat-card {
  background: var(--bg-card);
  border-radius: 10px;
  padding: 20px 24px;
  border: 1px solid var(--border-color);
  transition: box-shadow .2s;
}
.stat-card:hover {
  box-shadow: 0 2px 12px rgba(0,0,0,.06);
}
.stat-top {
  display: flex;
  align-items: center;
  justify-content: space-between;
  margin-bottom: 16px;
}
.stat-icon-wrap {
  width: 40px;
  height: 40px;
  border-radius: 8px;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 18px;
}

.stat-card.warehouses .stat-icon-wrap { background: rgba(107, 163, 104, .12); color: #6BA368; }
.stat-card.locations .stat-icon-wrap  { background: rgba(64, 158, 255, .12);  color: #409EFF; }
.stat-card.materials .stat-icon-wrap  { background: rgba(230, 162, 60, .12);  color: #E6A23C; }
.stat-card.stock .stat-icon-wrap      { background: rgba(103, 194, 58, .12);  color: #67C23A; }
.stat-card.receipt .stat-icon-wrap    { background: rgba(64, 158, 255, .12);  color: #409EFF; }
.stat-card.delivery .stat-icon-wrap   { background: rgba(245, 108, 108, .12); color: #F56C6C; }
.stat-card.tasks .stat-icon-wrap      { background: rgba(230, 162, 60, .12);  color: #E6A23C; }
.stat-card.warning .stat-icon-wrap    { background: rgba(245, 108, 108, .12); color: #F56C6C; }

.stat-badge {
  font-size: 12px;
  padding: 2px 10px;
  border-radius: 10px;
  font-weight: 500;
}
.stat-card.warehouses .stat-badge { background: rgba(107,163,104,.1);  color: #6BA368; }
.stat-card.locations .stat-badge  { background: rgba(64,158,255,.1);   color: #409EFF; }
.stat-card.materials .stat-badge  { background: rgba(230,162,60,.1);   color: #E6A23C; }
.stat-card.stock .stat-badge      { background: rgba(103,194,58,.1);   color: #67C23A; }
.stat-card.receipt .stat-badge    { background: rgba(64,158,255,.1);   color: #409EFF; }
.stat-card.delivery .stat-badge   { background: rgba(245,108,108,.1);  color: #F56C6C; }
.stat-card.tasks .stat-badge      { background: rgba(230,162,60,.1);   color: #E6A23C; }
.stat-card.warning .stat-badge    { background: rgba(245,108,108,.1);  color: #F56C6C; }

.stat-number {
  font-size: 32px;
  font-weight: 700;
  color: var(--text-title);
  line-height: 1;
  margin-bottom: 12px;
  font-variant-numeric: tabular-nums;
}
.stat-footer {
  display: flex;
  align-items: center;
  justify-content: space-between;
}
.stat-label {
  font-size: 13px;
  color: var(--text-secondary);
}

/* ========== 中部两栏 ========== */
.middle-row {
  display: grid;
  grid-template-columns: 1fr 380px;
  gap: 16px;
  margin-bottom: 24px;
}

/* ========== Section Title ========== */
.section-title {
  font-size: 16px;
  font-weight: 600;
  color: var(--text-title);
  margin: 0 0 14px;
  display: flex;
  align-items: center;
  gap: 8px;
}
.section-title i {
  color: var(--theme-color);
}

/* ========== 趋势图 ========== */
.chart-section {
  background: var(--bg-card);
  border-radius: 10px;
  padding: 20px 24px;
  border: 1px solid var(--border-color);
}
.chart-legend {
  display: flex;
  gap: 24px;
  margin-bottom: 16px;
}
.legend-item {
  font-size: 13px;
  color: var(--text-secondary);
  display: flex;
  align-items: center;
  gap: 6px;
}
.legend-dot {
  width: 10px;
  height: 10px;
  border-radius: 2px;
}
.legend-dot.receipt-dot  { background: #409EFF; }
.legend-dot.delivery-dot { background: #F56C6C; }

.bar-chart {
  display: flex;
  flex-direction: column;
  gap: 8px;
}
.bar-row {
  display: flex;
  align-items: center;
  gap: 10px;
}
.bar-label {
  font-size: 12px;
  color: var(--text-secondary);
  width: 42px;
  flex-shrink: 0;
  text-align: right;
}
.bar-area {
  flex: 1;
  display: flex;
  flex-direction: column;
  gap: 3px;
}
.bar-group {
  display: flex;
  align-items: center;
  gap: 6px;
  height: 20px;
}
.bar {
  height: 16px;
  border-radius: 3px;
  min-width: 2px;
  transition: width 0.4s ease;
}
.bar-receipt  { background: #409EFF; }
.bar-delivery { background: #F56C6C; }
.bar-val {
  font-size: 11px;
  color: var(--text-secondary);
  width: 24px;
  text-align: left;
  flex-shrink: 0;
}

/* ========== 库位使用率 ========== */
.usage-section {
  background: var(--bg-card);
  border-radius: 10px;
  padding: 20px 24px;
  border: 1px solid var(--border-color);
}
.usage-card {
  display: flex;
  align-items: flex-start;
  gap: 24px;
}
.usage-ring {
  position: relative;
  flex-shrink: 0;
}
.usage-text {
  position: absolute;
  top: 50%;
  left: 50%;
  transform: translate(-50%, -50%);
  text-align: center;
}
.usage-rate {
  font-size: 22px;
  font-weight: 700;
  color: var(--text-title);
  display: block;
  line-height: 1;
}
.usage-label {
  font-size: 11px;
  color: var(--text-secondary);
  margin-top: 2px;
}
.usage-info {
  display: flex;
  flex-direction: column;
  gap: 12px;
  padding-top: 8px;
}
.usage-item {
  font-size: 13px;
  color: var(--text-secondary);
  display: flex;
  align-items: center;
  gap: 8px;
}
.usage-dot {
  width: 8px;
  height: 8px;
  border-radius: 50%;
  flex-shrink: 0;
}
.usage-dot.green  { background: #6BA368; }
.usage-dot.gray   { background: #C0C4CC; }
.usage-dot.blue   { background: #409EFF; }
.usage-dot.orange { background: #E6A23C; }

/* ========== 底部三栏 ========== */
.bottom-row {
  display: grid;
  grid-template-columns: 300px 1fr 300px;
  gap: 16px;
}
.info-card {
  background: var(--bg-card);
  border-radius: 10px;
  padding: 20px 24px;
  border: 1px solid var(--border-color);
}
.section-link {
  margin-left: auto;
  font-size: 12px;
  font-weight: 400;
  color: var(--theme-color);
  cursor: pointer;
}
.section-link:hover { text-decoration: underline; }

/* 待办事项 */
.todo-list {
  display: flex;
  flex-direction: column;
  gap: 10px;
}
.todo-item {
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: 10px 14px;
  border-radius: 8px;
  background: var(--border-color-lighter);
  cursor: pointer;
  transition: background .2s;
}
.todo-item:hover { background: var(--border-color); }
.todo-name { font-size: 13px; color: var(--text-primary); }
.todo-count {
  font-size: 18px;
  font-weight: 700;
  font-variant-numeric: tabular-nums;
  color: var(--text-title);
}
.todo-count.warn { color: #E6A23C; }
.todo-count.danger { color: #F56C6C; }

/* 最近库存流水 */
.record-list {
  display: flex;
  flex-direction: column;
}
.record-item {
  display: flex;
  align-items: center;
  gap: 12px;
  padding: 9px 4px;
  border-bottom: 1px dashed var(--border-color);
  font-size: 13px;
}
.record-item:last-child { border-bottom: none; }
.record-dir {
  width: 22px;
  height: 22px;
  border-radius: 6px;
  font-size: 12px;
  display: flex;
  align-items: center;
  justify-content: center;
  flex-shrink: 0;
}
.record-dir.in  { background: rgba(103,194,58,.12);  color: #67C23A; }
.record-dir.out { background: rgba(245,108,108,.12); color: #F56C6C; }
.record-name {
  flex: 1;
  color: var(--text-primary);
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
}
.record-qty {
  font-weight: 600;
  font-variant-numeric: tabular-nums;
  flex-shrink: 0;
}
.record-qty.in  { color: #67C23A; }
.record-qty.out { color: #F56C6C; }
.record-time {
  font-size: 12px;
  color: var(--text-secondary);
  width: 84px;
  text-align: right;
  flex-shrink: 0;
}
.empty-tip {
  padding: 30px 0;
  text-align: center;
  font-size: 13px;
  color: var(--text-secondary);
}

/* 快捷入口 */
.shortcut-grid {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 10px;
}
.shortcut-item {
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 8px;
  padding: 16px 8px;
  border-radius: 8px;
  background: var(--border-color-lighter);
  cursor: pointer;
  font-size: 13px;
  color: var(--text-primary);
  transition: background .2s, transform .2s;
}
.shortcut-item:hover {
  background: var(--border-color);
  transform: translateY(-2px);
}
.shortcut-item i {
  font-size: 18px;
  color: var(--theme-color);
}

/* ========== 响应式 ========== */
@media (max-width: 1280px) {
  .middle-row { grid-template-columns: 1fr; }
  .bottom-row { grid-template-columns: 1fr; }
}
@media (max-width: 900px) {
  .stats-row { grid-template-columns: repeat(2, 1fr); }
}
@media (max-width: 600px) {
  .stats-row { grid-template-columns: 1fr; }
  .home-container { padding: 16px; }
  .welcome-banner { padding: 20px; }
  .banner-greeting { font-size: 20px; }
  .banner-illustration { display: none; }
  .usage-card { flex-direction: column; align-items: center; }
}
</style>
