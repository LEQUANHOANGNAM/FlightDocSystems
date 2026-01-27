using System.ComponentModel.DataAnnotations;

namespace FlightDocSystem.Models
{
    public class DocumentCategory
    {
        public Guid Id { get; set; }
        [MaxLength(10)]
        public string Code { get; set; }    
        [MaxLength(50)]
        public string Name { get; set; }  
        [MaxLength(200)]
        public string Description { get; set; }

        public bool IsActive { get; set; }

        public ICollection<FlightDocuments> FlightDocuments { get; set; }
    }
}