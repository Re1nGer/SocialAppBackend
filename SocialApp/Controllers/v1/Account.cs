using Google.Apis.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SocialApp.Models;
using SocialApp.Services;

namespace SocialApp.Controllers.v1
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class Account : ControllerBase
    {
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
    }
}
