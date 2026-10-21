using Microsoft.AspNetCore.Components.Authorization;
using SchoolNewspaperBlazorApp.Data.Emuns;
using SchoolNewspaperBlazorApp.Interfaces.Repository;
using SchoolNewspaperBlazorApp.Interfaces.Service;
using System.Security.Claims;

namespace SchoolNewspaperBlazorApp.Service
{
    public class PermissionService : IPermissionService
    {
        private readonly IPermissionRepository _permissionRepository;
        private readonly AuthenticationStateProvider _asp;
        public PermissionService(IPermissionRepository permissionRepository, AuthenticationStateProvider asp)
        {
            _permissionRepository = permissionRepository;
            _asp = asp;
        }

        public async Task<List<EnumPermissions>> GetUserPermissionsAsync(int userId)
        {
            return await _permissionRepository.GetUserPermissionsAsync(userId);
        }
        public async Task<Dictionary<EnumPermissions,bool>> GetAllPermissionsAsync()
        {
            var id = await GetUserId();
            return await _permissionRepository.GetAllUserPermissionsAsync(id);
        }
        public async Task<bool> HasPermissionAsync(EnumPermissions permission)
        {
            var authState = await _asp.GetAuthenticationStateAsync();
            var principal = authState.User;
            return await _permissionRepository.HasPermissionAsync(principal, permission);
        }
        public async Task SavePermissionsAsync(List<EnumPermissions> permissions, int userId)
        {

            bool hasPermission = await HasPermissionAsync(EnumPermissions.Master);
            if (!hasPermission) return;
            await _permissionRepository.SavePermissionsAsync(permissions, userId);
        }
        public async Task<int> GetUserId()
        {
            var authState = await _asp.GetAuthenticationStateAsync();
            var principal = authState.User;
            var stringId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (int.TryParse(stringId, out int id))
            {
                return id;
            }

            return 0;
        }

    }
}
