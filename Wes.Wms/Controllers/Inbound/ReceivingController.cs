using Microsoft.AspNetCore.Mvc;
using Wes.Business;
using Wes.Model;
using Wes.Utils.Model;
using Wes.Model;

namespace Wes.WebApi.Areas.WmsManage
{
    /// <summary>
    /// 收货单
    /// </summary>
    [ApiController]
    [Route("wms/receiving")]
    [ApiExplorerSettings(GroupName = "Inbound")]
    public class ReceivingController : ControllerBase
    {
        private readonly IReceivingBiz _receivingBiz;

        public ReceivingController(IReceivingBiz receivingBiz)
        {
            _receivingBiz = receivingBiz;
        }

        [HttpGet("list")]
        public ReturnData GetList([FromQuery] ParamData<WmsReceivingDetailParam> param)
        {
            return _receivingBiz.GetList(param);
        }

        [HttpGet("{id}")]
        public ReturnData GetById(long id)
        {
            return _receivingBiz.GetById(id);
        }

        [HttpGet("detailList/{receivingId}")]
        public ReturnData GetDetailList(long receivingId)
        {
            return _receivingBiz.GetDetailList(receivingId);
        }

        [HttpPost]
        public ReturnData Add([FromBody] WmsReceivingSaveDto dto)
        {
            if (dto != null && dto.Receiving != null)
                dto.Receiving.ReceivingId = 0;
            return _receivingBiz.Save(dto);
        }

        [HttpPut]
        public ReturnData Update([FromBody] WmsReceivingSaveDto dto)
        {
            return _receivingBiz.Save(dto);
        }

        [HttpDelete("{ids}")]
        public ReturnData Delete(string ids)
        {
            return _receivingBiz.Delete(ids);
        }

        /// <summary>
        /// 确认收货：免检生成入库单，需检生成检验单
        /// </summary>
        [HttpPut("confirmReceipt/{receivingId}")]
        public ReturnData ConfirmReceipt(long receivingId)
        {
            return _receivingBiz.ConfirmReceipt(receivingId);
        }
    }
}
