using JewelryEcommerce.Domain;

namespace JewelryEcommerce.Services;

public interface IProductService
{
    List<Product> GetAll();
    List<Product> GetFeatured();
    List<Product> GetByCategory(string category);
    List<Product> Search(string query);
    Product? GetById(int id);
    void Add(Product product);
    void Update(Product product);
    void Delete(int id);
    List<string> GetCategories();
}

public class ProductService : IProductService
{
    private static List<Product> _products = new()
    {
        new Product { Id=1, Name="Anel Solitário Diamante", Description="Anel em ouro 18k com diamante central de 0.5ct. Corte brilhante, clareza VS2, cor G. Design clássico e atemporal para ocasiões especiais.", Price=4800, OriginalPrice=5500, ImageUrl="/images/products/ring1.jpg", Category="Anéis", Material="Ouro 18k + Diamante", Stock=5, IsFeatured=true, IsNew=false, Rating=4.9, ReviewCount=48 },
        new Product { Id=2, Name="Colar Veneziana Ouro", Description="Colar veneziana em ouro 18k, 45cm, largura 1.5mm. Fecho caixa com travas de segurança. Peça elegante para uso diário.", Price=1200, ImageUrl="/images/products/necklace1.jpg", Category="Colares", Material="Ouro 18k", Stock=12, IsFeatured=true, IsNew=false, Rating=4.7, ReviewCount=31 },
        new Product { Id=3, Name="Brinco Argola Diamantada", Description="Brincos argola em ouro 18k cravejados com diamantes. Diâmetro 25mm, peso total 4g. Fecho pressão.", Price=2900, OriginalPrice=3400, ImageUrl="/images/products/earring1.jpg", Category="Brincos", Material="Ouro 18k + Diamantes", Stock=8, IsFeatured=true, IsNew=true, Rating=4.8, ReviewCount=22 },
        new Product { Id=4, Name="Pulseira Riviera Safira", Description="Pulseira riviera em ouro branco 18k com safiras naturais e diamantes. 18cm, 11 pedras. Peça de luxo inigualável.", Price=8500, ImageUrl="/images/products/bracelet1.jpg", Category="Pulseiras", Material="Ouro Branco 18k + Safiras", Stock=3, IsFeatured=true, IsNew=false, Rating=5.0, ReviewCount=15 },
        new Product { Id=5, Name="Anel Aparador Esmeralda", Description="Anel aparador em ouro amarelo 18k com esmeralda central colombiana de 1ct e diamantes laterais. Exclusividade e charme.", Price=6200, ImageUrl="/images/products/ring2.jpg", Category="Anéis", Material="Ouro 18k + Esmeralda", Stock=4, IsFeatured=false, IsNew=true, Rating=4.6, ReviewCount=9 },
        new Product { Id=6, Name="Pingente Cruz Diamantes", Description="Pingente cruz em ouro branco 18k pavê com diamantes. Dimensão 2.5x1.5cm. Vendido sem corrente.", Price=1800, ImageUrl="/images/products/pendant1.jpg", Category="Pingentes", Material="Ouro Branco 18k + Diamantes", Stock=10, IsFeatured=false, IsNew=false, Rating=4.7, ReviewCount=37 },
        new Product { Id=7, Name="Colar Choker Pérolas", Description="Choker em ouro 14k com pérolas cultivadas japonesas de 7-8mm. 40cm. Fecho lagosta. Elegância pura.", Price=950, ImageUrl="/images/products/necklace2.jpg", Category="Colares", Material="Ouro 14k + Pérolas", Stock=7, IsFeatured=false, IsNew=true, Rating=4.5, ReviewCount=19 },
        new Product { Id=8, Name="Brinco Gota Rubi", Description="Brincos gota em ouro rosé 18k com rubi oval central e halo de diamantes. Fecho bailarina. Comprimento 3cm.", Price=3600, OriginalPrice=4100, ImageUrl="/images/products/earring2.jpg", Category="Brincos", Material="Ouro Rosé 18k + Rubi", Stock=6, IsFeatured=false, IsNew=false, Rating=4.8, ReviewCount=27 },
        new Product { Id=9, Name="Pulseira Bracelete Tennis", Description="Bracelete tennis em prata 925 com zircônias de alta qualidade. 18cm, 5mm de largura. Fecho dobrável com trava.", Price=380, ImageUrl="/images/products/bracelet2.jpg", Category="Pulseiras", Material="Prata 925 + Zircônia", Stock=20, IsFeatured=false, IsNew=false, Rating=4.4, ReviewCount=63 },
        new Product { Id=10, Name="Anel Infinity Diamantes", Description="Anel infinity em ouro branco 18k com diamantes brilhantes de 0.3ct no total. Símbolo do amor eterno.", Price=3200, ImageUrl="/images/products/ring3.jpg", Category="Anéis", Material="Ouro Branco 18k + Diamantes", Stock=9, IsFeatured=false, IsNew=true, Rating=4.7, ReviewCount=41 },
    };

