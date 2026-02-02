using FlightDocSystem.ViewModels;
namespace FlightDocSystem.Service
{
    public interface IRoleSVC
    {
        Task<List<RoleRequest>> GetAllRoleAsync();
        Task<RoleRequest?> GetByIdAsync(int id);
        Task CreateAsync(CreateRoleRequest request);
        Task UpdateAsync(int id, UpdateRoleRequest request);
        Task DeleteAsync(int RoleId);

        Task AssignPermissionsAsync(int roleId, List<int> permissionIds);
    }
}
