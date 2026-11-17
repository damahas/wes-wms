using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json.Linq;
using Wes.Service;
using Wes.ViewModel;
using Wes.Utils.Security;
using Wes.Utils.Extension;
using Wes.Entity;
using Wes.Utils.Model;
using Wes.Utils.Cache;
using Wes.Utils;
using System.Linq;
using Wes.ViewModel.SystemManage;
using Wes.Utils.Hepler;

namespace Wes.Business
{
    public class SysUserBiz : ISysUserBiz
    {
        private ISysUserService _sysUserService;
        private ISysRoleService _sysRoleService;
        private ISysDeptService _sysDeptService;
        private ISysPostService _sysPostService;
        private ISysLoginLogService _sysLoginLogService;
        private ISysDicDataService _sysDicDataService;
        private ISysMenuService _sysMenuService;
        private ISysConfigBiz _sysConfigBiz;
        private ISysUserTwoFactorService _twoFactorService;

        public SysUserBiz(
            ISysUserService sysUserService,
            ISysRoleService sysRoleService,
            ISysDeptService sysDeptService,
            ISysPostService sysPostService,
            ISysLoginLogService sysLoginLogService,
            ISysDicDataService sysDicDataService,
            ISysMenuService sysMenuService,
            ISysConfigBiz sysConfigBiz,
            ISysUserTwoFactorService twoFactorService
            )
        {
            _sysUserService = sysUserService;
            _sysRoleService = sysRoleService;
            _sysDeptService = sysDeptService;
            _sysPostService = sysPostService;
            _sysLoginLogService = sysLoginLogService;
            _sysDicDataService = sysDicDataService;
            _sysMenuService = sysMenuService;
            _sysConfigBiz = sysConfigBiz;
            _twoFactorService = twoFactorService;
        }

        #region 登录登出

        /// <summary>
        /// 登录（第一因素：账号密码 + 滑块验证码）
        /// 两步验证开启时返回中间票据，需调用 system/auth/2fa/verify 完成第二因素换取正式 token
        /// </summary>
        public ReturnData Login(LoginInfo login)
        {
            if (GlobalContext.LicenseModel == null || GlobalContext.LicenseModel.ExpireTime < DateTime.Now)
            {
                return new LoginData(402, "许可证已过期，为了不影响您的使用，请联系服务专员处理");
            }

            if (IsCaptchaEnabled())
            {
                decimal positionX = CacheFactory.Cache.GetCache<decimal>($"{CacheKey.CaptchaVaildImg}{login.code}");

                if (positionX == 0)
                {
                    SaveLoginLog(login.UserName, "1", "验证码已过期");
                    return new LoginData(408, "验证码已过期");
                }
                CacheFactory.Cache.RemoveCache($"{CacheKey.CaptchaVaildImg}{login.code}");
                if (login.positionX < positionX - 0.01m || login.positionX > positionX + 0.01m)
                {
                    SaveLoginLog(login.UserName, "1", "验证码验证失败");
                    return new LoginData(500, "验证码验证失败");
                }
            }

            // 密码由前端 RSA 加密传输，解密失败即视为无效请求（页面停留过久或版本不匹配）
            if (!RsaUtils.TryDecrypt(login.Password, out var loginPassword))
            {
                SaveLoginLog(login.UserName, "1", "登录信息已失效");
                return new LoginData(408, "登录信息已失效，请刷新页面后重试");
            }
            login.Password = AESUtils.Encrypt(MD5Utils.Encrypt(loginPassword));
            var user = _sysUserService.Login(login.UserName, login.Password);
            if (user == null)
            {
                SaveLoginLog(login.UserName, "1", "账号密码错误或找不到用户");
                return new LoginData(500, "账号密码错误或找不到用户");
            }
            if (user.Status == "1")
            {
                SaveLoginLog(login.UserName, "1", "账号已停用");
                return new LoginData(500, "账号已停用");
            }

            // 两步验证：开启时不直接发放正式 token，改为返回一次性中间票据
            var config = _sysConfigBiz.GetTwoFactorConfig();
            if (config.Enabled)
            {
                var needBind = BindRequired(user, config, out var bindMsg);
                if (needBind)
                {
                    return new TwoFactorLoginData(332, bindMsg)
                    {
                        ChallengeToken = IssueChallengeToken(user.UserId),
                        Action = "bind",
                        Mode = config.VerifyMode,
                        Email = MaskEmail(user.Email)
                    };
                }

                if (!needBind && user.TwoFactorBindTime.HasValue)
                {
                    return new TwoFactorLoginData(331, "请输入两步验证验证码")
                    {
                        ChallengeToken = IssueChallengeToken(user.UserId),
                        Action = "verify",
                        Mode = config.VerifyMode,
                        Email = MaskEmail(user.Email)
                    };
                }
            }

            return IssueToken(user);
        }

