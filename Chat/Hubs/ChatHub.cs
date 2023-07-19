using System.Security.Claims;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
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

        [Authorize]
        public async Task SendMessage(Guid sendingUserId, Guid receivingUserId, Guid userChatId, string message, CancellationToken token)
        {
            var chat = await _context.UserChats
                .FirstOrDefaultAsync(item => item.Id == userChatId, token);

            var targetUserChat = await _context.UserChats
                .FirstOrDefaultAsync(item => item.UserId == receivingUserId, token);
            
            if (chat is null)
                Context.Abort();
            
            var newMessage = new UserMessage
            {
                SourceUserId = sendingUserId,
                TargetUserId = receivingUserId,
                Message = message,
                CreatedAt = DateTime.UtcNow,
            };

            if (chat?.Messages is null)
                chat.Messages = new List<UserMessage> { newMessage };
            
            else chat.Messages.Add(newMessage);
            
            if (targetUserChat is null)
                targetUserChat.Messages = new List<UserMessage> { newMessage };
            
            else targetUserChat.Messages.Add(newMessage);
            
            await _context.SaveChangesAsync(token);
        }
        
        [Authorize]
        public async Task GetMessages(string clientUserId, CancellationToken token)
        {
            var userId = Context.User.FindFirstValue("userId");

            var parsedUserId = Guid.Parse(userId);

            if (userId == clientUserId)
            {
                var messages = await _context.UserChats
                    .Where(item => item.UserId == parsedUserId)
                    .ToListAsync(token);
                
                //Send the messages to the client that requested them
                await Clients.Caller.SendAsync("ReceiveMessages", messages, token);
            }
            Context.Abort();
        }
    }
}
