using System.ComponentModel.DataAnnotations;

namespace FlightDocSystem.Models
{
    public class DocumentFile
    {
        public Guid Id { get; set; }

        public Guid FlightDocumentId { get; set; }
        public FlightDocuments FlightDocument { get; set; }
        [MaxLength(255)]
        public string FileName { get; set; }
        [MaxLength(255)]
        public string FilePath { get; set; }
        public long FileSize { get; set; }
        public DateTime UploadedAt { get; set; }
    }
}