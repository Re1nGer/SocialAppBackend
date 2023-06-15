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
    public class FollowController : ControllerBase
    {
        private Guid GetUserId()
        {
            return Guid.Parse(User.Claims.FirstOrDefault(item => item.Type == "UserId")?.Value);
        }

        private readonly ApplicationDbContext _context;

        public FollowController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("isfollowing/{targetUserId}")]
        public async Task<IActionResult> IsFollowing([FromRoute]int targetUserId, CancellationToken token)
        {
            var userId = GetUserId();

            var user = await _context.Users
                .Include(item => item.Following)
                .FirstOrDefaultAsync(item => item.Id == userId);

            if (user == null) return BadRequest();

            var result = user.Following.FirstOrDefault(item => item.Id == targetUserId);

            return Ok(result);
        }

        [HttpPost("")]
        public async Task<IActionResult> Follow(FriedRequest request, CancellationToken token)
        {
            var userId = GetUserId();

            var newFriendRequest = new UserRequest
            {
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

            var userRequest = _context
                .UserRequests
                .Include(item => item.SendUser)
                .FirstOrDefault(item => item.SenderUserId == request.UserRequestId &&  item.UserReceivingRequestId == userId);

            if (userRequest is null)
                return BadRequest("No such follow request exists");

            userRequest.Status = "Accepted";

            await _context.SaveChangesAsync(CancellationToken.None);

            return Ok();
        }

        [HttpPost("reject")]
        public async Task<IActionResult> RejectFollow(AcceptRequest request, CancellationToken token)
        {
            var userId = GetUserId();

            var userRequest = _context
                .UserRequests
                .Include(item => item.SendUser)
                .FirstOrDefault(item => item.Id == request.UserRequestId);

            if (userRequest is null)
                return BadRequest("No such follow request exists");

            userRequest.Status = "Rejected";

            await _context.SaveChangesAsync(CancellationToken.None);

            return Ok();
        }

        [HttpGet("requests")]
        public async Task<IActionResult> FollowRequests(CancellationToken token)
        {
            var userId = GetUserId();

            var newFollowRequests = await _context.UserRequests
                .Where(item => item.UserReceivingRequestId == userId && item.Status == "Pending")
                .ToListAsync();

            return Ok(newFollowRequests);
        }
    }

    public class FriedRequest
    {
        public Guid TargetUserId { get; set; }
    }
    public class AcceptRequest
    {
        public Guid UserRequestId { get; set; }
    }
}
