using Domain.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Persistance;

namespace SocialApp.Controllers.v1
{
    [ApiController]
    [Route("api/v1/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class CommentController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CommentController(ApplicationDbContext context)
        {
            _context = context;
        }

        private Guid GetUserId()
        {
            return Guid.Parse(User.Claims.FirstOrDefault(item => item.Type == "UserId")?.Value);
        }

        [HttpGet]
        [Route("{postId}")]
        public async Task<IActionResult> GetComments(Guid postId)
        {

            // Add the new UserComment object to the UserComments table
            var comments  = await _context.UserComments
                .Where(item => item.PostId == postId).ToListAsync();

            // Return a response indicating success
            return Ok(comments);
        }

        [HttpPost]
        [Route("")]
        public async Task<IActionResult> PostComment([FromBody] PostCommentRequest comment)
        {
            if (comment is null || comment.PostId == Guid.Empty || comment.Message == "")
            {
                return BadRequest();
            }

            var user = await _context.Users.FirstOrDefaultAsync(item => item.Id == GetUserId());

            if (user is null)
            {
                return BadRequest("User not found");
            }

            // Create a new UserComment object and set its properties
            var userComment = new Comment
            {
                Username = user.Email,
                DateCreated = DateTime.UtcNow,
                Message = comment.Message,
                PostId = comment.PostId,
                UserId = GetUserId()
            };

            // Add the new UserComment object to the UserComments table
            await _context.UserComments.AddAsync(userComment);

            await _context.SaveChangesAsync();

            // Return a response indicating success
            return Ok();
        }

        [HttpDelete]
        [Route("{postId}/{commentId}")]
        public async Task<IActionResult> DeleteComment(Guid postId, Guid commentId)
        {
            // Create a new UserComment object and set its properties
            var commentExists = await _context
                .UserComments
                .AnyAsync(item => item.PostId == postId && item.Id == commentId && item.UserId == GetUserId());

            if (!commentExists)
            {
                return NotFound();
            }

            var userComment = await _context.UserComments.FirstAsync(item => item.Id == commentId);

            // Add the new UserComment object to the UserComments table
            _context.UserComments.Remove(userComment);

            await _context.SaveChangesAsync();

            // Return a response indicating success
            return Ok();
        }
        [HttpPut("")]
        public async Task<IActionResult> EditComment(PutCommentRequest request)
        {
            // Create a new UserComment object and set its properties
            var commentExists = await _context
                .UserComments
                .AnyAsync(item => item.PostId == request.PostId && item.Id == request.CommentId && item.UserId == GetUserId());

            if (!commentExists)
            {
                return NotFound();
            }

            var userComment = await _context.UserComments.FirstAsync(item => item.Id == request.CommentId);

            userComment.Message = request.Message;
            userComment.DateCreated = DateTime.UtcNow;

            // Add the new UserComment object to the UserComments table
            _context.UserComments.Update(userComment);

            await _context.SaveChangesAsync();

            // Return a response indicating success
            return Ok();
        }
    }
    public class PutCommentRequest : PostCommentRequest
    {
        public Guid CommentId { get; set; }
    }
    public class PostCommentRequest
    {
        public Guid PostId { get; set; }
        public string Message { get; set; }
    }
}
