namespace JewelryEcommerce.Controllers;

public class AdminLoginModel
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string ErrorMessage { get; set; } = string.Empty;
}
