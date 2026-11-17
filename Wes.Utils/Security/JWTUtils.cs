using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Http;
using System.Net;

namespace Wes.Utils
{
    public static class JWTUtils
    {
        /// <summary>票据阶段声明类型（两步验证中间票据用）</summary>
        public const string StageClaimType = "stage";

        /// <summary>两步验证中间票据：已通过第一因素但未通过第二因素，不可访问业务接口</summary>
        public const string StageTwoFactor = "2fa";

        /// <summary>
        /// 生成 JWT
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="stage">票据阶段（两步验证中间票据传 <see cref="StageTwoFactor"/>），正式票据传 null</param>
        /// <param name="expiresMinutes">有效期（分钟），为空时使用全局配置</param>
        public static string GenerateToken(string userId, string stage = null, int? expiresMinutes = null)
        {
            var claims = new List<Claim>
            {
             //JwtRegisteredClaimNames.Sub:
             // 这是一个预定义的 JWT 声明类型，表示 "subject"（主体）。
             // 在此上下文中，它通常用于存储用户的唯一标识符（如用户名或用户ID）。
             new Claim("userId", userId),
            };
            if (!string.IsNullOrWhiteSpace(stage))
            {
                claims.Add(new Claim(StageClaimType, stage));
            }
            JwtSecurityToken token = new JwtSecurityToken(
                issuer: GlobalContext.JwtSettings.Issuer,
                audience: GlobalContext.JwtSettings.Audience,
                claims: claims,
                expires: expiresMinutes.HasValue ? DateTime.Now.AddMinutes(expiresMinutes.Value) : DateTime.Now.AddHours(GlobalContext.JwtSettings.Expires),
                signingCredentials: new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(GlobalContext.JwtSettings.SecretKey)), SecurityAlgorithms.HmacSha256)
            );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        /// <summary>
        /// 解析并校验 JWT（用于不经过授权中间件的场景，如两步验证第二阶段）
        /// </summary>
        /// <param name="token">原始 token（可含 Bearer 前缀）</param>
        /// <param name="principal">校验通过后的身份</param>
        /// <returns>是否校验通过</returns>
        public static bool TryValidate(string token, out ClaimsPrincipal principal)
        {
            principal = null;
            if (string.IsNullOrWhiteSpace(token))
            {
                return false;
            }

            var jwt = GlobalContext.JwtSettings;
            if (jwt == null)
            {
                return false;
            }

            try
            {
                var handler = new JwtSecurityTokenHandler();
                principal = handler.ValidateToken(GetToken(token), new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwt.SecretKey)),
                    ValidateIssuer = true,
                    ValidIssuer = jwt.Issuer,
                    ValidateAudience = true,
                    ValidAudience = jwt.Audience,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.FromMinutes(1)
                }, out _);
                return principal != null;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 从身份中获取用户ID
        /// </summary>
        public static string GetUserId(ClaimsPrincipal principal)
        {
            return principal?.Claims?.FirstOrDefault(e => e.Type == "userId")?.Value ?? "";
        }

        /// <summary>
        /// 从身份中获取票据阶段（正式票据为空）
        /// </summary>
        public static string GetStage(ClaimsPrincipal principal)
        {
            return principal?.Claims?.FirstOrDefault(e => e.Type == StageClaimType)?.Value ?? "";
        }

        public static string GetUserId(HttpContext httpContext)
        {
            string userId = "";
            if (httpContext.User == null || !httpContext.User.Claims.Any())
                return userId;
            var claim = httpContext.User.Claims.FirstOrDefault(e => e.Type == "userId");
            if (claim == null)
                return userId;
            userId = claim.Value;
            return userId;
        }

        public static string GetToken(string str) {
            if (string.IsNullOrWhiteSpace(str)) {
                return "";
            }
            return str.Replace("Bearer ", "");
        }
    }
}
