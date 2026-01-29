using FlightDocSystem.ViewModels;

namespace FlightDocSystem.Service
{
    public interface IPermissionSVC
    {
        Task<List<PermissionRequest>> GetAllPermissionsAsync();
        Task CreatePermission(CreatePermissionRequest request);
        Task DeleteAsync(int id);
    }
}
