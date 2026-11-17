import request from '@/utils/request'

// 分页查询省市区列表
export function listRegion(query) {
  return request({
    url: '/wms/region/list',
    method: 'get',
    params: query
  })
}

// 查询省市区树（供级联选择）
export function treeRegion() {
  return request({
    url: '/wms/region/tree',
    method: 'get'
  })
}
