using Domain.Entities;
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
    public class LikeController : BaseController
    {
        private readonly ApplicationDbContext _context;

        public LikeController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPut("{postId}")]
        public async Task<IActionResult> PutLike(Guid postId, CancellationToken token)
        {
            var userId = GetUserId();
            
            var likedPost = await _context.UserLikes
                .AnyAsync(item => item.UserId == userId && item.PostId == postId, token);

            if (likedPost)
            {
                return BadRequest();
            }

            var newLike = new Like
            {
                PostId = postId, 
                UserId = userId,
                CreatedAt = DateTime.UtcNow,
            };

            await _context.UserLikes.AddAsync(newLike, token); 
            await _context.SaveChangesAsync(CancellationToken.None);
            return Ok();
        }

        [HttpDelete("{postId}")]
        public async Task<IActionResult> DeleteLike(Guid postId, CancellationToken token)
        {

            var userId = GetUserId();
            
            var likedPost = await _context.UserLikes
                .AnyAsync(item => item.UserId == userId
                                  && item.PostId == postId, token);

            if (!likedPost)
            {
                return BadRequest();
            }

            var like = await _context.UserLikes
                .FirstAsync(item => item.PostId == postId
                                    && item.UserId == userId, token);

            _context.UserLikes.Remove(like);

            await _context.SaveChangesAsync(CancellationToken.None);  

            return Ok();    
        }
    }
}
