using Microsoft.AspNetCore.Mvc;

namespace SocialApp.Controllers.v1;

public class BaseController : ControllerBase
{
    protected Guid GetUserId()
    {
        return Guid.Parse(User.Claims.FirstOrDefault(item => item.Type == "UserId")?.Value!);
    }
}