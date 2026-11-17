import request from '@/utils/request'

// 查询库存流水列表
export function listStockRecord(query) {
  return request({
    url: '/wms/stockRecord/list',
    method: 'get',
    params: query
  })
}

// 查询库存流水
export function getStockRecord(id) {
  return request({
    url: '/wms/stockRecord/' + id,
    method: 'get'
  })
}

// 新增库存流水（系统内部预留，页面不提供）
export function addStockRecord(data) {
  return request({
    url: '/wms/stockRecord',
    method: 'post',
    data: data
  })
}

// 修改库存流水（系统内部预留，页面不提供）
export function updateStockRecord(data) {
  return request({
    url: '/wms/stockRecord',
    method: 'put',
    data: data
  })
}

// 删除库存流水（系统内部预留，页面不提供）
export function delStockRecord(ids) {
  return request({
    url: '/wms/stockRecord/' + ids,
    method: 'delete'
  })
}
