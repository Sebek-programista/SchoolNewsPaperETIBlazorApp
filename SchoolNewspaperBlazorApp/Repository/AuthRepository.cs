
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SchoolNewspaperBlazorApp.Data;
using SchoolNewspaperBlazorApp.Interfaces.Repository;

namespace SchoolNewspaperBlazorApp.Repository
{
    public class AuthRepository : IAuthRepository
    {
        private readonly IDbContextFactory<NewspaperDbContext> _factory;
        private readonly ILogger<AuthRepository> _logger;

        public AuthRepository(
            IDbContextFactory<NewspaperDbContext> factory,
            ILogger<AuthRepository> logger)
        {
            _factory = factory;
            _logger = logger;
        }

        public async Task<Dictionary<int, string>> GetAllUsersAsync()
        {
            await using var context = await _factory.CreateDbContextAsync();

            return await context.User
                .Where(u => !u.IsDeleted)
                .ToDictionaryAsync(u => u.Id, u => u.Email);
        }

        public async Task<User?> GetUserByIdAsync(int id)
        {
            await using var context = await _factory.CreateDbContextAsync();

            return await context.User
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == id && !u.IsDeleted);
        }

        public async Task<User?> GetUserByLoginOrEmailAsync(string loginOrEmail)
        {
            await using var context = await _factory.CreateDbContextAsync();

            return await context.User
                .AsNoTracking()
                .FirstOrDefaultAsync(u =>
                    !u.IsDeleted &&
                    (u.Login == loginOrEmail || u.Email == loginOrEmail));
        }

        public async Task DeleteUserByIdAsync(int id)
        {
            await using var context = await _factory.CreateDbContextAsync();

            var user = await context.User
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
            {
                throw new Exception("Konto nie istnieje.");
            }

            user.IsDeleted = true;

            await context.SaveChangesAsync();
        }

        public async Task AddUserAsync(User user)
        {
            await using var context = await _factory.CreateDbContextAsync();

            await context.User.AddAsync(user);
            await context.SaveChangesAsync();
        }

        public async Task<bool> IsLoginOrEmailTakenAsync(
            int excludeUserId,
            string login,
            string email)
        {
            await using var context = await _factory.CreateDbContextAsync();

            return await context.User.AnyAsync(u =>
                u.Id != excludeUserId &&
                (u.Login == login || u.Email == email));
        }

        public async Task UpdateUserAsync(
            int id,
            string login,
            string email,
            string firstName,
            string lastName,
            string? passwordHash)
        {
            await using var context = await _factory.CreateDbContextAsync();

            var user = await context.User
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
            {
                throw new Exception("Konto nie istnieje.");
            }

            user.Login = login;
            user.Email = email;
            user.FirstName = firstName;
            user.LastName = lastName;

            if (passwordHash != null)
            {
                user.Password = passwordHash;
            }

            await context.SaveChangesAsync();
        }

        public async Task SetTemporaryPasswordAsync(
            int userId,
            string temporaryPasswordHash,
            DateTime expiresAt)
        {
            await using var context = await _factory.CreateDbContextAsync();

            var user = await context.User
                .FirstOrDefaultAsync(u => u.Id == userId && !u.IsDeleted);

            if (user == null)
            {
                throw new Exception("Konto nie istnieje.");
            }

            user.Password = temporaryPasswordHash;
            user.MustChangePassword = true;
            user.TemporaryPasswordExpiresAt = expiresAt;

            await context.SaveChangesAsync();
        }

        public async Task ResetPasswordAsync(
            int userId,
            string newPasswordHash)
        {
            await using var context = await _factory.CreateDbContextAsync();

            var user = await context.User
                .FirstOrDefaultAsync(u => u.Id == userId && !u.IsDeleted);

            if (user == null)
            {
                throw new Exception("Konto nie istnieje.");
            }

            user.Password = newPasswordHash;
            user.MustChangePassword = false;
            user.TemporaryPasswordExpiresAt = null;

            await context.SaveChangesAsync();
        }

        public async Task<User> CheckIfRegisteredAsync(
            string login,
            string password)
        {
            await using var context = await _factory.CreateDbContextAsync();

            var user = await context.User
                .AsNoTracking()
                .FirstOrDefaultAsync(u =>
                    !u.IsDeleted &&
                    (u.Login == login || u.Email == login));

            if (user == null)
            {
                throw new Exception("Konto nie istnieje.");
            }

            return user;
        }
    }
}
