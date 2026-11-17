using System.Collections.Generic;

namespace Wes.Model
{
    /// <summary>
    /// 收货单 主子表保存DTO
    /// </summary>
    public class WmsReceivingSaveDto
    {
        /// <summary>收货单主表</summary>
        public WmsReceivingEntity Receiving { get; set; }

        /// <summary>收货明细列表</summary>
        public List<WmsReceivingDetailEntity> Details { get; set; }
    }
}
