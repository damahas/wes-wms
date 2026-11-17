using System;
using System.Security.Cryptography;
using Wes.Entity;
using Wes.Service;
using Wes.Utils;
using Wes.Utils.Cache;
using Wes.Utils.Hepler;
using Wes.Utils.Model;
using Wes.Utils.Security;
using Wes.ViewModel.SystemManage;

namespace Wes.Business
{
    /// <summary>
    /// 两步验证业务处理
    /// 绑定信息（方式、密钥、绑定时间、已校验时间片）存储在 sys_user，
    /// 恢复码存储在 sys_user_recovery_code
    /// </summary>
    public class SysTwoFactorBiz : ISysTwoFactorBiz
    {
        private readonly ISysUserService _userService;
        private readonly ISysUserTwoFactorService _twoFactorService;
        private readonly ISysConfigBiz _configBiz;
        private readonly ISysLoginLogService _loginLogService;
        private readonly ISysUserBiz _userBiz;

        public SysTwoFactorBiz(
            ISysUserService userService,
            ISysUserTwoFactorService twoFactorService,
            ISysConfigBiz configBiz,
            ISysLoginLogService loginLogService,
            ISysUserBiz userBiz)
        {
            _userService = userService;
            _twoFactorService = twoFactorService;
            _configBiz = configBiz;
            _loginLogService = loginLogService;
            _userBiz = userBiz;
        }

        #region 登录第二阶段

        public ReturnData Verify(TwoFactorParam param)
        {
            if (param == null || string.IsNullOrWhiteSpace(param.ChallengeToken) || string.IsNullOrWhiteSpace(param.Code))
            {
                return new ReturnData(500, "参数有误，请重新登录");
            }

            var user = ResolveChallengeUser(param.ChallengeToken, out var error);
            if (user == null)
            {
                return new ReturnData(333, error);
            }

            var config = _configBiz.GetTwoFactorConfig();
            if (!config.Enabled)
            {
                return new ReturnData(500, "两步验证未开启，请重新登录");
            }
            if (!IsBound(user))
            {
                return new ReturnData(500, "尚未绑定两步验证");
            }

            var verify = VerifyCode(user, config, param.Code);
            if (verify.NeedRebind)
            {
                // 密钥缺失或无法解析，引导重新绑定
                return BindStepData(user, config, "两步验证密钥异常，请重新绑定");
            }
            if (!verify.Success)
            {
                // 校验失败即作废旧票据，必须重新走完整登录流程
                SaveLog(user.Account, "1", "两步验证失败");
                ConsumeTicket(param.ChallengeToken, user.UserId);
                return new ReturnData(333, "验证失败，请重新登录");
            }

            ConsumeTicket(param.ChallengeToken, user.UserId);
            SaveLog(user.Account, "0", "两步验证通过，登录成功");
            return _userBiz.IssueToken(user);
        }

        public ReturnData SendEmailCode(TwoFactorParam param)
        {
            var config = _configBiz.GetTwoFactorConfig();
            if (!config.Enabled)
            {
                return new ReturnData(500, "两步验证未开启");
            }

            SysUserEntity user;
            if (param != null && !string.IsNullOrWhiteSpace(param.ChallengeToken))
            {
                user = ResolveChallengeUser(param.ChallengeToken, out var error);
                if (user == null)
                {
                    return new ReturnData(333, error);
                }

            }
            else
            {
                user = _userService.GetById(GlobalContext.CurrentUser.UserId);
                if (user == null)
                {
                    return new ReturnData(500, "找不到当前用户");
                }
            }

            return SendEmailCodeInternal(user, config);
        }

        public ReturnData PrepareLoginBind(TwoFactorParam param)
        {
            var config = _configBiz.GetTwoFactorConfig();
            if (!config.Enabled)
            {
                return new ReturnData(500, "两步验证未开启");
            }
            if (param == null || string.IsNullOrWhiteSpace(param.ChallengeToken))
            {
                return new ReturnData(500, "参数有误，请重新登录");
            }

            var user = ResolveChallengeUser(param.ChallengeToken, out var error);
            if (user == null)
            {
                return new ReturnData(333, error);
            }
            if (IsBound(user))
            {
                return new ReturnData(500, "已完成绑定，请直接输入验证码");
            }

            if (config.UseTotp)
            {
                var secret = CreatePendingSecret(user.UserId);
                return new TwoFactorLoginData(332, "请使用验证器 App 扫码并完成绑定")
                {
                    ChallengeToken = param.ChallengeToken,
                    Action = "bind",
                    Mode = TwoFactorConst.ModeTotp,
                    Secret = secret,
                    OtpauthUri = TotpUtils.GetOtpauthUri(config.Issuer, user.Account, secret),
                };
            }

            var sendResult = SendEmailCodeInternal(user, config);
            if (sendResult.Code != 200)
            {
                return sendResult;
            }
            return new TwoFactorLoginData(332, "验证码已发送到您的邮箱")
            {
                ChallengeToken = param.ChallengeToken,
                Action = "bind",
                Mode = TwoFactorConst.ModeEmail,
                Email = user.Email
            };
        }

