using System.ComponentModel.DataAnnotations;

namespace PortfolioApp.ViewModels;

public class LoginViewModel
{
    [Required(ErrorMessage = "Password is required.")]
    public string Password { get; set; } = string.Empty;
}
