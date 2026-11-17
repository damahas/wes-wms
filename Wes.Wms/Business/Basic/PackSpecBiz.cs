using System;
using Wes.Model;
using Wes.Service;
using Wes.Utils;
using Wes.Utils.Extension;
using Wes.Utils.Model;

namespace Wes.Business
{
    /// <summary>
    /// 包装规格 业务逻辑
    /// </summary>
    public class PackSpecBiz : IPackSpecBiz
    {
        private readonly IPackSpecService _packSpecService;

        public PackSpecBiz(IPackSpecService packSpecService)
        {
            _packSpecService = packSpecService;
        }

        public RowData<WmsPackSpecEntity> GetList(ParamData<WmsPackSpecParam> param)
        {
            int total = 0;
            var result = new RowData<WmsPackSpecEntity>(_packSpecService.GetList(param, out total))
            {
                total = total
            };
            return result;
        }

        public ResultData<WmsPackSpecEntity> GetById(long id)
        {
            return new ResultData<WmsPackSpecEntity>(_packSpecService.GetById(id));
        }

        public RowData<WmsPackSpecEntity> GetTree(long rootId)
        {
            return new RowData<WmsPackSpecEntity>(_packSpecService.GetTree(rootId));
        }

        public ReturnData Save(WmsPackSpecEntity model)
        {
            if (!_packSpecService.Save(model))
                return new ReturnData(500, "保存失败！");

            // 业务逻辑：顶层节点的 root_id 指向自身，便于整棵归组
            if (model.SpecId == 0 && model.ParentId == null && (model.RootId == null || model.RootId == 0))
            {
                model.RootId = model.SpecId;
                if (!_packSpecService.Save(model))
                    return new ReturnData(500, "保存失败！");
            }
            return new ReturnData();
        }

        public ReturnData Delete(string ids)
        {
            var delIds = ids.ToLongList();
            if (delIds == null || delIds.Count == 0)
                return new ReturnData(500, "参数有误！");

            foreach (var id in delIds)
            {
                var node = _packSpecService.GetById(id);
                if (node == null) continue;

                // 业务逻辑：顶层节点 → 级联整棵；子节点 → 级联自身及直属子节点
                if (node.ParentId == null)
                    _packSpecService.SoftDeleteByRootId(id);
                else
                    _packSpecService.SoftDeleteSelfAndChildren(id);
            }
            return new ReturnData();
        }
    }
}
