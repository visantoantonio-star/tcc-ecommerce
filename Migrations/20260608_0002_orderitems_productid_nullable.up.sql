-- Migration: 20260608_0002 - Make OrderItems.ProductId nullable and set FK to ON DELETE SET NULL

BEGIN;

-- 1) Drop existing FK constraint (if named fk_orderitems_product as in the seed)
ALTER TABLE "OrderItems" DROP CONSTRAINT IF EXISTS fk_orderitems_product;

-- 2) Alter column to allow NULLs
ALTER TABLE "OrderItems"
    ALTER COLUMN "ProductId" DROP NOT NULL;

-- 3) Recreate FK with ON DELETE SET NULL
ALTER TABLE "OrderItems"
    ADD CONSTRAINT fk_orderitems_product FOREIGN KEY ("ProductId") REFERENCES "Products"("Id") ON DELETE SET NULL;

COMMIT;
