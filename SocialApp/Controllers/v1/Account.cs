using Firebase.Auth;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Mvc;
using SocialApp.Models;
using SocialApp.Services;

namespace SocialApp.Controllers.v1
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class Account : ControllerBase
    {
        const string ApiKey = "AIzaSyAXgEXuez0ev-u2yOyK9oWFWv_6HJPnAwI";
        private readonly AuthServices _authService;

        public Account(AuthServices authService)
        {
            _authService = authService;
        }

        [HttpPost("google")]
        public IActionResult GoogleAuthenticate([FromBody] GoogleAuthenticateRequest request)
        {
            var payload = GoogleJsonWebSignature.ValidateAsync(request.GoogleToken, new GoogleJsonWebSignature.ValidationSettings()).Result;

            var user = _authService.Authenticate(payload);

            return Ok(user);
        }


        [HttpPost("signup")]
        public async Task<IActionResult> SignUp([FromBody] SignUpRequest request)
        {
            FirebaseAuthProvider provider = new FirebaseAuthProvider(new FirebaseConfig(ApiKey));

            try
            {
                var authLink = await provider.CreateUserWithEmailAndPasswordAsync(request.Email, request.Password);

                return Ok(new { token = authLink.FirebaseToken });

            } catch (FirebaseAuthException ex)
            {
                return BadRequest(ex.ResponseData);
            }
        }
    }


    public class SignUpRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }    
    }
}
