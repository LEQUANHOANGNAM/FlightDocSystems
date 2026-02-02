using FlightDocSystem.Data;
using FlightDocSystem.Helper;
using FlightDocSystem.Models;
using FlightDocSystem.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace FlightDocSystem.Service
{
    public class UserSVC : IUserSVC
    {
        private readonly AppDbContext _DBContext;
        private readonly IWebHostEnvironment _env;

        public UserSVC(AppDbContext dbContext, IWebHostEnvironment env)
        {
            _DBContext = dbContext;
            _env = env;
        }

        public async Task CreateAsync(CreateUserRequest request)
        {
            if (request == null) throw new Exception("Dữ liệu không hợp lệ.");
            request.FullName = request.FullName?.Trim();

            if (!request.Email.Trim().EndsWith("@vietjetair.com", StringComparison.OrdinalIgnoreCase))
            {
                throw new Exception("Chỉ chấp nhận email của VietJet (@vietjetair.com)!");
            }
            if (await _DBContext.Users.AnyAsync(u => u.Email == request.Email))
                throw new Exception("Email đã tồn tại.");

            bool roleExists = await _DBContext.Roles.AnyAsync(r => r.Id == request.RoleId);
            if (!roleExists) throw new Exception("RoleId không tồn tại.");

            var user = new User
            {
                FullName = request.FullName,
                Email = request.Email,
                RoleId = request.RoleId,
                Password = Md5Helper.Hash(request.Password),
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };
            _DBContext.Users.Add(user);
            await _DBContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(int userId)
        {
            var user = await _DBContext.Users.FindAsync(userId);
            if (user == null) throw new Exception("UserId không tồn tại.");

            // Xóa cứng (Remove) hoặc Xóa mềm (IsActive = false)
            _DBContext.Users.Remove(user);
            await _DBContext.SaveChangesAsync();
        }

        public async Task<UserRequest> GetUserByIdAsync(int userId, string currentUserRole)
        {
            var user = await _DBContext.Users
                .Include(x => x.Role)
                .FirstOrDefaultAsync(x => x.Id == userId);

            if (user == null) return null;

            var result = new UserRequest
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                RoleName = user.Role != null ? user.Role.Name : "N/A",
                CreatedAt = user.CreatedAt,
                RoleId = null,   // Mặc định ẩn
                IsActive = null  // Mặc định ẩn
            };

            // Admin mới được xem RoleId và trạng thái Active
            if (currentUserRole == "Admin")
            {
                result.RoleId = user.RoleId;
                result.IsActive = user.IsActive;
            }

            return result;
        }

        public async Task UpdateAsync(int userId, UpdateUserRequest request, string currentUserRole)
        {
            var user = await _DBContext.Users.FindAsync(userId);
            if (user == null) throw new Exception("UserId không tồn tại.");

            // 1. Cập nhật Tên
            if (!string.IsNullOrWhiteSpace(request.FullName))
                user.FullName = request.FullName.Trim();

            // 2. Cập nhật Avatar (ĐÃ FIX LỖI NULL CHECK)
            if (request.Avatar != null)
            {
                const long maxSizeBytes = 5 * 1024 * 1024; // 5MB
                if (request.Avatar.Length > maxSizeBytes)
                    throw new Exception("Dung lượng ảnh tối đa 5MB.");

                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
                var extension = Path.GetExtension(request.Avatar.FileName).ToLower();
                if (!allowedExtensions.Contains(extension))
                {
                    throw new Exception("Chỉ cho phép tải lên các định dạng ảnh: .jpg, .jpeg, .png");
                }

                string webRootPath = _env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
                string uploadFolder = Path.Combine(webRootPath, "Uploads");

                if (!Directory.Exists(uploadFolder)) Directory.CreateDirectory(uploadFolder);

                // Xóa ảnh cũ nếu có
                if (!string.IsNullOrWhiteSpace(user.AvatarPath))
                {
                    var oldFilePath = Path.Combine(webRootPath, user.AvatarPath.TrimStart('/').Replace("/", Path.DirectorySeparatorChar.ToString()));
                    if (File.Exists(oldFilePath)) File.Delete(oldFilePath);
                }

                // Lưu ảnh mới
                var fileName = $"Avatar_{userId}_{Guid.NewGuid()}{extension}";
                var filePath = Path.Combine(uploadFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await request.Avatar.CopyToAsync(stream);
                }
                user.AvatarPath = $"/Uploads/{fileName}";
            }

            // 3. Xử lý quyền hạn nâng cao (Chỉ Admin mới được sửa Role và Active)
            if (currentUserRole == "Admin")
            {
                if (request.RoleId.HasValue)
                {
                    if (await _DBContext.Roles.AnyAsync(r => r.Id == request.RoleId.Value))
                    {
                        user.RoleId = request.RoleId.Value;
                    }
                }

                if (request.IsActive.HasValue)
                {
                    user.IsActive = request.IsActive.Value;
                }
            }

            await _DBContext.SaveChangesAsync();
        }

        // Hàm bổ sung cho đổi mật khẩu
        public async Task ChangePasswordAsync(int userId, ChangePasswordRequest request)
        {
            var user = await _DBContext.Users.FindAsync(userId);
            if (user == null) throw new Exception("User không tồn tại.");

            // Bạn cần đảm bảo request.OldPassword và NewPassword đã được validate ở Model
            string oldPassHash = Md5Helper.Hash(request.OldPassword);
            if (user.Password != oldPassHash)
            {
                throw new Exception("Mật khẩu cũ không chính xác.");
            }

            user.Password = Md5Helper.Hash(request.NewPassword);
            await _DBContext.SaveChangesAsync();
        }

        async Task<List<UserRequest>> IUserSVC.GetAllUsersAsync()
        {
            return await _DBContext.Users
                .Include(u => u.Role)
                .Select(user => new UserRequest
                {
                    Id = user.Id,
                    FullName = user.FullName,
                    Email = user.Email,
                    RoleName = user.Role.Name,
                    CreatedAt = user.CreatedAt,
                    RoleId = user.RoleId,
                    IsActive = user.IsActive
                })
                .ToListAsync();
        }
    }
}