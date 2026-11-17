using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wes.Business;
using Wes.Utils.Model;
using Wes.ViewModel.SystemManage;

namespace Wes.WebApi.Controllers
{
    /// <summary>
    /// 两步验证（第二因素）
    /// 接口需要第一阶段下发的两步验证中间票据或已登录身份
    /// </summary>
    [ApiController]
    [AllowAnonymous]
    [Route("login/2fa")]
    public class TwoFactorController : ControllerBase
    {
        private readonly ISysTwoFactorBiz _twoFactorBiz;

        public TwoFactorController(ISysTwoFactorBiz twoFactorBiz)
        {
            _twoFactorBiz = twoFactorBiz;
        }

        /// <summary>
        /// 校验验证码并完成登录
        /// </summary>
        [HttpPost]
        [Route("verify")]
        public ReturnData Verify([FromBody] TwoFactorParam param)
        {
            return _twoFactorBiz.Verify(param);
        }

        /// <summary>
        /// 发送邮箱验证码
        /// </summary>
        [HttpPost]
        [Route("sendEmailCode")]
        public ReturnData SendEmailCode([FromBody] TwoFactorParam param)
        {
            return _twoFactorBiz.SendEmailCode(param);
        }

        /// <summary>
        /// 绑定准备（验证器方式返回密钥与扫码地址）
        /// </summary>
        [HttpPost]
        [Route("prepareBind")]
        public ReturnData PrepareBind([FromBody] TwoFactorParam param)
        {
            return _twoFactorBiz.PrepareLoginBind(param);
        }

        /// <summary>
        /// 首次登录强制绑定两步验证，绑定成功后直接完成登录
        /// </summary>
        [HttpPost]
        [Route("bind")]
        public ReturnData Bind([FromBody] TwoFactorParam param)
        {
            return _twoFactorBiz.BindAndLogin(param);
        }
    }
}
