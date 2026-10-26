
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

        // Pobiera aktywnych użytkowników, z pominięciem kont Master.
        public async Task<Dictionary<int, string>> GetAllUsersAsync()
        {
            await using var context = await _factory.CreateDbContextAsync();

            var users = await context.User
                .AsNoTracking()
                .Where(u => !u.IsDeleted)
                .Where(u => !u.UserPermissions.Any(
                    up => up.Permission.PermissionsName == EnumPermissions.Master))
                .OrderBy(u => u.Email)
                .Select(u => new
                {
                    u.Id,
                    u.Email
                })
                .ToListAsync();

            return users.ToDictionary(u => u.Id, u => u.Email);
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
            if (string.IsNullOrWhiteSpace(loginOrEmail))
                return null;

            await using var context = await _factory.CreateDbContextAsync();

            return await context.User
                .AsNoTracking()
                .FirstOrDefaultAsync(u =>
                    !u.IsDeleted &&
                    (u.Login == loginOrEmail || u.Email == loginOrEmail));
        }

        // Usuwa logicznie konto, nie kasując rekordu z bazy.
        public async Task DeleteUserByIdAsync(int id)
        {
            await using var context = await _factory.CreateDbContextAsync();

            var user = await context.User
                .FirstOrDefaultAsync(u => u.Id == id && !u.IsDeleted);

            if (user == null)
                throw new Exception("Konto nie istnieje.");

            bool isMaster = await context.UserPermissions
                .AnyAsync(up =>
                    up.UserId == id &&
                    up.Permission.PermissionsName == EnumPermissions.Master);

            if (isMaster)
                throw new InvalidOperationException(
                    "Nie można usunąć konta Master.");

            user.IsDeleted = true;

            await context.SaveChangesAsync();
        }

        public async Task AddUserAsync(User user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            await using var context = await _factory.CreateDbContextAsync();

            bool exists = await context.User.AnyAsync(u =>
                u.Login == user.Login || u.Email == user.Email);

            if (exists)
                throw new InvalidOperationException(
                    "Podany login lub adres e-mail jest już zajęty.");

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
                !u.IsDeleted &&
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
                .FirstOrDefaultAsync(u => u.Id == id && !u.IsDeleted);

            if (user == null)
                throw new Exception("Konto nie istnieje.");

            bool duplicate = await context.User.AnyAsync(u =>
                !u.IsDeleted &&
                u.Id != id &&
                (u.Login == login || u.Email == email));

            if (duplicate)
                throw new InvalidOperationException(
                    "Podany login lub adres e-mail jest już zajęty.");

            user.Login = login;
            user.Email = email;
            user.FirstName = firstName;
            user.LastName = lastName;

            if (!string.IsNullOrWhiteSpace(passwordHash))
                user.Password = passwordHash;

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
                throw new Exception("Konto nie istnieje.");

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
                throw new Exception("Konto nie istnieje.");

            user.Password = newPasswordHash;
            user.MustChangePassword = false;
            user.TemporaryPasswordExpiresAt = null;

            await context.SaveChangesAsync();
        }

        // Hasło jest weryfikowane w AuthService, więc ta metoda
        // wyszukuje użytkownika po loginie lub adresie e-mail.
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
                throw new Exception("Konto nie istnieje.");

            return user;
        }
    }
}
