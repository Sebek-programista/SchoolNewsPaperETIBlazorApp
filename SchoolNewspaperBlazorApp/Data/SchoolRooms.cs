
using System.ComponentModel.DataAnnotations;

namespace SchoolNewspaperBlazorApp.Data;

public class SchoolRoom
{
    public int Id { get; set; }

    [Required]
    [MaxLength(30)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(100)]
    public string? Description { get; set; }

    public bool IsActive { get; set; } = true;
}
