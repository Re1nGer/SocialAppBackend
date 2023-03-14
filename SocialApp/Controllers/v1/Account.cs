using Microsoft.AspNetCore.Mvc;
using Persistance;
using SocialApp.Services;
using System.Security.Claims;
using Firebase.Auth;
using Microsoft.EntityFrameworkCore;

namespace SocialApp.Controllers.v1
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class Account : ControllerBase
    {
        const string ApiKey = "AIzaSyAXgEXuez0ev-u2yOyK9oWFWv_6HJPnAwI";

        private readonly ApplicationDbContext _context;

        public Account(ApplicationDbContext context)
        {
            _context = context;
        }

        private string GetUserId()
        {
            return User.Claims.FirstOrDefault(x => x.Type == "UserId")?.Value ?? "";
        }

        [HttpPost("signin")]
        public async Task<IActionResult> SignIn([FromBody] SignUpRequest request)
        {
            FirebaseAuthProvider provider = new FirebaseAuthProvider(new FirebaseConfig(ApiKey));

            try
            {
                var authLink = await provider.SignInWithEmailAndPasswordAsync(request.Email, request.Password);
                //authLink.
                //var user = await provider.GetUserAsync(authLink);

                var user = await _context.Users.FirstOrDefaultAsync(item => item.Email == authLink.User.Email);

                var claims = new List<Claim>() {  new Claim("UserId", user.Id.ToString()) }.ToArray();

                var accessToken = JwtService.GenerateJwtToken(claims, 30);

                var refreshToken = JwtService.GenerateJwtToken(Array.Empty<Claim>(), 60);

                HttpContext.Response.Cookies.Append("RefreshToken", refreshToken);

                return Ok(new { token = accessToken });

            } catch (FirebaseAuthException ex)
            {
                return BadRequest(ex.ResponseData);
            }
        }

        [HttpPost("signup")]
        public async Task<IActionResult> SignUp([FromBody] SignUpRequest request, CancellationToken token)
        {
            FirebaseAuthProvider provider = new FirebaseAuthProvider(new FirebaseConfig(ApiKey));

            try
            {
                var authLink = await provider.CreateUserWithEmailAndPasswordAsync(request.Email, request.Password);

                var user = new Domain.Entities.User
                {
                    Email = request.Email,
                    Username = request.Email,
                    RegisteredAt = DateTime.UtcNow,
                };

                await _context.Users.AddAsync(user, token);

                var claims = new List<Claim>() { new Claim("UserId", user.Id.ToString()) }.ToArray();

                var accessToken = JwtService.GenerateJwtToken(claims, 30);

                var refreshToken = JwtService.GenerateJwtToken(Array.Empty<Claim>(), 60);

                HttpContext.Response.Cookies.Append("RefreshToken", refreshToken);

                await _context.SaveChangesAsync(token);

                return Ok(new { token = accessToken });

            } catch (FirebaseAuthException ex)
            {
                return BadRequest(ex.ResponseData);
            }
        }

        [HttpPost("refresh")]
        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> RefreshToken([FromBody] SignUpRequest request)
        {
            try
            {
                //TODO: Revoke prev refresh token

                string bearerToken = HttpContext.Request.Cookies["RefreshToken"];

                string token = bearerToken.Substring("Bearer ".Length);

                var tokenValid = JwtService.ValidateJwtToken(token);

                if (tokenValid is null) return Forbid();

                var claims = new List<Claim>() { new Claim("UserId", GetUserId()) }.ToArray();

                var newAccessToken = JwtService.GenerateJwtToken(claims, 30);

                var newRefreshToken = JwtService.GenerateJwtToken(Array.Empty<Claim>(), 60);

                HttpContext.Response.Cookies.Append("RefreshToken", newRefreshToken);

                return Ok(new { token = newAccessToken });

            } catch (FirebaseAuthException ex)
            {
                return BadRequest(ex.ResponseData);
            }
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

    public class RefreshTokenRequest
    {

    }

    public class SignUpRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }    
    }
}
