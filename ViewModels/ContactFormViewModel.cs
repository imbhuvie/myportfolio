using System.ComponentModel.DataAnnotations;

namespace PortfolioApp.ViewModels;

public class ContactFormViewModel
{
    [Required(ErrorMessage = "Please enter your name.")]
    [StringLength(200)]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Please enter your email.")]
    [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
    [StringLength(200)]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Please tell me a little about your project.")]
    public string Message { get; set; } = string.Empty;

    public string? Honeypot { get; set; }
}
