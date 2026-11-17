import request from '@/utils/request'

// 查询包装规格列表
export function listPackSpec(query) {
  return request({
    url: '/wms/packSpec/list',
    method: 'get',
    params: query
  })
}

// 查询包装规格详情
export function getPackSpec(id) {
  return request({
    url: '/wms/packSpec/' + id,
    method: 'get'
  })
}

// 新增包装规格
export function addPackSpec(data) {
  return request({
    url: '/wms/packSpec',
    method: 'post',
    data: data
  })
}

// 修改包装规格
export function updatePackSpec(data) {
  return request({
    url: '/wms/packSpec',
    method: 'put',
    data: data
  })
}

// 删除包装规格
export function delPackSpec(ids) {
  return request({
    url: '/wms/packSpec/' + ids,
    method: 'delete'
  })
}

// 获取某包装规格整套层级树（按 rootId 归组）
export function listPackSpecTree(rootId) {
  return request({
    url: '/wms/packSpec/tree',
    method: 'get',
    params: { rootId, pageSize: 0 }
  })
}

// 查询所有顶层包装规格（用于物料页选择包装规格）
export function listPackSpecTop(query) {
  return request({
    url: '/wms/packSpec/list',
    method: 'get',
    params: query
  })
}
