using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SchoolNewspaperBlazorApp.Data;
using SchoolNewspaperBlazorApp.Interfaces.Repository;

namespace SchoolNewspaperBlazorApp.Repository
{
    public class AuthRepository : IAuthRepository
    {
        private readonly NewspaperDbContext _context;

        public AuthRepository(NewspaperDbContext context)
        {
            _context = context;
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

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
        }
        public async Task<string> CheckIfRegisteredAsync(string login, string password)
        {
            var user = await _context.Users
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
