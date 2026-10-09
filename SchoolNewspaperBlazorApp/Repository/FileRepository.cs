using SchoolNewspaperBlazorApp.Data;
using SchoolNewspaperBlazorApp.Interfaces.Repository;
using Microsoft.EntityFrameworkCore;


namespace SchoolNewspaperBlazorApp.Repository
{
    public class FileRepository : IFileRepository
    {
        private readonly NewspaperDbContext _context;
        public FileRepository(NewspaperDbContext context)
        {
            _context = context;
        }
        public async Task<int> GetLastFileID()
        {
            var id = await _context.Files
                .OrderByDescending(f => f.Id)
                .Select(f => f.Id)
                .FirstOrDefaultAsync();
            return ++id;

        }
        public async Task<MediaFile> GetFileByIdAsync(int id)
        {
            var fileId = await _context.Files.FirstOrDefaultAsync(a => a.Id == id);
            if (fileId == null) {
                throw new Exception($"File with ID {id} not found.");
            }
            return fileId;
        }
        public async Task AddFileAsync(MediaFile file) 
        {
            await _context.Files.AddAsync(file);
            await _context.SaveChangesAsync();

        }
    }
}
