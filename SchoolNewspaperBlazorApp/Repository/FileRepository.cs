using SchoolNewspaperBlazorApp.Data;
using SchoolNewspaperBlazorApp.Interfaces.Repository;
using Microsoft.EntityFrameworkCore;


namespace SchoolNewspaperBlazorApp.Repository
{
    public class FileRepository : IFileRepository
    {
        private readonly IDbContextFactory<NewspaperDbContext> _contextFactory;
        public FileRepository(IDbContextFactory<NewspaperDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }
        public async Task<int> GetLastFileID()
        {
            var id = await _contextFactory.CreateDbContext().Files
                .OrderByDescending(f => f.Id)
                .Select(f => f.Id)
                .FirstOrDefaultAsync();
            return ++id;

        }
        public async Task<MediaFile> GetFileByIdAsync(int id)
        {
            var fileId = await _contextFactory.CreateDbContext().Files.FirstOrDefaultAsync(a => a.Id == id);
            if (fileId == null) {
                throw new Exception($"File with ID {id} not found.");
            }
            return fileId;
        }
        public async Task AddFileAsync(MediaFile file) 
        {
            await _contextFactory.CreateDbContext().Files.AddAsync(file);
            await _contextFactory.CreateDbContext().SaveChangesAsync();

        }
    }
}
