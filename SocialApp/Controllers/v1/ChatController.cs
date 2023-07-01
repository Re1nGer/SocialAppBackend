using Domain.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Persistance;

namespace SocialApp.Controllers.v1
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class ChatController : BaseController
    {
        private readonly ApplicationDbContext _context;

        public ChatController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetChats()
        {
            var userId = GetUserId();

            // Add the new UserComment object to the UserComments table
            var chats  = await _context.UserChats
                .Where(item => item.UserId == userId)
                .ToListAsync();

            // Return a response indicating success
            return Ok(chats);
        }

        [HttpPost]
        public async Task<IActionResult> StartChat([FromBody] StartChatRequest request)
        {
            var userId = GetUserId();

            // Fow now we should store messages for both startUser and targetUser
            var newChatStartUser = await _context.UserChats.AddAsync(new UserChat { Messages = new List<UserMessage>(), UserId = userId  });

             //var newChatTargetUser = await _context.UserChats.AddAsync(new UserChat { Messages = new List<UserMessage>(), UserId = userId  });

            // Add the new UserComment object to the UserComments table
            var chats  = await _context.UserChats
                .Where(item => item.UserId == userId)
                .ToListAsync();

            // Return a response indicating success
            return Ok(chats);
        }

        public class StartChatRequest
        {
            //Id of the user with whom conversation needs to be started
            public int TargetUserId { get; set; }
        } 
    }

}
