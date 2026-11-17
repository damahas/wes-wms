using Wes.Model;
using Wes.Utils.Model;

namespace Wes.Business
{
    /// <summary>
    /// 收货单 业务逻辑接口
    /// </summary>
    public interface IReceivingBiz
    {
        RowData<WmsReceivingEntity> GetList(ParamData<WmsReceivingDetailParam> param);
        ResultData<WmsReceivingEntity> GetById(long id);
        ReturnData Save(WmsReceivingSaveDto dto);
        ReturnData Delete(string ids);
        ResultData<List<WmsReceivingDetailEntity>> GetDetailList(long receivingId);
        ReturnData ConfirmReceipt(long receivingId);
    }
}
