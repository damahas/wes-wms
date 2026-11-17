import request from '@/utils/request'

// 查询出库退货列表
export function listReturnOut(query) {
  return request({
    url: '/wms/returnOut/list',
    method: 'get',
    params: query
  })
}

// 查询出库退货详情
export function getReturnOut(id) {
  return request({
    url: '/wms/returnOut/' + id,
    method: 'get'
  })
}

// 新增出库退货
export function addReturnOut(data) {
  return request({
    url: '/wms/returnOut',
    method: 'post',
    data: data
  })
}

// 修改出库退货
export function updateReturnOut(data) {
  return request({
    url: '/wms/returnOut',
    method: 'put',
    data: data
  })
}

// 删除出库退货
export function delReturnOut(ids) {
  return request({
    url: '/wms/returnOut/' + ids,
    method: 'delete'
  })
}