        /// <summary>
        /// 签发两步验证中间票据（6 分钟有效，不可访问业务接口）
        /// </summary>
        public string IssueChallengeToken(long userId)
        {
            return JWTUtils.GenerateToken(userId.ToString(), JWTUtils.StageTwoFactor, TwoFactorConst.ChallengeMinutes);
        }

        /// <summary>
        /// 签发正式 token（第二因素通过后调用）
        /// </summary>
        public ReturnData IssueToken(SysUserEntity user)
        {
            var token = JWTUtils.GenerateToken(user.UserId.ToString());
            var expireTime = DateTime.Now.AddHours(GlobalContext.JwtSettings.Expires);
            _sysUserService.SaveToken(new SysTokenEntity()
            {
                UserId = user.UserId,
                Token = token,
                CreateTime = DateTime.Now,
                Status = 0,
                LoginIp = NetHepler.Ip,
                LoginLocation = NetHepler.IpLocation,
                Os = NetHepler.Os,
                Browser = NetHepler.Browser,
                ExpirationTime = expireTime,
                Source = "web"
            });
            SaveLoginLog(user.Account, "0", "登录成功");
            CacheFactory.Cache.SetCache($"{CacheKey.AccessToken}{token}", GetUserInfo(token), expireTime);
            return new LoginData(token);
        }

        /// <summary>
        /// 管理员重置用户两步验证（清空绑定与恢复码，该用户下次登录需重新绑定）
        /// </summary>
        public ReturnData ResetTwoFactor(long userId)
        {
            if (userId <= 0)
            {
                return new ReturnData(500, "参数有误，请检查参数！");
            }
            var user = _sysUserService.GetById(userId);
            if (user == null)
            {
                return new ReturnData(500, "找不到用户！");
            }
            _twoFactorService.ClearBind(userId);
            CacheFactory.Cache.RemoveCache($"{CacheKey.TwoFactorBindSecret}{userId}");
            CacheFactory.Cache.RemoveCache($"{CacheKey.TwoFactorEmailCode}{userId}");
            return new ReturnData(200, $"已重置 {user.UserName} 的两步验证，请该用户重新登录并完成绑定");
        }

        /// <summary>
        /// 是否需要进入绑定流程（未绑定且强制开启，或密钥缺失）
        /// </summary>
        private static bool BindRequired(SysUserEntity user, TwoFactorConfig config, out string msg)
        {
            msg = null;
            // 两步验证开启后所有用户都必须绑定，不再区分是否强制
            if (!user.TwoFactorBindTime.HasValue)
            {
                msg = "首次登录需绑定两步验证";
                return true;
            }
            // 切换为验证器方式后，原未绑定密钥的用户需重新绑定
            if (config.UseTotp && string.IsNullOrWhiteSpace(user.TwoFactorSecret))
            {
                msg = "两步验证密钥缺失，请重新绑定";
                return true;
            }
            return false;
        }

