-- Migration: 20260608_0001 - Revert conversion of Orders.Status to PostgreSQL enum
-- Down migration

BEGIN;

-- IMPORTANT: the enum type "order_status" must exist and contain the values
-- used by the application (Pending, Confirmed, Shipped, Delivered, Cancelled)
-- If it does not exist, create it before running this down migration.

-- 1) Remove default so we can change type back
ALTER TABLE "Orders"
    ALTER COLUMN "Status" DROP DEFAULT;

-- 2) Convert the column type back to the enum
ALTER TABLE "Orders"
    ALTER COLUMN "Status" TYPE order_status USING "Status"::order_status;

-- 3) Restore default as enum
ALTER TABLE "Orders"
    ALTER COLUMN "Status" SET DEFAULT 'Pending'::order_status;

COMMIT;
