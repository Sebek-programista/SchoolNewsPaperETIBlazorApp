
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

        public DbSet<Article> Articles { get; set; } = null!;
        public DbSet<MediaFile> Files { get; set; } = null!;
        public DbSet<User> User { get; set; } = null!;
        public DbSet<UserPermission> UserPermissions { get; set; } = null!;
        public DbSet<Permission> Permissions { get; set; } = null!;
        public DbSet<ArticleImage> Images { get; set; } = null!;
        public DbSet<Role> Roles { get; set; } = null!;

        public DbSet<SchoolClass> SchoolClasses { get; set; } = null!;
        public DbSet<Teacher> Teachers { get; set; } = null!;
        public DbSet<SchoolRoom> SchoolRooms { get; set; } = null!;
        public DbSet<SchoolSubject> SchoolSubjects { get; set; } = null!;
        public DbSet<Substitution> Substitutions { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder mb)
        {
            base.OnModelCreating(mb);

            mb.Entity<MediaFile>()
                .Property(x => x.Id)
                .UseIdentityColumn();

            mb.Entity<UserPermission>()
                .HasKey(x => new { x.UserId, x.PermissionId });

            mb.Entity<UserPermission>()
                .HasOne(x => x.User)
                .WithMany(x => x.UserPermissions)
                .HasForeignKey(x => x.UserId);

            mb.Entity<UserPermission>()
                .HasOne(x => x.Permission)
                .WithMany(x => x.Permissions)
                .HasForeignKey(x => x.PermissionId);

            mb.Entity<Role>()
                .HasOne(r => r.User)
                .WithOne(u => u.Role)
                .HasForeignKey<Role>(r => r.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            mb.Entity<Role>()
                .Property(r => r.RoleName)
                .IsRequired()
                .HasMaxLength(Role.MaxNameLength);

            mb.Entity<Article>()
                .HasOne(a => a.User)
                .WithMany()
                .HasForeignKey(a => a.UserId)
                .OnDelete(DeleteBehavior.SetNull);

            // Klasa i nauczyciele
            mb.Entity<SchoolClass>()
                .HasIndex(x => x.Name)
                .IsUnique();

            mb.Entity<Teacher>()
                .HasIndex(x => new { x.FirstName, x.LastName });

            // Sale i przedmioty
            mb.Entity<SchoolRoom>()
                .HasIndex(x => x.Name)
                .IsUnique();

            mb.Entity<SchoolSubject>()
                .HasIndex(x => x.Name)
                .IsUnique();

            // Zastępstwo -> klasa
            mb.Entity<Substitution>()
                .HasOne(x => x.SchoolClass)
                .WithMany()
                .HasForeignKey(x => x.SchoolClassId)
                .OnDelete(DeleteBehavior.Restrict);

            // Nauczyciel nieobecny
            mb.Entity<Substitution>()
                .HasOne(x => x.AbsentTeacher)
                .WithMany()
                .HasForeignKey(x => x.AbsentTeacherId)
                .OnDelete(DeleteBehavior.Restrict);

            // Nauczyciel zastępujący
            mb.Entity<Substitution>()
                .HasOne(x => x.SubstituteTeacher)
                .WithMany()
                .HasForeignKey(x => x.SubstituteTeacherId)
                .OnDelete(DeleteBehavior.Restrict);

            // Przedmiot przed zmianą
            mb.Entity<Substitution>()
                .HasOne(x => x.OriginalSubject)
                .WithMany()
                .HasForeignKey(x => x.OriginalSubjectId)
                .OnDelete(DeleteBehavior.Restrict);

            // Przedmiot po zmianie
            mb.Entity<Substitution>()
                .HasOne(x => x.SubstituteSubject)
                .WithMany()
                .HasForeignKey(x => x.SubstituteSubjectId)
                .OnDelete(DeleteBehavior.Restrict);

            // Sala przed zmianą
            mb.Entity<Substitution>()
                .HasOne(x => x.OriginalRoom)
                .WithMany()
                .HasForeignKey(x => x.OriginalRoomId)
                .OnDelete(DeleteBehavior.Restrict);

            // Sala po zmianie
            mb.Entity<Substitution>()
                .HasOne(x => x.SubstituteRoom)
                .WithMany()
                .HasForeignKey(x => x.SubstituteRoomId)
                .OnDelete(DeleteBehavior.Restrict);

            mb.Entity<Substitution>()
                .HasIndex(x => new { x.Date, x.LessonNumber });

            // Istniejące filtry soft delete
            mb.Entity<User>()
                .HasQueryFilter(u => !u.IsDeleted);

            mb.Entity<Article>()
                .HasQueryFilter(a => !a.IsDeleted);

            mb.Entity<Role>()
                .HasQueryFilter(r => !r.User.IsDeleted);

            mb.Entity<UserPermission>()
                .HasQueryFilter(up => !up.User.IsDeleted);
        }
    }
}
