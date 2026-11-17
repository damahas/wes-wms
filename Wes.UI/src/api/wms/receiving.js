import request from '@/utils/request'

// 查询收货单列表
export function listReceiving(query) {
  return request({
    url: '/wms/receiving/list',
    method: 'get',
    params: query
  })
}

// 查询收货单详情
export function getReceiving(id) {
  return request({
    url: '/wms/receiving/' + id,
    method: 'get'
  })
}

// 查询收货单明细列表
export function getReceivingDetailList(receivingId) {
  return request({
    url: '/wms/receiving/detailList/' + receivingId,
    method: 'get'
  })
}

// 新增收货单（含明细）
export function addReceiving(data) {
  return request({
    url: '/wms/receiving',
    method: 'post',
    data: data
  })
}

// 修改收货单（含明细）
export function updateReceiving(data) {
  return request({
    url: '/wms/receiving',
    method: 'put',
    data: data
  })
}

// 删除收货单
export function delReceiving(ids) {
  return request({
    url: '/wms/receiving/' + ids,
    method: 'delete'
  })
}

// 确认收货：免检生成入库单，需检生成检验单
export function confirmReceipt(receivingId) {
  return request({
    url: '/wms/receiving/confirmReceipt/' + receivingId,
    method: 'put'
  })
}
