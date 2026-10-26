
using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using SchoolNewspaperBlazorApp.Data;
using SchoolNewspaperBlazorApp.Interfaces.Repository;
using SchoolNewspaperBlazorApp.Interfaces.Service;

namespace SchoolNewspaperBlazorApp.Service
{
    public class AuthService : IAuthService
    {
        private readonly IAuthRepository _authRepository;
        private readonly IPermissionRepository _permissionRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IPermissionService _permissionService;
        private readonly AuthenticationStateProvider _authenticationStateProvider;
        private readonly ILogger<AuthService> _logger;

        public AuthService(
            IAuthRepository authRepository,
            IPermissionRepository permissionRepository,
            IRoleRepository roleRepository,
            IPermissionService permissionService,
            AuthenticationStateProvider authenticationStateProvider,
            ILogger<AuthService> logger)
        {
            _authRepository = authRepository;
            _permissionRepository = permissionRepository;
            _roleRepository = roleRepository;
            _permissionService = permissionService;
            _authenticationStateProvider = authenticationStateProvider;
            _logger = logger;
        }

        public async Task<Dictionary<int, string>> GetAllUsersAsync()
        {
            return await _authRepository.GetAllUsersAsync();
        }

        public async Task AddUserAsync(
            string login,
            string password,
            string email,
            string firstName,
            string lastName)
        {
            if (string.IsNullOrWhiteSpace(password))
                throw new InvalidOperationException("Hasło jest wymagane.");

            var hasher = new PasswordHasher<string>();
            string passwordHash = hasher.HashPassword(null!, password);

            var user = new User
            {
                Login = login,
                Password = passwordHash,
                Email = email,
                RegisterDate = DateTime.Now,
                FirstName = firstName.Trim(),
                LastName = lastName.Trim()
            };

            await _authRepository.AddUserAsync(user);
        }

        public async Task<int> CheckIfRegisteredAsync(
            string login,
            string password)
        {
            var hasher = new PasswordHasher<string>();

            try
            {
                var user =
                    await _authRepository.CheckIfRegisteredAsync(login, password);

                if (user.MustChangePassword)
                    return 0;

                if (user.TemporaryPasswordExpiresAt.HasValue &&
                    user.TemporaryPasswordExpiresAt.Value <= DateTime.Now)
                    return 0;

                var result = hasher.VerifyHashedPassword(
                    user.Login,
                    user.Password,
                    password);

                return result == PasswordVerificationResult.Failed
                    ? 0
                    : user.Id;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(
                    ex,
                    "Nie udało się zweryfikować danych logowania.");

                return 0;
            }
        }

        public async Task<string> GetFullNameAsync(int userId)
        {
            var user = await _authRepository.GetUserByIdAsync(userId);

            return user == null
                ? ""
                : $"{user.FirstName} {user.LastName}".Trim();
        }

        public async Task<UserDetails?> GetUserDetailsAsync(int id)
        {
            var user = await _authRepository.GetUserByIdAsync(id);

            return user == null
                ? null
                : new UserDetails(
                    user.Id,
                    user.Login,
                    user.Email,
                    user.FirstName,
                    user.LastName);
        }

