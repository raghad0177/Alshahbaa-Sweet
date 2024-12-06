// Server-Side: NotificationsHub.cs
using Microsoft.AspNetCore.SignalR;

namespace alshahbaasweets.DTO
{
    public class NotificationsHub : Hub
    {
        // This method can be called to notify clients of new orders
        public async Task NotifyNewOrder(string message)
        {
            // Sending the correct event name
            await Clients.All.SendAsync("ReceiveNotification", message);
        }
    }
}
