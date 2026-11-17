using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Wes.Utils.Security
{
    /// <summary>
    /// TOTP（RFC 6238）工具：Base32 密钥生成、动态码生成与校验
    /// 兼容 Google / Microsoft Authenticator、1Password 等标准验证器
    /// </summary>
    public static class TotpUtils
    {
        /// <summary>时间步长（秒）</summary>
        public const int StepSeconds = 30;

        /// <summary>动态码位数</summary>
        public const int CodeDigits = 6;

        private const string Base32Alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ234567";

        #region 密钥

        /// <summary>
        /// 生成随机密钥（Base32 字符串）
        /// </summary>
        /// <param name="byteLength">随机字节数，默认 20（160 bit，符合 RFC 推荐）</param>
        public static string GenerateSecret(int byteLength = 20)
        {
            var bytes = RandomNumberGenerator.GetBytes(byteLength);
            return ToBase32(bytes);
        }

        /// <summary>
        /// 生成用于验证器 App 的 otpauth 地址
        /// </summary>
        public static string GetOtpauthUri(string issuer, string account, string secret)
        {
            var label = Uri.EscapeDataString($"{issuer}:{account}");
            return $"otpauth://totp/{label}?secret={secret}&issuer={Uri.EscapeDataString(issuer)}" +
                   $"&digits={CodeDigits}&period={StepSeconds}&algorithm=SHA1";
        }

        #endregion

        #region 动态码

        /// <summary>
        /// 获取当前时间所在的时间片
        /// </summary>
        public static long GetStep(DateTime? utcTime = null)
        {
            var time = utcTime ?? DateTime.UtcNow;
            return GetUnixSeconds(time) / StepSeconds;
        }

        /// <summary>
        /// 生成指定时间片的动态码
        /// </summary>
        public static string GetCode(string secret, DateTime? utcTime = null)
        {
            return Generate(secret, GetStep(utcTime));
        }

        /// <summary>
        /// 校验动态码
        /// </summary>
        /// <param name="secret">Base32 密钥</param>
        /// <param name="code">用户输入的动态码</param>
        /// <param name="window">允许的时间片偏移（前后各 N 个）</param>
        /// <param name="lastStep">上次使用的时间片（防重放，同窗口不可重复使用）</param>
        /// <param name="usedStep">校验通过时的时间片</param>
        /// <returns>是否通过</returns>
        public static bool Validate(string secret, string code, int window, long lastStep, out long usedStep)
        {
            usedStep = 0;
            if (string.IsNullOrWhiteSpace(secret) || string.IsNullOrWhiteSpace(code))
            {
                return false;
            }

            var input = code.Replace(" ", string.Empty).Replace("-", string.Empty).Trim();
            if (input.Length != CodeDigits || !input.All(char.IsDigit))
            {
                return false;
            }

            var current = GetStep();
            for (var i = -window; i <= window; i++)
            {
                var candidate = current + i;
                // 已使用过的时间片不可重复通过
                if (candidate <= lastStep)
                {
                    continue;
                }
                if (Generate(secret, candidate) == input)
                {
                    usedStep = candidate;
                    return true;
                }
            }
            return false;
        }

        #endregion

        #region 私有方法

        private static string Generate(string secret, long step)
        {
            var key = FromBase32(secret);
            // 时间计数器为 8 字节大端
            var counter = BitConverter.GetBytes((long)step);
            if (BitConverter.IsLittleEndian)
            {
                counter = counter.Reverse().ToArray();
            }

            using var hmac = new HMACSHA1(key);
            var hash = hmac.ComputeHash(counter);

            // 动态截断
            var offset = hash[^1] & 0x0F;
            var binary =
                ((hash[offset] & 0x7F) << 24) |
                ((hash[offset + 1] & 0xFF) << 16) |
                ((hash[offset + 2] & 0xFF) << 8) |
                (hash[offset + 3] & 0xFF);

            var code = binary % (int)Math.Pow(10, CodeDigits);
            return code.ToString().PadLeft(CodeDigits, '0');
        }

        private static long GetUnixSeconds(DateTime time)
        {
            var utc = time.Kind == DateTimeKind.Utc ? time : time.ToUniversalTime();
            return (long)(utc - DateTime.UnixEpoch).TotalSeconds;
        }

        #endregion

        #region Base32

        /// <summary>
        /// 字节数组转 Base32
        /// </summary>
        public static string ToBase32(byte[] data)
        {
            if (data == null || data.Length == 0)
            {
                return string.Empty;
            }

            var result = new StringBuilder((data.Length + 7) * 8 / 5);
            int bitBuffer = 0, bitCount = 0;

            foreach (var b in data)
            {
                bitBuffer = (bitBuffer << 8) | b;
                bitCount += 8;

                while (bitCount >= 5)
                {
                    var index = (bitBuffer >> (bitCount - 5)) & 31;
                    result.Append(Base32Alphabet[index]);
                    bitCount -= 5;
                }
            }

            if (bitCount > 0)
            {
                var index = (bitBuffer << (5 - bitCount)) & 31;
                result.Append(Base32Alphabet[index]);
            }

            return result.ToString();
        }

        /// <summary>
        /// Base32 转字节数组（忽略大小写与空格，缺失的填充位按 0 处理）
        /// </summary>
        public static byte[] FromBase32(string base32)
        {
            if (string.IsNullOrWhiteSpace(base32))
            {
                return Array.Empty<byte>();
            }

            var clean = base32.Trim().Replace(" ", string.Empty).Replace("-", string.Empty).TrimEnd('=').ToUpperInvariant();
            var bytes = new List<byte>((clean.Length * 5 + 7) / 8);
            int bitBuffer = 0, bitCount = 0;

            foreach (var c in clean)
            {
                var index = Base32Alphabet.IndexOf(c);
                if (index < 0)
                {
                    throw new FormatException($"非法的 Base32 字符：{c}");
                }

                bitBuffer = (bitBuffer << 5) | index;
                bitCount += 5;

                if (bitCount >= 8)
                {
                    bytes.Add((byte)((bitBuffer >> (bitCount - 8)) & 0xFF));
                    bitCount -= 8;
                }
            }

            return bytes.ToArray();
        }

        #endregion
    }
}
