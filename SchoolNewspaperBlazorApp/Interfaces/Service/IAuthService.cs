
using SchoolNewspaperBlazorApp.Data;

namespace SchoolNewspaperBlazorApp.Interfaces.Service
{
    public interface IAuthService
    {
        Task AddUserAsync(
            string login,
            string password,
            string email,
            string firstName,
            string lastName);

        Task<Dictionary<int, string>> GetAllUsersAsync();

        Task<int> CheckIfRegisteredAsync(string login, string password);

        Task<string> GetFullNameAsync(int userId);

        Task DeleteUserByIdAsync(int id);

        Task<UserDetails?> GetUserDetailsAsync(int id);

        Task UpdateUserAsync(
            int id,
            string login,
            string email,
            string firstName,
            string lastName,
            string? newPassword);

        Task ChangeOwnPasswordAsync(
            string currentPassword,
            string newPassword);

        // Generuje hasło tymczasowe dla użytkownika.
        Task<string> GenerateTemporaryPasswordAsync(int userId);

        // Sprawdza hasło tymczasowe i ustawia nowe hasło.
        Task<bool> ResetPasswordWithTemporaryPasswordAsync(
            string loginOrEmail,
            string temporaryPassword,
            string newPassword);
    }
}
