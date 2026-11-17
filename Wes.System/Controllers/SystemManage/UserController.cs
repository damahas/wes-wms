using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wes.Business;
using Wes.Entity;
using Wes.Utils;
using Wes.Utils.Model;
using Wes.ViewModel.SystemManage;

namespace Wes.WebApi.Areas.SystemManage
{
    [ApiController]
    [Authorize]
    [Route("system/user")]
    public class UserController : ControllerBase
    {
        private ISysUserBiz _sysUserBiz;
        private ISysTwoFactorBiz _twoFactorBiz;

        public UserController(ISysUserBiz sysUserBiz, ISysTwoFactorBiz twoFactorBiz)
        {
            this._sysUserBiz = sysUserBiz;
            this._twoFactorBiz = twoFactorBiz;
        }

        [HttpGet]
        [Route("list")]
        public ReturnData GetList([FromQuery] ParamData<UserParam> param)
        {
            return _sysUserBiz.GetList(param);
        }

        [HttpGet]
        [Route("{id}")]
        public ReturnData GetById(long id)
        {
            return _sysUserBiz.GetById(id);
        }

        [HttpPut]
        public ReturnData Update([FromBody] UserModel dept)
        {
            return _sysUserBiz.Save(dept);
        }

        [HttpPost]
        public ReturnData Insert([FromBody] UserModel dept)
        {
            return _sysUserBiz.Save(dept);
        }

        [HttpPost]
        [Route("export")]
        public FileContentResult Export([FromForm] ParamData<UserParam> param)
        {
            return File(_sysUserBiz.Export(param), "application/ms-excel");
        }

        [HttpDelete]
        [Route("{ids}")]
        public ReturnData Delete(string ids)
        {
            return _sysUserBiz.Delete(ids);
        }

        [HttpPut]
        [Route("changeStatus")]
        public ReturnData ChangeStatus([FromBody] UserStatusParam userStatusParam)
        {
            return _sysUserBiz.ChangeStatus(userStatusParam.userId, userStatusParam.status);
        }

        [HttpGet]
        [Route("authRole/{id}")]
        public ReturnData GetUserRole(long id)
        {
            return _sysUserBiz.GetUserRole(id);
        }

        [HttpPut]
        [Route("authRole")]
        public ReturnData SaveUserRole(long userId, string roleIds)
        {
            return _sysUserBiz.SaveUserRole(userId, roleIds);
        }

        [HttpPut]
        [Route("resetPwd")]
        public ReturnData ResetUserPwd([FromBody] UserPwdParam userPwdParam)
        {
            return _sysUserBiz.ResetUserPwd(userPwdParam);
        }

        [HttpGet]
        [Route("2fa/status")]
        public ReturnData GetTwoFactorStatus()
        {
            return new ResultData<TwoFactorBindInfo>(_twoFactorBiz.GetBindInfo());
        }

        /// <summary>
        /// 绑定准备：totp 方式返回密钥与扫码地址，email 方式发送验证码
        /// 验证方式由参数配置 sys.login.twoFactor 决定
        /// </summary>
        [HttpGet]
        [Route("2fa/prepare")]
        public ReturnData PrepareTwoFactor()
        {
            return new ResultData<TwoFactorSecretInfo>(_twoFactorBiz.PrepareBind());
        }

        /// <summary>
        /// 确认绑定两步验证（返回一次性恢复码）
        /// </summary>
        [HttpPost]
        [Route("2fa/bind")]
        public ReturnData ConfirmTwoFactor([FromBody] TwoFactorParam param)
        {
            return _twoFactorBiz.ConfirmBind(param);
        }

        /// <summary>
        /// 解绑两步验证（需登录密码 + 验证码）
        /// </summary>
        [HttpPost]
        [Route("2fa/unbind")]
        public ReturnData UnbindTwoFactor([FromBody] TwoFactorUnbindParam param)
        {
            return _twoFactorBiz.Unbind(param);
        }

        /// <summary>
        /// 管理员重置用户两步验证
        /// </summary>
        [HttpPut]
        [Route("2fa/reset/{userId}")]
        public ReturnData ResetTwoFactor(long userId)
        {
            return _twoFactorBiz.Reset(userId);
        }

        /// <summary>
        /// 修改当前用户密码（密码经前端 RSA 加密，走 body 传输避免进入访问日志）
        /// </summary>
        [HttpPut]
        [Route("profile/updatePwd")]
        public ReturnData UpdateCurrentUserPwd([FromBody] UpdatePwdParam param)
        {
            return _sysUserBiz.UpdateCurrentUserPwd(param?.OldPassword, param?.NewPassword);
        }

        [HttpGet]
        [Route("profile")]
        public ReturnData GetCurrentUser()
        {
            return new ProfileInfo<UserInfo>()
            {
                Code = 200,
                Data = GlobalContext.CurrentUser
            };
        }

        [HttpPut]
        [Route("profile")]
        public ReturnData SaveCurrentUser([FromBody] UserInfo userInfo)
        {
            return _sysUserBiz.SaveCurrentUser(userInfo);
        }
    }
}