using Microsoft.AspNetCore.Mvc;
using Persistance;
using SocialApp.Services;
using System.Security.Claims;
using Firebase.Auth;
using Microsoft.EntityFrameworkCore;
using SocialApp.Models;
using User = Domain.Entities.User;

namespace SocialApp.Controllers.v1
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        const string ApiKey = "AIzaSyAXgEXuez0ev-u2yOyK9oWFWv_6HJPnAwI";

        private readonly ApplicationDbContext _context;

        public AccountController(ApplicationDbContext context)
        {
            _context = context;
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

                var accessToken = JwtService.GenerateJwtToken(30, claims);

                var refreshToken = JwtService.GenerateJwtToken(60, claims);
                
                SetTokenCookie(refreshToken);

                return Ok(new { token = accessToken });

            } catch (FirebaseAuthException ex)
            {
                return BadRequest(ex.ResponseData);
            }
        }
        
        [HttpPost("signinwithgoogle")]
        public async Task<IActionResult> SignInWithGoogle([FromBody] SignInGoogleRequest request, CancellationToken token)
        {
            FirebaseAuthProvider provider = new FirebaseAuthProvider(new FirebaseConfig(ApiKey));

            try
            {
                var authLink = await provider.SignInWithGoogleIdTokenAsync(request.GoogleIdToken);
                
                //authLink.
                //var user = await provider.GetUserAsync(authLink);

                var user = await _context.Users.FirstOrDefaultAsync(item => item.Email == authLink.User.Email, token);

                if (user is null)
                {
                    var newUser = new User
                    {
                        Email = authLink.User.Email,
                        Username = authLink.User.DisplayName,
                        RegisteredAt = DateTime.UtcNow,
                    };

                    await _context.Users.AddAsync(newUser, token);

                    await _context.SaveChangesAsync(token);
                }
                

                var claims = new List<Claim>() {  new Claim("UserId", user.Id.ToString()) }.ToArray();

                var accessToken = JwtService.GenerateJwtToken(30, claims);

                var refreshToken = JwtService.GenerateJwtToken(60, claims);
                
                SetTokenCookie(refreshToken);

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
                await provider.CreateUserWithEmailAndPasswordAsync(request.Email, request.Password);

                var user = new User
                {
                    Email = request.Email,
                    Username = request.Email,
                    RegisteredAt = DateTime.UtcNow,
                };

                await _context.Users.AddAsync(user, token);

                await _context.SaveChangesAsync(token);

                var claims = new List<Claim>() { new Claim("UserId", user.Id.ToString()) }.ToArray();

                var accessToken = JwtService.GenerateJwtToken(30, claims);

                var refreshToken = JwtService.GenerateJwtToken(60, Array.Empty<Claim>());

                SetTokenCookie(refreshToken);

                return Ok(new { token = accessToken });

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
                //TODO: Revoke prev refresh token

                string bearerToken = HttpContext.Request.Cookies["RefreshToken"];
                
                var stringToken = bearerToken.Split(" ")?.Last();

                var tokenValid = JwtService.ValidateJwtToken(stringToken);

                var userId = tokenValid.Claims.FirstOrDefault(item => item.Type == "UserId").Value;

                if (tokenValid is null) return Forbid();

                var claims = new List<Claim>() { new Claim("UserId", userId) }.ToArray();

                var newAccessToken = JwtService.GenerateJwtToken(30, claims);

                var newRefreshToken = JwtService.GenerateJwtToken(60, claims);

                SetTokenCookie(newRefreshToken);

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
        private void SetTokenCookie(string token)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = DateTime.UtcNow.AddHours(1),
                SameSite = SameSiteMode.None,
                Secure = true,
                IsEssential = true,
            };
            Response.Cookies.Append("RefreshToken", token, cookieOptions);
        }
    }

}
