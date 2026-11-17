namespace Wes.Model
{
    /// <summary>
    /// 收货单明细 查询参数
    /// </summary>
    public class WmsReceivingDetailParam
    {
        public string? Keywords { get; set; }
        public long? SupplierId { get; set; }
        public string? ReceivingStatus { get; set; }
    }
}
