namespace Wes.Service
{
    public interface ISysUserTwoFactorService
    {
        /// <summary>
        /// 保存绑定（secret 为空表示邮箱验证码方式）
        /// </summary>
        bool SaveBind(long userId, string secret);

        /// <summary>
        /// 更新已校验的时间片（防重放）
        /// </summary>
        bool UpdateVerifyStep(long userId, long step);

        /// <summary>
        /// 清空绑定
        /// </summary>
        bool ClearBind(long userId);
    }
}
