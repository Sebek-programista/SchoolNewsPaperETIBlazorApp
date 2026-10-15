using SchoolNewspaperBlazorApp.Data;
using SchoolNewspaperBlazorApp.Interfaces.Service;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace SchoolNewspaperBlazorApp.Service
{
    public class AuthService : IAuthService
    {
        private readonly NewspaperDbContext _authRepository;

        public AuthService(NewspaperDbContext context)
        {
            _authRepository = context;
        }

        public async Task AddUserAsync(string login, string password, string email)
        {
            var hasher = new PasswordHasher<string>();
            string passwordHash = hasher.HashPassword(null!, password);
            var user = new User
            {
                Login = login,
                Password = passwordHash,
                Email = email,
                RegisterDate = DateTime.Now
            };

            _authRepository.Users.Add(user);
            await _authRepository.SaveChangesAsync();
        }
        public async Task<bool> CheckIfRegisteredAsync(
            string login, string password)
        {
            var hasher = new PasswordHasher<string>();
            string passwordHash = hasher.HashPassword(null!, password)
            // Uwaga: hasła powinny być przechowywane w postaci skrótu (hash) — to tylko przykład działania.
            return await _authRepository.Users
                .AnyAsync(u => u.Login == login && u.Password == passwordHash);

        }
    }
}
