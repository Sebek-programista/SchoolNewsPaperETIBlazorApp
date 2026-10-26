
using System.ComponentModel.DataAnnotations;

namespace SchoolNewspaperBlazorApp.Data;

public class SchoolClass
{
    public int Id { get; set; }

    [Required]
    [MaxLength(30)]
    public string Name { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;
}
