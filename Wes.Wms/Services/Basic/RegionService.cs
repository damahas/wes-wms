using System;
using System.Collections.Generic;
using System.Linq;
using SqlSugar;
using Wes.Model;
using Wes.Utils.Model;

namespace Wes.Service
{
	/// <summary>
	/// 省市区 数据访问
	/// </summary>
	public class RegionService : Repository<WmsRegionEntity>, IRegionService
	{
		public RegionService(ISqlSugarClient db) : base(db) { }

		public List<WmsRegionEntity> GetList(WmsRegionParam param, out int total)
		{
			total = 0;
			var query = Context.Queryable<WmsRegionEntity>();
			if (param != null)
			{
				if (!string.IsNullOrWhiteSpace(param.Code))
					query = query.Where(p => p.Code.Contains(param.Code.Trim()));
				if (!string.IsNullOrWhiteSpace(param.Name))
					query = query.Where(p => p.Name.Contains(param.Name.Trim()));
			}
			return param != null && param.PageSize != 0
				? query.OrderBy(p => p.Code).ToPageList(param.PageNum, param.PageSize, ref total)
				: query.OrderBy(p => p.Code).ToList();
		}

		public List<RegionTreeDto> GetTree()
		{
			var all = Context.Queryable<WmsRegionEntity>().OrderBy(p => p.Code).ToList();
			// 以空字符串代表顶级（parent_code 为 NULL 的省）
			var map = all.GroupBy(p => p.ParentCode ?? "")
						.ToDictionary(g => g.Key, g => g.ToList());

			List<RegionTreeDto> Build(string parentCode)
			{
				if (!map.TryGetValue(parentCode, out var children))
					return new List<RegionTreeDto>();
				return children.Select(c => new RegionTreeDto
				{
					Value = c.Code,
					Label = c.Name,
					Children = Build(c.Code)
				}).ToList();
			}
			return Build("");
		}
	}
}
