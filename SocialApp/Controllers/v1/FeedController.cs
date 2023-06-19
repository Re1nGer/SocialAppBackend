using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Persistance;

namespace SocialApp.Controllers.v1
{
    [Route("api/v1/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class FeedController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public FeedController(ApplicationDbContext context)
        {
            _context = context;
        }
        
        private Guid GetUserId()
        {
            return Guid.Parse(User.Claims.FirstOrDefault(item => item.Type == "UserId")?.Value!);
        }

        [HttpGet("")]
        public async Task<IActionResult> GetFeedForUser(CancellationToken token)
        {
            // Add the new UserComment object to the UserComments table
            //first we need to get all the users he follows
            var userId = GetUserId();

            var user = await _context.Users
                .Include(item => item.Following)
                .AsSplitQuery()
                .AsNoTracking()
                .FirstOrDefaultAsync(item => item.Id == userId, token);

            if (user is null) return NotFound();

            if (user.Following == null) return Ok();
            
            var followingIds = user.Following.Select(item => item.FollowingId).ToList();

            var followingPosts = await _context
                .UserPosts
                .Include(item => item.User)
                .Include(item => item.Likes)
                .AsSplitQuery()
                .AsNoTracking()
                .OrderByDescending(item => item.CreatedAt)
                .Where(item => followingIds.Contains(item.UserId))
                .ToListAsync(token);

            // Return a response indicating success
            return Ok(followingPosts);
        }
    }
}
