using System.Collections.Generic;
using Wes.Model;
using Wes.Service;
using Wes.Utils.Model;

namespace Wes.Business
{
	/// <summary>
	/// 省市区 业务逻辑
	/// </summary>
	public class RegionBiz : IRegionBiz
	{
		private readonly IRegionService _regionService;

		public RegionBiz(IRegionService regionService)
		{
			_regionService = regionService;
		}

		public RowData<WmsRegionEntity> GetList(WmsRegionParam param)
		{
			int total = 0;
			var result = new RowData<WmsRegionEntity>(_regionService.GetList(param, out total))
			{
				total = total
			};
			return result;
		}

		public ResultData<List<RegionTreeDto>> GetTree()
		{
			return new ResultData<List<RegionTreeDto>>(_regionService.GetTree());
		}
	}
}
