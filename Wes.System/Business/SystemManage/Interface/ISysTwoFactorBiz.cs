using System;
using System.Collections.Generic;
using System.Text;
using Wes.Utils.Model;
using Wes.ViewModel.SystemManage;

namespace Wes.Business
{
    public interface ISysTwoFactorBiz
    {
        #region 登录第二阶段（凭中间票据）

        /// <summary>
        /// 校验验证码并完成登录（换取正式 token）
        /// </summary>
        public ReturnData Verify(TwoFactorParam param);

        /// <summary>
        /// 发送邮箱验证码（邮箱方式）
        /// </summary>
        public ReturnData SendEmailCode(TwoFactorParam param);

        /// <summary>
        /// 强制绑定前的准备（TOTP 方式返回密钥与扫码地址）
        /// </summary>
        public ReturnData PrepareLoginBind(TwoFactorParam param);

        /// <summary>
        /// 首次登录强制绑定两步验证，成功后直接完成登录
        /// </summary>
        public ReturnData BindAndLogin(TwoFactorParam param);

        #endregion

        #region 已登录用户自助管理

        /// <summary>
        /// 获取当前用户绑定状态
        /// </summary>
        public TwoFactorBindInfo GetBindInfo();

        /// <summary>
        /// 绑定准备（方式由参数配置决定：TOTP 返回密钥与扫码地址；邮件方式立即发送验证码）
        /// </summary>
        public TwoFactorSecretInfo PrepareBind();

        /// <summary>
        /// 确认绑定（校验首个验证码），成功后返回一次性恢复码
        /// </summary>
        public TwoFactorLoginData ConfirmBind(TwoFactorParam param);

        /// <summary>
        /// 解绑（需密码 + 验证码）
        /// </summary>
        public ReturnData Unbind(TwoFactorUnbindParam param);

        /// <summary>
        /// 重新生成恢复码
        /// </summary>

        /// <summary>
        /// 管理员重置用户两步验证
        /// </summary>
        public ReturnData Reset(long userId);

        #endregion
    }
}
