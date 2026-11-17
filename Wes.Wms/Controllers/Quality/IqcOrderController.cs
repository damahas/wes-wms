using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Wes.Business;
using Wes.Model;
using Wes.Utils.Model;

namespace Wes.WebApi.Areas.WmsManage
{
	/// <summary>
	/// 检验单
	/// </summary>
	[ApiController]
	[Route("wms/iqcOrder")]
	[ApiExplorerSettings(GroupName = "Inbound")]
	public class IqcOrderController : ControllerBase
	{
		private readonly IIqcOrderBiz _iqcOrderBiz;

		public IqcOrderController(IIqcOrderBiz iqcOrderBiz)
		{
			_iqcOrderBiz = iqcOrderBiz;
		}

		[HttpGet("list")]
		public ReturnData GetList([FromQuery] ParamData<WmsIqcOrderParam> param)
		{
			return _iqcOrderBiz.GetList(param);
		}

		[HttpGet("{id}")]
		public ReturnData GetById(long id)
		{
			return _iqcOrderBiz.GetById(id);
		}

		[HttpGet("{inspectionId}/items")]
		public ReturnData GetItems(long inspectionId)
		{
			return _iqcOrderBiz.GetItems(inspectionId);
		}

		/// <summary>
		/// 来料不良台账
		/// </summary>
		[HttpGet("defect/list")]
		public ReturnData GetDefectList([FromQuery] ParamData<WmsIqcDefectParam> param)
		{
			return _iqcOrderBiz.GetDefectList(param);
		}

		/// <summary>
		/// 检验单的不合格(NG)检验项
		/// </summary>
		[HttpGet("{inspectionId}/ngItems")]
		public ReturnData GetNgItems(long inspectionId)
		{
			return _iqcOrderBiz.GetNgItems(inspectionId);
		}

		[HttpPost]
		public ReturnData Add([FromBody] WmsIqcOrderSaveDto dto)
		{
			if (dto != null && dto.Inspection != null)
				dto.Inspection.InspectionId = 0;
			return _iqcOrderBiz.Save(dto);
		}

		[HttpPut]
		public ReturnData Update([FromBody] WmsIqcOrderSaveDto dto)
		{
			return _iqcOrderBiz.Save(dto);
		}

		[HttpPut("{inspectionId}/complete")]
		public ReturnData Complete(long inspectionId)
		{
			return _iqcOrderBiz.Complete(inspectionId);
		}

		[HttpDelete("{ids}")]
		public ReturnData Delete(string ids)
		{
			return _iqcOrderBiz.Delete(ids);
		}
	}
}
