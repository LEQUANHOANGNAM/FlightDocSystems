using FlightDocSystem.Models;
using Microsoft.AspNetCore.Identity.Data;

namespace FlightDocSystem.Service
{
    public interface IJwtService
    {
        string GenerateToken(User user, IEnumerable<string> permissions);
    }
}
