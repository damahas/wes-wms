import request from '@/utils/request'

// 查询库存预警列表
export function listStockWarning(query) {
  return request({
    url: '/wms/stockWarning/list',
    method: 'get',
    params: query
  })
}

// 查询库存预警详情
export function getStockWarning(id) {
  return request({
    url: '/wms/stockWarning/' + id,
    method: 'get'
  })
}

// 查询预警周期内的触发历史（每天一条快照）
export function getWarningLogs(id) {
  return request({
    url: '/wms/stockWarning/logs/' + id,
    method: 'get'
  })
}

// 删除库存预警记录
export function delStockWarning(ids) {
  return request({
    url: '/wms/stockWarning/' + ids,
    method: 'delete'
  })
}

// 手动跑一遍预警引擎（定时任务也会跑）
export function runStockWarning() {
  return request({
    url: '/wms/stockWarning/run',
    method: 'post'
  })
}

// 处理 / 忽略预警（status: handled-已处理 ignored-已忽略）
export function handleStockWarning(id, status, remark) {
  return request({
    url: '/wms/stockWarning/handle/' + id,
    method: 'put',
    params: { status, remark }
  })
}
