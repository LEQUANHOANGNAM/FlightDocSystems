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
    public class PermissionController : ControllerBase
    {
        private readonly IPermissionSVC _permissionSVC;

        public PermissionController(IPermissionSVC permissionSVC)
        {
            _permissionSVC = permissionSVC;
        }

        private bool IsAdmin()
        {
            // Cách 1: Check tên Role trực tiếp từ Token
            var roleName = User.FindFirstValue(ClaimTypes.Role);
            if (roleName == "Admin") return true;

            if (User.HasClaim(c => c.Type == "permission" && (c.Value == "SYSTEM_ADMIN")))
                return true;

            return false;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            if (!IsAdmin()) return Forbid("Chỉ Admin mới được xem quyền hạn!");
            return Ok(await _permissionSVC.GetAllPermissionsAsync());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            if (!IsAdmin()) return Forbid("Chỉ Admin mới được xem quyền hạn!");

            var permission = await _permissionSVC.GetByIdAsync(id);
            if (permission == null) return NotFound();

            return Ok(permission);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreatePermissionRequest request)
        {
            if (!IsAdmin()) return Forbid("Chỉ Admin mới được tạo quyền!");

            try
            {
                await _permissionSVC.CreatePermission(request);
                return Ok(new { message = "Tạo quyền thành công" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdatePermissionRequest request)
        {
            if (!IsAdmin()) return Forbid("Chỉ Admin mới được sửa quyền!");

            try
            {
                await _permissionSVC.UpdateAsync(id, request);
                return Ok(new { message = "Cập nhật quyền thành công" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            if (!IsAdmin()) return Forbid("Chỉ Admin mới được xóa quyền!");

            try
            {
                await _permissionSVC.DeleteAsync(id);
                return Ok(new { message = "Xóa quyền thành công" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}