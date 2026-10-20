using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PortfolioApp.Data;
using PortfolioApp.Models;

namespace PortfolioApp.Controllers;

[Authorize]
//[Route("Admin/Projects")]
public class ProjectsController : Controller
{
    private readonly AppDbContext _db;

    public ProjectsController(AppDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var projects = await _db.Projects
            .OrderBy(p => p.DisplayOrder)
            .ThenBy(p => p.Id)
            .ToListAsync();

        return View(projects);
    }

    [HttpGet]
    public IActionResult Create()
    {
        ViewBag.FormAction = nameof(Create);
        return View(new Project());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Project project)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.FormAction = nameof(Create);
            return View(project);
        }

        project.CreatedAt = DateTime.UtcNow;
        _db.Projects.Add(project);
        await _db.SaveChangesAsync();

        TempData["Message"] = $"Project \"{project.Title}\" created.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var project = await _db.Projects.FindAsync(id);
        if (project is null)
            return NotFound();

        ViewBag.FormAction = nameof(Edit);
        return View(project);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Project project)
    {
        if (id != project.Id)
            return BadRequest();

        var existing = await _db.Projects.FindAsync(id);
        if (existing is null)
            return NotFound();

        if (!ModelState.IsValid)
        {
            ViewBag.FormAction = nameof(Edit);
            return View(project);
        }

        existing.Title = project.Title;
        existing.Description = project.Description;
        existing.Problem = project.Problem;
        existing.Solution = project.Solution;
        existing.Features = project.Features;
        existing.TechStack = project.TechStack;
        existing.Url = project.Url;
        existing.ImageUrl = project.ImageUrl;
        existing.DisplayOrder = project.DisplayOrder;
        existing.IsActive = project.IsActive;
        existing.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();

        TempData["Message"] = $"Project \"{existing.Title}\" updated.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var project = await _db.Projects.FindAsync(id);
        if (project is null)
            return NotFound();

        return View(project);
    }

    [HttpPost]
    [ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var project = await _db.Projects.FindAsync(id);
        if (project is null)
            return NotFound();

        _db.Projects.Remove(project);
        await _db.SaveChangesAsync();

        TempData["Message"] = $"Project \"{project.Title}\" deleted.";
        return RedirectToAction(nameof(Index));
    }
}
