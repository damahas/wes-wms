using System.Collections.Generic;
using SqlSugar;
using Wes.Model;

namespace Wes.Service
{
    /// <summary>
    /// 物料供应商关联 数据访问接口
    /// </summary>
    public interface IMaterialSupplierService
    {
        bool DeleteByMaterialId(long materialId);
        bool DeleteBySupplierId(long supplierId);
        bool BatchSave(long materialId, List<WmsMaterialSupplierEntity> suppliers);
        string GetInspectionMode(long materialId, long? supplierId);
    }
}
