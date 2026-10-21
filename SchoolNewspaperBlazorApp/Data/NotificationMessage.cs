using SchoolNewspaperBlazorApp.Data.Emuns;

namespace SchoolNewspaperBlazorApp.Data
{
    public class NotificationMessage
    {
        public string? Message { get; set; }
        public EnumNotificationType? Type { get; set; }
    }
}