        public async Task UpdateUserAsync(
            int id,
            string login,
            string email,
            string firstName,
            string lastName,
            string? newPassword)
        {
            bool allowed =
                await _permissionService.HasPermissionAsync(
                    EnumPermissions.ManageUser)
                || await _permissionService.HasPermissionAsync(
                    EnumPermissions.Master);

            if (!allowed)
                throw new UnauthorizedAccessException(
                    "Brak uprawnień do edycji użytkowników.");

            login = (login ?? string.Empty).Trim();
            email = (email ?? string.Empty).Trim();
            firstName = (firstName ?? string.Empty).Trim();
            lastName = (lastName ?? string.Empty).Trim();

            if (login.Length == 0 || email.Length == 0)
                throw new InvalidOperationException(
                    "Login i e-mail są wymagane.");

            if (!email.Contains('@'))
                throw new InvalidOperationException(
                    "Podaj poprawny adres e-mail.");

            var state =
                await _authenticationStateProvider.GetAuthenticationStateAsync();

            var currentId = state.User
                .FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (currentId != id.ToString())
            {
                var permissions =
                    await _permissionRepository.GetUserPermissionsAsync(id);

                var role =
                    await _roleRepository.GetRoleByUserIdAsync(id);

                if (permissions.Contains(EnumPermissions.Master)
                    || Role.IsMasterName(role?.RoleName))
                {
                    throw new InvalidOperationException(
                        "Nie można edytować konta administratora.");
                }
            }

            if (await _authRepository.IsLoginOrEmailTakenAsync(
                id, login, email))
            {
                throw new InvalidOperationException(
                    "Ten login lub e-mail jest już zajęty.");
            }

            string? passwordHash = null;

            if (!string.IsNullOrEmpty(newPassword))
            {
                if (newPassword.Length < 8)
                    throw new InvalidOperationException(
                        "Hasło musi mieć co najmniej 8 znaków.");

                passwordHash =
                    new PasswordHasher<string>()
                        .HashPassword(null!, newPassword);
            }

            await _authRepository.UpdateUserAsync(
                id,
                login,
                email,
                firstName,
                lastName,
                passwordHash);
        }

        public async Task ChangeOwnPasswordAsync(
            string currentPassword,
            string newPassword)
        {
            if (string.IsNullOrWhiteSpace(currentPassword))
                throw new InvalidOperationException("Wpisz obecne hasło.");

            if (string.IsNullOrWhiteSpace(newPassword) || newPassword.Length < 8)
                throw new InvalidOperationException(
                    "Nowe hasło musi mieć co najmniej 8 znaków.");

            if (currentPassword == newPassword)
                throw new InvalidOperationException(
                    "Nowe hasło musi być inne niż obecne.");

            var state =
                await _authenticationStateProvider.GetAuthenticationStateAsync();

            if (state.User.Identity?.IsAuthenticated != true)
                throw new UnauthorizedAccessException("Musisz być zalogowany.");

            var idClaim = state.User
                .FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!int.TryParse(idClaim, out int userId))
                throw new InvalidOperationException(
                    "Nie można ustalić ID zalogowanego użytkownika.");

            var permissions =
                await _permissionRepository.GetUserPermissionsAsync(userId);

            var role =
                await _roleRepository.GetRoleByUserIdAsync(userId);

            if (permissions.Contains(EnumPermissions.Master)
                || Role.IsMasterName(role?.RoleName))
            {
                throw new UnauthorizedAccessException(
                    "Zmiana hasła konta administratora tą metodą jest zablokowana.");
            }

            var user = await _authRepository.GetUserByIdAsync(userId);

            if (user == null)
                throw new InvalidOperationException("Nie znaleziono użytkownika.");

            var hasher = new PasswordHasher<string>();

            var verification = hasher.VerifyHashedPassword(
                user.Login,
                user.Password,
                currentPassword);

            if (verification == PasswordVerificationResult.Failed)
                throw new InvalidOperationException(
                    "Obecne hasło jest nieprawidłowe.");

            string newPasswordHash = hasher.HashPassword(
                user.Login,
                newPassword);

            await _authRepository.UpdateUserAsync(
                user.Id,
                user.Login,
                user.Email,
                user.FirstName,
                user.LastName,
                newPasswordHash);

            _logger.LogInformation(
                "Użytkownik {UserId} zmienił swoje hasło.",
                user.Id);
        }

