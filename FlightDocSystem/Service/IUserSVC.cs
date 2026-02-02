using FlightDocSystem.Models;
using FlightDocSystem.ViewModels;

namespace FlightDocSystem.Service
{
    public interface IUserSVC
    {
        Task<List<UserRequest>> GetAllUsersAsync();
        Task<UserRequest> GetUserByIdAsync(int userId, string currentUserRole);
        Task CreateAsync(CreateUserRequest request);
        Task UpdateAsync(int userId, UpdateUserRequest request, string currentUserRole);
        Task DeleteAsync(int userId);
        Task ChangePasswordAsync(int userId, ChangePasswordRequest request);
    }
}
