using System;

namespace Wes.Utils.Model
{
    /// <summary>
    /// 两步验证配置（sys.login.twoFactor）
    /// </summary>
    public class TwoFactorConfig
    {
        /// <summary>是否启用两步验证（开启后所有用户必须完成绑定才能登录）</summary>
        public bool Enabled { get; set; }

        /// <summary>
        /// 验证方式（全局开关，由管理员在参数配置中选择）：
        /// email=邮箱验证码；totp=验证器 App 动态码
        /// </summary>
        public string Mode { get; set; } = "email";

        /// <summary>验证器 App 中显示的发行方名称</summary>
        public string Issuer { get; set; } = "WesAdmin";

        /// <summary>
        /// 归一化后的验证方式（配置值非法时按 email 处理）
        /// </summary>
        public string VerifyMode => Mode == TwoFactorConst.ModeTotp ? TwoFactorConst.ModeTotp : TwoFactorConst.ModeEmail;

        /// <summary>
        /// 是否使用验证器动态码方式
        /// </summary>
        public bool UseTotp => VerifyMode == TwoFactorConst.ModeTotp;
    }

    /// <summary>
    /// 两步验证配置键名
    /// </summary>
    public class TwoFactorKey
    {
        /// <summary>参数配置表中的键名</summary>
        public const string ConfigKey = "sys.login.twoFactor";
    }

    /// <summary>
    /// 两步验证绑定记录
    /// </summary>
    public class TwoFactorBindInfo
    {
        /// <summary>是否已绑定并启用</summary>
        public bool Bound { get; set; }

        /// <summary>绑定方式：email / totp</summary>
        public string Mode { get; set; }

        /// <summary>绑定时间</summary>
        public DateTime? BindTime { get; set; }
    }

    /// <summary>
    /// 两步验证绑定/切换时的返回信息（含密钥与二维码内容）
    /// </summary>
    public class TwoFactorSecretInfo
    {
        /// <summary>Base32 密钥</summary>
        public string Secret { get; set; }

        /// <summary>验证器 App 扫码地址（otpauth://totp/...）</summary>
        public string OtpauthUri { get; set; }

        /// <summary>是否允许用户自选方式</summary>
        public bool Enabled { get; set; } = true;

        /// <summary>失败时的提示信息</summary>
        public string Msg { get; set; }
    }

    /// <summary>
    /// 两步验证相关常量
    /// </summary>
    public static class TwoFactorConst
    {
        /// <summary>中间票据有效期（分钟）</summary>
        public const int ChallengeMinutes = 6;

        /// <summary>绑定过程中暂存密钥的有效期（分钟）</summary>
        public const int BindSecretMinutes = 10;

        /// <summary>邮箱验证码有效期（分钟）</summary>
        public const int EmailCodeExpireMinutes = 5;

        /// <summary>TOTP 允许的时间片偏移（前后各 1 个，即 ±30 秒）</summary>
        public const int TotpWindow = 1;

        /// <summary>绑定方式：邮箱验证码</summary>
        public const string ModeEmail = "email";

        /// <summary>绑定方式：验证器动态码</summary>
        public const string ModeTotp = "totp";
    }
}
