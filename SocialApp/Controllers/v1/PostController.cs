using Domain.Entities;
using Ganss.Xss;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Persistance;
using SixLabors.ImageSharp.Formats.Jpeg;
using SocialApp.Models;
using SocialApp.Services;

namespace SocialApp.Controllers.v1
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class PostController : ControllerBase
    {
        private string? GetUserId()
        {
            return User.Claims.FirstOrDefault(item => item.Type == "UserId")?.Value;
        }

        private readonly ApplicationDbContext _context;
        private readonly FileService _fileService;

        public PostController(ApplicationDbContext context, FileService fileService)
        {
            _context = context;
            _fileService = fileService;
        }

        [HttpPost("")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> AddPost([FromForm] CreatePostRequest request, CancellationToken cancellationToken)
        {

            if (!IsImage(request.Image))
            {
                return BadRequest("Invalid file type. Only images are allowed.");
            }

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

                var lowresImage = await GenerateLowResImage(request.Image);

                await _context.SaveChangesAsync(cancellationToken);

                using (var memoryStream = new MemoryStream())
                {
                    await request.Image.CopyToAsync(memoryStream);

                    byte[] lowresBytes;

                    using (var stream = new MemoryStream())
                    {
                        await lowresImage.SaveAsync(stream, new JpegEncoder(), cancellationToken);
                        lowresBytes = stream.ToArray();
                    }

                    var document = new FileDocument
                    {

                        Filename = request.Image.FileName,
                        Image = memoryStream.ToArray(),
                        UserId = userId,
                        PostId = postModel.Id,
                        LowResolutionImage = lowresBytes
                    };

                    await _fileService.InsertAsync(document);
                }

                return Ok();

            } catch (Exception ex)
            {
                return BadRequest(new ErrorModel { Message = ex.Message});  
            }

        }

        [HttpGet("list")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetAllUserPosts(CancellationToken token)
        {
            var userPosts = await _context.UserPosts
                .Include(item => item.Comments)
                .Include(item => item.Likes)
                .Where(item => item.UserId == int.Parse(GetUserId()))
                .AsSplitQuery()
                .AsNoTrackingWithIdentityResolution()
                .ToListAsync(token);
            
            var files = _fileService.Where(item => item.UserId == int.Parse(GetUserId())).ToList();

            var model = userPosts.Select(item => new PostModel
            {
                Id =  item.Id,
                HtmlContent = item.Message,
                ImgSrc = Convert.ToBase64String(files.FirstOrDefault(image => image.PostId == item.Id).LowResolutionImage),
                CommentCount = item.Comments is null ? 0 : item.Comments.Count,
                LikeCount = item.Likes is null ? 0 : item.Likes.Count,
            });

            return Ok(model);
        }

        [HttpGet("{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetPostById(int id)
        {
            var userPost = await _context.UserPosts
                .Include(item => item.Comments)
                .Include(item => item.Likes)
                .FirstOrDefaultAsync(item => item.UserId == int.Parse(GetUserId()) && item.Id == id);

            var file = _fileService
                .Where(item => item.PostId == userPost.Id).FirstOrDefault();

            var likeCount = await _context.UserLikes.Where(item => item.PostId == id).CountAsync();

            var commentCount = await _context.UserComments.Where(item => item.PostId == id).CountAsync();

            var model =  new PostModel
            {
                Id =  userPost.Id,
                HtmlContent = userPost.Message,
                //ImgSrc = Convert.ToBase64String(file.Image),
                LikeCount = likeCount,
                CommentCount = commentCount,
            };

            return Ok(model);
        }

        [HttpGet("highres/{postId}")]
        public async Task<IActionResult> GetHighResolutionImage(int postId)
        {
            // Find the post with the given postId
            var file = await _fileService.GetFileByPostIdAsync(postId);

            // Check if the post has a file attached
            if (file is null)
            {
                return NotFound("File not found");
            }

            //file.Filename.

            // Return the file as a stream
            //var stream = new MemoryStream(file.Image);
            var base64String = Convert.ToBase64String(file.Image);
            var src = $"data:image/{GetFileExtension(file.Filename)};base64,{base64String}";
            return Ok(src);
        }

        [HttpGet("lowres/{postId}")]
        public async Task<IActionResult> LowResImage(int postId)
        {
            // Find the post with the given postId
            var file = await _fileService.GetFileByPostIdAsync(postId);

            // Check if the post has a file attached
            if (file is null)
            {
                return NotFound("File not found");
            }

            //file.Filename.

            // Return the file as a stream
            //var stream = new MemoryStream(file.Image);
            var base64String = Convert.ToBase64String(file.LowResolutionImage);
            var src = $"data:image/{GetFileExtension(file.Filename)};base64,{base64String}";
            return Ok(src);
        }

        
        private bool IsImage(IFormFile file)
        {
            return file.ContentType.StartsWith("image/");
        }

        private async Task<Image> GenerateLowResImage(IFormFile file)
        {
            using (var stream = file.OpenReadStream())
            {
                var image = await Image.LoadAsync(stream);
                image.Mutate(x => x.Resize(new ResizeOptions
                {
                    Size = new Size(500, 500),
                    Mode = ResizeMode.Max
                }));
                return image;
            }
        }

        private string? GetFileExtension(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                return null;
            }

            int lastDotIndex = fileName.LastIndexOf('.');
            if (lastDotIndex == -1)
            {
                return null;
            }

            return fileName.Substring(lastDotIndex + 1);
        }

    }
    public class PostModel
    {
        public int Id { get; set; }
        public string HtmlContent { get; set; }
        public string ImgSrc { get; set; }
        public int LikeCount { get; set; }
        public int CommentCount { get; set; }
    }

    public class ErrorModel
    {
        public string Message { get; set; }
    }

    public class CreatePostRequest
    {
        public string HtmlContent { get; set; }
        public IFormFile Image { get; set; }
    } 
}
