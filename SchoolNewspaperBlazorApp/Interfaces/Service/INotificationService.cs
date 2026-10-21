using SchoolNewspaperBlazorApp.Data;
using SchoolNewspaperBlazorApp.Data.Emuns;

namespace SchoolNewspaperBlazorApp.Interfaces.Service
{
    public interface INotificationService
    {
        void Show(string message, EnumNotificationType type);
        bool TryDequeue(out NotificationMessage? message);
        event Action? OnChange;
    }
}
