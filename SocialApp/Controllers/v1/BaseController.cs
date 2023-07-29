using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Mvc;

namespace SocialApp.Controllers.v1;

public class BaseController : ControllerBase
{
    protected void SetTokenCookie(string token)
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
    protected Guid GetUserId()
    {
        return Guid.Parse(User.Claims.FirstOrDefault(item => item.Type == "UserId")?.Value!);
    }
}