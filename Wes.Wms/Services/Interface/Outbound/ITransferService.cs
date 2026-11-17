using System.Collections.Generic;
using SqlSugar;
using Wes.Model;
using Wes.Utils.Model;

namespace Wes.Service
{
    /// <summary>
    /// 调拨单 数据访问接口
    /// </summary>
    public interface ITransferService
    {
        List<WmsTransferEntity> GetList(ParamData<WmsTransferParam> param, out int total);
        WmsTransferEntity? GetById(long id);
        /// <summary>加载有效明细（过滤软删行）</summary>
        List<WmsTransferDetailEntity> GetDetailsByTransferId(long transferId);
        bool Save(WmsTransferEntity model);
        bool Delete(List<long> ids);
        bool DeleteDetailsByTransferId(long transferId);
        bool BatchSaveDetails(long transferId, List<WmsTransferDetailEntity> details);
    }
}
