
using Microsoft.EntityFrameworkCore;
using SchoolNewspaperBlazorApp.Data;
using SchoolNewspaperBlazorApp.Interfaces.Repository;

namespace SchoolNewspaperBlazorApp.Repository
{
    public class SchoolRoleRepository : ISchoolRoleRepository
    {
        private readonly IDbContextFactory<NewspaperDbContext> _contextFactory;
        private readonly ILogger<SchoolRoleRepository> _logger;

        public SchoolRoleRepository(
            IDbContextFactory<NewspaperDbContext> contextFactory,
            ILogger<SchoolRoleRepository> logger)
        {
            _contextFactory = contextFactory;
            _logger = logger;
        }

        public async Task<List<SchoolRole>> GetAllAsync(
            bool includeInactive = false)
        {
            try
            {
                await using var context =
                    await _contextFactory.CreateDbContextAsync();

                var query = context.SchoolRoles.AsNoTracking();

                if (!includeInactive)
                {
                    query = query.Where(role => role.IsActive);
                }

                return await query
                    .OrderBy(role => role.Category)
                    .ThenBy(role => role.Name)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Błąd pobierania ról szkolnych.");

                return new List<SchoolRole>();
            }
        }

        public async Task<SchoolRole?> GetByIdAsync(int id)
        {
            try
            {
                await using var context =
                    await _contextFactory.CreateDbContextAsync();

                return await context.SchoolRoles
                    .AsNoTracking()
                    .FirstOrDefaultAsync(role => role.Id == id);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Błąd pobierania roli o ID {RoleId}.",
                    id);

                return null;
            }
        }

        public async Task<bool> AddAsync(SchoolRole role)
        {
            try
            {
                if (role == null ||
                    string.IsNullOrWhiteSpace(role.Name) ||
                    string.IsNullOrWhiteSpace(role.Category))
                {
                    return false;
                }

                string name = role.Name.Trim();
                string category = role.Category.Trim();
                string description = role.Description?.Trim() ?? string.Empty;

                await using var context =
                    await _contextFactory.CreateDbContextAsync();

                bool exists = await context.SchoolRoles.AnyAsync(r =>
                    r.Name == name &&
                    r.Category == category);

                if (exists)
                {
                    _logger.LogWarning(
                        "Rola {RoleName} już istnieje w kategorii {Category}.",
                        name,
                        category);

                    return false;
                }

                var newRole = new SchoolRole
                {
                    Name = name,
                    Category = category,
                    Description = description,
                    IsActive = true
                };

                await context.SchoolRoles.AddAsync(newRole);
                await context.SaveChangesAsync();

                _logger.LogInformation(
                    "Dodano rolę szkolną o ID {RoleId}.",
                    newRole.Id);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Błąd dodawania roli szkolnej.");

                return false;
            }
        }

        public async Task<bool> UpdateAsync(SchoolRole role)
        {
            try
            {
                if (role == null ||
                    role.Id <= 0 ||
                    string.IsNullOrWhiteSpace(role.Name) ||
                    string.IsNullOrWhiteSpace(role.Category))
                {
                    return false;
                }

                string name = role.Name.Trim();
                string category = role.Category.Trim();
                string description = role.Description?.Trim() ?? string.Empty;

                await using var context =
                    await _contextFactory.CreateDbContextAsync();

                var existing = await context.SchoolRoles
                    .FirstOrDefaultAsync(r => r.Id == role.Id);

                if (existing == null)
                {
                    return false;
                }

                bool duplicate = await context.SchoolRoles.AnyAsync(r =>
                    r.Id != role.Id &&
                    r.Name == name &&
                    r.Category == category);

                if (duplicate)
                {
                    return false;
                }

                existing.Name = name;
                existing.Category = category;
                existing.Description = description;
                existing.IsActive = role.IsActive;

                await context.SaveChangesAsync();

                _logger.LogInformation(
                    "Zaktualizowano rolę szkolną o ID {RoleId}.",
                    role.Id);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Błąd aktualizacji roli o ID {RoleId}.",
                    role?.Id);

                return false;
            }
        }

        public async Task<bool> DeactivateAsync(int id)
        {
            try
            {
                await using var context =
                    await _contextFactory.CreateDbContextAsync();

                var role = await context.SchoolRoles
                    .FirstOrDefaultAsync(r => r.Id == id);

                if (role == null)
                {
                    return false;
                }

                role.IsActive = false;

                await context.SaveChangesAsync();

                _logger.LogInformation(
                    "Dezaktywowano rolę szkolną o ID {RoleId}.",
                    id);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Błąd dezaktywacji roli o ID {RoleId}.",
                    id);

                return false;
            }
        }
    }
}
