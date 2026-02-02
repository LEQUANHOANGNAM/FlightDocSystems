using FlightDocSystem.ViewModels;

namespace FlightDocSystem.Service
{
    public interface IPermissionSVC
    {
        Task<List<PermissionRequest>> GetAllPermissionsAsync();
        Task<PermissionRequest?> GetByIdAsync(int id);
        Task CreatePermission(CreatePermissionRequest request);
        Task UpdateAsync(int id, UpdatePermissionRequest request);
        Task DeleteAsync(int id);
    }
}
