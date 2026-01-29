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
        private readonly IWebHostEnvironment _env; // 1. Thêm cái này để xử lý đường dẫn file

        // 2. Tiêm vào Constructor
        public UserSVC(AppDbContext dbContext, IWebHostEnvironment env)
        {
            _DBContext = dbContext;
            _env = env;
        }

        public async Task CreateAsync(CreateUserRequest request)
        {
            if (!request.Email.Trim().EndsWith("@vietjetair.com", StringComparison.OrdinalIgnoreCase))
            {
                throw new Exception("Chỉ chấp nhận email của VietJet (@vietjetair.com)!");
            }
            if (await _DBContext.Users.AnyAsync(u => u.Email == request.Email))
                throw new Exception("Email đã tồn tại.");

            var user = new User
            {
                FullName = request.FullName,
                Email = request.Email,
                RoleId = request.RoleId,
                Password = Md5Helper.Hash(request.Password), // Nhớ đảm bảo Md5Helper hoạt động tốt
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
            _DBContext.Users.Remove(user);
            await _DBContext.SaveChangesAsync();
        }

        // --- HÀM QUAN TRỌNG: PHÂN QUYỀN HIỂN THỊ ---
        public async Task<UserRequest> GetUserByIdAsync(int userId, string currentUserRole)
        {
            var user = await _DBContext.Users
                .Include(x => x.Role)
                .FirstOrDefaultAsync(x => x.Id == userId);

            if (user == null) return null;

            // 3. Mapping dữ liệu cơ bản
            // Lưu ý: Class UserRequest của bạn phải có các trường int? RoleId và bool? IsActive nhé
            var result = new UserRequest
            {
                Id = user.Id, // Giữ nguyên ID của User
                FullName = user.FullName,
                Email = user.Email,
                RoleName = user.Role != null ? user.Role.Name : "N/A", // Tránh lỗi null reference
                CreatedAt = user.CreatedAt,
                // Mặc định gán null để giấu (nếu UserRequest cho phép null)
                RoleId = null,
                IsActive = null
            };

            if (currentUserRole == "Admin")
            {
                // Nếu là Admin: Hiện nguyên hình
                result.RoleId = user.RoleId;   // Sửa lỗi: Gán vào RoleId chứ không phải Id
                result.IsActive = user.IsActive;
            }
            // Else: Không làm gì cả, giữ nguyên null để ẩn

            return result;
        }

        public async Task UpdateAsync(int userId, UpdateUserRequest request, string currentUserRole)
        {
            var user = await _DBContext.Users.FindAsync(userId);
            if (user == null) throw new Exception("UserId không tồn tại.");

            user.FullName = request.FullName;

            if (request.Avatar != null)
            {
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
                var extension = Path.GetExtension(request.Avatar.FileName).ToLower();
                if (!allowedExtensions.Contains(extension))
                {
                    throw new Exception("Chỉ cho phép tải lên các định dạng ảnh: .jpg, .jpeg, .png");
                }

                string webRootPath = _env.WebRootPath;
                if (string.IsNullOrEmpty(webRootPath))
                {
                    webRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
                }

                string uploadFolder = Path.Combine(webRootPath, "Uploads");

                if (!Directory.Exists(uploadFolder))
                {
                    Directory.CreateDirectory(uploadFolder);
                }

                var fileName = $"Avatar_{userId}_{Guid.NewGuid()}{extension}";
                var filePath = Path.Combine(uploadFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await request.Avatar.CopyToAsync(stream);
                }

                user.AvatarPath = $"/Uploads/{fileName}";
            }

            // --- XỬ LÝ QUYỀN ADMIN ---
            if (currentUserRole == "Admin")
            {
                // --- SỬA LỖI ROLE ID ---
                // 1. Kiểm tra xem người dùng có gửi RoleId lên không (HasValue)
                if (request.RoleId.HasValue)
                {
                    // 2. Kiểm tra RoleId đó có tồn tại trong DB không (dùng .Value để lấy số int ra)
                    bool roleExists = await _DBContext.Roles.AnyAsync(r => r.Id == request.RoleId.Value);

                    if (roleExists)
                    {
                        // 3. Gán giá trị (dùng .Value)
                        user.RoleId = request.RoleId.Value;
                    }
                }

                // --- SỬA LỖI IS ACTIVE ---
                // 1. Kiểm tra xem người dùng có gửi trạng thái lên không
                if (request.IsActive.HasValue)
                {
                    // 2. Gán giá trị
                    user.IsActive = request.IsActive.Value;
                }
            }

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
                    // GetAll thường để list, có thể ẩn hoặc hiện tùy bạn.
                    // Ở đây mình để hiện RoleId cho dễ debug
                    RoleId = user.RoleId,
                    IsActive = user.IsActive
                })
                .ToListAsync();
        }
    }
}