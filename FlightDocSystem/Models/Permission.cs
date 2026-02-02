using System.ComponentModel.DataAnnotations;

namespace FlightDocSystem.Models
{
    public class Permission
    {
        [Key]
        public int Id { get; set; }
        [MaxLength(50)]
        public string Code { get; set; }
        [MaxLength(100)]
        public string Description { get; set; }
    }
}
