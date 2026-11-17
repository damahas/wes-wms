import request from '@/utils/request'

// 查询实时库存列表
export function listStock(query) {
  return request({
    url: '/wms/stock/list',
    method: 'get',
    params: query
  })
}

// 查询实时库存详情
export function getStock(id) {
  return request({
    url: '/wms/stock/' + id,
    method: 'get'
  })
}

// 新增实时库存
export function addStock(data) {
  return request({
    url: '/wms/stock',
    method: 'post',
    data: data
  })
}

// 修改实时库存
export function updateStock(data) {
  return request({
    url: '/wms/stock',
    method: 'put',
    data: data
  })
}

// 删除实时库存
export function delStock(ids) {
  return request({
    url: '/wms/stock/' + ids,
    method: 'delete'
  })
}
