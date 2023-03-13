using Domain.Entities;
using Ganss.Xss;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Persistance;

namespace SocialApp.Controllers.v1
{
    [Route("api//v1/[controller]")]
    [ApiController]
    public class Post : ControllerBase
    {
        private readonly ApplicationDbContext _context;


        public Post(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost("/post")]
        public async Task<IActionResult> AddPost(CreatePostRequest request)
        {
            var sanitizer = new HtmlSanitizer();

            var sanitizedHtmlInput = sanitizer.Sanitize(request.HtmlContent);

            var postModel = new UserPost
            {
                Message = sanitizedHtmlInput,
                CreatedAt = DateTime.UtcNow,
                UserId = 123
            };

            await _context.UserPosts.AddAsync(postModel);

            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpGet("/")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public IActionResult DummyGet()
        {
            return Ok();
        }
    }

    public class CreatePostRequest
    {
        public string Title { get; set; }
        public string HtmlContent { get; set; }
        public IFormFile Image { get; set; }
    } 
}
