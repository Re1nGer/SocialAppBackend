using Domain.Entities;
using Ganss.Xss;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Persistance;
using SocialApp.Models;
using SocialApp.Services;

namespace SocialApp.Controllers.v1
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class Post : ControllerBase
    {
        private string? GetUserId()
        {
            return User.Claims.FirstOrDefault(item => item.Type == "UserId")?.Value;
        }

        private readonly ApplicationDbContext _context;
        private readonly FileService _fileService;

        public Post(ApplicationDbContext context, FileService fileService)
        {
            _context = context;
            _fileService = fileService;
        }

        [HttpPost("/post")]
        public async Task<IActionResult> AddPost([FromForm] CreatePostRequest request, CancellationToken cancellationToken)
        {
            var sanitizer = new HtmlSanitizer();

            var sanitizedHtmlInput = sanitizer.Sanitize(request.HtmlContent);

            var userId = int.Parse(GetUserId());

            try
            {
                var postModel = new UserPost
                {
                    Message = sanitizedHtmlInput,
                    CreatedAt = DateTime.UtcNow,
                    UserId = userId
                };

                await _context.UserPosts.AddAsync(postModel, cancellationToken);

                await _context.SaveChangesAsync(cancellationToken);

                using (var memoryStream = new MemoryStream())
                {
                    await request.Image.CopyToAsync(memoryStream);

                    var document = new FileDocument
                    {

                        Filename = request.Image.FileName,
                        Data = memoryStream.ToArray(),
                        UserId = userId,
                        PostId = postModel.Id
                    };

                    await _fileService.InsertAsync(document);

                    return Ok();
                }

            } catch (Exception ex)
            {
                return BadRequest(ex.Message);  
            }

        }

        [HttpGet("/list")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetAllUserPosts()
        {
            var userPosts = await _context.UserPosts
                .Where(item => item.UserId == int.Parse(GetUserId()))
                .ToListAsync();

            var files = _fileService.Where(item => item.UserId == int.Parse(GetUserId())).ToList();

            var model = userPosts.Select(item => new PostModel
            {
                Id =  item.Id,
                HtmlContent = item.Message,
                ImgSrc = Convert.ToBase64String(files.FirstOrDefault(image => image.PostId == item.Id).Data),
            });

            return Ok(model);
        }

        [HttpGet("/")]
        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public IActionResult DummyGet()
        {
            return Ok();
        }
    }
    public class PostModel
    {
        public int Id { get; set; }
        public string HtmlContent { get; set; }
        public string ImgSrc { get; set; }
    }

    public class CreatePostRequest
    {
        public string HtmlContent { get; set; }
        public IFormFile Image { get; set; }
    } 
}
