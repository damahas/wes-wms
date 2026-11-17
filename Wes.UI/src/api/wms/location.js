import request from '@/utils/request'

// 查询库位列表
export function listLocation(query) {
  return request({
    url: '/wms/location/list',
    method: 'get',
    params: query
  })
}

// 查询库位详情
export function getLocation(id) {
  return request({
    url: '/wms/location/' + id,
    method: 'get'
  })
}

// 新增库位
export function addLocation(data) {
  return request({
    url: '/wms/location',
    method: 'post',
    data: data
  })
}

// 修改库位
export function updateLocation(data) {
  return request({
    url: '/wms/location',
    method: 'put',
    data: data
  })
}

// 删除库位
export function delLocation(ids) {
  return request({
    url: '/wms/location/' + ids,
    method: 'delete'
  })
}

// 修改库位状态
export function changeLocationStatus(data) {
  return request({
    url: '/wms/location/changeStatus',
    method: 'put',
    data: data
  })
}
