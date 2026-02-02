using System.ComponentModel.DataAnnotations;

namespace FlightDocSystem.ViewModels
{
    public class UserRequest
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string RoleName { get; set; }
        public int? RoleId { get; set; }
        public bool? IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }
    public class CreateUserRequest
    {
        [Required]
        public string FullName { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public int RoleId { get; set; }
    }
    public class UpdateUserRequest
    {
        public string FullName { get; set; }
        public IFormFile? Avatar { get; set; } 
        public int? RoleId { get; set; }
        public bool? IsActive { get; set; }
    }
    public class ChangePasswordRequest
    {
        [Required]
        public string OldPassword { get; set; }
        [Required]
        [MinLength(6)]
        public string NewPassword { get; set; }
        [Required]
        [Compare("NewPassword")]
        public string ConfirmPassword { get; set; }
    }
}
