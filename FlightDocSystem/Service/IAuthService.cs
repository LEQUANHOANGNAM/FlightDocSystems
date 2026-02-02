using FlightDocSystem.ViewModels;

namespace FlightDocSystem.Service
{
    public interface IAuthService
    {
        Task<object> LoginAsync(LoginRequest request);
        Task LogoutAsync(string token);
    }
}
