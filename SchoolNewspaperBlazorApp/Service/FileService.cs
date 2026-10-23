
using Microsoft.AspNetCore.Components.Forms;
using SchoolNewspaperBlazorApp.Data;
using SchoolNewspaperBlazorApp.Data.Enums;
using SchoolNewspaperBlazorApp.Interfaces.Repository;
using SchoolNewspaperBlazorApp.Interfaces.Service;

namespace SchoolNewspaperBlazorApp.Service
{
    public class FileService : IFileService
    {
        private readonly IFileRepository _fileRepository;
        private readonly IPermissionService _permissionService;
        private readonly INotificationService _notificationService;
        private readonly string _uploadPath;

        private const long MaxFileSize = 5 * 1024 * 1024;

        private static readonly HashSet<string> AllowedExtensions =
            new(StringComparer.OrdinalIgnoreCase)
            {
                ".jpg",
                ".jpeg",
                ".png",
                ".webp"
            };

        public FileService(
            IFileRepository fileRepository,
            IWebHostEnvironment environment,
            IPermissionService permissionService,
            INotificationService notificationService)
        {
            _fileRepository = fileRepository;
            _permissionService = permissionService;
            _notificationService = notificationService;

            _uploadPath = Path.Combine(
                environment.ContentRootPath,
                "Images");
        }

        public async Task<string> GetPreviewAsync(IBrowserFile file)
        {
            if (file.Size > MaxFileSize)
            {
                throw new InvalidOperationException(
                    "Maksymalny rozmiar zdjęcia to 5 MB.");
            }

            using var stream = file.OpenReadStream(MaxFileSize);
            using var memoryStream = new MemoryStream();

            await stream.CopyToAsync(memoryStream);

            return $"data:{file.ContentType};base64," +
                   Convert.ToBase64String(memoryStream.ToArray());
        }

        public async Task<int> UploadImage(
            IBrowserFile file,
            int? ArticleId = null)
        {
            bool hasPermission =
                await _permissionService.HasPermissionAsync(
                    EnumPermissions.AddArticle);

            if (!hasPermission)
            {
                _notificationService.Show(
                    "Brak uprawnień!",
                    EnumNotificationType.Failure);

                throw new UnauthorizedAccessException(
                    "Brak uprawnień do dodawania zdjęć.");
            }

            if (file.Size <= 0 || file.Size > MaxFileSize)
            {
                throw new InvalidOperationException(
                    "Plik jest pusty lub przekracza limit 5 MB.");
            }

            string extension = Path.GetExtension(file.Name);

            if (!AllowedExtensions.Contains(extension))
            {
                throw new InvalidOperationException(
                    "Dozwolone formaty zdjęć: JPG, JPEG, PNG i WEBP.");
            }

            Directory.CreateDirectory(_uploadPath);

            string fileName = $"{Guid.NewGuid()}{extension}";
            string fullPath = Path.Combine(_uploadPath, fileName);

            try
            {
                await using (var stream =
                    file.OpenReadStream(MaxFileSize))
                await using (var fileStream = new FileStream(
                    fullPath,
                    FileMode.CreateNew,
                    FileAccess.Write,
                    FileShare.None))
                {
                    await stream.CopyToAsync(fileStream);
                }

                var mediaFile = new MediaFile
                {
                    FileName = fileName,
                    FileType = extension,
                    ArticleId = ArticleId,
                    IsMainImage = ArticleId == null
                };

                await _fileRepository.AddFileAsync(mediaFile);

                return mediaFile.Id;
            }
            catch
            {
                if (File.Exists(fullPath))
                {
                    File.Delete(fullPath);
                }

                throw;
            }
        }

        public async Task<string> GetFileNameByIdAsync(int id)
        {
            var file = await _fileRepository.GetFileByIdAsync(id);

            return file.FileName;
        }

        public async Task<List<string>> GetImagesByArticleIdAsync(
            int articleId)
        {
            var files =
                await _fileRepository.GetFilesByArticleIdAsync(articleId);

            return files
                .Select(file => $"/Images/{file.FileName}")
                .ToList();
        }
    }
}
