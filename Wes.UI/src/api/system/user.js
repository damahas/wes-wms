import request from '@/utils/request'

// 查询用户列表
export function listUser(query) {
  return request({
    url: '/system/user/list',
    method: 'get',
    params: query
  })
}

// 查询用户详细
export function getUser(userId) {
  return request({
    url: '/system/user/' + userId ?? 0,
    method: 'get'
  })
}

// 新增用户
export function addUser(data) {
  return request({
    url: '/system/user',
    method: 'post',
    data: data
  })
}

// 修改用户
export function updateUser(data) {
  return request({
    url: '/system/user',
    method: 'put',
    data: data
  })
}

// 删除用户
export function delUser(userId) {
  return request({
    url: '/system/user/' + userId,
    method: 'delete'
  })
}

// 用户密码重置（password 为 RSA 加密后的密文）
export function resetUserPwd(userId, password) {
  const data = {
    userId,
    password
  }
  return request({
    url: '/system/user/resetPwd',
    method: 'put',
    data: data
  })
}

// 用户状态修改
export function changeUserStatus(userId, status) {
  const data = {
    userId,
    status
  }
  return request({
    url: '/system/user/changeStatus',
    method: 'put',
    data: data
  })
}

// 查询用户个人信息
export function getUserProfile() {
  return request({
    url: '/system/user/profile',
    method: 'get'
  })
}

// 修改用户个人信息
export function updateUserProfile(data) {
  return request({
    url: '/system/user/profile',
    method: 'put',
    data: data
  })
}

// 修改当前用户密码（密码经 RSA 加密，走 body 传输）
export function updateUserPwd(oldPassword, newPassword) {
  const data = {
    oldPassword,
    newPassword
  }
  return request({
    url: '/system/user/profile/updatePwd',
    method: 'put',
    data: data
  })
}

// 用户头像上传
export function uploadAvatar(data) {
  return request({
    url: '/system/user/profile/avatar',
    method: 'post',
    headers: { 'Content-Type': 'application/x-www-form-urlencoded' },
    data: data
  })
}

// 查询授权角色
export function getAuthRole(userId) {
  return request({
    url: '/system/user/authRole/' + userId,
    method: 'get'
  })
}

// 查询当前用户两步验证状态
export function getTwoFactorStatus() {
  return request({
    url: '/system/user/2fa/status',
    method: 'get'
  })
}

// 两步验证绑定准备（方式由系统参数配置决定）
export function prepareTwoFactor() {
  return request({
    url: '/system/user/2fa/prepare',
    method: 'get'
  })
}

// 确认绑定两步验证
export function confirmTwoFactor(data) {
  return request({
    url: '/system/user/2fa/bind',
    method: 'post',
    data: data
  })
}

// 解绑两步验证
export function unbindTwoFactor(data) {
  return request({
    url: '/system/user/2fa/unbind',
    method: 'post',
    data: data
  })
}

// 管理员重置用户两步验证
export function resetTwoFactor(userId) {
  return request({
    url: '/system/user/2fa/reset/' + userId,
    method: 'put'
  })
}

// 保存授权角色
export function updateAuthRole(data) {
  return request({
    url: '/system/user/authRole',
    method: 'put',
    params: data
  })
}

// 查询部门下拉树结构
export function deptTreeSelect() {
  return request({
    url: '/system/dept/treeselect',
    method: 'get'
  })
}
