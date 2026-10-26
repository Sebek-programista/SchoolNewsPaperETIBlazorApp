
using System.ComponentModel.DataAnnotations;

namespace SchoolNewspaperBlazorApp.Data;

public class Substitution
{
    public int Id { get; set; }

    [Required]
    public DateTime Date { get; set; } = DateTime.Today;

    [Range(1, 20)]
    public int LessonNumber { get; set; } = 1;

    public int SchoolClassId { get; set; }
    public SchoolClass SchoolClass { get; set; } = null!;

    public int? AbsentTeacherId { get; set; }
    public Teacher? AbsentTeacher { get; set; }

    public int? SubstituteTeacherId { get; set; }
    public Teacher? SubstituteTeacher { get; set; }

    public int? OriginalSubjectId { get; set; }
    public SchoolSubject? OriginalSubject { get; set; }

    public int? SubstituteSubjectId { get; set; }
    public SchoolSubject? SubstituteSubject { get; set; }

    public int? OriginalRoomId { get; set; }
    public SchoolRoom? OriginalRoom { get; set; }

    public int? SubstituteRoomId { get; set; }
    public SchoolRoom? SubstituteRoom { get; set; }

    [MaxLength(1000)]
    public string? Notes { get; set; }

    public bool IsPublished { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.Now;
}
