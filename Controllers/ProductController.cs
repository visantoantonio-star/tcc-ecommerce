using Microsoft.AspNetCore.Mvc;
using JewelryEcommerce.Services;

namespace JewelryEcommerce.Controllers;

public class ProductController : Controller
{
    private readonly IProductService _products;
    private readonly ICartService _cart;

    public ProductController(IProductService products, ICartService cart)
    {
        _products = products;
        _cart = cart;
    }

    public IActionResult Index(string? category, string? search)
    {
        var list = string.IsNullOrEmpty(search)
            ? (string.IsNullOrEmpty(category) ? _products.GetAll() : _products.GetByCategory(category))
            : _products.Search(search);

        ViewBag.Categories = _products.GetCategories();
        ViewBag.CurrentCategory = category;
        ViewBag.SearchQuery = search;
        ViewBag.CartCount = _cart.GetItemCount(HttpContext);
        return View(list);
    }

    public IActionResult Details(int id)
    {
        var product = _products.GetById(id);
        if (product == null) return NotFound();
        ViewBag.Related = _products.GetByCategory(product.Category).Where(p => p.Id != id).Take(4).ToList();
        ViewBag.CartCount = _cart.GetItemCount(HttpContext);
        return View(product);
    }
}
