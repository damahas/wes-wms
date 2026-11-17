import request from "@/utils/request";

// 来料不良台账列表
export function listIqcDefect(query) {
  return request({ url: "/wms/iqcOrder/defect/list", method: "get", params: query });
}

// 某检验单的不合格(NG)检验项
export function getIqcNgItems(inspectionId) {
  return request({ url: `/wms/iqcOrder/${inspectionId}/ngItems`, method: "get" });
}
