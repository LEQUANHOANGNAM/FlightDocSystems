using FlightDocSystem.Service;
using FlightDocSystem.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FlightDocSystem.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserSVC _userSVC;

        public UserController(IUserSVC userSVC)
        {
            _userSVC = userSVC;
        }

        // Helper lấy thông tin từ Token (Nhanh, không gọi DB)
        private int CurrentUserId()
            => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

        private string CurrentUserRole()
            => User.FindFirstValue(ClaimTypes.Role);

        // Helper check quyền cho các chức năng Create/Delete (Đọc từ Token)
        private bool HasPermission(string code)
        {
            if (User.HasClaim(c => c.Type == "permission" && c.Value == "SYSTEM_ADMIN")) return true;
            return User.HasClaim(c => c.Type == "permission" && c.Value == code);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllUserAsync()
        {
            if (!HasPermission("USER_VIEW")) return Forbid();
            return Ok(await _userSVC.GetAllUsersAsync());
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetUserByIdAsync(int userId)
        {
            var currentUserId = CurrentUserId();
            // Cho phép xem nếu là Chính chủ HOẶC có quyền USER_VIEW
            if (currentUserId != userId && !HasPermission("USER_VIEW"))
                return Forbid();

            var user = await _userSVC.GetUserByIdAsync(userId, CurrentUserRole());
            if (user == null) return NotFound();
            return Ok(user);
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody] CreateUserRequest request)
        {
            // Chỉ ai có quyền USER_CREATE (thường là Admin) mới được tạo
            if (!HasPermission("USER_CREATE")) return Forbid();

            try
            {
                await _userSVC.CreateAsync(request);
                return Ok(new { message = "Tạo tài khoản thành công" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{userId}")]
        public async Task<IActionResult> UpdateAsync(int userId, [FromForm] UpdateUserRequest request)
        {
            var currentUserId = CurrentUserId();
            var currentUserRole = CurrentUserRole();

            // --- LOGIC CHỐT: CHỈ CHÍNH CHỦ HOẶC ADMIN ---
            // Bỏ qua tất cả permission khác, chỉ check ID và Role Admin
            if (currentUserId != userId && currentUserRole != "Admin")
            {
                return StatusCode(403, "Bạn chỉ được sửa thông tin của chính mình!");
            }

            try
            {
                await _userSVC.UpdateAsync(userId, request, currentUserRole);
                return Ok(new { message = "Cập nhật thành công" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{userId}")]
        public async Task<IActionResult> DeleteAsync(int userId)
        {
            if (!HasPermission("USER_DELETE")) return Forbid();

            try
            {
                await _userSVC.DeleteAsync(userId);
                return Ok(new { message = "Xóa tài khoản thành công" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPatch("ChangePass")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            // Chỉ cho phép TỰ ĐỔI mật khẩu của chính mình
            var userId = CurrentUserId();
            try
            {
                await _userSVC.ChangePasswordAsync(userId, request);
                return Ok(new { message = "Đổi mật khẩu thành công. Vui lòng đăng nhập lại." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}