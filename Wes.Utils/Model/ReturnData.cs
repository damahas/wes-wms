using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Wes.Utils.Model
{
    /// <summary>
    /// 接口返回值基类
    /// </summary>
    public class ReturnData
    {
        public long Code { set; get; }
        public string Msg { set; get; }

        public ReturnData()
        {
            this.Code = 200;
            this.Msg = "操作成功";
        }
        public ReturnData(long code, string msg)
        {
            this.Code = code;
            this.Msg = msg;
        }

    }

    /// <summary>
    /// 多行返回值
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class RowData<T> : ReturnData
    {
        public int total { set; get; }

        public List<T> Rows { set; get; }

        public RowData(long code, string msg) : base(code, msg)
        { }

        public RowData(List<T> rows)
        {
            this.Code = 200;
            this.Rows = rows;
        }
    }

    /// <summary>
    /// 结果返回值
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ResultData<T> : ReturnData
    {

        public T Data { set; get; }

        public ResultData()
        { }

        public ResultData(long code, string msg) : base(code, msg)
        { }

        public ResultData(T Result)
        {
            this.Code = 200;
            this.Data = Result;
        }
    }

    /// <summary>
    /// 登录接口返回值
    /// </summary>
    public class LoginData : ReturnData
    {
        public string Token { set; get; }

        public UserInfo UserInfo { set; get; }

        public LoginData(long code, string msg) : base(code, msg)
        { }

        public LoginData(string token)
        {
            this.Code = 200;
            this.Token = token;
        }
    }

    /// <summary>
    /// 个人信息返回值
    /// </summary>
    public class UserInfoData : ReturnData
    {
        public UserInfo User { set; get; }

        public List<string> Roles
        {
            get
            {
                return this.User?.Roles?.Select(p => p.RoleKey).ToList() ?? new List<string>();
            }
        }

        public List<string> Permissions
        {
            get
            {
                if (this.Roles.Contains("admin"))
                {
                    return new List<string>() { "*:*:*" };
                }
                return this.User?.Permissions;
            }
        }

        public UserInfoData(long code, string msg) : base(code, msg)
        { }

        public UserInfoData(UserInfo userInfo)
        {
            this.Code = 200;
            this.User = userInfo;
        }
    }

    /// <summary>
    /// 两步验证第一阶段结果
    /// </summary>
    public class TwoFactorLoginData : ReturnData
    {
        /// <summary>两步验证中间票据（已通过第一因素，尚不能访问业务接口）</summary>
        public string ChallengeToken { get; set; }

        /// <summary>正式 token（未完成两步验证时为空）</summary>
        public string Token { get; set; }

        /// <summary>下一步动作：verify=输入验证码 bind=尚未绑定需先绑定</summary>
        public string Action { get; set; }

        /// <summary>验证方式：email / totp，both 时由用户选择</summary>
        public string Mode { get; set; }

        /// <summary>邮箱验证码发送目标（脱敏）</summary>
        public string Email { get; set; }

        /// <summary>绑定 TOTP 时的密钥（仅绑定流程返回一次）</summary>
        public string Secret { get; set; }

        /// <summary>绑定 TOTP 时的扫码地址</summary>
        public string OtpauthUri { get; set; }

        public TwoFactorLoginData(long code, string msg) : base(code, msg)
        { }
    }
}