    private static int _nextId = 11;

    public List<Product> GetAll() => _products.OrderByDescending(p => p.CreatedAt).ToList();

    public List<Product> GetFeatured() => _products.Where(p => p.IsFeatured).ToList();

    public List<Product> GetByCategory(string category) =>
        _products.Where(p => p.Category.Equals(category, StringComparison.OrdinalIgnoreCase)).ToList();

    public List<Product> Search(string query) =>
        _products.Where(p =>
            p.Name.Contains(query, StringComparison.OrdinalIgnoreCase) ||
            p.Description.Contains(query, StringComparison.OrdinalIgnoreCase) ||
            p.Category.Contains(query, StringComparison.OrdinalIgnoreCase) ||
            p.Material.Contains(query, StringComparison.OrdinalIgnoreCase)
        ).ToList();

    public Product? GetById(int id) => _products.FirstOrDefault(p => p.Id == id);

    public void Add(Product product)
    {
        product.Id = _nextId++;
        product.CreatedAt = DateTime.UtcNow;
        _products.Add(product);
    }

    public void Update(Product product)
    {
        var index = _products.FindIndex(p => p.Id == product.Id);
        if (index >= 0) _products[index] = product;
    }

    public void Delete(int id) => _products.RemoveAll(p => p.Id == id);

    public List<string> GetCategories() =>
        _products.Select(p => p.Category).Distinct().OrderBy(c => c).ToList();
}

public interface IOrderService
{
    List<Order> GetAll();
    Order? GetById(int id);
    void Add(Order order);
    void UpdateStatus(int id, OrderStatus status);
    DashboardStats GetDashboardStats();
}

public class OrderService : IOrderService
{
    private static List<Order> _orders = new()
    {
        new Order { Id=1, CustomerName="Ana Silva", CustomerEmail="ana@email.com", CustomerPhone="(11)99999-0001", ShippingAddress="Rua das Flores, 100 - São Paulo, SP", Items=new(){new OrderItem{ProductId=1,ProductName="Anel Solitário Diamante",Price=4800,Quantity=1}}, Total=4800, Status=OrderStatus.Delivered, CreatedAt=DateTime.UtcNow.AddDays(-15) },
        new Order { Id=2, CustomerName="Carlos Mendes", CustomerEmail="carlos@email.com", CustomerPhone="(21)98888-0002", ShippingAddress="Av. Atlântica, 200 - Rio de Janeiro, RJ", Items=new(){new OrderItem{ProductId=2,ProductName="Colar Veneziana Ouro",Price=1200,Quantity=1}, new OrderItem{ProductId=6,ProductName="Pingente Cruz Diamantes",Price=1800,Quantity=1}}, Total=3000, Status=OrderStatus.Shipped, CreatedAt=DateTime.UtcNow.AddDays(-5) },
        new Order { Id=3, CustomerName="Mariana Costa", CustomerEmail="mariana@email.com", CustomerPhone="(31)97777-0003", ShippingAddress="Rua do Ouro, 50 - Belo Horizonte, MG", Items=new(){new OrderItem{ProductId=3,ProductName="Brinco Argola Diamantada",Price=2900,Quantity=1}}, Total=2900, Status=OrderStatus.Confirmed, CreatedAt=DateTime.UtcNow.AddDays(-2) },
        new Order { Id=4, CustomerName="Pedro Alves", CustomerEmail="pedro@email.com", CustomerPhone="(41)96666-0004", ShippingAddress="Rua XV de Novembro, 300 - Curitiba, PR", Items=new(){new OrderItem{ProductId=4,ProductName="Pulseira Riviera Safira",Price=8500,Quantity=1}}, Total=8500, Status=OrderStatus.Pending, CreatedAt=DateTime.UtcNow.AddDays(-1) },
        new Order { Id=5, CustomerName="Fernanda Lima", CustomerEmail="fernanda@email.com", CustomerPhone="(51)95555-0005", ShippingAddress="Av. Ipiranga, 400 - Porto Alegre, RS", Items=new(){new OrderItem{ProductId=7,ProductName="Colar Choker Pérolas",Price=950,Quantity=2}}, Total=1900, Status=OrderStatus.Pending, CreatedAt=DateTime.UtcNow.AddHours(-3) },
    };

