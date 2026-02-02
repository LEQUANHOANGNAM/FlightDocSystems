using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace FlightDocSystem.Hubs
{
    [Authorize]
    public class NotificationHub : Hub
    {
        // Client gọi sau khi connect để join group theo userId
        public Task JoinMyGroup()
        {
            var userId = Context.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId)) return Task.CompletedTask;

            return Groups.AddToGroupAsync(Context.ConnectionId, $"USER:{userId}");
        }
    }
}