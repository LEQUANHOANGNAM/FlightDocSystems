using System.ComponentModel.DataAnnotations;

namespace FlightDocSystem.Models
{
    public class FlightDocuments
    {
        public Guid Id { get; set; }
        public Guid FlightId { get; set; }
        public Flights Flight { get; set; }
        public Guid CategoryId { get; set; }
        public DocumentCategory Category { get; set; }
        [MaxLength(200)]
        public string Title { get; set; }
        public ICollection<DocumentFile> Files { get; set; }
    }
}
