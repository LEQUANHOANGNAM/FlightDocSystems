using FlightDocSystem.Models;
using FlightDocSystem.Service;
using FlightDocSystem.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllUserAsync()
        {
            return Ok(await _userSVC.GetAllUsersAsync());
        }

        // --- SỬA LỖI Ở ĐÂY ---
        [HttpGet("{userId}")]
        public async Task<IActionResult> GetUserByIdAsync(int userId)
        {
            // 1. Lấy Role của người đang đăng nhập
            var currentUserRole = User.FindFirst(ClaimTypes.Role)?.Value;

            // 2. Truyền Role vào Service (Code cũ của bạn quên truyền tham số này)
            var user = await _userSVC.GetUserByIdAsync(userId, currentUserRole);

            if (user == null) return NotFound();
            return Ok(user);
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody] CreateUserRequest request)
        {
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
            var getUserIDString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var getUserRole = User.FindFirst(ClaimTypes.Role)?.Value;

            if (getUserIDString != null)
            {
                int getUserID = int.Parse(getUserIDString);
                // Logic: Không phải chính chủ VÀ không phải Admin thì chặn
                if (getUserID != userId && getUserRole != "Admin")
                {
                    return Forbid(); // Hoặc trả về StatusCode 403 kèm message
                }
            }
            try
            {
                await _userSVC.UpdateAsync(userId, request, getUserRole);
                return Ok(new { message = "Cập nhật thành công" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}