        public ReturnData BindAndLogin(TwoFactorParam param)
        {
            var config = _configBiz.GetTwoFactorConfig();
            if (!config.Enabled)
            {
                return new ReturnData(500, "两步验证未开启");
            }
            if (param == null || string.IsNullOrWhiteSpace(param.ChallengeToken) || string.IsNullOrWhiteSpace(param.Code))
            {
                return new ReturnData(500, "参数有误，请重新登录");
            }

            var user = ResolveChallengeUser(param.ChallengeToken, out var error);
            if (user == null)
            {
                return new ReturnData(333, error);
            }

            if (IsBound(user))
            {
                return new ReturnData(500, "已完成绑定，请重新登录并输入验证码");
            }

            var result = SaveBind(user, param.Code, config);
            if (result.Code != 200)
            {
                // 绑定校验失败同样作废旧票据，必须重新走完整登录流程
                SaveLog(user.Account, "1", "两步验证绑定失败");
                ConsumeTicket(param.ChallengeToken, user.UserId);
                return new ReturnData(333, "验证失败，请重新登录");
            }

            ConsumeTicket(param.ChallengeToken, user.UserId);
            SaveLog(user.Account, "0", "两步验证绑定成功，登录成功");

            var login = _userBiz.IssueToken(user);
            return new TwoFactorLoginData(login.Code, login.Msg)
            {
                Token = (login as LoginData)?.Token,
                Action = "bound",
                Mode = config.VerifyMode
            };
        }

        #endregion

        #region 已登录用户自助管理

        public TwoFactorBindInfo GetBindInfo()
        {
            var userId = GlobalContext.CurrentUser?.UserId ?? 0;
            var user = _userService.GetById(userId);
            var config = _configBiz.GetTwoFactorConfig();
            return new TwoFactorBindInfo
            {
                Bound = IsBound(user),
                Mode = config.VerifyMode,
                BindTime = user?.TwoFactorBindTime
            };
        }

        public TwoFactorSecretInfo PrepareBind()
        {
            var config = _configBiz.GetTwoFactorConfig();
            if (!config.Enabled)
            {
                return new TwoFactorSecretInfo { Enabled = false, Msg = "两步验证未开启" };
            }

            var user = _userService.GetById(GlobalContext.CurrentUser.UserId);
            if (user == null)
            {
                return new TwoFactorSecretInfo { Enabled = false, Msg = "找不到当前用户" };
            }
            if (IsBound(user))
            {
                return new TwoFactorSecretInfo { Enabled = false, Msg = "已绑定两步验证，请先解绑再重新绑定" };
            }

            if (config.UseTotp)
            {
                var secret = CreatePendingSecret(user.UserId);
                return new TwoFactorSecretInfo
                {
                    Secret = secret,
                    OtpauthUri = TotpUtils.GetOtpauthUri(config.Issuer, user.Account, secret)
                };
            }

            var sendResult = SendEmailCodeInternal(user, config);
            return sendResult.Code != 200
                ? new TwoFactorSecretInfo { Enabled = false, Msg = sendResult.Msg }
                : new TwoFactorSecretInfo { Secret = null, OtpauthUri = null };
        }

        public TwoFactorLoginData ConfirmBind(TwoFactorParam param)
        {
            var userId = GlobalContext.CurrentUser?.UserId ?? 0;
            var user = _userService.GetById(userId);
            if (user == null)
            {
                return new TwoFactorLoginData(500, "找不到当前用户");
            }

            var config = _configBiz.GetTwoFactorConfig();
            if (!config.Enabled)
            {
                return new TwoFactorLoginData(500, "两步验证未开启");
            }
            if (IsBound(user))
            {
                return new TwoFactorLoginData(500, "已绑定两步验证，请先解绑再重新绑定");
            }

            var result = SaveBind(user, param?.Code, config);
            if (result.Code != 200)
            {
                // 已登录场景下的自助绑定，失败直接提示即可
                return new TwoFactorLoginData(result.Code, result.Msg);
            }

            return new TwoFactorLoginData(200, "绑定成功")
            {
                Mode = config.VerifyMode,
                Action = "bound"
            };
        }

