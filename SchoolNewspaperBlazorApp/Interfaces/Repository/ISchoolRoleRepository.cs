
using SchoolNewspaperBlazorApp.Data;

namespace SchoolNewspaperBlazorApp.Interfaces.Repository
{
    public interface ISchoolRoleRepository
    {
        Task<List<SchoolRole>> GetAllAsync(
            bool includeInactive = false);

        Task<SchoolRole?> GetByIdAsync(int id);

        Task<bool> AddAsync(SchoolRole role);

        Task<bool> UpdateAsync(SchoolRole role);

        Task<bool> DeactivateAsync(int id);
    }
}
