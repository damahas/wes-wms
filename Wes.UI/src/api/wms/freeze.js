import request from '@/utils/request'

// 查询库存冻结列表
export function listFreeze(query) {
  return request({
    url: '/wms/freeze/list',
    method: 'get',
    params: query
  })
}

// 查询库存冻结详情
export function getFreeze(id) {
  return request({
    url: '/wms/freeze/' + id,
    method: 'get'
  })
}

// 新增库存冻结
export function addFreeze(data) {
  return request({
    url: '/wms/freeze',
    method: 'post',
    data: data
  })
}

// 修改库存冻结
export function updateFreeze(data) {
  return request({
    url: '/wms/freeze',
    method: 'put',
    data: data
  })
}

// 删除库存冻结
export function delFreeze(ids) {
  return request({
    url: '/wms/freeze/' + ids,
    method: 'delete'
  })
}

// 执行冻结（库存冻结数量增加、可用数量减少）
export function executeFreeze(id) {
  return request({
    url: '/wms/freeze/' + id + '/execute',
    method: 'post'
  })
}

// 解冻（库存冻结数量减少、可用数量回补）
export function thawFreeze(id, thawReason) {
  return request({
    url: '/wms/freeze/' + id + '/thaw',
    method: 'post',
    params: { thawReason }
  })
}
