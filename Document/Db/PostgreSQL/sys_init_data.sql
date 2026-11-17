-- =============================================
-- WMS 系统初始化数据（PostgreSQL）
-- 与 MySQL 版同源生成：2026-11-17
-- =============================================

-- ---------------------------- sys_config ----------------------------
DROP TABLE IF EXISTS sys_config;
CREATE TABLE sys_config (
  "config_id" bigint NOT NULL,
  "config_name" varchar(100) DEFAULT '',
  "config_key" varchar(100) DEFAULT '',
  "config_value" varchar(500) DEFAULT NULL,
  "config_type" varchar(1) DEFAULT NULL,
  "is_del" integer DEFAULT 0,
  "create_by" varchar(64) DEFAULT '',
  "create_time" timestamp DEFAULT NULL,
  "update_by" varchar(64) DEFAULT '',
  "update_time" timestamp DEFAULT NULL,
  "remark" varchar(500) DEFAULT NULL,
  "sort_by" integer DEFAULT NULL,
  PRIMARY KEY (config_id)
);
COMMENT ON COLUMN sys_config.config_id IS '参数主键';
COMMENT ON COLUMN sys_config.config_name IS '参数名称';
COMMENT ON COLUMN sys_config.config_key IS '参数键名';
COMMENT ON COLUMN sys_config.config_value IS '参数键值';
COMMENT ON COLUMN sys_config.config_type IS '系统内置（Y是 N否）';
COMMENT ON COLUMN sys_config.is_del IS '删除标志（0代表存在 1代表删除）';
COMMENT ON COLUMN sys_config.create_by IS '创建者';
COMMENT ON COLUMN sys_config.create_time IS '创建时间';
COMMENT ON COLUMN sys_config.update_by IS '更新者';
COMMENT ON COLUMN sys_config.update_time IS '更新时间';
COMMENT ON COLUMN sys_config.remark IS '备注';
COMMENT ON COLUMN sys_config.sort_by IS '排序字段';
DELETE FROM sys_config;
INSERT INTO sys_config (config_id, config_name, config_key, config_value, config_type, is_del, create_by, create_time, update_by, update_time, remark, sort_by) VALUES (10000010, '【系统】账号初始密码', 'sys.user.initPassword', '123456', 'Y', NULL, NULL, '1900-01-01 00:00:00', 'admin', '2026-07-02 01:35:31', '初始化密码 123456', NULL);
INSERT INTO sys_config (config_id, config_name, config_key, config_value, config_type, is_del, create_by, create_time, update_by, update_time, remark, sort_by) VALUES (10000011, '登录验证码', 'sys.login.isCaptchaOn', '{"enabled":false}', 'Y', 0, NULL, '1900-01-01 00:00:00', 'admin', '2026-07-02 00:35:24', '是否开启验证码功能（true开启，false关闭）', 3);
INSERT INTO sys_config (config_id, config_name, config_key, config_value, config_type, is_del, create_by, create_time, update_by, update_time, remark, sort_by) VALUES (10000020, '【系统】子系统配置', 'sys.subSystem', '[{"systemId":"1","systemName":"23","systemUrl":"3"}]', 'Y', 0, 'admin', '2022-04-15 15:51:02', 'admin', '2026-04-07 18:06:27', NULL, 1);
INSERT INTO sys_config (config_id, config_name, config_key, config_value, config_type, is_del, create_by, create_time, update_by, update_time, remark, sort_by) VALUES (1860615424161484806, '【仓库】货物动态属性', 'wms.goods.attr', '[{"title":"木方编号","name":"attr1"}]', 'Y', 0, 'admin', '2022-10-01 01:08:31', 'admin', '2023-08-19 22:41:25', '', 6);
INSERT INTO sys_config (config_id, config_name, config_key, config_value, config_type, is_del, create_by, create_time, update_by, update_time, remark, sort_by) VALUES (1860615424161484807, '【仓库】自动出库', 'wms.outbound.auto', 'true', 'Y', 0, 'admin', '2022-11-16 23:55:59', NULL, NULL, NULL, 4);


-- ---------------------------- sys_dept ----------------------------
DROP TABLE IF EXISTS sys_dept;
CREATE TABLE sys_dept (
  "dept_id" bigint NOT NULL,
  "parent_id" bigint NOT NULL,
  "ancestors" varchar(50) DEFAULT '',
  "dept_name" varchar(30) DEFAULT '',
  "order_num" integer DEFAULT 0,
  "leader_user_id" bigint DEFAULT NULL,
  "phone" varchar(11) DEFAULT NULL,
  "email" varchar(50) DEFAULT NULL,
  "status" varchar(1) DEFAULT NULL,
  "is_del" integer DEFAULT 0,
  "create_by" varchar(64) DEFAULT '',
  "create_time" timestamp DEFAULT NULL,
  "update_by" varchar(64) DEFAULT '',
  "update_time" timestamp DEFAULT NULL,
  "dept_level" varchar(50) DEFAULT NULL,
  PRIMARY KEY (dept_id)
);
COMMENT ON COLUMN sys_dept.dept_id IS '部门id';
COMMENT ON COLUMN sys_dept.parent_id IS '父部门id';
COMMENT ON COLUMN sys_dept.ancestors IS '祖级列表';
COMMENT ON COLUMN sys_dept.dept_name IS '部门名称';
COMMENT ON COLUMN sys_dept.order_num IS '显示顺序';
COMMENT ON COLUMN sys_dept.leader_user_id IS '负责人';
COMMENT ON COLUMN sys_dept.phone IS '联系电话';
COMMENT ON COLUMN sys_dept.email IS '邮箱';
COMMENT ON COLUMN sys_dept.status IS '部门状态（0正常 1停用）';
COMMENT ON COLUMN sys_dept.is_del IS '删除标志（0代表存在 1代表删除）';
COMMENT ON COLUMN sys_dept.create_by IS '创建者';
COMMENT ON COLUMN sys_dept.create_time IS '创建时间';
COMMENT ON COLUMN sys_dept.update_by IS '更新者';
COMMENT ON COLUMN sys_dept.update_time IS '更新时间';
COMMENT ON COLUMN sys_dept.dept_level IS '部门等级';
DELETE FROM sys_dept;
INSERT INTO sys_dept (dept_id, parent_id, ancestors, dept_name, order_num, leader_user_id, phone, email, status, is_del, create_by, create_time, update_by, update_time, dept_level) VALUES (100, 0, '0', '二狗科技', 0, NULL, '15888888888', 'ry@qq.com', '0', 0, 'admin', '2022-04-15 15:51:01', 'admin', '2026-03-07 22:07:14', NULL);
INSERT INTO sys_dept (dept_id, parent_id, ancestors, dept_name, order_num, leader_user_id, phone, email, status, is_del, create_by, create_time, update_by, update_time, dept_level) VALUES (101, 100, '0,100', '深圳总公司', 1, 108, '15888888888', 'ry@qq.com', '0', 0, 'admin', '2022-04-15 15:51:01', 'admin', '2023-01-15 23:14:51', NULL);
INSERT INTO sys_dept (dept_id, parent_id, ancestors, dept_name, order_num, leader_user_id, phone, email, status, is_del, create_by, create_time, update_by, update_time, dept_level) VALUES (102, 100, '0,100', '长沙分公司', 2, NULL, '15888888888', 'ry@qq.com', '0', 0, 'admin', '2022-04-15 15:51:01', '', NULL, NULL);
INSERT INTO sys_dept (dept_id, parent_id, ancestors, dept_name, order_num, leader_user_id, phone, email, status, is_del, create_by, create_time, update_by, update_time, dept_level) VALUES (103, 101, '0,100,101', '研发部门', 1, NULL, '15888888888', 'ry@qq.com', '0', 0, 'admin', '2022-04-15 15:51:01', 'admin', '2022-05-12 23:57:23', NULL);
INSERT INTO sys_dept (dept_id, parent_id, ancestors, dept_name, order_num, leader_user_id, phone, email, status, is_del, create_by, create_time, update_by, update_time, dept_level) VALUES (104, 101, '0,100,101', '市场部门', 2, NULL, '15888888888', 'ry@qq.com', '0', 0, 'admin', '2022-04-15 15:51:01', '', NULL, NULL);
INSERT INTO sys_dept (dept_id, parent_id, ancestors, dept_name, order_num, leader_user_id, phone, email, status, is_del, create_by, create_time, update_by, update_time, dept_level) VALUES (105, 101, '0,100,101', '测试部门', 3, NULL, '15888888888', 'ry@qq.com', '0', 0, 'admin', '2022-04-15 15:51:01', '', NULL, NULL);
INSERT INTO sys_dept (dept_id, parent_id, ancestors, dept_name, order_num, leader_user_id, phone, email, status, is_del, create_by, create_time, update_by, update_time, dept_level) VALUES (106, 101, '0,100,101', '财务部门', 4, NULL, '15888888888', 'ry@qq.com', '0', 0, 'admin', '2022-04-15 15:51:01', '', NULL, NULL);
INSERT INTO sys_dept (dept_id, parent_id, ancestors, dept_name, order_num, leader_user_id, phone, email, status, is_del, create_by, create_time, update_by, update_time, dept_level) VALUES (107, 101, '0,100,101', '运维部门', 5, NULL, '15888888888', 'ry@qq.com', '0', 0, 'admin', '2022-04-15 15:51:01', '', NULL, NULL);
INSERT INTO sys_dept (dept_id, parent_id, ancestors, dept_name, order_num, leader_user_id, phone, email, status, is_del, create_by, create_time, update_by, update_time, dept_level) VALUES (108, 102, '0,100,102', '市场部门', 1, NULL, '15888888888', 'ry@qq.com', '0', 0, 'admin', '2022-04-15 15:51:01', '', NULL, NULL);
INSERT INTO sys_dept (dept_id, parent_id, ancestors, dept_name, order_num, leader_user_id, phone, email, status, is_del, create_by, create_time, update_by, update_time, dept_level) VALUES (109, 102, '0,100,102', '财务部门', 2, NULL, '15888888888', 'ry@qq.com', '0', 0, 'admin', '2022-04-15 15:51:01', '', NULL, NULL);
INSERT INTO sys_dept (dept_id, parent_id, ancestors, dept_name, order_num, leader_user_id, phone, email, status, is_del, create_by, create_time, update_by, update_time, dept_level) VALUES (2066408560685879296, 2066415154064658432, '2066415154064658432', '技术研发部', 1085091401, NULL, NULL, NULL, '0', 0, 'system', '2026-06-15 14:32:47', NULL, '2026-06-15 20:29:01', NULL);
INSERT INTO sys_dept (dept_id, parent_id, ancestors, dept_name, order_num, leader_user_id, phone, email, status, is_del, create_by, create_time, update_by, update_time, dept_level) VALUES (2066408561927393280, 2066415154064658432, '2066415154064658432', '销售部', 1084505570, NULL, NULL, NULL, '0', 0, 'system', '2026-06-15 14:32:48', NULL, '2026-06-15 20:29:01', NULL);
INSERT INTO sys_dept (dept_id, parent_id, ancestors, dept_name, order_num, leader_user_id, phone, email, status, is_del, create_by, create_time, update_by, update_time, dept_level) VALUES (2066408562711728128, 2066415154064658432, '2066415154064658432', '生产部', 1084845536, NULL, NULL, NULL, '0', 0, 'system', '2026-06-15 14:32:48', NULL, '2026-06-15 20:29:01', NULL);
INSERT INTO sys_dept (dept_id, parent_id, ancestors, dept_name, order_num, leader_user_id, phone, email, status, is_del, create_by, create_time, update_by, update_time, dept_level) VALUES (2066408563538006016, 2066415154064658432, '2066415154064658432', '供应链部', 1084944419, NULL, NULL, NULL, '0', 0, 'system', '2026-06-15 14:32:48', NULL, '2026-06-15 20:29:01', NULL);
INSERT INTO sys_dept (dept_id, parent_id, ancestors, dept_name, order_num, leader_user_id, phone, email, status, is_del, create_by, create_time, update_by, update_time, dept_level) VALUES (2066408565375111168, 2066415154064658432, '2066415154064658432', '财务部', 1084942359, NULL, NULL, NULL, '0', 0, 'system', '2026-06-15 14:32:49', NULL, '2026-06-15 20:29:01', NULL);
INSERT INTO sys_dept (dept_id, parent_id, ancestors, dept_name, order_num, leader_user_id, phone, email, status, is_del, create_by, create_time, update_by, update_time, dept_level) VALUES (2066408566247526400, 2066408562711728128, '2066415154064658432,2066408562711728128', '设备维护科', 1084562550, NULL, NULL, NULL, '0', 0, 'system', '2026-06-15 14:32:49', NULL, '2026-06-15 20:29:01', NULL);
INSERT INTO sys_dept (dept_id, parent_id, ancestors, dept_name, order_num, leader_user_id, phone, email, status, is_del, create_by, create_time, update_by, update_time, dept_level) VALUES (2066408567036055552, 2066408562711728128, '2066415154064658432,2066408562711728128', '一车间', 1084566691, NULL, NULL, NULL, '0', 0, 'system', '2026-06-15 14:32:49', NULL, '2026-06-15 20:29:01', NULL);
INSERT INTO sys_dept (dept_id, parent_id, ancestors, dept_name, order_num, leader_user_id, phone, email, status, is_del, create_by, create_time, update_by, update_time, dept_level) VALUES (2066408567828779008, 2066408562711728128, '2066415154064658432,2066408562711728128', '二车间', 1084619690, NULL, NULL, NULL, '0', 0, 'system', '2026-06-15 14:32:49', NULL, '2026-06-15 20:29:01', NULL);
INSERT INTO sys_dept (dept_id, parent_id, ancestors, dept_name, order_num, leader_user_id, phone, email, status, is_del, create_by, create_time, update_by, update_time, dept_level) VALUES (2066408570584436736, 2066408563538006016, '2066415154064658432,2066408563538006016', '仓储科', 1084978430, NULL, NULL, NULL, '0', 0, 'system', '2026-06-15 14:32:50', NULL, '2026-06-15 20:29:01', NULL);
INSERT INTO sys_dept (dept_id, parent_id, ancestors, dept_name, order_num, leader_user_id, phone, email, status, is_del, create_by, create_time, update_by, update_time, dept_level) VALUES (2066408574942318592, 2066408563538006016, '2066415154064658432,2066408563538006016', '采购科', 1084898355, NULL, NULL, NULL, '0', 0, 'system', '2026-06-15 14:32:51', NULL, '2026-06-15 20:29:02', NULL);
INSERT INTO sys_dept (dept_id, parent_id, ancestors, dept_name, order_num, leader_user_id, phone, email, status, is_del, create_by, create_time, update_by, update_time, dept_level) VALUES (2066415154064658432, 0, '0', '二狗子元宇宙养猪研讨会', 0, NULL, NULL, NULL, '0', 0, 'system', '2026-06-15 14:58:59', NULL, '2026-06-15 20:29:01', NULL);


-- ---------------------------- sys_dict_type ----------------------------
DROP TABLE IF EXISTS sys_dict_type;
CREATE TABLE sys_dict_type (
  "dict_id" bigint NOT NULL,
  "dict_name" varchar(100) DEFAULT '',
  "dict_type" varchar(100) DEFAULT '',
  "status" varchar(1) DEFAULT NULL,
  "create_by" varchar(64) DEFAULT '',
  "create_time" timestamp DEFAULT NULL,
  "update_by" varchar(64) DEFAULT '',
  "update_time" timestamp DEFAULT NULL,
  "remark" varchar(500) DEFAULT NULL,
  "is_del" integer DEFAULT 0,
  PRIMARY KEY (dict_id)
);
CREATE UNIQUE INDEX sys_dict_type_dict_type ON sys_dict_type (dict_type);
COMMENT ON COLUMN sys_dict_type.dict_id IS '字典主键';
COMMENT ON COLUMN sys_dict_type.dict_name IS '字典名称';
COMMENT ON COLUMN sys_dict_type.dict_type IS '字典类型';
COMMENT ON COLUMN sys_dict_type.status IS '状态（0正常 1停用）';
COMMENT ON COLUMN sys_dict_type.create_by IS '创建者';
COMMENT ON COLUMN sys_dict_type.create_time IS '创建时间';
COMMENT ON COLUMN sys_dict_type.update_by IS '更新者';
COMMENT ON COLUMN sys_dict_type.update_time IS '更新时间';
COMMENT ON COLUMN sys_dict_type.remark IS '备注';
COMMENT ON COLUMN sys_dict_type.is_del IS '删除标志（0代表存在 1代表删除）';
DELETE FROM sys_dict_type;
INSERT INTO sys_dict_type (dict_id, dict_name, dict_type, status, create_by, create_time, update_by, update_time, remark, is_del) VALUES (40, '仓库库区类型', 'wms_area_type', '0', 'admin', '2022-08-14 23:53:18', NULL, NULL, NULL, 1);
INSERT INTO sys_dict_type (dict_id, dict_name, dict_type, status, create_by, create_time, update_by, update_time, remark, is_del) VALUES (41, '仓库货物类别', 'wms_goods_type', '0', 'admin', '2022-09-26 21:57:22', 'admin', '2022-09-26 21:57:40', NULL, 1);
INSERT INTO sys_dict_type (dict_id, dict_name, dict_type, status, create_by, create_time, update_by, update_time, remark, is_del) VALUES (42, '仓库货物状态', 'wms_goods_status', '0', 'admin', '2022-09-28 00:54:40', NULL, NULL, NULL, 1);
INSERT INTO sys_dict_type (dict_id, dict_name, dict_type, status, create_by, create_time, update_by, update_time, remark, is_del) VALUES (43, '仓库送货单来源', 'wms_delivery_source', '0', 'admin', '2022-10-01 22:47:49', NULL, NULL, NULL, 1);
INSERT INTO sys_dict_type (dict_id, dict_name, dict_type, status, create_by, create_time, update_by, update_time, remark, is_del) VALUES (44, '仓库货物规格', 'wms_goods_standard', '0', 'admin', '2022-10-23 22:58:03', NULL, NULL, NULL, 1);
INSERT INTO sys_dict_type (dict_id, dict_name, dict_type, status, create_by, create_time, update_by, update_time, remark, is_del) VALUES (45, '仓库货物等级', 'wms_goods_quality', '0', 'admin', '2022-10-23 22:58:31', 'admin', '2022-10-23 22:58:48', NULL, 1);
INSERT INTO sys_dict_type (dict_id, dict_name, dict_type, status, create_by, create_time, update_by, update_time, remark, is_del) VALUES (46, '仓库货物工序', 'wms_goods_process', '0', 'admin', '2022-10-23 22:59:14', 'admin', '2024-11-01 23:19:34', NULL, 1);
INSERT INTO sys_dict_type (dict_id, dict_name, dict_type, status, create_by, create_time, update_by, update_time, remark, is_del) VALUES (900001, '物料类型', 'wms_material_type', '0', 'system', '2026-07-23 22:15:25', 'admin', '2026-07-23 23:30:44', 'WMS物料类型', 0);
INSERT INTO sys_dict_type (dict_id, dict_name, dict_type, status, create_by, create_time, update_by, update_time, remark, is_del) VALUES (900003, '物料分类', 'wms_material_category', '0', 'system', '2026-07-23 22:15:25', 'system', '2026-07-23 22:15:25', 'WMS物料分类(树状)', 0);
INSERT INTO sys_dict_type (dict_id, dict_name, dict_type, status, create_by, create_time, update_by, update_time, remark, is_del) VALUES (900004, '锁定类型', 'wms_lock_type', '0', 'system', '2026-07-21 23:35:14', 'system', '2026-07-21 23:35:14', 'WMS库位锁定类型', 0);
INSERT INTO sys_dict_type (dict_id, dict_name, dict_type, status, create_by, create_time, update_by, update_time, remark, is_del) VALUES (900005, '仓库类型', 'wms_warehouse_type', '0', 'system', '2026-07-23 22:14:48', 'system', '2026-07-23 22:14:48', 'WMS仓库类型', 0);
INSERT INTO sys_dict_type (dict_id, dict_name, dict_type, status, create_by, create_time, update_by, update_time, remark, is_del) VALUES (900006, '库区类型', 'wms_zone_type', '0', 'system', '2026-07-23 22:48:16', 'system', '2026-07-23 22:48:16', 'WMS库区类型', 0);
INSERT INTO sys_dict_type (dict_id, dict_name, dict_type, status, create_by, create_time, update_by, update_time, remark, is_del) VALUES (900007, '库位类型', 'wms_location_type', '0', 'system', '2026-07-23 22:48:17', 'system', '2026-07-23 22:48:17', 'WMS库位类型', 0);
INSERT INTO sys_dict_type (dict_id, dict_name, dict_type, status, create_by, create_time, update_by, update_time, remark, is_del) VALUES (1860613374338011136, '站内信类型', 'sys_message_type', '0', 'admin', '2024-11-24 17:16:12', NULL, NULL, '站内信类型', 0);
INSERT INTO sys_dict_type (dict_id, dict_name, dict_type, status, create_by, create_time, update_by, update_time, remark, is_del) VALUES (1860614565067362304, '菜单状态', 'sys_show_hide', '0', 'admin', '2022-04-15 15:51:02', '', NULL, '菜单状态列表', 0);
INSERT INTO sys_dict_type (dict_id, dict_name, dict_type, status, create_by, create_time, update_by, update_time, remark, is_del) VALUES (1860614591415980032, '系统开关', 'sys_normal_disable', '0', 'admin', '2022-04-15 15:51:02', '', NULL, '系统开关列表', 0);
INSERT INTO sys_dict_type (dict_id, dict_name, dict_type, status, create_by, create_time, update_by, update_time, remark, is_del) VALUES (1860614621262647296, '任务状态', 'sys_job_status', '0', 'admin', '2022-04-15 15:51:02', '', NULL, '任务状态列表', 0);
INSERT INTO sys_dict_type (dict_id, dict_name, dict_type, status, create_by, create_time, update_by, update_time, remark, is_del) VALUES (1860614640476753920, '任务分组', 'sys_job_group', '0', 'admin', '2022-04-15 15:51:02', '', NULL, '任务分组列表', 0);
INSERT INTO sys_dict_type (dict_id, dict_name, dict_type, status, create_by, create_time, update_by, update_time, remark, is_del) VALUES (1860615007348330496, '系统是否', 'sys_yes_no', '0', 'admin', '2022-04-15 15:51:02', '', NULL, '系统是否列表', 0);
INSERT INTO sys_dict_type (dict_id, dict_name, dict_type, status, create_by, create_time, update_by, update_time, remark, is_del) VALUES (1860615007348330497, '优先级类型', 'sys_priority_type', '0', 'admin', '2022-04-15 15:51:02', 'admin', '2022-07-06 01:06:05', '优先级类型列表', 0);
INSERT INTO sys_dict_type (dict_id, dict_name, dict_type, status, create_by, create_time, update_by, update_time, remark, is_del) VALUES (1860615007348330499, '系统状态', 'sys_common_status', '0', 'admin', '2022-04-15 15:51:02', '', NULL, '登录状态列表', 0);
INSERT INTO sys_dict_type (dict_id, dict_name, dict_type, status, create_by, create_time, update_by, update_time, remark, is_del) VALUES (1860615007348330500, '操作类型', 'sys_oper_type', '0', 'admin', '2022-04-15 15:51:02', '', NULL, '操作类型列表', 0);
INSERT INTO sys_dict_type (dict_id, dict_name, dict_type, status, create_by, create_time, update_by, update_time, remark, is_del) VALUES (1860615007348330501, '序号生成类型', 'sys_code_type', '0', 'admin', '2022-07-24 22:57:31', NULL, NULL, '系统序号生成类型', 0);
INSERT INTO sys_dict_type (dict_id, dict_name, dict_type, status, create_by, create_time, update_by, update_time, remark, is_del) VALUES (1860615007348330503, '流程模块', 'sys_flow_field', '0', 'admin', '2022-12-18 17:22:50', NULL, NULL, '流程模块类型', 0);
INSERT INTO sys_dict_type (dict_id, dict_name, dict_type, status, create_by, create_time, update_by, update_time, remark, is_del) VALUES (1860615007348330505, '系统单位', 'wms_sys_unit_type', '0', 'admin', '2022-04-15 15:51:02', '', NULL, '系统单位类型', 0);
INSERT INTO sys_dict_type (dict_id, dict_name, dict_type, status, create_by, create_time, update_by, update_time, remark, is_del) VALUES (1872169845576044544, '数据服务分类', 'sys_data_service_category', '0', 'admin', '2024-12-26 14:37:29', 'admin', '2024-12-26 14:37:40', '数据服务类型', 0);


-- ---------------------------- sys_dict_data ----------------------------
DROP TABLE IF EXISTS sys_dict_data;
CREATE TABLE sys_dict_data (
  "dict_data_id" bigint NOT NULL,
  "dict_sort" integer DEFAULT 0,
  "dict_label" varchar(100) DEFAULT '',
  "dict_value" varchar(100) DEFAULT '',
  "dict_type" varchar(100) DEFAULT '',
  "css_class" varchar(100) DEFAULT NULL,
  "list_class" varchar(100) DEFAULT NULL,
  "is_default" varchar(1) DEFAULT NULL,
  "status" varchar(1) DEFAULT NULL,
  "parent_id" bigint NOT NULL,
  "create_by" varchar(64) DEFAULT '',
  "create_time" timestamp DEFAULT NULL,
  "update_by" varchar(64) DEFAULT '',
  "update_time" timestamp DEFAULT NULL,
  "remark" varchar(500) DEFAULT NULL,
  "is_del" integer DEFAULT 0,
  PRIMARY KEY (dict_data_id)
);
COMMENT ON COLUMN sys_dict_data.dict_data_id IS '字典编码';
COMMENT ON COLUMN sys_dict_data.dict_sort IS '字典排序';
COMMENT ON COLUMN sys_dict_data.dict_label IS '字典标签';
COMMENT ON COLUMN sys_dict_data.dict_value IS '字典键值';
COMMENT ON COLUMN sys_dict_data.dict_type IS '字典类型';
COMMENT ON COLUMN sys_dict_data.css_class IS '样式属性（其他样式扩展）';
COMMENT ON COLUMN sys_dict_data.list_class IS '表格回显样式';
COMMENT ON COLUMN sys_dict_data.is_default IS '是否默认（Y是 N否）';
COMMENT ON COLUMN sys_dict_data.status IS '状态（0正常 1停用）';
COMMENT ON COLUMN sys_dict_data.parent_id IS '父节点id';
COMMENT ON COLUMN sys_dict_data.create_by IS '创建者';
COMMENT ON COLUMN sys_dict_data.create_time IS '创建时间';
COMMENT ON COLUMN sys_dict_data.update_by IS '更新者';
COMMENT ON COLUMN sys_dict_data.update_time IS '更新时间';
COMMENT ON COLUMN sys_dict_data.remark IS '备注';
COMMENT ON COLUMN sys_dict_data.is_del IS '删除标志（0代表存在 1代表删除）';
DELETE FROM sys_dict_data;
INSERT INTO sys_dict_data (dict_data_id, dict_sort, dict_label, dict_value, dict_type, css_class, list_class, is_default, status, parent_id, create_by, create_time, update_by, update_time, remark, is_del) VALUES (4, 1, '显示', '0', 'sys_show_hide', '', 'primary', 'Y', '0', 0, 'admin', '2022-04-15 15:51:02', 'admin', '2022-05-11 22:02:10', '显示菜单', 0);
INSERT INTO sys_dict_data (dict_data_id, dict_sort, dict_label, dict_value, dict_type, css_class, list_class, is_default, status, parent_id, create_by, create_time, update_by, update_time, remark, is_del) VALUES (5, 2, '隐藏', '1', 'sys_show_hide', '', 'danger', 'N', '0', 0, 'admin', '2022-04-15 15:51:02', '', NULL, '隐藏菜单', 0);
INSERT INTO sys_dict_data (dict_data_id, dict_sort, dict_label, dict_value, dict_type, css_class, list_class, is_default, status, parent_id, create_by, create_time, update_by, update_time, remark, is_del) VALUES (6, 1, '正常', '0', 'sys_normal_disable', '', 'primary', 'Y', '0', 0, 'admin', '2022-04-15 15:51:02', 'admin', '2022-05-20 22:51:46', '正常状态', 0);
INSERT INTO sys_dict_data (dict_data_id, dict_sort, dict_label, dict_value, dict_type, css_class, list_class, is_default, status, parent_id, create_by, create_time, update_by, update_time, remark, is_del) VALUES (7, 2, '停用', '1', 'sys_normal_disable', '', 'danger', 'N', '0', 0, 'admin', '2022-04-15 15:51:02', '', NULL, '停用状态', 0);
INSERT INTO sys_dict_data (dict_data_id, dict_sort, dict_label, dict_value, dict_type, css_class, list_class, is_default, status, parent_id, create_by, create_time, update_by, update_time, remark, is_del) VALUES (8, 1, '正常', '0', 'sys_job_status', '', 'primary', 'Y', '0', 0, 'admin', '2022-04-15 15:51:02', '', NULL, '正常状态', 0);
INSERT INTO sys_dict_data (dict_data_id, dict_sort, dict_label, dict_value, dict_type, css_class, list_class, is_default, status, parent_id, create_by, create_time, update_by, update_time, remark, is_del) VALUES (9, 2, '暂停', '1', 'sys_job_status', '', 'danger', 'N', '0', 0, 'admin', '2022-04-15 15:51:02', '', NULL, '停用状态', 0);
INSERT INTO sys_dict_data (dict_data_id, dict_sort, dict_label, dict_value, dict_type, css_class, list_class, is_default, status, parent_id, create_by, create_time, update_by, update_time, remark, is_del) VALUES (10, 1, '默认', 'DEFAULT', 'sys_job_group', '', '', 'Y', '0', 0, 'admin', '2022-04-15 15:51:02', '', NULL, '默认分组', 0);
INSERT INTO sys_dict_data (dict_data_id, dict_sort, dict_label, dict_value, dict_type, css_class, list_class, is_default, status, parent_id, create_by, create_time, update_by, update_time, remark, is_del) VALUES (11, 2, '系统', 'SYSTEM', 'sys_job_group', '', '', 'N', '0', 0, 'admin', '2022-04-15 15:51:02', '', NULL, '系统分组', 0);
INSERT INTO sys_dict_data (dict_data_id, dict_sort, dict_label, dict_value, dict_type, css_class, list_class, is_default, status, parent_id, create_by, create_time, update_by, update_time, remark, is_del) VALUES (12, 1, '是', 'Y', 'sys_yes_no', '', 'primary', 'Y', '0', 0, 'admin', '2022-04-15 15:51:02', '', NULL, '系统默认是', 0);
INSERT INTO sys_dict_data (dict_data_id, dict_sort, dict_label, dict_value, dict_type, css_class, list_class, is_default, status, parent_id, create_by, create_time, update_by, update_time, remark, is_del) VALUES (13, 2, '否', 'N', 'sys_yes_no', '', 'danger', 'N', '0', 0, 'admin', '2022-04-15 15:51:02', '', NULL, '系统默认否', 0);
INSERT INTO sys_dict_data (dict_data_id, dict_sort, dict_label, dict_value, dict_type, css_class, list_class, is_default, status, parent_id, create_by, create_time, update_by, update_time, remark, is_del) VALUES (14, 1, '正常', 'normal', 'sys_priority_type', '', 'primary', 'Y', '0', 0, 'admin', '2022-04-15 15:51:02', 'admin', '2022-07-06 01:08:04', '正常', 0);
INSERT INTO sys_dict_data (dict_data_id, dict_sort, dict_label, dict_value, dict_type, css_class, list_class, is_default, status, parent_id, create_by, create_time, update_by, update_time, remark, is_del) VALUES (15, 2, '加急', 'urgent', 'sys_priority_type', '', 'danger', 'N', '0', 0, 'admin', '2022-04-15 15:51:02', 'admin', '2022-07-06 01:08:11', '加急', 0);
INSERT INTO sys_dict_data (dict_data_id, dict_sort, dict_label, dict_value, dict_type, css_class, list_class, is_default, status, parent_id, create_by, create_time, update_by, update_time, remark, is_del) VALUES (18, 1, '新增', '1', 'sys_oper_type', '', 'info', 'N', '0', 0, 'admin', '2022-04-15 15:51:02', '', NULL, '新增操作', 0);
INSERT INTO sys_dict_data (dict_data_id, dict_sort, dict_label, dict_value, dict_type, css_class, list_class, is_default, status, parent_id, create_by, create_time, update_by, update_time, remark, is_del) VALUES (19, 2, '修改', '2', 'sys_oper_type', '', 'info', 'N', '0', 0, 'admin', '2022-04-15 15:51:02', '', NULL, '修改操作', 0);
INSERT INTO sys_dict_data (dict_data_id, dict_sort, dict_label, dict_value, dict_type, css_class, list_class, is_default, status, parent_id, create_by, create_time, update_by, update_time, remark, is_del) VALUES (20, 3, '删除', '3', 'sys_oper_type', '', 'danger', 'N', '0', 0, 'admin', '2022-04-15 15:51:02', '', NULL, '删除操作', 0);
INSERT INTO sys_dict_data (dict_data_id, dict_sort, dict_label, dict_value, dict_type, css_class, list_class, is_default, status, parent_id, create_by, create_time, update_by, update_time, remark, is_del) VALUES (21, 4, '授权', '4', 'sys_oper_type', '', 'primary', 'N', '0', 0, 'admin', '2022-04-15 15:51:02', '', NULL, '授权操作', 0);
INSERT INTO sys_dict_data (dict_data_id, dict_sort, dict_label, dict_value, dict_type, css_class, list_class, is_default, status, parent_id, create_by, create_time, update_by, update_time, remark, is_del) VALUES (22, 5, '导出', '5', 'sys_oper_type', '', 'warning', 'N', '0', 0, 'admin', '2022-04-15 15:51:02', '', NULL, '导出操作', 0);
INSERT INTO sys_dict_data (dict_data_id, dict_sort, dict_label, dict_value, dict_type, css_class, list_class, is_default, status, parent_id, create_by, create_time, update_by, update_time, remark, is_del) VALUES (23, 6, '导入', '6', 'sys_oper_type', '', 'warning', 'N', '0', 0, 'admin', '2022-04-15 15:51:02', '', NULL, '导入操作', 0);
INSERT INTO sys_dict_data (dict_data_id, dict_sort, dict_label, dict_value, dict_type, css_class, list_class, is_default, status, parent_id, create_by, create_time, update_by, update_time, remark, is_del) VALUES (24, 7, '强退', '7', 'sys_oper_type', '', 'danger', 'N', '0', 0, 'admin', '2022-04-15 15:51:02', '', NULL, '强退操作', 0);
INSERT INTO sys_dict_data (dict_data_id, dict_sort, dict_label, dict_value, dict_type, css_class, list_class, is_default, status, parent_id, create_by, create_time, update_by, update_time, remark, is_del) VALUES (25, 8, '生成代码', '8', 'sys_oper_type', '', 'warning', 'N', '0', 0, 'admin', '2022-04-15 15:51:02', '', NULL, '生成操作', 0);
INSERT INTO sys_dict_data (dict_data_id, dict_sort, dict_label, dict_value, dict_type, css_class, list_class, is_default, status, parent_id, create_by, create_time, update_by, update_time, remark, is_del) VALUES (26, 9, '清空数据', '9', 'sys_oper_type', '', 'danger', 'N', '0', 0, 'admin', '2022-04-15 15:51:02', '', NULL, '清空操作', 0);
INSERT INTO sys_dict_data (dict_data_id, dict_sort, dict_label, dict_value, dict_type, css_class, list_class, is_default, status, parent_id, create_by, create_time, update_by, update_time, remark, is_del) VALUES (27, 1, '成功', '0', 'sys_common_status', '', 'primary', 'N', '0', 0, 'admin', '2022-04-15 15:51:02', '', NULL, '正常状态', 0);
INSERT INTO sys_dict_data (dict_data_id, dict_sort, dict_label, dict_value, dict_type, css_class, list_class, is_default, status, parent_id, create_by, create_time, update_by, update_time, remark, is_del) VALUES (28, 2, '失败', '1', 'sys_common_status', '', 'danger', 'N', '0', 0, 'admin', '2022-04-15 15:51:02', '', NULL, '停用状态', 0);
INSERT INTO sys_dict_data (dict_data_id, dict_sort, dict_label, dict_value, dict_type, css_class, list_class, is_default, status, parent_id, create_by, create_time, update_by, update_time, remark, is_del) VALUES (29, 12, '1', '2', 'sys_user_sex', '', 'warning', 'N', '0', 0, NULL, '2022-05-11 21:44:01', 'admin', NULL, NULL, 1);
INSERT INTO sys_dict_data (dict_data_id, dict_sort, dict_label, dict_value, dict_type, css_class, list_class, is_default, status, parent_id, create_by, create_time, update_by, update_time, remark, is_del) VALUES (30, 0, '2', '3', 'sys_user_sex', '4', 'default', 'N', '0', 0, NULL, '2022-05-11 21:46:45', 'admin', NULL, NULL, 1);
INSERT INTO sys_dict_data (dict_data_id, dict_sort, dict_label, dict_value, dict_type, css_class, list_class, is_default, status, parent_id, create_by, create_time, update_by, update_time, remark, is_del) VALUES (31, 1, '压接工位', 'yjgw', 'mes_terminal_type', NULL, 'default', 'N', '0', 0, 'admin', '2022-07-01 00:44:15', 'admin', '2022-07-01 00:47:48', NULL, 0);
INSERT INTO sys_dict_data (dict_data_id, dict_sort, dict_label, dict_value, dict_type, css_class, list_class, is_default, status, parent_id, create_by, create_time, update_by, update_time, remark, is_del) VALUES (32, 0, '压接工序', 'yggx', 'mes_process_type', NULL, 'default', 'N', '0', 0, 'admin', '2022-07-03 00:38:02', NULL, NULL, NULL, 0);
INSERT INTO sys_dict_data (dict_data_id, dict_sort, dict_label, dict_value, dict_type, css_class, list_class, is_default, status, parent_id, create_by, create_time, update_by, update_time, remark, is_del) VALUES (33, 0, '压接工艺', 'yjgy', 'mes_route_type', NULL, 'default', 'N', '0', 0, 'admin', '2022-07-03 13:23:11', NULL, NULL, NULL, 0);
INSERT INTO sys_dict_data (dict_data_id, dict_sort, dict_label, dict_value, dict_type, css_class, list_class, is_default, status, parent_id, create_by, create_time, update_by, update_time, remark, is_del) VALUES (35, 0, '供应商', 'supplier', 'pub_company_type', NULL, 'default', 'Y', '0', 0, 'admin', '2022-07-06 22:11:59', 'admin', '2022-07-06 22:42:39', NULL, 0);
INSERT INTO sys_dict_data (dict_data_id, dict_sort, dict_label, dict_value, dict_type, css_class, list_class, is_default, status, parent_id, create_by, create_time, update_by, update_time, remark, is_del) VALUES (36, 1, '客户', 'customer', 'pub_company_type', NULL, 'default', 'N', '0', 0, 'admin', '2022-07-06 22:12:10', 'admin', '2022-07-06 22:42:41', NULL, 0);
INSERT INTO sys_dict_data (dict_data_id, dict_sort, dict_label, dict_value, dict_type, css_class, list_class, is_default, status, parent_id, create_by, create_time, update_by, update_time, remark, is_del) VALUES (37, 0, '成品', 'FG', 'mes_material_type', NULL, 'default', 'N', '0', 0, 'admin', '2022-07-06 23:55:57', NULL, NULL, NULL, 0);
INSERT INTO sys_dict_data (dict_data_id, dict_sort, dict_label, dict_value, dict_type, css_class, list_class, is_default, status, parent_id, create_by, create_time, update_by, update_time, remark, is_del) VALUES (38, 1, '半成品', 'WIP', 'mes_material_type', NULL, 'default', 'N', '0', 0, 'admin', '2022-07-06 23:56:18', NULL, NULL, NULL, 0);
INSERT INTO sys_dict_data (dict_data_id, dict_sort, dict_label, dict_value, dict_type, css_class, list_class, is_default, status, parent_id, create_by, create_time, update_by, update_time, remark, is_del) VALUES (39, 2, '原材料', 'RM', 'mes_material_type', NULL, 'default', 'N', '0', 0, 'admin', '2022-07-06 23:56:43', 'admin', '2022-07-06 23:56:50', NULL, 0);
INSERT INTO sys_dict_data (dict_data_id, dict_sort, dict_label, dict_value, dict_type, css_class, list_class, is_default, status, parent_id, create_by, create_time, update_by, update_time, remark, is_del) VALUES (40, 1, '标准', 'standard', 'mes_shoporder_type', NULL, 'default', 'N', '0', 0, 'admin', '2022-07-14 22:24:19', NULL, NULL, NULL, 0);
INSERT INTO sys_dict_data (dict_data_id, dict_sort, dict_label, dict_value, dict_type, css_class, list_class, is_default, status, parent_id, create_by, create_time, update_by, update_time, remark, is_del) VALUES (41, 2, '返工', 'rework', 'mes_shoporder_type', NULL, 'default', 'N', '0', 0, 'admin', '2022-07-14 22:24:49', 'admin', '2022-07-14 22:25:19', NULL, 0);
INSERT INTO sys_dict_data (dict_data_id, dict_sort, dict_label, dict_value, dict_type, css_class, list_class, is_default, status, parent_id, create_by, create_time, update_by, update_time, remark, is_del) VALUES (42, 3, '委外', 'outsourcing', 'mes_shoporder_type', NULL, 'default', 'N', '0', 0, 'admin', '2022-07-14 22:25:10', 'admin', '2022-07-14 22:25:16', NULL, 0);
INSERT INTO sys_dict_data (dict_data_id, dict_sort, dict_label, dict_value, dict_type, css_class, list_class, is_default, status, parent_id, create_by, create_time, update_by, update_time, remark, is_del) VALUES (43, 4, 'RMA', 'RMA', 'mes_shoporder_type', NULL, 'default', 'N', '0', 0, 'admin', '2022-07-14 22:25:29', NULL, NULL, NULL, 0);
INSERT INTO sys_dict_data (dict_data_id, dict_sort, dict_label, dict_value, dict_type, css_class, list_class, is_default, status, parent_id, create_by, create_time, update_by, update_time, remark, is_del) VALUES (44, 0, '新建', 'new', 'mes_shoporder_status', NULL, 'default', 'N', '0', 0, 'admin', '2022-07-14 22:27:36', NULL, NULL, NULL, 0);
INSERT INTO sys_dict_data (dict_data_id, dict_sort, dict_label, dict_value, dict_type, css_class, list_class, is_default, status, parent_id, create_by, create_time, update_by, update_time, remark, is_del) VALUES (45, 0, '等级一', '1', 'mes_quality_bad_grade', NULL, 'default', 'N', '0', 0, 'admin', '2022-07-18 23:36:26', NULL, NULL, NULL, 0);
INSERT INTO sys_dict_data (dict_data_id, dict_sort, dict_label, dict_value, dict_type, css_class, list_class, is_default, status, parent_id, create_by, create_time, update_by, update_time, remark, is_del) VALUES (46, 1, '等级二', '2', 'mes_quality_bad_grade', NULL, 'default', 'N', '0', 0, 'admin', '2022-07-18 23:36:35', NULL, NULL, NULL, 0);
INSERT INTO sys_dict_data (dict_data_id, dict_sort, dict_label, dict_value, dict_type, css_class, list_class, is_default, status, parent_id, create_by, create_time, update_by, update_time, remark, is_del) VALUES (47, 0, '残次品', 'ccp', 'mes_quality_code_type', NULL, 'default', 'N', '0', 0, 'admin', '2022-07-18 23:36:53', NULL, NULL, NULL, 0);
INSERT INTO sys_dict_data (dict_data_id, dict_sort, dict_label, dict_value, dict_type, css_class, list_class, is_default, status, parent_id, create_by, create_time, update_by, update_time, remark, is_del) VALUES (48, 0, '待检区', 'wait_check', 'wms_area_type', NULL, 'default', 'N', '0', 0, 'admin', '2022-08-14 23:54:24', 'admin', '2022-08-15 00:14:09', NULL, 0);
INSERT INTO sys_dict_data (dict_data_id, dict_sort, dict_label, dict_value, dict_type, css_class, list_class, is_default, status, parent_id, create_by, create_time, update_by, update_time, remark, is_del) VALUES (49, 1, '已检区', 'is_check', 'wms_area_type', NULL, 'default', 'N', '0', 0, 'admin', '2022-08-14 23:54:49', NULL, NULL, NULL, 0);
INSERT INTO sys_dict_data (dict_data_id, dict_sort, dict_label, dict_value, dict_type, css_class, list_class, is_default, status, parent_id, create_by, create_time, update_by, update_time, remark, is_del) VALUES (50, 2, '仓储区', 'storage', 'wms_area_type', NULL, 'default', 'Y', '0', 0, 'admin', '2022-08-14 23:55:31', 'admin', '2022-08-14 23:55:39', NULL, 0);
INSERT INTO sys_dict_data (dict_data_id, dict_sort, dict_label, dict_value, dict_type, css_class, list_class, is_default, status, parent_id, create_by, create_time, update_by, update_time, remark, is_del) VALUES (51, 3, '暂存区', 'temp_storage', 'wms_area_type', NULL, 'default', 'N', '0', 0, 'admin', '2022-08-14 23:56:22', NULL, NULL, NULL, 0);
INSERT INTO sys_dict_data (dict_data_id, dict_sort, dict_label, dict_value, dict_type, css_class, list_class, is_default, status, parent_id, create_by, create_time, update_by, update_time, remark, is_del) VALUES (52, 4, '待发货区', 'wait_delivery', 'wms_area_type', NULL, 'default', 'N', '0', 0, 'admin', '2022-08-14 23:57:05', NULL, NULL, NULL, 0);
INSERT INTO sys_dict_data (dict_data_id, dict_sort, dict_label, dict_value, dict_type, css_class, list_class, is_default, status, parent_id, create_by, create_time, update_by, update_time, remark, is_del) VALUES (53, 5, '多发库位', 'multiple_delivery', 'wms_area_type', NULL, 'default', 'N', '0', 0, 'admin', '2022-08-14 23:59:07', 'admin', '2022-08-14 23:59:13', NULL, 0);
INSERT INTO sys_dict_data (dict_data_id, dict_sort, dict_label, dict_value, dict_type, css_class, list_class, is_default, status, parent_id, create_by, create_time, update_by, update_time, remark, is_del) VALUES (56, 0, '收货中', 'receipt', 'wms_goods_status', NULL, 'warning', 'N', '0', 0, 'admin', '2022-09-28 00:55:53', 'admin', '2022-10-06 00:39:17', NULL, 0);
INSERT INTO sys_dict_data (dict_data_id, dict_sort, dict_label, dict_value, dict_type, css_class, list_class, is_default, status, parent_id, create_by, create_time, update_by, update_time, remark, is_del) VALUES (57, 1, '已入库', 'stock', 'wms_goods_status', NULL, 'primary', 'N', '0', 0, 'admin', '2022-09-28 00:56:15', 'admin', '2022-10-06 00:39:26', NULL, 0);
INSERT INTO sys_dict_data (dict_data_id, dict_sort, dict_label, dict_value, dict_type, css_class, list_class, is_default, status, parent_id, create_by, create_time, update_by, update_time, remark, is_del) VALUES (58, 2, '退货', 'back', 'wms_goods_status', NULL, 'danger', 'N', '0', 0, 'admin', '2022-09-28 00:56:42', NULL, NULL, NULL, 0);
INSERT INTO sys_dict_data (dict_data_id, dict_sort, dict_label, dict_value, dict_type, css_class, list_class, is_default, status, parent_id, create_by, create_time, update_by, update_time, remark, is_del) VALUES (59, 3, '已出库', 'outbound', 'wms_goods_status', NULL, 'info', 'N', '0', 0, 'admin', '2022-09-28 00:57:06', 'admin', '2022-10-06 00:39:37', NULL, 0);
INSERT INTO sys_dict_data (dict_data_id, dict_sort, dict_label, dict_value, dict_type, css_class, list_class, is_default, status, parent_id, create_by, create_time, update_by, update_time, remark, is_del) VALUES (60, 0, '毫米', '毫米', 'wms_sys_unit_type', NULL, 'default', 'N', '0', 0, 'admin', '2022-09-30 23:36:26', NULL, NULL, NULL, 0);
INSERT INTO sys_dict_data (dict_data_id, dict_sort, dict_label, dict_value, dict_type, css_class, list_class, is_default, status, parent_id, create_by, create_time, update_by, update_time, remark, is_del) VALUES (61, 2, '厘米', '厘米', 'wms_sys_unit_type', NULL, 'default', 'N', '0', 0, 'admin', '2022-09-30 23:36:26', NULL, NULL, NULL, 0);
INSERT INTO sys_dict_data (dict_data_id, dict_sort, dict_label, dict_value, dict_type, css_class, list_class, is_default, status, parent_id, create_by, create_time, update_by, update_time, remark, is_del) VALUES (62, 4, '米', '米', 'wms_sys_unit_type', NULL, 'default', 'N', '0', 0, 'admin', '2022-09-30 23:36:26', NULL, NULL, NULL, 0);
INSERT INTO sys_dict_data (dict_data_id, dict_sort, dict_label, dict_value, dict_type, css_class, list_class, is_default, status, parent_id, create_by, create_time, update_by, update_time, remark, is_del) VALUES (63, 6, '千米', '千米', 'wms_sys_unit_type', NULL, 'default', 'N', '0', 0, 'admin', '2022-09-30 23:36:26', NULL, NULL, NULL, 0);
INSERT INTO sys_dict_data (dict_data_id, dict_sort, dict_label, dict_value, dict_type, css_class, list_class, is_default, status, parent_id, create_by, create_time, update_by, update_time, remark, is_del) VALUES (64, 8, '毫克', '毫克', 'wms_sys_unit_type', NULL, 'default', 'N', '0', 0, 'admin', '2022-09-30 23:36:26', NULL, NULL, NULL, 0);
INSERT INTO sys_dict_data (dict_data_id, dict_sort, dict_label, dict_value, dict_type, css_class, list_class, is_default, status, parent_id, create_by, create_time, update_by, update_time, remark, is_del) VALUES (65, 10, '克', '克', 'wms_sys_unit_type', NULL, 'default', 'N', '0', 0, 'admin', '2022-09-30 23:36:26', NULL, NULL, NULL, 0);
INSERT INTO sys_dict_data (dict_data_id, dict_sort, dict_label, dict_value, dict_type, css_class, list_class, is_default, status, parent_id, create_by, create_time, update_by, update_time, remark, is_del) VALUES (66, 12, '千克', '千克', 'wms_sys_unit_type', NULL, 'default', 'N', '0', 0, 'admin', '2022-09-30 23:36:26', NULL, NULL, NULL, 0);
INSERT INTO sys_dict_data (dict_data_id, dict_sort, dict_label, dict_value, dict_type, css_class, list_class, is_default, status, parent_id, create_by, create_time, update_by, update_time, remark, is_del) VALUES (67, 14, '吨', '吨', 'wms_sys_unit_type', NULL, 'default', 'N', '0', 0, 'admin', '2022-09-30 23:36:26', NULL, NULL, NULL, 0);
INSERT INTO sys_dict_data (dict_data_id, dict_sort, dict_label, dict_value, dict_type, css_class, list_class, is_default, status, parent_id, create_by, create_time, update_by, update_time, remark, is_del) VALUES (68, 16, '毫升', '毫升', 'wms_sys_unit_type', NULL, 'default', 'N', '0', 0, 'admin', '2022-09-30 23:36:26', NULL, NULL, NULL, 0);
INSERT INTO sys_dict_data (dict_data_id, dict_sort, dict_label, dict_value, dict_type, css_class, list_class, is_default, status, parent_id, create_by, create_time, update_by, update_time, remark, is_del) VALUES (69, 18, '升', '升', 'wms_sys_unit_type', NULL, 'default', 'N', '0', 0, 'admin', '2022-09-30 23:36:26', NULL, NULL, NULL, 0);
INSERT INTO sys_dict_data (dict_data_id, dict_sort, dict_label, dict_value, dict_type, css_class, list_class, is_default, status, parent_id, create_by, create_time, update_by, update_time, remark, is_del) VALUES (70, 20, 'cm³', 'cm³', 'wms_sys_unit_type', NULL, 'default', 'N', '0', 0, 'admin', '2022-09-30 23:36:26', NULL, NULL, NULL, 0);
INSERT INTO sys_dict_data (dict_data_id, dict_sort, dict_label, dict_value, dict_type, css_class, list_class, is_default, status, parent_id, create_by, create_time, update_by, update_time, remark, is_del) VALUES (71, 22, 'm³', 'm³', 'wms_sys_unit_type', NULL, 'default', 'N', '0', 0, 'admin', '2022-09-30 23:36:26', NULL, NULL, NULL, 0);
INSERT INTO sys_dict_data (dict_data_id, dict_sort, dict_label, dict_value, dict_type, css_class, list_class, is_default, status, parent_id, create_by, create_time, update_by, update_time, remark, is_del) VALUES (72, 24, 'cm²', 'cm²', 'wms_sys_unit_type', NULL, 'default', 'N', '0', 0, 'admin', '2022-09-30 23:36:26', NULL, NULL, NULL, 0);
INSERT INTO sys_dict_data (dict_data_id, dict_sort, dict_label, dict_value, dict_type, css_class, list_class, is_default, status, parent_id, create_by, create_time, update_by, update_time, remark, is_del) VALUES (73, 26, 'm²', 'm²', 'wms_sys_unit_type', NULL, 'default', 'N', '0', 0, 'admin', '2022-09-30 23:36:26', NULL, NULL, NULL, 0);
INSERT INTO sys_dict_data (dict_data_id, dict_sort, dict_label, dict_value, dict_type, css_class, list_class, is_default, status, parent_id, create_by, create_time, update_by, update_time, remark, is_del) VALUES (74, 28, '两', '两', 'wms_sys_unit_type', NULL, 'default', 'N', '0', 0, 'admin', '2022-09-30 23:36:26', NULL, NULL, NULL, 0);
INSERT INTO sys_dict_data (dict_data_id, dict_sort, dict_label, dict_value, dict_type, css_class, list_class, is_default, status, parent_id, create_by, create_time, update_by, update_time, remark, is_del) VALUES (75, 30, '斤', '斤', 'wms_sys_unit_type', NULL, 'default', 'N', '0', 0, 'admin', '2022-09-30 23:36:26', NULL, NULL, NULL, 0);
INSERT INTO sys_dict_data (dict_data_id, dict_sort, dict_label, dict_value, dict_type, css_class, list_class, is_default, status, parent_id, create_by, create_time, update_by, update_time, remark, is_del) VALUES (76, 32, '尺', '尺', 'wms_sys_unit_type', NULL, 'default', 'N', '0', 0, 'admin', '2022-09-30 23:36:26', NULL, NULL, NULL, 0);
INSERT INTO sys_dict_data (dict_data_id, dict_sort, dict_label, dict_value, dict_type, css_class, list_class, is_default, status, parent_id, create_by, create_time, update_by, update_time, remark, is_del) VALUES (77, 34, '亩', '亩', 'wms_sys_unit_type', NULL, 'default', 'N', '0', 0, 'admin', '2022-09-30 23:36:26', NULL, NULL, NULL, 0);
INSERT INTO sys_dict_data (dict_data_id, dict_sort, dict_label, dict_value, dict_type, css_class, list_class, is_default, status, parent_id, create_by, create_time, update_by, update_time, remark, is_del) VALUES (78, 36, '加仑', '加仑', 'wms_sys_unit_type', NULL, 'default', 'N', '0', 0, 'admin', '2022-09-30 23:36:26', NULL, NULL, NULL, 0);
INSERT INTO sys_dict_data (dict_data_id, dict_sort, dict_label, dict_value, dict_type, css_class, list_class, is_default, status, parent_id, create_by, create_time, update_by, update_time, remark, is_del) VALUES (79, 38, '磅', '磅', 'wms_sys_unit_type', NULL, 'default', 'N', '0', 0, 'admin', '2022-09-30 23:36:26', NULL, NULL, NULL, 0);
INSERT INTO sys_dict_data (dict_data_id, dict_sort, dict_label, dict_value, dict_type, css_class, list_class, is_default, status, parent_id, create_by, create_time, update_by, update_time, remark, is_del) VALUES (80, 40, '箱', '箱', 'wms_sys_unit_type', NULL, 'default', 'N', '0', 0, 'admin', '2022-09-30 23:36:26', NULL, NULL, NULL, 0);
INSERT INTO sys_dict_data (dict_data_id, dict_sort, dict_label, dict_value, dict_type, css_class, list_class, is_default, status, parent_id, create_by, create_time, update_by, update_time, remark, is_del) VALUES (81, 42, '个', '个', 'wms_sys_unit_type', NULL, 'default', 'N', '0', 0, 'admin', '2022-09-30 23:36:26', NULL, NULL, NULL, 0);
INSERT INTO sys_dict_data (dict_data_id, dict_sort, dict_label, dict_value, dict_type, css_class, list_class, is_default, status, parent_id, create_by, create_time, update_by, update_time, remark, is_del) VALUES (82, 44, '条', '条', 'wms_sys_unit_type', NULL, 'default', 'N', '0', 0, 'admin', '2022-09-30 23:36:26', NULL, NULL, NULL, 0);
INSERT INTO sys_dict_data (dict_data_id, dict_sort, dict_label, dict_value, dict_type, css_class, list_class, is_default, status, parent_id, create_by, create_time, update_by, update_time, remark, is_del) VALUES (83, 46, '包', '包', 'wms_sys_unit_type', NULL, 'default', 'N', '0', 0, 'admin', '2022-09-30 23:36:26', NULL, NULL, NULL, 0);
INSERT INTO sys_dict_data (dict_data_id, dict_sort, dict_label, dict_value, dict_type, css_class, list_class, is_default, status, parent_id, create_by, create_time, update_by, update_time, remark, is_del) VALUES (84, 48, '件', '件', 'wms_sys_unit_type', NULL, 'default', 'N', '0', 0, 'admin', '2022-09-30 23:36:26', NULL, NULL, NULL, 0);
INSERT INTO sys_dict_data (dict_data_id, dict_sort, dict_label, dict_value, dict_type, css_class, list_class, is_default, status, parent_id, create_by, create_time, update_by, update_time, remark, is_del) VALUES (85, 50, '桶', '桶', 'wms_sys_unit_type', NULL, 'default', 'N', '0', 0, 'admin', '2022-09-30 23:36:26', NULL, NULL, NULL, 0);
INSERT INTO sys_dict_data (dict_data_id, dict_sort, dict_label, dict_value, dict_type, css_class, list_class, is_default, status, parent_id, create_by, create_time, update_by, update_time, remark, is_del) VALUES (86, 52, '瓶', '瓶', 'wms_sys_unit_type', NULL, 'default', 'N', '0', 0, 'admin', '2022-09-30 23:36:26', NULL, NULL, NULL, 0);
INSERT INTO sys_dict_data (dict_data_id, dict_sort, dict_label, dict_value, dict_type, css_class, list_class, is_default, status, parent_id, create_by, create_time, update_by, update_time, remark, is_del) VALUES (87, 54, '本', '本', 'wms_sys_unit_type', NULL, 'default', 'N', '0', 0, 'admin', '2022-09-30 23:36:26', NULL, NULL, NULL, 0);
INSERT INTO sys_dict_data (dict_data_id, dict_sort, dict_label, dict_value, dict_type, css_class, list_class, is_default, status, parent_id, create_by, create_time, update_by, update_time, remark, is_del) VALUES (88, 56, '丝', '丝', 'wms_sys_unit_type', NULL, 'default', 'N', '0', 0, 'admin', '2022-09-30 23:36:26', NULL, NULL, NULL, 0);
INSERT INTO sys_dict_data (dict_data_id, dict_sort, dict_label, dict_value, dict_type, css_class, list_class, is_default, status, parent_id, create_by, create_time, update_by, update_time, remark, is_del) VALUES (89, 58, '套', '套', 'wms_sys_unit_type', NULL, 'default', 'N', '0', 0, 'admin', '2022-09-30 23:36:26', NULL, NULL, NULL, 0);
INSERT INTO sys_dict_data (dict_data_id, dict_sort, dict_label, dict_value, dict_type, css_class, list_class, is_default, status, parent_id, create_by, create_time, update_by, update_time, remark, is_del) VALUES (90, 60, '张', '张', 'wms_sys_unit_type', NULL, 'default', 'Y', '0', 0, 'admin', '2022-09-30 23:36:26', NULL, NULL, NULL, 0);
INSERT INTO sys_dict_data (dict_data_id, dict_sort, dict_label, dict_value, dict_type, css_class, list_class, is_default, status, parent_id, create_by, create_time, update_by, update_time, remark, is_del) VALUES (91, 0, '手动', 'new', 'wms_delivery_source', NULL, 'default', 'N', '0', 0, 'admin', '2022-10-04 14:52:33', NULL, NULL, NULL, 0);
INSERT INTO sys_dict_data (dict_data_id, dict_sort, dict_label, dict_value, dict_type, css_class, list_class, is_default, status, parent_id, create_by, create_time, update_by, update_time, remark, is_del) VALUES (93, 0, '一等', '一等', 'wms_goods_quality', NULL, 'default', 'N', '0', 0, 'admin', '2022-10-23 23:04:38', NULL, NULL, NULL, 0);
INSERT INTO sys_dict_data (dict_data_id, dict_sort, dict_label, dict_value, dict_type, css_class, list_class, is_default, status, parent_id, create_by, create_time, update_by, update_time, remark, is_del) VALUES (94, 0, '优', '优', 'wms_goods_quality', NULL, 'default', 'N', '0', 0, 'admin', '2022-10-23 23:04:47', NULL, NULL, NULL, 0);
INSERT INTO sys_dict_data (dict_data_id, dict_sort, dict_label, dict_value, dict_type, css_class, list_class, is_default, status, parent_id, create_by, create_time, update_by, update_time, remark, is_del) VALUES (95, 0, 'AS', 'AS', 'wms_goods_type', NULL, 'default', 'N', '0', 0, 'admin', '2022-11-03 23:21:34', NULL, NULL, NULL, 0);
INSERT INTO sys_dict_data (dict_data_id, dict_sort, dict_label, dict_value, dict_type, css_class, list_class, is_default, status, parent_id, create_by, create_time, update_by, update_time, remark, is_del) VALUES (96, 0, 'AS+椴', 'AS+椴', 'wms_goods_type', NULL, 'default', 'N', '0', 0, 'admin', '2022-11-03 23:21:37', NULL, NULL, NULL, 0);
INSERT INTO sys_dict_data (dict_data_id, dict_sort, dict_label, dict_value, dict_type, css_class, list_class, is_default, status, parent_id, create_by, create_time, update_by, update_time, remark, is_del) VALUES (97, 0, '椴木', '椴木', 'wms_goods_type', NULL, 'default', 'N', '0', 0, 'admin', '2022-11-03 23:21:42', NULL, NULL, NULL, 0);
INSERT INTO sys_dict_data (dict_data_id, dict_sort, dict_label, dict_value, dict_type, css_class, list_class, is_default, status, parent_id, create_by, create_time, update_by, update_time, remark, is_del) VALUES (98, 0, '杨木', '杨木', 'wms_goods_type', NULL, 'default', 'N', '0', 0, 'admin', '2022-11-03 23:21:49', NULL, NULL, NULL, 0);
INSERT INTO sys_dict_data (dict_data_id, dict_sort, dict_label, dict_value, dict_type, css_class, list_class, is_default, status, parent_id, create_by, create_time, update_by, update_time, remark, is_del) VALUES (99, 0, 'AS+杨', 'AS+杨', 'wms_goods_type', NULL, 'default', 'N', '0', 0, 'admin', '2022-11-03 23:22:11', NULL, NULL, NULL, 0);
INSERT INTO sys_dict_data (dict_data_id, dict_sort, dict_label, dict_value, dict_type, css_class, list_class, is_default, status, parent_id, create_by, create_time, update_by, update_time, remark, is_del) VALUES (100, 0, '椴+杨', '椴+杨', 'wms_goods_type', NULL, 'default', 'N', '0', 0, 'admin', '2022-11-03 23:22:28', NULL, NULL, NULL, 0);
INSERT INTO sys_dict_data (dict_data_id, dict_sort, dict_label, dict_value, dict_type, css_class, list_class, is_default, status, parent_id, create_by, create_time, update_by, update_time, remark, is_del) VALUES (101, 0, '天然红橡', '天然红橡', 'wms_goods_type', NULL, 'default', 'N', '0', 0, 'admin', '2022-11-03 23:35:52', NULL, NULL, NULL, 0);
INSERT INTO sys_dict_data (dict_data_id, dict_sort, dict_label, dict_value, dict_type, css_class, list_class, is_default, status, parent_id, create_by, create_time, update_by, update_time, remark, is_del) VALUES (102, 0, '意杨', '意杨', 'wms_goods_type', NULL, 'default', 'N', '0', 0, 'admin', '2022-11-03 23:35:58', NULL, NULL, NULL, 0);
INSERT INTO sys_dict_data (dict_data_id, dict_sort, dict_label, dict_value, dict_type, css_class, list_class, is_default, status, parent_id, create_by, create_time, update_by, update_time, remark, is_del) VALUES (103, 0, '', '', 'wms_goods_type', NULL, 'default', 'N', '0', 0, 'admin', '2022-11-11 21:44:21', NULL, NULL, NULL, 1);
INSERT INTO sys_dict_data (dict_data_id, dict_sort, dict_label, dict_value, dict_type, css_class, list_class, is_default, status, parent_id, create_by, create_time, update_by, update_time, remark, is_del) VALUES (104, 0, '一道', '一道', 'wms_goods_process', NULL, 'default', 'N', '0', 0, 'admin', '2022-11-12 16:51:09', 'admin', '2024-11-01 23:22:35', NULL, 0);
INSERT INTO sys_dict_data (dict_data_id, dict_sort, dict_label, dict_value, dict_type, css_class, list_class, is_default, status, parent_id, create_by, create_time, update_by, update_time, remark, is_del) VALUES (105, 0, '一道半', '一道半', 'wms_goods_process', NULL, 'default', 'N', '0', 0, 'admin', '2022-11-12 16:51:31', NULL, NULL, NULL, 0);
INSERT INTO sys_dict_data (dict_data_id, dict_sort, dict_label, dict_value, dict_type, css_class, list_class, is_default, status, parent_id, create_by, create_time, update_by, update_time, remark, is_del) VALUES (106, 0, '二道', '二道', 'wms_goods_process', NULL, 'default', 'N', '0', 0, 'admin', '2022-11-12 16:52:13', NULL, NULL, NULL, 0);
INSERT INTO sys_dict_data (dict_data_id, dict_sort, dict_label, dict_value, dict_type, css_class, list_class, is_default, status, parent_id, create_by, create_time, update_by, update_time, remark, is_del) VALUES (107, 0, '二道半', '二道半', 'wms_goods_process', NULL, 'default', 'N', '0', 0, 'admin', '2022-11-12 16:52:28', NULL, NULL, NULL, 0);
INSERT INTO sys_dict_data (dict_data_id, dict_sort, dict_label, dict_value, dict_type, css_class, list_class, is_default, status, parent_id, create_by, create_time, update_by, update_time, remark, is_del) VALUES (108, 0, '13丝', '13丝', 'wms_goods_standard', NULL, 'default', 'N', '0', 0, 'admin', '2022-11-12 16:52:28', NULL, NULL, NULL, 0);
INSERT INTO sys_dict_data (dict_data_id, dict_sort, dict_label, dict_value, dict_type, css_class, list_class, is_default, status, parent_id, create_by, create_time, update_by, update_time, remark, is_del) VALUES (109, 0, '14丝', '14丝', 'wms_goods_standard', NULL, 'default', 'N', '0', 0, 'admin', '2022-11-12 16:52:28', NULL, NULL, NULL, 0);
INSERT INTO sys_dict_data (dict_data_id, dict_sort, dict_label, dict_value, dict_type, css_class, list_class, is_default, status, parent_id, create_by, create_time, update_by, update_time, remark, is_del) VALUES (110, 0, '15丝', '15丝', 'wms_goods_standard', NULL, 'default', 'N', '0', 0, 'admin', '2022-11-12 16:52:28', NULL, NULL, NULL, 0);
INSERT INTO sys_dict_data (dict_data_id, dict_sort, dict_label, dict_value, dict_type, css_class, list_class, is_default, status, parent_id, create_by, create_time, update_by, update_time, remark, is_del) VALUES (111, 0, '75丝', '75丝', 'wms_goods_standard', NULL, 'default', 'N', '0', 0, 'admin', '2022-11-12 16:52:28', 'admin', '2022-11-14 16:29:32', NULL, 0);
INSERT INTO sys_dict_data (dict_data_id, dict_sort, dict_label, dict_value, dict_type, css_class, list_class, is_default, status, parent_id, create_by, create_time, update_by, update_time, remark, is_del) VALUES (112, 0, '260丝', '260丝', 'wms_goods_standard', NULL, 'default', 'N', '0', 0, 'admin', '2022-11-12 16:52:28', 'admin', '2022-11-17 17:08:57', NULL, 0);
INSERT INTO sys_dict_data (dict_data_id, dict_sort, dict_label, dict_value, dict_type, css_class, list_class, is_default, status, parent_id, create_by, create_time, update_by, update_time, remark, is_del) VALUES (113, 0, '18丝', '18丝', 'wms_goods_standard', NULL, 'default', 'N', '0', 0, 'admin', '2022-11-12 16:52:28', NULL, NULL, NULL, 0);
INSERT INTO sys_dict_data (dict_data_id, dict_sort, dict_label, dict_value, dict_type, css_class, list_class, is_default, status, parent_id, create_by, create_time, update_by, update_time, remark, is_del) VALUES (114, 0, '19丝', '19丝', 'wms_goods_standard', NULL, 'default', 'N', '0', 0, 'admin', '2022-11-12 16:52:28', NULL, NULL, NULL, 0);
INSERT INTO sys_dict_data (dict_data_id, dict_sort, dict_label, dict_value, dict_type, css_class, list_class, is_default, status, parent_id, create_by, create_time, update_by, update_time, remark, is_del) VALUES (115, 0, '20丝', '20丝', 'wms_goods_standard', NULL, 'default', 'N', '0', 0, 'admin', '2022-11-12 16:52:28', NULL, NULL, NULL, 0);
INSERT INTO sys_dict_data (dict_data_id, dict_sort, dict_label, dict_value, dict_type, css_class, list_class, is_default, status, parent_id, create_by, create_time, update_by, update_time, remark, is_del) VALUES (116, 0, '21丝', '21丝', 'wms_goods_standard', NULL, 'default', 'N', '0', 0, 'admin', '2022-11-12 16:52:28', NULL, NULL, NULL, 0);
INSERT INTO sys_dict_data (dict_data_id, dict_sort, dict_label, dict_value, dict_type, css_class, list_class, is_default, status, parent_id, create_by, create_time, update_by, update_time, remark, is_del) VALUES (117, 0, '22丝', '22丝', 'wms_goods_standard', NULL, 'default', 'N', '0', 0, 'admin', '2022-11-12 16:52:28', NULL, NULL, NULL, 0);
INSERT INTO sys_dict_data (dict_data_id, dict_sort, dict_label, dict_value, dict_type, css_class, list_class, is_default, status, parent_id, create_by, create_time, update_by, update_time, remark, is_del) VALUES (118, 0, '23丝', '23丝', 'wms_goods_standard', NULL, 'default', 'N', '0', 0, 'admin', '2022-11-12 16:52:28', NULL, NULL, NULL, 0);
INSERT INTO sys_dict_data (dict_data_id, dict_sort, dict_label, dict_value, dict_type, css_class, list_class, is_default, status, parent_id, create_by, create_time, update_by, update_time, remark, is_del) VALUES (119, 0, '24丝', '24丝', 'wms_goods_standard', NULL, 'default', 'N', '0', 0, 'admin', '2022-11-12 16:52:28', NULL, NULL, NULL, 0);
INSERT INTO sys_dict_data (dict_data_id, dict_sort, dict_label, dict_value, dict_type, css_class, list_class, is_default, status, parent_id, create_by, create_time, update_by, update_time, remark, is_del) VALUES (120, 0, '25丝', '25丝', 'wms_goods_standard', NULL, 'default', 'N', '0', 0, 'admin', '2022-11-12 16:52:28', NULL, NULL, NULL, 0);
INSERT INTO sys_dict_data (dict_data_id, dict_sort, dict_label, dict_value, dict_type, css_class, list_class, is_default, status, parent_id, create_by, create_time, update_by, update_time, remark, is_del) VALUES (121, 0, '26丝', '26丝', 'wms_goods_standard', NULL, 'default', 'N', '0', 0, 'admin', '2022-11-12 16:52:28', NULL, NULL, NULL, 0);
INSERT INTO sys_dict_data (dict_data_id, dict_sort, dict_label, dict_value, dict_type, css_class, list_class, is_default, status, parent_id, create_by, create_time, update_by, update_time, remark, is_del) VALUES (122, 0, '28丝', '28丝', 'wms_goods_standard', NULL, 'default', 'N', '0', 0, 'admin', '2022-11-12 16:52:28', NULL, NULL, NULL, 0);
INSERT INTO sys_dict_data (dict_data_id, dict_sort, dict_label, dict_value, dict_type, css_class, list_class, is_default, status, parent_id, create_by, create_time, update_by, update_time, remark, is_del) VALUES (123, 0, '29丝', '29丝', 'wms_goods_standard', NULL, 'default', 'N', '0', 0, 'admin', '2022-11-12 16:52:28', NULL, NULL, NULL, 0);
INSERT INTO sys_dict_data (dict_data_id, dict_sort, dict_label, dict_value, dict_type, css_class, list_class, is_default, status, parent_id, create_by, create_time, update_by, update_time, remark, is_del) VALUES (124, 0, '30丝', '30丝', 'wms_goods_standard', NULL, 'default', 'N', '0', 0, 'admin', '2022-11-12 16:52:28', NULL, NULL, NULL, 0);
INSERT INTO sys_dict_data (dict_data_id, dict_sort, dict_label, dict_value, dict_type, css_class, list_class, is_default, status, parent_id, create_by, create_time, update_by, update_time, remark, is_del) VALUES (125, 0, '32丝', '32丝', 'wms_goods_standard', NULL, 'default', 'N', '0', 0, 'admin', '2022-11-12 16:52:28', NULL, NULL, NULL, 0);
INSERT INTO sys_dict_data (dict_data_id, dict_sort, dict_label, dict_value, dict_type, css_class, list_class, is_default, status, parent_id, create_by, create_time, update_by, update_time, remark, is_del) VALUES (126, 0, '33丝', '33丝', 'wms_goods_standard', NULL, 'default', 'N', '0', 0, 'admin', '2022-11-12 16:52:28', NULL, NULL, NULL, 0);
INSERT INTO sys_dict_data (dict_data_id, dict_sort, dict_label, dict_value, dict_type, css_class, list_class, is_default, status, parent_id, create_by, create_time, update_by, update_time, remark, is_del) VALUES (127, 0, '34丝', '34丝', 'wms_goods_standard', NULL, 'default', 'N', '0', 0, 'admin', '2022-11-12 16:52:28', NULL, NULL, NULL, 0);
INSERT INTO sys_dict_data (dict_data_id, dict_sort, dict_label, dict_value, dict_type, css_class, list_class, is_default, status, parent_id, create_by, create_time, update_by, update_time, remark, is_del) VALUES (128, 0, '35丝', '35丝', 'wms_goods_standard', NULL, 'default', 'N', '0', 0, 'admin', '2022-11-12 16:52:28', NULL, NULL, NULL, 0);
INSERT INTO sys_dict_data (dict_data_id, dict_sort, dict_label, dict_value, dict_type, css_class, list_class, is_default, status, parent_id, create_by, create_time, update_by, update_time, remark, is_del) VALUES (129, 0, '36丝', '36丝', 'wms_goods_standard', NULL, 'default', 'N', '0', 0, 'admin', '2022-11-12 16:52:28', NULL, NULL, NULL, 0);
INSERT INTO sys_dict_data (dict_data_id, dict_sort, dict_label, dict_value, dict_type, css_class, list_class, is_default, status, parent_id, create_by, create_time, update_by, update_time, remark, is_del) VALUES (130, 0, '37丝', '37丝', 'wms_goods_standard', NULL, 'default', 'N', '0', 0, 'admin', '2022-11-12 16:52:28', NULL, NULL, NULL, 0);
INSERT INTO sys_dict_data (dict_data_id, dict_sort, dict_label, dict_value, dict_type, css_class, list_class, is_default, status, parent_id, create_by, create_time, update_by, update_time, remark, is_del) VALUES (131, 0, '38丝', '38丝', 'wms_goods_standard', NULL, 'default', 'N', '0', 0, 'admin', '2022-11-12 16:52:28', NULL, NULL, NULL, 0);
INSERT INTO sys_dict_data (dict_data_id, dict_sort, dict_label, dict_value, dict_type, css_class, list_class, is_default, status, parent_id, create_by, create_time, update_by, update_time, remark, is_del) VALUES (132, 0, '39丝', '39丝', 'wms_goods_standard', NULL, 'default', 'N', '0', 0, 'admin', '2022-11-12 16:52:28', NULL, NULL, NULL, 0);
INSERT INTO sys_dict_data (dict_data_id, dict_sort, dict_label, dict_value, dict_type, css_class, list_class, is_default, status, parent_id, create_by, create_time, update_by, update_time, remark, is_del) VALUES (133, 0, '40丝', '40丝', 'wms_goods_standard', NULL, 'default', 'N', '0', 0, 'admin', '2022-11-12 16:52:28', NULL, NULL, NULL, 0);
INSERT INTO sys_dict_data (dict_data_id, dict_sort, dict_label, dict_value, dict_type, css_class, list_class, is_default, status, parent_id, create_by, create_time, update_by, update_time, remark, is_del) VALUES (134, 0, '41丝', '41丝', 'wms_goods_standard', NULL, 'default', 'N', '0', 0, 'admin', '2022-11-12 16:52:28', NULL, NULL, NULL, 0);
INSERT INTO sys_dict_data (dict_data_id, dict_sort, dict_label, dict_value, dict_type, css_class, list_class, is_default, status, parent_id, create_by, create_time, update_by, update_time, remark, is_del) VALUES (135, 0, '42丝', '42丝', 'wms_goods_standard', NULL, 'default', 'N', '0', 0, 'admin', '2022-11-12 16:52:28', NULL, NULL, NULL, 0);
INSERT INTO sys_dict_data (dict_data_id, dict_sort, dict_label, dict_value, dict_type, css_class, list_class, is_default, status, parent_id, create_by, create_time, update_by, update_time, remark, is_del) VALUES (136, 0, '43丝', '43丝', 'wms_goods_standard', NULL, 'default', 'N', '0', 0, 'admin', '2022-11-12 16:52:28', NULL, NULL, NULL, 0);
INSERT INTO sys_dict_data (dict_data_id, dict_sort, dict_label, dict_value, dict_type, css_class, list_class, is_default, status, parent_id, create_by, create_time, update_by, update_time, remark, is_del) VALUES (137, 0, '45丝', '45丝', 'wms_goods_standard', NULL, 'default', 'N', '0', 0, 'admin', '2022-11-12 16:52:28', NULL, NULL, NULL, 0);
INSERT INTO sys_dict_data (dict_data_id, dict_sort, dict_label, dict_value, dict_type, css_class, list_class, is_default, status, parent_id, create_by, create_time, update_by, update_time, remark, is_del) VALUES (138, 0, '47丝', '47丝', 'wms_goods_standard', NULL, 'default', 'N', '0', 0, 'admin', '2022-11-12 16:52:28', NULL, NULL, NULL, 0);
INSERT INTO sys_dict_data (dict_data_id, dict_sort, dict_label, dict_value, dict_type, css_class, list_class, is_default, status, parent_id, create_by, create_time, update_by, update_time, remark, is_del) VALUES (139, 0, '48丝', '48丝', 'wms_goods_standard', NULL, 'default', 'N', '0', 0, 'admin', '2022-11-12 16:52:28', NULL, NULL, NULL, 0);
INSERT INTO sys_dict_data (dict_data_id, dict_sort, dict_label, dict_value, dict_type, css_class, list_class, is_default, status, parent_id, create_by, create_time, update_by, update_time, remark, is_del) VALUES (140, 0, '50丝', '50丝', 'wms_goods_standard', NULL, 'default', 'N', '0', 0, 'admin', '2022-11-12 16:52:28', NULL, NULL, NULL, 0);
INSERT INTO sys_dict_data (dict_data_id, dict_sort, dict_label, dict_value, dict_type, css_class, list_class, is_default, status, parent_id, create_by, create_time, update_by, update_time, remark, is_del) VALUES (141, 0, '57丝', '57丝', 'wms_goods_standard', NULL, 'default', 'N', '0', 0, 'admin', '2022-11-12 16:52:28', NULL, NULL, NULL, 0);
INSERT INTO sys_dict_data (dict_data_id, dict_sort, dict_label, dict_value, dict_type, css_class, list_class, is_default, status, parent_id, create_by, create_time, update_by, update_time, remark, is_del) VALUES (142, 0, '58丝', '58丝', 'wms_goods_standard', NULL, 'default', 'N', '0', 0, 'admin', '2022-11-12 16:52:28', NULL, NULL, NULL, 0);
INSERT INTO sys_dict_data (dict_data_id, dict_sort, dict_label, dict_value, dict_type, css_class, list_class, is_default, status, parent_id, create_by, create_time, update_by, update_time, remark, is_del) VALUES (143, 0, '60丝', '60丝', 'wms_goods_standard', NULL, 'default', 'N', '0', 0, 'admin', '2022-11-12 16:52:28', NULL, NULL, NULL, 0);
INSERT INTO sys_dict_data (dict_data_id, dict_sort, dict_label, dict_value, dict_type, css_class, list_class, is_default, status, parent_id, create_by, create_time, update_by, update_time, remark, is_del) VALUES (144, 0, '65丝', '65丝', 'wms_goods_standard', NULL, 'default', 'N', '0', 0, 'admin', '2022-11-12 16:52:28', NULL, NULL, NULL, 0);
INSERT INTO sys_dict_data (dict_data_id, dict_sort, dict_label, dict_value, dict_type, css_class, list_class, is_default, status, parent_id, create_by, create_time, update_by, update_time, remark, is_del) VALUES (145, 0, '68丝', '68丝', 'wms_goods_standard', NULL, 'default', 'N', '0', 0, 'admin', '2022-11-12 16:52:28', NULL, NULL, NULL, 0);
INSERT INTO sys_dict_data (dict_data_id, dict_sort, dict_label, dict_value, dict_type, css_class, list_class, is_default, status, parent_id, create_by, create_time, update_by, update_time, remark, is_del) VALUES (146, 0, '100丝', '100丝', 'wms_goods_standard', NULL, 'default', 'N', '0', 0, 'admin', '2022-11-12 16:52:28', NULL, NULL, NULL, 0);
INSERT INTO sys_dict_data (dict_data_id, dict_sort, dict_label, dict_value, dict_type, css_class, list_class, is_default, status, parent_id, create_by, create_time, update_by, update_time, remark, is_del) VALUES (147, 0, '300丝', '300丝', 'wms_goods_standard', NULL, 'default', 'N', '0', 0, 'admin', '2022-11-12 16:52:28', NULL, NULL, NULL, 0);
INSERT INTO sys_dict_data (dict_data_id, dict_sort, dict_label, dict_value, dict_type, css_class, list_class, is_default, status, parent_id, create_by, create_time, update_by, update_time, remark, is_del) VALUES (148, 0, '500丝', '500丝', 'wms_goods_standard', NULL, 'default', 'N', '0', 0, 'admin', '2022-11-13 09:17:51', 'admin', '2022-11-13 09:18:14', NULL, 0);
INSERT INTO sys_dict_data (dict_data_id, dict_sort, dict_label, dict_value, dict_type, css_class, list_class, is_default, status, parent_id, create_by, create_time, update_by, update_time, remark, is_del) VALUES (149, 0, '合格', '合格', 'wms_goods_quality', NULL, 'default', 'N', '0', 0, 'admin', '2022-11-13 09:18:57', 'admin', '2022-11-13 12:59:14', NULL, 0);
INSERT INTO sys_dict_data (dict_data_id, dict_sort, dict_label, dict_value, dict_type, css_class, list_class, is_default, status, parent_id, create_by, create_time, update_by, update_time, remark, is_del) VALUES (150, 4, '报废', 'scrap', 'wms_goods_status', NULL, 'danger', 'N', '0', 0, 'admin', '2022-11-17 23:34:23', 'admin', '2022-11-17 23:34:33', NULL, 0);
INSERT INTO sys_dict_data (dict_data_id, dict_sort, dict_label, dict_value, dict_type, css_class, list_class, is_default, status, parent_id, create_by, create_time, update_by, update_time, remark, is_del) VALUES (151, 0, '17', '17', 'wms_goods_standard', NULL, 'default', 'N', '0', 0, 'admin', '2022-11-25 16:32:16', NULL, NULL, NULL, 0);
INSERT INTO sys_dict_data (dict_data_id, dict_sort, dict_label, dict_value, dict_type, css_class, list_class, is_default, status, parent_id, create_by, create_time, update_by, update_time, remark, is_del) VALUES (152, 0, '44丝', '44丝', 'wms_goods_standard', NULL, 'default', 'N', '0', 0, 'admin', '2022-12-05 15:30:30', NULL, NULL, NULL, 0);
INSERT INTO sys_dict_data (dict_data_id, dict_sort, dict_label, dict_value, dict_type, css_class, list_class, is_default, status, parent_id, create_by, create_time, update_by, update_time, remark, is_del) VALUES (153, 0, '55S', '55S', 'wms_goods_standard', NULL, 'default', 'N', '0', 0, 'admin', '2022-12-07 17:25:31', NULL, NULL, NULL, 0);
INSERT INTO sys_dict_data (dict_data_id, dict_sort, dict_label, dict_value, dict_type, css_class, list_class, is_default, status, parent_id, create_by, create_time, update_by, update_time, remark, is_del) VALUES (154, 0, '78S', '78S', 'wms_goods_standard', NULL, 'default', 'N', '0', 0, 'admin', '2022-12-10 10:22:50', NULL, NULL, NULL, 0);
INSERT INTO sys_dict_data (dict_data_id, dict_sort, dict_label, dict_value, dict_type, css_class, list_class, is_default, status, parent_id, create_by, create_time, update_by, update_time, remark, is_del) VALUES (155, 0, '120S', '120S', 'wms_goods_standard', NULL, 'default', 'N', '0', 0, 'admin', '2022-12-10 10:42:50', NULL, NULL, NULL, 0);
INSERT INTO sys_dict_data (dict_data_id, dict_sort, dict_label, dict_value, dict_type, css_class, list_class, is_default, status, parent_id, create_by, create_time, update_by, update_time, remark, is_del) VALUES (156, 0, '60S', '60S', 'wms_goods_standard', NULL, 'default', 'N', '0', 0, 'admin', '2022-12-10 12:06:29', NULL, NULL, NULL, 0);
INSERT INTO sys_dict_data (dict_data_id, dict_sort, dict_label, dict_value, dict_type, css_class, list_class, is_default, status, parent_id, create_by, create_time, update_by, update_time, remark, is_del) VALUES (157, 0, '52S', '52S', 'wms_goods_standard', NULL, 'default', 'N', '0', 0, 'admin', '2022-12-10 16:40:56', NULL, NULL, NULL, 0);
INSERT INTO sys_dict_data (dict_data_id, dict_sort, dict_label, dict_value, dict_type, css_class, list_class, is_default, status, parent_id, create_by, create_time, update_by, update_time, remark, is_del) VALUES (158, 0, '203*84', '203*84', 'wms_goods_standard', NULL, 'default', 'N', '0', 0, 'admin', '2022-12-11 10:16:03', NULL, NULL, NULL, 0);
INSERT INTO sys_dict_data (dict_data_id, dict_sort, dict_label, dict_value, dict_type, css_class, list_class, is_default, status, parent_id, create_by, create_time, update_by, update_time, remark, is_del) VALUES (159, 0, '54丝', '54丝', 'wms_goods_standard', NULL, 'default', 'N', '0', 0, 'admin', '2022-12-15 10:52:21', NULL, NULL, NULL, 0);
INSERT INTO sys_dict_data (dict_data_id, dict_sort, dict_label, dict_value, dict_type, css_class, list_class, is_default, status, parent_id, create_by, create_time, update_by, update_time, remark, is_del) VALUES (160, 0, '110丝', '110丝', 'wms_goods_standard', NULL, 'default', 'N', '0', 0, 'admin', '2022-12-15 11:17:17', NULL, NULL, NULL, 0);
INSERT INTO sys_dict_data (dict_data_id, dict_sort, dict_label, dict_value, dict_type, css_class, list_class, is_default, status, parent_id, create_by, create_time, update_by, update_time, remark, is_del) VALUES (161, 0, '测试', 'test', 'sys_flow_field', NULL, 'default', 'N', '0', 0, 'admin', '2022-12-18 17:23:45', NULL, NULL, NULL, 0);
INSERT INTO sys_dict_data (dict_data_id, dict_sort, dict_label, dict_value, dict_type, css_class, list_class, is_default, status, parent_id, create_by, create_time, update_by, update_time, remark, is_del) VALUES (900011, 1, '原材料', 'material', 'wms_material_type', NULL, 'info', 'N', '0', 0, 'system', '2026-07-23 22:15:25', 'system', '2026-07-23 22:15:25', NULL, 0);
INSERT INTO sys_dict_data (dict_data_id, dict_sort, dict_label, dict_value, dict_type, css_class, list_class, is_default, status, parent_id, create_by, create_time, update_by, update_time, remark, is_del) VALUES (900012, 2, '半成品', 'semi', 'wms_material_type', NULL, 'warning', 'N', '0', 0, 'system', '2026-07-23 22:15:25', 'system', '2026-07-23 22:15:25', NULL, 0);
INSERT INTO sys_dict_data (dict_data_id, dict_sort, dict_label, dict_value, dict_type, css_class, list_class, is_default, status, parent_id, create_by, create_time, update_by, update_time, remark, is_del) VALUES (900013, 3, '成品', 'product', 'wms_material_type', NULL, 'success', 'N', '0', 0, 'system', '2026-07-23 22:15:25', 'system', '2026-07-23 22:15:25', NULL, 0);
INSERT INTO sys_dict_data (dict_data_id, dict_sort, dict_label, dict_value, dict_type, css_class, list_class, is_default, status, parent_id, create_by, create_time, update_by, update_time, remark, is_del) VALUES (900014, 4, '包装物', 'package', 'wms_material_type', NULL, 'danger', 'N', '0', 0, 'system', '2026-07-24 15:46:51', 'admin', '2026-07-24 15:49:23', NULL, 0);
INSERT INTO sys_dict_data (dict_data_id, dict_sort, dict_label, dict_value, dict_type, css_class, list_class, is_default, status, parent_id, create_by, create_time, update_by, update_time, remark, is_del) VALUES (900040, 1, '原材料', '01', 'wms_material_category', NULL, 'default', 'N', '0', 0, 'system', '2026-07-23 22:15:45', 'system', '2026-07-23 22:15:45', NULL, 0);
INSERT INTO sys_dict_data (dict_data_id, dict_sort, dict_label, dict_value, dict_type, css_class, list_class, is_default, status, parent_id, create_by, create_time, update_by, update_time, remark, is_del) VALUES (900041, 1, '金属材料', '0101', 'wms_material_category', NULL, 'default', 'N', '0', 900040, 'system', '2026-07-23 22:15:45', 'system', '2026-07-23 22:15:45', NULL, 0);
INSERT INTO sys_dict_data (dict_data_id, dict_sort, dict_label, dict_value, dict_type, css_class, list_class, is_default, status, parent_id, create_by, create_time, update_by, update_time, remark, is_del) VALUES (900042, 2, '塑料原料', '0102', 'wms_material_category', NULL, 'default', 'N', '0', 900040, 'system', '2026-07-23 22:15:45', 'system', '2026-07-23 22:15:45', NULL, 0);
INSERT INTO sys_dict_data (dict_data_id, dict_sort, dict_label, dict_value, dict_type, css_class, list_class, is_default, status, parent_id, create_by, create_time, update_by, update_time, remark, is_del) VALUES (900045, 3, '成品', '03', 'wms_material_category', NULL, 'default', 'N', '0', 0, 'system', '2026-07-23 22:15:45', 'system', '2026-07-23 22:15:45', NULL, 0);
INSERT INTO sys_dict_data (dict_data_id, dict_sort, dict_label, dict_value, dict_type, css_class, list_class, is_default, status, parent_id, create_by, create_time, update_by, update_time, remark, is_del) VALUES (900046, 4, '包装物', '04', 'wms_material_category', NULL, 'default', 'N', '0', 0, 'system', '2026-07-23 22:15:45', 'system', '2026-07-23 22:15:45', NULL, 0);
INSERT INTO sys_dict_data (dict_data_id, dict_sort, dict_label, dict_value, dict_type, css_class, list_class, is_default, status, parent_id, create_by, create_time, update_by, update_time, remark, is_del) VALUES (900047, 3, '纺织面料', '0103', 'wms_material_category', NULL, 'default', 'N', '0', 900040, 'system', '2026-07-23 22:15:45', 'system', '2026-07-23 22:15:45', NULL, 0);
INSERT INTO sys_dict_data (dict_data_id, dict_sort, dict_label, dict_value, dict_type, css_class, list_class, is_default, status, parent_id, create_by, create_time, update_by, update_time, remark, is_del) VALUES (900048, 4, '化工原料', '0104', 'wms_material_category', NULL, 'default', 'N', '0', 900040, 'system', '2026-07-23 22:15:45', 'system', '2026-07-23 22:15:45', NULL, 0);
INSERT INTO sys_dict_data (dict_data_id, dict_sort, dict_label, dict_value, dict_type, css_class, list_class, is_default, status, parent_id, create_by, create_time, update_by, update_time, remark, is_del) VALUES (900049, 2, '半成品', '02', 'wms_material_category', NULL, 'default', 'N', '0', 0, 'system', '2026-07-23 22:15:45', 'system', '2026-07-23 22:15:45', NULL, 0);
INSERT INTO sys_dict_data (dict_data_id, dict_sort, dict_label, dict_value, dict_type, css_class, list_class, is_default, status, parent_id, create_by, create_time, update_by, update_time, remark, is_del) VALUES (900050, 1, '电子组件', '0201', 'wms_material_category', NULL, 'default', 'N', '0', 900049, 'system', '2026-07-23 22:15:45', 'system', '2026-07-23 22:15:45', NULL, 0);
INSERT INTO sys_dict_data (dict_data_id, dict_sort, dict_label, dict_value, dict_type, css_class, list_class, is_default, status, parent_id, create_by, create_time, update_by, update_time, remark, is_del) VALUES (900051, 1, '盘点异常', 'check', 'wms_lock_type', NULL, 'warning', 'N', '0', 0, 'system', '2026-07-21 23:35:14', 'system', '2026-07-21 23:35:14', NULL, 0);
INSERT INTO sys_dict_data (dict_data_id, dict_sort, dict_label, dict_value, dict_type, css_class, list_class, is_default, status, parent_id, create_by, create_time, update_by, update_time, remark, is_del) VALUES (900052, 2, '维护', 'maintain', 'wms_lock_type', NULL, 'info', 'N', '0', 0, 'system', '2026-07-21 23:35:14', 'system', '2026-07-21 23:35:14', NULL, 0);
INSERT INTO sys_dict_data (dict_data_id, dict_sort, dict_label, dict_value, dict_type, css_class, list_class, is_default, status, parent_id, create_by, create_time, update_by, update_time, remark, is_del) VALUES (900053, 3, '其他', 'other', 'wms_lock_type', NULL, 'success', 'N', '0', 0, 'system', '2026-07-21 23:35:14', 'admin', '2026-07-22 22:25:36', NULL, 0);
INSERT INTO sys_dict_data (dict_data_id, dict_sort, dict_label, dict_value, dict_type, css_class, list_class, is_default, status, parent_id, create_by, create_time, update_by, update_time, remark, is_del) VALUES (900054, 1, '电子产品', '0301', 'wms_material_category', NULL, 'default', 'N', '0', 900045, 'system', '2026-07-23 22:15:45', 'system', '2026-07-23 22:15:45', NULL, 0);
INSERT INTO sys_dict_data (dict_data_id, dict_sort, dict_label, dict_value, dict_type, css_class, list_class, is_default, status, parent_id, create_by, create_time, update_by, update_time, remark, is_del) VALUES (900055, 2, '机械设备', '0302', 'wms_material_category', NULL, 'default', 'N', '0', 900045, 'system', '2026-07-23 22:15:45', 'system', '2026-07-23 22:15:45', NULL, 0);
INSERT INTO sys_dict_data (dict_data_id, dict_sort, dict_label, dict_value, dict_type, css_class, list_class, is_default, status, parent_id, create_by, create_time, update_by, update_time, remark, is_del) VALUES (900056, 3, '服装成品', '0303', 'wms_material_category', NULL, 'default', 'N', '0', 900045, 'system', '2026-07-23 22:15:45', 'system', '2026-07-23 22:15:45', NULL, 0);
INSERT INTO sys_dict_data (dict_data_id, dict_sort, dict_label, dict_value, dict_type, css_class, list_class, is_default, status, parent_id, create_by, create_time, update_by, update_time, remark, is_del) VALUES (900057, 4, '日用品', '0304', 'wms_material_category', NULL, 'default', 'N', '0', 900045, 'system', '2026-07-23 22:15:45', 'system', '2026-07-23 22:15:45', NULL, 0);
INSERT INTO sys_dict_data (dict_data_id, dict_sort, dict_label, dict_value, dict_type, css_class, list_class, is_default, status, parent_id, create_by, create_time, update_by, update_time, remark, is_del) VALUES (900058, 1, '内包装', '0401', 'wms_material_category', NULL, 'default', 'N', '0', 900046, 'system', '2026-07-23 22:15:45', 'system', '2026-07-23 22:15:45', NULL, 0);
INSERT INTO sys_dict_data (dict_data_id, dict_sort, dict_label, dict_value, dict_type, css_class, list_class, is_default, status, parent_id, create_by, create_time, update_by, update_time, remark, is_del) VALUES (900059, 2, '外包装', '0402', 'wms_material_category', NULL, 'default', 'N', '0', 900046, 'system', '2026-07-23 22:15:45', 'system', '2026-07-23 22:15:45', NULL, 0);
INSERT INTO sys_dict_data (dict_data_id, dict_sort, dict_label, dict_value, dict_type, css_class, list_class, is_default, status, parent_id, create_by, create_time, update_by, update_time, remark, is_del) VALUES (900060, 3, '填充物', '0403', 'wms_material_category', NULL, 'default', 'N', '0', 900046, 'system', '2026-07-23 22:15:45', 'system', '2026-07-23 22:15:45', NULL, 0);
INSERT INTO sys_dict_data (dict_data_id, dict_sort, dict_label, dict_value, dict_type, css_class, list_class, is_default, status, parent_id, create_by, create_time, update_by, update_time, remark, is_del) VALUES (900064, 1, '普通仓', 'warehouse', 'wms_warehouse_type', NULL, 'info', 'N', '0', 0, 'system', '2026-07-23 22:14:48', 'admin', '2026-07-23 23:34:05', NULL, 0);
INSERT INTO sys_dict_data (dict_data_id, dict_sort, dict_label, dict_value, dict_type, css_class, list_class, is_default, status, parent_id, create_by, create_time, update_by, update_time, remark, is_del) VALUES (900065, 2, '冷库', 'cold', 'wms_warehouse_type', NULL, 'primary', 'N', '0', 0, 'system', '2026-07-23 22:14:48', 'system', '2026-07-23 22:14:48', NULL, 0);
INSERT INTO sys_dict_data (dict_data_id, dict_sort, dict_label, dict_value, dict_type, css_class, list_class, is_default, status, parent_id, create_by, create_time, update_by, update_time, remark, is_del) VALUES (900066, 3, '冷冻库', 'freezer', 'wms_warehouse_type', NULL, 'danger', 'N', '0', 0, 'system', '2026-07-23 22:14:48', 'system', '2026-07-23 22:14:48', NULL, 0);
INSERT INTO sys_dict_data (dict_data_id, dict_sort, dict_label, dict_value, dict_type, css_class, list_class, is_default, status, parent_id, create_by, create_time, update_by, update_time, remark, is_del) VALUES (900067, 1, '收货区', 'receive', 'wms_zone_type', NULL, 'info', 'N', '0', 0, 'system', '2026-07-23 22:48:16', 'system', '2026-07-23 22:48:16', NULL, 0);
INSERT INTO sys_dict_data (dict_data_id, dict_sort, dict_label, dict_value, dict_type, css_class, list_class, is_default, status, parent_id, create_by, create_time, update_by, update_time, remark, is_del) VALUES (900068, 2, '存储区', 'storage', 'wms_zone_type', NULL, 'primary', 'N', '0', 0, 'system', '2026-07-23 22:48:16', 'system', '2026-07-23 22:48:16', NULL, 0);
INSERT INTO sys_dict_data (dict_data_id, dict_sort, dict_label, dict_value, dict_type, css_class, list_class, is_default, status, parent_id, create_by, create_time, update_by, update_time, remark, is_del) VALUES (900069, 3, '拣货区', 'picking', 'wms_zone_type', NULL, 'success', 'N', '0', 0, 'system', '2026-07-23 22:48:16', 'system', '2026-07-23 22:48:16', NULL, 0);
INSERT INTO sys_dict_data (dict_data_id, dict_sort, dict_label, dict_value, dict_type, css_class, list_class, is_default, status, parent_id, create_by, create_time, update_by, update_time, remark, is_del) VALUES (900070, 4, '发货区', 'ship', 'wms_zone_type', NULL, 'warning', 'N', '0', 0, 'system', '2026-07-23 22:48:16', 'system', '2026-07-23 22:48:16', NULL, 0);
INSERT INTO sys_dict_data (dict_data_id, dict_sort, dict_label, dict_value, dict_type, css_class, list_class, is_default, status, parent_id, create_by, create_time, update_by, update_time, remark, is_del) VALUES (900071, 5, '退货区', 'return', 'wms_zone_type', NULL, 'danger', 'N', '0', 0, 'system', '2026-07-23 22:48:16', 'system', '2026-07-23 22:48:16', NULL, 0);
INSERT INTO sys_dict_data (dict_data_id, dict_sort, dict_label, dict_value, dict_type, css_class, list_class, is_default, status, parent_id, create_by, create_time, update_by, update_time, remark, is_del) VALUES (900072, 6, '暂存区', 'temp', 'wms_zone_type', NULL, 'info', 'N', '0', 0, 'system', '2026-07-23 22:48:16', 'admin', '2026-07-23 23:22:56', NULL, 0);
INSERT INTO sys_dict_data (dict_data_id, dict_sort, dict_label, dict_value, dict_type, css_class, list_class, is_default, status, parent_id, create_by, create_time, update_by, update_time, remark, is_del) VALUES (900073, 7, '不合格品区', 'reject', 'wms_zone_type', NULL, 'danger', 'N', '0', 0, 'system', '2026-07-23 22:48:16', 'system', '2026-07-23 22:48:16', NULL, 0);
INSERT INTO sys_dict_data (dict_data_id, dict_sort, dict_label, dict_value, dict_type, css_class, list_class, is_default, status, parent_id, create_by, create_time, update_by, update_time, remark, is_del) VALUES (900074, 1, '存储区', 'storage', 'wms_location_type', NULL, 'primary', 'N', '0', 0, 'system', '2026-07-23 22:48:17', 'system', '2026-07-23 22:48:17', NULL, 0);
INSERT INTO sys_dict_data (dict_data_id, dict_sort, dict_label, dict_value, dict_type, css_class, list_class, is_default, status, parent_id, create_by, create_time, update_by, update_time, remark, is_del) VALUES (900075, 2, '拣货区', 'picking', 'wms_location_type', NULL, 'success', 'N', '0', 0, 'system', '2026-07-23 22:48:17', 'system', '2026-07-23 22:48:17', NULL, 0);
INSERT INTO sys_dict_data (dict_data_id, dict_sort, dict_label, dict_value, dict_type, css_class, list_class, is_default, status, parent_id, create_by, create_time, update_by, update_time, remark, is_del) VALUES (900076, 3, '暂存区', 'temp', 'wms_location_type', NULL, 'info', 'N', '0', 0, 'system', '2026-07-23 22:48:17', 'admin', '2026-07-23 23:41:19', NULL, 0);
INSERT INTO sys_dict_data (dict_data_id, dict_sort, dict_label, dict_value, dict_type, css_class, list_class, is_default, status, parent_id, create_by, create_time, update_by, update_time, remark, is_del) VALUES (900077, 4, '退货区', 'return', 'wms_location_type', NULL, 'danger', 'N', '0', 0, 'system', '2026-07-23 22:48:17', 'system', '2026-07-23 22:48:17', NULL, 0);
INSERT INTO sys_dict_data (dict_data_id, dict_sort, dict_label, dict_value, dict_type, css_class, list_class, is_default, status, parent_id, create_by, create_time, update_by, update_time, remark, is_del) VALUES (1860616524608442368, 0, '公告', 'notice', 'sys_message_type', 'fa-sign-hanging', 'warning', 'N', '0', 0, 'admin', '2024-11-24 17:28:43', 'admin', '2026-03-19 17:50:56', NULL, 0);
INSERT INTO sys_dict_data (dict_data_id, dict_sort, dict_label, dict_value, dict_type, css_class, list_class, is_default, status, parent_id, create_by, create_time, update_by, update_time, remark, is_del) VALUES (1860616635468091392, 1, '流程', 'flow', 'sys_message_type', 'fa-stamp', 'success', 'N', '0', 0, 'admin', '2024-11-24 17:29:09', 'admin', '2026-03-19 17:51:21', NULL, 0);
INSERT INTO sys_dict_data (dict_data_id, dict_sort, dict_label, dict_value, dict_type, css_class, list_class, is_default, status, parent_id, create_by, create_time, update_by, update_time, remark, is_del) VALUES (1860618461936488448, 2, '系统', 'system', 'sys_message_type', 'fa-bell', 'info', 'N', '0', 0, 'admin', '2024-11-24 17:36:25', 'admin', '2026-03-19 17:51:02', NULL, 0);
INSERT INTO sys_dict_data (dict_data_id, dict_sort, dict_label, dict_value, dict_type, css_class, list_class, is_default, status, parent_id, create_by, create_time, update_by, update_time, remark, is_del) VALUES (1860618605608177664, 0, '预警', 'warning', 'sys_message_type', 'fa-alarm-clock', 'danger', 'N', '0', 0, 'admin', '2024-11-24 17:36:59', 'admin', '2026-11-11 21:58:29', NULL, 0);
INSERT INTO sys_dict_data (dict_data_id, dict_sort, dict_label, dict_value, dict_type, css_class, list_class, is_default, status, parent_id, create_by, create_time, update_by, update_time, remark, is_del) VALUES (1872886773923844096, 2, '默认', '0', 'sys_data_service_type', NULL, 'default', 'N', '0', 0, 'admin', '2024-12-28 14:06:18', 'admin', '2024-12-28 18:19:28', NULL, 0);
INSERT INTO sys_dict_data (dict_data_id, dict_sort, dict_label, dict_value, dict_type, css_class, list_class, is_default, status, parent_id, create_by, create_time, update_by, update_time, remark, is_del) VALUES (1872950353813704704, 1, '设备管理', '1', 'sys_data_service_type', NULL, 'default', 'N', '0', 0, 'admin', '2024-12-28 18:18:57', 'admin', '2024-12-28 18:19:01', NULL, 0);
INSERT INTO sys_dict_data (dict_data_id, dict_sort, dict_label, dict_value, dict_type, css_class, list_class, is_default, status, parent_id, create_by, create_time, update_by, update_time, remark, is_del) VALUES (2042162054340677632, 0, '系统内置', 'builtin', 'sys_data_service_category', NULL, 'default', 'N', '0', 0, 'admin', '2026-04-09 16:45:50', NULL, NULL, NULL, 0);
INSERT INTO sys_dict_data (dict_data_id, dict_sort, dict_label, dict_value, dict_type, css_class, list_class, is_default, status, parent_id, create_by, create_time, update_by, update_time, remark, is_del) VALUES (2042162266962530304, 0, '测试1', '1', 'sys_data_service_category', NULL, 'default', 'N', '0', 0, 'admin', '2026-04-09 16:46:41', NULL, NULL, NULL, 0);
INSERT INTO sys_dict_data (dict_data_id, dict_sort, dict_label, dict_value, dict_type, css_class, list_class, is_default, status, parent_id, create_by, create_time, update_by, update_time, remark, is_del) VALUES (2042162308423225344, 0, '测2', '2', 'sys_data_service_category', NULL, 'default', 'N', '0', 2042162266962530304, 'admin', '2026-04-09 16:46:51', 'admin', '2026-05-25 17:22:25', NULL, 0);
INSERT INTO sys_dict_data (dict_data_id, dict_sort, dict_label, dict_value, dict_type, css_class, list_class, is_default, status, parent_id, create_by, create_time, update_by, update_time, remark, is_del) VALUES (2042162343185616896, 0, '厕所2', '11', 'sys_data_service_category', NULL, 'default', 'N', '0', 2042162266962530304, 'admin', '2026-04-09 16:46:59', NULL, NULL, NULL, 0);
INSERT INTO sys_dict_data (dict_data_id, dict_sort, dict_label, dict_value, dict_type, css_class, list_class, is_default, status, parent_id, create_by, create_time, update_by, update_time, remark, is_del) VALUES (2042162343185616897, 2, '机械部件', '0202', 'wms_material_category', NULL, 'default', 'N', '0', 900049, 'system', '2026-07-23 22:16:11', 'system', '2026-07-23 22:16:11', NULL, 0);
INSERT INTO sys_dict_data (dict_data_id, dict_sort, dict_label, dict_value, dict_type, css_class, list_class, is_default, status, parent_id, create_by, create_time, update_by, update_time, remark, is_del) VALUES (2042162343185616898, 3, '注塑件', '0203', 'wms_material_category', NULL, 'default', 'N', '0', 900049, 'system', '2026-07-23 22:16:11', 'system', '2026-07-23 22:16:11', NULL, 0);
INSERT INTO sys_dict_data (dict_data_id, dict_sort, dict_label, dict_value, dict_type, css_class, list_class, is_default, status, parent_id, create_by, create_time, update_by, update_time, remark, is_del) VALUES (2042162343185616899, 4, '冲压件', '0204', 'wms_material_category', NULL, 'default', 'N', '0', 900049, 'system', '2026-07-23 22:16:11', 'system', '2026-07-23 22:16:11', NULL, 0);


-- ---------------------------- sys_i18n ----------------------------
DROP TABLE IF EXISTS sys_i18n;
CREATE TABLE sys_i18n (
  "i18n_id" bigint NOT NULL,
  "i18n_key" varchar(200) NOT NULL,
  "lang" varchar(10) NOT NULL,
  "i18n_value" varchar(500) CHARACTER SET utf8 DEFAULT NULL,
  "create_by" varchar(64) DEFAULT NULL,
  "create_time" timestamp DEFAULT NULL,
  "update_by" varchar(64) DEFAULT NULL,
  "update_time" timestamp DEFAULT NULL,
  "remark" varchar(500) DEFAULT NULL,
  "is_del" integer DEFAULT 0,
  PRIMARY KEY (i18n_id)
);
CREATE UNIQUE INDEX sys_i18n_idx_i18n_key_lang ON sys_i18n (i18n_key,lang);
COMMENT ON COLUMN sys_i18n.i18n_id IS '翻译ID';
COMMENT ON COLUMN sys_i18n.i18n_key IS '翻译键名';
COMMENT ON COLUMN sys_i18n.lang IS '语言';
COMMENT ON COLUMN sys_i18n.i18n_value IS '翻译文本';
COMMENT ON COLUMN sys_i18n.create_by IS '创建人';
COMMENT ON COLUMN sys_i18n.create_time IS '创建时间';
COMMENT ON COLUMN sys_i18n.update_by IS '修改人';
COMMENT ON COLUMN sys_i18n.update_time IS '修改时间';
COMMENT ON COLUMN sys_i18n.remark IS '备注';
COMMENT ON COLUMN sys_i18n.is_del IS '删除标志(0=正常,1=删除)';
DELETE FROM sys_i18n;
INSERT INTO sys_i18n (i18n_id, i18n_key, lang, i18n_value, create_by, create_time, update_by, update_time, remark, is_del) VALUES (1, 'sys.menu.1', 'zh-CN', '系统管理', NULL, '2026-07-03 23:45:36', NULL, '2026-07-03 23:45:36', NULL, 0);
INSERT INTO sys_i18n (i18n_id, i18n_key, lang, i18n_value, create_by, create_time, update_by, update_time, remark, is_del) VALUES (2, 'sys.menu.2', 'zh-CN', '流程管理', NULL, '1900-01-01 00:00:00', 'admin', '2026-07-15 08:56:48', NULL, NULL);
INSERT INTO sys_i18n (i18n_id, i18n_key, lang, i18n_value, create_by, create_time, update_by, update_time, remark, is_del) VALUES (3, 'sys.menu.3', 'zh-CN', '系统工具', NULL, '2026-07-03 23:45:36', NULL, '2026-07-03 23:45:36', NULL, 0);
INSERT INTO sys_i18n (i18n_id, i18n_key, lang, i18n_value, create_by, create_time, update_by, update_time, remark, is_del) VALUES (4, 'sys.menu.100', 'zh-CN', '用户管理', NULL, '2026-07-03 23:45:36', NULL, '2026-07-03 23:45:36', NULL, 0);
INSERT INTO sys_i18n (i18n_id, i18n_key, lang, i18n_value, create_by, create_time, update_by, update_time, remark, is_del) VALUES (5, 'sys.menu.101', 'zh-CN', '角色管理', NULL, '2026-07-03 23:45:36', NULL, '2026-07-03 23:45:36', NULL, 0);
INSERT INTO sys_i18n (i18n_id, i18n_key, lang, i18n_value, create_by, create_time, update_by, update_time, remark, is_del) VALUES (6, 'sys.menu.102', 'zh-CN', '菜单管理', NULL, '2026-07-03 23:45:36', NULL, '2026-07-03 23:45:36', NULL, 0);
INSERT INTO sys_i18n (i18n_id, i18n_key, lang, i18n_value, create_by, create_time, update_by, update_time, remark, is_del) VALUES (7, 'sys.menu.103', 'zh-CN', '部门管理', NULL, '2026-07-03 23:45:36', NULL, '2026-07-03 23:45:36', NULL, 0);
INSERT INTO sys_i18n (i18n_id, i18n_key, lang, i18n_value, create_by, create_time, update_by, update_time, remark, is_del) VALUES (8, 'sys.menu.104', 'zh-CN', '岗位管理', NULL, '2026-07-03 23:45:36', NULL, '2026-07-03 23:45:36', NULL, 0);
INSERT INTO sys_i18n (i18n_id, i18n_key, lang, i18n_value, create_by, create_time, update_by, update_time, remark, is_del) VALUES (9, 'sys.menu.105', 'zh-CN', '字典管理', NULL, '2026-07-03 23:45:36', NULL, '2026-07-03 23:45:36', NULL, 0);
INSERT INTO sys_i18n (i18n_id, i18n_key, lang, i18n_value, create_by, create_time, update_by, update_time, remark, is_del) VALUES (10, 'sys.menu.106', 'zh-CN', '参数设置', NULL, '2026-07-03 23:45:36', NULL, '2026-07-03 23:45:36', NULL, 0);
INSERT INTO sys_i18n (i18n_id, i18n_key, lang, i18n_value, create_by, create_time, update_by, update_time, remark, is_del) VALUES (11, 'sys.menu.109', 'zh-CN', '在线用户', NULL, '2026-07-03 23:45:36', NULL, '2026-07-03 23:45:36', NULL, 0);
INSERT INTO sys_i18n (i18n_id, i18n_key, lang, i18n_value, create_by, create_time, update_by, update_time, remark, is_del) VALUES (12, 'sys.menu.110', 'zh-CN', '定时任务', NULL, '2026-07-03 23:45:36', NULL, '2026-07-03 23:45:36', NULL, 0);
INSERT INTO sys_i18n (i18n_id, i18n_key, lang, i18n_value, create_by, create_time, update_by, update_time, remark, is_del) VALUES (13, 'sys.menu.114', 'zh-CN', '数据服务', NULL, '2026-07-03 23:45:36', NULL, '2026-07-03 23:45:36', NULL, 0);
INSERT INTO sys_i18n (i18n_id, i18n_key, lang, i18n_value, create_by, create_time, update_by, update_time, remark, is_del) VALUES (14, 'sys.menu.118', 'zh-CN', '系统日志', NULL, '1900-01-01 00:00:00', 'admin', '2026-07-09 14:51:23', NULL, NULL);
INSERT INTO sys_i18n (i18n_id, i18n_key, lang, i18n_value, create_by, create_time, update_by, update_time, remark, is_del) VALUES (15, 'sys.menu.119', 'zh-CN', '编码服务', NULL, '2026-07-03 23:45:36', NULL, '2026-07-03 23:45:36', NULL, 0);
INSERT INTO sys_i18n (i18n_id, i18n_key, lang, i18n_value, create_by, create_time, update_by, update_time, remark, is_del) VALUES (16, 'sys.menu.179', 'zh-CN', '许可证', NULL, '2026-07-03 23:45:36', NULL, '2026-07-03 23:45:36', NULL, 0);
INSERT INTO sys_i18n (i18n_id, i18n_key, lang, i18n_value, create_by, create_time, update_by, update_time, remark, is_del) VALUES (17, 'sys.menu.180', 'zh-CN', '流程管理', NULL, '2026-07-03 23:45:36', NULL, '2026-07-03 23:45:36', NULL, 0);
INSERT INTO sys_i18n (i18n_id, i18n_key, lang, i18n_value, create_by, create_time, update_by, update_time, remark, is_del) VALUES (18, 'sys.menu.181', 'zh-CN', '流程定义', NULL, '2026-07-03 23:45:36', NULL, '2026-07-03 23:45:36', NULL, 0);
INSERT INTO sys_i18n (i18n_id, i18n_key, lang, i18n_value, create_by, create_time, update_by, update_time, remark, is_del) VALUES (19, 'sys.menu.182', 'zh-CN', '流程实例', NULL, '2026-07-03 23:45:36', NULL, '2026-07-03 23:45:36', NULL, 0);
INSERT INTO sys_i18n (i18n_id, i18n_key, lang, i18n_value, create_by, create_time, update_by, update_time, remark, is_del) VALUES (20, 'sys.menu.183', 'zh-CN', '版本编辑', NULL, '2026-07-03 23:45:36', NULL, '2026-07-03 23:45:36', NULL, 0);
INSERT INTO sys_i18n (i18n_id, i18n_key, lang, i18n_value, create_by, create_time, update_by, update_time, remark, is_del) VALUES (21, 'sys.menu.185', 'zh-CN', '数据服务编辑', NULL, '2026-07-03 23:45:36', NULL, '2026-07-03 23:45:36', NULL, 0);
INSERT INTO sys_i18n (i18n_id, i18n_key, lang, i18n_value, create_by, create_time, update_by, update_time, remark, is_del) VALUES (22, 'sys.menu.188', 'zh-CN', '国际化词典', NULL, '2026-07-03 23:45:36', NULL, '2026-07-03 23:45:36', NULL, 0);
INSERT INTO sys_i18n (i18n_id, i18n_key, lang, i18n_value, create_by, create_time, update_by, update_time, remark, is_del) VALUES (23, 'sys.menu.1', 'en-US', 'System Management', NULL, '2026-07-03 23:45:40', NULL, '2026-07-03 23:45:40', NULL, 0);
INSERT INTO sys_i18n (i18n_id, i18n_key, lang, i18n_value, create_by, create_time, update_by, update_time, remark, is_del) VALUES (24, 'sys.menu.2', 'en-US', 'Process Management', NULL, '1900-01-01 00:00:00', 'admin', '2026-07-15 08:56:48', NULL, NULL);
INSERT INTO sys_i18n (i18n_id, i18n_key, lang, i18n_value, create_by, create_time, update_by, update_time, remark, is_del) VALUES (25, 'sys.menu.3', 'en-US', 'System Tools', NULL, '2026-07-03 23:45:40', NULL, '2026-07-03 23:45:40', NULL, 0);
INSERT INTO sys_i18n (i18n_id, i18n_key, lang, i18n_value, create_by, create_time, update_by, update_time, remark, is_del) VALUES (26, 'sys.menu.100', 'en-US', 'User Management', NULL, '2026-07-03 23:45:40', NULL, '2026-07-03 23:45:40', NULL, 0);
INSERT INTO sys_i18n (i18n_id, i18n_key, lang, i18n_value, create_by, create_time, update_by, update_time, remark, is_del) VALUES (27, 'sys.menu.101', 'en-US', 'Role Management', NULL, '2026-07-03 23:45:40', NULL, '2026-07-03 23:45:40', NULL, 0);
INSERT INTO sys_i18n (i18n_id, i18n_key, lang, i18n_value, create_by, create_time, update_by, update_time, remark, is_del) VALUES (28, 'sys.menu.102', 'en-US', 'Menu Management', NULL, '2026-07-03 23:45:40', NULL, '2026-07-03 23:45:40', NULL, 0);
INSERT INTO sys_i18n (i18n_id, i18n_key, lang, i18n_value, create_by, create_time, update_by, update_time, remark, is_del) VALUES (29, 'sys.menu.103', 'en-US', 'Dept Management', NULL, '2026-07-03 23:45:40', NULL, '2026-07-03 23:45:40', NULL, 0);
INSERT INTO sys_i18n (i18n_id, i18n_key, lang, i18n_value, create_by, create_time, update_by, update_time, remark, is_del) VALUES (30, 'sys.menu.104', 'en-US', 'Position Management', NULL, '2026-07-03 23:45:40', NULL, '2026-07-03 23:45:40', NULL, 0);
INSERT INTO sys_i18n (i18n_id, i18n_key, lang, i18n_value, create_by, create_time, update_by, update_time, remark, is_del) VALUES (31, 'sys.menu.105', 'en-US', 'Dict Management', NULL, '2026-07-03 23:45:40', NULL, '2026-07-03 23:45:40', NULL, 0);
INSERT INTO sys_i18n (i18n_id, i18n_key, lang, i18n_value, create_by, create_time, update_by, update_time, remark, is_del) VALUES (32, 'sys.menu.106', 'en-US', 'Parameter Settings', NULL, '2026-07-03 23:45:40', NULL, '2026-07-03 23:45:40', NULL, 0);
INSERT INTO sys_i18n (i18n_id, i18n_key, lang, i18n_value, create_by, create_time, update_by, update_time, remark, is_del) VALUES (33, 'sys.menu.109', 'en-US', 'Online Users', NULL, '2026-07-03 23:45:40', NULL, '2026-07-03 23:45:40', NULL, 0);
INSERT INTO sys_i18n (i18n_id, i18n_key, lang, i18n_value, create_by, create_time, update_by, update_time, remark, is_del) VALUES (34, 'sys.menu.110', 'en-US', 'Scheduled Tasks', NULL, '2026-07-03 23:45:40', NULL, '2026-07-03 23:45:40', NULL, 0);
INSERT INTO sys_i18n (i18n_id, i18n_key, lang, i18n_value, create_by, create_time, update_by, update_time, remark, is_del) VALUES (35, 'sys.menu.114', 'en-US', 'Data Services', NULL, '2026-07-03 23:45:40', NULL, '2026-07-03 23:45:40', NULL, 0);
INSERT INTO sys_i18n (i18n_id, i18n_key, lang, i18n_value, create_by, create_time, update_by, update_time, remark, is_del) VALUES (36, 'sys.menu.118', 'en-US', 'System Logs', NULL, '1900-01-01 00:00:00', 'admin', '2026-07-09 14:51:23', NULL, NULL);
INSERT INTO sys_i18n (i18n_id, i18n_key, lang, i18n_value, create_by, create_time, update_by, update_time, remark, is_del) VALUES (37, 'sys.menu.119', 'en-US', 'Code Services', NULL, '2026-07-03 23:45:40', NULL, '2026-07-03 23:45:40', NULL, 0);
INSERT INTO sys_i18n (i18n_id, i18n_key, lang, i18n_value, create_by, create_time, update_by, update_time, remark, is_del) VALUES (38, 'sys.menu.179', 'en-US', 'License', NULL, '2026-07-03 23:45:40', NULL, '2026-07-03 23:45:40', NULL, 0);
INSERT INTO sys_i18n (i18n_id, i18n_key, lang, i18n_value, create_by, create_time, update_by, update_time, remark, is_del) VALUES (39, 'sys.menu.180', 'en-US', 'Process Management', NULL, '2026-07-03 23:45:40', NULL, '2026-07-03 23:45:40', NULL, 0);
INSERT INTO sys_i18n (i18n_id, i18n_key, lang, i18n_value, create_by, create_time, update_by, update_time, remark, is_del) VALUES (40, 'sys.menu.181', 'en-US', 'Process Definition', NULL, '2026-07-03 23:45:40', NULL, '2026-07-03 23:45:40', NULL, 0);
INSERT INTO sys_i18n (i18n_id, i18n_key, lang, i18n_value, create_by, create_time, update_by, update_time, remark, is_del) VALUES (41, 'sys.menu.182', 'en-US', 'Process Instances', NULL, '2026-07-03 23:45:40', NULL, '2026-07-03 23:45:40', NULL, 0);
INSERT INTO sys_i18n (i18n_id, i18n_key, lang, i18n_value, create_by, create_time, update_by, update_time, remark, is_del) VALUES (42, 'sys.menu.183', 'en-US', 'Version Editor', NULL, '2026-07-03 23:45:40', NULL, '2026-07-03 23:45:40', NULL, 0);
INSERT INTO sys_i18n (i18n_id, i18n_key, lang, i18n_value, create_by, create_time, update_by, update_time, remark, is_del) VALUES (43, 'sys.menu.185', 'en-US', 'Data Services Editor', NULL, '2026-07-03 23:45:40', NULL, '2026-07-03 23:45:40', NULL, 0);
INSERT INTO sys_i18n (i18n_id, i18n_key, lang, i18n_value, create_by, create_time, update_by, update_time, remark, is_del) VALUES (44, 'sys.menu.188', 'en-US', 'I18n Dict', NULL, '2026-07-03 23:45:40', NULL, '2026-07-03 23:45:40', NULL, 0);


-- ---------------------------- sys_menu ----------------------------
DROP TABLE IF EXISTS sys_menu;
CREATE TABLE sys_menu (
  "menu_id" bigint NOT NULL,
  "menu_name" varchar(50) NOT NULL,
  "parent_id" bigint NOT NULL DEFAULT 0,
  "order_num" integer DEFAULT 0,
  "path" varchar(200) DEFAULT NULL,
  "route_name" varchar(200) DEFAULT NULL,
  "component" varchar(255) DEFAULT NULL,
  "query" varchar(255) DEFAULT NULL,
  "is_frame" integer DEFAULT 1,
  "is_cache" integer DEFAULT 0,
  "menu_type" varchar(1) DEFAULT NULL,
  "visible" varchar(1) DEFAULT NULL,
  "status" varchar(1) DEFAULT NULL,
  "perms" varchar(100) DEFAULT NULL,
  "icon" varchar(100) DEFAULT '#',
  "create_by" varchar(64) DEFAULT '',
  "create_time" timestamp DEFAULT NULL,
  "update_by" varchar(64) DEFAULT '',
  "update_time" timestamp DEFAULT NULL,
  "remark" varchar(500) DEFAULT '',
  "is_del" integer DEFAULT 0,
  "system_id" bigint DEFAULT NULL,
  PRIMARY KEY (menu_id)
);
COMMENT ON COLUMN sys_menu.menu_id IS '菜单ID';
COMMENT ON COLUMN sys_menu.menu_name IS '菜单名称';
COMMENT ON COLUMN sys_menu.parent_id IS '父菜单ID';
COMMENT ON COLUMN sys_menu.order_num IS '显示顺序';
COMMENT ON COLUMN sys_menu.path IS '路由地址';
COMMENT ON COLUMN sys_menu.route_name IS '路由名称';
COMMENT ON COLUMN sys_menu.component IS '组件路径';
COMMENT ON COLUMN sys_menu.query IS '路由参数';
COMMENT ON COLUMN sys_menu.is_frame IS '是否为外链（0是 1否）';
COMMENT ON COLUMN sys_menu.is_cache IS '是否缓存（0缓存 1不缓存）';
COMMENT ON COLUMN sys_menu.menu_type IS '菜单类型（M目录 C菜单 F按钮）';
COMMENT ON COLUMN sys_menu.visible IS '菜单状态（0显示 1隐藏）';
COMMENT ON COLUMN sys_menu.status IS '菜单状态（0正常 1停用）';
COMMENT ON COLUMN sys_menu.perms IS '权限标识';
COMMENT ON COLUMN sys_menu.icon IS '菜单图标';
COMMENT ON COLUMN sys_menu.create_by IS '创建者';
COMMENT ON COLUMN sys_menu.create_time IS '创建时间';
COMMENT ON COLUMN sys_menu.update_by IS '更新者';
COMMENT ON COLUMN sys_menu.update_time IS '更新时间';
COMMENT ON COLUMN sys_menu.remark IS '备注';
COMMENT ON COLUMN sys_menu.is_del IS '删除标志（0代表存在 1代表删除）';
COMMENT ON COLUMN sys_menu.system_id IS '系统id';
DELETE FROM sys_menu;
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, route_name, component, query, is_frame, is_cache, menu_type, visible, status, perms, icon, create_by, create_time, update_by, update_time, remark, is_del, system_id) VALUES (1, '系统管理', 0, 500, 'system', NULL, NULL, '', 1, 0, 'M', '0', '0', '', 'gear', 'admin', '2022-04-15 15:51:01', 'admin', '2024-11-01 22:55:50', '系统管理目录', 0, 100028);
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, route_name, component, query, is_frame, is_cache, menu_type, visible, status, perms, icon, create_by, create_time, update_by, update_time, remark, is_del, system_id) VALUES (2, '流程管理', 1, 499, 'flow', NULL, NULL, NULL, 1, 0, 'M', '0', '0', NULL, 'link', 'admin', '2026-07-01 22:48:12', 'admin', '2026-07-15 08:56:48', '流程管理目录', 0, 100028);
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, route_name, component, query, is_frame, is_cache, menu_type, visible, status, perms, icon, create_by, create_time, update_by, update_time, remark, is_del, system_id) VALUES (3, '系统工具', 0, 501, 'tool', NULL, NULL, '', 1, 0, 'M', '0', '0', '', 'toolbox', 'admin', '2022-04-15 15:51:01', 'admin', '2022-08-14 21:43:45', '系统工具目录', 0, 100028);
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, route_name, component, query, is_frame, is_cache, menu_type, visible, status, perms, icon, create_by, create_time, update_by, update_time, remark, is_del, system_id) VALUES (100, '用户管理', 1, 1, 'user', NULL, 'system/user/index', '', 1, 0, 'C', '0', '0', 'system:user:list', 'user', 'admin', '2022-04-15 15:51:01', '', NULL, '用户管理菜单', 0, 100028);
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, route_name, component, query, is_frame, is_cache, menu_type, visible, status, perms, icon, create_by, create_time, update_by, update_time, remark, is_del, system_id) VALUES (101, '角色管理', 1, 2, 'role', NULL, 'system/role/index', '', 1, 0, 'C', '0', '0', 'system:role:list', 'user-group', 'admin', '2022-04-15 15:51:01', 'admin', '2024-10-29 22:08:23', '角色管理菜单', 0, 100028);
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, route_name, component, query, is_frame, is_cache, menu_type, visible, status, perms, icon, create_by, create_time, update_by, update_time, remark, is_del, system_id) VALUES (102, '菜单管理', 1, 3, 'menu', NULL, 'system/menu/index', '', 1, 0, 'C', '0', '0', 'system:menu:list', 'folder-tree', 'admin', '2022-04-15 15:51:01', 'admin', '2024-10-29 22:14:19', '菜单管理菜单', 0, 100028);
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, route_name, component, query, is_frame, is_cache, menu_type, visible, status, perms, icon, create_by, create_time, update_by, update_time, remark, is_del, system_id) VALUES (103, '部门管理', 1, 4, 'dept', NULL, 'system/dept/index', '', 1, 0, 'C', '0', '0', 'system:dept:list', 'network-wired', 'admin', '2022-04-15 15:51:01', 'admin', '2024-10-29 22:15:33', '部门管理菜单', 0, 100028);
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, route_name, component, query, is_frame, is_cache, menu_type, visible, status, perms, icon, create_by, create_time, update_by, update_time, remark, is_del, system_id) VALUES (104, '岗位管理', 1, 5, 'post', NULL, 'system/post/index', '', 1, 0, 'C', '0', '0', 'system:post:list', 'user-tie', 'admin', '2022-04-15 15:51:01', 'admin', '2024-10-29 22:19:36', '岗位管理菜单', 0, 100028);
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, route_name, component, query, is_frame, is_cache, menu_type, visible, status, perms, icon, create_by, create_time, update_by, update_time, remark, is_del, system_id) VALUES (105, '字典管理', 1, 6, 'dict', NULL, 'system/dict/index', '', 1, 0, 'C', '0', '0', 'system:dict:list', 'file-invoice', 'admin', '2022-04-15 15:51:01', '', NULL, '字典管理菜单', 0, 100028);
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, route_name, component, query, is_frame, is_cache, menu_type, visible, status, perms, icon, create_by, create_time, update_by, update_time, remark, is_del, system_id) VALUES (106, '参数设置', 1, 7, 'config', NULL, 'system/config/index', '', 1, 0, 'C', '0', '0', 'system:config:list', 'file-signature', 'admin', '2022-04-15 15:51:01', '', NULL, '参数设置菜单', 0, 100028);
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, route_name, component, query, is_frame, is_cache, menu_type, visible, status, perms, icon, create_by, create_time, update_by, update_time, remark, is_del, system_id) VALUES (109, '在线用户', 3, 1, 'monitor', NULL, 'system/online/index', '', 1, 0, 'C', '0', '0', 'monitor:online:list', 'signal', 'admin', '2022-04-15 15:51:01', 'admin', '2026-07-01 17:42:46', '在线用户菜单', 0, 100028);
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, route_name, component, query, is_frame, is_cache, menu_type, visible, status, perms, icon, create_by, create_time, update_by, update_time, remark, is_del, system_id) VALUES (110, '定时任务', 3, 2, 'job', NULL, 'system/job/index', '', 1, 0, 'C', '0', '0', 'monitor:job:list', 'bars-progress', 'admin', '2022-04-15 15:51:01', 'admin', '2026-07-01 17:44:42', '定时任务菜单', 0, 100028);
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, route_name, component, query, is_frame, is_cache, menu_type, visible, status, perms, icon, create_by, create_time, update_by, update_time, remark, is_del, system_id) VALUES (114, '数据服务', 1, 98, 'data/service', 'DataService', 'system/dataService/index', '', 1, 0, 'C', '0', '0', 'system:dataService:list', 'crop-simple', 'admin', '2022-04-15 15:51:01', 'admin', '2025-01-01 14:20:45', '表单构建菜单', 0, 100028);
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, route_name, component, query, is_frame, is_cache, menu_type, visible, status, perms, icon, create_by, create_time, update_by, update_time, remark, is_del, system_id) VALUES (118, '系统日志', 3, 8, 'log', NULL, 'system/logRec/index', '', 1, 0, 'C', '0', '0', 'system:log:list', 'book-atlas', 'admin', '2022-04-15 15:51:01', 'admin', '2026-07-09 14:51:22', '登录日志菜单', 0, 100028);
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, route_name, component, query, is_frame, is_cache, menu_type, visible, status, perms, icon, create_by, create_time, update_by, update_time, remark, is_del, system_id) VALUES (119, '编码服务', 1, 9, 'coderule', NULL, 'system/codeRule/index', NULL, 1, 0, 'C', '0', '0', 'system:coderule:list', 'cubes', 'admin', '2022-07-24 23:41:24', 'admin', '2026-04-03 14:56:46', NULL, 0, 100028);
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, route_name, component, query, is_frame, is_cache, menu_type, visible, status, perms, icon, create_by, create_time, update_by, update_time, remark, is_del, system_id) VALUES (120, '序号生成新增', 119, 1, NULL, NULL, NULL, NULL, 1, 0, 'F', '0', '0', 'system:coderule:add', NULL, 'admin', '2022-07-25 01:10:30', NULL, NULL, NULL, 0, 100028);
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, route_name, component, query, is_frame, is_cache, menu_type, visible, status, perms, icon, create_by, create_time, update_by, update_time, remark, is_del, system_id) VALUES (121, '序号生成修改', 119, 2, NULL, NULL, NULL, NULL, 1, 0, 'F', '0', '0', 'system:coderule:edit', NULL, 'admin', '2022-07-25 01:10:53', NULL, NULL, NULL, 0, 100028);
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, route_name, component, query, is_frame, is_cache, menu_type, visible, status, perms, icon, create_by, create_time, update_by, update_time, remark, is_del, system_id) VALUES (122, '序号生成删除', 119, 3, NULL, NULL, NULL, NULL, 1, 0, 'F', '0', '0', 'system:coderule:remove', NULL, 'admin', '2022-07-25 01:11:12', NULL, NULL, NULL, 0, 100028);
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, route_name, component, query, is_frame, is_cache, menu_type, visible, status, perms, icon, create_by, create_time, update_by, update_time, remark, is_del, system_id) VALUES (123, '用户查询', 100, 1, '', NULL, '', '', 1, 0, 'F', '0', '0', 'system:user:query', '#', 'admin', '2022-04-15 15:51:01', '', NULL, '', 0, 100028);
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, route_name, component, query, is_frame, is_cache, menu_type, visible, status, perms, icon, create_by, create_time, update_by, update_time, remark, is_del, system_id) VALUES (124, '用户新增', 100, 2, '', NULL, '', '', 1, 0, 'F', '0', '0', 'system:user:add', '#', 'admin', '2022-04-15 15:51:01', '', NULL, '', 0, 100028);
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, route_name, component, query, is_frame, is_cache, menu_type, visible, status, perms, icon, create_by, create_time, update_by, update_time, remark, is_del, system_id) VALUES (125, '用户修改', 100, 3, '', NULL, '', '', 1, 0, 'F', '0', '0', 'system:user:edit', '#', 'admin', '2022-04-15 15:51:01', '', NULL, '', 0, 100028);
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, route_name, component, query, is_frame, is_cache, menu_type, visible, status, perms, icon, create_by, create_time, update_by, update_time, remark, is_del, system_id) VALUES (126, '用户删除', 100, 4, '', NULL, '', '', 1, 0, 'F', '0', '0', 'system:user:remove', '#', 'admin', '2022-04-15 15:51:01', '', NULL, '', 0, 100028);
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, route_name, component, query, is_frame, is_cache, menu_type, visible, status, perms, icon, create_by, create_time, update_by, update_time, remark, is_del, system_id) VALUES (127, '用户导出', 100, 5, '', NULL, '', '', 1, 0, 'F', '0', '0', 'system:user:export', '#', 'admin', '2022-04-15 15:51:01', '', NULL, '', 0, 100028);
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, route_name, component, query, is_frame, is_cache, menu_type, visible, status, perms, icon, create_by, create_time, update_by, update_time, remark, is_del, system_id) VALUES (128, '用户导入', 100, 6, '', NULL, '', '', 1, 0, 'F', '0', '0', 'system:user:import', '#', 'admin', '2022-04-15 15:51:01', '', NULL, '', 0, 100028);
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, route_name, component, query, is_frame, is_cache, menu_type, visible, status, perms, icon, create_by, create_time, update_by, update_time, remark, is_del, system_id) VALUES (129, '重置密码', 100, 7, '', NULL, '', '', 1, 0, 'F', '0', '0', 'system:user:resetPwd', '#', 'admin', '2022-04-15 15:51:01', '', NULL, '', 0, 100028);
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, route_name, component, query, is_frame, is_cache, menu_type, visible, status, perms, icon, create_by, create_time, update_by, update_time, remark, is_del, system_id) VALUES (130, '角色查询', 101, 1, '', NULL, '', '', 1, 0, 'F', '0', '0', 'system:role:query', '#', 'admin', '2022-04-15 15:51:01', '', NULL, '', 0, 100028);
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, route_name, component, query, is_frame, is_cache, menu_type, visible, status, perms, icon, create_by, create_time, update_by, update_time, remark, is_del, system_id) VALUES (131, '角色新增', 101, 2, '', NULL, '', '', 1, 0, 'F', '0', '0', 'system:role:add', '#', 'admin', '2022-04-15 15:51:01', '', NULL, '', 0, 100028);
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, route_name, component, query, is_frame, is_cache, menu_type, visible, status, perms, icon, create_by, create_time, update_by, update_time, remark, is_del, system_id) VALUES (132, '角色修改', 101, 3, '', NULL, '', '', 1, 0, 'F', '0', '0', 'system:role:edit', '#', 'admin', '2022-04-15 15:51:01', '', NULL, '', 0, 100028);
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, route_name, component, query, is_frame, is_cache, menu_type, visible, status, perms, icon, create_by, create_time, update_by, update_time, remark, is_del, system_id) VALUES (133, '角色删除', 101, 4, '', NULL, '', '', 1, 0, 'F', '0', '0', 'system:role:remove', '#', 'admin', '2022-04-15 15:51:01', '', NULL, '', 0, 100028);
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, route_name, component, query, is_frame, is_cache, menu_type, visible, status, perms, icon, create_by, create_time, update_by, update_time, remark, is_del, system_id) VALUES (134, '角色导出', 101, 5, '', NULL, '', '', 1, 0, 'F', '0', '0', 'system:role:export', '#', 'admin', '2022-04-15 15:51:01', '', NULL, '', 0, 100028);
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, route_name, component, query, is_frame, is_cache, menu_type, visible, status, perms, icon, create_by, create_time, update_by, update_time, remark, is_del, system_id) VALUES (135, '菜单查询', 102, 1, '', NULL, '', '', 1, 0, 'F', '0', '0', 'system:menu:query', '#', 'admin', '2022-04-15 15:51:01', '', NULL, '', 0, 100028);
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, route_name, component, query, is_frame, is_cache, menu_type, visible, status, perms, icon, create_by, create_time, update_by, update_time, remark, is_del, system_id) VALUES (136, '菜单新增', 102, 2, '', NULL, '', '', 1, 0, 'F', '0', '0', 'system:menu:add', '#', 'admin', '2022-04-15 15:51:01', '', NULL, '', 0, 100028);
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, route_name, component, query, is_frame, is_cache, menu_type, visible, status, perms, icon, create_by, create_time, update_by, update_time, remark, is_del, system_id) VALUES (137, '菜单修改', 102, 3, '', NULL, '', '', 1, 0, 'F', '0', '0', 'system:menu:edit', '#', 'admin', '2022-04-15 15:51:01', '', NULL, '', 0, 100028);
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, route_name, component, query, is_frame, is_cache, menu_type, visible, status, perms, icon, create_by, create_time, update_by, update_time, remark, is_del, system_id) VALUES (138, '菜单删除', 102, 4, '', NULL, '', '', 1, 0, 'F', '0', '0', 'system:menu:remove', '#', 'admin', '2022-04-15 15:51:01', '', NULL, '', 0, 100028);
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, route_name, component, query, is_frame, is_cache, menu_type, visible, status, perms, icon, create_by, create_time, update_by, update_time, remark, is_del, system_id) VALUES (139, '部门查询', 103, 1, '', NULL, '', '', 1, 0, 'F', '0', '0', 'system:dept:query', '#', 'admin', '2022-04-15 15:51:01', '', NULL, '', 0, 100028);
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, route_name, component, query, is_frame, is_cache, menu_type, visible, status, perms, icon, create_by, create_time, update_by, update_time, remark, is_del, system_id) VALUES (140, '部门新增', 103, 2, '', NULL, '', '', 1, 0, 'F', '0', '0', 'system:dept:add', '#', 'admin', '2022-04-15 15:51:01', '', NULL, '', 0, 100028);
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, route_name, component, query, is_frame, is_cache, menu_type, visible, status, perms, icon, create_by, create_time, update_by, update_time, remark, is_del, system_id) VALUES (141, '部门修改', 103, 3, '', NULL, '', '', 1, 0, 'F', '0', '0', 'system:dept:edit', '#', 'admin', '2022-04-15 15:51:01', '', NULL, '', 0, 100028);
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, route_name, component, query, is_frame, is_cache, menu_type, visible, status, perms, icon, create_by, create_time, update_by, update_time, remark, is_del, system_id) VALUES (142, '部门删除', 103, 4, '', NULL, '', '', 1, 0, 'F', '0', '0', 'system:dept:remove', '#', 'admin', '2022-04-15 15:51:01', '', NULL, '', 0, 100028);
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, route_name, component, query, is_frame, is_cache, menu_type, visible, status, perms, icon, create_by, create_time, update_by, update_time, remark, is_del, system_id) VALUES (143, '岗位查询', 104, 1, '', NULL, '', '', 1, 0, 'F', '0', '0', 'system:post:query', '#', 'admin', '2022-04-15 15:51:01', '', NULL, '', 0, 100028);
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, route_name, component, query, is_frame, is_cache, menu_type, visible, status, perms, icon, create_by, create_time, update_by, update_time, remark, is_del, system_id) VALUES (144, '岗位新增', 104, 2, '', NULL, '', '', 1, 0, 'F', '0', '0', 'system:post:add', '#', 'admin', '2022-04-15 15:51:01', '', NULL, '', 0, 100028);
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, route_name, component, query, is_frame, is_cache, menu_type, visible, status, perms, icon, create_by, create_time, update_by, update_time, remark, is_del, system_id) VALUES (145, '岗位修改', 104, 3, '', NULL, '', '', 1, 0, 'F', '0', '0', 'system:post:edit', '#', 'admin', '2022-04-15 15:51:01', '', NULL, '', 0, 100028);
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, route_name, component, query, is_frame, is_cache, menu_type, visible, status, perms, icon, create_by, create_time, update_by, update_time, remark, is_del, system_id) VALUES (146, '岗位删除', 104, 4, '', NULL, '', '', 1, 0, 'F', '0', '0', 'system:post:remove', '#', 'admin', '2022-04-15 15:51:01', '', NULL, '', 0, 100028);
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, route_name, component, query, is_frame, is_cache, menu_type, visible, status, perms, icon, create_by, create_time, update_by, update_time, remark, is_del, system_id) VALUES (147, '岗位导出', 104, 5, '', NULL, '', '', 1, 0, 'F', '0', '0', 'system:post:export', '#', 'admin', '2022-04-15 15:51:01', '', NULL, '', 0, 100028);
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, route_name, component, query, is_frame, is_cache, menu_type, visible, status, perms, icon, create_by, create_time, update_by, update_time, remark, is_del, system_id) VALUES (148, '字典查询', 105, 1, '#', NULL, '', '', 1, 0, 'F', '0', '0', 'system:dict:query', '#', 'admin', '2022-04-15 15:51:01', '', NULL, '', 0, 100028);
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, route_name, component, query, is_frame, is_cache, menu_type, visible, status, perms, icon, create_by, create_time, update_by, update_time, remark, is_del, system_id) VALUES (149, '字典新增', 105, 2, '#', NULL, '', '', 1, 0, 'F', '0', '0', 'system:dict:add', '#', 'admin', '2022-04-15 15:51:01', '', NULL, '', 0, 100028);
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, route_name, component, query, is_frame, is_cache, menu_type, visible, status, perms, icon, create_by, create_time, update_by, update_time, remark, is_del, system_id) VALUES (150, '字典修改', 105, 3, '#', NULL, '', '', 1, 0, 'F', '0', '0', 'system:dict:edit', '#', 'admin', '2022-04-15 15:51:01', '', NULL, '', 0, 100028);
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, route_name, component, query, is_frame, is_cache, menu_type, visible, status, perms, icon, create_by, create_time, update_by, update_time, remark, is_del, system_id) VALUES (151, '字典删除', 105, 4, '#', NULL, '', '', 1, 0, 'F', '0', '0', 'system:dict:remove', '#', 'admin', '2022-04-15 15:51:01', '', NULL, '', 0, 100028);
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, route_name, component, query, is_frame, is_cache, menu_type, visible, status, perms, icon, create_by, create_time, update_by, update_time, remark, is_del, system_id) VALUES (152, '字典导出', 105, 5, '#', NULL, '', '', 1, 0, 'F', '0', '0', 'system:dict:export', '#', 'admin', '2022-04-15 15:51:01', '', NULL, '', 0, 100028);
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, route_name, component, query, is_frame, is_cache, menu_type, visible, status, perms, icon, create_by, create_time, update_by, update_time, remark, is_del, system_id) VALUES (153, '参数查询', 106, 1, '#', NULL, '', '', 1, 0, 'F', '0', '0', 'system:config:query', '#', 'admin', '2022-04-15 15:51:01', '', NULL, '', 0, 100028);
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, route_name, component, query, is_frame, is_cache, menu_type, visible, status, perms, icon, create_by, create_time, update_by, update_time, remark, is_del, system_id) VALUES (154, '参数新增', 106, 2, '#', NULL, '', '', 1, 0, 'F', '0', '0', 'system:config:add', '#', 'admin', '2022-04-15 15:51:01', '', NULL, '', 0, 100028);
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, route_name, component, query, is_frame, is_cache, menu_type, visible, status, perms, icon, create_by, create_time, update_by, update_time, remark, is_del, system_id) VALUES (155, '参数修改', 106, 3, '#', NULL, '', '', 1, 0, 'F', '0', '0', 'system:config:edit', '#', 'admin', '2022-04-15 15:51:01', '', NULL, '', 0, 100028);
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, route_name, component, query, is_frame, is_cache, menu_type, visible, status, perms, icon, create_by, create_time, update_by, update_time, remark, is_del, system_id) VALUES (156, '参数删除', 106, 4, '#', NULL, '', '', 1, 0, 'F', '0', '0', 'system:config:remove', '#', 'admin', '2022-04-15 15:51:01', '', NULL, '', 0, 100028);
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, route_name, component, query, is_frame, is_cache, menu_type, visible, status, perms, icon, create_by, create_time, update_by, update_time, remark, is_del, system_id) VALUES (157, '参数导出', 106, 5, '#', NULL, '', '', 1, 0, 'F', '0', '0', 'system:config:export', '#', 'admin', '2022-04-15 15:51:01', '', NULL, '', 0, 100028);
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, route_name, component, query, is_frame, is_cache, menu_type, visible, status, perms, icon, create_by, create_time, update_by, update_time, remark, is_del, system_id) VALUES (161, '登录查询', 118, 1, '#', NULL, '', '', 1, 0, 'F', '0', '0', 'monitor:logininfor:query', '#', 'admin', '2022-04-15 15:51:01', '', NULL, '', 0, 100028);
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, route_name, component, query, is_frame, is_cache, menu_type, visible, status, perms, icon, create_by, create_time, update_by, update_time, remark, is_del, system_id) VALUES (162, '登录删除', 118, 2, '#', NULL, '', '', 1, 0, 'F', '0', '0', 'monitor:logininfor:remove', '#', 'admin', '2022-04-15 15:51:01', '', NULL, '', 0, 100028);
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, route_name, component, query, is_frame, is_cache, menu_type, visible, status, perms, icon, create_by, create_time, update_by, update_time, remark, is_del, system_id) VALUES (163, '日志导出', 118, 3, '#', NULL, '', '', 1, 0, 'F', '0', '0', 'monitor:logininfor:export', '#', 'admin', '2022-04-15 15:51:01', '', NULL, '', 0, 100028);
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, route_name, component, query, is_frame, is_cache, menu_type, visible, status, perms, icon, create_by, create_time, update_by, update_time, remark, is_del, system_id) VALUES (164, '在线查询', 109, 1, '#', NULL, '', '', 1, 0, 'F', '0', '0', 'monitor:online:query', '#', 'admin', '2022-04-15 15:51:01', '', NULL, '', 0, 100028);
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, route_name, component, query, is_frame, is_cache, menu_type, visible, status, perms, icon, create_by, create_time, update_by, update_time, remark, is_del, system_id) VALUES (165, '批量强退', 109, 2, '#', NULL, '', '', 1, 0, 'F', '0', '0', 'monitor:online:batchLogout', '#', 'admin', '2022-04-15 15:51:01', '', NULL, '', 0, 100028);
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, route_name, component, query, is_frame, is_cache, menu_type, visible, status, perms, icon, create_by, create_time, update_by, update_time, remark, is_del, system_id) VALUES (166, '单条强退', 109, 3, '#', NULL, '', '', 1, 0, 'F', '0', '0', 'monitor:online:forceLogout', '#', 'admin', '2022-04-15 15:51:01', '', NULL, '', 0, 100028);
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, route_name, component, query, is_frame, is_cache, menu_type, visible, status, perms, icon, create_by, create_time, update_by, update_time, remark, is_del, system_id) VALUES (167, '任务查询', 110, 1, '#', NULL, '', '', 1, 0, 'F', '0', '0', 'monitor:job:query', '#', 'admin', '2022-04-15 15:51:01', '', NULL, '', 0, 100028);
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, route_name, component, query, is_frame, is_cache, menu_type, visible, status, perms, icon, create_by, create_time, update_by, update_time, remark, is_del, system_id) VALUES (168, '任务新增', 110, 2, '#', NULL, '', '', 1, 0, 'F', '0', '0', 'monitor:job:add', '#', 'admin', '2022-04-15 15:51:01', '', NULL, '', 0, 100028);
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, route_name, component, query, is_frame, is_cache, menu_type, visible, status, perms, icon, create_by, create_time, update_by, update_time, remark, is_del, system_id) VALUES (169, '任务修改', 110, 3, '#', NULL, '', '', 1, 0, 'F', '0', '0', 'monitor:job:edit', '#', 'admin', '2022-04-15 15:51:01', '', NULL, '', 0, 100028);
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, route_name, component, query, is_frame, is_cache, menu_type, visible, status, perms, icon, create_by, create_time, update_by, update_time, remark, is_del, system_id) VALUES (170, '任务删除', 110, 4, '#', NULL, '', '', 1, 0, 'F', '0', '0', 'monitor:job:remove', '#', 'admin', '2022-04-15 15:51:01', '', NULL, '', 0, 100028);
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, route_name, component, query, is_frame, is_cache, menu_type, visible, status, perms, icon, create_by, create_time, update_by, update_time, remark, is_del, system_id) VALUES (171, '状态修改', 110, 5, '#', NULL, '', '', 1, 0, 'F', '0', '0', 'monitor:job:changeStatus', '#', 'admin', '2022-04-15 15:51:01', '', NULL, '', 0, 100028);
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, route_name, component, query, is_frame, is_cache, menu_type, visible, status, perms, icon, create_by, create_time, update_by, update_time, remark, is_del, system_id) VALUES (172, '任务导出', 110, 7, '#', NULL, '', '', 1, 0, 'F', '0', '0', 'monitor:job:export', '#', 'admin', '2022-04-15 15:51:01', '', NULL, '', 0, 100028);
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, route_name, component, query, is_frame, is_cache, menu_type, visible, status, perms, icon, create_by, create_time, update_by, update_time, remark, is_del, system_id) VALUES (179, '许可证', 3, 100, 'license', NULL, 'system/license/index', NULL, 1, 0, 'C', '0', '0', 'system:license:list', 'cubes', 'admin', '2022-12-07 21:43:43', 'admin', '2026-07-01 22:56:10', NULL, 0, 100028);
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, route_name, component, query, is_frame, is_cache, menu_type, visible, status, perms, icon, create_by, create_time, update_by, update_time, remark, is_del, system_id) VALUES (180, '流程管理', 1, 99, '/flow', NULL, NULL, NULL, 1, 0, 'M', '0', '0', NULL, 'shuffle', 'admin', '2022-12-16 23:06:28', 'admin', '2026-05-05 21:51:27', NULL, 1, 100028);
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, route_name, component, query, is_frame, is_cache, menu_type, visible, status, perms, icon, create_by, create_time, update_by, update_time, remark, is_del, system_id) VALUES (181, '流程定义', 2, 1, 'process', NULL, 'flow/process/index', NULL, 1, 0, 'C', '0', '0', 'flow:process:list', 'pen-ruler', 'admin', '2022-12-18 00:23:46', 'admin', '2026-07-01 22:50:43', NULL, 0, 100028);
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, route_name, component, query, is_frame, is_cache, menu_type, visible, status, perms, icon, create_by, create_time, update_by, update_time, remark, is_del, system_id) VALUES (182, '流程实例', 2, 2, 'instance', NULL, 'flow/instance/index', NULL, 1, 0, 'C', '0', '0', 'flow:instance:list', 'shapes', 'admin', '2022-12-18 16:36:06', 'admin', '2026-07-01 22:50:53', NULL, 0, 100028);
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, route_name, component, query, is_frame, is_cache, menu_type, visible, status, perms, icon, create_by, create_time, update_by, update_time, remark, is_del, system_id) VALUES (183, '版本编辑', 2, 11, 'process/:processId/version/:versionId', NULL, 'flow/process/flow', NULL, 1, 0, 'C', '1', '0', 'flow:version:edit', NULL, 'admin', '2022-12-21 00:52:26', 'admin', '2026-07-01 22:51:01', NULL, 0, 100028);
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, route_name, component, query, is_frame, is_cache, menu_type, visible, status, perms, icon, create_by, create_time, update_by, update_time, remark, is_del, system_id) VALUES (184, '数据服务查询', 114, 1, NULL, NULL, NULL, NULL, 1, 0, 'F', '0', '0', 'system:dataService:query', NULL, 'admin', '2024-12-30 11:54:13', NULL, NULL, NULL, 0, 100028);
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, route_name, component, query, is_frame, is_cache, menu_type, visible, status, perms, icon, create_by, create_time, update_by, update_time, remark, is_del, system_id) VALUES (185, '数据服务编辑', 1, 98, 'data/service/edit', 'DataServiceEdit', 'system/dataService/edit', NULL, 1, 0, 'C', '1', '0', 'system:dataService:edit', NULL, 'admin', '2024-12-30 11:55:11', 'admin', '2024-12-30 16:42:37', NULL, 0, 100028);
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, route_name, component, query, is_frame, is_cache, menu_type, visible, status, perms, icon, create_by, create_time, update_by, update_time, remark, is_del, system_id) VALUES (186, '数据服务删除', 114, 3, NULL, NULL, NULL, NULL, 1, 0, 'F', '0', '0', 'system:dataService:remove', NULL, 'admin', '2024-12-30 11:55:31', NULL, NULL, NULL, 0, 100028);
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, route_name, component, query, is_frame, is_cache, menu_type, visible, status, perms, icon, create_by, create_time, update_by, update_time, remark, is_del, system_id) VALUES (187, '数据服务新增', 114, 2, NULL, NULL, NULL, NULL, 1, 0, 'F', '0', '0', NULL, NULL, 'admin', '2026-07-03 10:37:28', NULL, NULL, NULL, 0, NULL);
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, route_name, component, query, is_frame, is_cache, menu_type, visible, status, perms, icon, create_by, create_time, update_by, update_time, remark, is_del, system_id) VALUES (188, '国际化词典', 1, 99, 'i18n', NULL, 'system/i18n/index', '', 1, 0, 'C', '0', '0', 'system:i18n:list', 'globe', 'admin', '2022-04-15 15:51:01', 'admin', '2026-07-03 22:43:08', '国际化词典管理菜单', 0, 100028);
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, route_name, component, query, is_frame, is_cache, menu_type, visible, status, perms, icon, create_by, create_time, update_by, update_time, remark, is_del, system_id) VALUES (1001, '仓库管理', 1045, 2, 'store', NULL, '', NULL, 1, 0, 'M', '0', '0', '', 'warehouse', 'admin', '2026-07-20 00:00:00', 'admin', '2026-09-22 11:16:27', '', 0, 100028);
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, route_name, component, query, is_frame, is_cache, menu_type, visible, status, perms, icon, create_by, create_time, update_by, update_time, remark, is_del, system_id) VALUES (1002, '仓库列表', 1001, 1, 'warehouse', NULL, 'wms/warehouse/index', NULL, 1, 0, 'C', '0', '0', 'wms:warehouse:list', 'warehouse', 'admin', '2026-07-20 00:00:00', '', NULL, '', 0, 100028);
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, route_name, component, query, is_frame, is_cache, menu_type, visible, status, perms, icon, create_by, create_time, update_by, update_time, remark, is_del, system_id) VALUES (1003, '查询', 1002, 1, '', NULL, '', NULL, 1, 0, 'F', '0', '0', 'wms:warehouse:query', '', 'admin', '2026-07-20 00:00:00', '', NULL, '', 0, 100028);
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, route_name, component, query, is_frame, is_cache, menu_type, visible, status, perms, icon, create_by, create_time, update_by, update_time, remark, is_del, system_id) VALUES (1004, '新增', 1002, 2, '', NULL, '', NULL, 1, 0, 'F', '0', '0', 'wms:warehouse:add', '', 'admin', '2026-07-20 00:00:00', '', NULL, '', 0, 100028);
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, route_name, component, query, is_frame, is_cache, menu_type, visible, status, perms, icon, create_by, create_time, update_by, update_time, remark, is_del, system_id) VALUES (1005, '修改', 1002, 3, '', NULL, '', NULL, 1, 0, 'F', '0', '0', 'wms:warehouse:edit', '', 'admin', '2026-07-20 00:00:00', '', NULL, '', 0, 100028);
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, route_name, component, query, is_frame, is_cache, menu_type, visible, status, perms, icon, create_by, create_time, update_by, update_time, remark, is_del, system_id) VALUES (1006, '删除', 1002, 4, '', NULL, '', NULL, 1, 0, 'F', '0', '0', 'wms:warehouse:remove', '', 'admin', '2026-07-20 00:00:00', '', NULL, '', 0, 100028);
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, route_name, component, query, is_frame, is_cache, menu_type, visible, status, perms, icon, create_by, create_time, update_by, update_time, remark, is_del, system_id) VALUES (1007, '导出', 1002, 5, '', NULL, '', NULL, 1, 0, 'F', '0', '0', 'wms:warehouse:export', '', 'admin', '2026-07-20 00:00:00', '', NULL, '', 0, 100028);
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, route_name, component, query, is_frame, is_cache, menu_type, visible, status, perms, icon, create_by, create_time, update_by, update_time, remark, is_del, system_id) VALUES (1008, '库区列表', 1001, 2, 'zone', NULL, 'wms/zone/index', NULL, 1, 0, 'C', '0', '0', 'wms:zone:list', 'map-location-dot', 'admin', '2026-07-20 00:00:00', '', NULL, '', 0, 100028);
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, route_name, component, query, is_frame, is_cache, menu_type, visible, status, perms, icon, create_by, create_time, update_by, update_time, remark, is_del, system_id) VALUES (1009, '查询', 1008, 1, '', NULL, '', NULL, 1, 0, 'F', '0', '0', 'wms:zone:query', '', 'admin', '2026-07-20 00:00:00', '', NULL, '', 0, 100028);
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, route_name, component, query, is_frame, is_cache, menu_type, visible, status, perms, icon, create_by, create_time, update_by, update_time, remark, is_del, system_id) VALUES (1010, '新增', 1008, 2, '', NULL, '', NULL, 1, 0, 'F', '0', '0', 'wms:zone:add', '', 'admin', '2026-07-20 00:00:00', '', NULL, '', 0, 100028);
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, route_name, component, query, is_frame, is_cache, menu_type, visible, status, perms, icon, create_by, create_time, update_by, update_time, remark, is_del, system_id) VALUES (1011, '修改', 1008, 3, '', NULL, '', NULL, 1, 0, 'F', '0', '0', 'wms:zone:edit', '', 'admin', '2026-07-20 00:00:00', '', NULL, '', 0, 100028);
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, route_name, component, query, is_frame, is_cache, menu_type, visible, status, perms, icon, create_by, create_time, update_by, update_time, remark, is_del, system_id) VALUES (1012, '删除', 1008, 4, '', NULL, '', NULL, 1, 0, 'F', '0', '0', 'wms:zone:remove', '', 'admin', '2026-07-20 00:00:00', '', NULL, '', 0, 100028);
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, route_name, component, query, is_frame, is_cache, menu_type, visible, status, perms, icon, create_by, create_time, update_by, update_time, remark, is_del, system_id) VALUES (1013, '导出', 1008, 5, '', NULL, '', NULL, 1, 0, 'F', '0', '0', 'wms:zone:export', '', 'admin', '2026-07-20 00:00:00', '', NULL, '', 0, 100028);
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, route_name, component, query, is_frame, is_cache, menu_type, visible, status, perms, icon, create_by, create_time, update_by, update_time, remark, is_del, system_id) VALUES (1014, '库位列表', 1001, 3, 'location', NULL, 'wms/location/index', NULL, 1, 0, 'C', '0', '0', 'wms:location:list', 'location-dot', 'admin', '2026-07-20 00:00:00', '', NULL, '', 0, 100028);
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, route_name, component, query, is_frame, is_cache, menu_type, visible, status, perms, icon, create_by, create_time, update_by, update_time, remark, is_del, system_id) VALUES (1015, '查询', 1014, 1, '', NULL, '', NULL, 1, 0, 'F', '0', '0', 'wms:location:query', '', 'admin', '2026-07-20 00:00:00', '', NULL, '', 0, 100028);
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, route_name, component, query, is_frame, is_cache, menu_type, visible, status, perms, icon, create_by, create_time, update_by, update_time, remark, is_del, system_id) VALUES (1016, '新增', 1014, 2, '', NULL, '', NULL, 1, 0, 'F', '0', '0', 'wms:location:add', '', 'admin', '2026-07-20 00:00:00', '', NULL, '', 0, 100028);
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, route_name, component, query, is_frame, is_cache, menu_type, visible, status, perms, icon, create_by, create_time, update_by, update_time, remark, is_del, system_id) VALUES (1017, '修改', 1014, 3, '', NULL, '', NULL, 1, 0, 'F', '0', '0', 'wms:location:edit', '', 'admin', '2026-07-20 00:00:00', '', NULL, '', 0, 100028);
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, route_name, component, query, is_frame, is_cache, menu_type, visible, status, perms, icon, create_by, create_time, update_by, update_time, remark, is_del, system_id) VALUES (1018, '删除', 1014, 4, '', NULL, '', NULL, 1, 0, 'F', '0', '0', 'wms:location:remove', '', 'admin', '2026-07-20 00:00:00', '', NULL, '', 0, 100028);
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, route_name, component, query, is_frame, is_cache, menu_type, visible, status, perms, icon, create_by, create_time, update_by, update_time, remark, is_del, system_id) VALUES (1019, '导出', 1014, 5, '', NULL, '', NULL, 1, 0, 'F', '0', '0', 'wms:location:export', '', 'admin', '2026-07-20 00:00:00', '', NULL, '', 0, 100028);
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, route_name, component, query, is_frame, is_cache, menu_type, visible, status, perms, icon, create_by, create_time, update_by, update_time, remark, is_del, system_id) VALUES (1020, '库位锁定', 1001, 4, 'locationLock', NULL, 'wms/locationLock/index', NULL, 1, 0, 'C', '0', '0', 'wms:locationLock:list', 'lock', 'admin', '2026-07-20 00:00:00', '', NULL, '', 0, 100028);
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, route_name, component, query, is_frame, is_cache, menu_type, visible, status, perms, icon, create_by, create_time, update_by, update_time, remark, is_del, system_id) VALUES (1021, '查询', 1020, 1, '', NULL, '', NULL, 1, 0, 'F', '0', '0', 'wms:locationLock:query', '', 'admin', '2026-07-20 00:00:00', '', NULL, '', 0, 100028);
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, route_name, component, query, is_frame, is_cache, menu_type, visible, status, perms, icon, create_by, create_time, update_by, update_time, remark, is_del, system_id) VALUES (1022, '新增', 1020, 2, '', NULL, '', NULL, 1, 0, 'F', '0', '0', 'wms:locationLock:add', '', 'admin', '2026-07-20 00:00:00', '', NULL, '', 0, 100028);
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, route_name, component, query, is_frame, is_cache, menu_type, visible, status, perms, icon, create_by, create_time, update_by, update_time, remark, is_del, system_id) VALUES (1023, '修改', 1020, 3, '', NULL, '', NULL, 1, 0, 'F', '0', '0', 'wms:locationLock:edit', '', 'admin', '2026-07-20 00:00:00', '', NULL, '', 0, 100028);
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, route_name, component, query, is_frame, is_cache, menu_type, visible, status, perms, icon, create_by, create_time, update_by, update_time, remark, is_del, system_id) VALUES (1024, '删除', 1020, 4, '', NULL, '', NULL, 1, 0, 'F', '0', '0', 'wms:locationLock:remove', '', 'admin', '2026-07-20 00:00:00', '', NULL, '', 0, 100028);
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, route_name, component, query, is_frame, is_cache, menu_type, visible, status, perms, icon, create_by, create_time, update_by, update_time, remark, is_del, system_id) VALUES (1025, '导出', 1020, 5, '', NULL, '', NULL, 1, 0, 'F', '0', '0', 'wms:locationLock:export', '', 'admin', '2026-07-20 00:00:00', '', NULL, '', 0, 100028);
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, route_name, component, query, is_frame, is_cache, menu_type, visible, status, perms, icon, create_by, create_time, update_by, update_time, remark, is_del, system_id) VALUES (1026, '物料管理', 1045, 1, 'goods', NULL, '', NULL, 1, 0, 'M', '0', '0', '', 'boxes-stacked', 'admin', '2026-07-20 00:00:00', 'admin', '2026-09-22 11:16:17', '', 0, 100028);
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, route_name, component, query, is_frame, is_cache, menu_type, visible, status, perms, icon, create_by, create_time, update_by, update_time, remark, is_del, system_id) VALUES (1027, '物料列表', 1026, 1, 'material', NULL, 'wms/material/index', NULL, 1, 0, 'C', '0', '0', 'wms:material:list', 'box', 'admin', '2026-07-20 00:00:00', '', NULL, '', 0, 100028);
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, route_name, component, query, is_frame, is_cache, menu_type, visible, status, perms, icon, create_by, create_time, update_by, update_time, remark, is_del, system_id) VALUES (1028, '查询', 1027, 1, '', NULL, '', NULL, 1, 0, 'F', '0', '0', 'wms:material:query', '', 'admin', '2026-07-20 00:00:00', '', NULL, '', 0, 100028);
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, route_name, component, query, is_frame, is_cache, menu_type, visible, status, perms, icon, create_by, create_time, update_by, update_time, remark, is_del, system_id) VALUES (1029, '新增', 1027, 2, '', NULL, '', NULL, 1, 0, 'F', '0', '0', 'wms:material:add', '', 'admin', '2026-07-20 00:00:00', '', NULL, '', 0, 100028);
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, route_name, component, query, is_frame, is_cache, menu_type, visible, status, perms, icon, create_by, create_time, update_by, update_time, remark, is_del, system_id) VALUES (1030, '修改', 1027, 3, '', NULL, '', NULL, 1, 0, 'F', '0', '0', 'wms:material:edit', '', 'admin', '2026-07-20 00:00:00', '', NULL, '', 0, 100028);
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, route_name, component, query, is_frame, is_cache, menu_type, visible, status, perms, icon, create_by, create_time, update_by, update_time, remark, is_del, system_id) VALUES (1031, '删除', 1027, 4, '', NULL, '', NULL, 1, 0, 'F', '0', '0', 'wms:material:remove', '', 'admin', '2026-07-20 00:00:00', '', NULL, '', 0, 100028);
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, route_name, component, query, is_frame, is_cache, menu_type, visible, status, perms, icon, create_by, create_time, update_by, update_time, remark, is_del, system_id) VALUES (1032, '导出', 1027, 5, '', NULL, '', NULL, 1, 0, 'F', '0', '0', 'wms:material:export', '', 'admin', '2026-07-20 00:00:00', '', NULL, '', 0, 100028);
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, route_name, component, query, is_frame, is_cache, menu_type, visible, status, perms, icon, create_by, create_time, update_by, update_time, remark, is_del, system_id) VALUES (1033, '包装规格', 1026, 2, 'packSpec', NULL, 'wms/packSpec/index', NULL, 1, 0, 'C', '0', '0', 'wms:packSpec:list', 'cube', 'admin', '2026-07-20 00:00:00', '', NULL, '', 0, 100028);
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, route_name, component, query, is_frame, is_cache, menu_type, visible, status, perms, icon, create_by, create_time, update_by, update_time, remark, is_del, system_id) VALUES (1034, '查询', 1033, 1, '', NULL, '', NULL, 1, 0, 'F', '0', '0', 'wms:packSpec:query', '', 'admin', '2026-07-20 00:00:00', '', NULL, '', 0, 100028);
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, route_name, component, query, is_frame, is_cache, menu_type, visible, status, perms, icon, create_by, create_time, update_by, update_time, remark, is_del, system_id) VALUES (1035, '新增', 1033, 2, '', NULL, '', NULL, 1, 0, 'F', '0', '0', 'wms:packSpec:add', '', 'admin', '2026-07-20 00:00:00', '', NULL, '', 0, 100028);
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, route_name, component, query, is_frame, is_cache, menu_type, visible, status, perms, icon, create_by, create_time, update_by, update_time, remark, is_del, system_id) VALUES (1036, '修改', 1033, 3, '', NULL, '', NULL, 1, 0, 'F', '0', '0', 'wms:packSpec:edit', '', 'admin', '2026-07-20 00:00:00', '', NULL, '', 0, 100028);
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, route_name, component, query, is_frame, is_cache, menu_type, visible, status, perms, icon, create_by, create_time, update_by, update_time, remark, is_del, system_id) VALUES (1037, '删除', 1033, 4, '', NULL, '', NULL, 1, 0, 'F', '0', '0', 'wms:packSpec:remove', '', 'admin', '2026-07-20 00:00:00', '', NULL, '', 0, 100028);
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, route_name, component, query, is_frame, is_cache, menu_type, visible, status, perms, icon, create_by, create_time, update_by, update_time, remark, is_del, system_id) VALUES (1038, '导出', 1033, 5, '', NULL, '', NULL, 1, 0, 'F', '0', '0', 'wms:packSpec:export', '', 'admin', '2026-07-20 00:00:00', '', NULL, '', 0, 100028);
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, route_name, component, query, is_frame, is_cache, menu_type, visible, status, perms, icon, create_by, create_time, update_by, update_time, remark, is_del, system_id) VALUES (1039, '检验项配置', 1026, 3, 'iqcItem', NULL, 'wms/iqcItem/index', NULL, 1, 0, 'C', '0', '0', 'wms:iqcItem:list', 'clipboard-check', 'admin', '2026-07-20 00:00:00', 'admin', '2026-08-03 17:34:06', '', 0, 100028);
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, route_name, component, query, is_frame, is_cache, menu_type, visible, status, perms, icon, create_by, create_time, update_by, update_time, remark, is_del, system_id) VALUES (1040, '查询', 1039, 1, '', NULL, '', NULL, 1, 0, 'F', '0', '0', 'wms:iqcItem:query', '', 'admin', '2026-07-20 00:00:00', 'admin', '2026-08-03 17:34:14', '', 0, 100028);
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, route_name, component, query, is_frame, is_cache, menu_type, visible, status, perms, icon, create_by, create_time, update_by, update_time, remark, is_del, system_id) VALUES (1041, '新增', 1039, 2, '', NULL, '', NULL, 1, 0, 'F', '0', '0', 'wms:iqcItem:add', '', 'admin', '2026-07-20 00:00:00', 'admin', '2026-08-03 17:34:19', '', 0, 100028);
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, route_name, component, query, is_frame, is_cache, menu_type, visible, status, perms, icon, create_by, create_time, update_by, update_time, remark, is_del, system_id) VALUES (1042, '修改', 1039, 3, '', NULL, '', NULL, 1, 0, 'F', '0', '0', 'wms:iqcItem:edit', '', 'admin', '2026-07-20 00:00:00', 'admin', '2026-08-03 17:34:25', '', 0, 100028);
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, route_name, component, query, is_frame, is_cache, menu_type, visible, status, perms, icon, create_by, create_time, update_by, update_time, remark, is_del, system_id) VALUES (1043, '删除', 1039, 4, '', NULL, '', NULL, 1, 0, 'F', '0', '0', 'wms:iqcItem:remove', '', 'admin', '2026-07-20 00:00:00', 'admin', '2026-08-03 17:34:30', '', 0, 100028);
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, route_name, component, query, is_frame, is_cache, menu_type, visible, status, perms, icon, create_by, create_time, update_by, update_time, remark, is_del, system_id) VALUES (1044, '导出', 1039, 5, '', NULL, '', NULL, 1, 0, 'F', '0', '0', 'wms:iqcItem:export', '', 'admin', '2026-07-20 00:00:00', 'admin', '2026-08-03 17:34:36', '', 0, 100028);
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, route_name, component, query, is_frame, is_cache, menu_type, visible, status, perms, icon, create_by, create_time, update_by, update_time, remark, is_del, system_id) VALUES (1045, '基础数据', 0, 50, 'baseinfo', NULL, '', NULL, 1, 0, 'M', '0', '0', '', 'wrench', 'admin', '2026-07-20 00:00:00', 'admin', '2026-09-22 11:14:34', '', 0, 100028);
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, route_name, component, query, is_frame, is_cache, menu_type, visible, status, perms, icon, create_by, create_time, update_by, update_time, remark, is_del, system_id) VALUES (1046, '供应商列表', 1045, 3, 'supplier', 'wms_supplier', 'wms/partner/index', NULL, 1, 0, 'C', '0', '0', 'wms:supplier:list', 'user-tie', 'admin', '2026-07-20 00:00:00', 'admin', '2026-09-22 11:23:20', '', 0, 100028);
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, route_name, component, query, is_frame, is_cache, menu_type, visible, status, perms, icon, create_by, create_time, update_by, update_time, remark, is_del, system_id) VALUES (1047, '查询', 1046, 1, '', NULL, '', NULL, 1, 0, 'F', '0', '0', 'wms:supplier:query', '', 'admin', '2026-07-20 00:00:00', '', NULL, '', 0, 100028);
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, route_name, component, query, is_frame, is_cache, menu_type, visible, status, perms, icon, create_by, create_time, update_by, update_time, remark, is_del, system_id) VALUES (1048, '新增', 1046, 2, '', NULL, '', NULL, 1, 0, 'F', '0', '0', 'wms:supplier:add', '', 'admin', '2026-07-20 00:00:00', '', NULL, '', 0, 100028);
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, route_name, component, query, is_frame, is_cache, menu_type, visible, status, perms, icon, create_by, create_time, update_by, update_time, remark, is_del, system_id) VALUES (1049, '修改', 1046, 3, '', NULL, '', NULL, 1, 0, 'F', '0', '0', 'wms:supplier:edit', '', 'admin', '2026-07-20 00:00:00', '', NULL, '', 0, 100028);
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, route_name, component, query, is_frame, is_cache, menu_type, visible, status, perms, icon, create_by, create_time, update_by, update_time, remark, is_del, system_id) VALUES (1050, '删除', 1046, 4, '', NULL, '', NULL, 1, 0, 'F', '0', '0', 'wms:supplier:remove', '', 'admin', '2026-07-20 00:00:00', '', NULL, '', 0, 100028);
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, route_name, component, query, is_frame, is_cache, menu_type, visible, status, perms, icon, create_by, create_time, update_by, update_time, remark, is_del, system_id) VALUES (1051, '导出', 1046, 5, '', NULL, '', NULL, 1, 0, 'F', '0', '0', 'wms:supplier:export', '', 'admin', '2026-07-20 00:00:00', '', NULL, '', 0, 100028);
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, route_name, component, query, is_frame, is_cache, menu_type, visible, status, perms, icon, create_by, create_time, update_by, update_time, remark, is_del, system_id) VALUES (1052, '客户管理', 0, 4, 'customer', NULL, '', NULL, 1, 0, 'M', '0', '0', '', 'users', 'admin', '2026-07-20 00:00:00', '', NULL, '', 1, 100028);
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, route_name, component, query, is_frame, is_cache, menu_type, visible, status, perms, icon, create_by, create_time, update_by, update_time, remark, is_del, system_id) VALUES (1053, '客户列表', 1045, 4, 'customer', 'wms_customer', 'wms/partner/index', NULL, 1, 0, 'C', '0', '0', 'wms:customer:list', 'users', 'admin', '2026-07-20 00:00:00', 'admin', '2026-09-22 11:23:34', '', 0, 100028);
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, route_name, component, query, is_frame, is_cache, menu_type, visible, status, perms, icon, create_by, create_time, update_by, update_time, remark, is_del, system_id) VALUES (1054, '查询', 1053, 1, '', NULL, '', NULL, 1, 0, 'F', '0', '0', 'wms:customer:query', '', 'admin', '2026-07-20 00:00:00', '', NULL, '', 0, 100028);
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, route_name, component, query, is_frame, is_cache, menu_type, visible, status, perms, icon, create_by, create_time, update_by, update_time, remark, is_del, system_id) VALUES (1055, '新增', 1053, 2, '', NULL, '', NULL, 1, 0, 'F', '0', '0', 'wms:customer:add', '', 'admin', '2026-07-20 00:00:00', '', NULL, '', 0, 100028);
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, route_name, component, query, is_frame, is_cache, menu_type, visible, status, perms, icon, create_by, create_time, update_by, update_time, remark, is_del, system_id) VALUES (1056, '修改', 1053, 3, '', NULL, '', NULL, 1, 0, 'F', '0', '0', 'wms:customer:edit', '', 'admin', '2026-07-20 00:00:00', '', NULL, '', 0, 100028);
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, route_name, component, query, is_frame, is_cache, menu_type, visible, status, perms, icon, create_by, create_time, update_by, update_time, remark, is_del, system_id) VALUES (1057, '删除', 1053, 4, '', NULL, '', NULL, 1, 0, 'F', '0', '0', 'wms:customer:remove', '', 'admin', '2026-07-20 00:00:00', '', NULL, '', 0, 100028);
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, route_name, component, query, is_frame, is_cache, menu_type, visible, status, perms, icon, create_by, create_time, update_by, update_time, remark, is_del, system_id) VALUES (1058, '导出', 1053, 5, '', NULL, '', NULL, 1, 0, 'F', '0', '0', 'wms:customer:export', '', 'admin', '2026-07-20 00:00:00', '', NULL, '', 0, 100028);
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, route_name, component, query, is_frame, is_cache, menu_type, visible, status, perms, icon, create_by, create_time, update_by, update_time, remark, is_del, system_id) VALUES (1059, '入库管理', 0, 5, 'in', NULL, '', NULL, 1, 0, 'M', '0', '0', '', 'truck-loading', 'admin', '2026-07-20 00:00:00', '', NULL, '', 0, 100028);
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, route_name, component, query, is_frame, is_cache, menu_type, visible, status, perms, icon, create_by, create_time, update_by, update_time, remark, is_del, system_id) VALUES (1060, '收货单', 1059, 1, 'receiving', NULL, 'wms/receiving/index', NULL, 1, 0, 'C', '0', '0', 'wms:receiving:list', 'calendar-check', 'admin', '2026-07-20 00:00:00', '', NULL, '', 0, 100028);
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, route_name, component, query, is_frame, is_cache, menu_type, visible, status, perms, icon, create_by, create_time, update_by, update_time, remark, is_del, system_id) VALUES (1061, '查询', 1060, 1, '', NULL, '', NULL, 1, 0, 'F', '0', '0', 'wms:receiving:query', '', 'admin', '2026-07-20 00:00:00', '', NULL, '', 0, 100028);
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, route_name, component, query, is_frame, is_cache, menu_type, visible, status, perms, icon, create_by, create_time, update_by, update_time, remark, is_del, system_id) VALUES (1062, '新增', 1060, 2, '', NULL, '', NULL, 1, 0, 'F', '0', '0', 'wms:receiving:add', '', 'admin', '2026-07-20 00:00:00', '', NULL, '', 0, 100028);
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, route_name, component, query, is_frame, is_cache, menu_type, visible, status, perms, icon, create_by, create_time, update_by, update_time, remark, is_del, system_id) VALUES (1063, '修改', 1060, 3, '', NULL, '', NULL, 1, 0, 'F', '0', '0', 'wms:receiving:edit', '', 'admin', '2026-07-20 00:00:00', '', NULL, '', 0, 100028);
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, route_name, component, query, is_frame, is_cache, menu_type, visible, status, perms, icon, create_by, create_time, update_by, update_time, remark, is_del, system_id) VALUES (1064, '删除', 1060, 4, '', NULL, '', NULL, 1, 0, 'F', '0', '0', 'wms:receiving:remove', '', 'admin', '2026-07-20 00:00:00', '', NULL, '', 0, 100028);
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, route_name, component, query, is_frame, is_cache, menu_type, visible, status, perms, icon, create_by, create_time, update_by, update_time, remark, is_del, system_id) VALUES (1065, '导出', 1060, 5, '', NULL, '', NULL, 1, 0, 'F', '0', '0', 'wms:receiving:export', '', 'admin', '2026-07-20 00:00:00', '', NULL, '', 0, 100028);
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, route_name, component, query, is_frame, is_cache, menu_type, visible, status, perms, icon, create_by, create_time, update_by, update_time, remark, is_del, system_id) VALUES (1066, '检验单', 1059, 2, 'iqcOrder', NULL, 'wms/iqcOrder/index', NULL, 1, 0, 'C', '0', '0', 'wms:iqcOrder:list', 'clipboard-check', 'admin', '2026-07-20 00:00:00', 'admin', '2026-08-03 16:37:13', '', 0, 100028);
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, route_name, component, query, is_frame, is_cache, menu_type, visible, status, perms, icon, create_by, create_time, update_by, update_time, remark, is_del, system_id) VALUES (1067, '查询', 1066, 1, '', NULL, '', NULL, 1, 0, 'F', '0', '0', 'wms:iqcOrder:query', '', 'admin', '2026-07-20 00:00:00', 'admin', '2026-08-03 16:40:02', '', 0, 100028);
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, route_name, component, query, is_frame, is_cache, menu_type, visible, status, perms, icon, create_by, create_time, update_by, update_time, remark, is_del, system_id) VALUES (1068, '新增', 1066, 2, '', NULL, '', NULL, 1, 0, 'F', '0', '0', 'wms:iqcOrder:add', '', 'admin', '2026-07-20 00:00:00', 'admin', '2026-08-03 16:40:08', '', 0, 100028);
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, route_name, component, query, is_frame, is_cache, menu_type, visible, status, perms, icon, create_by, create_time, update_by, update_time, remark, is_del, system_id) VALUES (1069, '修改', 1066, 3, '', NULL, '', NULL, 1, 0, 'F', '0', '0', 'wms:iqcOrder:edit', '', 'admin', '2026-07-20 00:00:00', 'admin', '2026-08-03 16:40:13', '', 0, 100028);
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, route_name, component, query, is_frame, is_cache, menu_type, visible, status, perms, icon, create_by, create_time, update_by, update_time, remark, is_del, system_id) VALUES (1070, '删除', 1066, 4, '', NULL, '', NULL, 1, 0, 'F', '0', '0', 'wms:iqcOrder:remove', '', 'admin', '2026-07-20 00:00:00', 'admin', '2026-08-03 16:41:18', '', 0, 100028);
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, route_name, component, query, is_frame, is_cache, menu_type, visible, status, perms, icon, create_by, create_time, update_by, update_time, remark, is_del, system_id) VALUES (1071, '导出', 1066, 5, '', NULL, '', NULL, 1, 0, 'F', '0', '0', 'wms:iqcOrder:export', '', 'admin', '2026-07-20 00:00:00', 'admin', '2026-08-03 16:41:25', '', 0, 100028);
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, route_name, component, query, is_frame, is_cache, menu_type, visible, status, perms, icon, create_by, create_time, update_by, update_time, remark, is_del, system_id) VALUES (1072, '入库单', 1059, 4, 'receipt', NULL, 'wms/receipt/index', NULL, 1, 0, 'C', '0', '0', 'wms:receipt:list', 'file-import', 'admin', '2026-07-20 00:00:00', '', NULL, '', 0, 100028);
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, route_name, component, query, is_frame, is_cache, menu_type, visible, status, perms, icon, create_by, create_time, update_by, update_time, remark, is_del, system_id) VALUES (1073, '查询', 1072, 1, '', NULL, '', NULL, 1, 0, 'F', '0', '0', 'wms:receipt:query', '', 'admin', '2026-07-20 00:00:00', '', NULL, '', 0, 100028);
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, route_name, component, query, is_frame, is_cache, menu_type, visible, status, perms, icon, create_by, create_time, update_by, update_time, remark, is_del, system_id) VALUES (1074, '新增', 1072, 2, '', NULL, '', NULL, 1, 0, 'F', '0', '0', 'wms:receipt:add', '', 'admin', '2026-07-20 00:00:00', '', NULL, '', 0, 100028);
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, route_name, component, query, is_frame, is_cache, menu_type, visible, status, perms, icon, create_by, create_time, update_by, update_time, remark, is_del, system_id) VALUES (1075, '修改', 1072, 3, '', NULL, '', NULL, 1, 0, 'F', '0', '0', 'wms:receipt:edit', '', 'admin', '2026-07-20 00:00:00', '', NULL, '', 0, 100028);
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, route_name, component, query, is_frame, is_cache, menu_type, visible, status, perms, icon, create_by, create_time, update_by, update_time, remark, is_del, system_id) VALUES (1076, '删除', 1072, 4, '', NULL, '', NULL, 1, 0, 'F', '0', '0', 'wms:receipt:remove', '', 'admin', '2026-07-20 00:00:00', '', NULL, '', 0, 100028);
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, route_name, component, query, is_frame, is_cache, menu_type, visible, status, perms, icon, create_by, create_time, update_by, update_time, remark, is_del, system_id) VALUES (1077, '导出', 1072, 5, '', NULL, '', NULL, 1, 0, 'F', '0', '0', 'wms:receipt:export', '', 'admin', '2026-07-20 00:00:00', '', NULL, '', 0, 100028);
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, route_name, component, query, is_frame, is_cache, menu_type, visible, status, perms, icon, create_by, create_time, update_by, update_time, remark, is_del, system_id) VALUES (1084, '入库退货', 1059, 6, 'returnIn', NULL, 'wms/returnIn/index', NULL, 1, 0, 'C', '0', '0', 'wms:returnIn:list', 'rotate-left', 'admin', '2026-07-20 00:00:00', '', NULL, '', 1, 100028);
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, route_name, component, query, is_frame, is_cache, menu_type, visible, status, perms, icon, create_by, create_time, update_by, update_time, remark, is_del, system_id) VALUES (1085, '查询', 1084, 1, '', NULL, '', NULL, 1, 0, 'F', '0', '0', 'wms:returnIn:query', '', 'admin', '2026-07-20 00:00:00', '', NULL, '', 1, 100028);
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, route_name, component, query, is_frame, is_cache, menu_type, visible, status, perms, icon, create_by, create_time, update_by, update_time, remark, is_del, system_id) VALUES (1086, '新增', 1084, 2, '', NULL, '', NULL, 1, 0, 'F', '0', '0', 'wms:returnIn:add', '', 'admin', '2026-07-20 00:00:00', '', NULL, '', 1, 100028);
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, route_name, component, query, is_frame, is_cache, menu_type, visible, status, perms, icon, create_by, create_time, update_by, update_time, remark, is_del, system_id) VALUES (1087, '修改', 1084, 3, '', NULL, '', NULL, 1, 0, 'F', '0', '0', 'wms:returnIn:edit', '', 'admin', '2026-07-20 00:00:00', '', NULL, '', 1, 100028);
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, route_name, component, query, is_frame, is_cache, menu_type, visible, status, perms, icon, create_by, create_time, update_by, update_time, remark, is_del, system_id) VALUES (1088, '删除', 1084, 4, '', NULL, '', NULL, 1, 0, 'F', '0', '0', 'wms:returnIn:remove', '', 'admin', '2026-07-20 00:00:00', '', NULL, '', 1, 100028);
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, route_name, component, query, is_frame, is_cache, menu_type, visible, status, perms, icon, create_by, create_time, update_by, update_time, remark, is_del, system_id) VALUES (1089, '导出', 1084, 5, '', NULL, '', NULL, 1, 0, 'F', '0', '0', 'wms:returnIn:export', '', 'admin', '2026-07-20 00:00:00', '', NULL, '', 1, 100028);
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, route_name, component, query, is_frame, is_cache, menu_type, visible, status, perms, icon, create_by, create_time, update_by, update_time, remark, is_del, system_id) VALUES (1090, '出库管理', 0, 6, 'out', NULL, '', NULL, 1, 0, 'M', '0', '0', '', 'truck', 'admin', '2026-07-20 00:00:00', '', NULL, '', 0, 100028);
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, route_name, component, query, is_frame, is_cache, menu_type, visible, status, perms, icon, create_by, create_time, update_by, update_time, remark, is_del, system_id) VALUES (1109, '出库单', 1090, 5, 'delivery', NULL, 'wms/delivery/index', NULL, 1, 0, 'C', '0', '0', 'wms:delivery:list', 'file-export', 'admin', '2026-07-20 00:00:00', '', NULL, '', 0, 100028);
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, route_name, component, query, is_frame, is_cache, menu_type, visible, status, perms, icon, create_by, create_time, update_by, update_time, remark, is_del, system_id) VALUES (1110, '查询', 1109, 1, '', NULL, '', NULL, 1, 0, 'F', '0', '0', 'wms:delivery:query', '', 'admin', '2026-07-20 00:00:00', '', NULL, '', 0, 100028);
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, route_name, component, query, is_frame, is_cache, menu_type, visible, status, perms, icon, create_by, create_time, update_by, update_time, remark, is_del, system_id) VALUES (1111, '新增', 1109, 2, '', NULL, '', NULL, 1, 0, 'F', '0', '0', 'wms:delivery:add', '', 'admin', '2026-07-20 00:00:00', '', NULL, '', 0, 100028);
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, route_name, component, query, is_frame, is_cache, menu_type, visible, status, perms, icon, create_by, create_time, update_by, update_time, remark, is_del, system_id) VALUES (1112, '修改', 1109, 3, '', NULL, '', NULL, 1, 0, 'F', '0', '0', 'wms:delivery:edit', '', 'admin', '2026-07-20 00:00:00', '', NULL, '', 0, 100028);
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, route_name, component, query, is_frame, is_cache, menu_type, visible, status, perms, icon, create_by, create_time, update_by, update_time, remark, is_del, system_id) VALUES (1113, '删除', 1109, 4, '', NULL, '', NULL, 1, 0, 'F', '0', '0', 'wms:delivery:remove', '', 'admin', '2026-07-20 00:00:00', '', NULL, '', 0, 100028);
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, route_name, component, query, is_frame, is_cache, menu_type, visible, status, perms, icon, create_by, create_time, update_by, update_time, remark, is_del, system_id) VALUES (1114, '导出', 1109, 5, '', NULL, '', NULL, 1, 0, 'F', '0', '0', 'wms:delivery:export', '', 'admin', '2026-07-20 00:00:00', '', NULL, '', 0, 100028);
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, route_name, component, query, is_frame, is_cache, menu_type, visible, status, perms, icon, create_by, create_time, update_by, update_time, remark, is_del, system_id) VALUES (1115, '出库退货', 1090, 6, 'returnOut', NULL, 'wms/returnOut/index', NULL, 1, 0, 'C', '0', '0', 'wms:returnOut:list', 'rotate-right', 'admin', '2026-07-20 00:00:00', '', NULL, '', 0, 100028);
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, route_name, component, query, is_frame, is_cache, menu_type, visible, status, perms, icon, create_by, create_time, update_by, update_time, remark, is_del, system_id) VALUES (1116, '查询', 1115, 1, '', NULL, '', NULL, 1, 0, 'F', '0', '0', 'wms:returnOut:query', '', 'admin', '2026-07-20 00:00:00', '', NULL, '', 0, 100028);
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, route_name, component, query, is_frame, is_cache, menu_type, visible, status, perms, icon, create_by, create_time, update_by, update_time, remark, is_del, system_id) VALUES (1117, '新增', 1115, 2, '', NULL, '', NULL, 1, 0, 'F', '0', '0', 'wms:returnOut:add', '', 'admin', '2026-07-20 00:00:00', '', NULL, '', 0, 100028);
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, route_name, component, query, is_frame, is_cache, menu_type, visible, status, perms, icon, create_by, create_time, update_by, update_time, remark, is_del, system_id) VALUES (1118, '修改', 1115, 3, '', NULL, '', NULL, 1, 0, 'F', '0', '0', 'wms:returnOut:edit', '', 'admin', '2026-07-20 00:00:00', '', NULL, '', 0, 100028);
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, route_name, component, query, is_frame, is_cache, menu_type, visible, status, perms, icon, create_by, create_time, update_by, update_time, remark, is_del, system_id) VALUES (1119, '删除', 1115, 4, '', NULL, '', NULL, 1, 0, 'F', '0', '0', 'wms:returnOut:remove', '', 'admin', '2026-07-20 00:00:00', '', NULL, '', 0, 100028);
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, route_name, component, query, is_frame, is_cache, menu_type, visible, status, perms, icon, create_by, create_time, update_by, update_time, remark, is_del, system_id) VALUES (1120, '导出', 1115, 5, '', NULL, '', NULL, 1, 0, 'F', '0', '0', 'wms:returnOut:export', '', 'admin', '2026-07-20 00:00:00', '', NULL, '', 0, 100028);
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, route_name, component, query, is_frame, is_cache, menu_type, visible, status, perms, icon, create_by, create_time, update_by, update_time, remark, is_del, system_id) VALUES (1121, '库内管理', 0, 7, 'inner', NULL, '', NULL, 1, 0, 'M', '0', '0', '', 'arrows-left-right', 'admin', '2026-07-20 00:00:00', '', NULL, '', 0, 100028);
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, route_name, component, query, is_frame, is_cache, menu_type, visible, status, perms, icon, create_by, create_time, update_by, update_time, remark, is_del, system_id) VALUES (1122, '库内移动', 1121, 1, 'move', NULL, 'wms/move/index', NULL, 1, 0, 'C', '0', '0', 'wms:move:list', 'arrows-up-down', 'admin', '2026-07-20 00:00:00', 'admin', '2026-11-16 22:15:21', '', 0, 100028);
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, route_name, component, query, is_frame, is_cache, menu_type, visible, status, perms, icon, create_by, create_time, update_by, update_time, remark, is_del, system_id) VALUES (1123, '查询', 1122, 1, '', NULL, '', NULL, 1, 0, 'F', '0', '0', 'wms:move:query', '', 'admin', '2026-07-20 00:00:00', '', NULL, '', 0, 100028);
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, route_name, component, query, is_frame, is_cache, menu_type, visible, status, perms, icon, create_by, create_time, update_by, update_time, remark, is_del, system_id) VALUES (1124, '新增', 1122, 2, '', NULL, '', NULL, 1, 0, 'F', '0', '0', 'wms:move:add', '', 'admin', '2026-07-20 00:00:00', '', NULL, '', 0, 100028);
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, route_name, component, query, is_frame, is_cache, menu_type, visible, status, perms, icon, create_by, create_time, update_by, update_time, remark, is_del, system_id) VALUES (1125, '修改', 1122, 3, '', NULL, '', NULL, 1, 0, 'F', '0', '0', 'wms:move:edit', '', 'admin', '2026-07-20 00:00:00', '', NULL, '', 0, 100028);
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, route_name, component, query, is_frame, is_cache, menu_type, visible, status, perms, icon, create_by, create_time, update_by, update_time, remark, is_del, system_id) VALUES (1126, '删除', 1122, 4, '', NULL, '', NULL, 1, 0, 'F', '0', '0', 'wms:move:remove', '', 'admin', '2026-07-20 00:00:00', '', NULL, '', 0, 100028);
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, route_name, component, query, is_frame, is_cache, menu_type, visible, status, perms, icon, create_by, create_time, update_by, update_time, remark, is_del, system_id) VALUES (1127, '导出', 1122, 5, '', NULL, '', NULL, 1, 0, 'F', '0', '0', 'wms:move:export', '', 'admin', '2026-07-20 00:00:00', '', NULL, '', 0, 100028);
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, route_name, component, query, is_frame, is_cache, menu_type, visible, status, perms, icon, create_by, create_time, update_by, update_time, remark, is_del, system_id) VALUES (1134, '库存调整', 1121, 3, 'adjust', NULL, 'wms/adjust/index', NULL, 1, 0, 'C', '0', '0', 'wms:adjust:list', 'sliders', 'admin', '2026-07-20 00:00:00', '', NULL, '', 0, 100028);
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, route_name, component, query, is_frame, is_cache, menu_type, visible, status, perms, icon, create_by, create_time, update_by, update_time, remark, is_del, system_id) VALUES (1135, '查询', 1134, 1, '', NULL, '', NULL, 1, 0, 'F', '0', '0', 'wms:adjust:query', '', 'admin', '2026-07-20 00:00:00', '', NULL, '', 0, 100028);
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, route_name, component, query, is_frame, is_cache, menu_type, visible, status, perms, icon, create_by, create_time, update_by, update_time, remark, is_del, system_id) VALUES (1136, '新增', 1134, 2, '', NULL, '', NULL, 1, 0, 'F', '0', '0', 'wms:adjust:add', '', 'admin', '2026-07-20 00:00:00', '', NULL, '', 0, 100028);
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, route_name, component, query, is_frame, is_cache, menu_type, visible, status, perms, icon, create_by, create_time, update_by, update_time, remark, is_del, system_id) VALUES (1137, '修改', 1134, 3, '', NULL, '', NULL, 1, 0, 'F', '0', '0', 'wms:adjust:edit', '', 'admin', '2026-07-20 00:00:00', '', NULL, '', 0, 100028);
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, route_name, component, query, is_frame, is_cache, menu_type, visible, status, perms, icon, create_by, create_time, update_by, update_time, remark, is_del, system_id) VALUES (1138, '删除', 1134, 4, '', NULL, '', NULL, 1, 0, 'F', '0', '0', 'wms:adjust:remove', '', 'admin', '2026-07-20 00:00:00', '', NULL, '', 0, 100028);
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, route_name, component, query, is_frame, is_cache, menu_type, visible, status, perms, icon, create_by, create_time, update_by, update_time, remark, is_del, system_id) VALUES (1139, '导出', 1134, 5, '', NULL, '', NULL, 1, 0, 'F', '0', '0', 'wms:adjust:export', '', 'admin', '2026-07-20 00:00:00', '', NULL, '', 0, 100028);
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, route_name, component, query, is_frame, is_cache, menu_type, visible, status, perms, icon, create_by, create_time, update_by, update_time, remark, is_del, system_id) VALUES (1140, '库存冻结', 1121, 4, 'freeze', NULL, 'wms/freeze/index', NULL, 1, 0, 'C', '0', '0', 'wms:freeze:list', 'snowflake', 'admin', '2026-07-20 00:00:00', '', NULL, '', 0, 100028);
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, route_name, component, query, is_frame, is_cache, menu_type, visible, status, perms, icon, create_by, create_time, update_by, update_time, remark, is_del, system_id) VALUES (1141, '查询', 1140, 1, '', NULL, '', NULL, 1, 0, 'F', '0', '0', 'wms:freeze:query', '', 'admin', '2026-07-20 00:00:00', '', NULL, '', 0, 100028);
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, route_name, component, query, is_frame, is_cache, menu_type, visible, status, perms, icon, create_by, create_time, update_by, update_time, remark, is_del, system_id) VALUES (1142, '新增', 1140, 2, '', NULL, '', NULL, 1, 0, 'F', '0', '0', 'wms:freeze:add', '', 'admin', '2026-07-20 00:00:00', '', NULL, '', 0, 100028);
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, route_name, component, query, is_frame, is_cache, menu_type, visible, status, perms, icon, create_by, create_time, update_by, update_time, remark, is_del, system_id) VALUES (1143, '修改', 1140, 3, '', NULL, '', NULL, 1, 0, 'F', '0', '0', 'wms:freeze:edit', '', 'admin', '2026-07-20 00:00:00', '', NULL, '', 0, 100028);
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, route_name, component, query, is_frame, is_cache, menu_type, visible, status, perms, icon, create_by, create_time, update_by, update_time, remark, is_del, system_id) VALUES (1144, '删除', 1140, 4, '', NULL, '', NULL, 1, 0, 'F', '0', '0', 'wms:freeze:remove', '', 'admin', '2026-07-20 00:00:00', '', NULL, '', 0, 100028);
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, route_name, component, query, is_frame, is_cache, menu_type, visible, status, perms, icon, create_by, create_time, update_by, update_time, remark, is_del, system_id) VALUES (1145, '导出', 1140, 5, '', NULL, '', NULL, 1, 0, 'F', '0', '0', 'wms:freeze:export', '', 'admin', '2026-07-20 00:00:00', '', NULL, '', 0, 100028);
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, route_name, component, query, is_frame, is_cache, menu_type, visible, status, perms, icon, create_by, create_time, update_by, update_time, remark, is_del, system_id) VALUES (1146, '库存锁定', 1121, 5, 'lock', NULL, 'wms/lock/index', NULL, 1, 0, 'C', '0', '0', 'wms:lock:list', 'lock', 'admin', '2026-07-20 00:00:00', '', NULL, '', 0, 100028);
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, route_name, component, query, is_frame, is_cache, menu_type, visible, status, perms, icon, create_by, create_time, update_by, update_time, remark, is_del, system_id) VALUES (1147, '查询', 1146, 1, '', NULL, '', NULL, 1, 0, 'F', '0', '0', 'wms:lock:query', '', 'admin', '2026-07-20 00:00:00', '', NULL, '', 0, 100028);
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, route_name, component, query, is_frame, is_cache, menu_type, visible, status, perms, icon, create_by, create_time, update_by, update_time, remark, is_del, system_id) VALUES (1148, '新增', 1146, 2, '', NULL, '', NULL, 1, 0, 'F', '0', '0', 'wms:lock:add', '', 'admin', '2026-07-20 00:00:00', '', NULL, '', 0, 100028);
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, route_name, component, query, is_frame, is_cache, menu_type, visible, status, perms, icon, create_by, create_time, update_by, update_time, remark, is_del, system_id) VALUES (1149, '修改', 1146, 3, '', NULL, '', NULL, 1, 0, 'F', '0', '0', 'wms:lock:edit', '', 'admin', '2026-07-20 00:00:00', '', NULL, '', 0, 100028);
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, route_name, component, query, is_frame, is_cache, menu_type, visible, status, perms, icon, create_by, create_time, update_by, update_time, remark, is_del, system_id) VALUES (1150, '删除', 1146, 4, '', NULL, '', NULL, 1, 0, 'F', '0', '0', 'wms:lock:remove', '', 'admin', '2026-07-20 00:00:00', '', NULL, '', 0, 100028);
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, route_name, component, query, is_frame, is_cache, menu_type, visible, status, perms, icon, create_by, create_time, update_by, update_time, remark, is_del, system_id) VALUES (1151, '导出', 1146, 5, '', NULL, '', NULL, 1, 0, 'F', '0', '0', 'wms:lock:export', '', 'admin', '2026-07-20 00:00:00', '', NULL, '', 0, 100028);
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, route_name, component, query, is_frame, is_cache, menu_type, visible, status, perms, icon, create_by, create_time, update_by, update_time, remark, is_del, system_id) VALUES (1152, '库存管理', 0, 8, 'stock', NULL, '', NULL, 1, 0, 'M', '0', '0', '', 'layer-group', 'admin', '2026-07-20 00:00:00', '', NULL, '', 0, 100028);
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, route_name, component, query, is_frame, is_cache, menu_type, visible, status, perms, icon, create_by, create_time, update_by, update_time, remark, is_del, system_id) VALUES (1153, '实时库存', 1152, 1, 'stock', NULL, 'wms/stock/index', NULL, 1, 0, 'C', '0', '0', 'wms:stock:list', 'boxes-stacked', 'admin', '2026-07-20 00:00:00', '', NULL, '', 0, 100028);
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, route_name, component, query, is_frame, is_cache, menu_type, visible, status, perms, icon, create_by, create_time, update_by, update_time, remark, is_del, system_id) VALUES (1154, '查询', 1153, 1, '', NULL, '', NULL, 1, 0, 'F', '0', '0', 'wms:stock:query', '', 'admin', '2026-07-20 00:00:00', '', NULL, '', 0, 100028);
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, route_name, component, query, is_frame, is_cache, menu_type, visible, status, perms, icon, create_by, create_time, update_by, update_time, remark, is_del, system_id) VALUES (1155, '新增', 1153, 2, '', NULL, '', NULL, 1, 0, 'F', '0', '0', 'wms:stock:add', '', 'admin', '2026-07-20 00:00:00', '', NULL, '', 0, 100028);
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, route_name, component, query, is_frame, is_cache, menu_type, visible, status, perms, icon, create_by, create_time, update_by, update_time, remark, is_del, system_id) VALUES (1156, '修改', 1153, 3, '', NULL, '', NULL, 1, 0, 'F', '0', '0', 'wms:stock:edit', '', 'admin', '2026-07-20 00:00:00', '', NULL, '', 0, 100028);
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, route_name, component, query, is_frame, is_cache, menu_type, visible, status, perms, icon, create_by, create_time, update_by, update_time, remark, is_del, system_id) VALUES (1157, '删除', 1153, 4, '', NULL, '', NULL, 1, 0, 'F', '0', '0', 'wms:stock:remove', '', 'admin', '2026-07-20 00:00:00', '', NULL, '', 0, 100028);
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, route_name, component, query, is_frame, is_cache, menu_type, visible, status, perms, icon, create_by, create_time, update_by, update_time, remark, is_del, system_id) VALUES (1158, '导出', 1153, 5, '', NULL, '', NULL, 1, 0, 'F', '0', '0', 'wms:stock:export', '', 'admin', '2026-07-20 00:00:00', '', NULL, '', 0, 100028);
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, route_name, component, query, is_frame, is_cache, menu_type, visible, status, perms, icon, create_by, create_time, update_by, update_time, remark, is_del, system_id) VALUES (1159, '库存流水', 1152, 2, 'stockRecord', NULL, 'wms/stockRecord/index', NULL, 1, 0, 'C', '0', '0', 'wms:stockRecord:list', 'receipt', 'admin', '2026-07-20 00:00:00', '', NULL, '', 0, 100028);
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, route_name, component, query, is_frame, is_cache, menu_type, visible, status, perms, icon, create_by, create_time, update_by, update_time, remark, is_del, system_id) VALUES (1160, '查询', 1159, 1, '', NULL, '', NULL, 1, 0, 'F', '0', '0', 'wms:stockRecord:query', '', 'admin', '2026-07-20 00:00:00', '', NULL, '', 0, 100028);
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, route_name, component, query, is_frame, is_cache, menu_type, visible, status, perms, icon, create_by, create_time, update_by, update_time, remark, is_del, system_id) VALUES (1161, '新增', 1159, 2, '', NULL, '', NULL, 1, 0, 'F', '0', '0', 'wms:stockRecord:add', '', 'admin', '2026-07-20 00:00:00', '', NULL, '', 0, 100028);
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, route_name, component, query, is_frame, is_cache, menu_type, visible, status, perms, icon, create_by, create_time, update_by, update_time, remark, is_del, system_id) VALUES (1162, '修改', 1159, 3, '', NULL, '', NULL, 1, 0, 'F', '0', '0', 'wms:stockRecord:edit', '', 'admin', '2026-07-20 00:00:00', '', NULL, '', 0, 100028);
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, route_name, component, query, is_frame, is_cache, menu_type, visible, status, perms, icon, create_by, create_time, update_by, update_time, remark, is_del, system_id) VALUES (1163, '删除', 1159, 4, '', NULL, '', NULL, 1, 0, 'F', '0', '0', 'wms:stockRecord:remove', '', 'admin', '2026-07-20 00:00:00', '', NULL, '', 0, 100028);
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, route_name, component, query, is_frame, is_cache, menu_type, visible, status, perms, icon, create_by, create_time, update_by, update_time, remark, is_del, system_id) VALUES (1164, '导出', 1159, 5, '', NULL, '', NULL, 1, 0, 'F', '0', '0', 'wms:stockRecord:export', '', 'admin', '2026-07-20 00:00:00', '', NULL, '', 0, 100028);
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, route_name, component, query, is_frame, is_cache, menu_type, visible, status, perms, icon, create_by, create_time, update_by, update_time, remark, is_del, system_id) VALUES (1165, '库存预警', 1152, 3, 'warning', NULL, 'wms/stockWarning/index', NULL, 1, 0, 'C', '0', '0', 'wms:stockWarning:list', 'triangle-exclamation', 'admin', '2026-07-20 00:00:00', 'admin', '2026-11-11 21:57:35', '', 0, 100028);
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, route_name, component, query, is_frame, is_cache, menu_type, visible, status, perms, icon, create_by, create_time, update_by, update_time, remark, is_del, system_id) VALUES (1166, '查询', 1165, 1, '', NULL, '', NULL, 1, 0, 'F', '0', '0', 'wms:stockWarning:query', '', 'admin', '2026-07-20 00:00:00', '', NULL, '', 0, 100028);
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, route_name, component, query, is_frame, is_cache, menu_type, visible, status, perms, icon, create_by, create_time, update_by, update_time, remark, is_del, system_id) VALUES (1167, '新增', 1165, 2, '', NULL, '', NULL, 1, 0, 'F', '0', '0', 'wms:stockWarning:add', '', 'admin', '2026-07-20 00:00:00', '', NULL, '', 0, 100028);
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, route_name, component, query, is_frame, is_cache, menu_type, visible, status, perms, icon, create_by, create_time, update_by, update_time, remark, is_del, system_id) VALUES (1168, '修改', 1165, 3, '', NULL, '', NULL, 1, 0, 'F', '0', '0', 'wms:stockWarning:edit', '', 'admin', '2026-07-20 00:00:00', '', NULL, '', 0, 100028);
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, route_name, component, query, is_frame, is_cache, menu_type, visible, status, perms, icon, create_by, create_time, update_by, update_time, remark, is_del, system_id) VALUES (1169, '删除', 1165, 4, '', NULL, '', NULL, 1, 0, 'F', '0', '0', 'wms:stockWarning:remove', '', 'admin', '2026-07-20 00:00:00', '', NULL, '', 0, 100028);
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, route_name, component, query, is_frame, is_cache, menu_type, visible, status, perms, icon, create_by, create_time, update_by, update_time, remark, is_del, system_id) VALUES (1170, '导出', 1165, 5, '', NULL, '', NULL, 1, 0, 'F', '0', '0', 'wms:stockWarning:export', '', 'admin', '2026-07-20 00:00:00', '', NULL, '', 0, 100028);
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, route_name, component, query, is_frame, is_cache, menu_type, visible, status, perms, icon, create_by, create_time, update_by, update_time, remark, is_del, system_id) VALUES (1177, '处理单元', 1152, 5, 'hu', NULL, 'wms/hu/index', NULL, 1, 0, 'C', '1', '0', 'wms:hu:list', 'pallet', 'admin', '2026-07-20 00:00:00', '', NULL, '', 0, 100028);
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, route_name, component, query, is_frame, is_cache, menu_type, visible, status, perms, icon, create_by, create_time, update_by, update_time, remark, is_del, system_id) VALUES (1178, '查询', 1177, 1, '', NULL, '', NULL, 1, 0, 'F', '0', '0', 'wms:hu:query', '', 'admin', '2026-07-20 00:00:00', '', NULL, '', 0, 100028);
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, route_name, component, query, is_frame, is_cache, menu_type, visible, status, perms, icon, create_by, create_time, update_by, update_time, remark, is_del, system_id) VALUES (1179, '新增', 1177, 2, '', NULL, '', NULL, 1, 0, 'F', '0', '0', 'wms:hu:add', '', 'admin', '2026-07-20 00:00:00', '', NULL, '', 0, 100028);
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, route_name, component, query, is_frame, is_cache, menu_type, visible, status, perms, icon, create_by, create_time, update_by, update_time, remark, is_del, system_id) VALUES (1180, '修改', 1177, 3, '', NULL, '', NULL, 1, 0, 'F', '0', '0', 'wms:hu:edit', '', 'admin', '2026-07-20 00:00:00', '', NULL, '', 0, 100028);
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, route_name, component, query, is_frame, is_cache, menu_type, visible, status, perms, icon, create_by, create_time, update_by, update_time, remark, is_del, system_id) VALUES (1181, '删除', 1177, 4, '', NULL, '', NULL, 1, 0, 'F', '0', '0', 'wms:hu:remove', '', 'admin', '2026-07-20 00:00:00', '', NULL, '', 0, 100028);
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, route_name, component, query, is_frame, is_cache, menu_type, visible, status, perms, icon, create_by, create_time, update_by, update_time, remark, is_del, system_id) VALUES (1182, '导出', 1177, 5, '', NULL, '', NULL, 1, 0, 'F', '0', '0', 'wms:hu:export', '', 'admin', '2026-07-20 00:00:00', '', NULL, '', 0, 100028);
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, route_name, component, query, is_frame, is_cache, menu_type, visible, status, perms, icon, create_by, create_time, update_by, update_time, remark, is_del, system_id) VALUES (1183, '来料不良台账', 1059, 10, 'iqcDefect', NULL, 'wms/iqcDefect/index', NULL, 1, 0, 'C', '0', '0', 'wms:iqcDefect:list', 'triangle-exclamation', 'admin', '2026-10-20 22:55:54', 'admin', '2026-10-20 23:17:03', '', 0, 100028);
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, route_name, component, query, is_frame, is_cache, menu_type, visible, status, perms, icon, create_by, create_time, update_by, update_time, remark, is_del, system_id) VALUES (1400, '出库申请', 1090, 2, 'issueOrder', NULL, 'wms/issueOrder/index', NULL, 1, 0, 'C', '0', '0', 'wms:issueOrder:list', 'box', 'admin', '2026-10-23 12:06:41', '', NULL, '', 0, 100028);
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, route_name, component, query, is_frame, is_cache, menu_type, visible, status, perms, icon, create_by, create_time, update_by, update_time, remark, is_del, system_id) VALUES (1401, '缺料看板', 1090, 7, 'shortage', NULL, 'wms/shortage/index', NULL, 1, 0, 'C', '0', '0', 'wms:shortage:list', 'warning', 'admin', '2026-10-23 12:06:41', '', NULL, '', 0, 100028);
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, route_name, component, query, is_frame, is_cache, menu_type, visible, status, perms, icon, create_by, create_time, update_by, update_time, remark, is_del, system_id) VALUES (1402, '跨库调拨', 1121, 2, 'transfer', NULL, 'wms/transfer/index', NULL, 1, 0, 'C', '0', '0', 'wms:transfer:list', 'share', 'admin', '2026-10-23 16:10:54', 'admin', '2026-11-16 22:15:38', '', 0, 100028);
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, route_name, component, query, is_frame, is_cache, menu_type, visible, status, perms, icon, create_by, create_time, update_by, update_time, remark, is_del, system_id) VALUES (1403, '查询', 1402, 1, '', NULL, '', NULL, 1, 0, 'F', '0', '0', 'wms:transfer:query', '', 'admin', '2026-10-23 16:10:54', '', NULL, '', 0, 100028);
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, route_name, component, query, is_frame, is_cache, menu_type, visible, status, perms, icon, create_by, create_time, update_by, update_time, remark, is_del, system_id) VALUES (1404, '新增', 1402, 2, '', NULL, '', NULL, 1, 0, 'F', '0', '0', 'wms:transfer:add', '', 'admin', '2026-10-23 16:10:54', '', NULL, '', 0, 100028);
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, route_name, component, query, is_frame, is_cache, menu_type, visible, status, perms, icon, create_by, create_time, update_by, update_time, remark, is_del, system_id) VALUES (1405, '修改', 1402, 3, '', NULL, '', NULL, 1, 0, 'F', '0', '0', 'wms:transfer:edit', '', 'admin', '2026-10-23 16:10:54', '', NULL, '', 0, 100028);
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, route_name, component, query, is_frame, is_cache, menu_type, visible, status, perms, icon, create_by, create_time, update_by, update_time, remark, is_del, system_id) VALUES (1406, '删除', 1402, 4, '', NULL, '', NULL, 1, 0, 'F', '0', '0', 'wms:transfer:remove', '', 'admin', '2026-10-23 16:10:54', '', NULL, '', 0, 100028);
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, route_name, component, query, is_frame, is_cache, menu_type, visible, status, perms, icon, create_by, create_time, update_by, update_time, remark, is_del, system_id) VALUES (1407, '移出', 1402, 5, '', NULL, '', NULL, 1, 0, 'F', '0', '0', 'wms:transfer:out', '', 'admin', '2026-10-23 16:10:54', '', NULL, '', 0, 100028);
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, route_name, component, query, is_frame, is_cache, menu_type, visible, status, perms, icon, create_by, create_time, update_by, update_time, remark, is_del, system_id) VALUES (1408, '移入', 1402, 6, '', NULL, '', NULL, 1, 0, 'F', '0', '0', 'wms:transfer:in', '', 'admin', '2026-10-23 16:10:54', '', NULL, '', 0, 100028);
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, route_name, component, query, is_frame, is_cache, menu_type, visible, status, perms, icon, create_by, create_time, update_by, update_time, remark, is_del, system_id) VALUES (1409, '导出', 1402, 7, '', NULL, '', NULL, 1, 0, 'F', '0', '0', 'wms:transfer:export', '', 'admin', '2026-10-23 16:10:54', '', NULL, '', 0, 100028);
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, route_name, component, query, is_frame, is_cache, menu_type, visible, status, perms, icon, create_by, create_time, update_by, update_time, remark, is_del, system_id) VALUES (1410, '查询', 1400, 1, '', NULL, '', NULL, 1, 0, 'F', '0', '0', 'wms:issueOrder:query', '#', 'admin', '2026-10-23 12:06:41', '', NULL, '', 0, 100028);
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, route_name, component, query, is_frame, is_cache, menu_type, visible, status, perms, icon, create_by, create_time, update_by, update_time, remark, is_del, system_id) VALUES (1411, '新增', 1400, 2, '', NULL, '', NULL, 1, 0, 'F', '0', '0', 'wms:issueOrder:add', '#', 'admin', '2026-10-23 12:06:41', '', NULL, '', 0, 100028);
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, route_name, component, query, is_frame, is_cache, menu_type, visible, status, perms, icon, create_by, create_time, update_by, update_time, remark, is_del, system_id) VALUES (1412, '修改', 1400, 3, '', NULL, '', NULL, 1, 0, 'F', '0', '0', 'wms:issueOrder:edit', '#', 'admin', '2026-10-23 12:06:41', '', NULL, '', 0, 100028);
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, route_name, component, query, is_frame, is_cache, menu_type, visible, status, perms, icon, create_by, create_time, update_by, update_time, remark, is_del, system_id) VALUES (1413, '删除', 1400, 4, '', NULL, '', NULL, 1, 0, 'F', '0', '0', 'wms:issueOrder:remove', '#', 'admin', '2026-10-23 12:06:41', '', NULL, '', 0, 100028);
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, route_name, component, query, is_frame, is_cache, menu_type, visible, status, perms, icon, create_by, create_time, update_by, update_time, remark, is_del, system_id) VALUES (1414, '发料', 1400, 5, '', NULL, '', NULL, 1, 0, 'F', '0', '0', 'wms:issueOrder:issue', '#', 'admin', '2026-10-23 12:06:41', '', NULL, '', 0, 100028);
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, route_name, component, query, is_frame, is_cache, menu_type, visible, status, perms, icon, create_by, create_time, update_by, update_time, remark, is_del, system_id) VALUES (1415, '导出', 1400, 6, '', NULL, '', NULL, 1, 0, 'F', '0', '0', 'wms:issueOrder:export', '#', 'admin', '2026-10-23 12:06:41', '', NULL, '', 0, 100028);
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, route_name, component, query, is_frame, is_cache, menu_type, visible, status, perms, icon, create_by, create_time, update_by, update_time, remark, is_del, system_id) VALUES (1420, '查询', 1401, 1, '', NULL, '', NULL, 1, 0, 'F', '0', '0', 'wms:shortage:query', '#', 'admin', '2026-10-23 12:06:41', '', NULL, '', 0, 100028);
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, route_name, component, query, is_frame, is_cache, menu_type, visible, status, perms, icon, create_by, create_time, update_by, update_time, remark, is_del, system_id) VALUES (179398067889600, '预警配置', 1045, 99, 'stockWarningConfig', 'wms_stock_warning_config', 'wms/stockWarningConfig/index', NULL, 1, 0, 'C', '0', '0', 'wms:stockWarningConfig:list', 'warning', 'admin', '2026-11-06 23:57:58', 'admin', '2026-11-07 00:28:54', '', 0, 100028);
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, route_name, component, query, is_frame, is_cache, menu_type, visible, status, perms, icon, create_by, create_time, update_by, update_time, remark, is_del, system_id) VALUES (179398067889601, '查询', 179398067889600, 1, '', NULL, '', NULL, 1, 0, 'F', '0', '0', 'wms:stockWarningConfig:query', '#', 'admin', '2026-11-06 23:57:58', '', NULL, '', 0, 100028);
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, route_name, component, query, is_frame, is_cache, menu_type, visible, status, perms, icon, create_by, create_time, update_by, update_time, remark, is_del, system_id) VALUES (179398067889602, '新增', 179398067889600, 2, '', NULL, '', NULL, 1, 0, 'F', '0', '0', 'wms:stockWarningConfig:add', '#', 'admin', '2026-11-06 23:57:58', '', NULL, '', 0, 100028);
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, route_name, component, query, is_frame, is_cache, menu_type, visible, status, perms, icon, create_by, create_time, update_by, update_time, remark, is_del, system_id) VALUES (179398067889603, '修改', 179398067889600, 3, '', NULL, '', NULL, 1, 0, 'F', '0', '0', 'wms:stockWarningConfig:edit', '#', 'admin', '2026-11-06 23:57:58', '', NULL, '', 0, 100028);
INSERT INTO sys_menu (menu_id, menu_name, parent_id, order_num, path, route_name, component, query, is_frame, is_cache, menu_type, visible, status, perms, icon, create_by, create_time, update_by, update_time, remark, is_del, system_id) VALUES (179398067889604, '删除', 179398067889600, 4, '', NULL, '', NULL, 1, 0, 'F', '0', '0', 'wms:stockWarningConfig:remove', '#', 'admin', '2026-11-06 23:57:58', '', NULL, '', 0, 100028);


-- ---------------------------- sys_post ----------------------------
DROP TABLE IF EXISTS sys_post;
CREATE TABLE sys_post (
  "post_id" bigint NOT NULL,
  "post_code" varchar(64) NOT NULL,
  "post_name" varchar(50) NOT NULL,
  "post_sort" integer NOT NULL,
  "status" varchar(1) NOT NULL,
  "is_del" integer DEFAULT 0,
  "create_by" varchar(64) DEFAULT '',
  "create_time" timestamp DEFAULT NULL,
  "update_by" varchar(64) DEFAULT '',
  "update_time" timestamp DEFAULT NULL,
  "remark" varchar(500) DEFAULT NULL,
  PRIMARY KEY (post_id)
);
COMMENT ON COLUMN sys_post.post_id IS '岗位ID';
COMMENT ON COLUMN sys_post.post_code IS '岗位编码';
COMMENT ON COLUMN sys_post.post_name IS '岗位名称';
COMMENT ON COLUMN sys_post.post_sort IS '显示顺序';
COMMENT ON COLUMN sys_post.status IS '状态（0正常 1停用）';
COMMENT ON COLUMN sys_post.is_del IS '删除标志（0代表存在 1代表删除）';
COMMENT ON COLUMN sys_post.create_by IS '创建者';
COMMENT ON COLUMN sys_post.create_time IS '创建时间';
COMMENT ON COLUMN sys_post.update_by IS '更新者';
COMMENT ON COLUMN sys_post.update_time IS '更新时间';
COMMENT ON COLUMN sys_post.remark IS '备注';
DELETE FROM sys_post;
INSERT INTO sys_post (post_id, post_code, post_name, post_sort, status, is_del, create_by, create_time, update_by, update_time, remark) VALUES (1, 'ceo', '董事长', 1, '0', 0, 'admin', '2022-04-15 15:51:01', 'admin', '2024-11-01 23:08:50', '');
INSERT INTO sys_post (post_id, post_code, post_name, post_sort, status, is_del, create_by, create_time, update_by, update_time, remark) VALUES (2, 'se', '项目经理', 2, '0', 0, 'admin', '2022-04-15 15:51:01', '', NULL, '');
INSERT INTO sys_post (post_id, post_code, post_name, post_sort, status, is_del, create_by, create_time, update_by, update_time, remark) VALUES (3, 'hr', '人力资源', 3, '0', 0, 'admin', '2022-04-15 15:51:01', '', NULL, '');
INSERT INTO sys_post (post_id, post_code, post_name, post_sort, status, is_del, create_by, create_time, update_by, update_time, remark) VALUES (4, 'user', '普通员工', 4, '0', 0, 'admin', '2022-04-15 15:51:01', '', NULL, '');


-- ---------------------------- sys_role ----------------------------
DROP TABLE IF EXISTS sys_role;
CREATE TABLE sys_role (
  "role_id" bigint NOT NULL,
  "role_name" varchar(30) NOT NULL,
  "role_key" varchar(100) NOT NULL,
  "role_sort" integer NOT NULL,
  "data_scope" varchar(1) DEFAULT NULL,
  "menu_check_strictly" smallint DEFAULT 1,
  "dept_check_strictly" smallint DEFAULT 1,
  "status" varchar(1) NOT NULL,
  "is_del" integer DEFAULT 0,
  "create_by" varchar(64) DEFAULT '',
  "create_time" timestamp DEFAULT NULL,
  "update_by" varchar(64) DEFAULT '',
  "update_time" timestamp DEFAULT NULL,
  "remark" varchar(500) DEFAULT NULL,
  PRIMARY KEY (role_id)
);
COMMENT ON COLUMN sys_role.role_id IS '角色ID';
COMMENT ON COLUMN sys_role.role_name IS '角色名称';
COMMENT ON COLUMN sys_role.role_key IS '角色权限字符串';
COMMENT ON COLUMN sys_role.role_sort IS '显示顺序';
COMMENT ON COLUMN sys_role.data_scope IS '数据范围（1：全部数据权限 2：自定数据权限 3：本部门数据权限 4：本部门及以下数据权限）';
COMMENT ON COLUMN sys_role.menu_check_strictly IS '菜单树选择项是否关联显示';
COMMENT ON COLUMN sys_role.dept_check_strictly IS '部门树选择项是否关联显示';
COMMENT ON COLUMN sys_role.status IS '角色状态（0正常 1停用）';
COMMENT ON COLUMN sys_role.is_del IS '删除标志（0代表存在 1代表删除）';
COMMENT ON COLUMN sys_role.create_by IS '创建者';
COMMENT ON COLUMN sys_role.create_time IS '创建时间';
COMMENT ON COLUMN sys_role.update_by IS '更新者';
COMMENT ON COLUMN sys_role.update_time IS '更新时间';
COMMENT ON COLUMN sys_role.remark IS '备注';
DELETE FROM sys_role;
INSERT INTO sys_role (role_id, role_name, role_key, role_sort, data_scope, menu_check_strictly, dept_check_strictly, status, is_del, create_by, create_time, update_by, update_time, remark) VALUES (1, '超级管理员', 'admin', 1, '1', 1, 1, '0', 0, 'admin', '2022-04-15 15:51:01', 'admin', '2022-05-27 23:55:48', '超级管理员');
INSERT INTO sys_role (role_id, role_name, role_key, role_sort, data_scope, menu_check_strictly, dept_check_strictly, status, is_del, create_by, create_time, update_by, update_time, remark) VALUES (2, '普通角色', 'common', 2, '2', 1, 1, '0', 0, 'admin', '2022-04-15 15:51:01', 'admin', '2022-11-11 10:12:06', '普通角色');
INSERT INTO sys_role (role_id, role_name, role_key, role_sort, data_scope, menu_check_strictly, dept_check_strictly, status, is_del, create_by, create_time, update_by, update_time, remark) VALUES (3, 'ceshi', 'ceshi', 0, NULL, 1, 1, '0', 1, 'admin', '2022-10-20 09:44:38', 'admin', '2022-10-20 09:44:46', NULL);
INSERT INTO sys_role (role_id, role_name, role_key, role_sort, data_scope, menu_check_strictly, dept_check_strictly, status, is_del, create_by, create_time, update_by, update_time, remark) VALUES (4, '仓管1', '1', 0, NULL, 1, 1, '0', 1, 'admin', '2022-10-21 14:01:07', 'admin', '2022-10-21 14:12:27', NULL);
INSERT INTO sys_role (role_id, role_name, role_key, role_sort, data_scope, menu_check_strictly, dept_check_strictly, status, is_del, create_by, create_time, update_by, update_time, remark) VALUES (5, '拣货', '拣货', 0, NULL, 1, 1, '0', 1, 'admin', '2022-10-21 14:13:09', 'admin', '2022-10-21 14:14:56', NULL);
INSERT INTO sys_role (role_id, role_name, role_key, role_sort, data_scope, menu_check_strictly, dept_check_strictly, status, is_del, create_by, create_time, update_by, update_time, remark) VALUES (6, 'test-bug', '2', 1, NULL, 1, 1, '0', 1, 'admin', '2022-10-21 19:52:10', NULL, NULL, NULL);
INSERT INTO sys_role (role_id, role_name, role_key, role_sort, data_scope, menu_check_strictly, dept_check_strictly, status, is_del, create_by, create_time, update_by, update_time, remark) VALUES (7, 'ceshi1', '11', 2, NULL, 1, 1, '0', 1, 'admin', '2022-10-23 21:11:51', 'admin', '2022-10-23 21:12:42', NULL);
INSERT INTO sys_role (role_id, role_name, role_key, role_sort, data_scope, menu_check_strictly, dept_check_strictly, status, is_del, create_by, create_time, update_by, update_time, remark) VALUES (8, 'ceshi3', '3', 0, NULL, 1, 1, '0', 1, 'admin', '2022-10-23 21:16:28', NULL, NULL, '4');
INSERT INTO sys_role (role_id, role_name, role_key, role_sort, data_scope, menu_check_strictly, dept_check_strictly, status, is_del, create_by, create_time, update_by, update_time, remark) VALUES (9, '测试0001', '4', 1, NULL, 1, 1, '0', 1, 'admin', '2022-10-24 17:06:17', NULL, NULL, NULL);
INSERT INTO sys_role (role_id, role_name, role_key, role_sort, data_scope, menu_check_strictly, dept_check_strictly, status, is_del, create_by, create_time, update_by, update_time, remark) VALUES (10, '销售组别', '9', 0, NULL, 1, 1, '0', 1, 'admin', '2022-10-25 14:13:26', NULL, NULL, NULL);
INSERT INTO sys_role (role_id, role_name, role_key, role_sort, data_scope, menu_check_strictly, dept_check_strictly, status, is_del, create_by, create_time, update_by, update_time, remark) VALUES (11, '普通管理员', '3', 3, '2', 1, 1, '0', 0, 'admin', '2022-11-11 13:53:22', 'admin', '2026-06-22 21:56:46', NULL);
INSERT INTO sys_role (role_id, role_name, role_key, role_sort, data_scope, menu_check_strictly, dept_check_strictly, status, is_del, create_by, create_time, update_by, update_time, remark) VALUES (12, '流程测试', 'lccs', 0, NULL, 1, 1, '0', 0, 'admin', '2023-08-10 22:16:26', NULL, NULL, NULL);
INSERT INTO sys_role (role_id, role_name, role_key, role_sort, data_scope, menu_check_strictly, dept_check_strictly, status, is_del, create_by, create_time, update_by, update_time, remark) VALUES (13, 'a', '1', 0, NULL, 1, 1, '0', 1, 'admin', '2024-11-01 22:52:01', NULL, NULL, NULL);


-- ---------------------------- sys_role_dept ----------------------------
DROP TABLE IF EXISTS sys_role_dept;
CREATE TABLE sys_role_dept (
  "role_id" bigint NOT NULL,
  "dept_id" bigint NOT NULL,
  PRIMARY KEY (role_id,dept_id)
);
COMMENT ON COLUMN sys_role_dept.role_id IS '角色ID';
COMMENT ON COLUMN sys_role_dept.dept_id IS '部门ID';
DELETE FROM sys_role_dept;
INSERT INTO sys_role_dept (role_id, dept_id) VALUES (2, 100);
INSERT INTO sys_role_dept (role_id, dept_id) VALUES (2, 101);
INSERT INTO sys_role_dept (role_id, dept_id) VALUES (2, 102);
INSERT INTO sys_role_dept (role_id, dept_id) VALUES (2, 103);
INSERT INTO sys_role_dept (role_id, dept_id) VALUES (2, 104);
INSERT INTO sys_role_dept (role_id, dept_id) VALUES (2, 105);
INSERT INTO sys_role_dept (role_id, dept_id) VALUES (2, 106);
INSERT INTO sys_role_dept (role_id, dept_id) VALUES (2, 107);
INSERT INTO sys_role_dept (role_id, dept_id) VALUES (2, 108);
INSERT INTO sys_role_dept (role_id, dept_id) VALUES (2, 109);
INSERT INTO sys_role_dept (role_id, dept_id) VALUES (11, 100);
INSERT INTO sys_role_dept (role_id, dept_id) VALUES (11, 101);
INSERT INTO sys_role_dept (role_id, dept_id) VALUES (11, 103);
INSERT INTO sys_role_dept (role_id, dept_id) VALUES (11, 104);
INSERT INTO sys_role_dept (role_id, dept_id) VALUES (11, 105);
INSERT INTO sys_role_dept (role_id, dept_id) VALUES (11, 106);
INSERT INTO sys_role_dept (role_id, dept_id) VALUES (11, 107);


-- ---------------------------- sys_role_menu ----------------------------
DROP TABLE IF EXISTS sys_role_menu;
CREATE TABLE sys_role_menu (
  "role_id" bigint NOT NULL,
  "menu_id" bigint NOT NULL,
  PRIMARY KEY (role_id,menu_id)
);
COMMENT ON COLUMN sys_role_menu.role_id IS '角色ID';
COMMENT ON COLUMN sys_role_menu.menu_id IS '菜单ID';
DELETE FROM sys_role_menu;
INSERT INTO sys_role_menu (role_id, menu_id) VALUES (1, 1001);
INSERT INTO sys_role_menu (role_id, menu_id) VALUES (1, 1002);
INSERT INTO sys_role_menu (role_id, menu_id) VALUES (1, 1003);
INSERT INTO sys_role_menu (role_id, menu_id) VALUES (1, 1004);
INSERT INTO sys_role_menu (role_id, menu_id) VALUES (1, 1005);
INSERT INTO sys_role_menu (role_id, menu_id) VALUES (1, 1006);
INSERT INTO sys_role_menu (role_id, menu_id) VALUES (1, 1007);
INSERT INTO sys_role_menu (role_id, menu_id) VALUES (1, 1008);
INSERT INTO sys_role_menu (role_id, menu_id) VALUES (1, 1009);
INSERT INTO sys_role_menu (role_id, menu_id) VALUES (1, 1010);
INSERT INTO sys_role_menu (role_id, menu_id) VALUES (1, 1011);
INSERT INTO sys_role_menu (role_id, menu_id) VALUES (1, 1012);
INSERT INTO sys_role_menu (role_id, menu_id) VALUES (1, 1013);
INSERT INTO sys_role_menu (role_id, menu_id) VALUES (1, 1014);
INSERT INTO sys_role_menu (role_id, menu_id) VALUES (1, 1015);
INSERT INTO sys_role_menu (role_id, menu_id) VALUES (1, 1016);
INSERT INTO sys_role_menu (role_id, menu_id) VALUES (1, 1017);
INSERT INTO sys_role_menu (role_id, menu_id) VALUES (1, 1018);
INSERT INTO sys_role_menu (role_id, menu_id) VALUES (1, 1019);
INSERT INTO sys_role_menu (role_id, menu_id) VALUES (1, 1020);
INSERT INTO sys_role_menu (role_id, menu_id) VALUES (1, 1021);
INSERT INTO sys_role_menu (role_id, menu_id) VALUES (1, 1022);
INSERT INTO sys_role_menu (role_id, menu_id) VALUES (1, 1023);
INSERT INTO sys_role_menu (role_id, menu_id) VALUES (1, 1024);
INSERT INTO sys_role_menu (role_id, menu_id) VALUES (1, 1025);
INSERT INTO sys_role_menu (role_id, menu_id) VALUES (1, 1026);
INSERT INTO sys_role_menu (role_id, menu_id) VALUES (1, 1027);
INSERT INTO sys_role_menu (role_id, menu_id) VALUES (1, 1028);
INSERT INTO sys_role_menu (role_id, menu_id) VALUES (1, 1029);
INSERT INTO sys_role_menu (role_id, menu_id) VALUES (1, 1030);
INSERT INTO sys_role_menu (role_id, menu_id) VALUES (1, 1031);
INSERT INTO sys_role_menu (role_id, menu_id) VALUES (1, 1032);
INSERT INTO sys_role_menu (role_id, menu_id) VALUES (1, 1033);
INSERT INTO sys_role_menu (role_id, menu_id) VALUES (1, 1034);
INSERT INTO sys_role_menu (role_id, menu_id) VALUES (1, 1035);
INSERT INTO sys_role_menu (role_id, menu_id) VALUES (1, 1036);
INSERT INTO sys_role_menu (role_id, menu_id) VALUES (1, 1037);
INSERT INTO sys_role_menu (role_id, menu_id) VALUES (1, 1038);
INSERT INTO sys_role_menu (role_id, menu_id) VALUES (1, 1039);
INSERT INTO sys_role_menu (role_id, menu_id) VALUES (1, 1040);
INSERT INTO sys_role_menu (role_id, menu_id) VALUES (1, 1041);
INSERT INTO sys_role_menu (role_id, menu_id) VALUES (1, 1042);
INSERT INTO sys_role_menu (role_id, menu_id) VALUES (1, 1043);
INSERT INTO sys_role_menu (role_id, menu_id) VALUES (1, 1044);
INSERT INTO sys_role_menu (role_id, menu_id) VALUES (1, 1045);
INSERT INTO sys_role_menu (role_id, menu_id) VALUES (1, 1046);
INSERT INTO sys_role_menu (role_id, menu_id) VALUES (1, 1047);
INSERT INTO sys_role_menu (role_id, menu_id) VALUES (1, 1048);
INSERT INTO sys_role_menu (role_id, menu_id) VALUES (1, 1049);
INSERT INTO sys_role_menu (role_id, menu_id) VALUES (1, 1050);
INSERT INTO sys_role_menu (role_id, menu_id) VALUES (1, 1051);
INSERT INTO sys_role_menu (role_id, menu_id) VALUES (1, 1052);
INSERT INTO sys_role_menu (role_id, menu_id) VALUES (1, 1053);
INSERT INTO sys_role_menu (role_id, menu_id) VALUES (1, 1054);
INSERT INTO sys_role_menu (role_id, menu_id) VALUES (1, 1055);
INSERT INTO sys_role_menu (role_id, menu_id) VALUES (1, 1056);
INSERT INTO sys_role_menu (role_id, menu_id) VALUES (1, 1057);
INSERT INTO sys_role_menu (role_id, menu_id) VALUES (1, 1058);
INSERT INTO sys_role_menu (role_id, menu_id) VALUES (1, 1059);
INSERT INTO sys_role_menu (role_id, menu_id) VALUES (1, 1060);
INSERT INTO sys_role_menu (role_id, menu_id) VALUES (1, 1061);
INSERT INTO sys_role_menu (role_id, menu_id) VALUES (1, 1062);
INSERT INTO sys_role_menu (role_id, menu_id) VALUES (1, 1063);
INSERT INTO sys_role_menu (role_id, menu_id) VALUES (1, 1064);
INSERT INTO sys_role_menu (role_id, menu_id) VALUES (1, 1065);
INSERT INTO sys_role_menu (role_id, menu_id) VALUES (1, 1066);
INSERT INTO sys_role_menu (role_id, menu_id) VALUES (1, 1067);
INSERT INTO sys_role_menu (role_id, menu_id) VALUES (1, 1068);
INSERT INTO sys_role_menu (role_id, menu_id) VALUES (1, 1069);
INSERT INTO sys_role_menu (role_id, menu_id) VALUES (1, 1070);
INSERT INTO sys_role_menu (role_id, menu_id) VALUES (1, 1071);
INSERT INTO sys_role_menu (role_id, menu_id) VALUES (1, 1072);
INSERT INTO sys_role_menu (role_id, menu_id) VALUES (1, 1073);
INSERT INTO sys_role_menu (role_id, menu_id) VALUES (1, 1074);
INSERT INTO sys_role_menu (role_id, menu_id) VALUES (1, 1075);
INSERT INTO sys_role_menu (role_id, menu_id) VALUES (1, 1076);
INSERT INTO sys_role_menu (role_id, menu_id) VALUES (1, 1077);
INSERT INTO sys_role_menu (role_id, menu_id) VALUES (1, 1084);
INSERT INTO sys_role_menu (role_id, menu_id) VALUES (1, 1085);
INSERT INTO sys_role_menu (role_id, menu_id) VALUES (1, 1086);
INSERT INTO sys_role_menu (role_id, menu_id) VALUES (1, 1087);
INSERT INTO sys_role_menu (role_id, menu_id) VALUES (1, 1088);
INSERT INTO sys_role_menu (role_id, menu_id) VALUES (1, 1089);
INSERT INTO sys_role_menu (role_id, menu_id) VALUES (1, 1090);
INSERT INTO sys_role_menu (role_id, menu_id) VALUES (1, 1109);
INSERT INTO sys_role_menu (role_id, menu_id) VALUES (1, 1110);
INSERT INTO sys_role_menu (role_id, menu_id) VALUES (1, 1111);
INSERT INTO sys_role_menu (role_id, menu_id) VALUES (1, 1112);
INSERT INTO sys_role_menu (role_id, menu_id) VALUES (1, 1113);
INSERT INTO sys_role_menu (role_id, menu_id) VALUES (1, 1114);
INSERT INTO sys_role_menu (role_id, menu_id) VALUES (1, 1115);
INSERT INTO sys_role_menu (role_id, menu_id) VALUES (1, 1116);
INSERT INTO sys_role_menu (role_id, menu_id) VALUES (1, 1117);
INSERT INTO sys_role_menu (role_id, menu_id) VALUES (1, 1118);
INSERT INTO sys_role_menu (role_id, menu_id) VALUES (1, 1119);
INSERT INTO sys_role_menu (role_id, menu_id) VALUES (1, 1120);
INSERT INTO sys_role_menu (role_id, menu_id) VALUES (1, 1121);
INSERT INTO sys_role_menu (role_id, menu_id) VALUES (1, 1122);
INSERT INTO sys_role_menu (role_id, menu_id) VALUES (1, 1123);
INSERT INTO sys_role_menu (role_id, menu_id) VALUES (1, 1124);
INSERT INTO sys_role_menu (role_id, menu_id) VALUES (1, 1125);
INSERT INTO sys_role_menu (role_id, menu_id) VALUES (1, 1126);
INSERT INTO sys_role_menu (role_id, menu_id) VALUES (1, 1127);
INSERT INTO sys_role_menu (role_id, menu_id) VALUES (1, 1134);
INSERT INTO sys_role_menu (role_id, menu_id) VALUES (1, 1135);
INSERT INTO sys_role_menu (role_id, menu_id) VALUES (1, 1136);
INSERT INTO sys_role_menu (role_id, menu_id) VALUES (1, 1137);
INSERT INTO sys_role_menu (role_id, menu_id) VALUES (1, 1138);
INSERT INTO sys_role_menu (role_id, menu_id) VALUES (1, 1139);
INSERT INTO sys_role_menu (role_id, menu_id) VALUES (1, 1140);
INSERT INTO sys_role_menu (role_id, menu_id) VALUES (1, 1141);
INSERT INTO sys_role_menu (role_id, menu_id) VALUES (1, 1142);
INSERT INTO sys_role_menu (role_id, menu_id) VALUES (1, 1143);
INSERT INTO sys_role_menu (role_id, menu_id) VALUES (1, 1144);
INSERT INTO sys_role_menu (role_id, menu_id) VALUES (1, 1145);
INSERT INTO sys_role_menu (role_id, menu_id) VALUES (1, 1146);
INSERT INTO sys_role_menu (role_id, menu_id) VALUES (1, 1147);
INSERT INTO sys_role_menu (role_id, menu_id) VALUES (1, 1148);
INSERT INTO sys_role_menu (role_id, menu_id) VALUES (1, 1149);
INSERT INTO sys_role_menu (role_id, menu_id) VALUES (1, 1150);
INSERT INTO sys_role_menu (role_id, menu_id) VALUES (1, 1151);
INSERT INTO sys_role_menu (role_id, menu_id) VALUES (1, 1152);
INSERT INTO sys_role_menu (role_id, menu_id) VALUES (1, 1153);
INSERT INTO sys_role_menu (role_id, menu_id) VALUES (1, 1154);
INSERT INTO sys_role_menu (role_id, menu_id) VALUES (1, 1155);
INSERT INTO sys_role_menu (role_id, menu_id) VALUES (1, 1156);
INSERT INTO sys_role_menu (role_id, menu_id) VALUES (1, 1157);
INSERT INTO sys_role_menu (role_id, menu_id) VALUES (1, 1158);
INSERT INTO sys_role_menu (role_id, menu_id) VALUES (1, 1159);
INSERT INTO sys_role_menu (role_id, menu_id) VALUES (1, 1160);
INSERT INTO sys_role_menu (role_id, menu_id) VALUES (1, 1161);
INSERT INTO sys_role_menu (role_id, menu_id) VALUES (1, 1162);
INSERT INTO sys_role_menu (role_id, menu_id) VALUES (1, 1163);
INSERT INTO sys_role_menu (role_id, menu_id) VALUES (1, 1164);
INSERT INTO sys_role_menu (role_id, menu_id) VALUES (1, 1165);
INSERT INTO sys_role_menu (role_id, menu_id) VALUES (1, 1166);
INSERT INTO sys_role_menu (role_id, menu_id) VALUES (1, 1167);
INSERT INTO sys_role_menu (role_id, menu_id) VALUES (1, 1168);
INSERT INTO sys_role_menu (role_id, menu_id) VALUES (1, 1169);
INSERT INTO sys_role_menu (role_id, menu_id) VALUES (1, 1170);
INSERT INTO sys_role_menu (role_id, menu_id) VALUES (1, 1177);
INSERT INTO sys_role_menu (role_id, menu_id) VALUES (1, 1178);
INSERT INTO sys_role_menu (role_id, menu_id) VALUES (1, 1179);
INSERT INTO sys_role_menu (role_id, menu_id) VALUES (1, 1180);
INSERT INTO sys_role_menu (role_id, menu_id) VALUES (1, 1181);
INSERT INTO sys_role_menu (role_id, menu_id) VALUES (1, 1182);
INSERT INTO sys_role_menu (role_id, menu_id) VALUES (1, 179398067889600);
INSERT INTO sys_role_menu (role_id, menu_id) VALUES (1, 179398067889601);
INSERT INTO sys_role_menu (role_id, menu_id) VALUES (1, 179398067889602);
INSERT INTO sys_role_menu (role_id, menu_id) VALUES (1, 179398067889603);
INSERT INTO sys_role_menu (role_id, menu_id) VALUES (1, 179398067889604);
INSERT INTO sys_role_menu (role_id, menu_id) VALUES (2, 1);
INSERT INTO sys_role_menu (role_id, menu_id) VALUES (2, 3);
INSERT INTO sys_role_menu (role_id, menu_id) VALUES (2, 100);
INSERT INTO sys_role_menu (role_id, menu_id) VALUES (2, 101);
INSERT INTO sys_role_menu (role_id, menu_id) VALUES (2, 102);
INSERT INTO sys_role_menu (role_id, menu_id) VALUES (2, 103);
INSERT INTO sys_role_menu (role_id, menu_id) VALUES (2, 104);
INSERT INTO sys_role_menu (role_id, menu_id) VALUES (2, 105);
INSERT INTO sys_role_menu (role_id, menu_id) VALUES (2, 106);
INSERT INTO sys_role_menu (role_id, menu_id) VALUES (2, 108);
INSERT INTO sys_role_menu (role_id, menu_id) VALUES (2, 109);
INSERT INTO sys_role_menu (role_id, menu_id) VALUES (2, 110);
INSERT INTO sys_role_menu (role_id, menu_id) VALUES (2, 114);
INSERT INTO sys_role_menu (role_id, menu_id) VALUES (2, 115);
INSERT INTO sys_role_menu (role_id, menu_id) VALUES (2, 500);
INSERT INTO sys_role_menu (role_id, menu_id) VALUES (2, 501);
INSERT INTO sys_role_menu (role_id, menu_id) VALUES (11, 3);
INSERT INTO sys_role_menu (role_id, menu_id) VALUES (11, 114);
INSERT INTO sys_role_menu (role_id, menu_id) VALUES (11, 501);
INSERT INTO sys_role_menu (role_id, menu_id) VALUES (11, 530);
INSERT INTO sys_role_menu (role_id, menu_id) VALUES (11, 531);
INSERT INTO sys_role_menu (role_id, menu_id) VALUES (11, 532);
INSERT INTO sys_role_menu (role_id, menu_id) VALUES (11, 533);
INSERT INTO sys_role_menu (role_id, menu_id) VALUES (11, 534);
INSERT INTO sys_role_menu (role_id, menu_id) VALUES (11, 551);
INSERT INTO sys_role_menu (role_id, menu_id) VALUES (11, 552);
INSERT INTO sys_role_menu (role_id, menu_id) VALUES (11, 553);
INSERT INTO sys_role_menu (role_id, menu_id) VALUES (11, 554);


-- ---------------------------- sys_user ----------------------------
DROP TABLE IF EXISTS sys_user;
CREATE TABLE sys_user (
  "user_id" bigint NOT NULL,
  "dept_id" bigint DEFAULT NULL,
  "account" varchar(100) NOT NULL,
  "user_name" varchar(30) NOT NULL,
  "user_type" varchar(2) DEFAULT 00,
  "email" varchar(50) DEFAULT '',
  "phonenumber" varchar(11) DEFAULT '',
  "sex" varchar(1) DEFAULT NULL,
  "avatar" varchar(100) DEFAULT '',
  "password" varchar(100) DEFAULT '',
  "status" varchar(1) DEFAULT NULL,
  "is_del" integer DEFAULT 0,
  "login_ip" varchar(128) DEFAULT '',
  "login_date" timestamp DEFAULT NULL,
  "create_by" varchar(64) DEFAULT '',
  "create_time" timestamp DEFAULT NULL,
  "update_by" varchar(64) DEFAULT '',
  "update_time" timestamp DEFAULT NULL,
  "remark" varchar(500) DEFAULT NULL,
  "two_factor_secret" varchar(500) DEFAULT NULL,
  "two_factor_last_step" bigint DEFAULT 0,
  "two_factor_bind_time" timestamp DEFAULT NULL,
  PRIMARY KEY (user_id)
);
COMMENT ON COLUMN sys_user.user_id IS '用户ID';
COMMENT ON COLUMN sys_user.dept_id IS '部门ID';
COMMENT ON COLUMN sys_user.account IS '用户账户';
COMMENT ON COLUMN sys_user.user_name IS '用户名称';
COMMENT ON COLUMN sys_user.user_type IS '用户类型（00系统用户）';
COMMENT ON COLUMN sys_user.email IS '用户邮箱';
COMMENT ON COLUMN sys_user.phonenumber IS '手机号码';
COMMENT ON COLUMN sys_user.sex IS '用户性别（0男 1女 2未知）';
COMMENT ON COLUMN sys_user.avatar IS '头像地址';
COMMENT ON COLUMN sys_user.password IS '密码';
COMMENT ON COLUMN sys_user.status IS '帐号状态（0正常 1停用）';
COMMENT ON COLUMN sys_user.is_del IS '删除标志（0代表存在 1代表删除）';
COMMENT ON COLUMN sys_user.login_ip IS '最后登录IP';
COMMENT ON COLUMN sys_user.login_date IS '最后登录时间';
COMMENT ON COLUMN sys_user.create_by IS '创建者';
COMMENT ON COLUMN sys_user.create_time IS '创建时间';
COMMENT ON COLUMN sys_user.update_by IS '更新者';
COMMENT ON COLUMN sys_user.update_time IS '更新时间';
COMMENT ON COLUMN sys_user.remark IS '备注';
COMMENT ON COLUMN sys_user.two_factor_secret IS '两步验证密钥（AES加密存储）';
COMMENT ON COLUMN sys_user.two_factor_last_step IS '两步验证上次校验通过的时间片';
COMMENT ON COLUMN sys_user.two_factor_bind_time IS '两步验证绑定时间';
DELETE FROM sys_user;
INSERT INTO sys_user (user_id, dept_id, account, user_name, user_type, email, phonenumber, sex, avatar, password, status, is_del, login_ip, login_date, create_by, create_time, update_by, update_time, remark, two_factor_secret, two_factor_last_step, two_factor_bind_time) VALUES (1, 103, 'admin', '管理员', '00', 'wes@gmail.com', '15888888888', '1', '', '0gtW3BPaOmFNQ1ag2mt7llGYkp4MXSaPSdbzwUmPWVEMlxcELN7PmYBWzSH6tqxo', '0', 0, '127.0.0.1', '2022-04-17 15:33:03', 'admin', '2022-04-15 15:51:01', 'admin', '2026-07-04 00:33:02', '管理员', NULL, NULL, NULL);
INSERT INTO sys_user (user_id, dept_id, account, user_name, user_type, email, phonenumber, sex, avatar, password, status, is_del, login_ip, login_date, create_by, create_time, update_by, update_time, remark, two_factor_secret, two_factor_last_step, two_factor_bind_time) VALUES (2, NULL, 'zhangsan', '张三', '00', 'zhangsan@wms.com', '13800000001', '0', '', '', '0', 0, '', NULL, 'admin', '2026-07-21 22:52:10', '', NULL, NULL, NULL, NULL, NULL);
INSERT INTO sys_user (user_id, dept_id, account, user_name, user_type, email, phonenumber, sex, avatar, password, status, is_del, login_ip, login_date, create_by, create_time, update_by, update_time, remark, two_factor_secret, two_factor_last_step, two_factor_bind_time) VALUES (3, NULL, 'lisi', '李四', '00', 'lisi@wms.com', '13800000002', '0', '', '', '0', 0, '', NULL, 'admin', '2026-07-21 22:52:10', '', NULL, NULL, NULL, NULL, NULL);
INSERT INTO sys_user (user_id, dept_id, account, user_name, user_type, email, phonenumber, sex, avatar, password, status, is_del, login_ip, login_date, create_by, create_time, update_by, update_time, remark, two_factor_secret, two_factor_last_step, two_factor_bind_time) VALUES (4, NULL, 'wangwu', '王五', '00', 'wangwu@wms.com', '13800000003', '0', '', '', '0', 0, '', NULL, 'admin', '2026-07-21 22:52:10', '', NULL, NULL, NULL, NULL, NULL);
INSERT INTO sys_user (user_id, dept_id, account, user_name, user_type, email, phonenumber, sex, avatar, password, status, is_del, login_ip, login_date, create_by, create_time, update_by, update_time, remark, two_factor_secret, two_factor_last_step, two_factor_bind_time) VALUES (5, NULL, 'zhaoliu', '赵六', '00', 'zhaoliu@wms.com', '13800000004', '0', '', '', '0', 0, '', NULL, 'admin', '2026-07-21 22:52:10', '', NULL, NULL, NULL, NULL, NULL);
INSERT INTO sys_user (user_id, dept_id, account, user_name, user_type, email, phonenumber, sex, avatar, password, status, is_del, login_ip, login_date, create_by, create_time, update_by, update_time, remark, two_factor_secret, two_factor_last_step, two_factor_bind_time) VALUES (6, NULL, 'sunqi', '孙七', '00', 'sunqi@wms.com', '13800000005', '0', '', '', '0', 0, '', NULL, 'admin', '2026-07-21 22:52:10', '', NULL, NULL, NULL, NULL, NULL);
INSERT INTO sys_user (user_id, dept_id, account, user_name, user_type, email, phonenumber, sex, avatar, password, status, is_del, login_ip, login_date, create_by, create_time, update_by, update_time, remark, two_factor_secret, two_factor_last_step, two_factor_bind_time) VALUES (7, NULL, 'zhouba', '周八', '00', 'zhouba@wms.com', '13800000006', '0', '', '', '0', 0, '', NULL, 'admin', '2026-07-21 22:52:10', '', NULL, NULL, NULL, NULL, NULL);
INSERT INTO sys_user (user_id, dept_id, account, user_name, user_type, email, phonenumber, sex, avatar, password, status, is_del, login_ip, login_date, create_by, create_time, update_by, update_time, remark, two_factor_secret, two_factor_last_step, two_factor_bind_time) VALUES (8, NULL, 'wujiu', '吴九', '00', 'wujiu@wms.com', '13800000007', '0', '', '', '0', 0, '', NULL, 'admin', '2026-07-21 22:52:10', '', NULL, NULL, NULL, NULL, NULL);
INSERT INTO sys_user (user_id, dept_id, account, user_name, user_type, email, phonenumber, sex, avatar, password, status, is_del, login_ip, login_date, create_by, create_time, update_by, update_time, remark, two_factor_secret, two_factor_last_step, two_factor_bind_time) VALUES (9, NULL, 'zhengshi', '郑十', '00', 'zhengshi@wms.com', '13800000008', '0', '', '', '0', 0, '', NULL, 'admin', '2026-07-21 22:52:10', '', NULL, NULL, NULL, NULL, NULL);
INSERT INTO sys_user (user_id, dept_id, account, user_name, user_type, email, phonenumber, sex, avatar, password, status, is_del, login_ip, login_date, create_by, create_time, update_by, update_time, remark, two_factor_secret, two_factor_last_step, two_factor_bind_time) VALUES (10, NULL, 'chenming', '陈明', '00', 'chenming@wms.com', '13800000009', '0', '', '', '0', 0, '', NULL, 'admin', '2026-07-21 22:52:10', '', NULL, NULL, NULL, NULL, NULL);
INSERT INTO sys_user (user_id, dept_id, account, user_name, user_type, email, phonenumber, sex, avatar, password, status, is_del, login_ip, login_date, create_by, create_time, update_by, update_time, remark, two_factor_secret, two_factor_last_step, two_factor_bind_time) VALUES (11, NULL, 'liuqiang', '刘强', '00', 'liuqiang@wms.com', '13800000010', '0', '', '', '0', 0, '', NULL, 'admin', '2026-07-21 22:52:10', '', NULL, NULL, NULL, NULL, NULL);
INSERT INTO sys_user (user_id, dept_id, account, user_name, user_type, email, phonenumber, sex, avatar, password, status, is_del, login_ip, login_date, create_by, create_time, update_by, update_time, remark, two_factor_secret, two_factor_last_step, two_factor_bind_time) VALUES (12, NULL, 'huangli', '黄丽', '00', 'huangli@wms.com', '13800000011', '1', '', '', '0', 0, '', NULL, 'admin', '2026-07-21 22:52:10', '', NULL, NULL, NULL, NULL, NULL);
INSERT INTO sys_user (user_id, dept_id, account, user_name, user_type, email, phonenumber, sex, avatar, password, status, is_del, login_ip, login_date, create_by, create_time, update_by, update_time, remark, two_factor_secret, two_factor_last_step, two_factor_bind_time) VALUES (13, NULL, 'linfeng', '林峰', '00', 'linfeng@wms.com', '13800000012', '0', '', '', '0', 0, '', NULL, 'admin', '2026-07-21 22:52:10', '', NULL, NULL, NULL, NULL, NULL);
INSERT INTO sys_user (user_id, dept_id, account, user_name, user_type, email, phonenumber, sex, avatar, password, status, is_del, login_ip, login_date, create_by, create_time, update_by, update_time, remark, two_factor_secret, two_factor_last_step, two_factor_bind_time) VALUES (14, NULL, 'hefang', '何芳', '00', 'hefang@wms.com', '13800000013', '1', '', '', '0', 0, '', NULL, 'admin', '2026-07-21 22:52:10', '', NULL, NULL, NULL, NULL, NULL);
INSERT INTO sys_user (user_id, dept_id, account, user_name, user_type, email, phonenumber, sex, avatar, password, status, is_del, login_ip, login_date, create_by, create_time, update_by, update_time, remark, two_factor_secret, two_factor_last_step, two_factor_bind_time) VALUES (15, NULL, 'machao', '马超', '00', 'machao@wms.com', '13800000014', '0', '', '', '0', 0, '', NULL, 'admin', '2026-07-21 22:52:10', '', NULL, NULL, NULL, NULL, NULL);
INSERT INTO sys_user (user_id, dept_id, account, user_name, user_type, email, phonenumber, sex, avatar, password, status, is_del, login_ip, login_date, create_by, create_time, update_by, update_time, remark, two_factor_secret, two_factor_last_step, two_factor_bind_time) VALUES (16, NULL, 'luowei', '罗伟', '00', 'luowei@wms.com', '13800000015', '0', '', '', '0', 0, '', NULL, 'admin', '2026-07-21 22:52:10', '', NULL, NULL, NULL, NULL, NULL);
INSERT INTO sys_user (user_id, dept_id, account, user_name, user_type, email, phonenumber, sex, avatar, password, status, is_del, login_ip, login_date, create_by, create_time, update_by, update_time, remark, two_factor_secret, two_factor_last_step, two_factor_bind_time) VALUES (17, NULL, 'liangjing', '梁静', '00', 'liangjing@wms.com', '13800000016', '1', '', '', '0', 0, '', NULL, 'admin', '2026-07-21 22:52:10', '', NULL, NULL, NULL, NULL, NULL);
INSERT INTO sys_user (user_id, dept_id, account, user_name, user_type, email, phonenumber, sex, avatar, password, status, is_del, login_ip, login_date, create_by, create_time, update_by, update_time, remark, two_factor_secret, two_factor_last_step, two_factor_bind_time) VALUES (18, NULL, 'songtao', '宋涛', '00', 'songtao@wms.com', '13800000017', '0', '', '', '0', 0, '', NULL, 'admin', '2026-07-21 22:52:10', '', NULL, NULL, NULL, NULL, NULL);
INSERT INTO sys_user (user_id, dept_id, account, user_name, user_type, email, phonenumber, sex, avatar, password, status, is_del, login_ip, login_date, create_by, create_time, update_by, update_time, remark, two_factor_secret, two_factor_last_step, two_factor_bind_time) VALUES (19, NULL, 'tanglei', '唐磊', '00', 'tanglei@wms.com', '13800000018', '0', '', '', '0', 0, '', NULL, 'admin', '2026-07-21 22:52:10', '', NULL, NULL, NULL, NULL, NULL);
INSERT INTO sys_user (user_id, dept_id, account, user_name, user_type, email, phonenumber, sex, avatar, password, status, is_del, login_ip, login_date, create_by, create_time, update_by, update_time, remark, two_factor_secret, two_factor_last_step, two_factor_bind_time) VALUES (20, NULL, 'hanxue', '韩雪', '00', 'hanxue@wms.com', '13800000019', '1', '', '', '0', 0, '', NULL, 'admin', '2026-07-21 22:52:10', '', NULL, NULL, NULL, NULL, NULL);
INSERT INTO sys_user (user_id, dept_id, account, user_name, user_type, email, phonenumber, sex, avatar, password, status, is_del, login_ip, login_date, create_by, create_time, update_by, update_time, remark, two_factor_secret, two_factor_last_step, two_factor_bind_time) VALUES (21, NULL, 'fenggang', '冯刚', '00', 'fenggang@wms.com', '13800000020', '0', '', '', '0', 0, '', NULL, 'admin', '2026-07-21 22:52:10', '', NULL, NULL, NULL, NULL, NULL);


-- ---------------------------- sys_user_post ----------------------------
DROP TABLE IF EXISTS sys_user_post;
CREATE TABLE sys_user_post (
  "user_id" bigint NOT NULL,
  "post_id" bigint NOT NULL,
  PRIMARY KEY (user_id,post_id)
);
COMMENT ON COLUMN sys_user_post.user_id IS '用户ID';
COMMENT ON COLUMN sys_user_post.post_id IS '岗位ID';
DELETE FROM sys_user_post;
INSERT INTO sys_user_post (user_id, post_id) VALUES (1, 1);


-- ---------------------------- sys_user_role ----------------------------
DROP TABLE IF EXISTS sys_user_role;
CREATE TABLE sys_user_role (
  "user_id" bigint NOT NULL,
  "role_id" bigint NOT NULL,
  PRIMARY KEY (user_id,role_id)
);
COMMENT ON COLUMN sys_user_role.user_id IS '用户ID';
COMMENT ON COLUMN sys_user_role.role_id IS '角色ID';
DELETE FROM sys_user_role;
INSERT INTO sys_user_role (user_id, role_id) VALUES (1, 1);
INSERT INTO sys_user_role (user_id, role_id) VALUES (1, 2);
INSERT INTO sys_user_role (user_id, role_id) VALUES (1, 12);


-- ---------------------------- sys_code_rule ----------------------------
DROP TABLE IF EXISTS sys_code_rule;
CREATE TABLE sys_code_rule (
  "rule_id" bigint NOT NULL,
  "rule_code" varchar(50) NOT NULL,
  "rule_name" varchar(50) NOT NULL,
  "rule_type" varchar(20) DEFAULT NULL,
  "status" varchar(1) NOT NULL,
  "is_del" integer DEFAULT 0,
  "create_by" varchar(64) DEFAULT '',
  "create_time" timestamp DEFAULT NULL,
  "update_by" varchar(64) DEFAULT '',
  "update_time" timestamp DEFAULT NULL,
  PRIMARY KEY (rule_id)
);
COMMENT ON COLUMN sys_code_rule.rule_id IS '规则id';
COMMENT ON COLUMN sys_code_rule.rule_code IS '规则编码';
COMMENT ON COLUMN sys_code_rule.rule_name IS '规则名称';
COMMENT ON COLUMN sys_code_rule.rule_type IS '规则类型';
COMMENT ON COLUMN sys_code_rule.status IS '状态（0正常 1停用）';
COMMENT ON COLUMN sys_code_rule.is_del IS '删除标志（0代表存在 1代表删除）';
COMMENT ON COLUMN sys_code_rule.create_by IS '创建者';
COMMENT ON COLUMN sys_code_rule.create_time IS '创建时间';
COMMENT ON COLUMN sys_code_rule.update_by IS '更新者';
COMMENT ON COLUMN sys_code_rule.update_time IS '更新时间';
DELETE FROM sys_code_rule;
INSERT INTO sys_code_rule (rule_id, rule_code, rule_name, rule_type, status, is_del, create_by, create_time, update_by, update_time) VALUES (10000, 'WMS_RECEIVING', '收货单号', 'wms', '0', 0, 'admin', '2026-07-20 00:00:00', '', NULL);
INSERT INTO sys_code_rule (rule_id, rule_code, rule_name, rule_type, status, is_del, create_by, create_time, update_by, update_time) VALUES (10001, 'WMS_INSPECT', '检验单号', 'wms', '0', 0, 'admin', '2026-07-20 00:00:00', '', NULL);
INSERT INTO sys_code_rule (rule_id, rule_code, rule_name, rule_type, status, is_del, create_by, create_time, update_by, update_time) VALUES (10002, 'WMS_RECEIPT', '入库单号', 'wms', '0', 0, 'admin', '2026-07-20 00:00:00', '', NULL);
INSERT INTO sys_code_rule (rule_id, rule_code, rule_name, rule_type, status, is_del, create_by, create_time, update_by, update_time) VALUES (10004, 'WMS_RETURN_ORDER', '退货单号', 'wms', '0', 0, 'admin', '2026-07-20 00:00:00', '', NULL);
INSERT INTO sys_code_rule (rule_id, rule_code, rule_name, rule_type, status, is_del, create_by, create_time, update_by, update_time) VALUES (10005, 'WMS_DELIVERY_ORDER', '出库订单号', 'wms', '0', 0, 'admin', '2026-07-20 00:00:00', '', NULL);
INSERT INTO sys_code_rule (rule_id, rule_code, rule_name, rule_type, status, is_del, create_by, create_time, update_by, update_time) VALUES (10006, 'WMS_WAVE', '波次号', 'wms', '0', 0, 'admin', '2026-07-20 00:00:00', '', NULL);
INSERT INTO sys_code_rule (rule_id, rule_code, rule_name, rule_type, status, is_del, create_by, create_time, update_by, update_time) VALUES (10007, 'WMS_PICK', '拣货任务号', 'wms', '0', 0, 'admin', '2026-07-20 00:00:00', '', NULL);
INSERT INTO sys_code_rule (rule_id, rule_code, rule_name, rule_type, status, is_del, create_by, create_time, update_by, update_time) VALUES (10008, 'WMS_DELIVERY', '出库单号', 'wms', '0', 0, 'admin', '2026-07-20 00:00:00', '', NULL);
INSERT INTO sys_code_rule (rule_id, rule_code, rule_name, rule_type, status, is_del, create_by, create_time, update_by, update_time) VALUES (10009, 'WMS_RETURN_OUT', '出库退货单号', 'wms', '0', 0, 'admin', '2026-07-20 00:00:00', '', NULL);
INSERT INTO sys_code_rule (rule_id, rule_code, rule_name, rule_type, status, is_del, create_by, create_time, update_by, update_time) VALUES (10010, 'WMS_MOVE', '移库单号', 'wms', '0', 0, 'admin', '2026-07-20 00:00:00', '', NULL);
INSERT INTO sys_code_rule (rule_id, rule_code, rule_name, rule_type, status, is_del, create_by, create_time, update_by, update_time) VALUES (10011, 'WMS_CHECK', '盘点单号', 'wms', '0', 0, 'admin', '2026-07-20 00:00:00', '', NULL);
INSERT INTO sys_code_rule (rule_id, rule_code, rule_name, rule_type, status, is_del, create_by, create_time, update_by, update_time) VALUES (10012, 'WMS_ADJUST', '库存调整单号', 'wms', '0', 0, 'admin', '2026-07-20 00:00:00', '', NULL);
INSERT INTO sys_code_rule (rule_id, rule_code, rule_name, rule_type, status, is_del, create_by, create_time, update_by, update_time) VALUES (10014, 'WMS_IQC_PLAN', '质检计划单号', 'wms', '0', 0, 'admin', '2026-07-20 00:00:00', '', NULL);
INSERT INTO sys_code_rule (rule_id, rule_code, rule_name, rule_type, status, is_del, create_by, create_time, update_by, update_time) VALUES (10015, 'WMS_IQC_RECORD', '质检记录单号', 'wms', '0', 0, 'admin', '2026-07-20 00:00:00', '', NULL);
INSERT INTO sys_code_rule (rule_id, rule_code, rule_name, rule_type, status, is_del, create_by, create_time, update_by, update_time) VALUES (10016, 'WMS_BARCODE', '库存条码', 'wms', '0', 0, 'admin', '2026-10-21 11:15:36', '', NULL);
INSERT INTO sys_code_rule (rule_id, rule_code, rule_name, rule_type, status, is_del, create_by, create_time, update_by, update_time) VALUES (10017, 'WMS_ISSUE', '领料单号', 'wms', '0', 0, 'admin', '2026-10-23 12:04:18', '', NULL);
INSERT INTO sys_code_rule (rule_id, rule_code, rule_name, rule_type, status, is_del, create_by, create_time, update_by, update_time) VALUES (10018, 'WMS_TRANSFER', '调拨单号', 'wms', '0', 0, 'admin', '2026-10-23 16:09:31', '', NULL);


-- ---------------------------- sys_code_rule_part ----------------------------
DROP TABLE IF EXISTS sys_code_rule_part;
CREATE TABLE sys_code_rule_part (
  "part_id" bigint NOT NULL,
  "rule_id" bigint NOT NULL,
  "part_type" varchar(20) NOT NULL,
  "part_value" varchar(200) DEFAULT NULL,
  "reset_type" varchar(255) DEFAULT NULL,
  "week_start_day" integer NOT NULL,
  "reset_time" timestamp DEFAULT NULL,
  "current_index" varchar(800) DEFAULT NULL,
  "sort" integer NOT NULL DEFAULT 0,
  "is_del" integer NOT NULL DEFAULT 0,
  "remark" varchar(200) DEFAULT NULL,
  "create_by" varchar(64) DEFAULT '',
  "create_time" timestamp DEFAULT NULL,
  "update_by" varchar(64) DEFAULT '',
  "update_time" timestamp DEFAULT NULL,
  "is_skip_zero" integer NOT NULL DEFAULT 0,
  PRIMARY KEY (part_id)
);
COMMENT ON COLUMN sys_code_rule_part.part_id IS '编码片段规则';
COMMENT ON COLUMN sys_code_rule_part.rule_id IS '规则id';
COMMENT ON COLUMN sys_code_rule_part.part_type IS '片段类型 string/calc/date';
COMMENT ON COLUMN sys_code_rule_part.part_value IS '固定字符串/字符规则/日期格式';
COMMENT ON COLUMN sys_code_rule_part.reset_type IS '重置类型 week/month/quarter/year';
COMMENT ON COLUMN sys_code_rule_part.week_start_day IS '周开始时间 0周日 1周一';
COMMENT ON COLUMN sys_code_rule_part.reset_time IS '下次重置日期';
COMMENT ON COLUMN sys_code_rule_part.current_index IS '当前位置';
COMMENT ON COLUMN sys_code_rule_part.sort IS '排序字段';
COMMENT ON COLUMN sys_code_rule_part.is_del IS '删除标志（0代表存在 1代表删除）';
COMMENT ON COLUMN sys_code_rule_part.remark IS '备注';
COMMENT ON COLUMN sys_code_rule_part.create_by IS '创建者';
COMMENT ON COLUMN sys_code_rule_part.create_time IS '创建时间';
COMMENT ON COLUMN sys_code_rule_part.update_by IS '更新者';
COMMENT ON COLUMN sys_code_rule_part.update_time IS '更新时间';
COMMENT ON COLUMN sys_code_rule_part.is_skip_zero IS '是否跳过0';
DELETE FROM sys_code_rule_part;
INSERT INTO sys_code_rule_part (part_id, rule_id, part_type, part_value, reset_type, week_start_day, reset_time, current_index, sort, is_del, remark, create_by, create_time, update_by, update_time, is_skip_zero) VALUES (10000, 10000, 'string', 'AR', NULL, 0, NULL, NULL, 1, 0, '前缀', 'admin', '2026-07-20 00:00:00', '管理员', '2026-10-20 17:56:58', 0);
INSERT INTO sys_code_rule_part (part_id, rule_id, part_type, part_value, reset_type, week_start_day, reset_time, current_index, sort, is_del, remark, create_by, create_time, update_by, update_time, is_skip_zero) VALUES (10001, 10000, 'date', 'yyyyMMdd', NULL, 0, NULL, NULL, 2, 0, '日期', 'admin', '2026-07-20 00:00:00', '管理员', '2026-10-20 17:56:58', 0);
INSERT INTO sys_code_rule_part (part_id, rule_id, part_type, part_value, reset_type, week_start_day, reset_time, current_index, sort, is_del, remark, create_by, create_time, update_by, update_time, is_skip_zero) VALUES (10002, 10000, 'calc', '[0-9][0-9][0-9][0-9]', 'year', 0, NULL, '[{"StartIndex":0,"EndIndex":9,"CurrentIndex":0,"StartChar":"0","EndChar":"9","CurrentChar":"0"},{"StartIndex":0,"EndIndex":9,"CurrentIndex":0,"StartChar":"0","EndChar":"9","CurrentChar":"0"},{"StartIndex":0,"EndIndex":9,"CurrentIndex":0,"StartChar":"0","EndChar":"9","CurrentChar":"0"},{"StartIndex":0,"EndIndex":9,"CurrentIndex":6,"StartChar":"0","EndChar":"9","CurrentChar":"6"}]', 3, 0, '流水号', 'admin', '2026-07-20 00:00:00', '管理员', '2026-10-20 17:56:58', 0);
INSERT INTO sys_code_rule_part (part_id, rule_id, part_type, part_value, reset_type, week_start_day, reset_time, current_index, sort, is_del, remark, create_by, create_time, update_by, update_time, is_skip_zero) VALUES (10003, 10001, 'string', 'QC', NULL, 0, NULL, NULL, 1, 0, '前缀', 'admin', '2026-07-20 00:00:00', '管理员', '2026-10-20 17:57:16', 0);
INSERT INTO sys_code_rule_part (part_id, rule_id, part_type, part_value, reset_type, week_start_day, reset_time, current_index, sort, is_del, remark, create_by, create_time, update_by, update_time, is_skip_zero) VALUES (10004, 10001, 'date', 'yyyyMMdd', NULL, 0, NULL, NULL, 2, 0, '日期', 'admin', '2026-07-20 00:00:00', '管理员', '2026-10-20 17:57:16', 0);
INSERT INTO sys_code_rule_part (part_id, rule_id, part_type, part_value, reset_type, week_start_day, reset_time, current_index, sort, is_del, remark, create_by, create_time, update_by, update_time, is_skip_zero) VALUES (10005, 10001, 'calc', '[0-9][0-9][0-9][0-9]', 'year', 0, NULL, '[{"StartIndex":0,"EndIndex":9,"CurrentIndex":0,"StartChar":"0","EndChar":"9","CurrentChar":"0"},{"StartIndex":0,"EndIndex":9,"CurrentIndex":0,"StartChar":"0","EndChar":"9","CurrentChar":"0"},{"StartIndex":0,"EndIndex":9,"CurrentIndex":0,"StartChar":"0","EndChar":"9","CurrentChar":"0"},{"StartIndex":0,"EndIndex":9,"CurrentIndex":3,"StartChar":"0","EndChar":"9","CurrentChar":"3"}]', 3, 0, '流水号', 'admin', '2026-07-20 00:00:00', '管理员', '2026-10-20 17:57:16', 0);
INSERT INTO sys_code_rule_part (part_id, rule_id, part_type, part_value, reset_type, week_start_day, reset_time, current_index, sort, is_del, remark, create_by, create_time, update_by, update_time, is_skip_zero) VALUES (10006, 10002, 'string', 'RK', NULL, 0, NULL, NULL, 1, 0, '前缀', 'admin', '2026-07-20 00:00:00', '管理员', '2026-10-22 17:26:21', 0);
INSERT INTO sys_code_rule_part (part_id, rule_id, part_type, part_value, reset_type, week_start_day, reset_time, current_index, sort, is_del, remark, create_by, create_time, update_by, update_time, is_skip_zero) VALUES (10007, 10002, 'date', 'yyyyMMdd', NULL, 0, NULL, NULL, 2, 0, '日期', 'admin', '2026-07-20 00:00:00', '管理员', '2026-10-22 17:26:21', 0);
INSERT INTO sys_code_rule_part (part_id, rule_id, part_type, part_value, reset_type, week_start_day, reset_time, current_index, sort, is_del, remark, create_by, create_time, update_by, update_time, is_skip_zero) VALUES (10008, 10002, 'calc', '[0-9][0-9][0-9][0-9]', 'year', 0, NULL, '[{"StartIndex":0,"EndIndex":9,"CurrentIndex":0,"StartChar":"0","EndChar":"9","CurrentChar":"0"},{"StartIndex":0,"EndIndex":9,"CurrentIndex":0,"StartChar":"0","EndChar":"9","CurrentChar":"0"},{"StartIndex":0,"EndIndex":9,"CurrentIndex":0,"StartChar":"0","EndChar":"9","CurrentChar":"0"},{"StartIndex":0,"EndIndex":9,"CurrentIndex":7,"StartChar":"0","EndChar":"9","CurrentChar":"7"}]', 3, 0, '流水号', 'admin', '2026-07-20 00:00:00', '管理员', '2026-10-22 17:26:21', 0);
INSERT INTO sys_code_rule_part (part_id, rule_id, part_type, part_value, reset_type, week_start_day, reset_time, current_index, sort, is_del, remark, create_by, create_time, update_by, update_time, is_skip_zero) VALUES (10012, 10004, 'string', 'RTI', NULL, 0, NULL, NULL, 1, 0, '前缀', 'admin', '2026-07-20 00:00:00', '', NULL, 0);
INSERT INTO sys_code_rule_part (part_id, rule_id, part_type, part_value, reset_type, week_start_day, reset_time, current_index, sort, is_del, remark, create_by, create_time, update_by, update_time, is_skip_zero) VALUES (10013, 10004, 'date', 'yyyyMMdd', NULL, 0, NULL, NULL, 2, 0, '日期', 'admin', '2026-07-20 00:00:00', '', NULL, 0);
INSERT INTO sys_code_rule_part (part_id, rule_id, part_type, part_value, reset_type, week_start_day, reset_time, current_index, sort, is_del, remark, create_by, create_time, update_by, update_time, is_skip_zero) VALUES (10014, 10004, 'calc', '[0-9][0-9][0-9][0-9]', 'year', 0, NULL, '[{"StartIndex": 0, "EndIndex": 9, "CurrentIndex": 0, "StartChar": "0", "EndChar": "9", "CurrentChar": "0"}, {"StartIndex": 0, "EndIndex": 9, "CurrentIndex": 0, "StartChar": "0", "EndChar": "9", "CurrentChar": "0"}, {"StartIndex": 0, "EndIndex": 9, "CurrentIndex": 0, "StartChar": "0", "EndChar": "9", "CurrentChar": "0"}, {"StartIndex": 0, "EndIndex": 9, "CurrentIndex": 0, "StartChar": "0", "EndChar": "9", "CurrentChar": "0"}]', 3, 0, '流水号', 'admin', '2026-07-20 00:00:00', '', NULL, 0);
INSERT INTO sys_code_rule_part (part_id, rule_id, part_type, part_value, reset_type, week_start_day, reset_time, current_index, sort, is_del, remark, create_by, create_time, update_by, update_time, is_skip_zero) VALUES (10015, 10005, 'string', 'DO', NULL, 0, NULL, NULL, 1, 0, '前缀', 'admin', '2026-07-20 00:00:00', '', NULL, 0);
INSERT INTO sys_code_rule_part (part_id, rule_id, part_type, part_value, reset_type, week_start_day, reset_time, current_index, sort, is_del, remark, create_by, create_time, update_by, update_time, is_skip_zero) VALUES (10016, 10005, 'date', 'yyyyMMdd', NULL, 0, NULL, NULL, 2, 0, '日期', 'admin', '2026-07-20 00:00:00', '', NULL, 0);
INSERT INTO sys_code_rule_part (part_id, rule_id, part_type, part_value, reset_type, week_start_day, reset_time, current_index, sort, is_del, remark, create_by, create_time, update_by, update_time, is_skip_zero) VALUES (10017, 10005, 'calc', '[0-9][0-9][0-9][0-9]', 'year', 0, NULL, '[{"StartIndex": 0, "EndIndex": 9, "CurrentIndex": 0, "StartChar": "0", "EndChar": "9", "CurrentChar": "0"}, {"StartIndex": 0, "EndIndex": 9, "CurrentIndex": 0, "StartChar": "0", "EndChar": "9", "CurrentChar": "0"}, {"StartIndex": 0, "EndIndex": 9, "CurrentIndex": 0, "StartChar": "0", "EndChar": "9", "CurrentChar": "0"}, {"StartIndex": 0, "EndIndex": 9, "CurrentIndex": 0, "StartChar": "0", "EndChar": "9", "CurrentChar": "0"}]', 3, 0, '流水号', 'admin', '2026-07-20 00:00:00', '', NULL, 0);
INSERT INTO sys_code_rule_part (part_id, rule_id, part_type, part_value, reset_type, week_start_day, reset_time, current_index, sort, is_del, remark, create_by, create_time, update_by, update_time, is_skip_zero) VALUES (10018, 10006, 'string', 'WV', NULL, 0, NULL, NULL, 1, 0, '前缀', 'admin', '2026-07-20 00:00:00', '', NULL, 0);
INSERT INTO sys_code_rule_part (part_id, rule_id, part_type, part_value, reset_type, week_start_day, reset_time, current_index, sort, is_del, remark, create_by, create_time, update_by, update_time, is_skip_zero) VALUES (10019, 10006, 'date', 'yyyyMMdd', NULL, 0, NULL, NULL, 2, 0, '日期', 'admin', '2026-07-20 00:00:00', '', NULL, 0);
INSERT INTO sys_code_rule_part (part_id, rule_id, part_type, part_value, reset_type, week_start_day, reset_time, current_index, sort, is_del, remark, create_by, create_time, update_by, update_time, is_skip_zero) VALUES (10020, 10006, 'calc', '[0-9][0-9][0-9][0-9]', 'year', 0, NULL, '[{"StartIndex": 0, "EndIndex": 9, "CurrentIndex": 0, "StartChar": "0", "EndChar": "9", "CurrentChar": "0"}, {"StartIndex": 0, "EndIndex": 9, "CurrentIndex": 0, "StartChar": "0", "EndChar": "9", "CurrentChar": "0"}, {"StartIndex": 0, "EndIndex": 9, "CurrentIndex": 0, "StartChar": "0", "EndChar": "9", "CurrentChar": "0"}, {"StartIndex": 0, "EndIndex": 9, "CurrentIndex": 0, "StartChar": "0", "EndChar": "9", "CurrentChar": "0"}]', 3, 0, '流水号', 'admin', '2026-07-20 00:00:00', '', NULL, 0);
INSERT INTO sys_code_rule_part (part_id, rule_id, part_type, part_value, reset_type, week_start_day, reset_time, current_index, sort, is_del, remark, create_by, create_time, update_by, update_time, is_skip_zero) VALUES (10021, 10007, 'string', 'PK', NULL, 0, NULL, NULL, 1, 0, '前缀', 'admin', '2026-07-20 00:00:00', '', NULL, 0);
INSERT INTO sys_code_rule_part (part_id, rule_id, part_type, part_value, reset_type, week_start_day, reset_time, current_index, sort, is_del, remark, create_by, create_time, update_by, update_time, is_skip_zero) VALUES (10022, 10007, 'date', 'yyyyMMdd', NULL, 0, NULL, NULL, 2, 0, '日期', 'admin', '2026-07-20 00:00:00', '', NULL, 0);
INSERT INTO sys_code_rule_part (part_id, rule_id, part_type, part_value, reset_type, week_start_day, reset_time, current_index, sort, is_del, remark, create_by, create_time, update_by, update_time, is_skip_zero) VALUES (10023, 10007, 'calc', '[0-9][0-9][0-9][0-9]', 'year', 0, NULL, '[{"StartIndex": 0, "EndIndex": 9, "CurrentIndex": 0, "StartChar": "0", "EndChar": "9", "CurrentChar": "0"}, {"StartIndex": 0, "EndIndex": 9, "CurrentIndex": 0, "StartChar": "0", "EndChar": "9", "CurrentChar": "0"}, {"StartIndex": 0, "EndIndex": 9, "CurrentIndex": 0, "StartChar": "0", "EndChar": "9", "CurrentChar": "0"}, {"StartIndex": 0, "EndIndex": 9, "CurrentIndex": 0, "StartChar": "0", "EndChar": "9", "CurrentChar": "0"}]', 3, 0, '流水号', 'admin', '2026-07-20 00:00:00', '', NULL, 0);
INSERT INTO sys_code_rule_part (part_id, rule_id, part_type, part_value, reset_type, week_start_day, reset_time, current_index, sort, is_del, remark, create_by, create_time, update_by, update_time, is_skip_zero) VALUES (10024, 10008, 'string', 'CK', NULL, 0, NULL, NULL, 1, 0, '前缀', 'admin', '2026-07-20 00:00:00', '', NULL, 0);
INSERT INTO sys_code_rule_part (part_id, rule_id, part_type, part_value, reset_type, week_start_day, reset_time, current_index, sort, is_del, remark, create_by, create_time, update_by, update_time, is_skip_zero) VALUES (10025, 10008, 'date', 'yyyyMMdd', NULL, 0, NULL, NULL, 2, 0, '日期', 'admin', '2026-07-20 00:00:00', '', NULL, 0);
INSERT INTO sys_code_rule_part (part_id, rule_id, part_type, part_value, reset_type, week_start_day, reset_time, current_index, sort, is_del, remark, create_by, create_time, update_by, update_time, is_skip_zero) VALUES (10026, 10008, 'calc', '[0-9][0-9][0-9][0-9]', 'year', 0, NULL, '[{"StartIndex": 0, "EndIndex": 9, "CurrentIndex": 0, "StartChar": "0", "EndChar": "9", "CurrentChar": "0"}, {"StartIndex": 0, "EndIndex": 9, "CurrentIndex": 0, "StartChar": "0", "EndChar": "9", "CurrentChar": "0"}, {"StartIndex": 0, "EndIndex": 9, "CurrentIndex": 0, "StartChar": "0", "EndChar": "9", "CurrentChar": "0"}, {"StartIndex": 0, "EndIndex": 9, "CurrentIndex": 0, "StartChar": "0", "EndChar": "9", "CurrentChar": "0"}]', 3, 0, '流水号', 'admin', '2026-07-20 00:00:00', '', NULL, 0);
INSERT INTO sys_code_rule_part (part_id, rule_id, part_type, part_value, reset_type, week_start_day, reset_time, current_index, sort, is_del, remark, create_by, create_time, update_by, update_time, is_skip_zero) VALUES (10027, 10009, 'string', 'RTO', NULL, 0, NULL, NULL, 1, 0, '前缀', 'admin', '2026-07-20 00:00:00', '', NULL, 0);
INSERT INTO sys_code_rule_part (part_id, rule_id, part_type, part_value, reset_type, week_start_day, reset_time, current_index, sort, is_del, remark, create_by, create_time, update_by, update_time, is_skip_zero) VALUES (10028, 10009, 'date', 'yyyyMMdd', NULL, 0, NULL, NULL, 2, 0, '日期', 'admin', '2026-07-20 00:00:00', '', NULL, 0);
INSERT INTO sys_code_rule_part (part_id, rule_id, part_type, part_value, reset_type, week_start_day, reset_time, current_index, sort, is_del, remark, create_by, create_time, update_by, update_time, is_skip_zero) VALUES (10029, 10009, 'calc', '[0-9][0-9][0-9][0-9]', 'year', 0, NULL, '[{"StartIndex": 0, "EndIndex": 9, "CurrentIndex": 0, "StartChar": "0", "EndChar": "9", "CurrentChar": "0"}, {"StartIndex": 0, "EndIndex": 9, "CurrentIndex": 0, "StartChar": "0", "EndChar": "9", "CurrentChar": "0"}, {"StartIndex": 0, "EndIndex": 9, "CurrentIndex": 0, "StartChar": "0", "EndChar": "9", "CurrentChar": "0"}, {"StartIndex": 0, "EndIndex": 9, "CurrentIndex": 0, "StartChar": "0", "EndChar": "9", "CurrentChar": "0"}]', 3, 0, '流水号', 'admin', '2026-07-20 00:00:00', '', NULL, 0);
INSERT INTO sys_code_rule_part (part_id, rule_id, part_type, part_value, reset_type, week_start_day, reset_time, current_index, sort, is_del, remark, create_by, create_time, update_by, update_time, is_skip_zero) VALUES (10030, 10010, 'string', 'YK', NULL, 0, NULL, NULL, 1, 0, '前缀', 'admin', '2026-07-20 00:00:00', '管理员', '2026-10-23 22:50:52', 0);
INSERT INTO sys_code_rule_part (part_id, rule_id, part_type, part_value, reset_type, week_start_day, reset_time, current_index, sort, is_del, remark, create_by, create_time, update_by, update_time, is_skip_zero) VALUES (10031, 10010, 'date', 'yyyyMMdd', NULL, 0, NULL, NULL, 2, 0, '日期', 'admin', '2026-07-20 00:00:00', '管理员', '2026-10-23 22:50:52', 0);
INSERT INTO sys_code_rule_part (part_id, rule_id, part_type, part_value, reset_type, week_start_day, reset_time, current_index, sort, is_del, remark, create_by, create_time, update_by, update_time, is_skip_zero) VALUES (10032, 10010, 'calc', '[0-9][0-9][0-9][0-9]', 'year', 0, NULL, '[{"StartIndex":0,"EndIndex":9,"CurrentIndex":0,"StartChar":"0","EndChar":"9","CurrentChar":"0"},{"StartIndex":0,"EndIndex":9,"CurrentIndex":0,"StartChar":"0","EndChar":"9","CurrentChar":"0"},{"StartIndex":0,"EndIndex":9,"CurrentIndex":0,"StartChar":"0","EndChar":"9","CurrentChar":"0"},{"StartIndex":0,"EndIndex":9,"CurrentIndex":1,"StartChar":"0","EndChar":"9","CurrentChar":"1"}]', 3, 0, '流水号', 'admin', '2026-07-20 00:00:00', '管理员', '2026-10-23 22:50:52', 0);
INSERT INTO sys_code_rule_part (part_id, rule_id, part_type, part_value, reset_type, week_start_day, reset_time, current_index, sort, is_del, remark, create_by, create_time, update_by, update_time, is_skip_zero) VALUES (10033, 10011, 'string', 'PD', NULL, 0, NULL, NULL, 1, 0, '前缀', 'admin', '2026-07-20 00:00:00', '', NULL, 0);
INSERT INTO sys_code_rule_part (part_id, rule_id, part_type, part_value, reset_type, week_start_day, reset_time, current_index, sort, is_del, remark, create_by, create_time, update_by, update_time, is_skip_zero) VALUES (10034, 10011, 'date', 'yyyyMMdd', NULL, 0, NULL, NULL, 2, 0, '日期', 'admin', '2026-07-20 00:00:00', '', NULL, 0);
INSERT INTO sys_code_rule_part (part_id, rule_id, part_type, part_value, reset_type, week_start_day, reset_time, current_index, sort, is_del, remark, create_by, create_time, update_by, update_time, is_skip_zero) VALUES (10035, 10011, 'calc', '[0-9][0-9][0-9][0-9]', 'year', 0, NULL, '[{"StartIndex": 0, "EndIndex": 9, "CurrentIndex": 0, "StartChar": "0", "EndChar": "9", "CurrentChar": "0"}, {"StartIndex": 0, "EndIndex": 9, "CurrentIndex": 0, "StartChar": "0", "EndChar": "9", "CurrentChar": "0"}, {"StartIndex": 0, "EndIndex": 9, "CurrentIndex": 0, "StartChar": "0", "EndChar": "9", "CurrentChar": "0"}, {"StartIndex": 0, "EndIndex": 9, "CurrentIndex": 0, "StartChar": "0", "EndChar": "9", "CurrentChar": "0"}]', 3, 0, '流水号', 'admin', '2026-07-20 00:00:00', '', NULL, 0);
INSERT INTO sys_code_rule_part (part_id, rule_id, part_type, part_value, reset_type, week_start_day, reset_time, current_index, sort, is_del, remark, create_by, create_time, update_by, update_time, is_skip_zero) VALUES (10036, 10012, 'string', 'TZ', NULL, 0, NULL, NULL, 1, 0, '前缀', 'admin', '2026-07-20 00:00:00', '', NULL, 0);
INSERT INTO sys_code_rule_part (part_id, rule_id, part_type, part_value, reset_type, week_start_day, reset_time, current_index, sort, is_del, remark, create_by, create_time, update_by, update_time, is_skip_zero) VALUES (10037, 10012, 'date', 'yyyyMMdd', NULL, 0, NULL, NULL, 2, 0, '日期', 'admin', '2026-07-20 00:00:00', '', NULL, 0);
INSERT INTO sys_code_rule_part (part_id, rule_id, part_type, part_value, reset_type, week_start_day, reset_time, current_index, sort, is_del, remark, create_by, create_time, update_by, update_time, is_skip_zero) VALUES (10038, 10012, 'calc', '[0-9][0-9][0-9][0-9]', 'year', 0, NULL, '[{"StartIndex": 0, "EndIndex": 9, "CurrentIndex": 0, "StartChar": "0", "EndChar": "9", "CurrentChar": "0"}, {"StartIndex": 0, "EndIndex": 9, "CurrentIndex": 0, "StartChar": "0", "EndChar": "9", "CurrentChar": "0"}, {"StartIndex": 0, "EndIndex": 9, "CurrentIndex": 0, "StartChar": "0", "EndChar": "9", "CurrentChar": "0"}, {"StartIndex": 0, "EndIndex": 9, "CurrentIndex": 0, "StartChar": "0", "EndChar": "9", "CurrentChar": "0"}]', 3, 0, '流水号', 'admin', '2026-07-20 00:00:00', '', NULL, 0);
INSERT INTO sys_code_rule_part (part_id, rule_id, part_type, part_value, reset_type, week_start_day, reset_time, current_index, sort, is_del, remark, create_by, create_time, update_by, update_time, is_skip_zero) VALUES (10042, 10014, 'string', 'IQCP', NULL, 0, NULL, NULL, 1, 0, '前缀', 'admin', '2026-07-20 00:00:00', '', NULL, 0);
INSERT INTO sys_code_rule_part (part_id, rule_id, part_type, part_value, reset_type, week_start_day, reset_time, current_index, sort, is_del, remark, create_by, create_time, update_by, update_time, is_skip_zero) VALUES (10043, 10014, 'date', 'yyyyMMdd', NULL, 0, NULL, NULL, 2, 0, '日期', 'admin', '2026-07-20 00:00:00', '', NULL, 0);
INSERT INTO sys_code_rule_part (part_id, rule_id, part_type, part_value, reset_type, week_start_day, reset_time, current_index, sort, is_del, remark, create_by, create_time, update_by, update_time, is_skip_zero) VALUES (10044, 10014, 'calc', '[0-9][0-9][0-9][0-9]', 'year', 0, NULL, '[{"StartIndex": 0, "EndIndex": 9, "CurrentIndex": 0, "StartChar": "0", "EndChar": "9", "CurrentChar": "0"}, {"StartIndex": 0, "EndIndex": 9, "CurrentIndex": 0, "StartChar": "0", "EndChar": "9", "CurrentChar": "0"}, {"StartIndex": 0, "EndIndex": 9, "CurrentIndex": 0, "StartChar": "0", "EndChar": "9", "CurrentChar": "0"}, {"StartIndex": 0, "EndIndex": 9, "CurrentIndex": 0, "StartChar": "0", "EndChar": "9", "CurrentChar": "0"}]', 3, 0, '流水号', 'admin', '2026-07-20 00:00:00', '', NULL, 0);
INSERT INTO sys_code_rule_part (part_id, rule_id, part_type, part_value, reset_type, week_start_day, reset_time, current_index, sort, is_del, remark, create_by, create_time, update_by, update_time, is_skip_zero) VALUES (10045, 10015, 'string', 'IQCR', NULL, 0, NULL, NULL, 1, 0, '前缀', 'admin', '2026-07-20 00:00:00', '', NULL, 0);
INSERT INTO sys_code_rule_part (part_id, rule_id, part_type, part_value, reset_type, week_start_day, reset_time, current_index, sort, is_del, remark, create_by, create_time, update_by, update_time, is_skip_zero) VALUES (10046, 10015, 'date', 'yyyyMMdd', NULL, 0, NULL, NULL, 2, 0, '日期', 'admin', '2026-07-20 00:00:00', '', NULL, 0);
INSERT INTO sys_code_rule_part (part_id, rule_id, part_type, part_value, reset_type, week_start_day, reset_time, current_index, sort, is_del, remark, create_by, create_time, update_by, update_time, is_skip_zero) VALUES (10047, 10015, 'calc', '[0-9][0-9][0-9][0-9]', 'year', 0, NULL, '[{"StartIndex": 0, "EndIndex": 9, "CurrentIndex": 0, "StartChar": "0", "EndChar": "9", "CurrentChar": "0"}, {"StartIndex": 0, "EndIndex": 9, "CurrentIndex": 0, "StartChar": "0", "EndChar": "9", "CurrentChar": "0"}, {"StartIndex": 0, "EndIndex": 9, "CurrentIndex": 0, "StartChar": "0", "EndChar": "9", "CurrentChar": "0"}, {"StartIndex": 0, "EndIndex": 9, "CurrentIndex": 0, "StartChar": "0", "EndChar": "9", "CurrentChar": "0"}]', 3, 0, '流水号', 'admin', '2026-07-20 00:00:00', '', NULL, 0);
INSERT INTO sys_code_rule_part (part_id, rule_id, part_type, part_value, reset_type, week_start_day, reset_time, current_index, sort, is_del, remark, create_by, create_time, update_by, update_time, is_skip_zero) VALUES (10048, 10016, 'string', 'S', NULL, 0, NULL, NULL, 1, 0, '前缀', 'admin', '2026-10-21 11:15:36', '管理员', '2026-10-22 23:55:17', 0);
INSERT INTO sys_code_rule_part (part_id, rule_id, part_type, part_value, reset_type, week_start_day, reset_time, current_index, sort, is_del, remark, create_by, create_time, update_by, update_time, is_skip_zero) VALUES (10049, 10016, 'date', 'yyyyMMdd', NULL, 0, NULL, NULL, 2, 0, '日期', 'admin', '2026-10-21 11:15:36', '管理员', '2026-10-22 23:55:17', 0);
INSERT INTO sys_code_rule_part (part_id, rule_id, part_type, part_value, reset_type, week_start_day, reset_time, current_index, sort, is_del, remark, create_by, create_time, update_by, update_time, is_skip_zero) VALUES (10050, 10016, 'calc', '[0-9][0-9][0-9][0-9][0-9][0-9]', 'year', 0, NULL, '[{"StartIndex":0,"EndIndex":9,"CurrentIndex":0,"StartChar":"0","EndChar":"9","CurrentChar":"0"},{"StartIndex":0,"EndIndex":9,"CurrentIndex":0,"StartChar":"0","EndChar":"9","CurrentChar":"0"},{"StartIndex":0,"EndIndex":9,"CurrentIndex":0,"StartChar":"0","EndChar":"9","CurrentChar":"0"},{"StartIndex":0,"EndIndex":9,"CurrentIndex":0,"StartChar":"0","EndChar":"9","CurrentChar":"0"},{"StartIndex":0,"EndIndex":9,"CurrentIndex":3,"StartChar":"0","EndChar":"9","CurrentChar":"3"},{"StartIndex":0,"EndIndex":9,"CurrentIndex":5,"StartChar":"0","EndChar":"9","CurrentChar":"5"}]', 3, 0, '流水号', 'admin', '2026-10-21 11:15:36', '管理员', '2026-10-22 23:55:17', 0);
INSERT INTO sys_code_rule_part (part_id, rule_id, part_type, part_value, reset_type, week_start_day, reset_time, current_index, sort, is_del, remark, create_by, create_time, update_by, update_time, is_skip_zero) VALUES (10051, 10017, 'string', 'LL', NULL, 0, NULL, NULL, 1, 0, '前缀', 'admin', '2026-10-23 12:04:18', '', NULL, 0);
INSERT INTO sys_code_rule_part (part_id, rule_id, part_type, part_value, reset_type, week_start_day, reset_time, current_index, sort, is_del, remark, create_by, create_time, update_by, update_time, is_skip_zero) VALUES (10052, 10017, 'date', 'yyyyMMdd', NULL, 0, NULL, NULL, 2, 0, '日期', 'admin', '2026-10-23 12:04:18', '', NULL, 0);
INSERT INTO sys_code_rule_part (part_id, rule_id, part_type, part_value, reset_type, week_start_day, reset_time, current_index, sort, is_del, remark, create_by, create_time, update_by, update_time, is_skip_zero) VALUES (10053, 10017, 'calc', '[0-9][0-9][0-9][0-9]', 'year', 0, NULL, '[{"StartIndex": 0, "EndIndex": 9, "CurrentIndex": 0, "StartChar": "0", "EndChar": "9", "CurrentChar": "0"}, {"StartIndex": 0, "EndIndex": 9, "CurrentIndex": 0, "StartChar": "0", "EndChar": "9", "CurrentChar": "0"}, {"StartIndex": 0, "EndIndex": 9, "CurrentIndex": 0, "StartChar": "0", "EndChar": "9", "CurrentChar": "0"}, {"StartIndex": 0, "EndIndex": 9, "CurrentIndex": 0, "StartChar": "0", "EndChar": "9", "CurrentChar": "0"}]', 3, 0, '流水号', 'admin', '2026-10-23 12:04:18', '', NULL, 0);
INSERT INTO sys_code_rule_part (part_id, rule_id, part_type, part_value, reset_type, week_start_day, reset_time, current_index, sort, is_del, remark, create_by, create_time, update_by, update_time, is_skip_zero) VALUES (10054, 10018, 'string', 'DB', NULL, 0, NULL, '', 1, 0, '前缀', 'admin', '2026-10-23 16:09:31', '', NULL, 0);
INSERT INTO sys_code_rule_part (part_id, rule_id, part_type, part_value, reset_type, week_start_day, reset_time, current_index, sort, is_del, remark, create_by, create_time, update_by, update_time, is_skip_zero) VALUES (10055, 10018, 'date', 'yyyyMMdd', NULL, 0, NULL, '', 2, 0, '日期', 'admin', '2026-10-23 16:09:31', '', NULL, 0);
INSERT INTO sys_code_rule_part (part_id, rule_id, part_type, part_value, reset_type, week_start_day, reset_time, current_index, sort, is_del, remark, create_by, create_time, update_by, update_time, is_skip_zero) VALUES (10056, 10018, 'calc', '[0-9][0-9][0-9][0-9]', 'year', 0, NULL, '', 3, 0, '流水号', 'admin', '2026-10-23 16:09:31', '', NULL, 0);


-- ---------------------------- sys_job ----------------------------
DROP TABLE IF EXISTS sys_job;
CREATE TABLE sys_job (
  "job_id" bigint NOT NULL,
  "job_name" varchar(64) NOT NULL DEFAULT '',
  "job_group" varchar(64) NOT NULL DEFAULT 'DEFAULT',
  "invoke_target" varchar(500) NOT NULL,
  "cron_expression" varchar(255) DEFAULT '',
  "misfire_policy" varchar(20) DEFAULT 3,
  "concurrent" char(1) DEFAULT 1,
  "status" char(1) DEFAULT 0,
  "create_by" varchar(64) DEFAULT '',
  "create_time" timestamp DEFAULT NULL,
  "update_by" varchar(64) DEFAULT '',
  "update_time" timestamp DEFAULT NULL,
  "remark" varchar(500) DEFAULT '',
  PRIMARY KEY (job_id,job_name,job_group)
);
COMMENT ON COLUMN sys_job.job_id IS '任务ID';
COMMENT ON COLUMN sys_job.job_name IS '任务名称';
COMMENT ON COLUMN sys_job.job_group IS '任务组名';
COMMENT ON COLUMN sys_job.invoke_target IS '调用目标字符串';
COMMENT ON COLUMN sys_job.cron_expression IS 'cron执行表达式';
COMMENT ON COLUMN sys_job.misfire_policy IS '计划执行错误策略（1立即执行 2执行一次 3放弃执行）';
COMMENT ON COLUMN sys_job.concurrent IS '是否并发执行（0允许 1禁止）';
COMMENT ON COLUMN sys_job.status IS '状态（0正常 1暂停）';
COMMENT ON COLUMN sys_job.create_by IS '创建者';
COMMENT ON COLUMN sys_job.create_time IS '创建时间';
COMMENT ON COLUMN sys_job.update_by IS '更新者';
COMMENT ON COLUMN sys_job.update_time IS '更新时间';
COMMENT ON COLUMN sys_job.remark IS '备注信息';
DELETE FROM sys_job;
INSERT INTO sys_job (job_id, job_name, job_group, invoke_target, cron_expression, misfire_policy, concurrent, status, create_by, create_time, update_by, update_time, remark) VALUES (1, '系统默认（无参）', 'DEFAULT', 'ryTask.ryNoParams', '0/10 * * * * ?', '3', '1', '1', 'admin', '2022-04-15 15:51:02', '', NULL, '');
INSERT INTO sys_job (job_id, job_name, job_group, invoke_target, cron_expression, misfire_policy, concurrent, status, create_by, create_time, update_by, update_time, remark) VALUES (2, '系统默认（有参）', 'DEFAULT', 'ryTask.ryParams(''ry'')', '0/15 * * * * ?', '3', '1', '1', 'admin', '2022-04-15 15:51:02', '', NULL, '');
INSERT INTO sys_job (job_id, job_name, job_group, invoke_target, cron_expression, misfire_policy, concurrent, status, create_by, create_time, update_by, update_time, remark) VALUES (3, '系统默认（多参）', 'DEFAULT', 'ryTask.ryMultipleParams(''ry'', true, 2000L, 316.50D, 100)', '0/20 * * * * ?', '3', '1', '1', 'admin', '2022-04-15 15:51:02', '', NULL, '');
INSERT INTO sys_job (job_id, job_name, job_group, invoke_target, cron_expression, misfire_policy, concurrent, status, create_by, create_time, update_by, update_time, remark) VALUES (1793980678955, '库存预警检查', 'SYSTEM', 'StockWarning', '0 0 8 * * ?', '3', '1', '0', 'admin', '2026-11-06 23:57:58', '', NULL, '按预警配置检查库存，触发预警并给处理人发站内信');
INSERT INTO sys_job (job_id, job_name, job_group, invoke_target, cron_expression, misfire_policy, concurrent, status, create_by, create_time, update_by, update_time, remark) VALUES (2071956606450536448, 'DingTalkUserSync', 'SYSTEM', 'DingTalkUserSync', '0 0 0 * * ?', '3', '1', '1', 'SYSTEM', '2026-06-30 21:58:45', NULL, NULL, '系统自动发现');


-- ---------------------------- sys_data_service ----------------------------
DROP TABLE IF EXISTS sys_data_service;
CREATE TABLE sys_data_service (
  "ds_id" bigint NOT NULL,
  "service_code" varchar(50) DEFAULT NULL,
  "service_name" varchar(100) DEFAULT NULL,
  "category" varchar(20) DEFAULT NULL,
  "param_config" varchar(1024) DEFAULT NULL,
  "status" varchar(11) DEFAULT NULL,
  "remark" varchar(50) DEFAULT NULL,
  "is_del" integer DEFAULT NULL,
  "create_by" varchar(64) DEFAULT NULL,
  "create_time" timestamp DEFAULT NULL,
  "update_by" varchar(64) DEFAULT NULL,
  "update_time" timestamp DEFAULT NULL,
  PRIMARY KEY (ds_id)
);
COMMENT ON COLUMN sys_data_service.ds_id IS '主键id';
COMMENT ON COLUMN sys_data_service.service_code IS '数据服务编码';
COMMENT ON COLUMN sys_data_service.service_name IS '名称';
COMMENT ON COLUMN sys_data_service.category IS '分类字典';
COMMENT ON COLUMN sys_data_service.param_config IS '参数配置';
COMMENT ON COLUMN sys_data_service.status IS '是否有效（0代表无效 1代表有效）';
COMMENT ON COLUMN sys_data_service.remark IS '描述';
COMMENT ON COLUMN sys_data_service.is_del IS '删除标志（0代表存在 1代表删除）';
COMMENT ON COLUMN sys_data_service.create_by IS '创建者';
COMMENT ON COLUMN sys_data_service.create_time IS '创建时间';
COMMENT ON COLUMN sys_data_service.update_by IS '更新者';
COMMENT ON COLUMN sys_data_service.update_time IS '更新时间';
DELETE FROM sys_data_service;
INSERT INTO sys_data_service (ds_id, service_code, service_name, category, param_config, status, remark, is_del, create_by, create_time, update_by, update_time) VALUES (1874451681413042176, 'aaa', 'test', 'builtin', '[{"key":"createTime","type":"date"},{"key":"user","type":"object"}]', '0', '', 0, 'admin', '2025-01-01 21:44:41', 'admin', '2026-07-18 22:47:41');


-- ---------------------------- sys_data_service_node ----------------------------
DROP TABLE IF EXISTS sys_data_service_node;
CREATE TABLE sys_data_service_node (
  "ds_part_id" bigint NOT NULL,
  "ds_id" bigint DEFAULT NULL,
  "part_name" varchar(100) DEFAULT NULL,
  "var_type" integer DEFAULT NULL,
  "var_name" varchar(100) DEFAULT NULL,
  "part_config" varchar(1024) DEFAULT NULL,
  "sort_by" integer DEFAULT NULL,
  "part_type" integer DEFAULT NULL,
  "is_del" integer DEFAULT NULL,
  PRIMARY KEY (ds_part_id)
);
COMMENT ON COLUMN sys_data_service_node.ds_part_id IS '主键id';
COMMENT ON COLUMN sys_data_service_node.ds_id IS '关联主表id';
COMMENT ON COLUMN sys_data_service_node.part_name IS '节点名称';
COMMENT ON COLUMN sys_data_service_node.var_type IS '变量类型';
COMMENT ON COLUMN sys_data_service_node.var_name IS '变量名';
COMMENT ON COLUMN sys_data_service_node.part_config IS '内容';
COMMENT ON COLUMN sys_data_service_node.sort_by IS '排序字段';
COMMENT ON COLUMN sys_data_service_node.part_type IS '类型 0 sql，1 js';
COMMENT ON COLUMN sys_data_service_node.is_del IS '删除标志（0代表存在 1代表删除）';
DELETE FROM sys_data_service_node;
INSERT INTO sys_data_service_node (ds_part_id, ds_id, part_name, var_type, var_name, part_config, sort_by, part_type, is_del) VALUES (2044976888220356608, 1874451681413042176, '步骤1', 0, 'user', 'SELECT * FROM sys_user where user_id=#{user.id} and create_time > #{createTime}', 0, 0, 0);
INSERT INTO sys_data_service_node (ds_part_id, ds_id, part_name, var_type, var_name, part_config, sort_by, part_type, is_del) VALUES (2044978518391132160, 1874451681413042176, '步骤2', 0, 'step2', 'function handle(data){data.user = [...data.user,...data.user];data.vvv=1;return data;}', 2, 1, 0);
INSERT INTO sys_data_service_node (ds_part_id, ds_id, part_name, var_type, var_name, part_config, sort_by, part_type, is_del) VALUES (2078486297990795264, 1874451681413042176, '步骤3', 0, 'param3', 'SELECT
    d.dept_id,
    d.dept_name,
    COUNT(u.user_id) AS user_count
FROM sys_dept d
LEFT JOIN sys_user u
    ON u.dept_id = d.dept_id
    AND (u.is_del IS NULL OR u.is_del = 0)
WHERE (d.is_del = 0 OR d.is_del IS NULL)
  AND d.dept_id = #{sys.deptId}
GROUP BY d.dept_id, d.dept_name
ORDER BY d.dept_id', 1, 0, 0);


-- ---------------------------- sys_third_party_mapping ----------------------------
DROP TABLE IF EXISTS sys_third_party_mapping;
CREATE TABLE sys_third_party_mapping (
  "id" bigint NOT NULL AUTO_INCREMENT,
  "provider" varchar(50) NOT NULL,
  "entity_type" varchar(50) NOT NULL,
  "third_party_id" varchar(100) NOT NULL,
  "local_id" varchar(100) NOT NULL,
  "display_name" varchar(200) DEFAULT NULL,
  "create_time" timestamp NOT NULL,
  PRIMARY KEY (id)
);
DELETE FROM sys_third_party_mapping;
INSERT INTO sys_third_party_mapping (id, provider, entity_type, third_party_id, local_id, display_name, create_time) VALUES (1, 'DingTalk', 'Dept', '1085091401', '2066408560685879296', '技术研发部', '2026-06-15 14:32:48');
INSERT INTO sys_third_party_mapping (id, provider, entity_type, third_party_id, local_id, display_name, create_time) VALUES (2, 'DingTalk', 'Dept', '1084505570', '2066408561927393280', '销售部', '2026-06-15 14:32:48');
INSERT INTO sys_third_party_mapping (id, provider, entity_type, third_party_id, local_id, display_name, create_time) VALUES (3, 'DingTalk', 'Dept', '1084845536', '2066408562711728128', '生产部', '2026-06-15 14:32:48');
INSERT INTO sys_third_party_mapping (id, provider, entity_type, third_party_id, local_id, display_name, create_time) VALUES (4, 'DingTalk', 'Dept', '1084944419', '2066408563538006016', '供应链部', '2026-06-15 14:32:48');
INSERT INTO sys_third_party_mapping (id, provider, entity_type, third_party_id, local_id, display_name, create_time) VALUES (5, 'DingTalk', 'Dept', '1084942359', '2066408565375111168', '财务部', '2026-06-15 14:32:49');
INSERT INTO sys_third_party_mapping (id, provider, entity_type, third_party_id, local_id, display_name, create_time) VALUES (6, 'DingTalk', 'Dept', '1084562550', '2066408566247526400', '设备维护科', '2026-06-15 14:32:49');
INSERT INTO sys_third_party_mapping (id, provider, entity_type, third_party_id, local_id, display_name, create_time) VALUES (7, 'DingTalk', 'Dept', '1084566691', '2066408567036055552', '一车间', '2026-06-15 14:32:49');
INSERT INTO sys_third_party_mapping (id, provider, entity_type, third_party_id, local_id, display_name, create_time) VALUES (8, 'DingTalk', 'Dept', '1084619690', '2066408567828779008', '二车间', '2026-06-15 14:32:49');
INSERT INTO sys_third_party_mapping (id, provider, entity_type, third_party_id, local_id, display_name, create_time) VALUES (9, 'DingTalk', 'Dept', '1084978430', '2066408570584436736', '仓储科', '2026-06-15 14:32:50');
INSERT INTO sys_third_party_mapping (id, provider, entity_type, third_party_id, local_id, display_name, create_time) VALUES (10, 'DingTalk', 'Dept', '1084898355', '2066408574942318592', '采购科', '2026-06-15 14:32:51');
INSERT INTO sys_third_party_mapping (id, provider, entity_type, third_party_id, local_id, display_name, create_time) VALUES (11, 'DingTalk', 'User', '115750566821596162', '2066408667003097088', '周春伟', '2026-06-15 14:33:13');
INSERT INTO sys_third_party_mapping (id, provider, entity_type, third_party_id, local_id, display_name, create_time) VALUES (12, 'DingTalk', 'Dept', '1', '2066415154064658432', '二狗子元宇宙养猪研讨会', '2026-06-15 14:59:00');
INSERT INTO sys_third_party_mapping (id, provider, entity_type, third_party_id, local_id, display_name, create_time) VALUES (13, 'DingTalk', 'User', '01326460133131937855', '2066415189598801920', '缪俊伟', '2026-06-15 14:59:08');
INSERT INTO sys_third_party_mapping (id, provider, entity_type, third_party_id, local_id, display_name, create_time) VALUES (14, 'DingTalk', 'User', '26026556651233159', '2066415191788228608', '陈鹏', '2026-06-15 14:59:09');
INSERT INTO sys_third_party_mapping (id, provider, entity_type, third_party_id, local_id, display_name, create_time) VALUES (15, 'DingTalk', 'User', '115168372538173315', '2066415192656449536', '顾京花', '2026-06-15 14:59:09');
INSERT INTO sys_third_party_mapping (id, provider, entity_type, third_party_id, local_id, display_name, create_time) VALUES (16, 'DingTalk', 'User', '07076241321077597', '2066415193549836288', '董楠', '2026-06-15 14:59:09');


-- ---------------------------- sys_ai_model ----------------------------
DROP TABLE IF EXISTS sys_ai_model;
CREATE TABLE sys_ai_model (
  "id" bigint NOT NULL,
  "provider" varchar(100) NOT NULL,
  "model_id" varchar(100) NOT NULL,
  "display_name" varchar(100) NOT NULL,
  "api_key" varchar(200) NOT NULL,
  "base_url" varchar(300) NOT NULL,
  "is_default" smallint NOT NULL,
  "sort" integer NOT NULL,
  "created_at" timestamp NOT NULL,
  "updated_at" timestamp NOT NULL,
  "max_context" integer NOT NULL,
  "capabilities" integer NOT NULL,
  "model_type" varchar(20) NOT NULL,
  PRIMARY KEY (id)
);
COMMENT ON COLUMN sys_ai_model.id IS '主键ID';
COMMENT ON COLUMN sys_ai_model.provider IS '提供商名称（分组键，同一名称共享同一接口地址）';
COMMENT ON COLUMN sys_ai_model.model_id IS '模型ID';
COMMENT ON COLUMN sys_ai_model.display_name IS '模型显示名称';
COMMENT ON COLUMN sys_ai_model.api_key IS 'API密钥';
COMMENT ON COLUMN sys_ai_model.base_url IS '接口地址（OpenAI兼容）';
COMMENT ON COLUMN sys_ai_model.is_default IS '是否默认模型（全表唯一，1是 0否）';
COMMENT ON COLUMN sys_ai_model.sort IS '排序号';
COMMENT ON COLUMN sys_ai_model.created_at IS '创建时间';
COMMENT ON COLUMN sys_ai_model.updated_at IS '更新时间';
COMMENT ON COLUMN sys_ai_model.max_context IS '最大上下文窗口（token）';
COMMENT ON COLUMN sys_ai_model.capabilities IS '能力位掩码（见 AiModelCapability）';
COMMENT ON COLUMN sys_ai_model.model_type IS '模型类型（llm=大语言模型 vision=视觉模型）';
DELETE FROM sys_ai_model;
INSERT INTO sys_ai_model (id, provider, model_id, display_name, api_key, base_url, is_default, sort, created_at, updated_at, max_context, capabilities, model_type) VALUES (2075931753100480512, 'deepseek', 'deepseek-chat', 'DeepSeek Chat', '', 'https://api.deepseek.com/v1', 1, 1, '2026-07-11 21:14:34', '2026-07-11 21:14:34', 0, 401, '');
INSERT INTO sys_ai_model (id, provider, model_id, display_name, api_key, base_url, is_default, sort, created_at, updated_at, max_context, capabilities, model_type) VALUES (2075931753113063424, 'deepseek', 'deepseek-reasoner', 'DeepSeek Reasoner', '', 'https://api.deepseek.com/v1', 0, 2, '2026-07-11 21:14:34', '2026-07-11 21:14:34', 0, 401, '');
INSERT INTO sys_ai_model (id, provider, model_id, display_name, api_key, base_url, is_default, sort, created_at, updated_at, max_context, capabilities, model_type) VALUES (2075931753113063425, 'qwen', 'qwen-plus', '通义千问 Plus', '', 'https://dashscope.aliyuncs.com/compatible-mode/v1', 1, 3, '2026-07-11 21:14:34', '2026-07-11 21:14:34', 0, 401, '');
INSERT INTO sys_ai_model (id, provider, model_id, display_name, api_key, base_url, is_default, sort, created_at, updated_at, max_context, capabilities, model_type) VALUES (2075931753113063426, 'qwen', 'qwen-max', '通义千问 Max', '', 'https://dashscope.aliyuncs.com/compatible-mode/v1', 0, 4, '2026-07-11 21:14:34', '2026-07-11 21:14:34', 0, 401, '');
INSERT INTO sys_ai_model (id, provider, model_id, display_name, api_key, base_url, is_default, sort, created_at, updated_at, max_context, capabilities, model_type) VALUES (2075931753113063427, 'glm', 'glm-4-flash', 'GLM-4-Flash', '', 'https://open.bigmodel.cn/api/paas/v4', 1, 5, '2026-07-11 21:14:34', '2026-07-11 21:14:34', 0, 401, '');
INSERT INTO sys_ai_model (id, provider, model_id, display_name, api_key, base_url, is_default, sort, created_at, updated_at, max_context, capabilities, model_type) VALUES (2075931753113063428, 'openai', 'gpt-4o-mini', 'GPT-4o Mini', '', 'https://api.openai.com/v1', 1, 6, '2026-07-11 21:14:34', '2026-07-11 21:14:34', 0, 401, '');
INSERT INTO sys_ai_model (id, provider, model_id, display_name, api_key, base_url, is_default, sort, created_at, updated_at, max_context, capabilities, model_type) VALUES (2075931753113063429, 'openai', 'gpt-4o', 'GPT-4o', '', 'https://api.openai.com/v1', 0, 7, '2026-07-11 21:14:34', '2026-07-11 21:14:34', 0, 401, '');
INSERT INTO sys_ai_model (id, provider, model_id, display_name, api_key, base_url, is_default, sort, created_at, updated_at, max_context, capabilities, model_type) VALUES (2075931753113063666, 'codebuddy', 'auto-chat', 'auto-chat', '123456', 'http://127.0.0.1:8001/codebuddy/v1', 1, 8, '2026-07-22 00:00:00', '2026-07-22 00:00:00', 0, 401, '');
INSERT INTO sys_ai_model (id, provider, model_id, display_name, api_key, base_url, is_default, sort, created_at, updated_at, max_context, capabilities, model_type) VALUES (2075931753113063888, 'qwen', 'qwen3.7-max-2026-05-20', 'qwen3.7-max', 'sk-ws-H.EDXXHDX.tw2v.MEYCIQD6mepbudQ5iAXMhzTCAxwh6pz4T_uCO0J-vRnom9xx0wIhANYc12MdfceccY8bnZMuU7hna0pdCSTCgqs4E1yHGnEo', 'https://ws-gz3rqoybjc6el9i6.cn-beijing.maas.aliyuncs.com/compatible-mode/v1', 1, 8, '2026-07-22 00:00:00', '2026-07-22 00:00:00', 0, 401, '');
