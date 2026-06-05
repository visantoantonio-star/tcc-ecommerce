using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using JewelryEcommerce.Services;
using JewelryEcommerce.Domain;

namespace JewelryEcommerce.Controllers;

public class AdminController : Controller
{
    private const string AdminUsername = "admin";
    private const string AdminPassword = "admin123";
    private const string AdminSessionKey = "AdminUser";

    private readonly IProductService _products;
    private readonly IOrderService _orders;
    private readonly IWebHostEnvironment _environment;

    public AdminController(IProductService products, IOrderService orders, IWebHostEnvironment environment)
    {
        _products = products;
        _orders = orders;
        _environment = environment;
    }

    private bool IsLoggedIn() => HttpContext.Session.GetString(AdminSessionKey) != null;

    private IActionResult? RequireAdmin()
    {
        if (!IsLoggedIn()) return RedirectToAction("Login");
        return null;
    }

    public IActionResult Login()
    {
        if (IsLoggedIn()) return RedirectToAction("Index");
        return View(new AdminLoginModel());
    }

    [HttpPost]
    public IActionResult Login(AdminLoginModel model)
    {
        if (model == null) return View(new AdminLoginModel { ErrorMessage = "Dados inválidos." });

        if (model.Username == AdminUsername && model.Password == AdminPassword)
        {
            HttpContext.Session.SetString(AdminSessionKey, model.Username);
            return RedirectToAction("Index");
        }

        model.ErrorMessage = "Usuário ou senha incorretos.";
        return View(model);
    }

    public IActionResult Logout()
    {
        HttpContext.Session.Remove(AdminSessionKey);
        return RedirectToAction("Login");
    }

    public IActionResult Index()
    {
        var authResult = RequireAdmin();
        if (authResult != null) return authResult;

        ViewBag.Stats = _orders.GetDashboardStats();
        ViewBag.RecentOrders = _orders.GetAll().Take(5).ToList();
        return View();
    }

    // Products
    public IActionResult Products()
    {
        var authResult = RequireAdmin();
        if (authResult != null) return authResult;
        return View(_products.GetAll());
    }

    public IActionResult CreateProduct()
    {
        var authResult = RequireAdmin();
        if (authResult != null) return authResult;
        return View(new Product());
    }

    [HttpPost]
    [HttpPost]
    public IActionResult CreateProduct(Product product, IFormFile? ImageFile)
    {
        var authResult = RequireAdmin();
        if (authResult != null) return authResult;

        if (ImageFile != null && ImageFile.Length > 0)
        {
            product.ImageUrl = SaveImageFile(ImageFile);
        }

        product.Rating = Math.Min(5m, Math.Max(0m, product.Rating));
        if (product.Price < 0) product.Price = 0;
        if (product.OriginalPrice.HasValue && product.OriginalPrice < 0) product.OriginalPrice = 0;

        _products.Add(product);
        TempData["Success"] = "Produto criado com sucesso!";
        return RedirectToAction("Products");
    }

    public IActionResult EditProduct(int id)
    {
        var authResult = RequireAdmin();
        if (authResult != null) return authResult;

        var product = _products.GetById(id);
        if (product == null) return NotFound();
        return View(product);
    }

    [HttpPost]
    public IActionResult EditProduct(Product product, IFormFile? ImageFile)
    {
        var authResult = RequireAdmin();
        if (authResult != null) return authResult;

        if (ImageFile != null && ImageFile.Length > 0)
        {
            product.ImageUrl = SaveImageFile(ImageFile);
        }

        product.Rating = Math.Min(5m, Math.Max(0m, product.Rating));
        if (product.Price < 0) product.Price = 0;
        if (product.OriginalPrice.HasValue && product.OriginalPrice < 0) product.OriginalPrice = 0;

        _products.Update(product);
        TempData["Success"] = "Produto atualizado com sucesso!";
        return RedirectToAction("Products");
    }

    [HttpPost]
    public IActionResult DeleteProduct(int id)
    {
        var authResult = RequireAdmin();
        if (authResult != null) return authResult;

        try
        {
            _products.Delete(id);
            TempData["Success"] = "Produto removido.";
        }
        catch (InvalidOperationException ex)
        {
            TempData["Error"] = ex.Message;
        }

        return RedirectToAction("Products");
    }

    // Orders
    public IActionResult Orders(string? status)
    {
        var authResult = RequireAdmin();
        if (authResult != null) return authResult;

        var all = _orders.GetAll();
        if (!string.IsNullOrEmpty(status) && Enum.TryParse<OrderStatus>(status, out var s))
            all = all.Where(o => o.Status == s).ToList();
        ViewBag.CurrentStatus = status;
        return View(all);
    }

    public IActionResult OrderDetails(int id)
    {
        var authResult = RequireAdmin();
        if (authResult != null) return authResult;

        var order = _orders.GetById(id);
        if (order == null) return NotFound();
        return View(order);
    }

    [HttpPost]
    public IActionResult UpdateOrderStatus(int id, string status)
    {
        var authResult = RequireAdmin();
        if (authResult != null) return authResult;

        if (Enum.TryParse<OrderStatus>(status, out var s))
            _orders.UpdateStatus(id, s);
        TempData["Success"] = "Status do pedido atualizado!";
        return RedirectToAction("OrderDetails", new { id });
    }

    private string SaveImageFile(IFormFile imageFile)
    {
        var uploadsRoot = Path.Combine(_environment.WebRootPath, "images", "products");
        if (!Directory.Exists(uploadsRoot))
            Directory.CreateDirectory(uploadsRoot);

        var fileName = Path.GetRandomFileName() + Path.GetExtension(imageFile.FileName);
        var filePath = Path.Combine(uploadsRoot, fileName);

        using var stream = System.IO.File.Create(filePath);
        imageFile.CopyTo(stream);

        return $"/images/products/{fileName}";
    }
}
