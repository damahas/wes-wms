using System;
using System.Collections.Generic;
using System.Text;

namespace Wes.Utils.Model
{
    public class LicenseModel
    {
        /// <summary>
        /// 客户名称
        /// </summary>
        public string CustomerName { set; get; }

        /// <summary>
        /// 许可证版本
        /// trial 试用版 enterpriseTrial 企业试运行版 enterprise 企业正式版
        /// </summary>
        public string LicenseType { set; get; }

        /// <summary>
        /// 激活时间
        /// </summary>
        public DateTime ActivateTime { set; get; }

        /// <summary>
        /// 过期时间
        /// </summary>
        public DateTime ExpireTime { set; get; }

        /// <summary>
        /// 刷新时间
        /// </summary>
        public DateTime RefreshTime { set; get; }
    }

    /// <summary>
    /// 许可证版本（LicenseModel.LicenseType 取值）
    /// </summary>
    public static class LicenseType
    {
        /// <summary>
        /// 试用版
        /// </summary>
        public const string Trial = "trial";

        /// <summary>
        /// 企业试运行版
        /// </summary>
        public const string EnterpriseTrial = "enterpriseTrial";

        /// <summary>
        /// 企业正式版
        /// </summary>
        public const string Enterprise = "enterprise";
    }
}
