
using System.ComponentModel.DataAnnotations;

namespace SchoolNewspaperBlazorApp.Data;

public class Teacher
{
    public int Id { get; set; }

    [Required]
    [MaxLength(80)]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string LastName { get; set; } = string.Empty;

    [MaxLength(150)]
    public string? Email { get; set; }

    public bool IsActive { get; set; } = true;

    public string FullName => $"{FirstName} {LastName}";
}
