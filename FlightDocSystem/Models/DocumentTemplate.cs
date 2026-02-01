using System.ComponentModel.DataAnnotations;

namespace FlightDocSystem.Models
{
    public class DocumentTemplate
    {
        public Guid Id { get; set; }

        [MaxLength(100)]
        public string Name { get; set; }

        [MaxLength(50)]
        public string Category { get; set; }   // Word / Excel / PDF

        [Required]
        [MaxLength(500)]
        public string FileUrl { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
