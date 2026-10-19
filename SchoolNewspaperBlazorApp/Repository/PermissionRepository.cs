using Microsoft.EntityFrameworkCore;
using SchoolNewspaperBlazorApp.Data;
using SchoolNewspaperBlazorApp.Interfaces.Repository;
using System.Security.Claims;

namespace SchoolNewspaperBlazorApp.Repository
{
    public class PermissionRepository : IPermissionRepository
    {
        private readonly IDbContextFactory<NewspaperDbContext> _contextFactory;
        public PermissionRepository(IDbContextFactory<NewspaperDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }
        public async Task<bool> HasPermissionAsync(ClaimsPrincipal principal, EnumPermissions permission)
        {

            var id = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(id))
            {
                return false;
            }
            if(!int.TryParse(id, out var permissionId))
            {
                return false;
            }
            await using var _context = await _contextFactory.CreateDbContextAsync();
            return await _context.UserPermissions
                .AnyAsync(up => up.UserId == permissionId &&
                up.Permission.PermissionName == permission);
        }
        public  async Task SavePermissionsync(List<EnumPermissions> permissions, int userId)
        {
            await using var _context = await _contextFactory.CreateDbContextAsync();
            var existingPermissions = _context.UserPermissions
                .Where(up => up.UserId == userId);
            _context.UserPermissions.RemoveRange(existingPermissions);
            foreach (var permission in permissions) 
            {
                var permissionsId = await _context.Permissions
                    .Where(p => p.PermissionName == permission)
                    .Select(p => p.Id)
                    .FirstOrDefaultAsync();
                if (permissionsId == 0)
                    continue;
                _context.UserPermissions.Add(new UserPermission
                {
                    UserId = userId,
                    PermissionId = permissionsId
                });
                await _context.SaveChangesAsync();
            }
        }
    }
}
