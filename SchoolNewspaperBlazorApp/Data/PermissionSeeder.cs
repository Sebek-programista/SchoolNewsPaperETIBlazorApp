
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace SchoolNewspaperBlazorApp.Data.Seeders
{
    public static class PermissionSeeder
    {
        public static async Task SeedAsync(
            IDbContextFactory<NewspaperDbContext> contextFactory)
        {
            await using var context =
                await contextFactory.CreateDbContextAsync();

            var permissions = new Dictionary<EnumPermissions, string>
            {
                { EnumPermissions.AddArticle, "Dodawanie artykułów" },
                { EnumPermissions.EditArticle, "Edycja artykułów" },
                { EnumPermissions.DeleteArticle, "Usuwanie artykułów" },
                { EnumPermissions.AddUser, "Dodawanie użytkowników" },
                { EnumPermissions.ManageUser, "Zarządzanie użytkownikami" },
                { EnumPermissions.Master, "Master" },
                { EnumPermissions.ManageSchoolData, "Zarządzanie danymi szkoły" },
                { EnumPermissions.ManageSubstitutions, "Zarządzanie zastępstwami" }
            };

            foreach (var item in permissions)
            {
                var existing = await context.Permissions
                    .FirstOrDefaultAsync(
                        p => p.PermissionsName == item.Key);

                if (existing == null)
                {
                    context.Permissions.Add(new Permission
                    {
                        PermissionsName = item.Key,
                        PermissionViewName = item.Value
                    });
                }
                else
                {
                    existing.PermissionViewName = item.Value;
                }
            }

            await context.SaveChangesAsync();

            const string adminEmail = "admin@admin.pl";
            const string adminPassword = "admin";

            var admin = await context.User
                .FirstOrDefaultAsync(u => u.Email == adminEmail);

            if (admin == null)
            {
                var hasher = new PasswordHasher<string>();

                admin = new User
                {
                    Email = adminEmail,
                    Password = hasher.HashPassword(null!, adminPassword),
                    RegisterDate = DateTime.Now,
                    Login = "admin",
                    Role = new Role { RoleName = "Admin" }
                };

                context.User.Add(admin);
                await context.SaveChangesAsync();
            }

            int masterPermissionId = await context.Permissions
                .Where(p => p.PermissionsName == EnumPermissions.Master)
                .Select(p => p.Id)
                .SingleAsync();

            bool masterExists = await context.UserPermissions
                .IgnoreQueryFilters()
                .AnyAsync(up =>
                    up.UserId == admin.Id &&
                    up.PermissionId == masterPermissionId);

            if (!masterExists)
            {
                context.UserPermissions.Add(new UserPermission
                {
                    UserId = admin.Id,
                    PermissionId = masterPermissionId,
                    PermissionName = "Administrator"
                });

                await context.SaveChangesAsync();
            }
        }
    }
}
