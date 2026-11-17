-- =============================================
-- WMS 仓库管理系统 业务表结构 V3.0（PostgreSQL）
-- 由 MySQL 结构转换：2026-11-17，共 42 张 wms 业务表
-- 说明：AUTO_INCREMENT 由应用侧(雪花ID)生成，无需序列；
--       update_time 的 ON UPDATE 语义由应用层维护
-- =============================================

-- ---------------------------- wms_adjust ----------------------------
DROP TABLE IF EXISTS wms_adjust;
CREATE TABLE wms_adjust (
  "adjust_id" bigint NOT NULL,
  "adjust_no" varchar(50) NOT NULL,
  "adjust_type" varchar(20) NOT NULL,
  "warehouse_id" bigint NOT NULL,
  "zone_id" bigint DEFAULT NULL,
  "source_type" varchar(20) DEFAULT NULL,
  "source_id" bigint DEFAULT NULL,
  "total_quantity" integer DEFAULT NULL,
  "adjust_reason" varchar(255) DEFAULT NULL,
  "adjust_status" varchar(20) DEFAULT NULL,
  "approve_id" bigint DEFAULT NULL,
  "approve_time" timestamp DEFAULT NULL,
  "is_del" integer DEFAULT 0,
  "create_by" varchar(64) DEFAULT NULL,
  "create_time" timestamp DEFAULT NULL,
  "update_by" varchar(64) DEFAULT NULL,
  "update_time" timestamp DEFAULT NULL,
  "remark" varchar(500) DEFAULT NULL,
  PRIMARY KEY (adjust_id)
);
CREATE UNIQUE INDEX wms_adjust_uk_adjust_no ON wms_adjust (adjust_no);
CREATE INDEX wms_adjust_idx_warehouse_id ON wms_adjust (warehouse_id);
CREATE INDEX wms_adjust_idx_zone_id ON wms_adjust (zone_id);
CREATE INDEX wms_adjust_idx_source_id ON wms_adjust (source_id);
COMMENT ON COLUMN wms_adjust.adjust_id IS '调整id';
COMMENT ON COLUMN wms_adjust.adjust_no IS '调整单号';
COMMENT ON COLUMN wms_adjust.adjust_type IS '调整类型 check_profit-盘盈 check_loss-盘亏 damage-报损 overflow-报溢 validity-有效期调整 other-其他';
COMMENT ON COLUMN wms_adjust.warehouse_id IS '仓库id';
COMMENT ON COLUMN wms_adjust.zone_id IS '库区id';
COMMENT ON COLUMN wms_adjust.source_type IS '来源类型 check-盘点 manual-人工';
COMMENT ON COLUMN wms_adjust.source_id IS '来源id';
COMMENT ON COLUMN wms_adjust.total_quantity IS '调整总数量';
COMMENT ON COLUMN wms_adjust.adjust_reason IS '调整原因';
COMMENT ON COLUMN wms_adjust.adjust_status IS '调整状态 pending-待审核 approved-已审核 completed-已完成';
COMMENT ON COLUMN wms_adjust.approve_id IS '审核人id';
COMMENT ON COLUMN wms_adjust.approve_time IS '审核时间';
COMMENT ON COLUMN wms_adjust.is_del IS '删除标志';
COMMENT ON COLUMN wms_adjust.create_by IS '创建者';
COMMENT ON COLUMN wms_adjust.create_time IS '创建时间';
COMMENT ON COLUMN wms_adjust.update_by IS '更新者';
COMMENT ON COLUMN wms_adjust.update_time IS '更新时间';
COMMENT ON COLUMN wms_adjust.remark IS '备注';

-- ---------------------------- wms_adjust_detail ----------------------------
DROP TABLE IF EXISTS wms_adjust_detail;
CREATE TABLE wms_adjust_detail (
  "detail_id" bigint NOT NULL,
  "adjust_id" bigint NOT NULL,
  "material_id" bigint NOT NULL,
  "batch_no" varchar(50) DEFAULT NULL,
  "zone_id" bigint DEFAULT NULL,
  "location_id" bigint DEFAULT NULL,
  "before_quantity" integer NOT NULL,
  "adjust_quantity" integer NOT NULL,
  "after_quantity" integer NOT NULL,
  "is_del" integer DEFAULT 0,
  "create_by" varchar(64) DEFAULT NULL,
  "create_time" timestamp DEFAULT NULL,
  "update_by" varchar(64) DEFAULT NULL,
  "update_time" timestamp DEFAULT NULL,
  "stock_id" bigint DEFAULT NULL,
  PRIMARY KEY (detail_id)
);
CREATE INDEX wms_adjust_detail_idx_adjust_id ON wms_adjust_detail (adjust_id);
CREATE INDEX wms_adjust_detail_idx_material_id ON wms_adjust_detail (material_id);
CREATE INDEX wms_adjust_detail_idx_zone_id ON wms_adjust_detail (zone_id);
CREATE INDEX wms_adjust_detail_idx_location_id ON wms_adjust_detail (location_id);
COMMENT ON COLUMN wms_adjust_detail.detail_id IS '明细id';
COMMENT ON COLUMN wms_adjust_detail.adjust_id IS '调整id';
COMMENT ON COLUMN wms_adjust_detail.material_id IS '物料id';
COMMENT ON COLUMN wms_adjust_detail.batch_no IS '批次号';
COMMENT ON COLUMN wms_adjust_detail.zone_id IS '库区id';
COMMENT ON COLUMN wms_adjust_detail.location_id IS '库位id';
COMMENT ON COLUMN wms_adjust_detail.before_quantity IS '调整前数量';
COMMENT ON COLUMN wms_adjust_detail.adjust_quantity IS '调整数量';
COMMENT ON COLUMN wms_adjust_detail.after_quantity IS '调整后数量';
COMMENT ON COLUMN wms_adjust_detail.is_del IS '删除标志';
COMMENT ON COLUMN wms_adjust_detail.create_by IS '创建者';
COMMENT ON COLUMN wms_adjust_detail.create_time IS '创建时间';
COMMENT ON COLUMN wms_adjust_detail.update_by IS '更新者';
COMMENT ON COLUMN wms_adjust_detail.update_time IS '更新时间';
COMMENT ON COLUMN wms_adjust_detail.stock_id IS '库存行id（空=盘盈新增）';

-- ---------------------------- wms_delivery ----------------------------
DROP TABLE IF EXISTS wms_delivery;
CREATE TABLE wms_delivery (
  "delivery_id" bigint NOT NULL,
  "delivery_no" varchar(50) NOT NULL,
  "warehouse_id" bigint NOT NULL,
  "zone_id" bigint DEFAULT NULL,
  "customer_id" bigint DEFAULT NULL,
  "delivery_date" timestamp NOT NULL,
  "total_quantity" integer DEFAULT NULL,
  "total_amount" numeric(18,4) DEFAULT NULL,
  "carrier_name" varchar(100) DEFAULT NULL,
  "waybill_no" varchar(100) DEFAULT NULL,
  "package_count" integer DEFAULT NULL,
  "total_weight" numeric(10,2) DEFAULT NULL,
  "delivery_status" varchar(20) DEFAULT NULL,
  "ship_time" timestamp DEFAULT NULL,
  "sign_time" timestamp DEFAULT NULL,
  "operator_id" bigint DEFAULT NULL,
  "is_del" integer DEFAULT 0,
  "create_by" varchar(64) DEFAULT NULL,
  "create_time" timestamp DEFAULT NULL,
  "update_by" varchar(64) DEFAULT NULL,
  "update_time" timestamp DEFAULT NULL,
  "source_type" varchar(20) DEFAULT NULL,
  "source_id" bigint DEFAULT NULL,
  "source_no" varchar(50) DEFAULT NULL,
  PRIMARY KEY (delivery_id)
);
CREATE UNIQUE INDEX wms_delivery_uk_delivery_no ON wms_delivery (delivery_no);
CREATE INDEX wms_delivery_idx_warehouse_id ON wms_delivery (warehouse_id);
CREATE INDEX wms_delivery_idx_zone_id ON wms_delivery (zone_id);
COMMENT ON COLUMN wms_delivery.delivery_id IS '出库id';
COMMENT ON COLUMN wms_delivery.delivery_no IS '出库单号';
COMMENT ON COLUMN wms_delivery.warehouse_id IS '仓库id';
COMMENT ON COLUMN wms_delivery.zone_id IS '发货库区id';
COMMENT ON COLUMN wms_delivery.customer_id IS '客户id';
COMMENT ON COLUMN wms_delivery.delivery_date IS '出库日期';
COMMENT ON COLUMN wms_delivery.total_quantity IS '总数量';
COMMENT ON COLUMN wms_delivery.total_amount IS '总金额';
COMMENT ON COLUMN wms_delivery.carrier_name IS '承运商';
COMMENT ON COLUMN wms_delivery.waybill_no IS '运单号';
COMMENT ON COLUMN wms_delivery.package_count IS '包裹数量';
COMMENT ON COLUMN wms_delivery.total_weight IS '总重量';
COMMENT ON COLUMN wms_delivery.delivery_status IS '出库状态 pending-待确认 shipped-已发货 delivered-已签收';
COMMENT ON COLUMN wms_delivery.ship_time IS '发货时间';
COMMENT ON COLUMN wms_delivery.sign_time IS '签收时间';
COMMENT ON COLUMN wms_delivery.operator_id IS '操作人id';
COMMENT ON COLUMN wms_delivery.is_del IS '删除标志';
COMMENT ON COLUMN wms_delivery.create_by IS '创建者';
COMMENT ON COLUMN wms_delivery.create_time IS '创建时间';
COMMENT ON COLUMN wms_delivery.update_by IS '更新者';
COMMENT ON COLUMN wms_delivery.update_time IS '更新时间';
COMMENT ON COLUMN wms_delivery.source_type IS '来源单据类型 sale-销售出库订单 issue-领料单 manual-手工创建';
COMMENT ON COLUMN wms_delivery.source_id IS '来源单据id';
COMMENT ON COLUMN wms_delivery.source_no IS '来源单据号';

-- ---------------------------- wms_delivery_detail ----------------------------
DROP TABLE IF EXISTS wms_delivery_detail;
CREATE TABLE wms_delivery_detail (
  "detail_id" bigint NOT NULL,
  "delivery_id" bigint NOT NULL,
  "material_id" bigint NOT NULL,
  "spec" varchar(100) DEFAULT NULL,
  "unit" varchar(20) DEFAULT NULL,
  "batch_no" varchar(50) DEFAULT NULL,
  "zone_id" bigint DEFAULT NULL,
  "location_id" bigint DEFAULT NULL,
  "delivery_quantity" integer NOT NULL,
  "unit_price" numeric(15,4) DEFAULT NULL,
  "amount" numeric(15,2) DEFAULT NULL,
  "is_del" integer DEFAULT 0,
  "create_by" varchar(64) DEFAULT NULL,
  "create_time" timestamp DEFAULT NULL,
  "update_by" varchar(64) DEFAULT NULL,
  "update_time" timestamp DEFAULT NULL,
  PRIMARY KEY (detail_id)
);
CREATE INDEX wms_delivery_detail_idx_delivery_id ON wms_delivery_detail (delivery_id);
CREATE INDEX wms_delivery_detail_idx_material_id ON wms_delivery_detail (material_id);
CREATE INDEX wms_delivery_detail_idx_zone_id ON wms_delivery_detail (zone_id);
CREATE INDEX wms_delivery_detail_idx_location_id ON wms_delivery_detail (location_id);
COMMENT ON COLUMN wms_delivery_detail.detail_id IS '明细id';
COMMENT ON COLUMN wms_delivery_detail.delivery_id IS '出库id';
COMMENT ON COLUMN wms_delivery_detail.material_id IS '物料id';
COMMENT ON COLUMN wms_delivery_detail.spec IS '规格';
COMMENT ON COLUMN wms_delivery_detail.unit IS '单位';
COMMENT ON COLUMN wms_delivery_detail.batch_no IS '批次号';
COMMENT ON COLUMN wms_delivery_detail.zone_id IS '库区id';
COMMENT ON COLUMN wms_delivery_detail.location_id IS '库位id';
COMMENT ON COLUMN wms_delivery_detail.delivery_quantity IS '出库数量';
COMMENT ON COLUMN wms_delivery_detail.unit_price IS '单价';
COMMENT ON COLUMN wms_delivery_detail.amount IS '金额';
COMMENT ON COLUMN wms_delivery_detail.is_del IS '删除标志';
COMMENT ON COLUMN wms_delivery_detail.create_by IS '创建者';
COMMENT ON COLUMN wms_delivery_detail.create_time IS '创建时间';
COMMENT ON COLUMN wms_delivery_detail.update_by IS '更新者';
COMMENT ON COLUMN wms_delivery_detail.update_time IS '更新时间';

-- ---------------------------- wms_freeze ----------------------------
DROP TABLE IF EXISTS wms_freeze;
CREATE TABLE wms_freeze (
  "freeze_id" bigint NOT NULL,
  "freeze_no" varchar(50) DEFAULT NULL,
  "material_id" bigint NOT NULL,
  "batch_no" varchar(50) DEFAULT NULL,
  "warehouse_id" bigint NOT NULL,
  "zone_id" bigint DEFAULT NULL,
  "location_id" bigint DEFAULT NULL,
  "freeze_quantity" integer NOT NULL,
  "freeze_type" varchar(20) DEFAULT NULL,
  "freeze_reason" varchar(255) DEFAULT NULL,
  "freeze_status" varchar(20) DEFAULT NULL,
  "freeze_user_id" bigint DEFAULT NULL,
  "freeze_time" timestamp DEFAULT NULL,
  "thaw_user_id" bigint DEFAULT NULL,
  "thaw_time" timestamp DEFAULT NULL,
  "thaw_reason" varchar(255) DEFAULT NULL,
  "is_del" integer DEFAULT 0,
  "create_by" varchar(64) DEFAULT NULL,
  "create_time" timestamp DEFAULT NULL,
  "update_by" varchar(64) DEFAULT NULL,
  "update_time" timestamp DEFAULT NULL,
  "stock_id" bigint DEFAULT NULL,
  PRIMARY KEY (freeze_id)
);
CREATE INDEX wms_freeze_idx_material_id ON wms_freeze (material_id);
CREATE INDEX wms_freeze_idx_zone_id ON wms_freeze (zone_id);
CREATE INDEX wms_freeze_idx_location_id ON wms_freeze (location_id);
CREATE INDEX wms_freeze_idx_batch_no ON wms_freeze (batch_no);
CREATE INDEX wms_freeze_idx_freeze_status ON wms_freeze (freeze_status);
COMMENT ON COLUMN wms_freeze.freeze_id IS '冻结id';
COMMENT ON COLUMN wms_freeze.freeze_no IS '冻结单号';
COMMENT ON COLUMN wms_freeze.material_id IS '物料id';
COMMENT ON COLUMN wms_freeze.batch_no IS '批次号';
COMMENT ON COLUMN wms_freeze.warehouse_id IS '仓库id';
COMMENT ON COLUMN wms_freeze.zone_id IS '库区id';
COMMENT ON COLUMN wms_freeze.location_id IS '库位id';
COMMENT ON COLUMN wms_freeze.freeze_quantity IS '冻结数量';
COMMENT ON COLUMN wms_freeze.freeze_type IS '冻结类型 quality-质量隔离 legal-司法查封 reserve-预留锁定 other-其他';
COMMENT ON COLUMN wms_freeze.freeze_reason IS '冻结原因';
COMMENT ON COLUMN wms_freeze.freeze_status IS '冻结状态 frozen-已冻结 thawed-已解冻';
COMMENT ON COLUMN wms_freeze.freeze_user_id IS '冻结人id';
COMMENT ON COLUMN wms_freeze.freeze_time IS '冻结时间';
COMMENT ON COLUMN wms_freeze.thaw_user_id IS '解冻人id';
COMMENT ON COLUMN wms_freeze.thaw_time IS '解冻时间';
COMMENT ON COLUMN wms_freeze.thaw_reason IS '解冻原因';
COMMENT ON COLUMN wms_freeze.is_del IS '删除标志';
COMMENT ON COLUMN wms_freeze.create_by IS '创建者';
COMMENT ON COLUMN wms_freeze.create_time IS '创建时间';
COMMENT ON COLUMN wms_freeze.update_by IS '更新者';
COMMENT ON COLUMN wms_freeze.update_time IS '更新时间';
COMMENT ON COLUMN wms_freeze.stock_id IS '库存行id';

