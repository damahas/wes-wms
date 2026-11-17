import request from '@/utils/request'

// 查询往来单位联系人列表
export function listPartnerContact(query) {
  return request({
    url: '/wms/partnerContact/list',
    method: 'get',
    params: query
  })
}

// 查询往来单位联系人详情
export function getPartnerContact(id) {
  return request({
    url: '/wms/partnerContact/' + id,
    method: 'get'
  })
}

// 新增往来单位联系人
export function addPartnerContact(data) {
  return request({
    url: '/wms/partnerContact',
    method: 'post',
    data: data
  })
}

// 修改往来单位联系人
export function updatePartnerContact(data) {
  return request({
    url: '/wms/partnerContact',
    method: 'put',
    data: data
  })
}

// 删除往来单位联系人
export function delPartnerContact(ids) {
  return request({
    url: '/wms/partnerContact/' + ids,
    method: 'delete'
  })
}
