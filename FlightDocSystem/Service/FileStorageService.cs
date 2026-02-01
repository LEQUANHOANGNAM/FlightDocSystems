namespace FlightDocSystem.Service
{
    public class FileStorageService:IFileStorageService
    {
        private readonly IWebHostEnvironment _env;

        public FileStorageService(IWebHostEnvironment env)
        {
            _env = env;
        }

        public string SaveFile(IFormFile file)
        {
            EnsureUploadFolders();

            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            var folderName = GetFolderByExtension(extension);

            if (folderName == null)
                throw new InvalidOperationException("File type not supported");

            var uploadRoot = Path.Combine(_env.ContentRootPath, "Uploads");
            var folderPath = Path.Combine(uploadRoot, folderName);

            var safeFileName = $"{Guid.NewGuid()}{extension}";
            var filePath = Path.Combine(folderPath, safeFileName);

            using var stream = new FileStream(filePath, FileMode.Create);
            file.CopyTo(stream);

            return filePath;
        }

        private void EnsureUploadFolders()
        {
            var root = Path.Combine(_env.ContentRootPath, "Uploads");
            var folders = new[] { "Word", "Excel", "PDF" };

            foreach (var folder in folders)
            {
                var path = Path.Combine(root, folder);
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
            }
        }

        private string? GetFolderByExtension(string extension)
        {
            return extension switch
            {
                ".docx" => "Word",
                ".xlsx" => "Excel",
                ".pdf" => "PDF",
                _ => null
            };
        }
    }
}