-- ---------------------------- wms_inspect ----------------------------
DROP TABLE IF EXISTS wms_inspect;
CREATE TABLE wms_inspect (
  "inspect_id" bigint NOT NULL,
  "inspect_no" varchar(50) NOT NULL,
  "receiving_id" bigint DEFAULT NULL,
  "supplier_id" bigint DEFAULT NULL,
  "inspect_date" timestamp NOT NULL,
  "inspector_id" bigint DEFAULT NULL,
  "total_plan_quantity" integer DEFAULT NULL,
  "total_actual_quantity" integer DEFAULT NULL,
  "total_qualified_quantity" integer DEFAULT NULL,
  "total_unqualified_quantity" integer DEFAULT NULL,
  "inspect_result" varchar(20) DEFAULT NULL,
  "is_del" integer DEFAULT 0,
  "create_by" varchar(64) DEFAULT NULL,
  "create_time" timestamp DEFAULT NULL,
  "update_by" varchar(64) DEFAULT NULL,
  "update_time" timestamp DEFAULT NULL,
  PRIMARY KEY (inspect_id)
);
CREATE UNIQUE INDEX wms_inspect_uk_inspect_no ON wms_inspect (inspect_no);
CREATE INDEX wms_inspect_idx_arrival_id ON wms_inspect (receiving_id);
COMMENT ON COLUMN wms_inspect.inspect_id IS '验收id';
COMMENT ON COLUMN wms_inspect.inspect_no IS '验收单号';
COMMENT ON COLUMN wms_inspect.receiving_id IS '收货单id';
COMMENT ON COLUMN wms_inspect.supplier_id IS '供应商id';
COMMENT ON COLUMN wms_inspect.inspect_date IS '验收日期';
COMMENT ON COLUMN wms_inspect.inspector_id IS '验收人id';
COMMENT ON COLUMN wms_inspect.total_plan_quantity IS '计划数量';
COMMENT ON COLUMN wms_inspect.total_actual_quantity IS '实际数量';
COMMENT ON COLUMN wms_inspect.total_qualified_quantity IS '合格数量';
COMMENT ON COLUMN wms_inspect.total_unqualified_quantity IS '不合格数量';
COMMENT ON COLUMN wms_inspect.inspect_result IS '验收结果 all_pass-全部合格 part_pass-部分合格 all_fail-全部不合格';
COMMENT ON COLUMN wms_inspect.is_del IS '删除标志';
COMMENT ON COLUMN wms_inspect.create_by IS '创建者';
COMMENT ON COLUMN wms_inspect.create_time IS '创建时间';
COMMENT ON COLUMN wms_inspect.update_by IS '更新者';
COMMENT ON COLUMN wms_inspect.update_time IS '更新时间';

-- ---------------------------- wms_inspect_detail ----------------------------
DROP TABLE IF EXISTS wms_inspect_detail;
CREATE TABLE wms_inspect_detail (
  "detail_id" bigint NOT NULL,
  "inspect_id" bigint NOT NULL,
  "material_id" bigint NOT NULL,
  "batch_no" varchar(50) DEFAULT NULL,
  "plan_quantity" integer DEFAULT NULL,
  "actual_quantity" integer NOT NULL,
  "qualified_quantity" integer NOT NULL,
  "unqualified_quantity" integer DEFAULT 0,
  "unqualified_reason" varchar(255) DEFAULT NULL,
  "unit_price" numeric(15,4) DEFAULT NULL,
  "amount" numeric(15,2) DEFAULT NULL,
  "is_del" integer DEFAULT 0,
  "create_by" varchar(64) DEFAULT NULL,
  "create_time" timestamp DEFAULT NULL,
  "update_by" varchar(64) DEFAULT NULL,
  "update_time" timestamp DEFAULT NULL,
  PRIMARY KEY (detail_id)
);
CREATE INDEX wms_inspect_detail_idx_inspect_id ON wms_inspect_detail (inspect_id);
CREATE INDEX wms_inspect_detail_idx_material_id ON wms_inspect_detail (material_id);
COMMENT ON COLUMN wms_inspect_detail.detail_id IS '明细id';
COMMENT ON COLUMN wms_inspect_detail.inspect_id IS '验收id';
COMMENT ON COLUMN wms_inspect_detail.material_id IS '物料id';
COMMENT ON COLUMN wms_inspect_detail.batch_no IS '批次号';
COMMENT ON COLUMN wms_inspect_detail.plan_quantity IS '计划数量';
COMMENT ON COLUMN wms_inspect_detail.actual_quantity IS '实际数量';
COMMENT ON COLUMN wms_inspect_detail.qualified_quantity IS '合格数量';
COMMENT ON COLUMN wms_inspect_detail.unqualified_quantity IS '不合格数量';
COMMENT ON COLUMN wms_inspect_detail.unqualified_reason IS '不合格原因';
COMMENT ON COLUMN wms_inspect_detail.unit_price IS '单价';
COMMENT ON COLUMN wms_inspect_detail.amount IS '金额';
COMMENT ON COLUMN wms_inspect_detail.is_del IS '删除标志';
COMMENT ON COLUMN wms_inspect_detail.create_by IS '创建者';
COMMENT ON COLUMN wms_inspect_detail.create_time IS '创建时间';
COMMENT ON COLUMN wms_inspect_detail.update_by IS '更新者';
COMMENT ON COLUMN wms_inspect_detail.update_time IS '更新时间';

-- ---------------------------- wms_iqc_item ----------------------------
DROP TABLE IF EXISTS wms_iqc_item;
CREATE TABLE wms_iqc_item (
  "item_id" bigint NOT NULL,
  "category_id" varchar(50) NOT NULL,
  "item_code" varchar(50) DEFAULT NULL,
  "item_name" varchar(100) NOT NULL,
  "inspection_type" varchar(30) DEFAULT NULL,
  "standard_value" varchar(100) DEFAULT NULL,
  "unit" varchar(20) DEFAULT NULL,
  "is_critical" integer DEFAULT NULL,
  "test_method" varchar(200) DEFAULT NULL,
  "sort_no" integer DEFAULT NULL,
  "status" varchar(5) DEFAULT NULL,
  "remark" varchar(500) DEFAULT NULL,
  "is_del" integer NOT NULL,
  "create_by" varchar(50) DEFAULT NULL,
  "create_time" timestamp DEFAULT NULL,
  "update_by" varchar(50) DEFAULT NULL,
  "update_time" timestamp DEFAULT NULL,
  PRIMARY KEY (item_id)
);
COMMENT ON COLUMN wms_iqc_item.item_id IS '检验项id';
COMMENT ON COLUMN wms_iqc_item.category_id IS '物料分类';
COMMENT ON COLUMN wms_iqc_item.item_code IS '检验项编码';
COMMENT ON COLUMN wms_iqc_item.item_name IS '检验项名称';
COMMENT ON COLUMN wms_iqc_item.inspection_type IS '检验类型';
COMMENT ON COLUMN wms_iqc_item.standard_value IS '规格/标准值';
COMMENT ON COLUMN wms_iqc_item.unit IS '单位';
COMMENT ON COLUMN wms_iqc_item.is_critical IS '是否关键项';
COMMENT ON COLUMN wms_iqc_item.test_method IS '检测方法/工具';
COMMENT ON COLUMN wms_iqc_item.sort_no IS '排序号';
COMMENT ON COLUMN wms_iqc_item.status IS '状态';
COMMENT ON COLUMN wms_iqc_item.remark IS '备注';
COMMENT ON COLUMN wms_iqc_item.is_del IS '软删除标志';
COMMENT ON COLUMN wms_iqc_item.create_by IS '创建人';
COMMENT ON COLUMN wms_iqc_item.create_time IS '创建时间';
COMMENT ON COLUMN wms_iqc_item.update_by IS '修改人';
COMMENT ON COLUMN wms_iqc_item.update_time IS '修改时间';

-- ---------------------------- wms_iqc_order ----------------------------
DROP TABLE IF EXISTS wms_iqc_order;
CREATE TABLE wms_iqc_order (
  "inspection_id" bigint NOT NULL,
  "inspection_no" varchar(50) DEFAULT NULL,
  "receiving_id" bigint NOT NULL,
  "receiving_detail_id" bigint NOT NULL,
  "supplier_id" bigint DEFAULT NULL,
  "material_id" bigint NOT NULL,
  "spec" varchar(100) DEFAULT NULL,
  "unit" varchar(20) DEFAULT NULL,
  "batch_no" varchar(50) DEFAULT NULL,
  "delivery_quantity" integer DEFAULT NULL,
  "allow_quantity" integer DEFAULT 0,
  "defect_quantity" integer DEFAULT 0,
  "return_quantity" integer DEFAULT 0,
  "dispose_type" varchar(20) DEFAULT 'return',
  "inspect_status" varchar(20) DEFAULT NULL,
  "inspect_date" timestamp DEFAULT NULL,
  "inspector" varchar(50) DEFAULT NULL,
  "remark" varchar(500) DEFAULT NULL,
  "is_del" integer DEFAULT 0,
  "create_by" varchar(64) DEFAULT NULL,
  "create_time" timestamp DEFAULT NULL,
  "update_by" varchar(64) DEFAULT NULL,
  "update_time" timestamp DEFAULT NULL,
  PRIMARY KEY (inspection_id)
);
COMMENT ON COLUMN wms_iqc_order.inspection_id IS '检验单id';
COMMENT ON COLUMN wms_iqc_order.inspection_no IS '检验单编号';
COMMENT ON COLUMN wms_iqc_order.receiving_id IS '收货单id';
COMMENT ON COLUMN wms_iqc_order.receiving_detail_id IS '收货单明细id';
COMMENT ON COLUMN wms_iqc_order.supplier_id IS '供应商id';
COMMENT ON COLUMN wms_iqc_order.material_id IS '物料id';
COMMENT ON COLUMN wms_iqc_order.spec IS '规格';
COMMENT ON COLUMN wms_iqc_order.unit IS '单位';
COMMENT ON COLUMN wms_iqc_order.batch_no IS '批次号';
COMMENT ON COLUMN wms_iqc_order.delivery_quantity IS '收货数量';
COMMENT ON COLUMN wms_iqc_order.allow_quantity IS '允许入库数量';
COMMENT ON COLUMN wms_iqc_order.defect_quantity IS '不良数量';
COMMENT ON COLUMN wms_iqc_order.return_quantity IS '退货数量';
COMMENT ON COLUMN wms_iqc_order.dispose_type IS '不良品处置方式 return-退回供应商 concession-让步接收 sorting-挑选使用';
COMMENT ON COLUMN wms_iqc_order.inspect_status IS '状态 pending-待检验 inspecting-检验中 completed-已完成';
COMMENT ON COLUMN wms_iqc_order.inspect_date IS '检验日期';
COMMENT ON COLUMN wms_iqc_order.inspector IS '检验人';
COMMENT ON COLUMN wms_iqc_order.remark IS '备注';
COMMENT ON COLUMN wms_iqc_order.is_del IS '删除标志';
COMMENT ON COLUMN wms_iqc_order.create_by IS '创建者';
COMMENT ON COLUMN wms_iqc_order.create_time IS '创建时间';
COMMENT ON COLUMN wms_iqc_order.update_by IS '更新者';
COMMENT ON COLUMN wms_iqc_order.update_time IS '更新时间';

-- ---------------------------- wms_iqc_order_item ----------------------------
DROP TABLE IF EXISTS wms_iqc_order_item;
CREATE TABLE wms_iqc_order_item (
  "item_id" bigint NOT NULL,
  "inspection_id" bigint NOT NULL,
  "item_code" varchar(50) DEFAULT NULL,
  "item_name" varchar(100) NOT NULL,
  "inspection_type" varchar(30) DEFAULT NULL,
  "standard_value" varchar(100) DEFAULT NULL,
  "unit" varchar(20) DEFAULT NULL,
  "is_critical" integer DEFAULT NULL,
  "test_method" varchar(200) DEFAULT NULL,
  "inspect_result" varchar(500) DEFAULT NULL,
  "is_passed" smallint DEFAULT NULL,
  "sort_no" integer DEFAULT NULL,
  "is_del" integer DEFAULT 0,
  "create_by" varchar(64) DEFAULT NULL,
  "create_time" timestamp DEFAULT NULL,
  "update_by" varchar(64) DEFAULT NULL,
  "update_time" timestamp DEFAULT NULL,
  PRIMARY KEY (item_id)
);
COMMENT ON COLUMN wms_iqc_order_item.item_id IS '检验项id';
COMMENT ON COLUMN wms_iqc_order_item.inspection_id IS '检验单id';
COMMENT ON COLUMN wms_iqc_order_item.item_code IS '检验项编码';
COMMENT ON COLUMN wms_iqc_order_item.item_name IS '检验项名称';
COMMENT ON COLUMN wms_iqc_order_item.inspection_type IS '检验类型';
COMMENT ON COLUMN wms_iqc_order_item.standard_value IS '标准值';
COMMENT ON COLUMN wms_iqc_order_item.unit IS '单位';
COMMENT ON COLUMN wms_iqc_order_item.is_critical IS '是否关键项';
COMMENT ON COLUMN wms_iqc_order_item.test_method IS '检测方法/工具';
COMMENT ON COLUMN wms_iqc_order_item.inspect_result IS '检验结果';
COMMENT ON COLUMN wms_iqc_order_item.is_passed IS '是否通过';
COMMENT ON COLUMN wms_iqc_order_item.sort_no IS '排序号';
COMMENT ON COLUMN wms_iqc_order_item.is_del IS '删除标志';
COMMENT ON COLUMN wms_iqc_order_item.create_by IS '创建者';
COMMENT ON COLUMN wms_iqc_order_item.create_time IS '创建时间';
COMMENT ON COLUMN wms_iqc_order_item.update_by IS '更新者';
COMMENT ON COLUMN wms_iqc_order_item.update_time IS '更新时间';

