using Microsoft.AspNetCore.Components.Forms;

namespace SchoolNewspaperBlazorApp.Interfaces.Service
{
    public interface IFileService
    {
        Task<string> GetPreviewAsync(IBrowserFile file);
        Task UploadImage(IBrowserFile file);

    }
}
