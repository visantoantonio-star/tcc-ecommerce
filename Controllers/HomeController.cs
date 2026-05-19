using Microsoft.AspNetCore.Mvc;
using JewelryEcommerce.Services;
using JewelryEcommerce.Domain;

namespace JewelryEcommerce.Controllers;

public class HomeController : Controller
{
    private readonly IProductService _products;
    private readonly ICartService _cart;

    public HomeController(IProductService products, ICartService cart)
    {
        _products = products;
        _cart = cart;
    }

    public IActionResult Index()
    {
        ViewBag.Featured = _products.GetFeatured();
        ViewBag.Categories = _products.GetCategories();
        ViewBag.CartCount = _cart.GetItemCount(HttpContext);
        return View();
    }

    public IActionResult Error() => View();
}
