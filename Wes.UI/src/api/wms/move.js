import request from '@/utils/request'

// 查询移库单列表
export function listMove(query) {
  return request({
    url: '/wms/move/list',
    method: 'get',
    params: query
  })
}

// 查询移库单详情
export function getMove(id) {
  return request({
    url: '/wms/move/' + id,
    method: 'get'
  })
}

// 新增移库单
export function addMove(data) {
  return request({
    url: '/wms/move',
    method: 'post',
    data: data
  })
}

// 修改移库单
export function updateMove(data) {
  return request({
    url: '/wms/move',
    method: 'put',
    data: data
  })
}

// 删除移库单
export function delMove(ids) {
  return request({
    url: '/wms/move/' + ids,
    method: 'delete'
  })
}

// 执行移库（源库位扣减、目标库位增加，并写移出/移入流水）
export function executeMove(id) {
  return request({
    url: '/wms/move/' + id + '/execute',
    method: 'post'
  })
}
