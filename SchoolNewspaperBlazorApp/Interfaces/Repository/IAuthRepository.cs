
using SchoolNewspaperBlazorApp.Data;

namespace SchoolNewspaperBlazorApp.Interfaces.Repository
{
    public interface IAuthRepository
    {
        Task AddUserAsync(User user);

        Task<User> CheckIfRegisteredAsync(string login, string password);

        Task<Dictionary<int, string>> GetAllUsersAsync();

        Task<User?> GetUserByIdAsync(int id);

        Task DeleteUserByIdAsync(int id);

        Task<bool> IsLoginOrEmailTakenAsync(int excludeUserId, string login, string email);

        Task UpdateUserAsync(
            int id,
            string login,
            string email,
            string firstName,
            string lastName,
            string? passwordHash);

        // Pobranie użytkownika po loginie lub adresie e-mail.
        Task<User?> GetUserByLoginOrEmailAsync(string loginOrEmail);

        // Ustawienie hasła tymczasowego i czasu jego wygaśnięcia.
        Task SetTemporaryPasswordAsync(
            int userId,
            string temporaryPasswordHash,
            DateTime expiresAt);

        // Ustawienie nowego hasła i wyłączenie trybu zmiany hasła.
        Task ResetPasswordAsync(
            int userId,
            string newPasswordHash);
    }
}
