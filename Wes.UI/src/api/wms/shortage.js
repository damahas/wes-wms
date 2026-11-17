import request from '@/utils/request'

// 缺料看板列表
export function listShortage(query) {
  return request({
    url: '/wms/shortage/list',
    method: 'get',
    params: query
  })
}

// 缺料看板汇总
export function summaryShortage(query) {
  return request({
    url: '/wms/shortage/summary',
    method: 'get',
    params: query
  })
}
