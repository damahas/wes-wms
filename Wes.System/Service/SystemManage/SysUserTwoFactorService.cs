using SqlSugar;
using System;
using Wes.Entity;

namespace Wes.Service
{
    /// <summary>
    /// 两步验证数据访问（绑定信息存于 sys_user）
    /// </summary>
    public class SysUserTwoFactorService : Repository<SysUserEntity>, ISysUserTwoFactorService
    {
        public SysUserTwoFactorService(ISqlSugarClient db) : base(db) { }

        public bool SaveBind(long userId, string secret)
        {
            return Context.Updateable<SysUserEntity>()
                .SetColumns(p => new SysUserEntity
                {
                    TwoFactorSecret = secret,
                    TwoFactorLastStep = 0,
                    TwoFactorBindTime = DateTime.Now
                })
                .Where(p => p.UserId == userId)
                .ExecuteCommand() > 0;
        }

        public bool UpdateVerifyStep(long userId, long step)
        {
            return Context.Updateable<SysUserEntity>()
                .SetColumns(p => p.TwoFactorLastStep == step)
                .Where(p => p.UserId == userId)
                .ExecuteCommand() > 0;
        }

        public bool ClearBind(long userId)
        {
            return Context.Updateable<SysUserEntity>()
                .SetColumns(p => new SysUserEntity
                {
                    TwoFactorSecret = null,
                    TwoFactorLastStep = 0,
                    TwoFactorBindTime = null
                })
                .Where(p => p.UserId == userId)
                .ExecuteCommand() > 0;
        }
    }
}
