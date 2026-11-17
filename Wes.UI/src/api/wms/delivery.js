import request from '@/utils/request'

// 查询出库单列表
export function listDelivery(query) {
  return request({
    url: '/wms/delivery/list',
    method: 'get',
    params: query
  })
}

// 查询出库单详情
export function getDelivery(id) {
  return request({
    url: '/wms/delivery/' + id,
    method: 'get'
  })
}

// 新增出库单
export function addDelivery(data) {
  return request({
    url: '/wms/delivery',
    method: 'post',
    data: data
  })
}

// 修改出库单
export function updateDelivery(data) {
  return request({
    url: '/wms/delivery',
    method: 'put',
    data: data
  })
}

// 删除出库单
export function delDelivery(ids) {
  return request({
    url: '/wms/delivery/' + ids,
    method: 'delete'
  })
}

// 执行出库（扣减库存可用量 + 写出库流水）
export function executeDelivery(id) {
  return request({
    url: '/wms/delivery/' + id + '/execute',
    method: 'post'
  })
}
