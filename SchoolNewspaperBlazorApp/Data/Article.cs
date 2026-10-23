
using SchoolNewspaperBlazorApp.Data.Enums;

namespace SchoolNewspaperBlazorApp.Data
{
    public class Article
    {
        public int Id { get; set; }

        public required string Title { get; set; }

        public required string Text { get; set; }

        public required string Author { get; set; }

        public DateTime PublishDate { get; set; }

        public int MainImageId { get; set; }

        public EnumArticleCategory Category { get; set; }

        // Szczegóły ogłoszenia
        public DateTime? AnnouncementDeadline { get; set; }

        // Szczegóły zastępstwa
        public DateTime? SubstituteDate { get; set; }
        public string? SchoolClass { get; set; }
        public string? Teacher { get; set; }

        // Szczegóły wydarzenia
        public DateTime? EventDate { get; set; }
        public TimeOnly? EventTime { get; set; }
        public string? EventLocation { get; set; }

        // Szczegóły zawodów
        public DateTime? CompetitionDate { get; set; }
        public string? CompetitionDiscipline { get; set; }
        public string? CompetitionLocation { get; set; }

        // Szczegóły rekrutacji
        public DateTime? AdmissionsStart { get; set; }
        public DateTime? AdmissionsEnd { get; set; }

        // Szczegóły egzaminu
        public DateTime? ExamDate { get; set; }
        public string? ExamSubject { get; set; }
        public string? ExamClass { get; set; }

        // Szczegóły samorządu uczniowskiego
        public string? CouncilInitiative { get; set; }
    }
}
