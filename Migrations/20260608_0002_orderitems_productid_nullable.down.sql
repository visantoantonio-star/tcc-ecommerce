-- Down migration: revert ProductId to NOT NULL and FK ON DELETE RESTRICT
-- WARNING: This will fail if there are NULL values in OrderItems.ProductId.

BEGIN;

ALTER TABLE "OrderItems" DROP CONSTRAINT IF EXISTS fk_orderitems_product;

-- Attempt to set column NOT NULL (will fail if NULLs exist)
ALTER TABLE "OrderItems"
    ALTER COLUMN "ProductId" SET NOT NULL;

-- Recreate FK with RESTRICT (previous behavior)
ALTER TABLE "OrderItems"
    ADD CONSTRAINT fk_orderitems_product FOREIGN KEY ("ProductId") REFERENCES "Products"("Id") ON DELETE RESTRICT;

COMMIT;
