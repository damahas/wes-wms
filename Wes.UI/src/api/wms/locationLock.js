import request from '@/utils/request'

// 查询库位锁定列表
export function listLocationLock(query) {
  return request({
    url: '/wms/locationLock/list',
    method: 'get',
    params: query
  })
}

// 查询库位锁定详情
export function getLocationLock(id) {
  return request({
    url: '/wms/locationLock/' + id,
    method: 'get'
  })
}

// 新增库位锁定
export function addLocationLock(data) {
  return request({
    url: '/wms/locationLock',
    method: 'post',
    data: data
  })
}

// 修改库位锁定
export function updateLocationLock(data) {
  return request({
    url: '/wms/locationLock',
    method: 'put',
    data: data
  })
}

// 删除库位锁定
export function delLocationLock(ids) {
  return request({
    url: '/wms/locationLock/' + ids,
    method: 'delete'
  })
}
