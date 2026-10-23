
using SchoolNewspaperBlazorApp.Data;
using SchoolNewspaperBlazorApp.Data.Enums;
using SchoolNewspaperBlazorApp.Interfaces.Repository;
using SchoolNewspaperBlazorApp.Interfaces.Service;

namespace SchoolNewspaperBlazorApp.Service
{
    public class ArticleService : IArticleService
    {
        private readonly IArticleRepository _articleRepository;
        private readonly IPermissionService _permissionService;
        private readonly INotificationService _notificationService;
        private readonly IAuthService _authService;

        public ArticleService(
            IArticleRepository articleRepository,
            IPermissionService permissionService,
            INotificationService notificationService,
            IAuthService authService)
        {
            _articleRepository = articleRepository;
            _permissionService = permissionService;
            _notificationService = notificationService;
            _authService = authService;
        }

        public async Task<int> AddArticleAsync(
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
            string? councilInitiative = null)
        {
            if (string.IsNullOrWhiteSpace(title))
            {
                _notificationService.Show(
                    "Tytuł artykułu jest wymagany!",
                    EnumNotificationType.Failure);
                return 0;
            }

            if (string.IsNullOrWhiteSpace(text))
            {
                _notificationService.Show(
                    "Treść artykułu jest wymagana!",
                    EnumNotificationType.Failure);
                return 0;
            }

            if (fileId <= 0)
            {
                _notificationService.Show(
                    "Artykuł musi zawierać zdjęcie główne!",
                    EnumNotificationType.Failure);
                return 0;
            }

            if (!Enum.IsDefined(typeof(EnumArticleCategory), category))
            {
                _notificationService.Show(
                    "Wybierz prawidłową kategorię artykułu!",
                    EnumNotificationType.Failure);
                return 0;
            }

            bool hasPermission = await _permissionService
                .HasPermissionAsync(EnumPermissions.AddArticle);

            if (!hasPermission)
            {
                _notificationService.Show(
                    "Brak uprawnień do dodawania artykułów!",
                    EnumNotificationType.Failure);
                return 0;
            }

            // Pobranie ID aktualnie zalogowanego użytkownika.
            int userId = await _permissionService.GetUserId();

            if (userId <= 0)
            {
                _notificationService.Show(
                    "Nie udało się ustalić zalogowanego użytkownika!",
                    EnumNotificationType.Failure);
                return 0;
            }

            // Pobranie danych autora z bazy na podstawie ID.
            var users = await _authService.GetAllUsersAsync();

            if (!users.TryGetValue(userId, out string? email))
            {
                _notificationService.Show(
                    "Nie udało się pobrać danych autora!",
                    EnumNotificationType.Failure);

                return 0;
            }

            string authorName = email;

           ;

            var article = new Article
            {
                Title = title.Trim(),
                Text = text.Trim(),

                // Autor pochodzi z konta zalogowanego użytkownika.
                Author = authorName,

                PublishDate = DateTime.Now,
                MainImageId = fileId,
                Category = category,

                AnnouncementDeadline = announcementDeadline,

                SubstituteDate = substituteDate,
                SchoolClass = schoolClass?.Trim(),
                Teacher = teacher?.Trim(),

                EventDate = eventDate,
                EventTime = eventTime,
                EventLocation = eventLocation?.Trim(),

                CompetitionDate = competitionDate,
                CompetitionDiscipline = competitionDiscipline?.Trim(),
                CompetitionLocation = competitionLocation?.Trim(),

                AdmissionsStart = admissionsStart,
                AdmissionsEnd = admissionsEnd,

                ExamDate = examDate,
                ExamSubject = examSubject?.Trim(),
                ExamClass = examClass?.Trim(),

                CouncilInitiative = councilInitiative?.Trim()
            };

            int articleId = await _articleRepository
                .AddArticleAsync(article);

            if (articleId > 0)
            {
                _notificationService.Show(
                    "Pomyślnie dodano artykuł!",
                    EnumNotificationType.Success);
            }
            else
            {
                _notificationService.Show(
                    "Podczas dodawania artykułu wystąpił błąd!",
                    EnumNotificationType.Error);
            }

            return articleId;
        }

