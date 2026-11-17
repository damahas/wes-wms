import request from "@/utils/request";

// 获取验证码图片
export function getCodeImg(width, height) {
  return request({
    url: `/captchaImage?width=${width}&height=${height}`,
    headers: {
      isToken: false,
    },
    method: "get",
    timeout: 20000,
  });
}

// 验证验证码
export function validCodeImg(code, sliderPositionX) {
  return request({
    url: `/validCaptchaImage?code=${code}&sliderPositionX=${sliderPositionX}`,
    headers: {
      isToken: false,
    },
    method: "get",
    timeout: 20000,
  });
}

// 登录
export function login(data) {
  // {
  //   username,
  //   password,
  //   code,
  //   positionX
  // }
  return request({
    url: "/login",
    method: "post",
    data,
  });
}

// 获取用户详细信息
export function getInfo() {
  return request({
    url: "/getInfo",
    method: "get",
  });
}

// 退出方法
export function logout() {
  return request({
    url: '/logout',
    method: 'post'
  })
}
export function isCaptchaOn() {
  return request({
    url: "/isCaptchaOn",
    method: "get",
  });
}

// 两步验证开关与方式
export function isTwoFactorOn() {
  return request({
    url: "/isTwoFactorOn",
    method: "get",
  });
}

// 两步验证：校验验证码并完成登录
export function verifyTwoFactor(data) {
  return request({
    url: "/login/2fa/verify",
    method: "post",
    data,
  });
}

// 两步验证：发送邮箱验证码
export function sendTwoFactorEmailCode(data) {
  return request({
    url: "/login/2fa/sendEmailCode",
    method: "post",
    data,
  });
}

// 两步验证：绑定准备（验证器方式返回密钥）
export function prepareLoginBind(data) {
  return request({
    url: "/login/2fa/prepareBind",
    method: "post",
    data,
  });
}

// 两步验证：首次登录强制绑定，成功后直接登录
export function bindAndLogin(data) {
  return request({
    url: "/login/2fa/bind",
    method: "post",
    data,
  });
}