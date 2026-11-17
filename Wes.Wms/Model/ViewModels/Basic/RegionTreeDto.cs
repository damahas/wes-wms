using System.Collections.Generic;

namespace Wes.Model
{
	/// <summary>
	/// 省市区树节点（value/label/children 对应 el-cascader 默认字段）
	/// </summary>
	public class RegionTreeDto
	{
		public string Value { get; set; } = "";
		public string Label { get; set; } = "";
		public List<RegionTreeDto> Children { get; set; } = new List<RegionTreeDto>();
	}
}
