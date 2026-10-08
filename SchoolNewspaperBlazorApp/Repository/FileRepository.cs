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
            return id++;

        }
        public async Task AddFileAsync(Data.MediaFile file) 
        {
            await _context.Files.AddAsync(file);
            await _context.SaveChangesAsync();

        }
    }
}
