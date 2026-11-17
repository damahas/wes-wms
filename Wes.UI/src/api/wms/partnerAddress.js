import request from '@/utils/request'

// 查询往来单位地址列表
export function listPartnerAddress(query) {
  return request({
    url: '/wms/partnerAddress/list',
    method: 'get',
    params: query
  })
}

// 查询往来单位地址详情
export function getPartnerAddress(id) {
  return request({
    url: '/wms/partnerAddress/' + id,
    method: 'get'
  })
}

// 新增往来单位地址
export function addPartnerAddress(data) {
  return request({
    url: '/wms/partnerAddress',
    method: 'post',
    data: data
  })
}

// 修改往来单位地址
export function updatePartnerAddress(data) {
  return request({
    url: '/wms/partnerAddress',
    method: 'put',
    data: data
  })
}

// 删除往来单位地址
export function delPartnerAddress(ids) {
  return request({
    url: '/wms/partnerAddress/' + ids,
    method: 'delete'
  })
}
