
using Microsoft.EntityFrameworkCore;
using SchoolNewspaperBlazorApp.Data;
using SchoolNewspaperBlazorApp.Data.Enums;
using SchoolNewspaperBlazorApp.Interfaces.Repository;
using System.Security.Claims;

namespace SchoolNewspaperBlazorApp.Repository
{
    public class PermissionRepository : IPermissionRepository
    {
        private readonly IDbContextFactory<NewspaperDbContext> _contextFactory;
        private readonly ILogger<PermissionRepository> _logger;

        public PermissionRepository(
            IDbContextFactory<NewspaperDbContext> contextFactory,
            ILogger<PermissionRepository> logger)
        {
            _contextFactory = contextFactory;
            _logger = logger;
        }

        public async Task<List<EnumPermissions>> GetUserPermissionsAsync(
            int userId)
        {
            try
            {
                await using var context =
                    await _contextFactory.CreateDbContextAsync();

                return await context.UserPermissions
                    .Where(up => up.UserId == userId)
                    .Select(up => up.Permission.PermissionName)
                    .Distinct()
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Błąd pobierania uprawnień użytkownika {UserId}", userId);
                return new List<EnumPermissions>();
            }
        }

        public async Task<Dictionary<EnumPermissions, bool>>
            GetAllUserPermissionsAsync(int userId)
        {
            try
            {
                var permissions = await GetUserPermissionsAsync(userId);

                return Enum.GetValues<EnumPermissions>()
                    .ToDictionary(
                        p => p,
                        p => permissions.Contains(EnumPermissions.Master)
                             || permissions.Contains(p));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Błąd pobierania listy uprawnień użytkownika {UserId}",
                    userId);

                return Enum.GetValues<EnumPermissions>()
                    .ToDictionary(p => p, p => false);
            }
        }

        public async Task<bool> HasPermissionAsync(
            ClaimsPrincipal principal,
            EnumPermissions permission)
        {
            try
            {
                var id = principal
                    .FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (!int.TryParse(id, out int userId) || userId <= 0)
                {
                    _logger.LogWarning(
                        "Nie można ustalić ID użytkownika z ClaimsPrincipal.");
                    return false;
                }

                await using var context =
                    await _contextFactory.CreateDbContextAsync();

                return await context.UserPermissions.AnyAsync(up =>
                    up.UserId == userId &&
                    (up.Permission.PermissionName == permission ||
                     up.Permission.PermissionName == EnumPermissions.Master));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Błąd sprawdzania uprawnienia {Permission}", permission);
                return false;
            }
        }

        public async Task<bool> SavePermissionsAsync(
            List<EnumPermissions> permissions, int userId)
        {
            try
            {
                if (userId <= 0)
                    return false;

                await using var context =
                    await _contextFactory.CreateDbContextAsync();

                bool userExists = await context.Users
                    .AnyAsync(u => u.Id == userId);

                if (!userExists)
                {
                    _logger.LogWarning(
                        "Nie znaleziono użytkownika {UserId} do zmiany uprawnień.",
                        userId);
                    return false;
                }

                int masterId = await context.Permissions
                    .Where(p => p.PermissionName == EnumPermissions.Master)
                    .Select(p => p.Id)
                    .FirstOrDefaultAsync();

                var existing = await context.UserPermissions
                    .Where(up =>
                        up.UserId == userId &&
                        up.PermissionId != masterId)
                    .ToListAsync();

                context.UserPermissions.RemoveRange(existing);

                var requested = permissions
                    .Where(p => p != EnumPermissions.Master)
                    .Distinct()
                    .ToList();

                var records = await context.Permissions
                    .Where(p => requested.Contains(p.PermissionName))
                    .Select(p => new
                    {
                        p.Id,
                        p.PermissionName
                    })
                    .ToListAsync();

                foreach (var permission in records)
                {
                    context.UserPermissions.Add(new UserPermission
                    {
                        UserId = userId,
                        PermissionId = permission.Id,
                        PermissionName = permission.PermissionName.ToString()
                    });
                }

                await context.SaveChangesAsync();

                _logger.LogInformation(
                    "Zapisano uprawnienia użytkownika {UserId}", userId);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Błąd zapisywania uprawnień użytkownika {UserId}", userId);
                return false;
            }
        }
    }
}
