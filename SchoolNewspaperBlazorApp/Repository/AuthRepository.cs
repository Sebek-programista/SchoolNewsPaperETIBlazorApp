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

        public async Task AddUserAsync(User user)
        {
            // DbSet nazwany Users w NewspaperDbContext
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
        }
        public async Task<bool> CheckIfRegisteredAsync(string login, string password)
        {
            var exist = await _context.Users
                .AnyAsync(u => u.Login == login && u.Password == password);
            if (exist)
            {
                return true;
            }
            else 
            {
                return false;
            }

        }
    }
}
