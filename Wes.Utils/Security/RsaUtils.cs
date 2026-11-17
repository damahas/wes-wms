using System;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace Wes.Utils.Security
{
    /// <summary>
    /// 登录/密码类接口的传输加密：前端用固定公钥做 RSA（PKCS#1 v1.5）加密，
    /// 后端用固定私钥解密，避免密码在网络链路上明文传输。
    /// 载荷格式：{ "p": "原始密码", "t": 毫秒时间戳 }
    /// </summary>
    public static class RsaUtils
    {
        /// <summary>
        /// 固定私钥（PKCS#8）。
        /// 更换方式：openssl genrsa -out login_rsa.pem 2048 生成新密钥对，
        /// openssl rsa -in login_rsa.pem -pubout 导出公钥，同步替换前端 PUBLIC_KEY_PEM。
        /// </summary>
        private const string PrivateKeyPem = @"-----BEGIN PRIVATE KEY-----
MIIEvQIBADANBgkqhkiG9w0BAQEFAASCBKcwggSjAgEAAoIBAQCaQ8lvmG/Ux0Bk
OoeoNXUn4VUcuLiEkfTC0tcHWkpPQ6b5Ad5ClMPL5ioh28SR2XrlyCgSuubvcwrt
xH6mhG3zmTFpU0T90qwPa8pZFg5R2TKBo9oJuRQe+G2BFmjvtg1pAkrlp76PyED8
WRITcRVHicoUlQWPQeyDZyWbQkjlB8ZGi26L5Bd+ZqEFS+SrdnQSegDZ42B0Flba
T48cZLw6F93d47obrdIV+2DxvTICuqpjUzVdNCmy2RAWUsuMJEAdVT4ezvT1g7GI
y5CiMpw5zaCplLyBUJFx5f3A+rB8a/iP4jNKULB8O+GRlEAB6681aN4qZfCNhsFK
h0N/+lohAgMBAAECggEASHV0pW1Fgf1BOpLfa1wItn25267MRMDF5cjg5Xw8kTSm
tilxe3ai7SAkRBkVIKi1eD7my71sgaZYyMvV7WMgvh4DvoQ1UL/mv6uStPuOKFXL
ymbrIEwFrnJkBRFAou+xJtTD11FOSfz2wVEZE3a3Vw3alIZWwS5m6GXHnUCqOqEM
iH1D2B2bVRWLtR0mmQB1zzdlJoaurkaSEyJmCWfmShpcSPjYzzX11fgpKUIsMgaD
YJ8PrxYtrhgCBpkbWfLUjf1WFpJOAttDgCAFyj3fP/kw08m6UbY+1Tr7BMRvKrd/
+O9wiJaA1BGDCQhsfMvXshX7Rh1afcjUcNRhacSybQKBgQDFM4lpMgbgsJfIbOnf
Wp3TWo8p5aPanb0T/HrAWAh8c6NFoNVK5vMDUMJN0lZIdrOuvrB5/sAvQldLcXDu
2zk6QEDe8zvaEYBVfoYWQSkhm7F70X8r3GcHcqPgJ9gDNxFWKbiG6gCIp6l09Raj
6YWgs0oDQOmv4Stzi+UmlvUv+wKBgQDIQuLqTabuFKWmIoi6JMk8Zt+eMjbQXj4u
M/nFqY65IFc35mQvc/GUDLwSVmLu7y0mvslmarzBh2d6uVS11kPUCLDojk9/Rm8a
Libbs8dLP9dclHfhETgBxcLVOUUbVeoeIiP5CRM13y5DG8i8E/A6QKQN0LB3Ky9/
/pQfeGzXkwKBgGctBLr+SqzqmC9sFJ0guyym1/qyxJ0QJKo9Ujgqlx4OHqHCeXY8
CW91pIO1BiHZwT8vVAQwLntLeYLmcpD2MCXzEj3UAz+gIoYYsa0ZIJ0N6bl/rcrU
AS/bYjRYhGOCP1bWc05bRjY5iqa/yL2v45Nl2taC8wFrjSFiU11yZAxbAoGBAKUZ
b5YXOr4hS4eyBOLhnP4tGcKER2h1qfHRa+Qnq8C4Bu/RA1uTzOBvt0Wu7pVpneHJ
fKTw7njmtssYq9vVr8nI+IBhguy9Fq+QyPxsNKu3EEgFJufomDHt5nh/ZJfqtr7t
CpcvgiTxwGQXLeDQ10ZPMHErUbCNa/Wr3lYrpB/9AoGABGLIRe/MzK7mOnaRKHV0
LKTXvpcWwjYMx5LmqeWhYwqTsGDgB4+/ZIzgs3Rp49+JJQwIMo+MCwdVm7fpeelB
5s5YXox7ENJ+SgfFBHqX1UwfUmki5IN1uU0mUeUlsIst0WFklkIIe6H/8aOX2ES/
rwXepRPL/3tKmfqwV/0d9iM=
-----END PRIVATE KEY-----";

        /// <summary>载荷时间戳允许的最大偏差（分钟），超出即拒绝</summary>
        public const int ExpireMinutes = 5;

        /// <summary>
        /// 解密前端加密的密码，失败或超窗时返回 false
        /// </summary>
        public static bool TryDecrypt(string base64Cipher, out string password)
        {
            password = null;
            if (string.IsNullOrWhiteSpace(base64Cipher))
            {
                return false;
            }

            try
            {
                using var rsa = RSA.Create();
                rsa.ImportFromPem(PrivateKeyPem);
                var plain = Encoding.UTF8.GetString(
                    rsa.Decrypt(Convert.FromBase64String(base64Cipher), RSAEncryptionPadding.Pkcs1));

                using var doc = JsonDocument.Parse(plain);
                var root = doc.RootElement;
                if (!root.TryGetProperty("p", out var pwdElement) || !root.TryGetProperty("t", out var timeElement))
                {
                    return false;
                }

                var timestamp = timeElement.GetInt64();
                var offset = Math.Abs(DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() - timestamp);
                if (offset > ExpireMinutes * 60L * 1000L)
                {
                    return false;
                }

                password = pwdElement.GetString();
                return !string.IsNullOrEmpty(password);
            }
            catch
            {
                return false;
            }
        }
    }
}
