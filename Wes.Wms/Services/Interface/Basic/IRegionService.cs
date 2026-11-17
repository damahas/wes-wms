using System.Collections.Generic;
using Wes.Model;
using Wes.Utils.Model;

namespace Wes.Service
{
	/// <summary>
	/// 省市区 数据访问接口
	/// </summary>
	public interface IRegionService
	{
		List<WmsRegionEntity> GetList(WmsRegionParam param, out int total);
		List<RegionTreeDto> GetTree();
	}
}
