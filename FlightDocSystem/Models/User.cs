using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FlightDocSystem.Models
{
    public class User
    {
        public int Id { get; set; }
        [NotMapped]
        public IFormFile? Avatar { get; set; } 
        public string? AvatarPath { get; set; }
        [MaxLength(100)]
        public string FullName { get; set; }
        [MaxLength(100)]
        public string Email { get; set; }
        [MaxLength(100)]
        public string Password { get; set; }
        public int RoleId { get; set; }
        public Role Role { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }

        public ICollection<FlightAssigment> FlightAssigments { get; set; }= new List<FlightAssigment>();
    }
}
