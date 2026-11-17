import request from '@/utils/request'

// 当前用户未读消息数（铃铛角标）
export function getUnreadCount() {
    return request({
        url: '/system/message/unread',
        method: 'get',
    })
}

// 查询消息列表
export function getMessage(id) {
    return request({
        url: '/system/message/' + id,
        method: 'get',
    })
}

// 查询消息列表
export function listMessage(query) {
    return request({
        url: '/system/message/list',
        method: 'get',
        params: query
    })
}

// 全部已读
export function readAllMessage() {
    return request({
        url: '/system/message/read/all',
        method: 'post',
    })
}

// 已读消息
export function readMessage(id) {
    return request({
        url: '/system/message/read/' + id,
        method: 'post',
    })
}

// 已读消息
export function readMessages(ids) {
    return request({
        url: '/system/message/read',
        method: 'post',
        data: {
            ids
        }
    })
}

// 删除消息
export function delMessage(ids) {
    return request({
        url: '/system/message/' + ids,
        method: 'delete'
    })
}