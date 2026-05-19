using Microsoft.AspNetCore.Mvc;
using JewelryEcommerce.Services;
using JewelryEcommerce.Domain;

namespace JewelryEcommerce.Controllers;

public class CartController : Controller
{
    private readonly ICartService _cart;
    private readonly IProductService _products;
    private readonly IOrderService _orders;

    public CartController(ICartService cart, IProductService products, IOrderService orders)
    {
        _cart = cart;
        _products = products;
        _orders = orders;
    }

    public IActionResult Index()
    {
        ViewBag.CartCount = _cart.GetItemCount(HttpContext);
        var items = _cart.GetCart(HttpContext);
        ViewBag.Total = _cart.GetTotal(HttpContext);
        return View(items);
    }

    [HttpPost]
    public IActionResult Add(int productId, int quantity = 1)
    {
        var product = _products.GetById(productId);
        if (product != null) _cart.AddToCart(HttpContext, product, quantity);
        return RedirectToAction("Index");
    }

    [HttpPost]
    public IActionResult Remove(int productId)
    {
        _cart.RemoveFromCart(HttpContext, productId);
        return RedirectToAction("Index");
    }

    [HttpPost]
    public IActionResult Update(int productId, int quantity)
    {
        _cart.UpdateQuantity(HttpContext, productId, quantity);
        return RedirectToAction("Index");
    }

    public IActionResult Checkout()
    {
        var items = _cart.GetCart(HttpContext);
        if (!items.Any()) return RedirectToAction("Index");
        ViewBag.Total = _cart.GetTotal(HttpContext);
        ViewBag.CartCount = _cart.GetItemCount(HttpContext);
        return View(items);
    }

    [HttpPost]
    public IActionResult PlaceOrder(string customerName, string customerEmail, string customerPhone, string shippingAddress)
    {
        var items = _cart.GetCart(HttpContext);
        if (!items.Any()) return RedirectToAction("Index");

        var order = new Order
        {
            CustomerName = customerName,
            CustomerEmail = customerEmail,
            CustomerPhone = customerPhone,
            ShippingAddress = shippingAddress,
            Items = items.Select(i => new OrderItem
            {
                ProductId = i.ProductId,
                ProductName = i.ProductName,
                Price = i.Price,
                Quantity = i.Quantity
            }).ToList(),
            Total = _cart.GetTotal(HttpContext),
            Status = OrderStatus.Pending
        };

        _orders.Add(order);
        _cart.ClearCart(HttpContext);

        TempData["OrderId"] = order.Id;
        TempData["OrderTotal"] = order.Total.ToString("F2");
        return RedirectToAction("Confirmation");
    }

    public IActionResult Confirmation()
    {
        ViewBag.CartCount = 0;
        return View();
    }
}
