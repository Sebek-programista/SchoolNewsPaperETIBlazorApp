using SchoolNewspaperBlazorApp.Data;

namespace SchoolNewspaperBlazorApp.Interfaces.Repository
{
    public interface IFileRepository
    {
        Task<int> GetLastFileID();

        Task AddFileAsync(Data.MediaFile file);
        Task<MediaFile> GetFileByIdAsync(int id);
    }
}
