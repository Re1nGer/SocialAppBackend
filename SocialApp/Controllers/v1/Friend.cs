using Domain.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Persistance;
using SocialApp.Models;

namespace SocialApp.Controllers.v1
{
    [ApiController]
    [Route("api/v1/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class Friend : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public Friend(ApplicationDbContext context)
        {
            _context = context;
        }

        //for now let's assume that anyone can request follow request 
        [HttpPost("follow-request")]
        public IActionResult Follow([FromBody] FollowFriendRequest request)
        {
            try
            {
                var conectionExists = _context.UserFollowers.Any(x => x.FollowerId == request.SourceUserId && x.FollowingId == request.TargetUserId);

                if (conectionExists)
                    return BadRequest("User already has target connection");

                var friendFollower = new UserFollower
                {
                    FollowerId = request.SourceUserId,
                    FollowingId = request.TargetUserId,
                    CreatedAt = DateTime.UtcNow,
                    Type = 1,
                };

                _context.UserFollowers.Add(friendFollower);

                return Ok();

            } catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