-- ---------------------------- wms_issue_order ----------------------------
DROP TABLE IF EXISTS wms_issue_order;
CREATE TABLE wms_issue_order (
  "issue_id" bigint NOT NULL,
  "issue_no" varchar(50) DEFAULT NULL,
  "source_type" varchar(20) DEFAULT NULL,
  "source_no" varchar(50) DEFAULT NULL,
  "work_order_no" varchar(50) DEFAULT NULL,
  "dept_name" varchar(100) DEFAULT NULL,
  "receiver_id" bigint DEFAULT NULL,
  "receiver_name" varchar(50) DEFAULT NULL,
  "warehouse_id" bigint DEFAULT NULL,
  "warehouse_name" varchar(100) DEFAULT NULL,
  "require_date" timestamp DEFAULT NULL,
  "total_quantity" integer DEFAULT 0,
  "issued_quantity" integer DEFAULT 0,
  "shortage_quantity" integer DEFAULT 0,
  "issue_status" varchar(20) DEFAULT 'pending',
  "remark" varchar(500) DEFAULT NULL,
  "is_del" integer DEFAULT 0,
  "create_by" varchar(64) DEFAULT NULL,
  "create_time" timestamp DEFAULT NULL,
  "update_by" varchar(64) DEFAULT NULL,
  "update_time" timestamp DEFAULT NULL,
  "outbound_type" varchar(20) DEFAULT 'issue',
  "customer_name" varchar(100) DEFAULT NULL,
  PRIMARY KEY (issue_id)
);
CREATE UNIQUE INDEX wms_issue_order_uk_issue_no ON wms_issue_order (issue_no);
CREATE INDEX wms_issue_order_idx_work_order_no ON wms_issue_order (work_order_no);
CREATE INDEX wms_issue_order_idx_warehouse_id ON wms_issue_order (warehouse_id);
CREATE INDEX wms_issue_order_idx_issue_status ON wms_issue_order (issue_status);
COMMENT ON COLUMN wms_issue_order.issue_id IS '出库申请单id';
COMMENT ON COLUMN wms_issue_order.issue_no IS '出库申请单号';
COMMENT ON COLUMN wms_issue_order.source_type IS '来源类型 mes-MES下发 manual-手工新建';
COMMENT ON COLUMN wms_issue_order.source_no IS '来源单据号';
COMMENT ON COLUMN wms_issue_order.work_order_no IS '生产工单号';
COMMENT ON COLUMN wms_issue_order.dept_name IS '领用部门/车间';
COMMENT ON COLUMN wms_issue_order.receiver_id IS '领用人id';
COMMENT ON COLUMN wms_issue_order.receiver_name IS '领用人';
COMMENT ON COLUMN wms_issue_order.warehouse_id IS '仓库id';
COMMENT ON COLUMN wms_issue_order.warehouse_name IS '仓库名称';
COMMENT ON COLUMN wms_issue_order.require_date IS '需求日期';
COMMENT ON COLUMN wms_issue_order.total_quantity IS '需求总数量';
COMMENT ON COLUMN wms_issue_order.issued_quantity IS '已发总数量';
COMMENT ON COLUMN wms_issue_order.shortage_quantity IS '欠料总数量';
COMMENT ON COLUMN wms_issue_order.issue_status IS '发料状态 pending-待发料 partial-部分发料 issued-已发完 cancelled-已取消';
COMMENT ON COLUMN wms_issue_order.remark IS '备注';
COMMENT ON COLUMN wms_issue_order.is_del IS '删除标志';
COMMENT ON COLUMN wms_issue_order.create_by IS '创建者';
COMMENT ON COLUMN wms_issue_order.create_time IS '创建时间';
COMMENT ON COLUMN wms_issue_order.update_by IS '更新者';
COMMENT ON COLUMN wms_issue_order.update_time IS '更新时间';
COMMENT ON COLUMN wms_issue_order.outbound_type IS '出库类型 issue-生产领料 sales-销售发货 other-其他';
COMMENT ON COLUMN wms_issue_order.customer_name IS '客户名称';

-- ---------------------------- wms_issue_order_detail ----------------------------
DROP TABLE IF EXISTS wms_issue_order_detail;
CREATE TABLE wms_issue_order_detail (
  "detail_id" bigint NOT NULL,
  "issue_id" bigint NOT NULL,
  "material_id" bigint NOT NULL,
  "material_code" varchar(50) DEFAULT NULL,
  "material_name" varchar(100) DEFAULT NULL,
  "spec" varchar(100) DEFAULT NULL,
  "unit" varchar(20) DEFAULT NULL,
  "batch_no" varchar(50) DEFAULT NULL,
  "require_quantity" integer DEFAULT 0,
  "issued_quantity" integer DEFAULT 0,
  "shortage_quantity" integer DEFAULT NULL,
  "remark" varchar(500) DEFAULT NULL,
  "is_del" integer DEFAULT 0,
  "create_by" varchar(64) DEFAULT NULL,
  "create_time" timestamp DEFAULT NULL,
  "update_by" varchar(64) DEFAULT NULL,
  "update_time" timestamp DEFAULT NULL,
  PRIMARY KEY (detail_id)
);
CREATE INDEX wms_issue_order_detail_idx_issue_id ON wms_issue_order_detail (issue_id);
CREATE INDEX wms_issue_order_detail_idx_material_id ON wms_issue_order_detail (material_id);
COMMENT ON COLUMN wms_issue_order_detail.detail_id IS '明细id';
COMMENT ON COLUMN wms_issue_order_detail.issue_id IS '领料单id';
COMMENT ON COLUMN wms_issue_order_detail.material_id IS '物料id';
COMMENT ON COLUMN wms_issue_order_detail.material_code IS '物料编码';
COMMENT ON COLUMN wms_issue_order_detail.material_name IS '物料名称';
COMMENT ON COLUMN wms_issue_order_detail.spec IS '规格';
COMMENT ON COLUMN wms_issue_order_detail.unit IS '单位';
COMMENT ON COLUMN wms_issue_order_detail.batch_no IS '指定批次号';
COMMENT ON COLUMN wms_issue_order_detail.require_quantity IS '需求数量';
COMMENT ON COLUMN wms_issue_order_detail.issued_quantity IS '已发数量';
COMMENT ON COLUMN wms_issue_order_detail.shortage_quantity IS '欠料数量';
COMMENT ON COLUMN wms_issue_order_detail.remark IS '备注';
COMMENT ON COLUMN wms_issue_order_detail.is_del IS '删除标志';
COMMENT ON COLUMN wms_issue_order_detail.create_by IS '创建者';
COMMENT ON COLUMN wms_issue_order_detail.create_time IS '创建时间';
COMMENT ON COLUMN wms_issue_order_detail.update_by IS '更新者';
COMMENT ON COLUMN wms_issue_order_detail.update_time IS '更新时间';

-- ---------------------------- wms_location ----------------------------
DROP TABLE IF EXISTS wms_location;
CREATE TABLE wms_location (
  "location_id" bigint NOT NULL,
  "warehouse_id" bigint NOT NULL,
  "zone_id" bigint NOT NULL,
  "location_code" varchar(50) NOT NULL,
  "location_name" varchar(100) DEFAULT NULL,
  "location_type" varchar(20) DEFAULT NULL,
  "location_attr" varchar(50) DEFAULT NULL,
  "shelf_code" varchar(20) DEFAULT NULL,
  "length" numeric(18,4) DEFAULT NULL,
  "width" numeric(18,4) DEFAULT NULL,
  "height" numeric(18,4) DEFAULT NULL,
  "max_weight" numeric(18,4) DEFAULT NULL,
  "max_volume" numeric(18,4) DEFAULT NULL,
  "max_quantity" integer DEFAULT NULL,
  "location_status" varchar(1) DEFAULT NULL,
  "create_by" varchar(64) DEFAULT NULL,
  "create_time" timestamp DEFAULT NULL,
  "update_by" varchar(64) DEFAULT NULL,
  "update_time" timestamp DEFAULT NULL,
  "remark" varchar(500) DEFAULT NULL,
  "is_del" integer DEFAULT 0,
  PRIMARY KEY (location_id)
);
CREATE UNIQUE INDEX wms_location_uk_location_code ON wms_location (location_code);
CREATE INDEX wms_location_idx_warehouse_id ON wms_location (warehouse_id);
CREATE INDEX wms_location_idx_zone_id ON wms_location (zone_id);
CREATE INDEX wms_location_idx_status ON wms_location (location_status);
COMMENT ON COLUMN wms_location.location_id IS '库位id';
COMMENT ON COLUMN wms_location.warehouse_id IS '仓库id';
COMMENT ON COLUMN wms_location.zone_id IS '库区id';
COMMENT ON COLUMN wms_location.location_code IS '库位编码';
COMMENT ON COLUMN wms_location.location_name IS '库位名称';
COMMENT ON COLUMN wms_location.location_type IS '库位类型 storage-存储区 picking-拣货区 temp-暂存区 return-退货区';
COMMENT ON COLUMN wms_location.location_attr IS '库位属性 normal-常温 cold-低温 damp-防潮 dangerous-危险品';
COMMENT ON COLUMN wms_location.shelf_code IS '货架编号';
COMMENT ON COLUMN wms_location.length IS '长度';
COMMENT ON COLUMN wms_location.width IS '宽度';
COMMENT ON COLUMN wms_location.height IS '高度';
COMMENT ON COLUMN wms_location.max_weight IS '最大承重';
COMMENT ON COLUMN wms_location.max_volume IS '最大容量';
COMMENT ON COLUMN wms_location.max_quantity IS '最大存储数量';
COMMENT ON COLUMN wms_location.location_status IS '库位状态';
COMMENT ON COLUMN wms_location.create_by IS '创建者';
COMMENT ON COLUMN wms_location.create_time IS '创建时间';
COMMENT ON COLUMN wms_location.update_by IS '更新者';
COMMENT ON COLUMN wms_location.update_time IS '更新时间';
COMMENT ON COLUMN wms_location.remark IS '备注';
COMMENT ON COLUMN wms_location.is_del IS '删除标志';

-- ---------------------------- wms_location_lock ----------------------------
DROP TABLE IF EXISTS wms_location_lock;
CREATE TABLE wms_location_lock (
  "lock_id" bigint NOT NULL,
  "warehouse_id" bigint DEFAULT NULL,
  "zone_id" bigint DEFAULT NULL,
  "location_id" bigint NOT NULL,
  "lock_type" varchar(20) NOT NULL,
  "lock_reason" varchar(255) DEFAULT NULL,
  "lock_user_id" bigint DEFAULT NULL,
  "lock_time" timestamp DEFAULT NULL,
  "unlock_user_id" bigint DEFAULT NULL,
  "unlock_time" timestamp DEFAULT NULL,
  "is_del" integer DEFAULT 0,
  "create_by" varchar(64) DEFAULT NULL,
  "create_time" timestamp DEFAULT NULL,
  "update_by" varchar(64) DEFAULT NULL,
  "update_time" timestamp DEFAULT NULL,
  PRIMARY KEY (lock_id)
);
CREATE INDEX wms_location_lock_idx_location_id ON wms_location_lock (location_id);
CREATE INDEX wms_location_lock_idx_zone_id ON wms_location_lock (zone_id);
CREATE INDEX wms_location_lock_idx_lock_type ON wms_location_lock (lock_type);
COMMENT ON COLUMN wms_location_lock.lock_id IS '锁定id';
COMMENT ON COLUMN wms_location_lock.warehouse_id IS '仓库id';
COMMENT ON COLUMN wms_location_lock.zone_id IS '库区id';
COMMENT ON COLUMN wms_location_lock.location_id IS '库位id';
COMMENT ON COLUMN wms_location_lock.lock_type IS '锁定类型 check-盘点异常 maintain-维护 other-其他';
COMMENT ON COLUMN wms_location_lock.lock_reason IS '锁定原因';
COMMENT ON COLUMN wms_location_lock.lock_user_id IS '锁定人id';
COMMENT ON COLUMN wms_location_lock.lock_time IS '锁定时间';
COMMENT ON COLUMN wms_location_lock.unlock_user_id IS '解锁人id';
COMMENT ON COLUMN wms_location_lock.unlock_time IS '解锁时间';
COMMENT ON COLUMN wms_location_lock.is_del IS '删除标志';
COMMENT ON COLUMN wms_location_lock.create_by IS '创建者';
COMMENT ON COLUMN wms_location_lock.create_time IS '创建时间';
COMMENT ON COLUMN wms_location_lock.update_by IS '更新者';
COMMENT ON COLUMN wms_location_lock.update_time IS '更新时间';

-- ---------------------------- wms_lock ----------------------------
DROP TABLE IF EXISTS wms_lock;
CREATE TABLE wms_lock (
  "lock_id" bigint NOT NULL,
  "lock_type" varchar(20) DEFAULT NULL,
  "source_type" varchar(20) DEFAULT NULL,
  "source_id" bigint DEFAULT NULL,
  "source_no" varchar(50) DEFAULT NULL,
  "material_id" bigint NOT NULL,
  "batch_no" varchar(50) DEFAULT NULL,
  "warehouse_id" bigint NOT NULL,
  "zone_id" bigint DEFAULT NULL,
  "location_id" bigint DEFAULT NULL,
  "lock_quantity" integer NOT NULL,
  "used_quantity" integer DEFAULT 0,
  "lock_status" varchar(20) DEFAULT NULL,
  "lock_user_id" bigint DEFAULT NULL,
  "lock_time" timestamp DEFAULT NULL,
  "expire_time" timestamp DEFAULT NULL,
  "is_del" integer DEFAULT 0,
  "create_by" varchar(64) DEFAULT NULL,
  "create_time" timestamp DEFAULT NULL,
  "update_by" varchar(64) DEFAULT NULL,
  "update_time" timestamp DEFAULT NULL,
  "stock_id" bigint DEFAULT NULL,
  PRIMARY KEY (lock_id)
);
CREATE INDEX wms_lock_idx_material_id ON wms_lock (material_id);
CREATE INDEX wms_lock_idx_zone_id ON wms_lock (zone_id);
CREATE INDEX wms_lock_idx_location_id ON wms_lock (location_id);
CREATE INDEX wms_lock_idx_source_id ON wms_lock (source_id);
CREATE INDEX wms_lock_idx_lock_status ON wms_lock (lock_status);
COMMENT ON COLUMN wms_lock.lock_id IS '锁定id';
COMMENT ON COLUMN wms_lock.lock_type IS '锁定类型 order-订单预留 produce-生产预留 exhibit-展会预留 manual-手动锁定';
COMMENT ON COLUMN wms_lock.source_type IS '来源类型';
COMMENT ON COLUMN wms_lock.source_id IS '来源id';
COMMENT ON COLUMN wms_lock.source_no IS '来源单据号';
COMMENT ON COLUMN wms_lock.material_id IS '物料id';
COMMENT ON COLUMN wms_lock.batch_no IS '批次号';
COMMENT ON COLUMN wms_lock.warehouse_id IS '仓库id';
COMMENT ON COLUMN wms_lock.zone_id IS '库区id';
COMMENT ON COLUMN wms_lock.location_id IS '库位id';
COMMENT ON COLUMN wms_lock.lock_quantity IS '锁定数量';
COMMENT ON COLUMN wms_lock.used_quantity IS '已使用数量';
COMMENT ON COLUMN wms_lock.lock_status IS '锁定状态 locked-已锁定 partial-部分释放 released-已释放';
COMMENT ON COLUMN wms_lock.lock_user_id IS '锁定人id';
COMMENT ON COLUMN wms_lock.lock_time IS '锁定时间';
COMMENT ON COLUMN wms_lock.expire_time IS '过期时间';
COMMENT ON COLUMN wms_lock.is_del IS '删除标志';
COMMENT ON COLUMN wms_lock.create_by IS '创建者';
COMMENT ON COLUMN wms_lock.create_time IS '创建时间';
COMMENT ON COLUMN wms_lock.update_by IS '更新者';
COMMENT ON COLUMN wms_lock.update_time IS '更新时间';
COMMENT ON COLUMN wms_lock.stock_id IS '库存行id';

