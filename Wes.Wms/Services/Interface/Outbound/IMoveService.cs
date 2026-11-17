using System.Collections.Generic;
using SqlSugar;
using Wes.Model;
using Wes.Utils.Model;

namespace Wes.Service
{
    /// <summary>
    /// 移库单 数据访问接口
    /// </summary>
    public interface IMoveService
    {
        List<WmsMoveEntity> GetList(ParamData<WmsMoveParam> param, out int total);
        WmsMoveEntity? GetById(long id);
        bool Save(WmsMoveEntity model);
        bool Delete(List<long> ids);
        bool DeleteDetailsByMoveId(long moveId);
        bool BatchSaveDetails(long moveId, List<WmsMoveDetailEntity> details);
    }
}
