
using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Logging;
using SchoolNewspaperBlazorApp.Data;
using SchoolNewspaperBlazorApp.Interfaces.Repository;
using SchoolNewspaperBlazorApp.Interfaces.Service;

namespace SchoolNewspaperBlazorApp.Service
{
    public class ArticleService : IArticleService
    {
        private readonly IArticleRepository _articleRepository;
        private readonly IAuthRepository _authRepository;
        private readonly IPermissionService _permissionService;
        private readonly AuthenticationStateProvider _authenticationStateProvider;
        private readonly ILogger<ArticleService> _logger;

        public ArticleService(
            IArticleRepository articleRepository,
            IAuthRepository authRepository,
            IPermissionService permissionService,
            AuthenticationStateProvider authenticationStateProvider,
            ILogger<ArticleService> logger)
        {
            _articleRepository = articleRepository;
            _authRepository = authRepository;
            _permissionService = permissionService;
            _authenticationStateProvider = authenticationStateProvider;
            _logger = logger;
        }

        public async Task AddArticleAsync(
            string title,
            string text,
            List<int> fileIds,
            ArticleType? eventType = null)
        {
            try
            {
                // Sprawdzenie uprawnień
                bool hasPermission =
                    await _permissionService.HasPermissionAsync(
                        EnumPermissions.AddArticle);

                bool isMaster =
                    await _permissionService.HasPermissionAsync(
                        EnumPermissions.Master);

                if (!hasPermission && !isMaster)
                {
                    throw new UnauthorizedAccessException(
                        "Nie masz uprawnień do dodawania artykułów.");
                }

                // Sprawdzenie danych
                if (string.IsNullOrWhiteSpace(title))
                {
                    throw new ArgumentException(
                        "Tytuł artykułu nie może być pusty.");
                }

                if (string.IsNullOrWhiteSpace(text))
                {
                    throw new ArgumentException(
                        "Treść artykułu nie może być pusta.");
                }

                fileIds ??= new List<int>();

                // Pobranie zalogowanego użytkownika
                var state =
                    await _authenticationStateProvider
                        .GetAuthenticationStateAsync();

                var idClaim = state.User
                    .FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (!int.TryParse(idClaim, out int userId))
                {
                    throw new InvalidOperationException(
                        "Nie udało się odczytać ID zalogowanego użytkownika.");
                }

                var user =
                    await _authRepository.GetUserByIdAsync(userId);

                if (user == null)
                {
                    throw new InvalidOperationException(
                        "Nie znaleziono użytkownika w bazie danych.");
                }

                // Przygotowanie autora
                string fullName =
                    $"{user.FirstName} {user.LastName}".Trim();

                if (string.IsNullOrWhiteSpace(fullName))
                {
                    fullName = user.Login;
                }

                // Utworzenie artykułu
                var article = new Article
                {
                    Title = title.Trim(),
                    Text = text.Trim(),
                    UserId = user.Id,
                    Author = fullName,
                    PublishDate = DateTime.Now,

                    // Pierwszy plik jako główny obraz
                    FileId = fileIds.FirstOrDefault(),

                    // Wszystkie obrazy artykułu
                    Images = fileIds
                        .Select((fileId, index) =>
                            new ArticleImage
                            {
                                FileId = fileId,
                                Order = index
                            })
                        .ToList(),

                    EventsType = eventType,
                    IsDeleted = false
                };

                // Zapis do bazy danych
                await _articleRepository.AddArticleAsync(article);

                _logger.LogInformation(
                    "Dodano artykuł. ID użytkownika: {UserId}, tytuł: {Title}",
                    user.Id,
                    article.Title);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Błąd podczas dodawania artykułu.");

                throw;
            }
        }

        public async Task<List<Article>> GetAllArticlesAsync()
        {
            try
            {
                return await _articleRepository.GetAllArticlesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Błąd podczas pobierania artykułów.");

                throw;
            }
        }

        public async Task<Article> GetArticleByIdAsync(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentException(
                    "Nieprawidłowe ID artykułu.");
            }

            try
            {
                return await _articleRepository.GetArticleByIdAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Nie udało się pobrać artykułu o ID {ArticleId}.",
                    id);

                throw;
            }
        }

        public async Task RemoveArticleByIdAsync(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentException(
                    "Nieprawidłowe ID artykułu.");
            }

            try
            {
                bool hasPermission =
                    await _permissionService.HasPermissionAsync(
                        EnumPermissions.DeleteArticle);

                bool isMaster =
                    await _permissionService.HasPermissionAsync(
                        EnumPermissions.Master);

                if (!hasPermission && !isMaster)
                {
                    throw new UnauthorizedAccessException(
                        "Nie masz uprawnień do usuwania artykułów.");
                }

                await _articleRepository.RemoveArticleByIdAsync(id);

                _logger.LogInformation(
                    "Usunięto artykuł o ID {ArticleId}.",
                    id);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Błąd podczas usuwania artykułu o ID {ArticleId}.",
                    id);

                throw;
            }
        }

        public async Task UpdateArticleByIdAsync(
            int id,
            string title,
            string text,
            int fileId,
            ArticleType? eventType = null)
        {
            if (id <= 0)
            {
                throw new ArgumentException(
                    "Nieprawidłowe ID artykułu.");
            }

            if (string.IsNullOrWhiteSpace(title))
            {
                throw new ArgumentException(
                    "Tytuł artykułu nie może być pusty.");
            }

            if (string.IsNullOrWhiteSpace(text))
            {
                throw new ArgumentException(
                    "Treść artykułu nie może być pusta.");
            }

            try
            {
                bool hasPermission =
                    await _permissionService.HasPermissionAsync(
                        EnumPermissions.EditArticle);

                bool isMaster =
                    await _permissionService.HasPermissionAsync(
                        EnumPermissions.Master);

                if (!hasPermission && !isMaster)
                {
                    throw new UnauthorizedAccessException(
                        "Nie masz uprawnień do edytowania artykułów.");
                }

                var article = new Article
                {
                    Id = id,
                    Title = title.Trim(),
                    Text = text.Trim(),
                    FileId = fileId,
                    EventsType = eventType
                };

                await _articleRepository.UpdateArticleByIdAsync(
                    id,
                    article);

                _logger.LogInformation(
                    "Zaktualizowano artykuł o ID {ArticleId}.",
                    id);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Błąd podczas edycji artykułu o ID {ArticleId}.",
                    id);

                throw;
            }
        }
    }
}