-- ---------------------------- wms_material ----------------------------
DROP TABLE IF EXISTS wms_material;
CREATE TABLE wms_material (
  "material_id" bigint NOT NULL,
  "material_code" varchar(50) NOT NULL,
  "material_name" varchar(100) NOT NULL,
  "material_short_name" varchar(50) DEFAULT NULL,
  "category_id" varchar(50) DEFAULT NULL,
  "spec" varchar(100) DEFAULT NULL,
  "barcode" varchar(100) DEFAULT NULL,
  "unit" varchar(20) DEFAULT NULL,
  "pack_spec_id" bigint DEFAULT NULL,
  "material_type" varchar(20) DEFAULT NULL,
  "default_warehouse_id" bigint DEFAULT NULL,
  "default_zone_id" bigint DEFAULT NULL,
  "shelf_life" integer DEFAULT NULL,
  "min_stock" numeric(18,4) DEFAULT NULL,
  "max_stock" numeric(18,4) DEFAULT NULL,
  "purchase_cycle" integer DEFAULT NULL,
  "weight" numeric(18,4) DEFAULT NULL,
  "volume" numeric(18,4) DEFAULT NULL,
  "image_url" varchar(500) DEFAULT NULL,
  "status" varchar(1) DEFAULT NULL,
  "is_del" integer DEFAULT 0,
  "create_by" varchar(64) DEFAULT NULL,
  "create_time" timestamp DEFAULT NULL,
  "update_by" varchar(64) DEFAULT NULL,
  "update_time" timestamp DEFAULT NULL,
  "remark" varchar(500) DEFAULT NULL,
  "default_location_id" bigint DEFAULT NULL,
  PRIMARY KEY (material_id)
);
CREATE UNIQUE INDEX wms_material_uk_material_code ON wms_material (material_code);
CREATE INDEX wms_material_idx_category_id ON wms_material (category_id);
CREATE INDEX wms_material_idx_barcode ON wms_material (barcode);
CREATE INDEX wms_material_idx_status ON wms_material (status);
COMMENT ON COLUMN wms_material.material_id IS '物料id';
COMMENT ON COLUMN wms_material.material_code IS '物料编码';
COMMENT ON COLUMN wms_material.material_name IS '物料名称';
COMMENT ON COLUMN wms_material.material_short_name IS '物料简称';
COMMENT ON COLUMN wms_material.category_id IS '物料分类';
COMMENT ON COLUMN wms_material.spec IS '规格型号';
COMMENT ON COLUMN wms_material.barcode IS '条码';
COMMENT ON COLUMN wms_material.unit IS '计量单位';
COMMENT ON COLUMN wms_material.pack_spec_id IS '包装规格id';
COMMENT ON COLUMN wms_material.material_type IS '物料类型';
COMMENT ON COLUMN wms_material.default_warehouse_id IS '默认仓库id';
COMMENT ON COLUMN wms_material.default_zone_id IS '默认库区id';
COMMENT ON COLUMN wms_material.shelf_life IS '保质期天数';
COMMENT ON COLUMN wms_material.min_stock IS '安全库存量';
COMMENT ON COLUMN wms_material.max_stock IS '最大库存量';
COMMENT ON COLUMN wms_material.purchase_cycle IS '采购周期';
COMMENT ON COLUMN wms_material.weight IS '重量';
COMMENT ON COLUMN wms_material.volume IS '体积';
COMMENT ON COLUMN wms_material.image_url IS '物料图片';
COMMENT ON COLUMN wms_material.status IS '状态';
COMMENT ON COLUMN wms_material.is_del IS '删除标志';
COMMENT ON COLUMN wms_material.create_by IS '创建者';
COMMENT ON COLUMN wms_material.create_time IS '创建时间';
COMMENT ON COLUMN wms_material.update_by IS '更新者';
COMMENT ON COLUMN wms_material.update_time IS '更新时间';
COMMENT ON COLUMN wms_material.remark IS '备注';
COMMENT ON COLUMN wms_material.default_location_id IS '默认库位id';

-- ---------------------------- wms_material_supplier ----------------------------
DROP TABLE IF EXISTS wms_material_supplier;
CREATE TABLE wms_material_supplier (
  "material_supplier_id" bigint NOT NULL,
  "material_id" bigint NOT NULL,
  "supplier_id" bigint NOT NULL,
  "moq" numeric(18,4) DEFAULT NULL,
  "lead_time" integer DEFAULT NULL,
  "minor_qty" integer DEFAULT NULL,
  "major_qty" integer DEFAULT NULL,
  "critical_qty" integer DEFAULT NULL,
  "sample_ratio" numeric(5,2) DEFAULT NULL,
  "aql_level" varchar(20) DEFAULT NULL,
  "inspection_mode" varchar(20) DEFAULT NULL,
  "is_default" varchar(1) DEFAULT NULL,
  "status" varchar(1) DEFAULT NULL,
  "is_del" integer DEFAULT 0,
  "create_by" varchar(64) DEFAULT NULL,
  "create_time" timestamp DEFAULT NULL,
  "update_by" varchar(64) DEFAULT NULL,
  "update_time" timestamp DEFAULT NULL,
  "remark" varchar(500) DEFAULT NULL,
  PRIMARY KEY (material_supplier_id)
);
COMMENT ON COLUMN wms_material_supplier.material_id IS '物料id';
COMMENT ON COLUMN wms_material_supplier.supplier_id IS '供应商id';
COMMENT ON COLUMN wms_material_supplier.moq IS '最小起订量';
COMMENT ON COLUMN wms_material_supplier.lead_time IS '交货周期';
COMMENT ON COLUMN wms_material_supplier.minor_qty IS '次要缺陷数量上限';
COMMENT ON COLUMN wms_material_supplier.major_qty IS '主要缺陷数量上限';
COMMENT ON COLUMN wms_material_supplier.critical_qty IS '严重缺陷数量上限';
COMMENT ON COLUMN wms_material_supplier.sample_ratio IS '抽检比例%';
COMMENT ON COLUMN wms_material_supplier.aql_level IS 'AQL标准级别';
COMMENT ON COLUMN wms_material_supplier.inspection_mode IS '质检模式 exempt-免检 sampling-抽检 all-全检';
COMMENT ON COLUMN wms_material_supplier.is_default IS '是否默认';
COMMENT ON COLUMN wms_material_supplier.status IS '状态';
COMMENT ON COLUMN wms_material_supplier.is_del IS '逻辑删除标记';
COMMENT ON COLUMN wms_material_supplier.create_by IS '创建人';
COMMENT ON COLUMN wms_material_supplier.create_time IS '创建时间';
COMMENT ON COLUMN wms_material_supplier.update_by IS '更新人';
COMMENT ON COLUMN wms_material_supplier.update_time IS '更新时间';
COMMENT ON COLUMN wms_material_supplier.remark IS '备注';

-- ---------------------------- wms_move ----------------------------
DROP TABLE IF EXISTS wms_move;
CREATE TABLE wms_move (
  "move_id" bigint NOT NULL,
  "move_no" varchar(50) NOT NULL,
  "warehouse_id" bigint NOT NULL,
  "move_type" varchar(20) DEFAULT NULL,
  "move_reason" varchar(255) DEFAULT NULL,
  "total_quantity" integer DEFAULT 0,
  "move_status" varchar(20) DEFAULT NULL,
  "operator_id" bigint DEFAULT NULL,
  "is_del" integer DEFAULT 0,
  "create_by" varchar(64) DEFAULT NULL,
  "create_time" timestamp DEFAULT NULL,
  "update_by" varchar(64) DEFAULT NULL,
  "update_time" timestamp DEFAULT NULL,
  "remark" varchar(500) DEFAULT NULL,
  PRIMARY KEY (move_id)
);
CREATE UNIQUE INDEX wms_move_uk_move_no ON wms_move (move_no);
CREATE INDEX wms_move_idx_warehouse_id ON wms_move (warehouse_id);
COMMENT ON COLUMN wms_move.move_id IS '移库id';
COMMENT ON COLUMN wms_move.move_no IS '移库单号';
COMMENT ON COLUMN wms_move.warehouse_id IS '仓库id';
COMMENT ON COLUMN wms_move.move_type IS '移库类型 adjust-库位调整 optimize-库位优化 zone_change-库区转移 sort-归类整理 split-拆层 merge-合并';
COMMENT ON COLUMN wms_move.move_reason IS '移库原因';
COMMENT ON COLUMN wms_move.total_quantity IS '移库总数量';
COMMENT ON COLUMN wms_move.move_status IS '移库状态 pending-待审核 approved-已审核 doing-移库中 completed-已完成 cancelled-已取消';
COMMENT ON COLUMN wms_move.operator_id IS '操作人id';
COMMENT ON COLUMN wms_move.is_del IS '删除标志';
COMMENT ON COLUMN wms_move.create_by IS '创建者';
COMMENT ON COLUMN wms_move.create_time IS '创建时间';
COMMENT ON COLUMN wms_move.update_by IS '更新者';
COMMENT ON COLUMN wms_move.update_time IS '更新时间';
COMMENT ON COLUMN wms_move.remark IS '备注';

-- ---------------------------- wms_move_detail ----------------------------
DROP TABLE IF EXISTS wms_move_detail;
CREATE TABLE wms_move_detail (
  "detail_id" bigint NOT NULL,
  "move_id" bigint NOT NULL,
  "material_id" bigint NOT NULL,
  "batch_no" varchar(50) DEFAULT NULL,
  "pack_level" varchar(30) DEFAULT NULL,
  "from_zone_id" bigint NOT NULL,
  "from_location_id" bigint NOT NULL,
  "to_zone_id" bigint NOT NULL,
  "to_location_id" bigint NOT NULL,
  "move_quantity" numeric(18,3) NOT NULL,
  "move_status" varchar(20) DEFAULT NULL,
  "move_time" timestamp DEFAULT NULL,
  "is_del" integer DEFAULT 0,
  "create_by" varchar(64) DEFAULT NULL,
  "create_time" timestamp DEFAULT NULL,
  "update_by" varchar(64) DEFAULT NULL,
  "update_time" timestamp DEFAULT NULL,
  "material_code" varchar(50) DEFAULT NULL,
  "material_name" varchar(100) DEFAULT NULL,
  "from_zone_code" varchar(50) DEFAULT NULL,
  "from_location_code" varchar(50) DEFAULT NULL,
  "to_zone_code" varchar(50) DEFAULT NULL,
  "to_location_code" varchar(50) DEFAULT NULL,
  PRIMARY KEY (detail_id)
);
CREATE INDEX wms_move_detail_idx_move_id ON wms_move_detail (move_id);
CREATE INDEX wms_move_detail_idx_material_id ON wms_move_detail (material_id);
COMMENT ON COLUMN wms_move_detail.detail_id IS '明细id';
COMMENT ON COLUMN wms_move_detail.move_id IS '移库id';
COMMENT ON COLUMN wms_move_detail.material_id IS '物料id';
COMMENT ON COLUMN wms_move_detail.batch_no IS '批次号';
COMMENT ON COLUMN wms_move_detail.pack_level IS '包装层级';
COMMENT ON COLUMN wms_move_detail.from_zone_id IS '源库区id';
COMMENT ON COLUMN wms_move_detail.from_location_id IS '原库位id';
COMMENT ON COLUMN wms_move_detail.to_zone_id IS '目标库区id';
COMMENT ON COLUMN wms_move_detail.to_location_id IS '目标库位id';
COMMENT ON COLUMN wms_move_detail.move_quantity IS '移库数量';
COMMENT ON COLUMN wms_move_detail.move_status IS '状态 pending-待执行 completed-已完成';
COMMENT ON COLUMN wms_move_detail.move_time IS '移库时间';
COMMENT ON COLUMN wms_move_detail.is_del IS '删除标志';
COMMENT ON COLUMN wms_move_detail.create_by IS '创建者';
COMMENT ON COLUMN wms_move_detail.create_time IS '创建时间';
COMMENT ON COLUMN wms_move_detail.update_by IS '更新者';
COMMENT ON COLUMN wms_move_detail.update_time IS '更新时间';
COMMENT ON COLUMN wms_move_detail.material_code IS '物料编码';
COMMENT ON COLUMN wms_move_detail.material_name IS '物料名称';
COMMENT ON COLUMN wms_move_detail.from_zone_code IS '源库区编码';
COMMENT ON COLUMN wms_move_detail.from_location_code IS '源库位编码';
COMMENT ON COLUMN wms_move_detail.to_zone_code IS '目标库区编码';
COMMENT ON COLUMN wms_move_detail.to_location_code IS '目标库位编码';

-- ---------------------------- wms_pack_spec ----------------------------
DROP TABLE IF EXISTS wms_pack_spec;
CREATE TABLE wms_pack_spec (
  "spec_id" bigint NOT NULL,
  "level_name" varchar(50) NOT NULL,
  "uom" varchar(20) DEFAULT NULL,
  "qty_per_parent" numeric(18,4) DEFAULT NULL,
  "is_continuous" integer DEFAULT NULL,
  "is_base" integer DEFAULT NULL,
  "sort_order" integer DEFAULT 0,
  "effective_date" timestamp DEFAULT NULL,
  "is_del" integer DEFAULT 0,
  "create_by" varchar(64) DEFAULT NULL,
  "create_time" timestamp DEFAULT NULL,
  "update_by" varchar(64) DEFAULT NULL,
  "update_time" timestamp DEFAULT NULL,
  "parent_id" bigint DEFAULT NULL,
  "root_id" bigint DEFAULT NULL,
  PRIMARY KEY (spec_id)
);
COMMENT ON COLUMN wms_pack_spec.spec_id IS '规格id';
COMMENT ON COLUMN wms_pack_spec.level_name IS '层级名称';
COMMENT ON COLUMN wms_pack_spec.uom IS '计量单位';
COMMENT ON COLUMN wms_pack_spec.qty_per_parent IS '上级层级包含的本层级数量';
COMMENT ON COLUMN wms_pack_spec.is_continuous IS '是否连续量';
COMMENT ON COLUMN wms_pack_spec.is_base IS '是否最小基本单位';
COMMENT ON COLUMN wms_pack_spec.sort_order IS '层级排序';
COMMENT ON COLUMN wms_pack_spec.effective_date IS '生效日期';
COMMENT ON COLUMN wms_pack_spec.is_del IS '删除标志';
COMMENT ON COLUMN wms_pack_spec.create_by IS '创建者';
COMMENT ON COLUMN wms_pack_spec.create_time IS '创建时间';
COMMENT ON COLUMN wms_pack_spec.update_by IS '更新者';
COMMENT ON COLUMN wms_pack_spec.update_time IS '更新时间';
COMMENT ON COLUMN wms_pack_spec.parent_id IS '父级ID';
COMMENT ON COLUMN wms_pack_spec.root_id IS '根节点ID';

