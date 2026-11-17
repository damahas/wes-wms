import request from "@/utils/request";

// 检验单列表
export function listIqcOrder(query) {
  return request({ url: "/wms/iqcOrder/list", method: "get", params: query });
}

// 检验单详情
export function getIqcOrder(id) {
  return request({ url: `/wms/iqcOrder/${id}`, method: "get" });
}

// 保存检验单（inspection + items）— 必须用 PUT，POST 是新增接口会强制清零 InspectionId 导致重复新增
export function updateIqcOrder(data) {
  return request({ url: "/wms/iqcOrder", method: "put", data });
}

// 完成检验（回写收货单明细）
export function completeIqcOrder(id) {
  return request({ url: `/wms/iqcOrder/${id}/complete`, method: "put" });
}

// 检验项列表
export function getIqcOrderItems(inspectionId) {
  return request({ url: `/wms/iqcOrder/${inspectionId}/items`, method: "get" });
}
