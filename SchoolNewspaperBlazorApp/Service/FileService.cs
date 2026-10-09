using Microsoft.AspNetCore.Components.Forms;
using SchoolNewspaperBlazorApp.Data;
using SchoolNewspaperBlazorApp.Interfaces.Service;
using SchoolNewspaperBlazorApp.Interfaces.Repository;
using System.Net.Mail;

namespace SchoolNewspaperBlazorApp.Service
{
    public class FileService : IFileService
    {
        private IFileRepository _fileRepository;
        public FileService(IFileRepository fileRepository)
        {
            _fileRepository = fileRepository;
        }
        private readonly string uploadPath = @"C:\Users\Uczeń2026\source\repos\SchoolNewspaperBlazorApp\Images";
        public async Task<string> GetPreviewAsync(IBrowserFile file)
        {
            using var stream = file.OpenReadStream(5 * 1024 * 1024);
            using var memoryStream = new MemoryStream();
            await stream.CopyToAsync(memoryStream);
            byte[] bytes = memoryStream.ToArray();
            return $"data:{file.ContentType};base64,{Convert.ToBase64String(bytes)}";
        }
        public async Task<int> UploadImage(IBrowserFile file)
        {
            //Tworzenie folderu
            Directory.CreateDirectory(uploadPath);
            //Potwierdzenie, że plik jest obrazem
            string extension = Path.GetExtension(file.Name);
            //Generownaie unikalnej nazwy pliku
            string fileName = $"{Guid.NewGuid()}{extension}";
            //łączenie ścieżki do folderu z nazwą pliku
            string fullPath = Path.Combine(uploadPath, fileName);

            using var stream = file.OpenReadStream(5 * 1024 * 1024);
            using var fileStream = new FileStream(fullPath, FileMode.Create);
            await stream.CopyToAsync(fileStream);
            var id = await _fileRepository.GetLastFileID();
            var mediaFile = new MediaFile
            {
               Id = id,
               FileName = fileName,
                FileType = extension
            };
            await _fileRepository.AddFileAsync(mediaFile);
            return id;
        }
        public async Task<string> GetFileNameByIdAsync(int id)
        {
            var mediaFile = await _fileRepository.GetFileByIdAsync(id);
            return mediaFile.FileName;
        }
    }

}