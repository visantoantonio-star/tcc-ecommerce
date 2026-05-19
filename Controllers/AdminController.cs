using Microsoft.AspNetCore.Mvc;
using JewelryEcommerce.Services;
using JewelryEcommerce.Domain;

namespace JewelryEcommerce.Controllers;

public class AdminController : Controller
{
    private readonly IProductService _products;
    private readonly IOrderService _orders;

    public AdminController(IProductService products, IOrderService orders)
    {
        _products = products;
        _orders = orders;
    }

    public IActionResult Index()
    {
        ViewBag.Stats = _orders.GetDashboardStats();
        ViewBag.RecentOrders = _orders.GetAll().Take(5).ToList();
        return View();
    }

    // Products
    public IActionResult Products() => View(_products.GetAll());

    public IActionResult CreateProduct() => View(new Product());

    [HttpPost]
    public IActionResult CreateProduct(Product product)
    {
        _products.Add(product);
        TempData["Success"] = "Produto criado com sucesso!";
        return RedirectToAction("Products");
    }

    public IActionResult EditProduct(int id)
    {
        var product = _products.GetById(id);
        if (product == null) return NotFound();
        return View(product);
    }

    [HttpPost]
    public IActionResult EditProduct(Product product)
    {
        _products.Update(product);
        TempData["Success"] = "Produto atualizado com sucesso!";
        return RedirectToAction("Products");
    }

    [HttpPost]
    public IActionResult DeleteProduct(int id)
    {
        _products.Delete(id);
        TempData["Success"] = "Produto removido.";
        return RedirectToAction("Products");
    }

    // Orders
    public IActionResult Orders(string? status)
    {
        var all = _orders.GetAll();
        if (!string.IsNullOrEmpty(status) && Enum.TryParse<OrderStatus>(status, out var s))
            all = all.Where(o => o.Status == s).ToList();
        ViewBag.CurrentStatus = status;
        return View(all);
    }

    public IActionResult OrderDetails(int id)
    {
        var order = _orders.GetById(id);
        if (order == null) return NotFound();
        return View(order);
    }

    [HttpPost]
    public IActionResult UpdateOrderStatus(int id, string status)
    {
        if (Enum.TryParse<OrderStatus>(status, out var s))
            _orders.UpdateStatus(id, s);
        TempData["Success"] = "Status do pedido atualizado!";
        return RedirectToAction("OrderDetails", new { id });
    }
}
