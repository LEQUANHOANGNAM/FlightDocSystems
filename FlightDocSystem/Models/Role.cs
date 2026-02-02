using System.ComponentModel.DataAnnotations;

namespace FlightDocSystem.Models
{
    public class Role
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [MaxLength(20)]
        public string Name { get; set; }
        [MaxLength(100)]
        public string Description { get; set; }

        public ICollection<RolePermission> RolePermissions { get; set; }
    }
}
