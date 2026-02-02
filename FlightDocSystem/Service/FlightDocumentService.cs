using FlightDocSystem.Data;
using FlightDocSystem.Models;
using FlightDocSystem.ViewModels;
using System.IO.Compression;

namespace FlightDocSystem.Service
{
    public class FlightDocumentService:IFlightDocumentService
    {
        private readonly AppDbContext _context;
        private readonly IFileStorageService _fileStorage;
        private readonly IWebHostEnvironment _env;

        public FlightDocumentService(AppDbContext context, IWebHostEnvironment env,IFileStorageService fileStorage)
        {
            _context = context;
            _fileStorage = fileStorage;
            _env = env;
        }
        public List<FlightDocument> GetByFlight(Guid flightId)
        {
            return _context.FlightDocuments.Where(fd => fd.FlightId == flightId)
                .Select(fd => new FlightDocument
                {
                    Id = fd.Id,
                    Title = fd.Title,
                    CategoryName = fd.Category.Name
                })
                .ToList();
        }

        public void Upload(Guid flightId, UploadDocument dto)
        {
            var document = new FlightDocuments
            {
                Id = Guid.NewGuid(),
                FlightId = flightId,
                CategoryId = dto.CategoryId,
                Title = dto.Title
            };

            _context.FlightDocuments.Add(document);
            _context.SaveChanges();

            foreach (var file in dto.Files)
            {
                var filePath = _fileStorage.SaveFile(file);

                _context.DocumentFiles.Add(new DocumentFile
                {
                    Id = Guid.NewGuid(),
                    FlightDocumentId = document.Id,
                    FileName = file.FileName,
                    FilePath = filePath,
                    FileSize = file.Length,
                    UploadedAt = DateTime.UtcNow
                });
            }

            _context.SaveChanges();
        }


        public byte[] DownloadZip(Guid documentId)
        {
            var files = _context.DocumentFiles
            .Where(f => f.FlightDocumentId == documentId)
            .ToList();

            using var ms = new MemoryStream();
            using var zip = new ZipArchive(ms, ZipArchiveMode.Create, true);

            foreach (var file in files)
            {
                var entry = zip.CreateEntry(file.FileName);
                using var entryStream = entry.Open();
                entryStream.Write(File.ReadAllBytes(file.FilePath));
            }

            return ms.ToArray();
        }
        public List<FlightDocumentFileDto> GetFilesForUser(
    Guid flightId,
    int userId)
        {
            // 1. Kiểm tra user có được phân công flight không
            var isAssigned = _context.FlightAssigments
                .Any(a => a.FlightId == flightId && a.UserId == userId);

            if (!isAssigned)
                throw new UnauthorizedAccessException("User is not assigned to this flight");

            // 2. Lấy danh sách file của flight
            var files = _context.DocumentFiles
                .Where(f => f.FlightDocument.FlightId == flightId)
                .Select(f => new FlightDocumentFileDto
                {
                    DocumentId = f.FlightDocumentId,
                    DocumentTitle = f.FlightDocument.Title,
                    CategoryName = f.FlightDocument.Category.Name,
                    FileName = f.FileName,
                    FileSize = f.FileSize,
                    UploadedAt = f.UploadedAt
                })
                .ToList();

            return files;
        }
    }
}