-- ---------------------------- wms_partner ----------------------------
DROP TABLE IF EXISTS wms_partner;
CREATE TABLE wms_partner (
  "partner_id" bigint NOT NULL,
  "partner_code" varchar(50) NOT NULL,
  "partner_name" varchar(100) NOT NULL,
  "partner_type" varchar(20) NOT NULL,
  "province" varchar(50) DEFAULT NULL,
  "city" varchar(50) DEFAULT NULL,
  "district" varchar(50) DEFAULT NULL,
  "address" varchar(255) DEFAULT NULL,
  "bank_name" varchar(100) DEFAULT NULL,
  "bank_account" varchar(50) DEFAULT NULL,
  "tax_no" varchar(50) DEFAULT NULL,
  "partner_level" varchar(20) DEFAULT NULL,
  "settle_type" varchar(20) DEFAULT NULL,
  "credit_limit" numeric(15,2) DEFAULT NULL,
  "cooperate_start_date" timestamp DEFAULT NULL,
  "status" varchar(1) DEFAULT NULL,
  "is_del" integer DEFAULT 0,
  "create_by" varchar(64) DEFAULT NULL,
  "create_time" timestamp DEFAULT NULL,
  "update_by" varchar(64) DEFAULT NULL,
  "update_time" timestamp DEFAULT NULL,
  "remark" varchar(500) DEFAULT NULL,
  PRIMARY KEY (partner_id)
);
CREATE UNIQUE INDEX wms_partner_uk_partner_code ON wms_partner (partner_code);
COMMENT ON COLUMN wms_partner.partner_id IS '往来单位主键ID';
COMMENT ON COLUMN wms_partner.partner_code IS '往来单位编码';
COMMENT ON COLUMN wms_partner.partner_name IS '往来单位名称';
COMMENT ON COLUMN wms_partner.partner_type IS '往来单位类型';
COMMENT ON COLUMN wms_partner.province IS '省份';
COMMENT ON COLUMN wms_partner.city IS '城市';
COMMENT ON COLUMN wms_partner.district IS '区县';
COMMENT ON COLUMN wms_partner.address IS '详细地址';
COMMENT ON COLUMN wms_partner.bank_name IS '开户银行名称';
COMMENT ON COLUMN wms_partner.bank_account IS '银行账号';
COMMENT ON COLUMN wms_partner.tax_no IS '税号';
COMMENT ON COLUMN wms_partner.partner_level IS '合作等级';
COMMENT ON COLUMN wms_partner.settle_type IS '结算方式';
COMMENT ON COLUMN wms_partner.credit_limit IS '信用额度';
COMMENT ON COLUMN wms_partner.cooperate_start_date IS '合作开始日期';
COMMENT ON COLUMN wms_partner.status IS '状态';
COMMENT ON COLUMN wms_partner.is_del IS '逻辑删除标记';
COMMENT ON COLUMN wms_partner.create_by IS '创建人';
COMMENT ON COLUMN wms_partner.create_time IS '创建时间';
COMMENT ON COLUMN wms_partner.update_by IS '更新人';
COMMENT ON COLUMN wms_partner.update_time IS '更新时间';
COMMENT ON COLUMN wms_partner.remark IS '备注';

-- ---------------------------- wms_partner_address ----------------------------
DROP TABLE IF EXISTS wms_partner_address;
CREATE TABLE wms_partner_address (
  "address_id" bigint NOT NULL,
  "partner_id" bigint NOT NULL,
  "address_type" varchar(20) DEFAULT NULL,
  "province" varchar(50) DEFAULT NULL,
  "city" varchar(50) DEFAULT NULL,
  "district" varchar(50) DEFAULT NULL,
  "address" varchar(255) NOT NULL,
  "contact_name" varchar(50) DEFAULT NULL,
  "contact_phone" varchar(20) DEFAULT NULL,
  "is_default" varchar(1) DEFAULT NULL,
  "is_del" integer DEFAULT 0,
  "create_by" varchar(64) DEFAULT NULL,
  "create_time" timestamp DEFAULT NULL,
  "update_by" varchar(64) DEFAULT NULL,
  "update_time" timestamp DEFAULT NULL,
  PRIMARY KEY (address_id)
);
CREATE INDEX wms_partner_address_idx_partner_id ON wms_partner_address (partner_id);
COMMENT ON COLUMN wms_partner_address.address_id IS '地址主键ID';
COMMENT ON COLUMN wms_partner_address.partner_id IS '关联往来单位ID';
COMMENT ON COLUMN wms_partner_address.address_type IS '地址类型';
COMMENT ON COLUMN wms_partner_address.province IS '省份';
COMMENT ON COLUMN wms_partner_address.city IS '城市';
COMMENT ON COLUMN wms_partner_address.district IS '区县';
COMMENT ON COLUMN wms_partner_address.address IS '详细地址';
COMMENT ON COLUMN wms_partner_address.contact_name IS '联系人姓名';
COMMENT ON COLUMN wms_partner_address.contact_phone IS '联系电话';
COMMENT ON COLUMN wms_partner_address.is_default IS '是否默认';
COMMENT ON COLUMN wms_partner_address.is_del IS '逻辑删除标记';
COMMENT ON COLUMN wms_partner_address.create_by IS '创建人';
COMMENT ON COLUMN wms_partner_address.create_time IS '创建时间';
COMMENT ON COLUMN wms_partner_address.update_by IS '更新人';
COMMENT ON COLUMN wms_partner_address.update_time IS '更新时间';

-- ---------------------------- wms_partner_contact ----------------------------
DROP TABLE IF EXISTS wms_partner_contact;
CREATE TABLE wms_partner_contact (
  "contact_id" bigint NOT NULL AUTO_INCREMENT,
  "partner_id" bigint NOT NULL,
  "contact_name" varchar(50) DEFAULT NULL,
  "title" varchar(50) DEFAULT NULL,
  "phone" varchar(20) DEFAULT NULL,
  "email" varchar(100) DEFAULT NULL,
  "is_default" varchar(1) DEFAULT NULL,
  "remark" varchar(500) DEFAULT NULL,
  "is_del" integer DEFAULT 0,
  "create_by" varchar(64) DEFAULT NULL,
  "create_time" timestamp DEFAULT NULL,
  "update_by" varchar(64) DEFAULT NULL,
  "update_time" timestamp DEFAULT NULL,
  PRIMARY KEY (contact_id)
);
CREATE INDEX wms_partner_contact_idx_partner ON wms_partner_contact (partner_id);
COMMENT ON COLUMN wms_partner_contact.contact_id IS '联系人主键ID';
COMMENT ON COLUMN wms_partner_contact.partner_id IS '关联往来单位ID';
COMMENT ON COLUMN wms_partner_contact.contact_name IS '联系人姓名';
COMMENT ON COLUMN wms_partner_contact.title IS '职位';
COMMENT ON COLUMN wms_partner_contact.phone IS '联系电话';
COMMENT ON COLUMN wms_partner_contact.email IS '邮箱';
COMMENT ON COLUMN wms_partner_contact.is_default IS '是否默认';
COMMENT ON COLUMN wms_partner_contact.remark IS '备注';
COMMENT ON COLUMN wms_partner_contact.is_del IS '逻辑删除标记';
COMMENT ON COLUMN wms_partner_contact.create_by IS '创建人';
COMMENT ON COLUMN wms_partner_contact.create_time IS '创建时间';
COMMENT ON COLUMN wms_partner_contact.update_by IS '更新人';
COMMENT ON COLUMN wms_partner_contact.update_time IS '更新时间';

-- ---------------------------- wms_receipt ----------------------------
DROP TABLE IF EXISTS wms_receipt;
CREATE TABLE wms_receipt (
  "receipt_id" bigint NOT NULL,
  "receipt_no" varchar(50) NOT NULL,
  "receipt_type" varchar(20) NOT NULL,
  "warehouse_id" bigint NOT NULL,
  "zone_id" bigint DEFAULT NULL,
  "supplier_id" bigint DEFAULT NULL,
  "customer_id" bigint DEFAULT NULL,
  "source_type" varchar(20) DEFAULT NULL,
  "source_id" bigint DEFAULT NULL,
  "receipt_date" timestamp NOT NULL,
  "total_quantity" integer DEFAULT NULL,
  "operator_id" bigint DEFAULT NULL,
  "receipt_status" varchar(20) DEFAULT NULL,
  "approve_status" varchar(20) DEFAULT NULL,
  "approve_id" bigint DEFAULT NULL,
  "approve_time" timestamp DEFAULT NULL,
  "is_del" integer DEFAULT 0,
  "create_by" varchar(64) DEFAULT NULL,
  "create_time" timestamp DEFAULT NULL,
  "update_by" varchar(64) DEFAULT NULL,
  "update_time" timestamp DEFAULT NULL,
  "remark" varchar(500) DEFAULT NULL,
  PRIMARY KEY (receipt_id)
);
CREATE UNIQUE INDEX wms_receipt_uk_receipt_no ON wms_receipt (receipt_no);
CREATE INDEX wms_receipt_idx_warehouse_id ON wms_receipt (warehouse_id);
CREATE INDEX wms_receipt_idx_zone_id ON wms_receipt (zone_id);
CREATE INDEX wms_receipt_idx_receipt_date ON wms_receipt (receipt_date);
COMMENT ON COLUMN wms_receipt.receipt_id IS '入库id';
COMMENT ON COLUMN wms_receipt.receipt_no IS '入库单号';
COMMENT ON COLUMN wms_receipt.receipt_type IS '入库类型 purchase-采购入库 produce-生产入库 pending_in-待入库 return_in-退货入库 transfer_in-调拨入库 other-其他入库';
COMMENT ON COLUMN wms_receipt.warehouse_id IS '仓库id';
COMMENT ON COLUMN wms_receipt.zone_id IS '默认入库库区id';
COMMENT ON COLUMN wms_receipt.supplier_id IS '供应商id';
COMMENT ON COLUMN wms_receipt.customer_id IS '客户id';
COMMENT ON COLUMN wms_receipt.source_type IS '来源单据类型';
COMMENT ON COLUMN wms_receipt.source_id IS '来源单据id';
COMMENT ON COLUMN wms_receipt.receipt_date IS '入库日期';
COMMENT ON COLUMN wms_receipt.total_quantity IS '总数量';
COMMENT ON COLUMN wms_receipt.operator_id IS '作业人员id';
COMMENT ON COLUMN wms_receipt.receipt_status IS '入库状态 pending_in-待入库 in_stock-已入库 completed-已完成';
COMMENT ON COLUMN wms_receipt.approve_status IS '审核状态 0未审核 1已审核';
COMMENT ON COLUMN wms_receipt.approve_id IS '审核人id';
COMMENT ON COLUMN wms_receipt.approve_time IS '审核时间';
COMMENT ON COLUMN wms_receipt.is_del IS '删除标志';
COMMENT ON COLUMN wms_receipt.create_by IS '创建者';
COMMENT ON COLUMN wms_receipt.create_time IS '创建时间';
COMMENT ON COLUMN wms_receipt.update_by IS '更新者';
COMMENT ON COLUMN wms_receipt.update_time IS '更新时间';
COMMENT ON COLUMN wms_receipt.remark IS '备注';

-- ---------------------------- wms_receipt_detail ----------------------------
DROP TABLE IF EXISTS wms_receipt_detail;
CREATE TABLE wms_receipt_detail (
  "detail_id" bigint NOT NULL,
  "receipt_id" bigint NOT NULL,
  "material_id" bigint NOT NULL,
  "spec" varchar(100) DEFAULT NULL,
  "unit" varchar(20) DEFAULT NULL,
  "batch_no" varchar(50) DEFAULT NULL,
  "expire_date" timestamp DEFAULT NULL,
  "receipt_quantity" integer NOT NULL,
  "shelf_quantity" integer DEFAULT 0,
  "barcode" varchar(50) DEFAULT NULL,
  "detail_status" varchar(20) DEFAULT NULL,
  "location_id" bigint DEFAULT NULL,
  "is_del" integer DEFAULT 0,
  "create_by" varchar(64) DEFAULT NULL,
  "create_time" timestamp DEFAULT NULL,
  "update_by" varchar(64) DEFAULT NULL,
  "update_time" timestamp DEFAULT NULL,
  "materialion_date" timestamp DEFAULT NULL,
  "supplier_id" bigint DEFAULT NULL,
  PRIMARY KEY (detail_id)
);
CREATE UNIQUE INDEX wms_receipt_detail_uk_barcode ON wms_receipt_detail (barcode);
CREATE INDEX wms_receipt_detail_idx_receipt_id ON wms_receipt_detail (receipt_id);
CREATE INDEX wms_receipt_detail_idx_material_id ON wms_receipt_detail (material_id);
CREATE INDEX wms_receipt_detail_idx_location_id ON wms_receipt_detail (location_id);
CREATE INDEX wms_receipt_detail_idx_batch_no ON wms_receipt_detail (batch_no);
COMMENT ON COLUMN wms_receipt_detail.detail_id IS '明细id';
COMMENT ON COLUMN wms_receipt_detail.receipt_id IS '入库id';
COMMENT ON COLUMN wms_receipt_detail.material_id IS '物料id';
COMMENT ON COLUMN wms_receipt_detail.spec IS '规格';
COMMENT ON COLUMN wms_receipt_detail.unit IS '单位';
COMMENT ON COLUMN wms_receipt_detail.batch_no IS '批次号';
COMMENT ON COLUMN wms_receipt_detail.expire_date IS '有效期至';
COMMENT ON COLUMN wms_receipt_detail.receipt_quantity IS '入库数量';
COMMENT ON COLUMN wms_receipt_detail.shelf_quantity IS '已上架数量';
COMMENT ON COLUMN wms_receipt_detail.barcode IS '库存条码';
COMMENT ON COLUMN wms_receipt_detail.detail_status IS '上架状态';
COMMENT ON COLUMN wms_receipt_detail.location_id IS '库位id';
COMMENT ON COLUMN wms_receipt_detail.is_del IS '删除标志';
COMMENT ON COLUMN wms_receipt_detail.create_by IS '创建者';
COMMENT ON COLUMN wms_receipt_detail.create_time IS '创建时间';
COMMENT ON COLUMN wms_receipt_detail.update_by IS '更新者';
COMMENT ON COLUMN wms_receipt_detail.update_time IS '更新时间';
COMMENT ON COLUMN wms_receipt_detail.materialion_date IS '生产日期';
COMMENT ON COLUMN wms_receipt_detail.supplier_id IS '供应商id';

-- ---------------------------- wms_receipt_detail_barcode ----------------------------
DROP TABLE IF EXISTS wms_receipt_detail_barcode;
CREATE TABLE wms_receipt_detail_barcode (
  "barcode_id" bigint NOT NULL,
  "receipt_id" bigint NOT NULL,
  "detail_id" bigint NOT NULL,
  "barcode" varchar(50) DEFAULT NULL,
  "seq_no" integer DEFAULT NULL,
  "quantity" numeric(18,4) DEFAULT NULL,
  "base_quantity" numeric(18,4) DEFAULT NULL,
  "unit" varchar(20) DEFAULT NULL,
  "pack_spec_id" bigint DEFAULT NULL,
  "batch_no" varchar(50) DEFAULT NULL,
  "location_id" bigint DEFAULT NULL,
  "status" varchar(20) DEFAULT NULL,
  "shelve_time" timestamp DEFAULT NULL,
  "is_del" integer DEFAULT 0,
  "create_by" varchar(64) DEFAULT '',
  "create_time" timestamp DEFAULT NULL,
  "update_by" varchar(64) DEFAULT '',
  "update_time" timestamp DEFAULT NULL,
  PRIMARY KEY (barcode_id)
);
CREATE UNIQUE INDEX wms_receipt_detail_barcode_uk_barcode ON wms_receipt_detail_barcode (barcode);
CREATE INDEX wms_receipt_detail_barcode_idx_detail_id ON wms_receipt_detail_barcode (detail_id);
CREATE INDEX wms_receipt_detail_barcode_idx_receipt_id ON wms_receipt_detail_barcode (receipt_id);
COMMENT ON COLUMN wms_receipt_detail_barcode.barcode_id IS '条码id';
COMMENT ON COLUMN wms_receipt_detail_barcode.receipt_id IS '入库单id';
COMMENT ON COLUMN wms_receipt_detail_barcode.detail_id IS '入库明细id';
COMMENT ON COLUMN wms_receipt_detail_barcode.barcode IS '库存条码';
COMMENT ON COLUMN wms_receipt_detail_barcode.seq_no IS '箱序号';
COMMENT ON COLUMN wms_receipt_detail_barcode.quantity IS '本码承载数量';
COMMENT ON COLUMN wms_receipt_detail_barcode.base_quantity IS '折算基本单位数量';
COMMENT ON COLUMN wms_receipt_detail_barcode.unit IS '单位';
COMMENT ON COLUMN wms_receipt_detail_barcode.pack_spec_id IS '包装规格层级id';
COMMENT ON COLUMN wms_receipt_detail_barcode.batch_no IS '批次号';
COMMENT ON COLUMN wms_receipt_detail_barcode.location_id IS '上架库位id';
COMMENT ON COLUMN wms_receipt_detail_barcode.status IS '状态';
COMMENT ON COLUMN wms_receipt_detail_barcode.shelve_time IS '上架时间';
COMMENT ON COLUMN wms_receipt_detail_barcode.is_del IS '删除标志';
COMMENT ON COLUMN wms_receipt_detail_barcode.create_by IS '创建者';
COMMENT ON COLUMN wms_receipt_detail_barcode.create_time IS '创建时间';
COMMENT ON COLUMN wms_receipt_detail_barcode.update_by IS '更新者';
COMMENT ON COLUMN wms_receipt_detail_barcode.update_time IS '更新时间';

