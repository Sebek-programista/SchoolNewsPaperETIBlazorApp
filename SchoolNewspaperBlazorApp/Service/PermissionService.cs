
using Microsoft.AspNetCore.Components.Authorization;
using SchoolNewspaperBlazorApp.Data.Enums;
using SchoolNewspaperBlazorApp.Interfaces.Repository;
using SchoolNewspaperBlazorApp.Interfaces.Service;
using System.Security.Claims;

namespace SchoolNewspaperBlazorApp.Service
{
    public class PermissionService : IPermissionService
    {
        private readonly IPermissionRepository _permissionRepository;
        private readonly INotificationService _notificationService;
        private readonly AuthenticationStateProvider _asp;
        private readonly ILogger<PermissionService> _logger;

        public PermissionService(
            IPermissionRepository permissionRepository,
            AuthenticationStateProvider asp,
            INotificationService notificationService,
            ILogger<PermissionService> logger)
        {
            _permissionRepository = permissionRepository;
            _asp = asp;
            _notificationService = notificationService;
            _logger = logger;
        }

        public async Task<List<EnumPermissions>> GetUserPermissionsAsync(
            int userId)
        {
            try
            {
                return await _permissionRepository
                    .GetUserPermissionsAsync(userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Błąd pobierania uprawnień użytkownika {UserId}", userId);
                return new List<EnumPermissions>();
            }
        }

        public async Task<Dictionary<EnumPermissions, bool>>
            GetAllPermissionsAsync()
        {
            try
            {
                int id = await GetUserId();

                return await _permissionRepository
                    .GetAllUserPermissionsAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Błąd pobierania wszystkich uprawnień.");

                return Enum.GetValues<EnumPermissions>()
                    .ToDictionary(p => p, p => false);
            }
        }

        public async Task<bool> HasPermissionAsync(
            EnumPermissions permission)
        {
            try
            {
                var authState = await _asp.GetAuthenticationStateAsync();

                return await _permissionRepository.HasPermissionAsync(
                    authState.User, permission);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Błąd sprawdzania uprawnienia {Permission}", permission);
                return false;
            }
        }

        public async Task SavePermissionsAsync(
            List<EnumPermissions> permissions, int userId)
        {
            try
            {
                bool hasPermission =
                    await HasPermissionAsync(EnumPermissions.Master);

                if (!hasPermission)
                {
                    _notificationService.Show(
                        "Brak uprawnień!",
                        EnumNotificationType.Failure);
                    return;
                }

                bool success = await _permissionRepository
                    .SavePermissionsAsync(permissions, userId);

                if (success)
                {
                    _notificationService.Show(
                        "Nadano uprawnienia!",
                        EnumNotificationType.Success);
                }
                else
                {
                    _notificationService.Show(
                        "Błąd podczas nadawania uprawnień!",
                        EnumNotificationType.Failure);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Błąd zapisywania uprawnień użytkownika {UserId}", userId);

                _notificationService.Show(
                    "Wystąpił błąd podczas zapisywania uprawnień.",
                    EnumNotificationType.Error);
            }
        }

        public async Task<int> GetUserId()
        {
            try
            {
                var authState = await _asp.GetAuthenticationStateAsync();

                var stringId = authState.User
                    .FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (int.TryParse(stringId, out int id) && id > 0)
                {
                    return id;
                }

                return 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Błąd pobierania ID zalogowanego użytkownika.");
                return 0;
            }
        }
    }
}
