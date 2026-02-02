using FlightDocSystem.Service;
using FlightDocSystem.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FlightDocSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class RoleController : ControllerBase
    {
        private readonly IRoleSVC _roleSVC;

        public RoleController(IRoleSVC roleSVC)
        {
            _roleSVC = roleSVC;
        }
        private bool IsAdmin()
        {
            // Check tên Role là "Admin"
            var roleName = User.FindFirstValue(ClaimTypes.Role);
            if (roleName == "Admin") return true;

            // Hoặc check permission quản lý
            if (User.HasClaim(c => c.Type == "permission" && (c.Value == "SYSTEM_ADMIN")))
                return true;

            return false;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            if (!IsAdmin()) return Forbid("Chỉ Admin mới được xem danh sách Role!");
            return Ok(await _roleSVC.GetAllRoleAsync());
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateRoleRequest request)
        {
            if (!IsAdmin()) return Forbid("Chỉ Admin mới được tạo Role!");

            try
            {
                await _roleSVC.CreateAsync(request);
                return Ok(new { message = "Tạo Role thành công" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{RoleId}")]
        public async Task<IActionResult> UpdateRoleRequest(int RoleId, [FromBody] UpdateRoleRequest request)
        {
            if (!IsAdmin()) return Forbid("Chỉ Admin mới được sửa Role!");

            try
            {
                await _roleSVC.UpdateAsync(RoleId, request);
                return Ok(new { message = "Cập nhật Role thành công" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("assign-permissions")]
        public async Task<IActionResult> AssignPermissions([FromBody] AssignPermissionRequest request)
        {
            if (!IsAdmin()) return Forbid("Chỉ Admin mới được phân quyền!");

            try
            {
                await _roleSVC.AssignPermissionsAsync(request.RoleId, request.PermissionIds);
                return Ok(new { message = "Phân quyền thành công" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{RoleId}")]
        public async Task<IActionResult> DeleleRoleRequest(int RoleId)
        {
            if (!IsAdmin()) return Forbid("Chỉ Admin mới được xóa Role!");

            try
            {
                await _roleSVC.DeleteAsync(RoleId);
                return Ok(new { message = "Xóa Role thành công" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}