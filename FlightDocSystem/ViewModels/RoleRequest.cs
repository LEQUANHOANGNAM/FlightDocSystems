using System.ComponentModel.DataAnnotations;

namespace FlightDocSystem.ViewModels
{
    public class RoleRequest
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public List<string> Permissions { get; set; }
    }

    public class CreateRoleRequest
    {
        [Required(ErrorMessage = "Tên vai trò là bắt buộc")]
        public string Name { get; set; }
        public string Description { get; set; }
    }

    public class UpdateRoleRequest
    {
        [Required]
        public string Name { get; set; }
        public string Description { get; set; }
    }

    // Dùng để Gán quyền
    public class AssignPermissionRequest
    {
        [Required]
        public int RoleId { get; set; }
        public List<int> PermissionIds { get; set; }
    }
}
