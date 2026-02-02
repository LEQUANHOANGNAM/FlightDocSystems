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

            var currentPerms = _DBContext.RolePermissions.Where(rp => rp.RoleId == roleId);

            _DBContext.RolePermissions.RemoveRange(currentPerms);

            if (permissionIds != null && permissionIds.Count > 0)
            {
                foreach (var permId in permissionIds.Distinct())
                {
                    bool permExists = await _DBContext.Permissions.AnyAsync(p => p.Id == permId);
                    if (permExists)
                    {
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
            if (request == null)
            {
                throw new Exception("Dữ liệu không hợp lệ.");
            }
            request.Name = request.Name?.Trim();
            request.Description = request.Description?.Trim();

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

        public async Task DeleteAsync(int RoleId)
        {
            var role = _DBContext.Roles.Find(RoleId);

            if (role == null)
                throw new Exception("Role này không tồn tại.");
            bool isUser = await _DBContext.Users.AnyAsync(u => u.RoleId == RoleId);

            if (isUser)
                throw new Exception("Role đang được sử dụng, không thể xóa.");

            var mappings = _DBContext.RolePermissions.Where(rp => rp.RoleId == RoleId);
            _DBContext.RolePermissions.RemoveRange(mappings);

            _DBContext.Roles.Remove(role);
            await _DBContext.SaveChangesAsync();
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

        public async Task<RoleRequest?> GetByIdAsync(int id)
        {
            return await _DBContext.Roles
                .Include(r => r.RolePermissions)
                    .ThenInclude(rp => rp.Permission)
                .Where(r => r.Id == id)
                .Select(r => new RoleRequest
                {
                    Id = r.Id,
                    Name = r.Name,
                    Description = r.Description,
                    Permissions = r.RolePermissions
                        .Select(rp => rp.Permission.Code)
                        .ToList()
                })
                .FirstOrDefaultAsync();
        }

        public async Task UpdateAsync(int id, UpdateRoleRequest request)
        {
            var role = await _DBContext.Roles.FindAsync(id);
            if (role == null) throw new Exception("Role không tồn tại.");

            if (request == null)
            {
                throw new Exception("Dữ liệu không hợp lệ.");
            }

            bool NameRole = await _DBContext.Roles.AnyAsync(r => r.Name == request.Name && r.Id != id);
            if (NameRole)
            {
                throw new Exception("Role đã tồn tại.");
            }

            // Cập nhật thông tin
            role.Name = request.Name?.Trim();
            role.Description = request.Description?.Trim();

            await _DBContext.SaveChangesAsync();
        }
    }
}
