using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PortfolioApp.Data;

namespace PortfolioApp.Controllers;

[Authorize]
public class ContactMessagesController : Controller
{
    private readonly AppDbContext _db;

    public ContactMessagesController(AppDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var messages = await _db.ContactMessages
            .OrderByDescending(m => m.CreatedAt)
            .ToListAsync();

        return View(messages);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> MarkRead(int id)
    {
        var message = await _db.ContactMessages.FindAsync(id);
        if (message is null)
            return NotFound();

        message.IsRead = true;
        await _db.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var message = await _db.ContactMessages.FindAsync(id);
        if (message is null)
            return NotFound();

        _db.ContactMessages.Remove(message);
        await _db.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }
}