        /// <summary>
        /// 邮箱脱敏（w**@example.com）
        /// </summary>
        private static string MaskEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email) || !email.Contains('@'))
            {
                return email;
            }
            var parts = email.Split('@');
            var name = parts[0];
            var masked = name.Length <= 2
                ? (name.Length == 1 ? "*" : name[0] + "*")
                : name[0] + new string('*', Math.Min(name.Length - 2, 4)) + name[^1];
            return $"{masked}@{parts[1]}";
        }

        //public ResultData<object> Login(string userName, string password)
        //{

        //    password = AESUtils.Encrypt(MD5Utils.Encrypt(password));
        //    var user = _sysUserService.Login(userName, password);
        //    if (user == null)
        //    {
        //        SaveLoginLog(userName, "1", "账号密码错误或找不到用户");
        //        return new ResultData<object>(500, "账号密码错误或找不到用户");
        //    }
        //    if (user.Status == "1")
        //    {
        //        SaveLoginLog(userName, "1", "账号已停用");
        //        return new ResultData<object>(500, "账号已停用");
        //    }
        //    var token = Guid.NewGuid().ToString("N");
        //    var expireTime = DateTime.Now.AddDays(7);
        //    var tokenSaveResult = _sysUserService.SaveToken(new SysTokenEntity()
        //    {
        //        UserId = user.UserId,
        //        Token = token,
        //        CreateTime = DateTime.Now,
        //        Status = 0,
        //        LoginIp = NetHepler.Ip,
        //        Os = NetHepler.Os,
        //        Browser = NetHepler.Browser,
        //        ExpirationTime = expireTime,
        //        Source = "web"
        //    });
        //    SaveLoginLog(userName, "0", "登录成功");
        //    CacheFactory.Cache.SetCache($"{CacheKey.AccessToken}{token}", GetUserInfo(token), expireTime);
        //    return new ResultData<object>(new
        //    {
        //        user = user,
        //        token = token
        //    });
        //}

        public ReturnData Logout(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                return new ReturnData();
            }
            _sysUserService.InvalidToken(token);
            SaveLoginLog(CacheFactory.Cache.GetCache<UserInfo>($"{CacheKey.AccessToken}{token}").UserName, "0", "退出成功");
            CacheFactory.Cache.RemoveCache($"{CacheKey.AccessToken}{token}");
            return new ReturnData();
        }

        #endregion

        public bool TryGetUser(string token, out UserInfo userInfo)
        {
            userInfo = CacheFactory.Cache.GetCache<UserInfo>($"{CacheKey.AccessToken}{token}");
            if (userInfo != null)
            {
                return true;
            }
            userInfo = GetUserInfo(token);
            if (userInfo != null)
            {
                CacheFactory.Cache.SetCache($"{CacheKey.AccessToken}{token}", userInfo, userInfo.ExpirationTime);
                return true;
            }
            return false;
        }

        #region 用户操作

        public RowData<SysUserEntity> GetList(ParamData<UserParam> param)
        {
            int total = 0;
            RowData<SysUserEntity> result = new RowData<SysUserEntity>(_sysUserService.GetList(param, out total));
            result.total = total;
            return result;
        }

        public UserReturnInfo GetById(long id)
        {
            UserReturnInfo reslut = new UserReturnInfo() { };
            if (id > 0)
            {
                reslut.Data = _sysUserService.GetById(id).ToEntityCopy<SysUserEntity, UserModel>();
                //if (reslut.Data == null)
                //{
                //    return new UserReturnInfo(500, "找不到用户！");
                //}
                reslut.Data.RoleIds = _sysUserService.GetUserRole(id)?.Select(p => p.RoleId).ToList() ?? new List<long>();
                reslut.Data.PostIds = _sysUserService.GetUserPost(id)?.Select(p => p.PostId).ToList() ?? new List<long>();
            }

            reslut.Roles = _sysRoleService.GetAll();
            reslut.Posts = _sysPostService.GetAll();
            return reslut;
        }

        public ReturnData Save(UserModel user)
        {
            var exist = _sysUserService.GetByUserName(user.Account);
            if (exist != null && exist.UserId != user.UserId)
            {
                return new ReturnData(500, $"已存在 {user.Account}，用户账户不能重复！");
            }
            exist = _sysUserService.GetById(user.UserId);
            if (exist != null)
            {
                user.Password = exist.Password;
            }
            else
            {
                if (!RsaUtils.TryDecrypt(user.Password, out var newPassword))
                {
                    return new ReturnData(500, "密码信息已失效，请刷新页面后重试");
                }
                user.Password = AESUtils.Encrypt(MD5Utils.Encrypt(newPassword));
            }
            _sysUserService.Save(user.ToEntityCopy<UserModel, SysUserEntity>());
            // 处理角色保存
            var roles = _sysUserService.GetUserRole(user.UserId)?.Select(p => p.RoleId).ToList() ?? new List<long>();
            var newRoles = user.RoleIds.Except(roles);
            if (newRoles.Any())
            {
                _sysUserService.SaveUserRole(newRoles.Select(p =>
                {
                    return new SysUserRoleEntity()
                    {
                        UserId = user.UserId,
                        RoleId = p
                    };
                }).ToList());
            }
            _sysUserService.DeleteUserRole(user.UserId, user.RoleIds);
            // 处理岗位保存
            var posts = _sysUserService.GetUserPost(user.UserId)?.Select(p => p.PostId).ToList() ?? new List<long>();
            var newPosts = user.PostIds.Except(posts);
            if (newPosts.Any())
            {
                _sysUserService.SaveUserPost(newPosts.Select(p =>
                {
                    return new SysUserPostEntity()
                    {
                        UserId = user.UserId,
                        PostId = p
                    };
                }).ToList());
            }
            _sysUserService.DeleteUserPost(user.UserId, user.PostIds);
            return new ReturnData();
        }

        public ReturnData Delete(string ids)
        {
            var userIds = ids.ToLongList();
            if (userIds == null || userIds.Count == 0)
            {
                return new ReturnData(500, "参数有误，请检查参数！");
            }
            if (_sysUserService.Delete(userIds))
            {
                return new ReturnData();
            }
            return new ReturnData(500, "删除失败！");
        }

        public byte[] Export(ParamData<UserParam> param)
        {
            int total = 0;
            param.PageSize = 0;
            return NPOIHepler.ExportExcel(_sysUserService.GetList(param, out total), _sysDicDataService);
        }

        public ReturnData ChangeStatus(long userId, string status)
        {
            var user = _sysUserService.GetById(userId);
            user.Status = status;
            if (_sysUserService.Save(user))
            {
                return new ReturnData();
            }
            return new ReturnData(500, "状态保存失败！");
        }

        #endregion

        #region 角色用户

        public RowData<SysUserEntity> GetRoleUserList(ParamData<RoleUserParam> param)
        {
            int total = 0;
            if (param.Params == null) param.Params = new RoleUserParam();
            param.Params.IsNoExist = false;
            RowData<SysUserEntity> result = new RowData<SysUserEntity>(_sysUserService.GetListByRoleId(param, out total));
            result.total = total;
            return result;
        }

        public RowData<SysUserEntity> GetRoleNoExistUserList(ParamData<RoleUserParam> param)
        {
            int total = 0;
            if (param.Params == null) param.Params = new RoleUserParam();
            param.Params.IsNoExist = true;
            RowData<SysUserEntity> result = new RowData<SysUserEntity>(_sysUserService.GetListByRoleId(param, out total));
            result.total = total;
            return result;
        }

        public ResultData<object> GetUserRole(long id)
        {
            var user = _sysUserService.GetById(id).ToEntityCopy<SysUserEntity, UserModel>();
            user.RoleIds = _sysUserService.GetUserRole(id)?.Select(p => p.RoleId).ToList() ?? new List<long>();
            return new ResultData<object>(new
            {
                Roles = _sysRoleService.GetAll(),
                User = user
            });
        }

        public ReturnData SaveUserRole(long userId, string roleIds)
        {
            if (userId == 0) return new ReturnData(500, "用户id有误！");
            List<long> roleSaveIds = new List<long>();
            if (!string.IsNullOrWhiteSpace(roleIds))
            {
                roleSaveIds = roleIds.ToLongList();
            }
            var roles = _sysUserService.GetUserRole(userId)?.Select(p => p.RoleId).ToList() ?? new List<long>();
            var newRoles = roleSaveIds.Except(roles);
            if (newRoles.Any())
            {
                _sysUserService.SaveUserRole(newRoles.Select(p =>
                {
                    return new SysUserRoleEntity()
                    {
                        UserId = userId,
                        RoleId = p
                    };
                }).ToList());
            }
            _sysUserService.DeleteUserRole(userId, roleSaveIds);
            return new ReturnData();
        }

        #endregion

        #region 密码

        public ReturnData ResetUserPwd(UserPwdParam userPwdParam)
        {
            //TODO 重置密码需要验证
            var user = _sysUserService.GetById(userPwdParam.userId);
            if (user == null)
            {
                return new ReturnData(500, "找不到用户！");
            }
            if (!RsaUtils.TryDecrypt(userPwdParam.Password, out var resetPassword))
            {
                return new ReturnData(500, "密码信息已失效，请刷新页面后重试");
            }
            user.Password = AESUtils.Encrypt(MD5Utils.Encrypt(resetPassword));
            _sysUserService.Save(user);
            return new ReturnData();
        }

        public ReturnData UpdateCurrentUserPwd(string oldPassword, string newPassword)
        {
            var user = _sysUserService.GetById(GlobalContext.CurrentUser.UserId);
            if (user == null)
            {
                return new ReturnData(500, "找不到用户！");
            }
            if (!RsaUtils.TryDecrypt(oldPassword, out var oldPwdPlain)
                || !RsaUtils.TryDecrypt(newPassword, out var newPwdPlain))
            {
                return new ReturnData(500, "密码信息已失效，请刷新页面后重试");
            }
            var oldPwd = AESUtils.Encrypt(MD5Utils.Encrypt(oldPwdPlain));
            if (user.Password != oldPwd)
            {
                return new ReturnData(500, "旧密码输入错误！");
            }
            user.Password = AESUtils.Encrypt(MD5Utils.Encrypt(newPwdPlain));
            _sysUserService.Save(user);
            return new ReturnData();
        }

        #endregion

        #region 个人中心

        public ReturnData SaveCurrentUser(UserInfo userInfo)
        {
            var user = _sysUserService.GetById(GlobalContext.CurrentUser.UserId);
            if (user == null)
            {
                return new ReturnData(500, "找不到当前用户！");
            }
            user.Sex = userInfo.Sex;
            user.UserName = userInfo.UserName;
            user.Phonenumber = userInfo.Phonenumber;
            _sysUserService.Save(user);
            var currentUser = GetUserInfo(GlobalContext.Token.Value);
            CacheFactory.Cache.SetCache($"{CacheKey.AccessToken}{GlobalContext.Token.Value}", currentUser, currentUser.ExpirationTime);
            return new ReturnData();
        }

        #endregion

        #region 私有方法

        private UserInfo GetUserInfo(string token)
        {
            var user = _sysUserService.GetByToken(token);
            if (user == null)
            {
                return null;
            }
            UserInfo userInfo = user.ToEntityCopy<SysUserEntity, UserInfo>();
            userInfo.Roles = _sysRoleService.GetByUserId(userInfo.UserId).Select(p => p.ToEntityCopy<SysRoleEntity, RoleInfo>()).ToList();
            userInfo.Dept = _sysDeptService.GetById(userInfo.DeptId)?.ToEntityCopy<SysDeptEntity, DeptInfo>() ?? null;
            userInfo.Posts = _sysPostService.GetByUserId(userInfo.UserId).Select(p => p.ToEntityCopy<SysPostEntity, PostInfo>()).ToList();
            userInfo.Permissions = _sysMenuService.GetPermissionsByUserId(userInfo.UserId);
            return userInfo;
        }

        private bool SaveLoginLog(string userName, string status, string msg)
        {
            return _sysLoginLogService.Save(new SysLoginLogEntity()
            {
                UserName = userName,
                Ipaddr = NetHepler.Ip,
                LoginLocation = NetHepler.IpLocation,
                Browser = NetHepler.Browser,
                Os = NetHepler.Os,
                Status = status,
                Msg = msg,
                LoginTime = DateTime.Now
            }); ;
        }

        public bool IsCaptchaEnabled()
        {
            var configValue = _sysConfigBiz.GetByConfigKey("sys.login.isCaptchaOn");
            if (string.IsNullOrWhiteSpace(configValue)) return false;
            var obj = JObject.Parse(configValue);
            return obj.Value<bool>("enabled");
        }

        #endregion
    }
}
