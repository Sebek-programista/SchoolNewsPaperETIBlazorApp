using SchoolNewspaperBlazorApp.Data;

namespace SchoolNewspaperBlazorApp.Interfaces.Repository
{
    public interface IAuthRepository
    {
        Task AddUserAsync(string login, string password, string email);
        Task<string> CheckIfRegisteredAsync(string login, string password);
    }
}
