using System;
using System.Collections.Generic;
using System.Text;
using Wes.Entity;
using Wes.Utils.Model;
using Wes.ViewModel;
using Wes.ViewModel.SystemManage;

namespace Wes.Business
{
    public interface ISysUserBiz
    {
        #region 登录登出

        /// <summary>
        /// 登录（第一因素），两步验证开启时返回 TwoFactorLoginData
        /// </summary>
        public ReturnData Login(LoginInfo login);

        /// <summary>
        /// 签发两步验证中间票据
        /// </summary>
        public string IssueChallengeToken(long userId);

        /// <summary>
        /// 签发正式 token（第二因素通过后）
        /// </summary>
        public ReturnData IssueToken(SysUserEntity user);

        /// <summary>
        /// 管理员重置用户两步验证
        /// </summary>
        public ReturnData ResetTwoFactor(long userId);

        public ReturnData Logout(string token);

        public bool IsCaptchaEnabled();

        #endregion

        public bool TryGetUser(string token, out UserInfo userInfo);

        #region 用户操作

        public RowData<SysUserEntity> GetList(ParamData<UserParam> param);

        public UserReturnInfo GetById(long id);

        public ReturnData Save(UserModel user);

        public ReturnData Delete(string ids);

        public byte[] Export(ParamData<UserParam> param);

        public ReturnData ChangeStatus(long userId, string status);

        #endregion

        #region 角色用户

        public RowData<SysUserEntity> GetRoleUserList(ParamData<RoleUserParam> param);

        public RowData<SysUserEntity> GetRoleNoExistUserList(ParamData<RoleUserParam> param);

        public ResultData<object> GetUserRole(long id);

        public ReturnData SaveUserRole(long userId, string roleIds);

        #endregion

        #region 密码

        public ReturnData ResetUserPwd(UserPwdParam userPwdParam);

        public ReturnData UpdateCurrentUserPwd(string oldPassword, string newPassword);

        #endregion

        #region 个人中心

        public ReturnData SaveCurrentUser(UserInfo userInfo);

        #endregion
    }
}
