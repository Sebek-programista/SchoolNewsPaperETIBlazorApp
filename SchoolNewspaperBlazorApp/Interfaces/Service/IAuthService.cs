
using SchoolNewspaperBlazorApp.Data;

namespace SchoolNewspaperBlazorApp.Interfaces.Service
{
    public interface IAuthService
    {
        Task AddUserAsync(string password, string email);

        Task<bool> AddUserAsync(
            string password,
            string email,

            int? schoolRoleId);

        Task<int> CheckIfRegisteredAsync(
            string login,
            string password);

        Task<Dictionary<int, string>> GetAllUsersAsync();

        Task<bool> RemoveUserByIdAsync(int userId);
    }
}