-- ---------------------------- wms_receiving ----------------------------
DROP TABLE IF EXISTS wms_receiving;
CREATE TABLE wms_receiving (
  "receiving_id" bigint NOT NULL,
  "receiving_no" varchar(50) NOT NULL,
  "supplier_id" bigint DEFAULT NULL,
  "expect_date" timestamp NOT NULL,
  "carrier_name" varchar(100) DEFAULT NULL,
  "carrier_phone" varchar(20) DEFAULT NULL,
  "total_quantity" integer DEFAULT NULL,
  "receiving_status" varchar(20) DEFAULT NULL,
  "is_del" integer DEFAULT 0,
  "create_by" varchar(64) DEFAULT NULL,
  "create_time" timestamp DEFAULT NULL,
  "update_by" varchar(64) DEFAULT NULL,
  "update_time" timestamp DEFAULT NULL,
  "remark" varchar(500) DEFAULT NULL,
  PRIMARY KEY (receiving_id)
);
CREATE UNIQUE INDEX wms_receiving_uk_receiving_no ON wms_receiving (receiving_no);
CREATE INDEX wms_receiving_idx_supplier_id ON wms_receiving (supplier_id);
CREATE INDEX wms_receiving_idx_expect_date ON wms_receiving (expect_date);
COMMENT ON COLUMN wms_receiving.receiving_id IS '收货单id';
COMMENT ON COLUMN wms_receiving.receiving_no IS '收货单号';
COMMENT ON COLUMN wms_receiving.supplier_id IS '供应商id';
COMMENT ON COLUMN wms_receiving.expect_date IS '期望到货日期';
COMMENT ON COLUMN wms_receiving.carrier_name IS '承运人';
COMMENT ON COLUMN wms_receiving.carrier_phone IS '承运人电话';
COMMENT ON COLUMN wms_receiving.total_quantity IS '收货总数量';
COMMENT ON COLUMN wms_receiving.receiving_status IS '收货状态 pending-待收货 arriving-收货中 completed-已完成 cancelled-已取消';
COMMENT ON COLUMN wms_receiving.is_del IS '删除标志';
COMMENT ON COLUMN wms_receiving.create_by IS '创建者';
COMMENT ON COLUMN wms_receiving.create_time IS '创建时间';
COMMENT ON COLUMN wms_receiving.update_by IS '更新者';
COMMENT ON COLUMN wms_receiving.update_time IS '更新时间';
COMMENT ON COLUMN wms_receiving.remark IS '备注';

-- ---------------------------- wms_receiving_detail ----------------------------
DROP TABLE IF EXISTS wms_receiving_detail;
CREATE TABLE wms_receiving_detail (
  "detail_id" bigint NOT NULL,
  "receiving_id" bigint NOT NULL,
  "material_id" bigint NOT NULL,
  "spec" varchar(100) DEFAULT NULL,
  "unit" varchar(20) DEFAULT NULL,
  "batch_no" varchar(50) DEFAULT NULL,
  "plan_quantity" integer NOT NULL,
  "stock_in_quantity" integer DEFAULT NULL,
  "pending_inbound_qty" integer DEFAULT NULL,
  "return_qty" integer DEFAULT 0,
  "is_del" integer DEFAULT 0,
  "create_by" varchar(64) DEFAULT NULL,
  "create_time" timestamp DEFAULT NULL,
  "update_by" varchar(64) DEFAULT NULL,
  "update_time" timestamp DEFAULT NULL,
  PRIMARY KEY (detail_id)
);
CREATE INDEX wms_receiving_detail_idx_receiving_id ON wms_receiving_detail (receiving_id);
CREATE INDEX wms_receiving_detail_idx_material_id ON wms_receiving_detail (material_id);
COMMENT ON COLUMN wms_receiving_detail.detail_id IS '明细id';
COMMENT ON COLUMN wms_receiving_detail.receiving_id IS '收货单id';
COMMENT ON COLUMN wms_receiving_detail.material_id IS '物料id';
COMMENT ON COLUMN wms_receiving_detail.spec IS '规格';
COMMENT ON COLUMN wms_receiving_detail.unit IS '单位';
COMMENT ON COLUMN wms_receiving_detail.batch_no IS '批次号';
COMMENT ON COLUMN wms_receiving_detail.plan_quantity IS '收货数量';
COMMENT ON COLUMN wms_receiving_detail.stock_in_quantity IS '入库数';
COMMENT ON COLUMN wms_receiving_detail.pending_inbound_qty IS '待入库数量';
COMMENT ON COLUMN wms_receiving_detail.return_qty IS '退货数量';
COMMENT ON COLUMN wms_receiving_detail.is_del IS '删除标志';
COMMENT ON COLUMN wms_receiving_detail.create_by IS '创建者';
COMMENT ON COLUMN wms_receiving_detail.create_time IS '创建时间';
COMMENT ON COLUMN wms_receiving_detail.update_by IS '更新者';
COMMENT ON COLUMN wms_receiving_detail.update_time IS '更新时间';

-- ---------------------------- wms_region ----------------------------
DROP TABLE IF EXISTS wms_region;
CREATE TABLE wms_region (
  "code" varchar(20) NOT NULL,
  "name" varchar(100) NOT NULL,
  "parent_code" varchar(20) DEFAULT NULL,
  "update_date" timestamp DEFAULT NULL,
  PRIMARY KEY (code)
);
COMMENT ON COLUMN wms_region.code IS '行政区划编码';
COMMENT ON COLUMN wms_region.name IS '名称';
COMMENT ON COLUMN wms_region.parent_code IS '父级行政区划编码';
COMMENT ON COLUMN wms_region.update_date IS '更新日期';

-- ---------------------------- wms_return_order ----------------------------
DROP TABLE IF EXISTS wms_return_order;
CREATE TABLE wms_return_order (
  "return_order_id" bigint NOT NULL,
  "return_order_no" varchar(50) NOT NULL,
  "return_type" varchar(20) DEFAULT NULL,
  "supplier_id" bigint DEFAULT NULL,
  "total_quantity" integer DEFAULT NULL,
  "is_del" integer DEFAULT 0,
  "create_by" varchar(64) DEFAULT NULL,
  "create_time" timestamp DEFAULT NULL,
  "update_by" varchar(64) DEFAULT NULL,
  "update_time" timestamp DEFAULT NULL,
  "remark" varchar(500) DEFAULT NULL,
  "return_status" varchar(20) DEFAULT NULL,
  PRIMARY KEY (return_order_id)
);
COMMENT ON COLUMN wms_return_order.return_order_id IS '退货单id';
COMMENT ON COLUMN wms_return_order.return_order_no IS '退货单号';
COMMENT ON COLUMN wms_return_order.return_type IS '退货类型 iqc_unqualified-质检不合格 other-其他';
COMMENT ON COLUMN wms_return_order.supplier_id IS '供应商id';
COMMENT ON COLUMN wms_return_order.total_quantity IS '退货总数量';
COMMENT ON COLUMN wms_return_order.is_del IS '删除标志';
COMMENT ON COLUMN wms_return_order.create_by IS '创建者';
COMMENT ON COLUMN wms_return_order.create_time IS '创建时间';
COMMENT ON COLUMN wms_return_order.update_by IS '更新者';
COMMENT ON COLUMN wms_return_order.update_time IS '更新时间';
COMMENT ON COLUMN wms_return_order.remark IS '备注';
COMMENT ON COLUMN wms_return_order.return_status IS '退货状态 pending-待处理 completed-已完成 cancelled-已取消';

-- ---------------------------- wms_return_order_detail ----------------------------
DROP TABLE IF EXISTS wms_return_order_detail;
CREATE TABLE wms_return_order_detail (
  "detail_id" bigint NOT NULL,
  "return_order_id" bigint NOT NULL,
  "material_id" bigint NOT NULL,
  "spec" varchar(100) DEFAULT NULL,
  "unit" varchar(20) DEFAULT NULL,
  "batch_no" varchar(50) DEFAULT NULL,
  "plan_quantity" integer NOT NULL,
  "return_reason" varchar(255) DEFAULT NULL,
  "location_id" bigint DEFAULT NULL,
  "is_del" integer DEFAULT 0,
  "create_by" varchar(64) DEFAULT NULL,
  "create_time" timestamp DEFAULT NULL,
  "update_by" varchar(64) DEFAULT NULL,
  "update_time" timestamp DEFAULT NULL,
  PRIMARY KEY (detail_id)
);
COMMENT ON COLUMN wms_return_order_detail.detail_id IS '明细id';
COMMENT ON COLUMN wms_return_order_detail.return_order_id IS '退货单id';
COMMENT ON COLUMN wms_return_order_detail.material_id IS '物料id';
COMMENT ON COLUMN wms_return_order_detail.spec IS '规格';
COMMENT ON COLUMN wms_return_order_detail.unit IS '单位';
COMMENT ON COLUMN wms_return_order_detail.batch_no IS '批次号';
COMMENT ON COLUMN wms_return_order_detail.plan_quantity IS '退货数量';
COMMENT ON COLUMN wms_return_order_detail.return_reason IS '退货原因';
COMMENT ON COLUMN wms_return_order_detail.location_id IS '默认库位id';
COMMENT ON COLUMN wms_return_order_detail.is_del IS '删除标志';
COMMENT ON COLUMN wms_return_order_detail.create_by IS '创建者';
COMMENT ON COLUMN wms_return_order_detail.create_time IS '创建时间';
COMMENT ON COLUMN wms_return_order_detail.update_by IS '更新者';
COMMENT ON COLUMN wms_return_order_detail.update_time IS '更新时间';

-- ---------------------------- wms_return_out ----------------------------
DROP TABLE IF EXISTS wms_return_out;
CREATE TABLE wms_return_out (
  "return_id" bigint NOT NULL,
  "return_no" varchar(50) NOT NULL,
  "delivery_id" bigint DEFAULT NULL,
  "customer_id" bigint DEFAULT NULL,
  "warehouse_id" bigint DEFAULT NULL,
  "zone_id" bigint DEFAULT NULL,
  "return_date" timestamp NOT NULL,
  "total_quantity" integer DEFAULT 0,
  "return_reason" varchar(255) DEFAULT NULL,
  "return_status" varchar(20) DEFAULT NULL,
  "is_del" integer DEFAULT 0,
  "create_by" varchar(64) DEFAULT NULL,
  "create_time" timestamp DEFAULT NULL,
  "update_by" varchar(64) DEFAULT NULL,
  "update_time" timestamp DEFAULT NULL,
  "return_type" varchar(20) DEFAULT NULL,
  PRIMARY KEY (return_id)
);
CREATE UNIQUE INDEX wms_return_out_uk_return_no ON wms_return_out (return_no);
COMMENT ON COLUMN wms_return_out.return_id IS '退货id';
COMMENT ON COLUMN wms_return_out.return_no IS '退货单号';
COMMENT ON COLUMN wms_return_out.delivery_id IS '原出库id';
COMMENT ON COLUMN wms_return_out.customer_id IS '客户id';
COMMENT ON COLUMN wms_return_out.warehouse_id IS '仓库id';
COMMENT ON COLUMN wms_return_out.zone_id IS '库区id';
COMMENT ON COLUMN wms_return_out.return_date IS '退货日期';
COMMENT ON COLUMN wms_return_out.total_quantity IS '退货总数量';
COMMENT ON COLUMN wms_return_out.return_reason IS '退货原因';
COMMENT ON COLUMN wms_return_out.return_status IS '退货状态 pending-待审核 approved-已审核 received-已收货 completed-已完成';
COMMENT ON COLUMN wms_return_out.is_del IS '删除标志';
COMMENT ON COLUMN wms_return_out.create_by IS '创建者';
COMMENT ON COLUMN wms_return_out.create_time IS '创建时间';
COMMENT ON COLUMN wms_return_out.update_by IS '更新者';
COMMENT ON COLUMN wms_return_out.update_time IS '更新时间';
COMMENT ON COLUMN wms_return_out.return_type IS '退货类型 return_out-退货出库';

-- ---------------------------- wms_stock ----------------------------
DROP TABLE IF EXISTS wms_stock;
CREATE TABLE wms_stock (
  "stock_id" bigint NOT NULL,
  "barcode" varchar(50) DEFAULT NULL,
  "material_id" bigint NOT NULL,
  "material_code" varchar(50) DEFAULT NULL,
  "material_name" varchar(100) DEFAULT NULL,
  "spec" varchar(100) DEFAULT NULL,
  "unit" varchar(20) DEFAULT NULL,
  "batch_no" varchar(50) DEFAULT NULL,
  "warehouse_id" bigint NOT NULL DEFAULT 0,
  "warehouse_name" varchar(100) DEFAULT NULL,
  "zone_id" bigint DEFAULT NULL,
  "zone_code" varchar(50) DEFAULT NULL,
  "zone_name" varchar(100) DEFAULT NULL,
  "location_id" bigint NOT NULL DEFAULT 0,
  "location_code" varchar(50) DEFAULT NULL,
  "location_name" varchar(100) DEFAULT NULL,
  "hu_id" bigint DEFAULT NULL,
  "pack_spec_id" bigint DEFAULT NULL,
  "supplier_id" bigint DEFAULT NULL,
  "receipt_id" bigint DEFAULT NULL,
  "receipt_detail_id" bigint DEFAULT NULL,
  "production_date" timestamp DEFAULT NULL,
  "expire_date" timestamp DEFAULT NULL,
  "stock_quantity" numeric(18,3) DEFAULT '0.000',
  "base_quantity" numeric(18,4) DEFAULT NULL,
  "available_quantity" numeric(18,3) DEFAULT '0.000',
  "freeze_quantity" numeric(18,3) DEFAULT '0.000',
  "lock_quantity" numeric(18,3) DEFAULT '0.000',
  "cost_price" numeric(15,4) DEFAULT NULL,
  "cost_amount" numeric(15,2) DEFAULT NULL,
  "is_del" integer NOT NULL DEFAULT 0,
  "create_by" varchar(64) DEFAULT '',
  "create_time" timestamp DEFAULT NULL,
  "update_by" varchar(64) DEFAULT '',
  "update_time" timestamp DEFAULT NULL,
  PRIMARY KEY (stock_id)
);
CREATE UNIQUE INDEX wms_stock_uk_barcode ON wms_stock (barcode);
CREATE UNIQUE INDEX wms_stock_uk_material_batch_location_hu ON wms_stock (material_id,batch_no,location_id,hu_id);
CREATE INDEX wms_stock_idx_warehouse_id ON wms_stock (warehouse_id);
CREATE INDEX wms_stock_idx_zone_id ON wms_stock (zone_id);
CREATE INDEX wms_stock_idx_location_id ON wms_stock (location_id);
CREATE INDEX wms_stock_idx_batch_no ON wms_stock (batch_no);
CREATE INDEX wms_stock_idx_hu_id ON wms_stock (hu_id);
COMMENT ON COLUMN wms_stock.stock_id IS '库存id';
COMMENT ON COLUMN wms_stock.barcode IS '库存条码';
COMMENT ON COLUMN wms_stock.material_id IS '物料id';
COMMENT ON COLUMN wms_stock.material_code IS '物料编码';
COMMENT ON COLUMN wms_stock.material_name IS '物料名称';
COMMENT ON COLUMN wms_stock.spec IS '规格';
COMMENT ON COLUMN wms_stock.unit IS '单位';
COMMENT ON COLUMN wms_stock.batch_no IS '批次号';
COMMENT ON COLUMN wms_stock.warehouse_id IS '仓库id';
COMMENT ON COLUMN wms_stock.warehouse_name IS '仓库名称';
COMMENT ON COLUMN wms_stock.zone_id IS '库区id';
COMMENT ON COLUMN wms_stock.zone_code IS '库区编码';
COMMENT ON COLUMN wms_stock.zone_name IS '库区名称';
COMMENT ON COLUMN wms_stock.location_id IS '库位id';
COMMENT ON COLUMN wms_stock.location_code IS '库位编码';
COMMENT ON COLUMN wms_stock.location_name IS '库位名称';
COMMENT ON COLUMN wms_stock.hu_id IS '处理单元id';
COMMENT ON COLUMN wms_stock.pack_spec_id IS '包装规格层级id';
COMMENT ON COLUMN wms_stock.supplier_id IS '供应商id';
COMMENT ON COLUMN wms_stock.receipt_id IS '来源入库单id';
COMMENT ON COLUMN wms_stock.receipt_detail_id IS '来源入库明细id';
COMMENT ON COLUMN wms_stock.production_date IS '生产日期';
COMMENT ON COLUMN wms_stock.expire_date IS '有效期至';
COMMENT ON COLUMN wms_stock.stock_quantity IS '库存数量';
COMMENT ON COLUMN wms_stock.base_quantity IS '基本单位数量';
COMMENT ON COLUMN wms_stock.available_quantity IS '可用数量';
COMMENT ON COLUMN wms_stock.freeze_quantity IS '冻结数量';
COMMENT ON COLUMN wms_stock.lock_quantity IS '锁定数量';
COMMENT ON COLUMN wms_stock.cost_price IS '成本单价';
COMMENT ON COLUMN wms_stock.cost_amount IS '成本金额';
COMMENT ON COLUMN wms_stock.is_del IS '删除标志';
COMMENT ON COLUMN wms_stock.create_by IS '创建者';
COMMENT ON COLUMN wms_stock.create_time IS '创建时间';
COMMENT ON COLUMN wms_stock.update_by IS '更新者';
COMMENT ON COLUMN wms_stock.update_time IS '更新时间';

