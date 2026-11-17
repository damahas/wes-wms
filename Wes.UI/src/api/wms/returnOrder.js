import request from '@/utils/request'

// 查询退货单列表
export function listReturnOrder(query) {
  return request({
    url: '/wms/returnOrder/list',
    method: 'get',
    params: query
  })
}

// 查询退货单详情
export function getReturnOrder(id) {
  return request({
    url: '/wms/returnOrder/' + id,
    method: 'get'
  })
}

// 查询退货单明细列表
export function getReturnOrderDetailList(returnOrderId) {
  return request({
    url: '/wms/returnOrder/detailList/' + returnOrderId,
    method: 'get'
  })
}

// 新增退货单（含明细）
export function addReturnOrder(data) {
  return request({
    url: '/wms/returnOrder',
    method: 'post',
    data: data
  })
}

// 修改退货单（含明细）
export function updateReturnOrder(data) {
  return request({
    url: '/wms/returnOrder',
    method: 'put',
    data: data
  })
}

// 删除退货单
export function delReturnOrder(ids) {
  return request({
    url: '/wms/returnOrder/' + ids,
    method: 'delete'
  })
}
