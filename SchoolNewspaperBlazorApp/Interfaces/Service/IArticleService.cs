
using SchoolNewspaperBlazorApp.Data;
using SchoolNewspaperBlazorApp.Data.Enums;

namespace SchoolNewspaperBlazorApp.Interfaces.Service
{
    public interface IArticleService
    {
        Task<int> AddArticleAsync(
            string title,
            string text,
            string author,
            int fileId,
            EnumArticleCategory category,
            DateTime? announcementDeadline = null,
            DateTime? substituteDate = null,
            string? schoolClass = null,
            string? teacher = null,
            DateTime? eventDate = null,
            TimeOnly? eventTime = null,
            string? eventLocation = null,
            DateTime? competitionDate = null,
            string? competitionDiscipline = null,
            string? competitionLocation = null,
            DateTime? admissionsStart = null,
            DateTime? admissionsEnd = null,
            DateTime? examDate = null,
            string? examSubject = null,
            string? examClass = null,
            string? councilInitiative = null);

        Task<List<Article>> GetAllArticlesAsync();

        Task<Article> GetArticleByIdAsync(int id);

        Task<bool> RemoveArticleByIdAsync(int id);

        Task UpdateArticleByIdAsync(
            int id,
            string title,
            string text,
            string author,
            int fileId,
            EnumArticleCategory category,
            DateTime? announcementDeadline = null,
            DateTime? substituteDate = null,
            string? schoolClass = null,
            string? teacher = null,
            DateTime? eventDate = null,
            TimeOnly? eventTime = null,
            string? eventLocation = null,
            DateTime? competitionDate = null,
            string? competitionDiscipline = null,
            string? competitionLocation = null,
            DateTime? admissionsStart = null,
            DateTime? admissionsEnd = null,
            DateTime? examDate = null,
            string? examSubject = null,
            string? examClass = null,
            string? councilInitiative = null);
    }
}
