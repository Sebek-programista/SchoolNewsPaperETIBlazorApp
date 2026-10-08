namespace SchoolNewspaperBlazorApp.Interfaces.Repository
{
    public interface IFileRepository
    {
        Task<int> GetLastFileID();

        Task AddFileAsync(Data.MediaFile file);
    }
}
