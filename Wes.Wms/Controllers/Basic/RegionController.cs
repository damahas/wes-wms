using Microsoft.AspNetCore.Mvc;
using Wes.Business;
using Wes.Model;
using Wes.Utils.Model;

namespace Wes.WebApi.Areas.WmsManage
{
	/// <summary>
	/// 省市区（行政区划）
	/// </summary>
	[ApiController]
	[Route("wms/region")]
	[ApiExplorerSettings(GroupName = "Basic")]
	public class RegionController : ControllerBase
	{
		private readonly IRegionBiz _regionBiz;

		public RegionController(IRegionBiz regionBiz)
		{
			_regionBiz = regionBiz;
		}

		/// <summary>
		/// 查询全部省市区（扁平列表）
		/// </summary>
		[HttpGet("list")]
		public ReturnData GetList([FromQuery] WmsRegionParam param)
		{
			return _regionBiz.GetList(param);
		}

		/// <summary>
		/// 查询省市区树（供级联选择）
		/// </summary>
		[HttpGet("tree")]
		public ReturnData GetTree()
		{
			return _regionBiz.GetTree();
		}
	}
}
