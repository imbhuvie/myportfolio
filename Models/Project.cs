using System.ComponentModel.DataAnnotations;

namespace PortfolioApp.Models;

public class Project
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Title is required.")]
    [StringLength(200)]
    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    public string? Problem { get; set; }

    public string? Solution { get; set; }

    public string? Features { get; set; }

    public string? TechStack { get; set; }

    public string? Url { get; set; }

    public string? ImageUrl { get; set; }

    public int DisplayOrder { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }
}
