using Domain.Entities;
using SocialApp.Models;
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
    public class FollowController : BaseController
    {
        private readonly ApplicationDbContext _context;

        public FollowController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("isfollowing/{targetUserId}")]
        public async Task<IActionResult> IsFollowing([FromRoute]Guid targetUserId, CancellationToken token)
        {
            var userId = GetUserId();

            var user = await _context.Users
                .Include(item => item.Following)
                .AsSplitQuery()
                .FirstOrDefaultAsync(item => item.Id == userId, token);

            if (user == null) return BadRequest();

            var result = user
                .Following
                .FirstOrDefault(item => item.FollowerId == targetUserId);

            return Ok(result);
        }

        [HttpPost("")]
        public async Task<IActionResult> Follow(FriendRequest request, CancellationToken token)
        {
            var userId = GetUserId();

            var newFriendRequest = new UserRequest
            {
                DateCreated = DateTime.UtcNow,
                SenderUserId = userId,  
                Status = "Pending",
                UserReceivingRequestId = request.TargetUserId
            };

            var hasRequest = _context
                .UserRequests
                .Any(item => item.SenderUserId == userId && item.UserReceivingRequestId == request.TargetUserId); 

            if (hasRequest)
            {
                return BadRequest();
            }

            await _context.UserRequests.AddAsync(newFriendRequest, token);

            await _context.SaveChangesAsync(token);  

            return Ok();
        }

        [HttpPost("accept")]
        public async Task<IActionResult> AcceptFollow(AcceptRequest request, CancellationToken token)
        {
            var userId = GetUserId();

            var userRequest = await _context
                .UserRequests
                .Include(item => item.SendUser)
                .AsSplitQuery()
                .FirstOrDefaultAsync(item => item.SenderUserId == request.UserRequestId && item.UserReceivingRequestId == userId, token);

            if (userRequest is null)
                return BadRequest("No such follow request exists");

            userRequest.Status = "Accepted";
            
            userRequest.DateUpdated = DateTime.UtcNow;
            
            var user = await _context
                .Users
                .Include(item => item.Following)
                .AsSplitQuery()
                .FirstOrDefaultAsync(item => item.Id == userId, token);

            user.Following.Add(new() { FollowerId = request.UserRequestId, FollowingId = userId, CreatedAt = DateTime.UtcNow});
            
            await _context.SaveChangesAsync(CancellationToken.None);

            return Ok();
        }

        [HttpPost("reject")]
        public async Task<IActionResult> RejectFollow(AcceptRequest request, CancellationToken token)
        {
            var userId = GetUserId();

            var userRequest = await _context
                .UserRequests
                .Include(item => item.SendUser)
                .AsSplitQuery()
                .FirstOrDefaultAsync(item => item.SenderUserId == request.UserRequestId && item.UserReceivingRequestId == userId, token);

            if (userRequest is null)
                return BadRequest("No such follow request exists");

            userRequest.Status = "Rejected";
            
            userRequest.DateUpdated = DateTime.UtcNow;

            await _context.SaveChangesAsync(token);

            return Ok();
        }

        [HttpGet("requests")]
        public async Task<IActionResult> FollowRequests(CancellationToken token)
        {
            var userId = GetUserId();

            var newFollowRequests = await _context.UserRequests
                .Where(item => item.UserReceivingRequestId == userId && item.Status == "Pending")
                .ToListAsync(token);

            return Ok(newFollowRequests);
        }
    }
}
