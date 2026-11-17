using System;
using SqlSugar;
using Wes.Model;
using Wes.Service;
using Wes.Utils;
using Wes.Utils.Extension;
using Wes.Utils.Model;

namespace Wes.Business
{
	/// <summary>
	/// 往来单位 业务逻辑
	/// </summary>
	public class PartnerBiz : IPartnerBiz
	{
		private readonly IPartnerService _partnerService;
		private readonly IMaterialSupplierService _materialSupplierService;
		private readonly ISqlSugarClient _client;

		public PartnerBiz(IPartnerService partnerService, IMaterialSupplierService materialSupplierService,
			ISqlSugarClient sqlSugarClient)
		{
			_partnerService = partnerService;
			_materialSupplierService = materialSupplierService;
			_client = sqlSugarClient;
		}

		public RowData<WmsPartnerEntity> GetList(ParamData<WmsPartnerParam> param)
		{
			int total = 0;
			var result = new RowData<WmsPartnerEntity>(_partnerService.GetList(param, out total))
			{
				total = total
			};
			return result;
		}

		public ResultData<WmsPartnerEntity> GetById(long id)
		{
			return new ResultData<WmsPartnerEntity>(_partnerService.GetById(id));
		}

		public ReturnData Save(WmsPartnerEntity model)
		{
			return _partnerService.Save(model)
				? new ReturnData()
				: new ReturnData(500, "保存失败！");
		}

		public ReturnData Delete(string ids)
		{
			var delIds = ids.ToLongList();
			if (delIds == null || delIds.Count == 0)
				return new ReturnData(500, "参数有误！");

			_client.Ado.BeginTran();
			try
			{
				// 级联软删除物料供应商关联表
				foreach (var id in delIds)
					_materialSupplierService.DeleteBySupplierId(id);

				// 软删除往来单位主表
				if (!_partnerService.Delete(delIds))
				{
					_client.Ado.RollbackTran();
					return new ReturnData(500, "删除失败！");
				}

				_client.Ado.CommitTran();
			}
			catch (Exception ex)
			{
				_client.Ado.RollbackTran();
				return new ReturnData(500, $"删除失败：{ex.Message}");
			}

			return new ReturnData();
		}

		public ReturnData ChangeStatus(WmsPartnerEntity model)
		{
			if (_partnerService.GetById(model.PartnerId) == null)
				return new ReturnData(500, "往来单位不存在！");
			return _partnerService.ChangeStatus(model)
				? new ReturnData()
				: new ReturnData(500, "状态修改失败！");
		}
	}
}
