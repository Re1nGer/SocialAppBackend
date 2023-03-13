using Microsoft.AspNetCore.SignalR;

namespace Chat.Hubs
{
    public class ChatHub : Hub
    {
         public async Task NewMessage(long username, string message) =>
            await Clients.All.SendAsync("messageReceived", username, message);

         public async Task SendMessage(string user, string message) =>
                 await Clients.All.SendAsync("receiveMessage", message, user );
    }
}
