using Microsoft.AspNetCore.Mvc;
using PortfolioApp.Data;
using PortfolioApp.Models;
using PortfolioApp.ViewModels;
using System.Net;
using System.Net.Mail;

namespace PortfolioApp.Controllers;

public class ContactController : Controller
{
    private readonly AppDbContext _db;
    private readonly IConfiguration _config;
    private readonly ILogger<ContactController> _logger;

    public ContactController(AppDbContext db, IConfiguration config, ILogger<ContactController> logger)
    {
        _db = db;
        _config = config;
        _logger = logger;
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Send(ContactFormViewModel model)
    {
        if (!ModelState.IsValid)
            return Json(new { success = false, message = "Please fill in all the fields correctly." });

        if (!string.IsNullOrWhiteSpace(model.Honeypot))
            return Json(new { success = true });

        var message = new ContactMessage
        {
            Name = model.Name.Trim(),
            Email = model.Email.Trim(),
            Message = model.Message.Trim(),
            CreatedAt = DateTime.UtcNow
        };

        _db.ContactMessages.Add(message);
        await _db.SaveChangesAsync();

        await TrySendEmailAsync(message);

        return Json(new
        {
            success = true,
            message = "Project brief received — I'll get back to you within 24 hours."
        });
    }

    private async Task TrySendEmailAsync(ContactMessage message)
    {
        try
        {
            var host = _config["Smtp:Host"];
            var toEmail = _config["Smtp:ToEmail"];

            if (string.IsNullOrWhiteSpace(host) || string.IsNullOrWhiteSpace(toEmail))
            {
                _logger.LogInformation("SMTP not configured; contact message #{Id} saved to database only.", message.Id);
                return;
            }

            var port = int.TryParse(_config["Smtp:Port"], out var p) ? p : 587;
            var username = _config["Smtp:Username"];
            var password = _config["Smtp:Password"];
            var fromEmail = _config["Smtp:FromEmail"] ?? toEmail;
            var enableSsl = !bool.TryParse(_config["Smtp:EnableSsl"], out var ssl) || ssl;

            using var client = new SmtpClient(host, port)
            {
                EnableSsl = enableSsl,
                Credentials = string.IsNullOrWhiteSpace(username)
                    ? CredentialCache.DefaultNetworkCredentials
                    : new NetworkCredential(username, password)
            };

            using var mail = new MailMessage
            {
                From = new MailAddress(fromEmail, "PortfolioApp"),
                Subject = $"New project inquiry from {message.Name} ({message.Email})",
                Body = $"Name: {message.Name}\nEmail: {message.Email}\n\n{message.Message}",
                IsBodyHtml = false
            };
            mail.To.Add(toEmail);

            await client.SendMailAsync(mail);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to send contact email for message #{Id}.", message.Id);
        }
    }
}
