using System.ComponentModel.DataAnnotations;

namespace FlightDocSystem.ViewModels
{
    public class PermissionRequest
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
    }

    public class CreatePermissionRequest
    {
        [Required(ErrorMessage = "Mã quyền là bắt buộc")]
        public string Code { get; set; } // VD: "USER_VIEW"
        public string Description { get; set; }
    }

    public class UpdatePermissionRequest
    {
        [Required(ErrorMessage = "Mã quyền là bắt buộc")]
        public string Code { get; set; }
        public string Description { get; set; }
    }
}