-- ---------------------------- wms_stock_alert_config ----------------------------
DROP TABLE IF EXISTS wms_stock_alert_config;
CREATE TABLE wms_stock_alert_config (
  "config_id" bigint NOT NULL,
  "warehouse_id" bigint NOT NULL,
  "zone_id" bigint DEFAULT NULL,
  "location_id" bigint DEFAULT NULL,
  "material_id" bigint DEFAULT NULL,
  "warning_type" varchar(30) NOT NULL,
  "threshold_value" numeric(18,3) NOT NULL,
  "warning_level" varchar(10) DEFAULT 'normal',
  "notify_user_ids" varchar(500) DEFAULT NULL,
  "enabled" integer DEFAULT 1,
  "remark" varchar(255) DEFAULT NULL,
  "is_del" integer DEFAULT 0,
  "create_by" varchar(64) DEFAULT NULL,
  "create_time" timestamp DEFAULT NULL,
  "update_by" varchar(64) DEFAULT NULL,
  "update_time" timestamp DEFAULT NULL,
  PRIMARY KEY (config_id)
);
COMMENT ON COLUMN wms_stock_alert_config.config_id IS '配置id';
COMMENT ON COLUMN wms_stock_alert_config.warehouse_id IS '仓库id';
COMMENT ON COLUMN wms_stock_alert_config.zone_id IS '库区id';
COMMENT ON COLUMN wms_stock_alert_config.location_id IS '库位id';
COMMENT ON COLUMN wms_stock_alert_config.material_id IS '物料id';
COMMENT ON COLUMN wms_stock_alert_config.warning_type IS '预警类型 min_stock-低于下限 max_stock-高于上限';
COMMENT ON COLUMN wms_stock_alert_config.threshold_value IS '阈值';
COMMENT ON COLUMN wms_stock_alert_config.warning_level IS '预警级别 normal-一般 urgent-紧急 critical-危机';
COMMENT ON COLUMN wms_stock_alert_config.notify_user_ids IS '处理人（逗号分隔用户id）';
COMMENT ON COLUMN wms_stock_alert_config.enabled IS '是否启用 1-启用 0-停用';
COMMENT ON COLUMN wms_stock_alert_config.remark IS '备注';
COMMENT ON COLUMN wms_stock_alert_config.is_del IS '删除标志';
COMMENT ON COLUMN wms_stock_alert_config.create_by IS '创建者';
COMMENT ON COLUMN wms_stock_alert_config.create_time IS '创建时间';
COMMENT ON COLUMN wms_stock_alert_config.update_by IS '更新者';
COMMENT ON COLUMN wms_stock_alert_config.update_time IS '更新时间';

-- ---------------------------- wms_stock_record ----------------------------
DROP TABLE IF EXISTS wms_stock_record;
CREATE TABLE wms_stock_record (
  "record_id" bigint NOT NULL,
  "record_no" varchar(50) DEFAULT NULL,
  "material_id" bigint NOT NULL,
  "batch_no" varchar(50) DEFAULT NULL,
  "warehouse_id" bigint NOT NULL,
  "zone_id" bigint DEFAULT NULL,
  "location_id" bigint DEFAULT NULL,
  "biz_type" varchar(30) NOT NULL,
  "source_type" varchar(30) DEFAULT NULL,
  "source_id" bigint DEFAULT NULL,
  "source_no" varchar(50) DEFAULT NULL,
  "direction" varchar(10) NOT NULL,
  "before_quantity" numeric(18,3) DEFAULT '0.000',
  "change_quantity" numeric(18,3) NOT NULL,
  "after_quantity" numeric(18,3) DEFAULT '0.000',
  "unit_price" numeric(15,4) DEFAULT NULL,
  "amount" numeric(15,2) DEFAULT NULL,
  "operator_id" bigint DEFAULT NULL,
  "operate_time" timestamp DEFAULT NULL,
  "remark" varchar(500) DEFAULT NULL,
  "is_del" integer DEFAULT 0,
  "create_by" varchar(64) DEFAULT NULL,
  "create_time" timestamp DEFAULT NULL,
  "update_by" varchar(64) DEFAULT NULL,
  "update_time" timestamp DEFAULT NULL,
  PRIMARY KEY (record_id)
);
CREATE INDEX wms_stock_record_idx_material_id ON wms_stock_record (material_id);
CREATE INDEX wms_stock_record_idx_warehouse_id ON wms_stock_record (warehouse_id);
CREATE INDEX wms_stock_record_idx_zone_id ON wms_stock_record (zone_id);
CREATE INDEX wms_stock_record_idx_location_id ON wms_stock_record (location_id);
CREATE INDEX wms_stock_record_idx_source_no ON wms_stock_record (source_no);
CREATE INDEX wms_stock_record_idx_biz_type ON wms_stock_record (biz_type);
CREATE INDEX wms_stock_record_idx_operate_time ON wms_stock_record (operate_time);
COMMENT ON COLUMN wms_stock_record.record_id IS '记录id';
COMMENT ON COLUMN wms_stock_record.record_no IS '记录单号';
COMMENT ON COLUMN wms_stock_record.material_id IS '物料id';
COMMENT ON COLUMN wms_stock_record.batch_no IS '批次号';
COMMENT ON COLUMN wms_stock_record.warehouse_id IS '仓库id';
COMMENT ON COLUMN wms_stock_record.zone_id IS '库区id';
COMMENT ON COLUMN wms_stock_record.location_id IS '库位id';
COMMENT ON COLUMN wms_stock_record.biz_type IS '业务类型 receipt-入库 delivery-出库 move-移库 check-盘点 adjust-调整 freeze-冻结 thaw-解冻 lock-锁定 unlock-解锁';
COMMENT ON COLUMN wms_stock_record.source_type IS '来源单据类型';
COMMENT ON COLUMN wms_stock_record.source_id IS '来源单据id';
COMMENT ON COLUMN wms_stock_record.source_no IS '来源单据号';
COMMENT ON COLUMN wms_stock_record.direction IS '方向 in-入库 out-出库';
COMMENT ON COLUMN wms_stock_record.before_quantity IS '变动前数量';
COMMENT ON COLUMN wms_stock_record.change_quantity IS '变动数量';
COMMENT ON COLUMN wms_stock_record.after_quantity IS '变动后数量';
COMMENT ON COLUMN wms_stock_record.unit_price IS '单价';
COMMENT ON COLUMN wms_stock_record.amount IS '金额';
COMMENT ON COLUMN wms_stock_record.operator_id IS '操作人id';
COMMENT ON COLUMN wms_stock_record.operate_time IS '操作时间';
COMMENT ON COLUMN wms_stock_record.remark IS '备注';
COMMENT ON COLUMN wms_stock_record.is_del IS '删除标志';
COMMENT ON COLUMN wms_stock_record.create_by IS '创建者';
COMMENT ON COLUMN wms_stock_record.create_time IS '创建时间';
COMMENT ON COLUMN wms_stock_record.update_by IS '更新者';
COMMENT ON COLUMN wms_stock_record.update_time IS '更新时间';

-- ---------------------------- wms_stock_warning ----------------------------
DROP TABLE IF EXISTS wms_stock_warning;
CREATE TABLE wms_stock_warning (
  "warning_id" bigint NOT NULL,
  "warning_type" varchar(30) NOT NULL,
  "material_id" bigint NOT NULL,
  "warehouse_id" bigint DEFAULT NULL,
  "zone_id" bigint DEFAULT NULL,
  "current_quantity" integer DEFAULT NULL,
  "threshold_value" numeric(15,2) DEFAULT NULL,
  "diff_value" numeric(18,4) DEFAULT NULL,
  "warning_level" varchar(10) DEFAULT NULL,
  "warning_status" varchar(20) DEFAULT NULL,
  "handle_user_id" bigint DEFAULT NULL,
  "handle_time" timestamp DEFAULT NULL,
  "handle_remark" varchar(500) DEFAULT NULL,
  "is_del" integer DEFAULT 0,
  "create_by" varchar(64) DEFAULT NULL,
  "create_time" timestamp DEFAULT NULL,
  "update_by" varchar(64) DEFAULT NULL,
  "update_time" timestamp DEFAULT NULL,
  "config_id" bigint DEFAULT NULL,
  "location_id" bigint DEFAULT NULL,
  "notify_user_ids" varchar(500) DEFAULT NULL,
  "trigger_count" integer DEFAULT 1,
  "last_trigger_time" timestamp DEFAULT NULL,
  "resolved_time" timestamp DEFAULT NULL,
  "resolved_quantity" numeric(18,3) DEFAULT NULL,
  PRIMARY KEY (warning_id)
);
CREATE INDEX wms_stock_warning_idx_material_id ON wms_stock_warning (material_id);
CREATE INDEX wms_stock_warning_idx_warehouse_id ON wms_stock_warning (warehouse_id);
CREATE INDEX wms_stock_warning_idx_zone_id ON wms_stock_warning (zone_id);
CREATE INDEX wms_stock_warning_idx_warning_type ON wms_stock_warning (warning_type);
CREATE INDEX wms_stock_warning_idx_warning_status ON wms_stock_warning (warning_status);
COMMENT ON COLUMN wms_stock_warning.warning_id IS '预警id';
COMMENT ON COLUMN wms_stock_warning.warning_type IS '预警类型 min_stock-安全库存 max_stock-最大库存 shelf_life-保质期呆滞-stagnation';
COMMENT ON COLUMN wms_stock_warning.material_id IS '物料id';
COMMENT ON COLUMN wms_stock_warning.warehouse_id IS '仓库id';
COMMENT ON COLUMN wms_stock_warning.zone_id IS '库区id';
COMMENT ON COLUMN wms_stock_warning.current_quantity IS '当前库存';
COMMENT ON COLUMN wms_stock_warning.threshold_value IS '阈值';
COMMENT ON COLUMN wms_stock_warning.diff_value IS '差值';
COMMENT ON COLUMN wms_stock_warning.warning_level IS '预警级别 normal-一般 urgent-紧急 critical-危机';
COMMENT ON COLUMN wms_stock_warning.warning_status IS '处理状态 pending-待处理 handled-已处理 ignored-已忽略 resolved-已自动恢复';
COMMENT ON COLUMN wms_stock_warning.handle_user_id IS '处理人id';
COMMENT ON COLUMN wms_stock_warning.handle_time IS '处理时间';
COMMENT ON COLUMN wms_stock_warning.handle_remark IS '处理说明';
COMMENT ON COLUMN wms_stock_warning.is_del IS '删除标志';
COMMENT ON COLUMN wms_stock_warning.create_by IS '创建者';
COMMENT ON COLUMN wms_stock_warning.create_time IS '创建时间';
COMMENT ON COLUMN wms_stock_warning.update_by IS '更新者';
COMMENT ON COLUMN wms_stock_warning.update_time IS '更新时间';
COMMENT ON COLUMN wms_stock_warning.config_id IS '来源配置id';
COMMENT ON COLUMN wms_stock_warning.location_id IS '库位id';
COMMENT ON COLUMN wms_stock_warning.notify_user_ids IS '通知人（逗号分隔用户id）';
COMMENT ON COLUMN wms_stock_warning.trigger_count IS '本周期累计触发次数';
COMMENT ON COLUMN wms_stock_warning.last_trigger_time IS '最近触发时间';
COMMENT ON COLUMN wms_stock_warning.resolved_time IS '自动恢复时间';
COMMENT ON COLUMN wms_stock_warning.resolved_quantity IS '恢复时的库存量';

-- ---------------------------- wms_stock_warning_log ----------------------------
DROP TABLE IF EXISTS wms_stock_warning_log;
CREATE TABLE wms_stock_warning_log (
  "log_id" bigint NOT NULL,
  "warning_id" bigint NOT NULL,
  "config_id" bigint DEFAULT NULL,
  "material_id" bigint NOT NULL,
  "current_quantity" numeric(18,3) NOT NULL,
  "threshold_value" numeric(18,3) NOT NULL,
  "diff_value" numeric(18,3) DEFAULT NULL,
  "message_sent" integer DEFAULT 0,
  "create_time" timestamp DEFAULT NULL,
  PRIMARY KEY (log_id)
);
COMMENT ON COLUMN wms_stock_warning_log.log_id IS '明细id';
COMMENT ON COLUMN wms_stock_warning_log.warning_id IS '预警记录id';
COMMENT ON COLUMN wms_stock_warning_log.config_id IS '来源配置id';
COMMENT ON COLUMN wms_stock_warning_log.material_id IS '物料id';
COMMENT ON COLUMN wms_stock_warning_log.current_quantity IS '触发时库存量';
COMMENT ON COLUMN wms_stock_warning_log.threshold_value IS '阈值';
COMMENT ON COLUMN wms_stock_warning_log.diff_value IS '差值';
COMMENT ON COLUMN wms_stock_warning_log.message_sent IS '是否发送站内信 1-是 0-否';
COMMENT ON COLUMN wms_stock_warning_log.create_time IS '触发时间';

