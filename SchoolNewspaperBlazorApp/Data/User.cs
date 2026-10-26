
using System.ComponentModel.DataAnnotations.Schema;

namespace SchoolNewspaperBlazorApp.Data
{
    public class User
    {
        public int Id { get; set; }

        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;

        public required string Login { get; set; }

        public required string Password { get; set; }

        public required string Email { get; set; }

        public required DateTime RegisterDate { get; set; }

        [Column("AddedBy")]
        public string AddBy { get; set; } = string.Empty;

        public bool IsDeleted { get; set; }

        // Czy użytkownik musi ustawić nowe hasło.
        public bool MustChangePassword { get; set; }

        // Termin ważności hasła tymczasowego.
        public DateTime? TemporaryPasswordExpiresAt { get; set; }

        public ICollection<UserPermission> UserPermissions { get; set; }
            = new List<UserPermission>();

        public Role? Role { get; set; }
    }
}
