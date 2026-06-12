using JewelryEcommerce.Data;
using JewelryEcommerce.Domain;
using Microsoft.EntityFrameworkCore;

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
    private readonly AppDBContext _db;

    public ProductService(AppDBContext db) => _db = db;

    public List<Product> GetAll() =>
        _db.Products.OrderByDescending(p => p.CreatedAt).ToList();

    public List<Product> GetFeatured() =>
        _db.Products.Where(p => p.IsFeatured).ToList();

    public List<Product> GetByCategory(string category) =>
        _db.Products.Where(p => p.Category != null && p.Category.ToLower() == category.ToLower()).ToList();

    public List<Product> Search(string query) =>
        _db.Products.Where(p =>
            p.Name.Contains(query) ||
            p.Description.Contains(query) ||
            (p.Category != null && p.Category.Contains(query)) ||
            (p.Material != null && p.Material.Contains(query))
        ).ToList();

    public Product? GetById(int id) =>
        _db.Products.FirstOrDefault(p => p.Id == id);

    public void Add(Product product)
    {
        product.CreatedAt = DateTime.UtcNow;
        _db.Products.Add(product);
        try
        {
            _db.SaveChanges();
        }
        catch (Microsoft.EntityFrameworkCore.DbUpdateException)
        {
            Console.WriteLine($"Failed saving Product: Name={product.Name}, Price={product.Price}, OriginalPrice={product.OriginalPrice}, Rating={product.Rating}, Stock={product.Stock}, ReviewCount={product.ReviewCount}");
            throw;
        }
    }

    public void Update(Product product)
    {
        _db.Products.Update(product);
        _db.SaveChanges();
    }

    public void Delete(int id)
    {
        var product = _db.Products.Find(id);
        if (product == null)
            return;

        // Prevent deletion if the product is referenced by active orders.
        // Delivered or canceled orders are allowed because the order history
        // keeps product name/price independently of the Product entity.
        var hasActiveOrderItems = _db.OrderItems
            .Where(oi => oi.ProductId == id)
            .Join(_db.Orders,
                oi => oi.OrderId,
                o => o.Id,
                (oi, o) => o.Status)
            .Any(status => status != OrderStatus.Delivered && status != OrderStatus.Cancelled);

        if (hasActiveOrderItems)
            throw new InvalidOperationException("Não é possível excluir o produto porque ele está vinculado a pedidos ativos.");

        _db.Products.Remove(product);
        _db.SaveChanges();
    }

    public List<string> GetCategories() =>
        _db.Products.Where(p => p.Category != null)
                    .Select(p => p.Category!)
                    .Distinct()
                    .OrderBy(c => c)
                    .ToList();
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
    private readonly AppDBContext _db;

    public OrderService(AppDBContext db) => _db = db;

    public List<Order> GetAll() =>
        _db.Orders
           .Include(o => o.Items)
           .OrderByDescending(o => o.CreatedAt)
           .ToList();

    public Order? GetById(int id) =>
        _db.Orders
           .Include(o => o.Items)
           .FirstOrDefault(o => o.Id == id);

    public void Add(Order order)
    {
        order.CreatedAt = DateTime.UtcNow;
        _db.Orders.Add(order);
        _db.SaveChanges();
    }

    public void UpdateStatus(int id, OrderStatus status)
    {
        var order = _db.Orders.Find(id);
        if (order != null)
        {
            order.Status = status;
            _db.SaveChanges();
        }
    }

    public DashboardStats GetDashboardStats()
    {
        var orders = _db.Orders.ToList();
        var now = DateTime.UtcNow;

        var monthlySales = Enumerable.Range(0, 6)
            .Select(i => now.AddMonths(-5 + i))
            .Select(date => new MonthlySale
            {
                Month = date.ToString("MMM"),
                Revenue = orders
                    .Where(o => o.Status != OrderStatus.Cancelled &&
                                o.CreatedAt.Year == date.Year &&
                                o.CreatedAt.Month == date.Month)
                    .Sum(o => o.Total),
                Orders = orders
                    .Count(o => o.CreatedAt.Year == date.Year &&
                                o.CreatedAt.Month == date.Month)
            })
            .ToList();

        return new DashboardStats
        {
            TotalRevenue = orders.Where(o => o.Status != OrderStatus.Cancelled).Sum(o => o.Total),
            TotalOrders = orders.Count,
            PendingOrders = orders.Count(o => o.Status == OrderStatus.Pending),
            TotalProducts = _db.Products.Count(),
            MonthlySales = monthlySales
        };
    }
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
