using Microsoft.AspNetCore.SignalR;
using Persistance;

namespace Chat.Hubs
{
    public class ChatHub : Hub
    {
        private readonly ApplicationDbContext _context;

        public ChatHub(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task SendMessage(string user, string message)
        {
             //await Clients.Ad.SendAsync("receiveMessage", message, user );
        }
    }
}
