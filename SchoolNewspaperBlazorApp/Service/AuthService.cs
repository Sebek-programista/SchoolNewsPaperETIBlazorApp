using SchoolNewspaperBlazorApp.Data;
using SchoolNewspaperBlazorApp.Interfaces.Service;
using Microsoft.EntityFrameworkCore;

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
            var user = new User
            {
                Login = login,
                Password = password,
                Email = email,
                RegisterDate = DateTime.Now
            };

            _authRepository.Users.Add(user);
            await _authRepository.SaveChangesAsync();
        }
        public async Task<bool> CheckIfRegisteredAsync(
            string login, string password)
        {
            // Uwaga: hasła powinny być przechowywane w postaci skrótu (hash) — to tylko przykład działania.
            return await _authRepository.Users
                .AnyAsync(u => u.Login == login && u.Password == password);

        }
    }
}
