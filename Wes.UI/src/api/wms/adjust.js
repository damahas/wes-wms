import request from '@/utils/request'

// 查询库存调整列表
export function listAdjust(query) {
  return request({
    url: '/wms/adjust/list',
    method: 'get',
    params: query
  })
}

// 查询库存调整详情
export function getAdjust(id) {
  return request({
    url: '/wms/adjust/' + id,
    method: 'get'
  })
}

// 新增库存调整
export function addAdjust(data) {
  return request({
    url: '/wms/adjust',
    method: 'post',
    data: data
  })
}

// 修改库存调整
export function updateAdjust(data) {
  return request({
    url: '/wms/adjust',
    method: 'put',
    data: data
  })
}

// 删除库存调整
export function delAdjust(ids) {
  return request({
    url: '/wms/adjust/' + ids,
    method: 'delete'
  })
}

// 执行库存调整（更新库存 + 写库存流水）
export function executeAdjust(id) {
  return request({
    url: '/wms/adjust/' + id + '/execute',
    method: 'post'
  })
}