-- ---------------------------- wms_transfer ----------------------------
DROP TABLE IF EXISTS wms_transfer;
CREATE TABLE wms_transfer (
  "transfer_id" bigint NOT NULL,
  "transfer_no" varchar(50) NOT NULL,
  "from_warehouse_id" bigint NOT NULL,
  "to_warehouse_id" bigint NOT NULL,
  "transfer_type" varchar(20) DEFAULT 'normal',
  "transfer_reason" varchar(255) DEFAULT NULL,
  "total_quantity" numeric(18,3) DEFAULT '0.000',
  "out_quantity" numeric(18,3) DEFAULT '0.000',
  "in_quantity" numeric(18,3) DEFAULT '0.000',
  "in_transit_quantity" numeric(18,3) DEFAULT '0.000',
  "transfer_status" varchar(20) DEFAULT 'pending',
  "out_by" varchar(64) DEFAULT NULL,
  "out_time" timestamp DEFAULT NULL,
  "in_by" varchar(64) DEFAULT NULL,
  "in_time" timestamp DEFAULT NULL,
  "operator_id" bigint DEFAULT NULL,
  "is_del" integer DEFAULT 0,
  "create_by" varchar(64) DEFAULT '',
  "create_time" timestamp DEFAULT NULL,
  "update_by" varchar(64) DEFAULT '',
  "update_time" timestamp DEFAULT NULL,
  "remark" varchar(500) DEFAULT NULL,
  PRIMARY KEY (transfer_id)
);
CREATE UNIQUE INDEX wms_transfer_uk_transfer_no ON wms_transfer (transfer_no);
CREATE INDEX wms_transfer_idx_from_warehouse_id ON wms_transfer (from_warehouse_id);
CREATE INDEX wms_transfer_idx_to_warehouse_id ON wms_transfer (to_warehouse_id);
CREATE INDEX wms_transfer_idx_transfer_status ON wms_transfer (transfer_status);
COMMENT ON COLUMN wms_transfer.transfer_id IS '调拨id';
COMMENT ON COLUMN wms_transfer.transfer_no IS '调拨单号';
COMMENT ON COLUMN wms_transfer.from_warehouse_id IS '调出仓库id';
COMMENT ON COLUMN wms_transfer.to_warehouse_id IS '调入仓库id';
COMMENT ON COLUMN wms_transfer.transfer_type IS '调拨类型 normal-普通调拨 urgent-紧急调拨';
COMMENT ON COLUMN wms_transfer.transfer_reason IS '调拨原因';
COMMENT ON COLUMN wms_transfer.total_quantity IS '调拨总数量';
COMMENT ON COLUMN wms_transfer.out_quantity IS '已出库数量';
COMMENT ON COLUMN wms_transfer.in_quantity IS '已入库数量';
COMMENT ON COLUMN wms_transfer.in_transit_quantity IS '在途数量';
COMMENT ON COLUMN wms_transfer.transfer_status IS '调拨状态 pending-待调拨 in_transit-在途 completed-已完成 cancelled-已取消';
COMMENT ON COLUMN wms_transfer.out_by IS '移出人';
COMMENT ON COLUMN wms_transfer.out_time IS '移出时间';
COMMENT ON COLUMN wms_transfer.in_by IS '移入人';
COMMENT ON COLUMN wms_transfer.in_time IS '移入时间';
COMMENT ON COLUMN wms_transfer.operator_id IS '操作人id';
COMMENT ON COLUMN wms_transfer.is_del IS '删除标志';
COMMENT ON COLUMN wms_transfer.create_by IS '创建者';
COMMENT ON COLUMN wms_transfer.create_time IS '创建时间';
COMMENT ON COLUMN wms_transfer.update_by IS '更新者';
COMMENT ON COLUMN wms_transfer.update_time IS '更新时间';
COMMENT ON COLUMN wms_transfer.remark IS '备注';

-- ---------------------------- wms_transfer_detail ----------------------------
DROP TABLE IF EXISTS wms_transfer_detail;
CREATE TABLE wms_transfer_detail (
  "detail_id" bigint NOT NULL,
  "transfer_id" bigint NOT NULL,
  "material_id" bigint NOT NULL,
  "material_code" varchar(50) DEFAULT NULL,
  "material_name" varchar(100) DEFAULT NULL,
  "spec" varchar(100) DEFAULT NULL,
  "unit" varchar(20) DEFAULT NULL,
  "batch_no" varchar(50) DEFAULT NULL,
  "barcode" varchar(50) DEFAULT NULL,
  "from_zone_id" bigint DEFAULT NULL,
  "from_location_id" bigint DEFAULT NULL,
  "from_location_code" varchar(50) DEFAULT NULL,
  "to_zone_id" bigint DEFAULT NULL,
  "to_location_id" bigint DEFAULT NULL,
  "to_location_code" varchar(50) DEFAULT NULL,
  "transfer_quantity" numeric(18,3) NOT NULL,
  "out_quantity" numeric(18,3) DEFAULT '0.000',
  "in_quantity" numeric(18,3) DEFAULT '0.000',
  "in_transit_quantity" numeric(18,3) DEFAULT '0.000',
  "transfer_status" varchar(20) DEFAULT 'pending',
  "out_time" timestamp DEFAULT NULL,
  "in_time" timestamp DEFAULT NULL,
  "is_del" integer DEFAULT 0,
  "create_by" varchar(64) DEFAULT '',
  "create_time" timestamp DEFAULT NULL,
  "update_by" varchar(64) DEFAULT '',
  "update_time" timestamp DEFAULT NULL,
  "remark" varchar(500) DEFAULT NULL,
  PRIMARY KEY (detail_id)
);
CREATE INDEX wms_transfer_detail_idx_transfer_id ON wms_transfer_detail (transfer_id);
CREATE INDEX wms_transfer_detail_idx_material_id ON wms_transfer_detail (material_id);
COMMENT ON COLUMN wms_transfer_detail.detail_id IS '明细id';
COMMENT ON COLUMN wms_transfer_detail.transfer_id IS '调拨id';
COMMENT ON COLUMN wms_transfer_detail.material_id IS '物料id';
COMMENT ON COLUMN wms_transfer_detail.material_code IS '物料编码';
COMMENT ON COLUMN wms_transfer_detail.material_name IS '物料名称';
COMMENT ON COLUMN wms_transfer_detail.spec IS '规格型号';
COMMENT ON COLUMN wms_transfer_detail.unit IS '单位';
COMMENT ON COLUMN wms_transfer_detail.batch_no IS '批次号';
COMMENT ON COLUMN wms_transfer_detail.barcode IS '条码';
COMMENT ON COLUMN wms_transfer_detail.from_zone_id IS '调出库区id';
COMMENT ON COLUMN wms_transfer_detail.from_location_id IS '调出库位id';
COMMENT ON COLUMN wms_transfer_detail.from_location_code IS '调出库位编码';
COMMENT ON COLUMN wms_transfer_detail.to_zone_id IS '调入库区id';
COMMENT ON COLUMN wms_transfer_detail.to_location_id IS '调入库位id';
COMMENT ON COLUMN wms_transfer_detail.to_location_code IS '调入库位编码';
COMMENT ON COLUMN wms_transfer_detail.transfer_quantity IS '调拨数量';
COMMENT ON COLUMN wms_transfer_detail.out_quantity IS '已出库数量';
COMMENT ON COLUMN wms_transfer_detail.in_quantity IS '已入库数量';
COMMENT ON COLUMN wms_transfer_detail.in_transit_quantity IS '在途数量';
COMMENT ON COLUMN wms_transfer_detail.transfer_status IS '状态 pending-待调拨 in_transit-在途 completed-已完成';
COMMENT ON COLUMN wms_transfer_detail.out_time IS '移出时间';
COMMENT ON COLUMN wms_transfer_detail.in_time IS '移入时间';
COMMENT ON COLUMN wms_transfer_detail.is_del IS '删除标志';
COMMENT ON COLUMN wms_transfer_detail.create_by IS '创建者';
COMMENT ON COLUMN wms_transfer_detail.create_time IS '创建时间';
COMMENT ON COLUMN wms_transfer_detail.update_by IS '更新者';
COMMENT ON COLUMN wms_transfer_detail.update_time IS '更新时间';
COMMENT ON COLUMN wms_transfer_detail.remark IS '备注';

-- ---------------------------- wms_warehouse ----------------------------
DROP TABLE IF EXISTS wms_warehouse;
CREATE TABLE wms_warehouse (
  "warehouse_id" bigint NOT NULL,
  "warehouse_code" varchar(50) NOT NULL,
  "warehouse_name" varchar(100) NOT NULL,
  "warehouse_type" varchar(20) DEFAULT NULL,
  "province" varchar(50) DEFAULT NULL,
  "city" varchar(50) DEFAULT NULL,
  "district" varchar(50) DEFAULT NULL,
  "address" varchar(255) DEFAULT NULL,
  "manager_id" bigint DEFAULT NULL,
  "phone" varchar(20) DEFAULT NULL,
  "total_area" numeric(18,4) DEFAULT NULL,
  "usable_area" numeric(10,2) DEFAULT NULL,
  "status" varchar(1) DEFAULT NULL,
  "create_by" varchar(64) DEFAULT NULL,
  "create_time" timestamp DEFAULT NULL,
  "update_by" varchar(64) DEFAULT NULL,
  "update_time" timestamp DEFAULT NULL,
  "remark" varchar(500) DEFAULT NULL,
  "is_del" integer DEFAULT 0,
  PRIMARY KEY (warehouse_id)
);
CREATE UNIQUE INDEX wms_warehouse_uk_warehouse_code ON wms_warehouse (warehouse_code);
CREATE INDEX wms_warehouse_idx_status ON wms_warehouse (status);
COMMENT ON COLUMN wms_warehouse.warehouse_id IS '仓库id';
COMMENT ON COLUMN wms_warehouse.warehouse_code IS '仓库编码';
COMMENT ON COLUMN wms_warehouse.warehouse_name IS '仓库名称';
COMMENT ON COLUMN wms_warehouse.warehouse_type IS '仓库类型 warehouse-普通仓 cold-冷库 freezer-冷冻库';
COMMENT ON COLUMN wms_warehouse.province IS '省份';
COMMENT ON COLUMN wms_warehouse.city IS '城市';
COMMENT ON COLUMN wms_warehouse.district IS '区县';
COMMENT ON COLUMN wms_warehouse.address IS '详细地址';
COMMENT ON COLUMN wms_warehouse.manager_id IS '仓库负责人id';
COMMENT ON COLUMN wms_warehouse.phone IS '联系电话';
COMMENT ON COLUMN wms_warehouse.total_area IS '总面积';
COMMENT ON COLUMN wms_warehouse.usable_area IS '可用面积';
COMMENT ON COLUMN wms_warehouse.status IS '状态';
COMMENT ON COLUMN wms_warehouse.create_by IS '创建者';
COMMENT ON COLUMN wms_warehouse.create_time IS '创建时间';
COMMENT ON COLUMN wms_warehouse.update_by IS '更新者';
COMMENT ON COLUMN wms_warehouse.update_time IS '更新时间';
COMMENT ON COLUMN wms_warehouse.remark IS '备注';
COMMENT ON COLUMN wms_warehouse.is_del IS '删除标志';

-- ---------------------------- wms_warehouse_manager ----------------------------
DROP TABLE IF EXISTS wms_warehouse_manager;
CREATE TABLE wms_warehouse_manager (
  "id" bigint NOT NULL,
  "warehouse_id" bigint NOT NULL,
  "user_id" bigint NOT NULL,
  "role_type" varchar(20) DEFAULT NULL,
  "is_del" integer DEFAULT 0,
  "create_by" varchar(64) DEFAULT NULL,
  "create_time" timestamp DEFAULT NULL,
  "remark" varchar(500) DEFAULT NULL,
  PRIMARY KEY (id)
);
CREATE INDEX wms_warehouse_manager_idx_warehouse_id ON wms_warehouse_manager (warehouse_id);
CREATE INDEX wms_warehouse_manager_idx_user_id ON wms_warehouse_manager (user_id);
COMMENT ON COLUMN wms_warehouse_manager.id IS '主键id';
COMMENT ON COLUMN wms_warehouse_manager.warehouse_id IS '仓库id';
COMMENT ON COLUMN wms_warehouse_manager.user_id IS '仓管员用户id';
COMMENT ON COLUMN wms_warehouse_manager.role_type IS '角色类型 manager-仓库管理员 operator-作业人员';
COMMENT ON COLUMN wms_warehouse_manager.is_del IS '删除标志';
COMMENT ON COLUMN wms_warehouse_manager.create_by IS '创建者';
COMMENT ON COLUMN wms_warehouse_manager.create_time IS '创建时间';
COMMENT ON COLUMN wms_warehouse_manager.remark IS '备注';

-- ---------------------------- wms_zone ----------------------------
DROP TABLE IF EXISTS wms_zone;
CREATE TABLE wms_zone (
  "zone_id" bigint NOT NULL,
  "warehouse_id" bigint NOT NULL,
  "zone_code" varchar(50) NOT NULL,
  "zone_name" varchar(100) NOT NULL,
  "zone_type" varchar(20) DEFAULT NULL,
  "zone_attr" varchar(50) DEFAULT NULL,
  "floor" integer DEFAULT NULL,
  "zone_area" numeric(18,4) DEFAULT NULL,
  "shelf_rows" integer DEFAULT NULL,
  "shelf_cols" integer DEFAULT NULL,
  "shelf_levels" integer DEFAULT NULL,
  "zone_status" varchar(1) DEFAULT NULL,
  "create_by" varchar(64) DEFAULT NULL,
  "create_time" timestamp DEFAULT NULL,
  "update_by" varchar(64) DEFAULT NULL,
  "update_time" timestamp DEFAULT NULL,
  "remark" varchar(500) DEFAULT NULL,
  "is_del" integer DEFAULT 0,
  PRIMARY KEY (zone_id)
);
CREATE UNIQUE INDEX wms_zone_uk_zone_code ON wms_zone (zone_code);
CREATE INDEX wms_zone_idx_warehouse_id ON wms_zone (warehouse_id);
CREATE INDEX wms_zone_idx_zone_type ON wms_zone (zone_type);
CREATE INDEX wms_zone_idx_zone_status ON wms_zone (zone_status);
COMMENT ON COLUMN wms_zone.zone_id IS '库区id';
COMMENT ON COLUMN wms_zone.warehouse_id IS '仓库id';
COMMENT ON COLUMN wms_zone.zone_code IS '库区编码';
COMMENT ON COLUMN wms_zone.zone_name IS '库区名称';
COMMENT ON COLUMN wms_zone.zone_type IS '库区类型 receive-收货区 storage-存储区 picking-拣货区 ship-发货区 return-退货区 temp-暂存区 reject-不合格品区';
COMMENT ON COLUMN wms_zone.zone_attr IS '库区属性 normal-常温 cold-低温 freezer-冷冻 damp-防潮 dangerous-危险品';
COMMENT ON COLUMN wms_zone.floor IS '所在楼层';
COMMENT ON COLUMN wms_zone.zone_area IS '库区面积';
COMMENT ON COLUMN wms_zone.shelf_rows IS '货架排数';
COMMENT ON COLUMN wms_zone.shelf_cols IS '货架列数';
COMMENT ON COLUMN wms_zone.shelf_levels IS '货架层数';
COMMENT ON COLUMN wms_zone.zone_status IS '库区状态';
COMMENT ON COLUMN wms_zone.create_by IS '创建者';
COMMENT ON COLUMN wms_zone.create_time IS '创建时间';
COMMENT ON COLUMN wms_zone.update_by IS '更新者';
COMMENT ON COLUMN wms_zone.update_time IS '更新时间';
COMMENT ON COLUMN wms_zone.remark IS '备注';
COMMENT ON COLUMN wms_zone.is_del IS '删除标志';