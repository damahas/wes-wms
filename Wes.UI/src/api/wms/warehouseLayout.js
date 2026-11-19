import request from '@/utils/request'

// 查询仓库平面布局
export function getWarehouseLayout(warehouseId) {
  return request({
    url: '/wms/warehouseLayout/' + warehouseId,
    method: 'get'
  })
}

// 保存仓库平面布局
export function saveWarehouseLayout(data) {
  return request({
    url: '/wms/warehouseLayout',
    method: 'put',
    data: data
  })
}

// 发布仓库平面布局
export function publishWarehouseLayout(id) {
  return request({
    url: '/wms/warehouseLayout/publish/' + id,
    method: 'put'
  })
}
