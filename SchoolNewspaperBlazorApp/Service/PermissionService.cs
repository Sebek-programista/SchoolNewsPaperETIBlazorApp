using Microsoft.AspNetCore.Components.Authorization;
using SchoolNewspaperBlazorApp.Data;
using SchoolNewspaperBlazorApp.Interfaces.Repository;
using SchoolNewspaperBlazorApp.Interfaces.Service;
using System.Security.Claims;

namespace SchoolNewspaperBlazorApp.Service
{
    public class PermissionService : IPermissionService
    {
        private readonly IPermissionRepository _permissionRepository;
        private readonly AuthenticationStateProvider _asp;
        public PermissionService(IPermissionRepository permissionRepository,AuthenticationStateProvider asp)
        {
            _permissionRepository = permissionRepository;
            _asp = asp;
        }
        public async Task<bool> HasPermissionAsync(EnumPermissions permission)
        {
            var authState = await _asp.GetAuthenticationStateAsync();
            var principal = authState.User;
            return await _permissionRepository.HasPermissionAsync(principal, permission);
        }
        public async Task SavePermissionAsync(List<EnumPermissions> permissions, int userId) 
        {
            await _permissionRepository.SavePermissionsync(permissions, userId);
        }
    }
}
