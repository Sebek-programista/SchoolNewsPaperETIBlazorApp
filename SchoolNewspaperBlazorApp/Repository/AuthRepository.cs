
using Microsoft.EntityFrameworkCore;
using SchoolNewspaperBlazorApp.Data;
using SchoolNewspaperBlazorApp.Interfaces.Repository;

namespace SchoolNewspaperBlazorApp.Repository
{
    public class AuthRepository : IAuthRepository
    {
        private readonly IDbContextFactory<NewspaperDbContext> _contextFactory;
        private readonly ILogger<AuthRepository> _logger;

        public AuthRepository(
            IDbContextFactory<NewspaperDbContext> contextFactory,
            ILogger<AuthRepository> logger)
        {
            _contextFactory = contextFactory;
            _logger = logger;
        }

        public async Task<Dictionary<int, string>> GetAllUsersAsync()
        {
            try
            {
                await using var context =
                    await _contextFactory.CreateDbContextAsync();

                return await context.Users
                    .ToDictionaryAsync(u => u.Id, u => u.Email);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Błąd pobierania użytkowników.");
                return new Dictionary<int, string>();
            }
        }

        public async Task<User?> GetUserByIdAsync(int userId)
        {
            try
            {
                await using var context =
                    await _contextFactory.CreateDbContextAsync();

                return await context.Users
                    .FirstOrDefaultAsync(u => u.Id == userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Błąd pobierania użytkownika o ID {UserId}", userId);
                return null;
            }
        }

        public async Task<bool> AddUserAsync(User user)
        {
            try
            {
                await using var context =
                    await _contextFactory.CreateDbContextAsync();

                bool exists = await context.Users
                    .AnyAsync(u => u.Email == user.Email);

                if (exists)
                {
                    _logger.LogWarning(
                        "Próba dodania użytkownika z istniejącym e-mailem.");
                    return false;
                }

                await context.Users.AddAsync(user);
                await context.SaveChangesAsync();

                _logger.LogInformation(
                    "Dodano użytkownika o ID {UserId}", user.Id);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Błąd dodawania użytkownika.");
                return false;
            }
        }

        public async Task<User> CheckIfRegisteredAsync(string login)
        {
            try
            {
                await using var context =
                    await _contextFactory.CreateDbContextAsync();

                var user = await context.Users
                    .FirstOrDefaultAsync(u => u.Email == login);

                if (user == null)
                {
                    _logger.LogWarning(
                        "Nie znaleziono użytkownika podczas logowania.");
                    throw new Exception("Konto nie istnieje.");
                }

                return user;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Błąd wyszukiwania użytkownika podczas logowania.");
                throw;
            }
        }

        public async Task<bool> RemoveUserByIdAsync(int userId)
        {
            try
            {
                await using var context =
                    await _contextFactory.CreateDbContextAsync();

                var user = await context.Users
                    .Include(u => u.UserPermissions)
                    .FirstOrDefaultAsync(u => u.Id == userId);

                if (user == null)
                {
                    _logger.LogWarning(
                        "Nie znaleziono użytkownika o ID {UserId}", userId);
                    return false;
                }

                context.UserPermissions.RemoveRange(user.UserPermissions);
                context.Users.Remove(user);

                await context.SaveChangesAsync();

                _logger.LogInformation(
                    "Usunięto użytkownika o ID {UserId}", userId);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Błąd usuwania użytkownika o ID {UserId}", userId);
                return false;
            }
        }
    }
}
