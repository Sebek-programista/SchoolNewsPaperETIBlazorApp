
namespace SchoolNewspaperBlazorApp.Data
{
    public class SchoolRole
    {
        public int Id { get; set; }

        public required string Name { get; set; }

        public required string Category { get; set; }

        public string Description { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;

        public ICollection<User> Users { get; set; }
            = new List<User>();
    }
}
