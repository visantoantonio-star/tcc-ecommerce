using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
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

    public AdminController(IProductService products, IOrderService orders)
    {
        _products = products;
        _orders = orders;
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
    public IActionResult CreateProduct(Product product)
    {
        var authResult = RequireAdmin();
        if (authResult != null) return authResult;

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
    public IActionResult EditProduct(Product product)
    {
        var authResult = RequireAdmin();
        if (authResult != null) return authResult;

        _products.Update(product);
        TempData["Success"] = "Produto atualizado com sucesso!";
        return RedirectToAction("Products");
    }

    [HttpPost]
    public IActionResult DeleteProduct(int id)
    {
        var authResult = RequireAdmin();
        if (authResult != null) return authResult;

        _products.Delete(id);
        TempData["Success"] = "Produto removido.";
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
}
