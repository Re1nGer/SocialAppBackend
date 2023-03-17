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
    public class LikeController : ControllerBase
    {
        private string? GetUserId()
        {
            return User.Claims.FirstOrDefault(item => item.Type == "UserId")?.Value;
        }

        private readonly ApplicationDbContext _context;

        public LikeController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPut("{postId}")]
        public async Task<IActionResult> PutLike(int postId, CancellationToken token)
        {
            var likedPost = await _context.UserLikes.AnyAsync(item => item.UserId == int.Parse(GetUserId()) && item.PostId ==postId, token);

            if (likedPost)
            {
                return BadRequest();
            }

            var newLike = new Like
            {
                PostId = postId, 
                UserId = int.Parse(GetUserId()),
                CreatedAt = DateTime.UtcNow,
            };

            await _context.UserLikes.AddAsync(newLike); 
            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpDelete("{postId}")]
        public async Task<IActionResult> DeleteLike(int postId, CancellationToken token)
        {
            var likedPost = await _context.UserLikes.AnyAsync(item => item.UserId == int.Parse(GetUserId()) && item.PostId == postId, token);

            if (!likedPost)
            {
                return BadRequest();
            }

            var like = await _context.UserLikes.FirstAsync(item => item.PostId == postId && item.UserId == int.Parse(GetUserId()));

            _context.UserLikes.Remove(like);

            await _context.SaveChangesAsync(CancellationToken.None);  

            return Ok();    
        }


        [HttpGet("{postId}")]
        public async Task<IActionResult> GetLike(int postId, CancellationToken token)
        {
            var likedPost = await _context.UserLikes.AnyAsync(item => item.UserId == int.Parse(GetUserId()) && item.PostId == postId, token);

            if (!likedPost)
            {
                return Ok(false);
            }

            //var like = await _context.UserLikes.FirstAsync(item => item.U);

            return Ok(true);
        }

    }
}
