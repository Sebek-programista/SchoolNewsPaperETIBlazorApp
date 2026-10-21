using Microsoft.AspNetCore.Authentication;
using SchoolNewspaperBlazorApp.Data;
using SchoolNewspaperBlazorApp.Interfaces.Repository;
using SchoolNewspaperBlazorApp.Interfaces.Service;
using SchoolNewspaperBlazorApp.Repository;
using Microsoft.AspNetCore.Identity;
using SchoolNewspaperBlazorApp.Data.Emuns;

namespace SchoolNewspaperBlazorApp.Service
{
    public class AuthService : IAuthService
    {
        private readonly IAuthRepository _authRepository;
        private readonly IPermissionService _permissionService;
        private readonly INotificationService _notificationService;
        public AuthService(IAuthRepository authRepository, IPermissionService permissionService, INotificationService notificationService)
        {
            _authRepository = authRepository;
            _permissionService = permissionService;
            _notificationService = notificationService;
        }
        public async Task<Dictionary<int,string>> GetAllUsersAsync()
        {
            return await _authRepository.GetAllUsersAsync();
        }
        public async Task AddUserAsync(string login, string password, string email)
        {
            bool hasPermission = await _permissionService.HasPermissionAsync(EnumPermissions.AddUser);
            if (!hasPermission)
            {
                _notificationService.Show("Brak uprawnień!", EnumNotificationType.Faliure);
                return;
            }
            var hasher = new PasswordHasher<string>();
            string passwordHash = hasher.HashPassword(null!, password);
            var auth = new User
            {

               Password = passwordHash,
               Email = email,
               RegisterDate = DateTime.Now
            };
            var succes = await _authRepository.AddUserAsync(auth);
            if (succes)
            {
                _notificationService.Show("Pomyslnie dodano uzytkownika!", EnumNotificationType.Succes);
            }
            else
            {
                _notificationService.Show("Podczas dodawania uzytkownika wystąpił błąd lub już istnieje konto z takim adresem email! Skontaktuj sie z obslugą techniczną", EnumNotificationType.Faliure);
            }
        }
        public async Task<int> CheckIfRegisteredAsync(string login,string password)
        {
            var hasher = new PasswordHasher<string>();
           // string passwordHash = hasher.HashPassword(null!, password);
            try
            {
                var user = await _authRepository.CheckIfRegisteredAsync(login);
                var result = hasher.VerifyHashedPassword(login,user.Password,password);
                if(result == PasswordVerificationResult.Success)
                {
                    return user.Id;
                }
                else
                {
                    return 0;
                }
            }
            catch (Exception)
            {
                return 0;
            }

        }


    }
}
