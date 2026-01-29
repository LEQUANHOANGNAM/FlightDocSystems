using FlightDocSystem.Data;
using FlightDocSystem.Models;
using FlightDocSystem.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace FlightDocSystem.Service
{
    public class RoleSVC : IRoleSVC
    {
        private readonly AppDbContext _DBContext;
        public RoleSVC(AppDbContext dbContext)
        {
            _DBContext = dbContext;
        }

        public async Task AssignPermissionsAsync(int roleId, List<int> permissionIds)
        {
            var role = await _DBContext.Roles.FindAsync(roleId);
            if (role == null) throw new Exception("Vai trò không tồn tại.");

            // 2. CHIẾN THUẬT: "XÓA HẾT CŨ - THÊM HẾT MỚI"
            // Lấy tất cả dòng trong bảng trung gian thuộc về Role này
            var currentPerms = _DBContext.RolePermissions.Where(rp => rp.RoleId == roleId);

            // Xóa sạch (Bulk Delete)
            _DBContext.RolePermissions.RemoveRange(currentPerms);

            // 3. Thêm danh sách mới vào
            if (permissionIds != null && permissionIds.Count > 0)
            {
                // Distinct(): Loại bỏ ID trùng nếu client lỡ gửi [1, 1, 2]
                foreach (var permId in permissionIds.Distinct())
                {
                    // Kiểm tra xem PermissionId có tồn tại trong DB không (tránh lỗi khóa ngoại)
                    bool permExists = await _DBContext.Permissions.AnyAsync(p => p.Id == permId);
                    if (permExists)
                    {
                        // Tạo liên kết mới
                        _DBContext.RolePermissions.Add(new RolePermission
                        {
                            RoleId = roleId,
                            PermissionId = permId
                        });
                    }
                }
            }

            // 4. Lưu tất cả thay đổi (Vừa xóa cũ, vừa thêm mới) trong 1 lần giao dịch
            await _DBContext.SaveChangesAsync();
        }

        public async Task CreateAsync(CreateRoleRequest request)
        {
            if (await _DBContext.Roles.AnyAsync(r => r.Name == request.Name))
            {
                throw new Exception("Role đã tồn tại.");
            }
            var role = new Role
            {
                Name = request.Name,
                Description = request.Description
            };
            _DBContext.Roles.Add(role);
            await _DBContext.SaveChangesAsync();
        }

        public Task DeleteAsync(int RoleId)
        {
            var role = _DBContext.Roles.Find(RoleId);
            if (role == null)
                throw new Exception("Role này không tồn tại.");
            _DBContext.Roles.Remove(role);
            return _DBContext.SaveChangesAsync();
        }

        public async Task<List<RoleRequest>> GetAllRoleAsync()
        {
            return await _DBContext.Roles
                .Include(r => r.RolePermissions)
                    .ThenInclude(rp => rp.Permission)
                .Select(r => new RoleRequest
                {
                    Id = r.Id,
                    Name = r.Name,
                    Description = r.Description,
                    Permissions = r.RolePermissions
                        .Select(rp => rp.Permission.Code)
                        .ToList()
                }).ToListAsync();
        }

        public async Task UpdateAsync(int id, UpdateRoleRequest request)
        {
            var role = await _DBContext.Roles.FindAsync(id);
            if (role == null) throw new Exception("Vai trò không tồn tại.");

            // Cập nhật thông tin
            role.Name = request.Name;
            role.Description = request.Description;

            await _DBContext.SaveChangesAsync();
        }
    }
}
