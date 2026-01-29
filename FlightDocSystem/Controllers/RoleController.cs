using FlightDocSystem.Service;
using FlightDocSystem.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FlightDocSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class RoleController : ControllerBase
    {
        private readonly IRoleSVC _roleSVC;
        public RoleController(IRoleSVC roleSVC)
        {
            _roleSVC = roleSVC;
        }
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _roleSVC.GetAllRoleAsync());
        }
        [HttpPost]
        public  async Task<IActionResult> Create([FromBody] CreateRoleRequest request)
        {
            try
            {
                await _roleSVC.CreateAsync(request);
                return Ok(new { message = "Tạo vai trò thành công" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        [HttpPut("{RoleId}")]
        public async Task<IActionResult> UpdateRoleRequest(int RoleId, [FromBody] UpdateRoleRequest request)
        {
            try
            {
                await _roleSVC.UpdateAsync(RoleId, request);

                return Ok(new { message = "Cập nhật vai trò thành công" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        [HttpPost("assign-permissions")]
        public async Task<IActionResult> AssignPermissions([FromBody] AssignPermissionRequest request)
        {
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
            try
            {
                await _roleSVC.DeleteAsync(RoleId);
                return Ok(new { message = "Xóa vai trò thành công" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
