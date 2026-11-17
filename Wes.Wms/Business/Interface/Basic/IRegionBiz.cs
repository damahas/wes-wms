using System.Collections.Generic;
using Wes.Model;
using Wes.Utils.Model;

namespace Wes.Business
{
	/// <summary>
	/// 省市区 业务逻辑接口
	/// </summary>
	public interface IRegionBiz
	{
		RowData<WmsRegionEntity> GetList(WmsRegionParam param);
		ResultData<List<RegionTreeDto>> GetTree();
	}
}
