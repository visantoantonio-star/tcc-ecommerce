-- Migration SQL: allow deleting products when only delivered or cancelled orders reference them.
-- This makes OrderItems.ProductId nullable and recreates the foreign key with ON DELETE SET NULL.

BEGIN;

ALTER TABLE "OrderItems" DROP CONSTRAINT IF EXISTS fk_orderitems_product;

ALTER TABLE "OrderItems"
    ALTER COLUMN "ProductId" DROP NOT NULL;

ALTER TABLE "OrderItems"
    ADD CONSTRAINT fk_orderitems_product FOREIGN KEY ("ProductId") REFERENCES "Products"("Id") ON DELETE SET NULL;

COMMIT;
