namespace Wes.ViewModel.SystemManage
{
    /// <summary>
    /// 修改当前用户密码（密码为前端 RSA 加密后的密文）
    /// </summary>
    public class UpdatePwdParam
    {
        /// <summary>原密码（RSA 加密）</summary>
        public string OldPassword { get; set; }

        /// <summary>新密码（RSA 加密）</summary>
        public string NewPassword { get; set; }
    }
}
