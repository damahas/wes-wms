using Wes.Model;
using Wes.Utils.Model;

namespace Wes.Service
{
	/// <summary>
	/// 实时库存 数据访问接口
	/// </summary>
	public interface IStockService
	{
		List<WmsStockEntity> GetList(ParamData<WmsStockParam> param, out int total);
		WmsStockEntity? GetById(long id);
		bool Save(WmsStockEntity model);

		/// <summary>按条码取库存（PDA 扫码用）</summary>
		WmsStockEntity GetByBarcode(string barcode);

		/// <summary>上架写入库存：同条码（或物料+批号+库位）已存在则累加数量，否则新增</summary>
		bool AddOrUpdateStock(WmsStockEntity model);
		bool Delete(List<long> ids);
	}
}
