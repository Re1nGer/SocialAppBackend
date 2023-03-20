using Domain.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Persistance;

namespace SocialApp.Controllers.v1
{
    [ApiController]
    [Route("api/v1/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class FriendRequestController : ControllerBase
    {
        private string? GetUserId()
        {
            return User.Claims.FirstOrDefault(item => item.Type == "UserId")?.Value;
        }

        private readonly ApplicationDbContext _context;

        public FriendRequestController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost("")]
        public async Task<IActionResult> MakeFriendRequest(FriedRequest request, CancellationToken token)
        {
            var userId = int.Parse(GetUserId());

            var newFriendRequest = new UserRequest
            {
                SenderUserId = userId,  
                Status = "Pending",
            };

            await _context.UserRequests.AddAsync(newFriendRequest, token);

            await _context.SaveChangesAsync(token);  

            var newUserReceivingRequest = new UserReceivingRequest
            {
                TargetUserId = request.TargetUserId,
                UserRequestId = newFriendRequest.Id,
            };

            await _context.UserReceivingRequests.AddAsync(newUserReceivingRequest, token);

            await _context.SaveChangesAsync(CancellationToken.None);

            return Ok();
        }
    }

    public class FriedRequest
    {
        public int TargetUserId { get; set; }
    }
}
