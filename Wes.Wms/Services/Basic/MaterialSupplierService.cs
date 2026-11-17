using System;
using System.Collections.Generic;
using SqlSugar;
using Wes.Model;
using Wes.Utils;

namespace Wes.Service
{
    /// <summary>
    /// 物料供应商关联 数据访问
    /// </summary>
    public class MaterialSupplierService : Repository<WmsMaterialSupplierEntity>, IMaterialSupplierService
    {
        public MaterialSupplierService(ISqlSugarClient db) : base(db) { }

        public bool DeleteByMaterialId(long materialId)
        {
            var account = GlobalContext.CurrentUser?.Account;
            var now = DateTime.Now;
            return Context.Updateable<WmsMaterialSupplierEntity>()
                .SetColumns(p => new WmsMaterialSupplierEntity { IsDel = 1, UpdateTime = now, UpdateBy = account })
                .Where(p => p.MaterialId == materialId && p.IsDel == 0)
                .ExecuteCommand() > 0;
        }

        public bool DeleteBySupplierId(long supplierId)
        {
            var account = GlobalContext.CurrentUser?.Account;
            var now = DateTime.Now;
            return Context.Updateable<WmsMaterialSupplierEntity>()
                .SetColumns(p => new WmsMaterialSupplierEntity { IsDel = 1, UpdateTime = now, UpdateBy = account })
                .Where(p => p.SupplierId == supplierId && p.IsDel == 0)
                .ExecuteCommand() > 0;
        }

        public bool BatchSave(long materialId, List<WmsMaterialSupplierEntity> suppliers)
        {
            if (suppliers == null || suppliers.Count == 0) return true;
            var account = GlobalContext.CurrentUser?.Account;
            var now = DateTime.Now;
            foreach (var s in suppliers)
            {
                s.MaterialSupplierId = 0;
                s.MaterialId = materialId;
                s.IsDel = 0;
                s.CreateBy = account;
                s.CreateTime = now;
                s.MaterialSupplierId = Context.Insertable(s).ExecuteReturnSnowflakeId();
            }
            return true;
        }

        public string GetInspectionMode(long materialId, long? supplierId)
        {
            if (supplierId == null) return null;
            return Context.Queryable<WmsMaterialSupplierEntity>()
                .Where(p => p.MaterialId == materialId && p.SupplierId == supplierId.Value && p.IsDel == 0)
                .Select(p => p.InspectionMode)
                .First();
        }
    }
}
