-- ============================================================
-- TCC E-commerce de Joias - Script PostgreSQL
-- ============================================================

-- Tipo ENUM para status do pedido
DO $$ BEGIN
    CREATE TYPE order_status AS ENUM (
        'Pending',
        'Confirmed',
        'Shipped',
        'Delivered',
        'Cancelled'
    );
EXCEPTION
    WHEN duplicate_object THEN NULL;
END $$;

-- ============================================================
-- Tabela de Produtos
-- ============================================================
CREATE TABLE IF NOT EXISTS "Products" (
    "Id"            SERIAL          PRIMARY KEY,
    "Name"          VARCHAR(255)    NOT NULL,
    "Description"   TEXT            NOT NULL,
    "Price"         NUMERIC(10, 2)  NOT NULL CHECK ("Price" >= 0),
    "OriginalPrice" NUMERIC(10, 2)  CHECK ("OriginalPrice" >= 0),
    "ImageUrl"      VARCHAR(500),
    "Category"      VARCHAR(100),
    "Material"      VARCHAR(100),
    "Stock"         INT             NOT NULL DEFAULT 0 CHECK ("Stock" >= 0),
    "IsFeatured"    BOOLEAN         NOT NULL DEFAULT FALSE,
    "IsNew"         BOOLEAN         NOT NULL DEFAULT FALSE,
    "Rating"        NUMERIC(3, 2)   NOT NULL DEFAULT 0 CHECK ("Rating" BETWEEN 0 AND 5),
    "ReviewCount"   INT             NOT NULL DEFAULT 0 CHECK ("ReviewCount" >= 0),
    "CreatedAt"     TIMESTAMPTZ     NOT NULL DEFAULT NOW()
);

-- ============================================================
-- Tabela de Pedidos
-- ============================================================
CREATE TABLE IF NOT EXISTS "Orders" (
    "Id"              SERIAL          PRIMARY KEY,
    "CustomerName"    VARCHAR(255)    NOT NULL,
    "CustomerEmail"   VARCHAR(255)    NOT NULL,
    "CustomerPhone"   VARCHAR(20),
    "ShippingAddress" TEXT            NOT NULL,
    "Total"           NUMERIC(12, 2)  NOT NULL CHECK ("Total" >= 0),
    "Status"          order_status    NOT NULL DEFAULT 'Pending',
    "CreatedAt"       TIMESTAMPTZ     NOT NULL DEFAULT NOW()
);

-- ============================================================
-- Tabela de Itens do Pedido
-- ============================================================
CREATE TABLE IF NOT EXISTS "OrderItems" (
    "Id"          SERIAL          PRIMARY KEY,
    "OrderId"     INT             NOT NULL,
    "ProductId"   INT             NOT NULL,
    "ProductName" VARCHAR(255)    NOT NULL,
    "Price"       NUMERIC(10, 2)  NOT NULL CHECK ("Price" >= 0),
    "Quantity"    INT             NOT NULL CHECK ("Quantity" > 0),
    CONSTRAINT fk_orderitems_order
        FOREIGN KEY ("OrderId")   REFERENCES "Orders"("Id")   ON DELETE CASCADE,
    CONSTRAINT fk_orderitems_product
        FOREIGN KEY ("ProductId") REFERENCES "Products"("Id") ON DELETE RESTRICT
);

-- ============================================================
-- Índices para melhor performance
-- ============================================================
CREATE INDEX IF NOT EXISTS idx_products_category   ON "Products"("Category");
CREATE INDEX IF NOT EXISTS idx_products_featured   ON "Products"("IsFeatured");
CREATE INDEX IF NOT EXISTS idx_products_is_new     ON "Products"("IsNew");
CREATE INDEX IF NOT EXISTS idx_orders_status       ON "Orders"("Status");
CREATE INDEX IF NOT EXISTS idx_orders_email        ON "Orders"("CustomerEmail");
CREATE INDEX IF NOT EXISTS idx_orders_created_at   ON "Orders"("CreatedAt" DESC);
CREATE INDEX IF NOT EXISTS idx_orderitems_order    ON "OrderItems"("OrderId");
CREATE INDEX IF NOT EXISTS idx_orderitems_product  ON "OrderItems"("ProductId");

-- ============================================================
-- Dados de exemplo
-- ============================================================
INSERT INTO "Products" ("Name", "Description", "Price", "OriginalPrice", "Category", "Material", "Stock", "IsFeatured", "IsNew", "Rating", "ReviewCount")
VALUES
    ('Colar de Ouro 18K',   'Colar elegante em ouro 18 quilates com design moderno',  299.99,  399.99, 'Colares',   'Ouro',             15, TRUE,  TRUE,  4.8, 42),
    ('Anel de Diamante',    'Anel solitário com diamante natural certificado',         1299.99, 1599.99,'Anéis',     'Ouro e Diamante',   8, TRUE,  FALSE, 4.9, 28),
    ('Brinco de Pérola',    'Brincos de pérola natural água-doce',                     149.99,  199.99, 'Brincos',   'Pérola',           25, FALSE, TRUE,  4.6, 35),
    ('Pulseira de Prata',   'Pulseira em prata esterlina 925',                          89.99,  129.99, 'Pulseiras', 'Prata',            40, FALSE, FALSE, 4.5, 18)
ON CONFLICT DO NOTHING;