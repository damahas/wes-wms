import request from '@/utils/request'

// 查询往来单位列表
export function listPartner(query) {
  return request({
    url: '/wms/partner/list',
    method: 'get',
    params: query
  })
}

// 查询往来单位详情
export function getPartner(id) {
  return request({
    url: '/wms/partner/' + id,
    method: 'get'
  })
}

// 新增往来单位
export function addPartner(data) {
  return request({
    url: '/wms/partner',
    method: 'post',
    data: data
  })
}

// 修改往来单位
export function updatePartner(data) {
  return request({
    url: '/wms/partner',
    method: 'put',
    data: data
  })
}

// 删除往来单位
export function delPartner(ids) {
  return request({
    url: '/wms/partner/' + ids,
    method: 'delete'
  })
}

// 修改往来单位状态
export function changePartnerStatus(data) {
  return request({
    url: '/wms/partner/changeStatus',
    method: 'put',
    data: data
  })
}
