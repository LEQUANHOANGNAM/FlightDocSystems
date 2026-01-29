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
    public class PermissionController : ControllerBase
    {
        private readonly IPermissionSVC _permissionSVC;
        public PermissionController(IPermissionSVC permissionSVC)
        {
            _permissionSVC = permissionSVC;
        }
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var permissions = await _permissionSVC.GetAllPermissionsAsync();
            return Ok(permissions);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreatePermissionRequest request)
        {
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

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
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
