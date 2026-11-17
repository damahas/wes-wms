using System;
using System.Collections.Generic;
using System.Text;

namespace Wes.ViewModel.SystemManage
{
    /// <summary>
    /// 两步验证通用入参（中间票据 + 验证码 + 方式）
    /// </summary>
    public class TwoFactorParam
    {
        /// <summary>两步验证中间票据（第一阶段返回）</summary>
        public string ChallengeToken { get; set; }

        /// <summary>绑定/验证方式：email / totp</summary>
        public string Mode { get; set; }

        /// <summary>验证码（邮箱验证码 / 验证器动态码 / 恢复码）</summary>
        public string Code { get; set; }
    }

    /// <summary>
    /// 解绑两步验证入参
    /// </summary>
    public class TwoFactorUnbindParam
    {
        /// <summary>登录密码</summary>
        public string Password { get; set; }

        /// <summary>验证码（动态码或恢复码）</summary>
        public string Code { get; set; }
    }
}