    private static int _nextId = 6;

    public List<Order> GetAll() => _orders.OrderByDescending(o => o.CreatedAt).ToList();
    public Order? GetById(int id) => _orders.FirstOrDefault(o => o.Id == id);

    public void Add(Order order)
    {
        order.Id = _nextId++;
        order.CreatedAt = DateTime.UtcNow;
        _orders.Add(order);
    }

    public void UpdateStatus(int id, OrderStatus status)
    {
        var order = _orders.FirstOrDefault(o => o.Id == id);
        if (order != null) order.Status = status;
    }

    public DashboardStats GetDashboardStats() => new()
    {
        TotalRevenue = _orders.Where(o => o.Status != OrderStatus.Cancelled).Sum(o => o.Total),
        TotalOrders = _orders.Count,
        PendingOrders = _orders.Count(o => o.Status == OrderStatus.Pending),
        TotalProducts = 10,
        MonthlySales = new List<MonthlySale>
        {
            new() { Month = "Jan", Revenue = 12400, Orders = 8 },
            new() { Month = "Fev", Revenue = 18200, Orders = 12 },
            new() { Month = "Mar", Revenue = 15600, Orders = 10 },
            new() { Month = "Abr", Revenue = 22800, Orders = 15 },
            new() { Month = "Mai", Revenue = 19400, Orders = 13 },
            new() { Month = "Jun", Revenue = 27600, Orders = 18 },
        }
    };
}

public interface ICartService
{
    List<CartItem> GetCart(HttpContext context);
    void AddToCart(HttpContext context, Product product, int quantity = 1);
    void RemoveFromCart(HttpContext context, int productId);
    void UpdateQuantity(HttpContext context, int productId, int quantity);
    void ClearCart(HttpContext context);
    decimal GetTotal(HttpContext context);
    int GetItemCount(HttpContext context);
}

public class CartService : ICartService
{
    private const string CartKey = "cart";

    public List<CartItem> GetCart(HttpContext context)
    {
        var cart = context.Session.GetString(CartKey);
        if (string.IsNullOrEmpty(cart)) return new();
        return System.Text.Json.JsonSerializer.Deserialize<List<CartItem>>(cart) ?? new();
    }

    private void SaveCart(HttpContext context, List<CartItem> cart)
    {
        context.Session.SetString(CartKey, System.Text.Json.JsonSerializer.Serialize(cart));
    }

    public void AddToCart(HttpContext context, Product product, int quantity = 1)
    {
        var cart = GetCart(context);
        var existing = cart.FirstOrDefault(i => i.ProductId == product.Id);
        if (existing != null) existing.Quantity += quantity;
        else cart.Add(new CartItem { ProductId = product.Id, ProductName = product.Name, Price = product.Price, ImageUrl = product.ImageUrl, Quantity = quantity });
        SaveCart(context, cart);
    }

    public void RemoveFromCart(HttpContext context, int productId)
    {
        var cart = GetCart(context);
        cart.RemoveAll(i => i.ProductId == productId);
        SaveCart(context, cart);
    }

    public void UpdateQuantity(HttpContext context, int productId, int quantity)
    {
        var cart = GetCart(context);
        var item = cart.FirstOrDefault(i => i.ProductId == productId);
        if (item != null)
        {
            if (quantity <= 0) cart.Remove(item);
            else item.Quantity = quantity;
        }
        SaveCart(context, cart);
    }

    public void ClearCart(HttpContext context) => context.Session.Remove(CartKey);

    public decimal GetTotal(HttpContext context) => GetCart(context).Sum(i => i.Subtotal);

    public int GetItemCount(HttpContext context) => GetCart(context).Sum(i => i.Quantity);
}
