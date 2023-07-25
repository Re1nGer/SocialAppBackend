using Domain.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Persistance;
using StreamChat.Clients;
using StreamChat.Models;
using UserRequest = StreamChat.Models.UserRequest;

namespace SocialApp.Controllers.v1
{
    [ApiController]
    [Route("api/v1/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class ChatController : BaseController
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        public ChatController(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpGet("list")]
        public async Task<IActionResult> GetChat(CancellationToken cancellationToken)
        {
            var userId = GetUserId();

            var chats = await _context.UserChats
                .Where(item => item.UserId == userId)
                .ToListAsync(cancellationToken);
            
            return Ok(chats);
        }
        
        [HttpPost("")]
        public async Task<IActionResult> PostChat([FromBody] PostChatRequest request, CancellationToken cancellationToken)
        {
            var userId = GetUserId();
            
            var user = await _context.Users
                .FirstOrDefaultAsync(item => item.Id == userId, cancellationToken);

            var userChatWith = await _context.Users
                .FirstOrDefaultAsync(item => item.Id == request.UserId, cancellationToken);

            if (user is null || userChatWith is null)
                return BadRequest("No Such User Exists");

            var chatExists = await _context.UserChats.AnyAsync(item =>
                item.UserId == user.Id && item.UserWithChatId == userChatWith.Id, cancellationToken);

            if (chatExists)
            {
                return BadRequest("Such Chat Already Exists");
            }
            StreamClientFactory factory = new(_configuration.GetSection("StreamPubKey").Value,
                _configuration.GetSection("StreamPrivKey").Value);

            var channelClient = factory.GetChannelClient();

            var channelId = Guid.NewGuid().ToString();
            
            try
            {

                var chanData = new ChannelRequest
                {
                    CreatedBy = new UserRequest
                        { Id = user.Id.ToString(), Name = user.Username + " & " + userChatWith.Username },
                    Members = new List<ChannelMember>
                        { new() { UserId = user.Id.ToString() }, new() { UserId = userChatWith.Id.ToString() } },
                };


                await channelClient.GetOrCreateAsync("messaging", channelId, new ChannelGetRequest
                {
                    Data = chanData,
                });

                var userChat = new UserChat
                {
                    UserId = userId,
                    UserWithChatId = userChatWith.Id,
                    ChannelId = channelId,
                    Name = $"Conversation with {userChatWith.Username}"
                };

                var otherUserChat = new UserChat
                {
                    UserId = userChatWith.Id,
                    UserWithChatId = userId,
                    ChannelId = channelId,
                    Name = $"Conversation with {user.Username}"
                };

                await _context.UserChats.AddAsync(otherUserChat, cancellationToken);

                await _context.UserChats.AddAsync(userChat, cancellationToken);

                await _context.SaveChangesAsync(cancellationToken);

                return Ok();
            }
            catch (Exception ex)
            {
                await channelClient.DeleteAsync("messaging", channelId);
                return BadRequest(ex.Data.ToString());
            }
        }
    }

    public class PostChatRequest
    {
        public Guid UserId { get; set; }
    }
}