        public ReturnData Unbind(TwoFactorUnbindParam param)
        {
            var userId = GlobalContext.CurrentUser?.UserId ?? 0;
            var user = _userService.GetById(userId);
            if (user == null)
            {
                return new ReturnData(500, "找不到当前用户");
            }
            if (string.IsNullOrWhiteSpace(param?.Password) || string.IsNullOrWhiteSpace(param.Code))
            {
                return new ReturnData(500, "请输入登录密码与验证码");
            }
            if (!RsaUtils.TryDecrypt(param.Password, out var password))
            {
                return new ReturnData(500, "密码信息已失效，请刷新页面后重试");
            }
            if (user.Password != AESUtils.Encrypt(MD5Utils.Encrypt(password)))
            {
                return new ReturnData(500, "登录密码错误");
            }
            if (!IsBound(user))
            {
                return new ReturnData(500, "尚未绑定两步验证");
            }

            var config = _configBiz.GetTwoFactorConfig();
            var verify = VerifyCode(user, config, param.Code);
            if (!verify.Success)
            {
                return new ReturnData(500, "验证码错误或已失效");
            }

            _twoFactorService.ClearBind(userId);
            ClearPendingSecret(userId);
            return new ReturnData(200, "已解绑两步验证");
        }

        public ReturnData Reset(long userId)
        {
            return _userBiz.ResetTwoFactor(userId);
        }

        #endregion

        #region 私有方法

        /// <summary>
        /// 是否已绑定两步验证（sys_user.two_factor_bind_time 非空即视为已绑定）
        /// </summary>
        private static bool IsBound(SysUserEntity user)
        {
            return user != null && user.TwoFactorBindTime.HasValue;
        }

        /// <summary>
        /// 写入绑定：校验首个验证码后落库（方式由配置决定）
        /// </summary>
        private ReturnData SaveBind(SysUserEntity user, string code, TwoFactorConfig config)
        {
            if (string.IsNullOrWhiteSpace(code))
            {
                return new ReturnData(500, "请输入验证码");
            }

            if (config.UseTotp)
            {
                var secret = GetPendingSecret(user.UserId);
                if (string.IsNullOrWhiteSpace(secret))
                {
                    return new ReturnData(500, "绑定已超时，请重新发起绑定");
                }
                if (!TotpUtils.Validate(secret, code.Trim(), TwoFactorConst.TotpWindow, 0, out _))
                {
                    return new ReturnData(500, "动态码错误");
                }
                _twoFactorService.SaveBind(user.UserId, AESUtils.Encrypt(secret));
                ClearPendingSecret(user.UserId);
                return new ReturnData(200, "绑定成功");
            }

            if (!VerifyEmailCode(user.UserId, code))
            {
                return new ReturnData(500, "验证码错误或已失效");
            }
            _twoFactorService.SaveBind(user.UserId, null);
            return new ReturnData(200, "绑定成功");
        }

        /// <summary>
        /// 返回引导绑定的中间态数据
        /// </summary>
        private ReturnData BindStepData(SysUserEntity user, TwoFactorConfig config, string msg)
        {
            _twoFactorService.ClearBind(user.UserId);
            return new TwoFactorLoginData(332, msg)
            {
                ChallengeToken = _userBiz.IssueChallengeToken(user.UserId),
                Action = "bind",
                Mode = config.VerifyMode,
                Email = MaskEmail(user.Email)
            };
        }

        /// <summary>
        /// 从中间票据解析用户（必须为两步验证阶段票据且未被消费）
        /// </summary>
        private SysUserEntity ResolveChallengeUser(string challengeToken, out string error)
        {
            error = null;
            if (!JWTUtils.TryValidate(challengeToken, out var principal))
            {
                error = "验证信息已失效，请重新登录";
                return null;
            }
            if (JWTUtils.GetStage(principal) != JWTUtils.StageTwoFactor)
            {
                error = "无效的两步验证票据";
                return null;
            }
            if (!long.TryParse(JWTUtils.GetUserId(principal), out var userId) || userId <= 0)
            {
                error = "无效的两步验证票据";
                return null;
            }
            if (CacheFactory.Cache.GetCache<bool>($"{CacheKey.TwoFactorTicket}{userId}_{MD5Utils.Encrypt(challengeToken)}"))
            {
                error = "验证信息已失效，请重新登录";
                return null;
            }

            var user = _userService.GetById(userId);
            if (user == null || user.IsDel != 0 || user.Status == "1")
            {
                error = "账号不可用，请联系管理员";
                return null;
            }
            return user;
        }

        /// <summary>
        /// 标记中间票据已消费
        /// </summary>
        private void ConsumeTicket(string challengeToken, long userId)
        {
            CacheFactory.Cache.SetCache(
                $"{CacheKey.TwoFactorTicket}{userId}_{MD5Utils.Encrypt(challengeToken)}",
                true,
                DateTime.Now.AddMinutes(TwoFactorConst.ChallengeMinutes));
        }

