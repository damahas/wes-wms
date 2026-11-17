import request from '@/utils/request'

// 查询分类下检验项列表(不分页)
export function listIqcItem(categoryId) {
  return request({
    url: '/wms/iqcItem/list',
    method: 'get',
    params: { categoryId }
  })
}

// 按物料获取检验项(含物料分类及其所有祖先分类的检验项)
export function listIqcItemByMaterial(materialId) {
  return request({
    url: '/wms/iqcItem/listByMaterial',
    method: 'get',
    params: { materialId }
  })
}

// 批量保存(事务内先删后插)
export function batchSaveIqcItem(categoryId, data) {
  return request({
    url: '/wms/iqcItem/batchSave',
    method: 'post',
    params: { categoryId },
    data: data
  })
}

// 删除检验项
export function delIqcItem(id) {
  return request({
    url: '/wms/iqcItem/' + id,
    method: 'delete'
  })
}
