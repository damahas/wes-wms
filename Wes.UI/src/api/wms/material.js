import request from '@/utils/request'

// 查询物料列表
export function listMaterial(query) {
  return request({
    url: '/wms/material/list',
    method: 'get',
    params: query
  })
}

// 查询物料详情
export function getMaterial(materialId) {
  return request({
    url: '/wms/material/' + materialId,
    method: 'get'
  })
}

// 新增物料
export function addMaterial(data) {
  return request({
    url: '/wms/material',
    method: 'post',
    data: data
  })
}

// 修改物料
export function updateMaterial(data) {
  return request({
    url: '/wms/material',
    method: 'put',
    data: data
  })
}

// 删除物料
export function delMaterial(materialIds) {
  return request({
    url: '/wms/material/' + materialIds,
    method: 'delete'
  })
}

// 修改物料状态
export function changeMaterialStatus(data) {
  return request({
    url: '/wms/material/changeStatus',
    method: 'put',
    data: data
  })
}