        /// <summary>
        /// 校验验证码（动态码 / 邮箱验证码 / 恢复码）
        /// NeedRebind 表示密钥缺失或损坏，需要重新绑定
        /// </summary>
        private (bool Success, bool NeedRebind) VerifyCode(SysUserEntity user, TwoFactorConfig config, string code)
        {
            if (string.IsNullOrWhiteSpace(code))
            {
                return (false, false);
            }

            var input = code.Trim();

            if (config.UseTotp)
            {
                var secret = DecryptSecret(user.TwoFactorSecret);
                if (string.IsNullOrWhiteSpace(secret))
                {
                    return (false, true);
                }
                if (TotpUtils.Validate(secret, input, TwoFactorConst.TotpWindow, user.TwoFactorLastStep ?? 0, out var step))
                {
                    _twoFactorService.UpdateVerifyStep(user.UserId, step);
                    return (true, false);
                }
                return (false, false);
            }

            return (VerifyEmailCode(user.UserId, input), false);
        }

        private static string DecryptSecret(string encrypted)
        {
            if (string.IsNullOrWhiteSpace(encrypted))
            {
                return null;
            }
            try
            {
                return AESUtils.Decrypt(encrypted);
            }
            catch
            {
                return null;
            }
        }

        private bool VerifyEmailCode(long userId, string code)
        {
            if (string.IsNullOrWhiteSpace(code))
            {
                return false;
            }
            var cached = CacheFactory.Cache.GetCache<string>($"{CacheKey.TwoFactorEmailCode}{userId}");
            if (string.IsNullOrWhiteSpace(cached))
            {
                return false;
            }
            if (cached != MD5Utils.Encrypt(code.Trim()))
            {
                return false;
            }
            CacheFactory.Cache.RemoveCache($"{CacheKey.TwoFactorEmailCode}{userId}");
            return true;
        }

        /// <summary>
        /// 发送邮箱验证码（邮箱取自用户资料）
        /// </summary>
        private ReturnData SendEmailCodeInternal(SysUserEntity user, TwoFactorConfig config)
        {
            if (string.IsNullOrWhiteSpace(user.Email))
            {
                return new ReturnData(500, "该用户未维护邮箱，无法使用邮箱验证码，请联系管理员");
            }

            var mailConfig = _configBiz.GetMailModel();
            if (mailConfig == null || string.IsNullOrWhiteSpace(mailConfig.MailHost))
            {
                return new ReturnData(500, "未配置邮箱服务（sys.integration.mail），请联系管理员");
            }

            var code = RandomNumberGenerator.GetInt32(100000, 1000000).ToString();
            CacheFactory.Cache.SetCache(
                $"{CacheKey.TwoFactorEmailCode}{user.UserId}",
                MD5Utils.Encrypt(code),
                DateTime.Now.AddMinutes(TwoFactorConst.EmailCodeExpireMinutes));

            var body =
                $"<p>您好，<b>{user.UserName}</b>：</p>" +
                $"<p>您正在登录 Wes 管理系统，本次登录验证码为：</p>" +
                $"<p style=\"font-size:26px;font-weight:bold;letter-spacing:6px\">{code}</p>" +
                $"<p>验证码 {TwoFactorConst.EmailCodeExpireMinutes} 分钟内有效，请勿转发他人。</p>" +
                $"<p>登录信息：IP {NetHepler.Ip}（{NetHepler.IpLocation}）· {NetHepler.Os} · {NetHepler.Browser}</p>" +
                $"<p style=\"color:#999\">若非本人操作，请忽略此邮件并及时修改密码。</p>";

            var ok = MailHelper.Send(mailConfig, user.Email, "登录验证码", body, null, out var errorMsg);
            return ok
                ? new ReturnData(200, "验证码已发送")
                : new ReturnData(500, $"验证码发送失败：{errorMsg}");
        }

        #region TOTP 密钥暂存

        private string CreatePendingSecret(long userId)
        {
            var secret = TotpUtils.GenerateSecret();
            CacheFactory.Cache.SetCache(
                $"{CacheKey.TwoFactorBindSecret}{userId}",
                secret,
                DateTime.Now.AddMinutes(TwoFactorConst.BindSecretMinutes));
            return secret;
        }

        private string GetPendingSecret(long userId)
        {
            return CacheFactory.Cache.GetCache<string>($"{CacheKey.TwoFactorBindSecret}{userId}");
        }

        private void ClearPendingSecret(long userId)
        {
            CacheFactory.Cache.RemoveCache($"{CacheKey.TwoFactorBindSecret}{userId}");
        }

        #endregion

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

        private bool SaveLog(string userName, string status, string msg)
        {
            return _loginLogService.Save(new SysLoginLogEntity
            {
                UserName = userName,
                Ipaddr = NetHepler.Ip,
                LoginLocation = NetHepler.IpLocation,
                Browser = NetHepler.Browser,
                Os = NetHepler.Os,
                Status = status,
                Msg = msg,
                LoginTime = DateTime.Now
            });
        }

        #endregion
    }
}
