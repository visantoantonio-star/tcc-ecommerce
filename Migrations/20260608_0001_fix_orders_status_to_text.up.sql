-- Migration: 20260608_0001 - Convert Orders.Status column from PostgreSQL enum to text
-- Up migration

BEGIN;

-- 1) Remove default so we can change type
ALTER TABLE "Orders"
    ALTER COLUMN "Status" DROP DEFAULT;

-- 2) Convert the column type to text using safe cast
ALTER TABLE "Orders"
    ALTER COLUMN "Status" TYPE text USING "Status"::text;

-- 3) Restore a sensible default (as text)
ALTER TABLE "Orders"
    ALTER COLUMN "Status" SET DEFAULT 'Pending';

COMMIT;

-- Note: this migration intentionally does NOT drop the "order_status" type.
-- If you are sure no other objects depend on the enum type you may drop it separately.