        public async Task UpdateArticleByIdAsync(
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
            string? councilInitiative = null)
        {
            if (id <= 0)
            {
                _notificationService.Show(
                    "Nieprawidłowe ID artykułu!",
                    EnumNotificationType.Failure);
                return;
            }

            if (string.IsNullOrWhiteSpace(title) ||
                string.IsNullOrWhiteSpace(text))
            {
                _notificationService.Show(
                    "Tytuł i treść są wymagane!",
                    EnumNotificationType.Failure);
                return;
            }

            if (!Enum.IsDefined(typeof(EnumArticleCategory), category))
            {
                _notificationService.Show(
                    "Wybierz prawidłową kategorię artykułu!",
                    EnumNotificationType.Failure);
                return;
            }

            bool hasPermission = await _permissionService
                .HasPermissionAsync(EnumPermissions.EditArticle);

            if (!hasPermission)
            {
                _notificationService.Show(
                    "Brak uprawnień do edytowania artykułów!",
                    EnumNotificationType.Failure);
                return;
            }

            Article existingArticle;

            try
            {
                existingArticle = await _articleRepository
                    .GetArticleByIdAsync(id);
            }
            catch (Exception)
            {
                _notificationService.Show(
                    "Nie znaleziono artykułu do edycji!",
                    EnumNotificationType.Error);
                return;
            }

            var article = new Article
            {
                Id = id,
                Title = title.Trim(),
                Text = text.Trim(),

                // Przy edycji zachowujemy pierwotnego autora.
                Author = existingArticle.Author,

                PublishDate = existingArticle.PublishDate,
                MainImageId = fileId > 0
                    ? fileId
                    : existingArticle.MainImageId,
                Category = category,

                AnnouncementDeadline = announcementDeadline,

                SubstituteDate = substituteDate,
                SchoolClass = schoolClass?.Trim(),
                Teacher = teacher?.Trim(),

                EventDate = eventDate,
                EventTime = eventTime,
                EventLocation = eventLocation?.Trim(),

                CompetitionDate = competitionDate,
                CompetitionDiscipline = competitionDiscipline?.Trim(),
                CompetitionLocation = competitionLocation?.Trim(),

                AdmissionsStart = admissionsStart,
                AdmissionsEnd = admissionsEnd,

                ExamDate = examDate,
                ExamSubject = examSubject?.Trim(),
                ExamClass = examClass?.Trim(),

                CouncilInitiative = councilInitiative?.Trim()
            };

            int result = await _articleRepository
                .UpdateArticleByIdAsync(article);

            if (result > 0)
            {
                _notificationService.Show(
                    "Pomyślnie zaktualizowano artykuł!",
                    EnumNotificationType.Success);
            }
            else
            {
                _notificationService.Show(
                    "Podczas edytowania artykułu wystąpił błąd!",
                    EnumNotificationType.Error);
            }
        }

        public async Task<List<Article>> GetAllArticlesAsync()
        {
            return await _articleRepository.GetAllArticlesAsync();
        }

        public async Task<Article> GetArticleByIdAsync(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentException(
                    "Nieprawidłowe ID artykułu.",
                    nameof(id));
            }

            return await _articleRepository.GetArticleByIdAsync(id);
        }

        public async Task<bool> RemoveArticleByIdAsync(int id)
        {
            if (id <= 0)
            {
                _notificationService.Show(
                    "Nieprawidłowe ID artykułu!",
                    EnumNotificationType.Failure);
                return false;
            }

            bool hasPermission = await _permissionService
                .HasPermissionAsync(EnumPermissions.DeleteArticle);

            if (!hasPermission)
            {
                _notificationService.Show(
                    "Brak uprawnień do usuwania artykułów!",
                    EnumNotificationType.Failure);
                return false;
            }

            await _articleRepository.RemoveArticleByIdAsync(id);

            _notificationService.Show(
                "Pomyślnie usunięto artykuł!",
                EnumNotificationType.Success);

            return true;
        }
    }
}
