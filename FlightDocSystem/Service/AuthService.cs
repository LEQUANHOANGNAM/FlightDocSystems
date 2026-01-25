using FlightDocSystem.Data;
using FlightDocSystem.ViewModels;
using FlightDocSystem.Helper;
using FlightDocSystem.Service;
using Microsoft.EntityFrameworkCore;

namespace FlightDocSystem.Services.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _config;
        private readonly IJwtService _jwtService;

        public AuthService(AppDbContext context,
                           IConfiguration config,
                           IJwtService jwtService)
        {
            _context = context;
            _config = config;
            _jwtService = jwtService;
        }

        public async Task<object> LoginAsync(LoginRequest request)
        {
            var domain = _config["AllowedEmailDomain"];
            if (!request.Email.EndsWith($"@{domain}"))
                throw new Exception("Email domain không hợp lệ");

            var passwordHash = Md5Helper.Hash(request.Password);

            var user = await _context.Users
                .Include(u => u.Role)
                    .ThenInclude(r => r.RolePermissions)
                        .ThenInclude(rp => rp.Permission)
                .FirstOrDefaultAsync(u =>
                    u.Email == request.Email &&
                    u.Password == passwordHash &&
                    u.IsActive);

            if (user == null)
                throw new Exception("Sai tài khoản hoặc mật khẩu");

            var permissions = user.Role.RolePermissions
                .Select(rp => rp.Permission.Code)
                .ToList();

            var token = _jwtService.GenerateToken(user, permissions);

            return new
            {
                token,
                role = user.Role.Name,
                permissions
            };
        }
    }
}
