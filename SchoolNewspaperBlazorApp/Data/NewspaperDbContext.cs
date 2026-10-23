using Microsoft.EntityFrameworkCore;

namespace SchoolNewspaperBlazorApp.Data
{
    public class NewspaperDbContext : DbContext
    {
        public NewspaperDbContext(
            DbContextOptions<NewspaperDbContext> options)
            : base(options)
        {
        }

        public DbSet<Article> Articles { get; set; }

        public DbSet<User> Users { get; set; }

        public DbSet<MediaFile> Files { get; set; }

        public DbSet<Permission> Permissions { get; set; }

        public DbSet<UserPermission> UserPermissions { get; set; }

        public DbSet<SchoolRole> SchoolRoles { get; set; }

        protected override void OnModelCreating(ModelBuilder mb)
        {
            base.OnModelCreating(mb);

            // Konfiguracja plików multimedialnych
            mb.Entity<MediaFile>()
                .Property(x => x.Id)
                .ValueGeneratedNever();

            // Klucz złożony dla uprawnień użytkowników
            mb.Entity<UserPermission>()
                .HasKey(up => new
                {
                    up.UserId,
                    up.PermissionId
                });

            // Relacja użytkownika z uprawnieniami
            mb.Entity<UserPermission>()
                .HasOne(up => up.User)
                .WithMany(u => u.UserPermissions)
                .HasForeignKey(up => up.UserId);

            // Relacja uprawnienia z użytkownikami
            mb.Entity<UserPermission>()
                .HasOne(up => up.Permission)
                .WithMany(p => p.Permissions)
                .HasForeignKey(up => up.PermissionId);

            // Relacja użytkownika z rolą szkolną
            mb.Entity<User>()
                .HasOne(u => u.SchoolRole)
                .WithMany(r => r.Users)
                .HasForeignKey(u => u.SchoolRoleId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.SetNull);

            // Konfiguracja roli szkolnej
            mb.Entity<SchoolRole>()
                .Property(r => r.Name)
                .IsRequired()
                .HasMaxLength(100);

            mb.Entity<SchoolRole>()
                .Property(r => r.Category)
                .IsRequired()
                .HasMaxLength(100);

            mb.Entity<SchoolRole>()
                .Property(r => r.Description)
                .HasMaxLength(500);

            mb.Entity<SchoolRole>()
                .Property(r => r.IsActive)
                .HasDefaultValue(true);
        }
    }
}