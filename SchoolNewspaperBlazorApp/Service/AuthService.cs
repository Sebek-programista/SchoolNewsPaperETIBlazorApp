
using Microsoft.AspNetCore.Identity;
using SchoolNewspaperBlazorApp.Data;
using SchoolNewspaperBlazorApp.Data.Enums;
using SchoolNewspaperBlazorApp.Interfaces.Repository;
using SchoolNewspaperBlazorApp.Interfaces.Service;
using System.Text.RegularExpressions;

namespace SchoolNewspaperBlazorApp.Service
{
    public class AuthService : IAuthService
    {
        private readonly IAuthRepository _authRepository;
        private readonly ISchoolRoleRepository _schoolRoleRepository;
        private readonly IPermissionService _permissionService;
        private readonly INotificationService _notificationService;

        private static readonly Regex EmailRegex = new(
            @"^[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,}$"
        );

        public AuthService(
            IAuthRepository authRepository,
            ISchoolRoleRepository schoolRoleRepository,
            IPermissionService permissionService,
            INotificationService notificationService)
        {
            _authRepository = authRepository;
            _schoolRoleRepository = schoolRoleRepository;
            _permissionService = permissionService;
            _notificationService = notificationService;
        }

        public async Task<Dictionary<int, string>> GetAllUsersAsync()
        {
            return await _authRepository.GetAllUsersAsync();
        }

        // Tworzenie użytkownika z przypisaną rolą szkolną.
        public async Task<bool> AddUserAsync(
            string password,
            string email,
            string name,
            string surname,
            int? schoolRoleId)
        {
            if (string.IsNullOrWhiteSpace(password))
            {
                _notificationService.Show(
                    "Hasło jest puste!",
                    EnumNotificationType.Failure);
                return false;
            }

            if (string.IsNullOrWhiteSpace(email) ||
                !EmailRegex.IsMatch(email.Trim()))
            {
                _notificationService.Show(
                    "E-mail jest niepoprawny!",
                    EnumNotificationType.Failure);
                return false;
            }

            // Zachowujemy parametry imienia i nazwiska,
            // ponieważ mogą być wykorzystywane w innych formularzach.
            // Obecny model User nie przechowuje tych pól.

            bool hasPermission =
                await _permissionService.HasPermissionAsync(
                    EnumPermissions.AddUser);

            if (!hasPermission)
            {
                _notificationService.Show(
                    "Brak uprawnień do dodawania użytkowników!",
                    EnumNotificationType.Failure);
                return false;
            }

            // Jeżeli wybrano rolę, sprawdzamy ją w bazie.
            if (schoolRoleId.HasValue)
            {
                var schoolRole =
                    await _schoolRoleRepository.GetByIdAsync(
                        schoolRoleId.Value);

                if (schoolRole == null || !schoolRole.IsActive)
                {
                    _notificationService.Show(
                        "Wybrana rola nie istnieje lub jest nieaktywna!",
                        EnumNotificationType.Failure);
                    return false;
                }
            }

            var hasher = new PasswordHasher<string>();

            string passwordHash =
                hasher.HashPassword(null!, password);

            var user = new User
            {
                Password = passwordHash,
                Email = email.Trim(),
                RegisterDate = DateTime.Now,
                SchoolRoleId = schoolRoleId
            };

            bool success =
                await _authRepository.AddUserAsync(user);

            if (success)
            {
                _notificationService.Show(
                    "Dodawanie użytkownika powiodło się!",
                    EnumNotificationType.Success);
            }
            else
            {
                _notificationService.Show(
                    "Nie udało się dodać użytkownika lub e-mail już istnieje!",
                    EnumNotificationType.Error);
            }

            return success;
        }

        // Zachowanie kompatybilności ze starszymi formularzami.
        public async Task AddUserAsync(
            string password,
            string email)
        {
            await AddUserAsync(
                password,
                email,
                string.Empty,
                string.Empty,
                null);
        }

        public async Task<int> CheckIfRegisteredAsync(
            string login,
            string password)
        {
            try
            {
                var user =
                    await _authRepository.CheckIfRegisteredAsync(login);

                var hasher = new PasswordHasher<string>();

                var result = hasher.VerifyHashedPassword(
                    login,
                    user.Password,
                    password);

                if (result != PasswordVerificationResult.Failed)
                {
                    return user.Id;
                }

                return 0;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public async Task<bool> RemoveUserByIdAsync(int userId)
        {
            try
            {
                bool hasPermission =
                    await _permissionService.HasPermissionAsync(
                        EnumPermissions.Master);

                if (!hasPermission)
                {
                    _notificationService.Show(
                        "Tylko użytkownik Master może usuwać konta.",
                        EnumNotificationType.Failure);
                    return false;
                }

                if (userId <= 0)
                {
                    _notificationService.Show(
                        "Nieprawidłowe ID użytkownika.",
                        EnumNotificationType.Failure);
                    return false;
                }

                int currentUserId =
                    await _permissionService.GetUserId();

                if (userId == currentUserId)
                {
                    _notificationService.Show(
                        "Nie możesz usunąć własnego konta.",
                        EnumNotificationType.Failure);
                    return false;
                }

                bool success =
                    await _authRepository.RemoveUserByIdAsync(userId);

                if (!success)
                {
                    _notificationService.Show(
                        "Nie udało się usunąć użytkownika.",
                        EnumNotificationType.Error);
                    return false;
                }

                _notificationService.Show(
                    "Użytkownik został usunięty.",
                    EnumNotificationType.Success);

                return true;
            }
            catch (Exception)
            {
                _notificationService.Show(
                    "Wystąpił błąd podczas usuwania użytkownika.",
                    EnumNotificationType.Error);

                return false;
            }
        }
    }
}
