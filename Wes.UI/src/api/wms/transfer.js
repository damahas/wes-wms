import request from '@/utils/request'

// 查询调拨单列表
export function listTransfer(query) {
  return request({
    url: '/wms/transfer/list',
    method: 'get',
    params: query
  })
}

// 查询调拨单详情
export function getTransfer(id) {
  return request({
    url: '/wms/transfer/' + id,
    method: 'get'
  })
}

// 新增调拨单
export function addTransfer(data) {
  return request({
    url: '/wms/transfer',
    method: 'post',
    data: data
  })
}

// 修改调拨单
export function updateTransfer(data) {
  return request({
    url: '/wms/transfer',
    method: 'put',
    data: data
  })
}

// 删除调拨单
export function delTransfer(ids) {
  return request({
    url: '/wms/transfer/' + ids,
    method: 'delete'
  })
}

// 移出：调出仓扣库存并记在途
export function outTransfer(id) {
  return request({
    url: '/wms/transfer/' + id + '/out',
    method: 'post'
  })
}

// 移入：调入仓按实收数量入账
export function inTransfer(data) {
  return request({
    url: '/wms/transfer/in',
    method: 'post',
    data: data
  })
}
