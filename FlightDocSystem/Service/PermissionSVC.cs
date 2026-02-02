using FlightDocSystem.Data;
using FlightDocSystem.Models;
using FlightDocSystem.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace FlightDocSystem.Service
{
    public class PermissionSVC : IPermissionSVC
    {
        private readonly AppDbContext _DBContext;
        public PermissionSVC(AppDbContext DBContext)
        {
            _DBContext = DBContext;
        }
        public async Task CreatePermission(CreatePermissionRequest request)
        {
            if (request == null) throw new Exception("Dữ liệu không hợp lệ.");
            request.Code = request.Code?.Trim();
            request.Description = request.Description?.Trim();

            if (string.IsNullOrWhiteSpace(request.Code))
                throw new Exception("Mã quyền là bắt buộc.");
            bool exists = await _DBContext.Permissions.AnyAsync(p => p.Code == request.Code);
            if (exists)
            {
                throw new Exception("Permission đã tồn tại.");
            }
            var permission = new Permission
            {
                Code = request.Code,
                Description = request.Description
            };
            _DBContext.Permissions.Add(permission);
            await _DBContext.SaveChangesAsync();
        }

        public async Task<PermissionRequest?> GetByIdAsync(int id)
        {
            return await _DBContext.Permissions
                .Where(p => p.Id == id)
                .Select(p => new PermissionRequest
                {
                    Id = p.Id,
                    Code = p.Code,
                    Description = p.Description
                })
                .FirstOrDefaultAsync();
        }

        public async Task UpdateAsync(int id, UpdatePermissionRequest request)
        {
            if (request == null) throw new Exception("Dữ liệu không hợp lệ.");
            request.Code = request.Code?.Trim();
            request.Description = request.Description?.Trim();

            if (string.IsNullOrWhiteSpace(request.Code))
                throw new Exception("Mã quyền là bắt buộc.");

            var permission = await _DBContext.Permissions.FindAsync(id);
            if (permission == null)
                throw new Exception("Permission không tồn tại.");

            bool codeExists = await _DBContext.Permissions
                .AnyAsync(p => p.Code == request.Code && p.Id != id);
            if (codeExists)
                throw new Exception("Mã quyền đã tồn tại.");

            permission.Code = request.Code;
            permission.Description = request.Description;

            await _DBContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var permission = _DBContext.Permissions.Find(id);
            if (permission == null)
            {
                throw new Exception("Permission không tồn tại.");
            }

            bool isAssigned = await _DBContext.RolePermissions.AnyAsync(up => up.PermissionId == id);
            if (isAssigned)
            {
                throw new Exception("Permission đang được sử dụng, không thể xóa.");
            }
            _DBContext.Permissions.Remove(permission);
            await _DBContext.SaveChangesAsync();
        }

        public async Task<List<PermissionRequest>> GetAllPermissionsAsync()
        {
            return await _DBContext.Permissions
                .Select(p => new PermissionRequest
                {
                    Id = p.Id,
                    Code = p.Code,
                    Description = p.Description
                })
                .ToListAsync();
        }
    }
}
