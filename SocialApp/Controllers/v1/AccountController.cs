using Microsoft.AspNetCore.Mvc;
using Persistance;
using SocialApp.Services;
using System.Security.Claims;
using Firebase.Auth;
using Microsoft.EntityFrameworkCore;
using SocialApp.Models;
using StreamChat.Clients;
using StreamChat.Models;
using FirebaseConfig = Firebase.Auth.FirebaseConfig;
using User = Domain.Entities.User;

namespace SocialApp.Controllers.v1
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class AccountController : BaseController
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly MailService _mailService;

        public AccountController(ApplicationDbContext context, IConfiguration configuration, MailService mailService)
        {
            _context = context;
            _configuration = configuration;
            _mailService = mailService;
        }

        [HttpPost("signin")]
        public async Task<IActionResult> SignIn([FromBody] SignUpRequest request, CancellationToken token)
        {
            FirebaseAuthProvider provider = new FirebaseAuthProvider(new FirebaseConfig(_configuration.GetSection("FirebaseApiKey").Value));
            
            StreamClientFactory factory = new (_configuration.GetSection("StreamPubKey").Value, _configuration.GetSection("StreamPrivKey").Value);
            
            var userClient = factory.GetUserClient();

            try
            {
                var authLink = await provider.SignInWithEmailAndPasswordAsync(request.Email, request.Password);

                var user = await _context.Users.FirstOrDefaultAsync(item => item.Email == request.Email, token);
                
                if (user is null)
                    return BadRequest("No such user exists");

                var claims = new List<Claim>() {  new Claim("UserId", user.Id.ToString()) }.ToArray();

                var accessToken = JwtService.GenerateJwtToken(30, claims);

                var refreshToken = JwtService.GenerateJwtToken(60, claims);

                var streamToken = userClient.CreateToken(user.Id.ToString(), DateTimeOffset.UtcNow.AddHours(1));
                
                SetTokenCookie(refreshToken);

                return Ok(new { token = accessToken, streamToken });

            } catch (FirebaseAuthException ex)
            {
                return BadRequest(ex.ResponseData);
            }
        }

        [HttpPost("signup")]
        public async Task<IActionResult> SignUp([FromBody] SignUpRequest request, CancellationToken token)
        {
            FirebaseAuthProvider provider = new FirebaseAuthProvider(new FirebaseConfig(_configuration.GetSection("FirebaseApiKey").Value));
            StreamClientFactory factory = new (_configuration.GetSection("StreamPubKey").Value, _configuration.GetSection("StreamPrivKey").Value);
            var userClient = factory.GetUserClient();
            
            try
            {
                var authlink = await provider.CreateUserWithEmailAndPasswordAsync(request.Email, request.Password);

                var user = new User
                {
                    Email = request.Email,
                    Username = request.Email,
                    RegisteredAt = DateTime.UtcNow,
                };
                
                await _context.Users.AddAsync(user, token);

                await _context.SaveChangesAsync(token);
                
                var streamUser = new UserRequest
                {
                    Id = user.Id.ToString(),
                    Role = Role.User,
                    Name = user.Email
                };
                
                await userClient.UpsertAsync(streamUser);

                var claims = new List<Claim>() { new ("UserId", user.Id.ToString()) }.ToArray();

                var accessToken = JwtService.GenerateJwtToken(30, claims);

                var refreshToken = JwtService.GenerateJwtToken(60, Array.Empty<Claim>());

                var streamToken = userClient.CreateToken(user.Id.ToString(), DateTimeOffset.UtcNow.AddHours(1));

                var firebaseToken = authlink.FirebaseToken;
                
                SetTokenCookie(refreshToken);

                return Ok(new { token = accessToken, streamToken, firebaseToken });

            } catch (FirebaseAuthException ex)
            {
                return BadRequest(ex.ResponseData);
            }
        }

        [HttpGet("refresh")]
        public async Task<IActionResult> RefreshToken()
        {
            try
            {
                StreamClientFactory factory = new (_configuration.GetSection("StreamPubKey").Value, _configuration.GetSection("StreamPrivKey").Value);
                
                var userClient = factory.GetUserClient();
                //TODO: Revoke prev refresh token

                string bearerToken = HttpContext.Request.Cookies["RefreshToken"];
                
                var stringToken = bearerToken.Split(" ")?.Last();

                var tokenValid = JwtService.ValidateJwtToken(stringToken);

                var userId = tokenValid.Claims.FirstOrDefault(item => item.Type == "UserId").Value;

                if (tokenValid is null) return Forbid();

                var claims = new List<Claim>() { new Claim("UserId", userId) }.ToArray();

                var newAccessToken = JwtService.GenerateJwtToken(30, claims);

                var newRefreshToken = JwtService.GenerateJwtToken(60, claims);
                
                var streamToken = userClient.CreateToken(userId, DateTimeOffset.UtcNow.AddHours(1));

                SetTokenCookie(newRefreshToken);

                return Ok(new { token = newAccessToken, streamToken });

            } catch (FirebaseAuthException ex)
            {
                return BadRequest(ex.ResponseData);
            }
        }

        [HttpGet("test")]
        public async Task<IActionResult> TestEmail()
        {
            _mailService.SendEmail(new MailServiceRequest() { Body = "Some Body", To = "bekjonibr@gmail.com", Subject = "TestEmail"});
            return Ok();
        }

        [HttpPost("revoke")]
        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> Logout()
        {
            try
            {
                //TODO: Revoke prev refresh token

                string bearerToken = HttpContext.Request.Cookies["RefreshToken"];

                string token = bearerToken.Substring("Bearer ".Length);

                var tokenValid = JwtService.ValidateJwtToken(token);

                if (tokenValid is null) return Forbid();

                return Ok();

            } catch (FirebaseAuthException ex)
            {
                return BadRequest(ex.ResponseData);
            }
        }
    }

}
