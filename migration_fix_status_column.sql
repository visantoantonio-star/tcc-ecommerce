-- Migration to fix Status column type from PostgreSQL enum to text
-- This allows the application to properly handle enum conversion at the application level

-- Step 1: Alter the Orders table to convert Status column from order_status to text
ALTER TABLE "Orders"
ALTER COLUMN "Status" DROP DEFAULT,
ALTER COLUMN "Status" TYPE text USING "Status"::text,
ALTER COLUMN "Status" SET DEFAULT 'Pending';

-- Step 2: Verify the change
SELECT column_name, data_type, column_default 
FROM information_schema.columns 
WHERE table_name = 'Orders' AND column_name = 'Status';
