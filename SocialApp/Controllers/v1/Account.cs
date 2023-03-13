using Firebase.Auth;
using FirebaseAdmin;
using FirebaseAdmin.Auth;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Persistance;
using SocialApp.Models;
using SocialApp.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

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

        [HttpPost("signin")]
        public async Task<IActionResult> SignIn([FromBody] SignUpRequest request)
        {
            FirebaseAuthProvider provider = new FirebaseAuthProvider(new FirebaseConfig(ApiKey));

            try
            {
                var authLink = await provider.SignInWithEmailAndPasswordAsync(request.Email, request.Password);
                //authLink.

                var accessToken = JwtService.GenerateJwtToken(Array.Empty<Claim>(), 30);

                var refreshToken = JwtService.GenerateJwtToken(Array.Empty<Claim>(), 60);

                return Ok(new { token = accessToken, refreshToken = refreshToken });

            } catch (Firebase.Auth.FirebaseAuthException ex)
            {
                return BadRequest(ex.ResponseData);
            }
        }

        [HttpPost("signup")]
        public async Task<IActionResult> SignUp([FromBody] SignUpRequest request)
        {
            FirebaseAuthProvider provider = new FirebaseAuthProvider(new FirebaseConfig(ApiKey));

            try
            {
                var authLink = await provider.CreateUserWithEmailAndPasswordAsync(request.Email, request.Password);

                var accessToken = JwtService.GenerateJwtToken(Array.Empty<Claim>(), 30);

                var refreshToken = JwtService.GenerateJwtToken(Array.Empty<Claim>(), 60);


                return Ok(new { token = accessToken, refreshToken = refreshToken });

            } catch (Firebase.Auth.FirebaseAuthException ex)
            {
                return BadRequest(ex.ResponseData);
            }
        }

        [HttpPost("refresh")]
        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> RefreshToken([FromBody] SignUpRequest request)
        {
            FirebaseAuthProvider provider = new FirebaseAuthProvider(new FirebaseConfig(ApiKey));
            //provider.RefreshAuthAsync

            //Firebase.Auth.FirebaseAuth auth = new Firebase.Auth.FirebaseAuth();

            //var link = await provider.RefreshAuthAsync(auth);

            string bearerToken = HttpContext.Request.Cookies["RefreshToken"];

            string token = bearerToken.Substring("Bearer ".Length);

            //var instance = FirebaseAdmin.Auth.FirebaseAuth.GetAuth(_firebaseApp);
            
            //var firebaseToken = instance.VerifyIdTokenAsync(token, true);

            //provider.SignInWithGoogleIdTokenAsync

            //var user = await provider.GetUserAsync(token);
            //var newToken = user.
            //var newToken = await provider.SignInWithEmailAndPasswordAsync();
            //await instance.Re

            try
            {
                //var authLink = await provider.RefreshAuthAsync(request.Email, request.Password);

                return Ok(new { token = "" });

            } catch (Firebase.Auth.FirebaseAuthException ex)
            {
                return BadRequest(ex.ResponseData);
            }
        }
        private IEnumerable<Claim>ToClaims(IReadOnlyDictionary<string, object> claims)
        {
            return new List<Claim> {
                new Claim("id", claims["user_id"].ToString()),
                new Claim ("email", claims["email"].ToString()),
                new Claim("time", claims["auth_time"].ToString()),
            };
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
