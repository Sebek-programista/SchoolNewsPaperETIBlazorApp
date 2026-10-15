using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SchoolNewspaperBlazorApp.Data;
using SchoolNewspaperBlazorApp.Interfaces.Repository;

namespace SchoolNewspaperBlazorApp.Repository
{
    public class AuthRepository : IAuthRepository
    {
        private readonly IDbContextFactory<NewspaperDbContext> _contextFactory;

        public AuthRepository(IDbContextFactory<NewspaperDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
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

            _contextFactory.CreateDbContext().Users.Add(user);
            await _contextFactory.CreateDbContext().SaveChangesAsync();
        }
        public async Task<string> CheckIfRegisteredAsync(string login, string password)
        {
            var user = await _contextFactory.CreateDbContext().Users
                .FirstOrDefaultAsync(u => u.Login == login);
            if (user != null)
            {
                return user.Password;
            }
            else 
            {
                throw new Exception("Konto nie istenieje");
            }

        }
    }
}
