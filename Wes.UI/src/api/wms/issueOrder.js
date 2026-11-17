import request from '@/utils/request'

// 查询领料单列表
export function listIssueOrder(query) {
  return request({
    url: '/wms/issueOrder/list',
    method: 'get',
    params: query
  })
}

// 查询领料单详情
export function getIssueOrder(id) {
  return request({
    url: '/wms/issueOrder/' + id,
    method: 'get'
  })
}

// 新增领料单
export function addIssueOrder(data) {
  return request({
    url: '/wms/issueOrder',
    method: 'post',
    data: data
  })
}

// 修改领料单
export function updateIssueOrder(data) {
  return request({
    url: '/wms/issueOrder',
    method: 'put',
    data: data
  })
}

// 删除领料单
export function delIssueOrder(ids) {
  return request({
    url: '/wms/issueOrder/' + ids,
    method: 'delete'
  })
}

// 发料：按 FEFO/FIFO 有多少发多少，生成出库单；支持多次发料
export function issueIssueOrder(id) {
  return request({
    url: '/wms/issueOrder/' + id + '/issue',
    method: 'post'
  })
}

// 取消领料单
export function cancelIssueOrder(id) {
  return request({
    url: '/wms/issueOrder/' + id + '/cancel',
    method: 'post'
  })
}
