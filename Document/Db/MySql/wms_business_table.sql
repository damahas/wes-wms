-- =============================================
-- WMS 仓库管理系统 业务表结构 V3.0（MySQL）
-- 从数据库导出：2026-11-17，共 42 张 wms 业务表
-- 与后端 SqlSugar 实体一一对应
-- =============================================
SET NAMES utf8mb4;
SET FOREIGN_KEY_CHECKS = 0;

-- ---------------------------- wms_adjust ----------------------------
DROP TABLE IF EXISTS `wms_adjust`;
CREATE TABLE `wms_adjust` (
  `adjust_id` bigint(20) NOT NULL COMMENT '调整id',
  `adjust_no` varchar(50) NOT NULL COMMENT '调整单号',
  `adjust_type` varchar(20) NOT NULL COMMENT '调整类型 check_profit-盘盈 check_loss-盘亏 damage-报损 overflow-报溢 validity-有效期调整 other-其他',
  `warehouse_id` bigint(20) NOT NULL COMMENT '仓库id',
  `zone_id` bigint(20) DEFAULT NULL COMMENT '库区id',
  `source_type` varchar(20) DEFAULT NULL COMMENT '来源类型 check-盘点 manual-人工',
  `source_id` bigint(20) DEFAULT NULL COMMENT '来源id',
  `total_quantity` int(11) DEFAULT NULL COMMENT '调整总数量',
  `adjust_reason` varchar(255) DEFAULT NULL COMMENT '调整原因',
  `adjust_status` varchar(20) DEFAULT NULL COMMENT '调整状态 pending-待审核 approved-已审核 completed-已完成',
  `approve_id` bigint(20) DEFAULT NULL COMMENT '审核人id',
  `approve_time` datetime DEFAULT NULL COMMENT '审核时间',
  `is_del` int(11) DEFAULT '0' COMMENT '删除标志',
  `create_by` varchar(64) DEFAULT NULL COMMENT '创建者',
  `create_time` datetime DEFAULT NULL COMMENT '创建时间',
  `update_by` varchar(64) DEFAULT NULL COMMENT '更新者',
  `update_time` datetime DEFAULT NULL COMMENT '更新时间',
  `remark` varchar(500) DEFAULT NULL COMMENT '备注',
  PRIMARY KEY (`adjust_id`),
  UNIQUE KEY `uk_adjust_no` (`adjust_no`) USING BTREE,
  KEY `idx_warehouse_id` (`warehouse_id`),
  KEY `idx_zone_id` (`zone_id`),
  KEY `idx_source_id` (`source_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='库存调整单表';

-- ---------------------------- wms_adjust_detail ----------------------------
DROP TABLE IF EXISTS `wms_adjust_detail`;
CREATE TABLE `wms_adjust_detail` (
  `detail_id` bigint(20) NOT NULL COMMENT '明细id',
  `adjust_id` bigint(20) NOT NULL COMMENT '调整id',
  `material_id` bigint(20) NOT NULL COMMENT '物料id',
  `batch_no` varchar(50) DEFAULT NULL COMMENT '批次号',
  `zone_id` bigint(20) DEFAULT NULL COMMENT '库区id',
  `location_id` bigint(20) DEFAULT NULL COMMENT '库位id',
  `before_quantity` int(11) NOT NULL COMMENT '调整前数量',
  `adjust_quantity` int(11) NOT NULL COMMENT '调整数量',
  `after_quantity` int(11) NOT NULL COMMENT '调整后数量',
  `is_del` int(11) DEFAULT '0' COMMENT '删除标志',
  `create_by` varchar(64) DEFAULT NULL COMMENT '创建者',
  `create_time` datetime DEFAULT NULL COMMENT '创建时间',
  `update_by` varchar(64) DEFAULT NULL COMMENT '更新者',
  `update_time` datetime DEFAULT NULL COMMENT '更新时间',
  `stock_id` bigint(20) DEFAULT NULL COMMENT '库存行id（空=盘盈新增）',
  PRIMARY KEY (`detail_id`),
  KEY `idx_adjust_id` (`adjust_id`),
  KEY `idx_material_id` (`material_id`),
  KEY `idx_zone_id` (`zone_id`),
  KEY `idx_location_id` (`location_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='库存调整明细表';

-- ---------------------------- wms_delivery ----------------------------
DROP TABLE IF EXISTS `wms_delivery`;
CREATE TABLE `wms_delivery` (
  `delivery_id` bigint(20) NOT NULL COMMENT '出库id',
  `delivery_no` varchar(50) NOT NULL COMMENT '出库单号',
  `warehouse_id` bigint(20) NOT NULL COMMENT '仓库id',
  `zone_id` bigint(20) DEFAULT NULL COMMENT '发货库区id',
  `customer_id` bigint(20) DEFAULT NULL COMMENT '客户id',
  `delivery_date` datetime NOT NULL COMMENT '出库日期',
  `total_quantity` int(11) DEFAULT NULL COMMENT '总数量',
  `total_amount` decimal(18,4) DEFAULT NULL COMMENT '总金额',
  `carrier_name` varchar(100) DEFAULT NULL COMMENT '承运商',
  `waybill_no` varchar(100) DEFAULT NULL COMMENT '运单号',
  `package_count` int(11) DEFAULT NULL COMMENT '包裹数量',
  `total_weight` decimal(10,2) DEFAULT NULL COMMENT '总重量',
  `delivery_status` varchar(20) DEFAULT NULL COMMENT '出库状态 pending-待确认 shipped-已发货 delivered-已签收',
  `ship_time` datetime DEFAULT NULL COMMENT '发货时间',
  `sign_time` datetime DEFAULT NULL COMMENT '签收时间',
  `operator_id` bigint(20) DEFAULT NULL COMMENT '操作人id',
  `is_del` int(11) DEFAULT '0' COMMENT '删除标志',
  `create_by` varchar(64) DEFAULT NULL COMMENT '创建者',
  `create_time` datetime DEFAULT NULL COMMENT '创建时间',
  `update_by` varchar(64) DEFAULT NULL COMMENT '更新者',
  `update_time` datetime DEFAULT NULL COMMENT '更新时间',
  `source_type` varchar(20) DEFAULT NULL COMMENT '来源单据类型 sale-销售出库订单 issue-领料单 manual-手工创建',
  `source_id` bigint(20) DEFAULT NULL COMMENT '来源单据id',
  `source_no` varchar(50) DEFAULT NULL COMMENT '来源单据号',
  PRIMARY KEY (`delivery_id`),
  UNIQUE KEY `uk_delivery_no` (`delivery_no`) USING BTREE,
  KEY `idx_warehouse_id` (`warehouse_id`),
  KEY `idx_zone_id` (`zone_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='出库单主表';

-- ---------------------------- wms_delivery_detail ----------------------------
DROP TABLE IF EXISTS `wms_delivery_detail`;
CREATE TABLE `wms_delivery_detail` (
  `detail_id` bigint(20) NOT NULL COMMENT '明细id',
  `delivery_id` bigint(20) NOT NULL COMMENT '出库id',
  `material_id` bigint(20) NOT NULL COMMENT '物料id',
  `spec` varchar(100) DEFAULT NULL COMMENT '规格',
  `unit` varchar(20) DEFAULT NULL COMMENT '单位',
  `batch_no` varchar(50) DEFAULT NULL COMMENT '批次号',
  `zone_id` bigint(20) DEFAULT NULL COMMENT '库区id',
  `location_id` bigint(20) DEFAULT NULL COMMENT '库位id',
  `delivery_quantity` int(11) NOT NULL COMMENT '出库数量',
  `unit_price` decimal(15,4) DEFAULT NULL COMMENT '单价',
  `amount` decimal(15,2) DEFAULT NULL COMMENT '金额',
  `is_del` int(11) DEFAULT '0' COMMENT '删除标志',
  `create_by` varchar(64) DEFAULT NULL COMMENT '创建者',
  `create_time` datetime DEFAULT NULL COMMENT '创建时间',
  `update_by` varchar(64) DEFAULT NULL COMMENT '更新者',
  `update_time` datetime DEFAULT NULL COMMENT '更新时间',
  PRIMARY KEY (`detail_id`),
  KEY `idx_delivery_id` (`delivery_id`),
  KEY `idx_material_id` (`material_id`),
  KEY `idx_zone_id` (`zone_id`),
  KEY `idx_location_id` (`location_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='出库单明细表';

-- ---------------------------- wms_freeze ----------------------------
DROP TABLE IF EXISTS `wms_freeze`;
CREATE TABLE `wms_freeze` (
  `freeze_id` bigint(20) NOT NULL COMMENT '冻结id',
  `freeze_no` varchar(50) DEFAULT NULL COMMENT '冻结单号',
  `material_id` bigint(20) NOT NULL COMMENT '物料id',
  `batch_no` varchar(50) DEFAULT NULL COMMENT '批次号',
  `warehouse_id` bigint(20) NOT NULL COMMENT '仓库id',
  `zone_id` bigint(20) DEFAULT NULL COMMENT '库区id',
  `location_id` bigint(20) DEFAULT NULL COMMENT '库位id',
  `freeze_quantity` int(11) NOT NULL COMMENT '冻结数量',
  `freeze_type` varchar(20) DEFAULT NULL COMMENT '冻结类型 quality-质量隔离 legal-司法查封 reserve-预留锁定 other-其他',
  `freeze_reason` varchar(255) DEFAULT NULL COMMENT '冻结原因',
  `freeze_status` varchar(20) DEFAULT NULL COMMENT '冻结状态 frozen-已冻结 thawed-已解冻',
  `freeze_user_id` bigint(20) DEFAULT NULL COMMENT '冻结人id',
  `freeze_time` datetime DEFAULT NULL COMMENT '冻结时间',
  `thaw_user_id` bigint(20) DEFAULT NULL COMMENT '解冻人id',
  `thaw_time` datetime DEFAULT NULL COMMENT '解冻时间',
  `thaw_reason` varchar(255) DEFAULT NULL COMMENT '解冻原因',
  `is_del` int(11) DEFAULT '0' COMMENT '删除标志',
  `create_by` varchar(64) DEFAULT NULL COMMENT '创建者',
  `create_time` datetime DEFAULT NULL COMMENT '创建时间',
  `update_by` varchar(64) DEFAULT NULL COMMENT '更新者',
  `update_time` datetime DEFAULT NULL COMMENT '更新时间',
  `stock_id` bigint(20) DEFAULT NULL COMMENT '库存行id',
  PRIMARY KEY (`freeze_id`),
  KEY `idx_material_id` (`material_id`),
  KEY `idx_zone_id` (`zone_id`),
  KEY `idx_location_id` (`location_id`),
  KEY `idx_batch_no` (`batch_no`),
  KEY `idx_freeze_status` (`freeze_status`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='库存冻结表';

-- ---------------------------- wms_inspect ----------------------------
DROP TABLE IF EXISTS `wms_inspect`;
CREATE TABLE `wms_inspect` (
  `inspect_id` bigint(20) NOT NULL COMMENT '验收id',
  `inspect_no` varchar(50) NOT NULL COMMENT '验收单号',
  `receiving_id` bigint(20) DEFAULT NULL COMMENT '收货单id',
  `supplier_id` bigint(20) DEFAULT NULL COMMENT '供应商id',
  `inspect_date` datetime NOT NULL COMMENT '验收日期',
  `inspector_id` bigint(20) DEFAULT NULL COMMENT '验收人id',
  `total_plan_quantity` int(11) DEFAULT NULL COMMENT '计划数量',
  `total_actual_quantity` int(11) DEFAULT NULL COMMENT '实际数量',
  `total_qualified_quantity` int(11) DEFAULT NULL COMMENT '合格数量',
  `total_unqualified_quantity` int(11) DEFAULT NULL COMMENT '不合格数量',
  `inspect_result` varchar(20) DEFAULT NULL COMMENT '验收结果 all_pass-全部合格 part_pass-部分合格 all_fail-全部不合格',
  `is_del` int(11) DEFAULT '0' COMMENT '删除标志',
  `create_by` varchar(64) DEFAULT NULL COMMENT '创建者',
  `create_time` datetime DEFAULT NULL COMMENT '创建时间',
  `update_by` varchar(64) DEFAULT NULL COMMENT '更新者',
  `update_time` datetime DEFAULT NULL COMMENT '更新时间',
  PRIMARY KEY (`inspect_id`),
  UNIQUE KEY `uk_inspect_no` (`inspect_no`) USING BTREE,
  KEY `idx_arrival_id` (`receiving_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='到货验收表';

-- ---------------------------- wms_inspect_detail ----------------------------
DROP TABLE IF EXISTS `wms_inspect_detail`;
CREATE TABLE `wms_inspect_detail` (
  `detail_id` bigint(20) NOT NULL COMMENT '明细id',
  `inspect_id` bigint(20) NOT NULL COMMENT '验收id',
  `material_id` bigint(20) NOT NULL COMMENT '物料id',
  `batch_no` varchar(50) DEFAULT NULL COMMENT '批次号',
  `plan_quantity` int(11) DEFAULT NULL COMMENT '计划数量',
  `actual_quantity` int(11) NOT NULL COMMENT '实际数量',
  `qualified_quantity` int(11) NOT NULL COMMENT '合格数量',
  `unqualified_quantity` int(11) DEFAULT '0' COMMENT '不合格数量',
  `unqualified_reason` varchar(255) DEFAULT NULL COMMENT '不合格原因',
  `unit_price` decimal(15,4) DEFAULT NULL COMMENT '单价',
  `amount` decimal(15,2) DEFAULT NULL COMMENT '金额',
  `is_del` int(11) DEFAULT '0' COMMENT '删除标志',
  `create_by` varchar(64) DEFAULT NULL COMMENT '创建者',
  `create_time` datetime DEFAULT NULL COMMENT '创建时间',
  `update_by` varchar(64) DEFAULT NULL COMMENT '更新者',
  `update_time` datetime DEFAULT NULL COMMENT '更新时间',
  PRIMARY KEY (`detail_id`),
  KEY `idx_inspect_id` (`inspect_id`),
  KEY `idx_material_id` (`material_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='到货验收明细表';

-- ---------------------------- wms_iqc_item ----------------------------
DROP TABLE IF EXISTS `wms_iqc_item`;
CREATE TABLE `wms_iqc_item` (
  `item_id` bigint(20) NOT NULL COMMENT '检验项id',
  `category_id` varchar(50) NOT NULL COMMENT '物料分类',
  `item_code` varchar(50) DEFAULT NULL COMMENT '检验项编码',
  `item_name` varchar(100) NOT NULL COMMENT '检验项名称',
  `inspection_type` varchar(30) DEFAULT NULL COMMENT '检验类型',
  `standard_value` varchar(100) DEFAULT NULL COMMENT '规格/标准值',
  `unit` varchar(20) DEFAULT NULL COMMENT '单位',
  `is_critical` int(11) DEFAULT NULL COMMENT '是否关键项',
  `test_method` varchar(200) DEFAULT NULL COMMENT '检测方法/工具',
  `sort_no` int(11) DEFAULT NULL COMMENT '排序号',
  `status` varchar(5) DEFAULT NULL COMMENT '状态',
  `remark` varchar(500) DEFAULT NULL COMMENT '备注',
  `is_del` int(11) NOT NULL COMMENT '软删除标志',
  `create_by` varchar(50) DEFAULT NULL COMMENT '创建人',
  `create_time` datetime DEFAULT NULL COMMENT '创建时间',
  `update_by` varchar(50) DEFAULT NULL COMMENT '修改人',
  `update_time` datetime DEFAULT NULL COMMENT '修改时间',
  PRIMARY KEY (`item_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='检验项表';

-- ---------------------------- wms_iqc_order ----------------------------
DROP TABLE IF EXISTS `wms_iqc_order`;
CREATE TABLE `wms_iqc_order` (
  `inspection_id` bigint(20) NOT NULL COMMENT '检验单id',
  `inspection_no` varchar(50) DEFAULT NULL COMMENT '检验单编号',
  `receiving_id` bigint(20) NOT NULL COMMENT '收货单id',
  `receiving_detail_id` bigint(20) NOT NULL COMMENT '收货单明细id',
  `supplier_id` bigint(20) DEFAULT NULL COMMENT '供应商id',
  `material_id` bigint(20) NOT NULL COMMENT '物料id',
  `spec` varchar(100) DEFAULT NULL COMMENT '规格',
  `unit` varchar(20) DEFAULT NULL COMMENT '单位',
  `batch_no` varchar(50) DEFAULT NULL COMMENT '批次号',
  `delivery_quantity` int(11) DEFAULT NULL COMMENT '收货数量',
  `allow_quantity` int(11) DEFAULT '0' COMMENT '允许入库数量',
  `defect_quantity` int(11) DEFAULT '0' COMMENT '不良数量',
  `return_quantity` int(11) DEFAULT '0' COMMENT '退货数量',
  `dispose_type` varchar(20) DEFAULT 'return' COMMENT '不良品处置方式 return-退回供应商 concession-让步接收 sorting-挑选使用',
  `inspect_status` varchar(20) DEFAULT NULL COMMENT '状态 pending-待检验 inspecting-检验中 completed-已完成',
  `inspect_date` datetime DEFAULT NULL COMMENT '检验日期',
  `inspector` varchar(50) DEFAULT NULL COMMENT '检验人',
  `remark` varchar(500) DEFAULT NULL COMMENT '备注',
  `is_del` int(11) DEFAULT '0' COMMENT '删除标志',
  `create_by` varchar(64) DEFAULT NULL COMMENT '创建者',
  `create_time` datetime DEFAULT NULL COMMENT '创建时间',
  `update_by` varchar(64) DEFAULT NULL COMMENT '更新者',
  `update_time` datetime DEFAULT NULL COMMENT '更新时间',
  PRIMARY KEY (`inspection_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='检验单';

-- ---------------------------- wms_iqc_order_item ----------------------------
DROP TABLE IF EXISTS `wms_iqc_order_item`;
CREATE TABLE `wms_iqc_order_item` (
  `item_id` bigint(20) NOT NULL COMMENT '检验项id',
  `inspection_id` bigint(20) NOT NULL COMMENT '检验单id',
  `item_code` varchar(50) DEFAULT NULL COMMENT '检验项编码',
  `item_name` varchar(100) NOT NULL COMMENT '检验项名称',
  `inspection_type` varchar(30) DEFAULT NULL COMMENT '检验类型',
  `standard_value` varchar(100) DEFAULT NULL COMMENT '标准值',
  `unit` varchar(20) DEFAULT NULL COMMENT '单位',
  `is_critical` int(11) DEFAULT NULL COMMENT '是否关键项',
  `test_method` varchar(200) DEFAULT NULL COMMENT '检测方法/工具',
  `inspect_result` varchar(500) DEFAULT NULL COMMENT '检验结果',
  `is_passed` tinyint(1) DEFAULT NULL COMMENT '是否通过',
  `sort_no` int(11) DEFAULT NULL COMMENT '排序号',
  `is_del` int(11) DEFAULT '0' COMMENT '删除标志',
  `create_by` varchar(64) DEFAULT NULL COMMENT '创建者',
  `create_time` datetime DEFAULT NULL COMMENT '创建时间',
  `update_by` varchar(64) DEFAULT NULL COMMENT '更新者',
  `update_time` datetime DEFAULT NULL COMMENT '更新时间',
  PRIMARY KEY (`item_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='检验单检验项表';

-- ---------------------------- wms_issue_order ----------------------------
DROP TABLE IF EXISTS `wms_issue_order`;
CREATE TABLE `wms_issue_order` (
  `issue_id` bigint(20) NOT NULL COMMENT '出库申请单id',
  `issue_no` varchar(50) DEFAULT NULL COMMENT '出库申请单号',
  `source_type` varchar(20) DEFAULT NULL COMMENT '来源类型 mes-MES下发 manual-手工新建',
  `source_no` varchar(50) DEFAULT NULL COMMENT '来源单据号',
  `work_order_no` varchar(50) DEFAULT NULL COMMENT '生产工单号',
  `dept_name` varchar(100) DEFAULT NULL COMMENT '领用部门/车间',
  `receiver_id` bigint(20) DEFAULT NULL COMMENT '领用人id',
  `receiver_name` varchar(50) DEFAULT NULL COMMENT '领用人',
  `warehouse_id` bigint(20) DEFAULT NULL COMMENT '仓库id',
  `warehouse_name` varchar(100) DEFAULT NULL COMMENT '仓库名称',
  `require_date` datetime DEFAULT NULL COMMENT '需求日期',
  `total_quantity` int(11) DEFAULT '0' COMMENT '需求总数量',
  `issued_quantity` int(11) DEFAULT '0' COMMENT '已发总数量',
  `shortage_quantity` int(11) DEFAULT '0' COMMENT '欠料总数量',
  `issue_status` varchar(20) DEFAULT 'pending' COMMENT '发料状态 pending-待发料 partial-部分发料 issued-已发完 cancelled-已取消',
  `remark` varchar(500) DEFAULT NULL COMMENT '备注',
  `is_del` int(11) DEFAULT '0' COMMENT '删除标志',
  `create_by` varchar(64) DEFAULT NULL COMMENT '创建者',
  `create_time` datetime DEFAULT NULL COMMENT '创建时间',
  `update_by` varchar(64) DEFAULT NULL COMMENT '更新者',
  `update_time` datetime DEFAULT NULL COMMENT '更新时间',
  `outbound_type` varchar(20) DEFAULT 'issue' COMMENT '出库类型 issue-生产领料 sales-销售发货 other-其他',
  `customer_name` varchar(100) DEFAULT NULL COMMENT '客户名称',
  PRIMARY KEY (`issue_id`),
  UNIQUE KEY `uk_issue_no` (`issue_no`),
  KEY `idx_work_order_no` (`work_order_no`),
  KEY `idx_warehouse_id` (`warehouse_id`),
  KEY `idx_issue_status` (`issue_status`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='出库申请单';

-- ---------------------------- wms_issue_order_detail ----------------------------
DROP TABLE IF EXISTS `wms_issue_order_detail`;
CREATE TABLE `wms_issue_order_detail` (
  `detail_id` bigint(20) NOT NULL COMMENT '明细id',
  `issue_id` bigint(20) NOT NULL COMMENT '领料单id',
  `material_id` bigint(20) NOT NULL COMMENT '物料id',
  `material_code` varchar(50) DEFAULT NULL COMMENT '物料编码',
  `material_name` varchar(100) DEFAULT NULL COMMENT '物料名称',
  `spec` varchar(100) DEFAULT NULL COMMENT '规格',
  `unit` varchar(20) DEFAULT NULL COMMENT '单位',
  `batch_no` varchar(50) DEFAULT NULL COMMENT '指定批次号',
  `require_quantity` int(11) DEFAULT '0' COMMENT '需求数量',
  `issued_quantity` int(11) DEFAULT '0' COMMENT '已发数量',
  `shortage_quantity` int(11) DEFAULT NULL COMMENT '欠料数量',
  `remark` varchar(500) DEFAULT NULL COMMENT '备注',
  `is_del` int(11) DEFAULT '0' COMMENT '删除标志',
  `create_by` varchar(64) DEFAULT NULL COMMENT '创建者',
  `create_time` datetime DEFAULT NULL COMMENT '创建时间',
  `update_by` varchar(64) DEFAULT NULL COMMENT '更新者',
  `update_time` datetime DEFAULT NULL COMMENT '更新时间',
  PRIMARY KEY (`detail_id`),
  KEY `idx_issue_id` (`issue_id`),
  KEY `idx_material_id` (`material_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='领料单明细';

-- ---------------------------- wms_location ----------------------------
DROP TABLE IF EXISTS `wms_location`;
CREATE TABLE `wms_location` (
  `location_id` bigint(20) NOT NULL COMMENT '库位id',
  `warehouse_id` bigint(20) NOT NULL COMMENT '仓库id',
  `zone_id` bigint(20) NOT NULL COMMENT '库区id',
  `location_code` varchar(50) NOT NULL COMMENT '库位编码',
  `location_name` varchar(100) DEFAULT NULL COMMENT '库位名称',
  `location_type` varchar(20) DEFAULT NULL COMMENT '库位类型 storage-存储区 picking-拣货区 temp-暂存区 return-退货区',
  `location_attr` varchar(50) DEFAULT NULL COMMENT '库位属性 normal-常温 cold-低温 damp-防潮 dangerous-危险品',
  `shelf_code` varchar(20) DEFAULT NULL COMMENT '货架编号',
  `length` decimal(18,4) DEFAULT NULL COMMENT '长度',
  `width` decimal(18,4) DEFAULT NULL COMMENT '宽度',
  `height` decimal(18,4) DEFAULT NULL COMMENT '高度',
  `max_weight` decimal(18,4) DEFAULT NULL COMMENT '最大承重',
  `max_volume` decimal(18,4) DEFAULT NULL COMMENT '最大容量',
  `max_quantity` int(11) DEFAULT NULL COMMENT '最大存储数量',
  `location_status` varchar(1) DEFAULT NULL COMMENT '库位状态',
  `create_by` varchar(64) DEFAULT NULL COMMENT '创建者',
  `create_time` datetime DEFAULT NULL COMMENT '创建时间',
  `update_by` varchar(64) DEFAULT NULL COMMENT '更新者',
  `update_time` datetime DEFAULT NULL COMMENT '更新时间',
  `remark` varchar(500) DEFAULT NULL COMMENT '备注',
  `is_del` int(11) DEFAULT '0' COMMENT '删除标志',
  PRIMARY KEY (`location_id`),
  UNIQUE KEY `uk_location_code` (`location_code`) USING BTREE,
  KEY `idx_warehouse_id` (`warehouse_id`),
  KEY `idx_zone_id` (`zone_id`),
  KEY `idx_status` (`location_status`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='库位表';

-- ---------------------------- wms_location_lock ----------------------------
DROP TABLE IF EXISTS `wms_location_lock`;
CREATE TABLE `wms_location_lock` (
  `lock_id` bigint(20) NOT NULL COMMENT '锁定id',
  `warehouse_id` bigint(20) DEFAULT NULL COMMENT '仓库id',
  `zone_id` bigint(20) DEFAULT NULL COMMENT '库区id',
  `location_id` bigint(20) NOT NULL COMMENT '库位id',
  `lock_type` varchar(20) NOT NULL COMMENT '锁定类型 check-盘点异常 maintain-维护 other-其他',
  `lock_reason` varchar(255) DEFAULT NULL COMMENT '锁定原因',
  `lock_user_id` bigint(20) DEFAULT NULL COMMENT '锁定人id',
  `lock_time` datetime DEFAULT NULL COMMENT '锁定时间',
  `unlock_user_id` bigint(20) DEFAULT NULL COMMENT '解锁人id',
  `unlock_time` datetime DEFAULT NULL COMMENT '解锁时间',
  `is_del` int(11) DEFAULT '0' COMMENT '删除标志',
  `create_by` varchar(64) DEFAULT NULL COMMENT '创建者',
  `create_time` datetime DEFAULT NULL COMMENT '创建时间',
  `update_by` varchar(64) DEFAULT NULL COMMENT '更新者',
  `update_time` datetime DEFAULT NULL COMMENT '更新时间',
  PRIMARY KEY (`lock_id`),
  KEY `idx_location_id` (`location_id`),
  KEY `idx_zone_id` (`zone_id`),
  KEY `idx_lock_type` (`lock_type`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='库位锁定记录表';

-- ---------------------------- wms_lock ----------------------------
DROP TABLE IF EXISTS `wms_lock`;
CREATE TABLE `wms_lock` (
  `lock_id` bigint(20) NOT NULL COMMENT '锁定id',
  `lock_type` varchar(20) DEFAULT NULL COMMENT '锁定类型 order-订单预留 produce-生产预留 exhibit-展会预留 manual-手动锁定',
  `source_type` varchar(20) DEFAULT NULL COMMENT '来源类型',
  `source_id` bigint(20) DEFAULT NULL COMMENT '来源id',
  `source_no` varchar(50) DEFAULT NULL COMMENT '来源单据号',
  `material_id` bigint(20) NOT NULL COMMENT '物料id',
  `batch_no` varchar(50) DEFAULT NULL COMMENT '批次号',
  `warehouse_id` bigint(20) NOT NULL COMMENT '仓库id',
  `zone_id` bigint(20) DEFAULT NULL COMMENT '库区id',
  `location_id` bigint(20) DEFAULT NULL COMMENT '库位id',
  `lock_quantity` int(11) NOT NULL COMMENT '锁定数量',
  `used_quantity` int(11) DEFAULT '0' COMMENT '已使用数量',
  `lock_status` varchar(20) DEFAULT NULL COMMENT '锁定状态 locked-已锁定 partial-部分释放 released-已释放',
  `lock_user_id` bigint(20) DEFAULT NULL COMMENT '锁定人id',
  `lock_time` datetime DEFAULT NULL COMMENT '锁定时间',
  `expire_time` datetime DEFAULT NULL COMMENT '过期时间',
  `is_del` int(11) DEFAULT '0' COMMENT '删除标志',
  `create_by` varchar(64) DEFAULT NULL COMMENT '创建者',
  `create_time` datetime DEFAULT NULL COMMENT '创建时间',
  `update_by` varchar(64) DEFAULT NULL COMMENT '更新者',
  `update_time` datetime DEFAULT NULL COMMENT '更新时间',
  `stock_id` bigint(20) DEFAULT NULL COMMENT '库存行id',
  PRIMARY KEY (`lock_id`),
  KEY `idx_material_id` (`material_id`),
  KEY `idx_zone_id` (`zone_id`),
  KEY `idx_location_id` (`location_id`),
  KEY `idx_source_id` (`source_id`),
  KEY `idx_lock_status` (`lock_status`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='库存锁定表';

-- ---------------------------- wms_material ----------------------------
DROP TABLE IF EXISTS `wms_material`;
CREATE TABLE `wms_material` (
  `material_id` bigint(20) NOT NULL COMMENT '物料id',
  `material_code` varchar(50) NOT NULL COMMENT '物料编码',
  `material_name` varchar(100) NOT NULL COMMENT '物料名称',
  `material_short_name` varchar(50) DEFAULT NULL COMMENT '物料简称',
  `category_id` varchar(50) DEFAULT NULL COMMENT '物料分类',
  `spec` varchar(100) DEFAULT NULL COMMENT '规格型号',
  `barcode` varchar(100) DEFAULT NULL COMMENT '条码',
  `unit` varchar(20) DEFAULT NULL COMMENT '计量单位',
  `pack_spec_id` bigint(20) DEFAULT NULL COMMENT '包装规格id',
  `material_type` varchar(20) DEFAULT NULL COMMENT '物料类型',
  `default_warehouse_id` bigint(20) DEFAULT NULL COMMENT '默认仓库id',
  `default_zone_id` bigint(20) DEFAULT NULL COMMENT '默认库区id',
  `shelf_life` int(11) DEFAULT NULL COMMENT '保质期天数',
  `min_stock` decimal(18,4) DEFAULT NULL COMMENT '安全库存量',
  `max_stock` decimal(18,4) DEFAULT NULL COMMENT '最大库存量',
  `purchase_cycle` int(11) DEFAULT NULL COMMENT '采购周期',
  `weight` decimal(18,4) DEFAULT NULL COMMENT '重量',
  `volume` decimal(18,4) DEFAULT NULL COMMENT '体积',
  `image_url` varchar(500) DEFAULT NULL COMMENT '物料图片',
  `status` varchar(1) DEFAULT NULL COMMENT '状态',
  `is_del` int(11) DEFAULT '0' COMMENT '删除标志',
  `create_by` varchar(64) DEFAULT NULL COMMENT '创建者',
  `create_time` datetime DEFAULT NULL COMMENT '创建时间',
  `update_by` varchar(64) DEFAULT NULL COMMENT '更新者',
  `update_time` datetime DEFAULT NULL COMMENT '更新时间',
  `remark` varchar(500) DEFAULT NULL COMMENT '备注',
  `default_location_id` bigint(20) DEFAULT NULL COMMENT '默认库位id',
  PRIMARY KEY (`material_id`),
  UNIQUE KEY `uk_material_code` (`material_code`) USING BTREE,
  KEY `idx_category_id` (`category_id`),
  KEY `idx_barcode` (`barcode`),
  KEY `idx_status` (`status`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='物料表';

-- ---------------------------- wms_material_supplier ----------------------------
DROP TABLE IF EXISTS `wms_material_supplier`;
CREATE TABLE `wms_material_supplier` (
  `material_supplier_id` bigint(20) NOT NULL,
  `material_id` bigint(20) NOT NULL COMMENT '物料id',
  `supplier_id` bigint(20) NOT NULL COMMENT '供应商id',
  `moq` decimal(18,4) DEFAULT NULL COMMENT '最小起订量',
  `lead_time` int(11) DEFAULT NULL COMMENT '交货周期',
  `minor_qty` int(11) DEFAULT NULL COMMENT '次要缺陷数量上限',
  `major_qty` int(11) DEFAULT NULL COMMENT '主要缺陷数量上限',
  `critical_qty` int(11) DEFAULT NULL COMMENT '严重缺陷数量上限',
  `sample_ratio` decimal(5,2) DEFAULT NULL COMMENT '抽检比例%',
  `aql_level` varchar(20) DEFAULT NULL COMMENT 'AQL标准级别',
  `inspection_mode` varchar(20) DEFAULT NULL COMMENT '质检模式 exempt-免检 sampling-抽检 all-全检',
  `is_default` varchar(1) DEFAULT NULL COMMENT '是否默认',
  `status` varchar(1) DEFAULT NULL COMMENT '状态',
  `is_del` int(11) DEFAULT '0' COMMENT '逻辑删除标记',
  `create_by` varchar(64) DEFAULT NULL COMMENT '创建人',
  `create_time` datetime DEFAULT NULL COMMENT '创建时间',
  `update_by` varchar(64) DEFAULT NULL COMMENT '更新人',
  `update_time` datetime DEFAULT NULL COMMENT '更新时间',
  `remark` varchar(500) DEFAULT NULL COMMENT '备注',
  PRIMARY KEY (`material_supplier_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='物料供应商关系表';

-- ---------------------------- wms_move ----------------------------
DROP TABLE IF EXISTS `wms_move`;
CREATE TABLE `wms_move` (
  `move_id` bigint(20) NOT NULL COMMENT '移库id',
  `move_no` varchar(50) NOT NULL COMMENT '移库单号',
  `warehouse_id` bigint(20) NOT NULL COMMENT '仓库id',
  `move_type` varchar(20) DEFAULT NULL COMMENT '移库类型 adjust-库位调整 optimize-库位优化 zone_change-库区转移 sort-归类整理 split-拆层 merge-合并',
  `move_reason` varchar(255) DEFAULT NULL COMMENT '移库原因',
  `total_quantity` int(11) DEFAULT '0' COMMENT '移库总数量',
  `move_status` varchar(20) DEFAULT NULL COMMENT '移库状态 pending-待审核 approved-已审核 doing-移库中 completed-已完成 cancelled-已取消',
  `operator_id` bigint(20) DEFAULT NULL COMMENT '操作人id',
  `is_del` int(11) DEFAULT '0' COMMENT '删除标志',
  `create_by` varchar(64) DEFAULT NULL COMMENT '创建者',
  `create_time` datetime DEFAULT NULL COMMENT '创建时间',
  `update_by` varchar(64) DEFAULT NULL COMMENT '更新者',
  `update_time` datetime DEFAULT NULL COMMENT '更新时间',
  `remark` varchar(500) DEFAULT NULL COMMENT '备注',
  PRIMARY KEY (`move_id`),
  UNIQUE KEY `uk_move_no` (`move_no`) USING BTREE,
  KEY `idx_warehouse_id` (`warehouse_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='移库单表';

-- ---------------------------- wms_move_detail ----------------------------
DROP TABLE IF EXISTS `wms_move_detail`;
CREATE TABLE `wms_move_detail` (
  `detail_id` bigint(20) NOT NULL COMMENT '明细id',
  `move_id` bigint(20) NOT NULL COMMENT '移库id',
  `material_id` bigint(20) NOT NULL COMMENT '物料id',
  `batch_no` varchar(50) DEFAULT NULL COMMENT '批次号',
  `pack_level` varchar(30) DEFAULT NULL COMMENT '包装层级',
  `from_zone_id` bigint(20) NOT NULL COMMENT '源库区id',
  `from_location_id` bigint(20) NOT NULL COMMENT '原库位id',
  `to_zone_id` bigint(20) NOT NULL COMMENT '目标库区id',
  `to_location_id` bigint(20) NOT NULL COMMENT '目标库位id',
  `move_quantity` decimal(18,3) NOT NULL COMMENT '移库数量',
  `move_status` varchar(20) DEFAULT NULL COMMENT '状态 pending-待执行 completed-已完成',
  `move_time` datetime DEFAULT NULL COMMENT '移库时间',
  `is_del` int(11) DEFAULT '0' COMMENT '删除标志',
  `create_by` varchar(64) DEFAULT NULL COMMENT '创建者',
  `create_time` datetime DEFAULT NULL COMMENT '创建时间',
  `update_by` varchar(64) DEFAULT NULL COMMENT '更新者',
  `update_time` datetime DEFAULT NULL COMMENT '更新时间',
  `material_code` varchar(50) DEFAULT NULL COMMENT '物料编码',
  `material_name` varchar(100) DEFAULT NULL COMMENT '物料名称',
  `from_zone_code` varchar(50) DEFAULT NULL COMMENT '源库区编码',
  `from_location_code` varchar(50) DEFAULT NULL COMMENT '源库位编码',
  `to_zone_code` varchar(50) DEFAULT NULL COMMENT '目标库区编码',
  `to_location_code` varchar(50) DEFAULT NULL COMMENT '目标库位编码',
  PRIMARY KEY (`detail_id`),
  KEY `idx_move_id` (`move_id`),
  KEY `idx_material_id` (`material_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='移库明细表';

-- ---------------------------- wms_pack_spec ----------------------------
DROP TABLE IF EXISTS `wms_pack_spec`;
CREATE TABLE `wms_pack_spec` (
  `spec_id` bigint(20) NOT NULL COMMENT '规格id',
  `level_name` varchar(50) NOT NULL COMMENT '层级名称',
  `uom` varchar(20) DEFAULT NULL COMMENT '计量单位',
  `qty_per_parent` decimal(18,4) DEFAULT NULL COMMENT '上级层级包含的本层级数量',
  `is_continuous` int(11) DEFAULT NULL COMMENT '是否连续量',
  `is_base` int(11) DEFAULT NULL COMMENT '是否最小基本单位',
  `sort_order` int(11) DEFAULT '0' COMMENT '层级排序',
  `effective_date` datetime DEFAULT NULL COMMENT '生效日期',
  `is_del` int(11) DEFAULT '0' COMMENT '删除标志',
  `create_by` varchar(64) DEFAULT NULL COMMENT '创建者',
  `create_time` datetime DEFAULT NULL COMMENT '创建时间',
  `update_by` varchar(64) DEFAULT NULL COMMENT '更新者',
  `update_time` datetime DEFAULT NULL COMMENT '更新时间',
  `parent_id` bigint(20) DEFAULT NULL COMMENT '父级ID',
  `root_id` bigint(20) DEFAULT NULL COMMENT '根节点ID',
  PRIMARY KEY (`spec_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='包装规格表';

-- ---------------------------- wms_partner ----------------------------
DROP TABLE IF EXISTS `wms_partner`;
CREATE TABLE `wms_partner` (
  `partner_id` bigint(20) NOT NULL COMMENT '往来单位主键ID',
  `partner_code` varchar(50) NOT NULL COMMENT '往来单位编码',
  `partner_name` varchar(100) NOT NULL COMMENT '往来单位名称',
  `partner_type` varchar(20) NOT NULL COMMENT '往来单位类型',
  `province` varchar(50) DEFAULT NULL COMMENT '省份',
  `city` varchar(50) DEFAULT NULL COMMENT '城市',
  `district` varchar(50) DEFAULT NULL COMMENT '区县',
  `address` varchar(255) DEFAULT NULL COMMENT '详细地址',
  `bank_name` varchar(100) DEFAULT NULL COMMENT '开户银行名称',
  `bank_account` varchar(50) DEFAULT NULL COMMENT '银行账号',
  `tax_no` varchar(50) DEFAULT NULL COMMENT '税号',
  `partner_level` varchar(20) DEFAULT NULL COMMENT '合作等级',
  `settle_type` varchar(20) DEFAULT NULL COMMENT '结算方式',
  `credit_limit` decimal(15,2) DEFAULT NULL COMMENT '信用额度',
  `cooperate_start_date` datetime DEFAULT NULL COMMENT '合作开始日期',
  `status` varchar(1) DEFAULT NULL COMMENT '状态',
  `is_del` int(11) DEFAULT '0' COMMENT '逻辑删除标记',
  `create_by` varchar(64) DEFAULT NULL COMMENT '创建人',
  `create_time` datetime DEFAULT NULL COMMENT '创建时间',
  `update_by` varchar(64) DEFAULT NULL COMMENT '更新人',
  `update_time` datetime DEFAULT NULL COMMENT '更新时间',
  `remark` varchar(500) DEFAULT NULL COMMENT '备注',
  PRIMARY KEY (`partner_id`),
  UNIQUE KEY `uk_partner_code` (`partner_code`) USING BTREE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='往来单位表';

-- ---------------------------- wms_partner_address ----------------------------
DROP TABLE IF EXISTS `wms_partner_address`;
CREATE TABLE `wms_partner_address` (
  `address_id` bigint(20) NOT NULL COMMENT '地址主键ID',
  `partner_id` bigint(20) NOT NULL COMMENT '关联往来单位ID',
  `address_type` varchar(20) DEFAULT NULL COMMENT '地址类型',
  `province` varchar(50) DEFAULT NULL COMMENT '省份',
  `city` varchar(50) DEFAULT NULL COMMENT '城市',
  `district` varchar(50) DEFAULT NULL COMMENT '区县',
  `address` varchar(255) NOT NULL COMMENT '详细地址',
  `contact_name` varchar(50) DEFAULT NULL COMMENT '联系人姓名',
  `contact_phone` varchar(20) DEFAULT NULL COMMENT '联系电话',
  `is_default` varchar(1) DEFAULT NULL COMMENT '是否默认',
  `is_del` int(11) DEFAULT '0' COMMENT '逻辑删除标记',
  `create_by` varchar(64) DEFAULT NULL COMMENT '创建人',
  `create_time` datetime DEFAULT NULL COMMENT '创建时间',
  `update_by` varchar(64) DEFAULT NULL COMMENT '更新人',
  `update_time` datetime DEFAULT NULL COMMENT '更新时间',
  PRIMARY KEY (`address_id`),
  KEY `idx_partner_id` (`partner_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='往来单位地址表';

-- ---------------------------- wms_partner_contact ----------------------------
DROP TABLE IF EXISTS `wms_partner_contact`;
CREATE TABLE `wms_partner_contact` (
  `contact_id` bigint(20) NOT NULL AUTO_INCREMENT COMMENT '联系人主键ID',
  `partner_id` bigint(20) NOT NULL COMMENT '关联往来单位ID',
  `contact_name` varchar(50) DEFAULT NULL COMMENT '联系人姓名',
  `title` varchar(50) DEFAULT NULL COMMENT '职位',
  `phone` varchar(20) DEFAULT NULL COMMENT '联系电话',
  `email` varchar(100) DEFAULT NULL COMMENT '邮箱',
  `is_default` varchar(1) DEFAULT NULL COMMENT '是否默认',
  `remark` varchar(500) DEFAULT NULL COMMENT '备注',
  `is_del` int(11) DEFAULT '0' COMMENT '逻辑删除标记',
  `create_by` varchar(64) DEFAULT NULL COMMENT '创建人',
  `create_time` datetime DEFAULT NULL COMMENT '创建时间',
  `update_by` varchar(64) DEFAULT NULL COMMENT '更新人',
  `update_time` datetime DEFAULT NULL COMMENT '更新时间',
  PRIMARY KEY (`contact_id`),
  KEY `idx_partner` (`partner_id`)
) ENGINE=InnoDB AUTO_INCREMENT=3000000000007 DEFAULT CHARSET=utf8 COMMENT='往来单位联系人表';

-- ---------------------------- wms_receipt ----------------------------
DROP TABLE IF EXISTS `wms_receipt`;
CREATE TABLE `wms_receipt` (
  `receipt_id` bigint(20) NOT NULL COMMENT '入库id',
  `receipt_no` varchar(50) NOT NULL COMMENT '入库单号',
  `receipt_type` varchar(20) NOT NULL COMMENT '入库类型 purchase-采购入库 produce-生产入库 pending_in-待入库 return_in-退货入库 transfer_in-调拨入库 other-其他入库',
  `warehouse_id` bigint(20) NOT NULL COMMENT '仓库id',
  `zone_id` bigint(20) DEFAULT NULL COMMENT '默认入库库区id',
  `supplier_id` bigint(20) DEFAULT NULL COMMENT '供应商id',
  `customer_id` bigint(20) DEFAULT NULL COMMENT '客户id',
  `source_type` varchar(20) DEFAULT NULL COMMENT '来源单据类型',
  `source_id` bigint(20) DEFAULT NULL COMMENT '来源单据id',
  `receipt_date` datetime NOT NULL COMMENT '入库日期',
  `total_quantity` int(11) DEFAULT NULL COMMENT '总数量',
  `operator_id` bigint(20) DEFAULT NULL COMMENT '作业人员id',
  `receipt_status` varchar(20) DEFAULT NULL COMMENT '入库状态 pending_in-待入库 in_stock-已入库 completed-已完成',
  `approve_status` varchar(20) DEFAULT NULL COMMENT '审核状态 0未审核 1已审核',
  `approve_id` bigint(20) DEFAULT NULL COMMENT '审核人id',
  `approve_time` datetime DEFAULT NULL COMMENT '审核时间',
  `is_del` int(11) DEFAULT '0' COMMENT '删除标志',
  `create_by` varchar(64) DEFAULT NULL COMMENT '创建者',
  `create_time` datetime DEFAULT NULL COMMENT '创建时间',
  `update_by` varchar(64) DEFAULT NULL COMMENT '更新者',
  `update_time` datetime DEFAULT NULL COMMENT '更新时间',
  `remark` varchar(500) DEFAULT NULL COMMENT '备注',
  PRIMARY KEY (`receipt_id`),
  UNIQUE KEY `uk_receipt_no` (`receipt_no`) USING BTREE,
  KEY `idx_warehouse_id` (`warehouse_id`),
  KEY `idx_zone_id` (`zone_id`),
  KEY `idx_receipt_date` (`receipt_date`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='入库单主表';

-- ---------------------------- wms_receipt_detail ----------------------------
DROP TABLE IF EXISTS `wms_receipt_detail`;
CREATE TABLE `wms_receipt_detail` (
  `detail_id` bigint(20) NOT NULL COMMENT '明细id',
  `receipt_id` bigint(20) NOT NULL COMMENT '入库id',
  `material_id` bigint(20) NOT NULL COMMENT '物料id',
  `spec` varchar(100) DEFAULT NULL COMMENT '规格',
  `unit` varchar(20) DEFAULT NULL COMMENT '单位',
  `batch_no` varchar(50) DEFAULT NULL COMMENT '批次号',
  `expire_date` datetime DEFAULT NULL COMMENT '有效期至',
  `receipt_quantity` int(11) NOT NULL COMMENT '入库数量',
  `shelf_quantity` int(11) DEFAULT '0' COMMENT '已上架数量',
  `barcode` varchar(50) DEFAULT NULL COMMENT '库存条码',
  `detail_status` varchar(20) DEFAULT NULL COMMENT '上架状态',
  `location_id` bigint(20) DEFAULT NULL COMMENT '库位id',
  `is_del` int(11) DEFAULT '0' COMMENT '删除标志',
  `create_by` varchar(64) DEFAULT NULL COMMENT '创建者',
  `create_time` datetime DEFAULT NULL COMMENT '创建时间',
  `update_by` varchar(64) DEFAULT NULL COMMENT '更新者',
  `update_time` datetime DEFAULT NULL COMMENT '更新时间',
  `materialion_date` datetime DEFAULT NULL COMMENT '生产日期',
  `supplier_id` bigint(20) DEFAULT NULL COMMENT '供应商id',
  PRIMARY KEY (`detail_id`),
  UNIQUE KEY `uk_barcode` (`barcode`),
  KEY `idx_receipt_id` (`receipt_id`),
  KEY `idx_material_id` (`material_id`),
  KEY `idx_location_id` (`location_id`),
  KEY `idx_batch_no` (`batch_no`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='入库单明细表';

-- ---------------------------- wms_receipt_detail_barcode ----------------------------
DROP TABLE IF EXISTS `wms_receipt_detail_barcode`;
CREATE TABLE `wms_receipt_detail_barcode` (
  `barcode_id` bigint(20) NOT NULL COMMENT '条码id',
  `receipt_id` bigint(20) NOT NULL COMMENT '入库单id',
  `detail_id` bigint(20) NOT NULL COMMENT '入库明细id',
  `barcode` varchar(50) DEFAULT NULL COMMENT '库存条码',
  `seq_no` int(11) DEFAULT NULL COMMENT '箱序号',
  `quantity` decimal(18,4) DEFAULT NULL COMMENT '本码承载数量',
  `base_quantity` decimal(18,4) DEFAULT NULL COMMENT '折算基本单位数量',
  `unit` varchar(20) DEFAULT NULL COMMENT '单位',
  `pack_spec_id` bigint(20) DEFAULT NULL COMMENT '包装规格层级id',
  `batch_no` varchar(50) DEFAULT NULL COMMENT '批次号',
  `location_id` bigint(20) DEFAULT NULL COMMENT '上架库位id',
  `status` varchar(20) DEFAULT NULL COMMENT '状态',
  `shelve_time` datetime DEFAULT NULL COMMENT '上架时间',
  `is_del` int(11) DEFAULT '0' COMMENT '删除标志',
  `create_by` varchar(64) DEFAULT '' COMMENT '创建者',
  `create_time` datetime DEFAULT NULL COMMENT '创建时间',
  `update_by` varchar(64) DEFAULT '' COMMENT '更新者',
  `update_time` datetime DEFAULT NULL COMMENT '更新时间',
  PRIMARY KEY (`barcode_id`),
  UNIQUE KEY `uk_barcode` (`barcode`) USING BTREE,
  KEY `idx_detail_id` (`detail_id`),
  KEY `idx_receipt_id` (`receipt_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='入库单明细条码表';

-- ---------------------------- wms_receiving ----------------------------
DROP TABLE IF EXISTS `wms_receiving`;
CREATE TABLE `wms_receiving` (
  `receiving_id` bigint(20) NOT NULL COMMENT '收货单id',
  `receiving_no` varchar(50) NOT NULL COMMENT '收货单号',
  `supplier_id` bigint(20) DEFAULT NULL COMMENT '供应商id',
  `expect_date` datetime NOT NULL COMMENT '期望到货日期',
  `carrier_name` varchar(100) DEFAULT NULL COMMENT '承运人',
  `carrier_phone` varchar(20) DEFAULT NULL COMMENT '承运人电话',
  `total_quantity` int(11) DEFAULT NULL COMMENT '收货总数量',
  `receiving_status` varchar(20) DEFAULT NULL COMMENT '收货状态 pending-待收货 arriving-收货中 completed-已完成 cancelled-已取消',
  `is_del` int(11) DEFAULT '0' COMMENT '删除标志',
  `create_by` varchar(64) DEFAULT NULL COMMENT '创建者',
  `create_time` datetime DEFAULT NULL COMMENT '创建时间',
  `update_by` varchar(64) DEFAULT NULL COMMENT '更新者',
  `update_time` datetime DEFAULT NULL COMMENT '更新时间',
  `remark` varchar(500) DEFAULT NULL COMMENT '备注',
  PRIMARY KEY (`receiving_id`),
  UNIQUE KEY `uk_receiving_no` (`receiving_no`) USING BTREE,
  KEY `idx_supplier_id` (`supplier_id`),
  KEY `idx_expect_date` (`expect_date`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='收货单表';

-- ---------------------------- wms_receiving_detail ----------------------------
DROP TABLE IF EXISTS `wms_receiving_detail`;
CREATE TABLE `wms_receiving_detail` (
  `detail_id` bigint(20) NOT NULL COMMENT '明细id',
  `receiving_id` bigint(20) NOT NULL COMMENT '收货单id',
  `material_id` bigint(20) NOT NULL COMMENT '物料id',
  `spec` varchar(100) DEFAULT NULL COMMENT '规格',
  `unit` varchar(20) DEFAULT NULL COMMENT '单位',
  `batch_no` varchar(50) DEFAULT NULL COMMENT '批次号',
  `plan_quantity` int(11) NOT NULL COMMENT '收货数量',
  `stock_in_quantity` int(11) DEFAULT NULL COMMENT '入库数',
  `pending_inbound_qty` int(11) DEFAULT NULL COMMENT '待入库数量',
  `return_qty` int(11) DEFAULT '0' COMMENT '退货数量',
  `is_del` int(11) DEFAULT '0' COMMENT '删除标志',
  `create_by` varchar(64) DEFAULT NULL COMMENT '创建者',
  `create_time` datetime DEFAULT NULL COMMENT '创建时间',
  `update_by` varchar(64) DEFAULT NULL COMMENT '更新者',
  `update_time` datetime DEFAULT NULL COMMENT '更新时间',
  PRIMARY KEY (`detail_id`),
  KEY `idx_receiving_id` (`receiving_id`),
  KEY `idx_material_id` (`material_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='收货单明细表';

-- ---------------------------- wms_region ----------------------------
DROP TABLE IF EXISTS `wms_region`;
CREATE TABLE `wms_region` (
  `code` varchar(20) NOT NULL COMMENT '行政区划编码',
  `name` varchar(100) NOT NULL COMMENT '名称',
  `parent_code` varchar(20) DEFAULT NULL COMMENT '父级行政区划编码',
  `update_date` datetime DEFAULT NULL COMMENT '更新日期',
  PRIMARY KEY (`code`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='省市区';

-- ---------------------------- wms_return_order ----------------------------
DROP TABLE IF EXISTS `wms_return_order`;
CREATE TABLE `wms_return_order` (
  `return_order_id` bigint(20) NOT NULL COMMENT '退货单id',
  `return_order_no` varchar(50) NOT NULL COMMENT '退货单号',
  `return_type` varchar(20) DEFAULT NULL COMMENT '退货类型 iqc_unqualified-质检不合格 other-其他',
  `supplier_id` bigint(20) DEFAULT NULL COMMENT '供应商id',
  `total_quantity` int(11) DEFAULT NULL COMMENT '退货总数量',
  `is_del` int(11) DEFAULT '0' COMMENT '删除标志',
  `create_by` varchar(64) DEFAULT NULL COMMENT '创建者',
  `create_time` datetime DEFAULT NULL COMMENT '创建时间',
  `update_by` varchar(64) DEFAULT NULL COMMENT '更新者',
  `update_time` datetime DEFAULT NULL COMMENT '更新时间',
  `remark` varchar(500) DEFAULT NULL COMMENT '备注',
  `return_status` varchar(20) DEFAULT NULL COMMENT '退货状态 pending-待处理 completed-已完成 cancelled-已取消',
  PRIMARY KEY (`return_order_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='退货单表';

-- ---------------------------- wms_return_order_detail ----------------------------
DROP TABLE IF EXISTS `wms_return_order_detail`;
CREATE TABLE `wms_return_order_detail` (
  `detail_id` bigint(20) NOT NULL COMMENT '明细id',
  `return_order_id` bigint(20) NOT NULL COMMENT '退货单id',
  `material_id` bigint(20) NOT NULL COMMENT '物料id',
  `spec` varchar(100) DEFAULT NULL COMMENT '规格',
  `unit` varchar(20) DEFAULT NULL COMMENT '单位',
  `batch_no` varchar(50) DEFAULT NULL COMMENT '批次号',
  `plan_quantity` int(11) NOT NULL COMMENT '退货数量',
  `return_reason` varchar(255) DEFAULT NULL COMMENT '退货原因',
  `location_id` bigint(20) DEFAULT NULL COMMENT '默认库位id',
  `is_del` int(11) DEFAULT '0' COMMENT '删除标志',
  `create_by` varchar(64) DEFAULT NULL COMMENT '创建者',
  `create_time` datetime DEFAULT NULL COMMENT '创建时间',
  `update_by` varchar(64) DEFAULT NULL COMMENT '更新者',
  `update_time` datetime DEFAULT NULL COMMENT '更新时间',
  PRIMARY KEY (`detail_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='退货单明细表';

-- ---------------------------- wms_return_out ----------------------------
DROP TABLE IF EXISTS `wms_return_out`;
CREATE TABLE `wms_return_out` (
  `return_id` bigint(20) NOT NULL COMMENT '退货id',
  `return_no` varchar(50) NOT NULL COMMENT '退货单号',
  `delivery_id` bigint(20) DEFAULT NULL COMMENT '原出库id',
  `customer_id` bigint(20) DEFAULT NULL COMMENT '客户id',
  `warehouse_id` bigint(20) DEFAULT NULL COMMENT '仓库id',
  `zone_id` bigint(20) DEFAULT NULL COMMENT '库区id',
  `return_date` datetime NOT NULL COMMENT '退货日期',
  `total_quantity` int(11) DEFAULT '0' COMMENT '退货总数量',
  `return_reason` varchar(255) DEFAULT NULL COMMENT '退货原因',
  `return_status` varchar(20) DEFAULT NULL COMMENT '退货状态 pending-待审核 approved-已审核 received-已收货 completed-已完成',
  `is_del` int(11) DEFAULT '0' COMMENT '删除标志',
  `create_by` varchar(64) DEFAULT NULL COMMENT '创建者',
  `create_time` datetime DEFAULT NULL COMMENT '创建时间',
  `update_by` varchar(64) DEFAULT NULL COMMENT '更新者',
  `update_time` datetime DEFAULT NULL COMMENT '更新时间',
  `return_type` varchar(20) DEFAULT NULL COMMENT '退货类型 return_out-退货出库',
  PRIMARY KEY (`return_id`),
  UNIQUE KEY `uk_return_no` (`return_no`) USING BTREE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='出库退货表';

-- ---------------------------- wms_stock ----------------------------
DROP TABLE IF EXISTS `wms_stock`;
CREATE TABLE `wms_stock` (
  `stock_id` bigint(20) NOT NULL COMMENT '库存id',
  `barcode` varchar(50) DEFAULT NULL COMMENT '库存条码',
  `material_id` bigint(20) NOT NULL COMMENT '物料id',
  `material_code` varchar(50) DEFAULT NULL COMMENT '物料编码',
  `material_name` varchar(100) DEFAULT NULL COMMENT '物料名称',
  `spec` varchar(100) DEFAULT NULL COMMENT '规格',
  `unit` varchar(20) DEFAULT NULL COMMENT '单位',
  `batch_no` varchar(50) DEFAULT NULL COMMENT '批次号',
  `warehouse_id` bigint(20) NOT NULL DEFAULT '0' COMMENT '仓库id',
  `warehouse_name` varchar(100) DEFAULT NULL COMMENT '仓库名称',
  `zone_id` bigint(20) DEFAULT NULL COMMENT '库区id',
  `zone_code` varchar(50) DEFAULT NULL COMMENT '库区编码',
  `zone_name` varchar(100) DEFAULT NULL COMMENT '库区名称',
  `location_id` bigint(20) NOT NULL DEFAULT '0' COMMENT '库位id',
  `location_code` varchar(50) DEFAULT NULL COMMENT '库位编码',
  `location_name` varchar(100) DEFAULT NULL COMMENT '库位名称',
  `hu_id` bigint(20) DEFAULT NULL COMMENT '处理单元id',
  `pack_spec_id` bigint(20) DEFAULT NULL COMMENT '包装规格层级id',
  `supplier_id` bigint(20) DEFAULT NULL COMMENT '供应商id',
  `receipt_id` bigint(20) DEFAULT NULL COMMENT '来源入库单id',
  `receipt_detail_id` bigint(20) DEFAULT NULL COMMENT '来源入库明细id',
  `production_date` datetime DEFAULT NULL COMMENT '生产日期',
  `expire_date` datetime DEFAULT NULL COMMENT '有效期至',
  `stock_quantity` decimal(18,3) DEFAULT '0.000' COMMENT '库存数量',
  `base_quantity` decimal(18,4) DEFAULT NULL COMMENT '基本单位数量',
  `available_quantity` decimal(18,3) DEFAULT '0.000' COMMENT '可用数量',
  `freeze_quantity` decimal(18,3) DEFAULT '0.000' COMMENT '冻结数量',
  `lock_quantity` decimal(18,3) DEFAULT '0.000' COMMENT '锁定数量',
  `cost_price` decimal(15,4) DEFAULT NULL COMMENT '成本单价',
  `cost_amount` decimal(15,2) DEFAULT NULL COMMENT '成本金额',
  `is_del` int(11) NOT NULL DEFAULT '0' COMMENT '删除标志',
  `create_by` varchar(64) DEFAULT '' COMMENT '创建者',
  `create_time` datetime DEFAULT NULL COMMENT '创建时间',
  `update_by` varchar(64) DEFAULT '' COMMENT '更新者',
  `update_time` datetime DEFAULT NULL COMMENT '更新时间',
  PRIMARY KEY (`stock_id`),
  UNIQUE KEY `uk_barcode` (`barcode`) USING BTREE,
  UNIQUE KEY `uk_material_batch_location_hu` (`material_id`,`batch_no`,`location_id`,`hu_id`) USING BTREE,
  KEY `idx_warehouse_id` (`warehouse_id`),
  KEY `idx_zone_id` (`zone_id`),
  KEY `idx_location_id` (`location_id`),
  KEY `idx_batch_no` (`batch_no`),
  KEY `idx_hu_id` (`hu_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='实时库存表';

-- ---------------------------- wms_stock_alert_config ----------------------------
DROP TABLE IF EXISTS `wms_stock_alert_config`;
CREATE TABLE `wms_stock_alert_config` (
  `config_id` bigint(20) NOT NULL COMMENT '配置id',
  `warehouse_id` bigint(20) NOT NULL COMMENT '仓库id',
  `zone_id` bigint(20) DEFAULT NULL COMMENT '库区id',
  `location_id` bigint(20) DEFAULT NULL COMMENT '库位id',
  `material_id` bigint(20) DEFAULT NULL COMMENT '物料id',
  `warning_type` varchar(30) NOT NULL COMMENT '预警类型 min_stock-低于下限 max_stock-高于上限',
  `threshold_value` decimal(18,3) NOT NULL COMMENT '阈值',
  `warning_level` varchar(10) DEFAULT 'normal' COMMENT '预警级别 normal-一般 urgent-紧急 critical-危机',
  `notify_user_ids` varchar(500) DEFAULT NULL COMMENT '处理人（逗号分隔用户id）',
  `enabled` int(11) DEFAULT '1' COMMENT '是否启用 1-启用 0-停用',
  `remark` varchar(255) DEFAULT NULL COMMENT '备注',
  `is_del` int(11) DEFAULT '0' COMMENT '删除标志',
  `create_by` varchar(64) DEFAULT NULL COMMENT '创建者',
  `create_time` datetime DEFAULT NULL COMMENT '创建时间',
  `update_by` varchar(64) DEFAULT NULL COMMENT '更新者',
  `update_time` datetime DEFAULT NULL COMMENT '更新时间',
  PRIMARY KEY (`config_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='库存预警配置表';

-- ---------------------------- wms_stock_record ----------------------------
DROP TABLE IF EXISTS `wms_stock_record`;
CREATE TABLE `wms_stock_record` (
  `record_id` bigint(20) NOT NULL COMMENT '记录id',
  `record_no` varchar(50) DEFAULT NULL COMMENT '记录单号',
  `material_id` bigint(20) NOT NULL COMMENT '物料id',
  `batch_no` varchar(50) DEFAULT NULL COMMENT '批次号',
  `warehouse_id` bigint(20) NOT NULL COMMENT '仓库id',
  `zone_id` bigint(20) DEFAULT NULL COMMENT '库区id',
  `location_id` bigint(20) DEFAULT NULL COMMENT '库位id',
  `biz_type` varchar(30) NOT NULL COMMENT '业务类型 receipt-入库 delivery-出库 move-移库 check-盘点 adjust-调整 freeze-冻结 thaw-解冻 lock-锁定 unlock-解锁',
  `source_type` varchar(30) DEFAULT NULL COMMENT '来源单据类型',
  `source_id` bigint(20) DEFAULT NULL COMMENT '来源单据id',
  `source_no` varchar(50) DEFAULT NULL COMMENT '来源单据号',
  `direction` varchar(10) NOT NULL COMMENT '方向 in-入库 out-出库',
  `before_quantity` decimal(18,3) DEFAULT '0.000' COMMENT '变动前数量',
  `change_quantity` decimal(18,3) NOT NULL COMMENT '变动数量',
  `after_quantity` decimal(18,3) DEFAULT '0.000' COMMENT '变动后数量',
  `unit_price` decimal(15,4) DEFAULT NULL COMMENT '单价',
  `amount` decimal(15,2) DEFAULT NULL COMMENT '金额',
  `operator_id` bigint(20) DEFAULT NULL COMMENT '操作人id',
  `operate_time` datetime DEFAULT NULL COMMENT '操作时间',
  `remark` varchar(500) DEFAULT NULL COMMENT '备注',
  `is_del` int(11) DEFAULT '0' COMMENT '删除标志',
  `create_by` varchar(64) DEFAULT NULL COMMENT '创建者',
  `create_time` datetime DEFAULT NULL COMMENT '创建时间',
  `update_by` varchar(64) DEFAULT NULL COMMENT '更新者',
  `update_time` datetime DEFAULT NULL COMMENT '更新时间',
  PRIMARY KEY (`record_id`),
  KEY `idx_material_id` (`material_id`),
  KEY `idx_warehouse_id` (`warehouse_id`),
  KEY `idx_zone_id` (`zone_id`),
  KEY `idx_location_id` (`location_id`),
  KEY `idx_source_no` (`source_no`),
  KEY `idx_biz_type` (`biz_type`),
  KEY `idx_operate_time` (`operate_time`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='库存流水表';

-- ---------------------------- wms_stock_warning ----------------------------
DROP TABLE IF EXISTS `wms_stock_warning`;
CREATE TABLE `wms_stock_warning` (
  `warning_id` bigint(20) NOT NULL COMMENT '预警id',
  `warning_type` varchar(30) NOT NULL COMMENT '预警类型 min_stock-安全库存 max_stock-最大库存 shelf_life-保质期呆滞-stagnation',
  `material_id` bigint(20) NOT NULL COMMENT '物料id',
  `warehouse_id` bigint(20) DEFAULT NULL COMMENT '仓库id',
  `zone_id` bigint(20) DEFAULT NULL COMMENT '库区id',
  `current_quantity` int(11) DEFAULT NULL COMMENT '当前库存',
  `threshold_value` decimal(15,2) DEFAULT NULL COMMENT '阈值',
  `diff_value` decimal(18,4) DEFAULT NULL COMMENT '差值',
  `warning_level` varchar(10) DEFAULT NULL COMMENT '预警级别 normal-一般 urgent-紧急 critical-危机',
  `warning_status` varchar(20) DEFAULT NULL COMMENT '处理状态 pending-待处理 handled-已处理 ignored-已忽略 resolved-已自动恢复',
  `handle_user_id` bigint(20) DEFAULT NULL COMMENT '处理人id',
  `handle_time` datetime DEFAULT NULL COMMENT '处理时间',
  `handle_remark` varchar(500) DEFAULT NULL COMMENT '处理说明',
  `is_del` int(11) DEFAULT '0' COMMENT '删除标志',
  `create_by` varchar(64) DEFAULT NULL COMMENT '创建者',
  `create_time` datetime DEFAULT NULL COMMENT '创建时间',
  `update_by` varchar(64) DEFAULT NULL COMMENT '更新者',
  `update_time` datetime DEFAULT NULL COMMENT '更新时间',
  `config_id` bigint(20) DEFAULT NULL COMMENT '来源配置id',
  `location_id` bigint(20) DEFAULT NULL COMMENT '库位id',
  `notify_user_ids` varchar(500) DEFAULT NULL COMMENT '通知人（逗号分隔用户id）',
  `trigger_count` int(11) DEFAULT '1' COMMENT '本周期累计触发次数',
  `last_trigger_time` datetime DEFAULT NULL COMMENT '最近触发时间',
  `resolved_time` datetime DEFAULT NULL COMMENT '自动恢复时间',
  `resolved_quantity` decimal(18,3) DEFAULT NULL COMMENT '恢复时的库存量',
  PRIMARY KEY (`warning_id`),
  KEY `idx_material_id` (`material_id`),
  KEY `idx_warehouse_id` (`warehouse_id`),
  KEY `idx_zone_id` (`zone_id`),
  KEY `idx_warning_type` (`warning_type`),
  KEY `idx_warning_status` (`warning_status`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='库存预警表';

-- ---------------------------- wms_stock_warning_log ----------------------------
DROP TABLE IF EXISTS `wms_stock_warning_log`;
CREATE TABLE `wms_stock_warning_log` (
  `log_id` bigint(20) NOT NULL COMMENT '明细id',
  `warning_id` bigint(20) NOT NULL COMMENT '预警记录id',
  `config_id` bigint(20) DEFAULT NULL COMMENT '来源配置id',
  `material_id` bigint(20) NOT NULL COMMENT '物料id',
  `current_quantity` decimal(18,3) NOT NULL COMMENT '触发时库存量',
  `threshold_value` decimal(18,3) NOT NULL COMMENT '阈值',
  `diff_value` decimal(18,3) DEFAULT NULL COMMENT '差值',
  `message_sent` int(11) DEFAULT '0' COMMENT '是否发送站内信 1-是 0-否',
  `create_time` datetime DEFAULT NULL COMMENT '触发时间',
  PRIMARY KEY (`log_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='库存预警触发明细表';

-- ---------------------------- wms_transfer ----------------------------
DROP TABLE IF EXISTS `wms_transfer`;
CREATE TABLE `wms_transfer` (
  `transfer_id` bigint(20) NOT NULL COMMENT '调拨id',
  `transfer_no` varchar(50) NOT NULL COMMENT '调拨单号',
  `from_warehouse_id` bigint(20) NOT NULL COMMENT '调出仓库id',
  `to_warehouse_id` bigint(20) NOT NULL COMMENT '调入仓库id',
  `transfer_type` varchar(20) DEFAULT 'normal' COMMENT '调拨类型 normal-普通调拨 urgent-紧急调拨',
  `transfer_reason` varchar(255) DEFAULT NULL COMMENT '调拨原因',
  `total_quantity` decimal(18,3) DEFAULT '0.000' COMMENT '调拨总数量',
  `out_quantity` decimal(18,3) DEFAULT '0.000' COMMENT '已出库数量',
  `in_quantity` decimal(18,3) DEFAULT '0.000' COMMENT '已入库数量',
  `in_transit_quantity` decimal(18,3) DEFAULT '0.000' COMMENT '在途数量',
  `transfer_status` varchar(20) DEFAULT 'pending' COMMENT '调拨状态 pending-待调拨 in_transit-在途 completed-已完成 cancelled-已取消',
  `out_by` varchar(64) DEFAULT NULL COMMENT '移出人',
  `out_time` datetime DEFAULT NULL COMMENT '移出时间',
  `in_by` varchar(64) DEFAULT NULL COMMENT '移入人',
  `in_time` datetime DEFAULT NULL COMMENT '移入时间',
  `operator_id` bigint(20) DEFAULT NULL COMMENT '操作人id',
  `is_del` int(11) DEFAULT '0' COMMENT '删除标志',
  `create_by` varchar(64) DEFAULT '' COMMENT '创建者',
  `create_time` datetime DEFAULT NULL COMMENT '创建时间',
  `update_by` varchar(64) DEFAULT '' COMMENT '更新者',
  `update_time` datetime DEFAULT NULL COMMENT '更新时间',
  `remark` varchar(500) DEFAULT NULL COMMENT '备注',
  PRIMARY KEY (`transfer_id`),
  UNIQUE KEY `uk_transfer_no` (`transfer_no`) USING BTREE,
  KEY `idx_from_warehouse_id` (`from_warehouse_id`),
  KEY `idx_to_warehouse_id` (`to_warehouse_id`),
  KEY `idx_transfer_status` (`transfer_status`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='调拨单表';

-- ---------------------------- wms_transfer_detail ----------------------------
DROP TABLE IF EXISTS `wms_transfer_detail`;
CREATE TABLE `wms_transfer_detail` (
  `detail_id` bigint(20) NOT NULL COMMENT '明细id',
  `transfer_id` bigint(20) NOT NULL COMMENT '调拨id',
  `material_id` bigint(20) NOT NULL COMMENT '物料id',
  `material_code` varchar(50) DEFAULT NULL COMMENT '物料编码',
  `material_name` varchar(100) DEFAULT NULL COMMENT '物料名称',
  `spec` varchar(100) DEFAULT NULL COMMENT '规格型号',
  `unit` varchar(20) DEFAULT NULL COMMENT '单位',
  `batch_no` varchar(50) DEFAULT NULL COMMENT '批次号',
  `barcode` varchar(50) DEFAULT NULL COMMENT '条码',
  `from_zone_id` bigint(20) DEFAULT NULL COMMENT '调出库区id',
  `from_location_id` bigint(20) DEFAULT NULL COMMENT '调出库位id',
  `from_location_code` varchar(50) DEFAULT NULL COMMENT '调出库位编码',
  `to_zone_id` bigint(20) DEFAULT NULL COMMENT '调入库区id',
  `to_location_id` bigint(20) DEFAULT NULL COMMENT '调入库位id',
  `to_location_code` varchar(50) DEFAULT NULL COMMENT '调入库位编码',
  `transfer_quantity` decimal(18,3) NOT NULL COMMENT '调拨数量',
  `out_quantity` decimal(18,3) DEFAULT '0.000' COMMENT '已出库数量',
  `in_quantity` decimal(18,3) DEFAULT '0.000' COMMENT '已入库数量',
  `in_transit_quantity` decimal(18,3) DEFAULT '0.000' COMMENT '在途数量',
  `transfer_status` varchar(20) DEFAULT 'pending' COMMENT '状态 pending-待调拨 in_transit-在途 completed-已完成',
  `out_time` datetime DEFAULT NULL COMMENT '移出时间',
  `in_time` datetime DEFAULT NULL COMMENT '移入时间',
  `is_del` int(11) DEFAULT '0' COMMENT '删除标志',
  `create_by` varchar(64) DEFAULT '' COMMENT '创建者',
  `create_time` datetime DEFAULT NULL COMMENT '创建时间',
  `update_by` varchar(64) DEFAULT '' COMMENT '更新者',
  `update_time` datetime DEFAULT NULL COMMENT '更新时间',
  `remark` varchar(500) DEFAULT NULL COMMENT '备注',
  PRIMARY KEY (`detail_id`),
  KEY `idx_transfer_id` (`transfer_id`),
  KEY `idx_material_id` (`material_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='调拨明细表';

-- ---------------------------- wms_warehouse ----------------------------
DROP TABLE IF EXISTS `wms_warehouse`;
CREATE TABLE `wms_warehouse` (
  `warehouse_id` bigint(20) NOT NULL COMMENT '仓库id',
  `warehouse_code` varchar(50) NOT NULL COMMENT '仓库编码',
  `warehouse_name` varchar(100) NOT NULL COMMENT '仓库名称',
  `warehouse_type` varchar(20) DEFAULT NULL COMMENT '仓库类型 warehouse-普通仓 cold-冷库 freezer-冷冻库',
  `province` varchar(50) DEFAULT NULL COMMENT '省份',
  `city` varchar(50) DEFAULT NULL COMMENT '城市',
  `district` varchar(50) DEFAULT NULL COMMENT '区县',
  `address` varchar(255) DEFAULT NULL COMMENT '详细地址',
  `manager_id` bigint(20) DEFAULT NULL COMMENT '仓库负责人id',
  `phone` varchar(20) DEFAULT NULL COMMENT '联系电话',
  `total_area` decimal(18,4) DEFAULT NULL COMMENT '总面积',
  `usable_area` decimal(10,2) DEFAULT NULL COMMENT '可用面积',
  `status` varchar(1) DEFAULT NULL COMMENT '状态',
  `create_by` varchar(64) DEFAULT NULL COMMENT '创建者',
  `create_time` datetime DEFAULT NULL COMMENT '创建时间',
  `update_by` varchar(64) DEFAULT NULL COMMENT '更新者',
  `update_time` datetime DEFAULT NULL COMMENT '更新时间',
  `remark` varchar(500) DEFAULT NULL COMMENT '备注',
  `is_del` int(11) DEFAULT '0' COMMENT '删除标志',
  PRIMARY KEY (`warehouse_id`),
  UNIQUE KEY `uk_warehouse_code` (`warehouse_code`) USING BTREE,
  KEY `idx_status` (`status`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='仓库表';

-- ---------------------------- wms_warehouse_manager ----------------------------
DROP TABLE IF EXISTS `wms_warehouse_manager`;
CREATE TABLE `wms_warehouse_manager` (
  `id` bigint(20) NOT NULL COMMENT '主键id',
  `warehouse_id` bigint(20) NOT NULL COMMENT '仓库id',
  `user_id` bigint(20) NOT NULL COMMENT '仓管员用户id',
  `role_type` varchar(20) DEFAULT NULL COMMENT '角色类型 manager-仓库管理员 operator-作业人员',
  `is_del` int(11) DEFAULT '0' COMMENT '删除标志',
  `create_by` varchar(64) DEFAULT NULL COMMENT '创建者',
  `create_time` datetime DEFAULT NULL COMMENT '创建时间',
  `remark` varchar(500) DEFAULT NULL COMMENT '备注',
  PRIMARY KEY (`id`),
  KEY `idx_warehouse_id` (`warehouse_id`),
  KEY `idx_user_id` (`user_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='仓库仓管员关联表';

-- ---------------------------- wms_zone ----------------------------
DROP TABLE IF EXISTS `wms_zone`;
CREATE TABLE `wms_zone` (
  `zone_id` bigint(20) NOT NULL COMMENT '库区id',
  `warehouse_id` bigint(20) NOT NULL COMMENT '仓库id',
  `zone_code` varchar(50) NOT NULL COMMENT '库区编码',
  `zone_name` varchar(100) NOT NULL COMMENT '库区名称',
  `zone_type` varchar(20) DEFAULT NULL COMMENT '库区类型 receive-收货区 storage-存储区 picking-拣货区 ship-发货区 return-退货区 temp-暂存区 reject-不合格品区',
  `zone_attr` varchar(50) DEFAULT NULL COMMENT '库区属性 normal-常温 cold-低温 freezer-冷冻 damp-防潮 dangerous-危险品',
  `floor` int(11) DEFAULT NULL COMMENT '所在楼层',
  `zone_area` decimal(18,4) DEFAULT NULL COMMENT '库区面积',
  `shelf_rows` int(11) DEFAULT NULL COMMENT '货架排数',
  `shelf_cols` int(11) DEFAULT NULL COMMENT '货架列数',
  `shelf_levels` int(11) DEFAULT NULL COMMENT '货架层数',
  `zone_status` varchar(1) DEFAULT NULL COMMENT '库区状态',
  `create_by` varchar(64) DEFAULT NULL COMMENT '创建者',
  `create_time` datetime DEFAULT NULL COMMENT '创建时间',
  `update_by` varchar(64) DEFAULT NULL COMMENT '更新者',
  `update_time` datetime DEFAULT NULL COMMENT '更新时间',
  `remark` varchar(500) DEFAULT NULL COMMENT '备注',
  `is_del` int(11) DEFAULT '0' COMMENT '删除标志',
  PRIMARY KEY (`zone_id`),
  UNIQUE KEY `uk_zone_code` (`zone_code`) USING BTREE,
  KEY `idx_warehouse_id` (`warehouse_id`),
  KEY `idx_zone_type` (`zone_type`),
  KEY `idx_zone_status` (`zone_status`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='库区表';

SET FOREIGN_KEY_CHECKS = 1;