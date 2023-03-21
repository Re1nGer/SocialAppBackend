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
    public class UserController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public UserController(ApplicationDbContext context)
        {
            _context = context;
        }

        private string? GetUserId()
        {
            return User.Claims.FirstOrDefault(item => item.Type == "UserId")?.Value;
        }

        [HttpGet("")]
        public async Task<IActionResult> GetUserList([FromQuery] string q)
        {
            var userList = _context.Users.AsQueryable();

            userList = userList.Where(item => item.Email.Contains(q));  

            return Ok(await userList.ToListAsync());
        }

        [HttpPost("blockeduser")]
        public async Task<IActionResult> BlockUser([FromBody] BlockUserRequest request)
        {
            if (await _context.UsersBlocked.AnyAsync(item => item.UserBlockedId == request.BlockedUserId))
                return BadRequest("User is already blocked");

            var userId = int.Parse(GetUserId());

            var userBlocked = new UserBlocked
            {
                UserBlockedId = request.BlockedUserId,
                UserId = userId,    
            };

            await _context.UsersBlocked.AddAsync(userBlocked);

            await _context.SaveChangesAsync();

            return Ok();
        }
    }
    public class BlockUserRequest
    {
        public int BlockedUserId { get; set; }
    }
}
