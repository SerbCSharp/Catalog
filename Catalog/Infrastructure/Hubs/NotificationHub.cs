using Microsoft.AspNetCore.SignalR;

namespace Catalog.Infrastructure.Hubs
{
    public class NotificationHub : Hub
    {
        public async Task SendMessage(string message)
        {
            await Clients.Others.SendAsync("Send", message);
        }
    }
}
