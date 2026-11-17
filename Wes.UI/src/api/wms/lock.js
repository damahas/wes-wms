import request from '@/utils/request'

// 查询库存锁定列表
export function listLock(query) {
  return request({
    url: '/wms/lock/list',
    method: 'get',
    params: query
  })
}

// 查询库存锁定详情
export function getLock(id) {
  return request({
    url: '/wms/lock/' + id,
    method: 'get'
  })
}

// 新增库存锁定
export function addLock(data) {
  return request({
    url: '/wms/lock',
    method: 'post',
    data: data
  })
}

// 修改库存锁定
export function updateLock(data) {
  return request({
    url: '/wms/lock',
    method: 'put',
    data: data
  })
}

// 删除库存锁定
export function delLock(ids) {
  return request({
    url: '/wms/lock/' + ids,
    method: 'delete'
  })
}

// 执行锁定（库存锁定数量增加、可用数量减少）
export function executeLock(id) {
  return request({
    url: '/wms/lock/' + id + '/execute',
    method: 'post'
  })
}

// 释放锁定（未使用部分回补可用数量）
export function releaseLock(id) {
  return request({
    url: '/wms/lock/' + id + '/release',
    method: 'post'
  })
}
