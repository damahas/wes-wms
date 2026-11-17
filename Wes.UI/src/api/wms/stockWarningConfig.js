import request from '@/utils/request'

// 查询预警配置列表
export function listStockWarningConfig(query) {
  return request({
    url: '/wms/stockWarningConfig/list',
    method: 'get',
    params: query
  })
}

// 查询预警配置详情
export function getStockWarningConfig(id) {
  return request({
    url: '/wms/stockWarningConfig/' + id,
    method: 'get'
  })
}

// 新增预警配置
export function addStockWarningConfig(data) {
  return request({
    url: '/wms/stockWarningConfig',
    method: 'post',
    data: data
  })
}

// 修改预警配置
export function updateStockWarningConfig(data) {
  return request({
    url: '/wms/stockWarningConfig',
    method: 'put',
    data: data
  })
}

// 删除预警配置
export function delStockWarningConfig(ids) {
  return request({
    url: '/wms/stockWarningConfig/' + ids,
    method: 'delete'
  })
}
