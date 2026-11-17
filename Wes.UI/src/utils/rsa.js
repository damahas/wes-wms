import JSEncrypt from "jsencrypt";

/**
 * 登录/密码类接口的传输加密：RSA-2048 + PKCS#1 v1.5
 * 后端固定私钥见 Wes.Utils/Security/RsaUtils.cs，更换密钥时需同步替换此处公钥
 */
const PUBLIC_KEY_PEM = `-----BEGIN PUBLIC KEY-----
MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAmkPJb5hv1MdAZDqHqDV1
J+FVHLi4hJH0wtLXB1pKT0Om+QHeQpTDy+YqIdvEkdl65cgoErrm73MK7cR+poRt
85kxaVNE/dKsD2vKWRYOUdkygaPaCbkUHvhtgRZo77YNaQJK5ae+j8hA/FkSE3EV
R4nKFJUFj0Hsg2clm0JI5QfGRotui+QXfmahBUvkq3Z0EnoA2eNgdBZW2k+PHGS8
Ohfd3eO6G63SFftg8b0yArqqY1M1XTQpstkQFlLLjCRAHVU+Hs709YOxiMuQojKc
Oc2gqZS8gVCRceX9wPqwfGv4j+IzSlCwfDvhkZRAAeuvNWjeKmXwjYbBSodDf/pa
IQIDAQAB
-----END PUBLIC KEY-----`;

let encryptor = null;

const getEncryptor = () => {
  if (!encryptor) {
    encryptor = new JSEncrypt();
    encryptor.setPublicKey(PUBLIC_KEY_PEM);
  }
  return encryptor;
};

/**
 * 加密明文密码，返回 base64 密文
 * 载荷带毫秒时间戳：PKCS#1 v1.5 无内部随机数，时间戳保证每次密文不同并可防重放
 * @param {string} plain 明文密码
 * @returns {string} base64 密文
 */
export const encryptPassword = (plain) => {
  if (plain === null || plain === undefined || plain === "") {
    return "";
  }
  const payload = JSON.stringify({ p: String(plain), t: Date.now() });
  const cipher = getEncryptor().encrypt(payload);
  if (!cipher) {
    throw new Error("密码加密失败，请刷新页面后重试");
  }
  return cipher;
};
