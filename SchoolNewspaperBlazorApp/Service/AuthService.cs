using Microsoft.AspNetCore.Authentication;
using SchoolNewspaperBlazorApp.Data;
using SchoolNewspaperBlazorApp.Interfaces.Repository;
using SchoolNewspaperBlazorApp.Interfaces.Service;
using SchoolNewspaperBlazorApp.Repository;
using Microsoft.AspNetCore.Identity;

namespace SchoolNewspaperBlazorApp.Service
{
    public class AuthService : IAuthService
    {
        private readonly IAuthRepository _authRepository;
        private readonly IPermissionService _permissionService;
        public AuthService(IAuthRepository authRepository, IPermissionService permissionService)
        {
            _authRepository = authRepository;
            _permissionService = permissionService;
        }
        public async Task<Dictionary<int,string>> GetAllUsersAsync()
        {
            return await _authRepository.GetAllUsersAsync();
        }
        public async Task AddUserAsync(string login, string password, string email)
        {
            bool hasPermission = await _permissionService.HasPermissionAsync(EnumPermissions.AddUser);
            if (!hasPermission) return;
            var hasher = new PasswordHasher<string>();
            string passwordHash = hasher.HashPassword(null!, password);
            var auth = new User
            {
               Login = login,
               Password = passwordHash,
               Email = email,
               RegisterDate = DateTime.Now
            };
            await _authRepository.AddUserAsync(auth);
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