        public async Task<string> GenerateTemporaryPasswordAsync(int userId)
        {
            bool allowed =
                await _permissionService.HasPermissionAsync(
                    EnumPermissions.ManageUser)
                || await _permissionService.HasPermissionAsync(
                    EnumPermissions.Master);

            if (!allowed)
                throw new UnauthorizedAccessException(
                    "Brak uprawnień do generowania haseł tymczasowych.");

            if (userId <= 0)
                throw new InvalidOperationException("Nieprawidłowe ID użytkownika.");

            var target = await _authRepository.GetUserByIdAsync(userId);

            if (target == null)
                throw new InvalidOperationException("Nie znaleziono użytkownika.");

            var permissions =
                await _permissionRepository.GetUserPermissionsAsync(userId);

            var role = await _roleRepository.GetRoleByUserIdAsync(userId);

            if (permissions.Contains(EnumPermissions.Master)
                || Role.IsMasterName(role?.RoleName))
            {
                throw new UnauthorizedAccessException(
                    "Nie można wygenerować hasła tymczasowego dla konta Master.");
            }


            // Losowe hasło tymczasowe, które administrator przekazuje użytkownikowi.
            string temporaryPassword =
                Convert.ToBase64String(RandomNumberGenerator.GetBytes(12))
                    .Replace("+", "A")
                    .Replace("/", "B")
                    .Replace("=", "C");

            string passwordHash =
                new PasswordHasher<string>().HashPassword(
                    target.Login,
                    temporaryPassword);

            await _authRepository.SetTemporaryPasswordAsync(
                userId,
                passwordHash,
                DateTime.Now.AddMinutes(30));

            _logger.LogInformation(
                "Wygenerowano hasło tymczasowe dla użytkownika {UserId}.",
                userId);

            return temporaryPassword;
        }

        public async Task<bool> ResetPasswordWithTemporaryPasswordAsync(
            string loginOrEmail,
            string temporaryPassword,
            string newPassword)
        {
            if (string.IsNullOrWhiteSpace(loginOrEmail)
                || string.IsNullOrWhiteSpace(temporaryPassword)
                || string.IsNullOrWhiteSpace(newPassword))
            {
                throw new InvalidOperationException(
                    "Wypełnij wszystkie pola.");
            }

            if (newPassword.Length < 8)
                throw new InvalidOperationException(
                    "Nowe hasło musi mieć co najmniej 8 znaków.");

            var user =
                await _authRepository.GetUserByLoginOrEmailAsync(
                    loginOrEmail.Trim());

            if (user == null || !user.MustChangePassword)
                return false;

            if (!user.TemporaryPasswordExpiresAt.HasValue
                || user.TemporaryPasswordExpiresAt.Value <= DateTime.Now)
            {
                return false;
            }

            var permissions =
                await _permissionRepository.GetUserPermissionsAsync(user.Id);

            var role =
                await _roleRepository.GetRoleByUserIdAsync(user.Id);

            if (permissions.Contains(EnumPermissions.Master)
                || Role.IsMasterName(role?.RoleName))
            {
                return false;
            }

            var hasher = new PasswordHasher<string>();

            var result = hasher.VerifyHashedPassword(
                user.Login,
                user.Password,
                temporaryPassword);

            if (result == PasswordVerificationResult.Failed)
                return false;

            string newPasswordHash = hasher.HashPassword(
                user.Login,
                newPassword);

            await _authRepository.ResetPasswordAsync(
                user.Id,
                newPasswordHash);

            _logger.LogInformation(
                "Użytkownik {UserId} ustawił nowe hasło przy użyciu hasła tymczasowego.",
                user.Id);

            return true;
        }

        public async Task DeleteUserByIdAsync(int id)
        {
            bool allowed =
                await _permissionService.HasPermissionAsync(
                    EnumPermissions.ManageUser)
                || await _permissionService.HasPermissionAsync(
                    EnumPermissions.Master);

            if (!allowed)
                throw new UnauthorizedAccessException(
                    "Brak uprawnień do usuwania użytkowników.");

            var state =
                await _authenticationStateProvider.GetAuthenticationStateAsync();

            var currentId = state.User
                .FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (currentId == id.ToString())
                throw new InvalidOperationException(
                    "Nie możesz usunąć własnego konta.");

            var permissions =
                await _permissionRepository.GetUserPermissionsAsync(id);

            var role =
                await _roleRepository.GetRoleByUserIdAsync(id);

            if (permissions.Contains(EnumPermissions.Master)
                || Role.IsMasterName(role?.RoleName))
            {
                throw new InvalidOperationException(
                    "Nie można usunąć konta administratora.");
            }

            await _authRepository.DeleteUserByIdAsync(id);
        }
    }
}