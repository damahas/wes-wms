import request from '@/utils/request'

// 查询入库单列表
export function listReceipt(query) {
  return request({
    url: '/wms/receipt/list',
    method: 'get',
    params: query
  })
}

// 查询入库单详情
export function getReceipt(id) {
  return request({
    url: '/wms/receipt/' + id,
    method: 'get'
  })
}

// 新增入库单
export function addReceipt(data) {
  return request({
    url: '/wms/receipt',
    method: 'post',
    data: data
  })
}

// 修改入库单
export function updateReceipt(data) {
  return request({
    url: '/wms/receipt',
    method: 'put',
    data: data
  })
}

// 删除入库单
export function delReceipt(ids) {
  return request({
    url: '/wms/receipt/' + ids,
    method: 'delete'
  })
}

// 确认入库（兼容保留：全部明细已上架才允许）
export function confirmReceipt(id) {
  return request({
    url: '/wms/receipt/' + id + '/confirm',
    method: 'put'
  })
}

// 明细上架
export function shelveReceiptDetail(receiptId, detailId, data) {
  return request({
    url: `/wms/receipt/${receiptId}/details/${detailId}/shelve`,
    method: 'put',
    data: data
  })
}

// 按条码上架（PDA）
export function shelveByBarcode(data) {
  return request({
    url: '/wms/receipt/shelve-by-barcode',
    method: 'put',
    data: data
  })
}

// 明细条码列表
export function getReceiptBarcodes(receiptId, detailId) {
  return request({
    url: `/wms/receipt/${receiptId}/details/${detailId}/barcodes`,
    method: 'get'
  })
}

// 拆分条码：一个码拆成多个（quantities 之和 = 原数量）
export function splitReceiptBarcode(receiptId, detailId, data) {
  return request({
    url: `/wms/receipt/${receiptId}/details/${detailId}/barcodes/split`,
    method: 'put',
    data: data
  })
}

// 合并条码：多个码合并为一个（数量相加）
export function mergeReceiptBarcodes(receiptId, detailId, data) {
  return request({
    url: `/wms/receipt/${receiptId}/details/${detailId}/barcodes/merge`,
    method: 'put',
    data: data
  })
}

// 按条码查询入库明细（PDA 扫码回显）
export function getReceiptDetailByBarcode(barcode) {
  return request({
    url: '/wms/receipt/barcode/' + barcode,
    method: 'get'
  })
}
