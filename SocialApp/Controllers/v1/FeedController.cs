using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Persistance;
using SocialApp.Models;

namespace SocialApp.Controllers.v1
{
    [Route("api/v1/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class FeedController : BaseController
    {
        private readonly ApplicationDbContext _context;

        public FeedController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("")]
        public async Task<IActionResult> GetFeedForUser(CancellationToken token)
        {
            // Add the new UserComment object to the UserComments table
            //first we need to get all the users he follows
            var userId = GetUserId();

            var user = await _context.Users
                .Include(item => item.Following)
                .Include(item => item.Followers)
                .AsSplitQuery()
                .AsNoTracking()
                .FirstOrDefaultAsync(item => item.Id == userId, token);

            if (user is null) return NotFound();

            if (user.Following is null) return Ok();
            
            var followingIds = user
                .Followers
                .Select(item => item.FollowerId)
                .ToList();

            var userPosts = _context.UserPosts
                .Include(item => item.Likes)
                .Include(item => item.Comments)
                .Include(item => item.User)
                .Where(item => followingIds.Contains(item.UserId))
                .OrderByDescending(item => item.CreatedAt)
                .Take(10);
            
           var result = userPosts.Select(item => new PostModel
                {
                    Id = item.Id,
                    Message = item.Message,
                    LikeCount = item.Likes.Count,
                    CommentCount = item.Comments.Count,
                    MediaUrl = item.LowResMediaUrl,
                    UserImageLink = item.User.LowResImageLink,
                    Username = item.User.Username,
                    UserId = item.UserId,
                    HasUserLike = item.Likes.Any(like => like.UserId == userId),
                    HasUserSaved = item.PostBookmarks.Any(book => book.UserId == userId && book.UserPostId == item.Id)
                })
            .ToList();

            // Return a response indicating success
            return Ok(result);
        }
    }
}
