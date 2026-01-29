using FlightDocSystem.Data;
using FlightDocSystem.Models;
using FlightDocSystem.ViewModels;
using Microsoft.AspNetCore.Components.Forms;
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
            bool exists = _DBContext.Permissions.Any(p => p.Code == request.Code);
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
