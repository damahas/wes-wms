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
	/// 物料 业务逻辑
	/// </summary>
	public class MaterialBiz : IMaterialBiz
	{
		private readonly IMaterialService _materialService;
		private readonly IMaterialSupplierService _materialSupplierService;
		private readonly ISqlSugarClient _client;

		public MaterialBiz(IMaterialService materialService, IMaterialSupplierService materialSupplierService,
			ISqlSugarClient sqlSugarClient)
		{
			_materialService = materialService;
			_materialSupplierService = materialSupplierService;
			_client = sqlSugarClient;
		}

		public RowData<WmsMaterialEntity> GetList(ParamData<WmsMaterialParam> param)
		{
			int total = 0;
			var result = new RowData<WmsMaterialEntity>(_materialService.GetList(param, out total))
			{
				total = total
			};
			return result;
		}

		public ResultData<WmsMaterialEntity> GetById(long id)
		{
			return new ResultData<WmsMaterialEntity>(_materialService.GetById(id));
		}

		public ResultData<long> Save(WmsMaterialEntity model)
		{
			var account = GlobalContext.CurrentUser?.Account;

			_client.Ado.BeginTran();
			try
			{
				// 1. 保存主表
				if (!_materialService.Save(model))
				{
					_client.Ado.RollbackTran();
					return new ResultData<long>(500, "保存物料失败！");
				}

				// 2. 级联供应商子表：先删后插
				if (model.Suppliers != null)
				{
					if (model.MaterialId > 0)
						_materialSupplierService.DeleteByMaterialId(model.MaterialId);

					_materialSupplierService.BatchSave(model.MaterialId, model.Suppliers);
				}

				_client.Ado.CommitTran();
			}
			catch (Exception ex)
			{
				_client.Ado.RollbackTran();
				return new ResultData<long>(500, $"保存失败：{ex.Message}");
			}

			return new ResultData<long>(model.MaterialId);
		}

		public ReturnData Delete(string ids)
		{
			var delIds = ids.ToLongList();
			if (delIds == null || delIds.Count == 0)
				return new ReturnData(500, "参数有误！");

			_client.Ado.BeginTran();
			try
			{
				// 级联软删除供应商子表
				foreach (var id in delIds)
					_materialSupplierService.DeleteByMaterialId(id);

				// 软删除主表
				if (!_materialService.Delete(delIds))
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

		public ReturnData ChangeStatus(WmsMaterialEntity model)
		{
			if (_materialService.GetById(model.MaterialId) == null)
				return new ReturnData(500, "物料不存在！");
			return _materialService.ChangeStatus(model)
				? new ReturnData()
				: new ReturnData(500, "状态修改失败！");
		}
	}
}
