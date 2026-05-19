-- Tabela de Produtos
CREATE TABLE IF NOT EXISTS Products (
    Id SERIAL PRIMARY KEY,
    Name VARCHAR(255) NOT NULL,
    Description TEXT NOT NULL,
    Price DECIMAL(10, 2) NOT NULL,
    OriginalPrice DECIMAL(10, 2),
    ImageUrl VARCHAR(500),
    Category VARCHAR(100),
    Material VARCHAR(100),
    Stock INT NOT NULL DEFAULT 0,
    IsFeatured BOOLEAN DEFAULT FALSE,
    IsNew BOOLEAN DEFAULT FALSE,
    Rating DECIMAL(3, 2) DEFAULT 0,
    ReviewCount INT DEFAULT 0,
    CreatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- Tabela de Pedidos
CREATE TABLE IF NOT EXISTS Orders (
    Id SERIAL PRIMARY KEY,
    CustomerName VARCHAR(255) NOT NULL,
    CustomerEmail VARCHAR(255) NOT NULL,
    CustomerPhone VARCHAR(20),
    ShippingAddress TEXT NOT NULL,
    Total DECIMAL(12, 2) NOT NULL,
    Status VARCHAR(50) NOT NULL DEFAULT 'Pending',
    CreatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- Tabela de Itens do Pedido
CREATE TABLE IF NOT EXISTS OrderItems (
    Id SERIAL PRIMARY KEY,
    OrderId INT NOT NULL,
    ProductId INT NOT NULL,
    ProductName VARCHAR(255) NOT NULL,
    Price DECIMAL(10, 2) NOT NULL,
    Quantity INT NOT NULL,
    FOREIGN KEY (OrderId) REFERENCES Orders(Id) ON DELETE CASCADE,
    FOREIGN KEY (ProductId) REFERENCES Products(Id)
);

-- Índices para melhor performance
CREATE INDEX IF NOT EXISTS idx_products_category ON Products(Category);
CREATE INDEX IF NOT EXISTS idx_products_featured ON Products(IsFeatured);
CREATE INDEX IF NOT EXISTS idx_orders_status ON Orders(Status);
CREATE INDEX IF NOT EXISTS idx_orders_email ON Orders(CustomerEmail);
CREATE INDEX IF NOT EXISTS idx_orderitems_order ON OrderItems(OrderId);

-- Dados de exemplo (opcional)
INSERT INTO Products (Name, Description, Price, OriginalPrice, Category, Material, Stock, IsFeatured, IsNew, Rating, ReviewCount)
VALUES 
    ('Colar de Ouro 18K', 'Colar elegante em ouro 18 quilates com design moderno', 299.99, 399.99, 'Colares', 'Ouro', 15, TRUE, TRUE, 4.8, 42),
    ('Anel de Diamante', 'Anel solitário com diamante natural certificado', 1299.99, 1599.99, 'Anéis', 'Ouro e Diamante', 8, TRUE, FALSE, 4.9, 28),
    ('Brinco de Pérola', 'Brincos de pérola natural água-doce', 149.99, 199.99, 'Brincos', 'Pérola', 25, FALSE, TRUE, 4.6, 35),
    ('Pulseira de Prata', 'Pulseira em prata esterlina 925', 89.99, 129.99, 'Pulseiras', 'Prata', 40, FALSE, FALSE, 4.5, 18);
