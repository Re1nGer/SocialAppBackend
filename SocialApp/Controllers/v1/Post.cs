using Domain.Entities;
using Ganss.Xss;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Persistance;
using SocialApp.Models;
using SocialApp.Services;

namespace SocialApp.Controllers.v1
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class Post : ControllerBase
    {

        private readonly ApplicationDbContext _context;
        private readonly FileService _fileService;

        public Post(ApplicationDbContext context, FileService fileService)
        {
            _context = context;
            _fileService = fileService;
        }

        [HttpPost("/post")]
        public async Task<IActionResult> AddPost([FromForm] CreatePostRequest request)
        {
            var sanitizer = new HtmlSanitizer();

            var sanitizedHtmlInput = sanitizer.Sanitize(request.HtmlContent);

            try
            {
                var postModel = new UserPost
                {
                    Message = sanitizedHtmlInput,
                    CreatedAt = DateTime.UtcNow,
                    UserId = 123
                };

                await _context.UserPosts.AddAsync(postModel);

                using (var memoryStream = new MemoryStream())
                {
                    await request.Image.CopyToAsync(memoryStream);

                    var document = new FileDocument
                    {
                        Filename = request.Image.FileName,
                        Data = memoryStream.ToArray()
                    };

                    await _fileService.InsertAsync(document);

                    await _context.SaveChangesAsync();

                    return Ok();
                }

            } catch (Exception ex)
            {
                return BadRequest(ex.Message);  
            }

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
