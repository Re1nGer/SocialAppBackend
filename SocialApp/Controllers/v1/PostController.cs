using CloudinaryDotNet;
using Ganss.Xss;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CloudinaryDotNet.Actions;
using Persistance;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using SixLabors.ImageSharp.Formats.Jpeg;

namespace SocialApp.Controllers.v1
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class PostController : ControllerBase
    {
        private Guid GetUserId()
        {
            return Guid.Parse(User.Claims.FirstOrDefault(item => item.Type == "UserId")?.Value);
        }

        private readonly ApplicationDbContext _context;

        public PostController(ApplicationDbContext context)
        {
            _context = context;
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

            var userId = GetUserId();

            try
            {
                var postModel = new UserPost
                {
                    Message = sanitizedHtmlInput,
                    CreatedAt = DateTime.UtcNow,
                    UserId = userId,
                };

                await _context.UserPosts.AddAsync(postModel, cancellationToken);

                var lowresImageUrl = await ProcessAndUploadImage(request.Image);

                var highResImageUrl = await UploadImageToCloudinary(request.Image.Name, request.Image);

                postModel.LowResMediaUrl = lowresImageUrl;

                postModel.MediaUrl = highResImageUrl;

                await _context.SaveChangesAsync(cancellationToken);

                return Ok();

            }
            catch (Exception ex)
            {
                return BadRequest(new ErrorModel { Message = ex.Message });
            }
        }

        [HttpGet("list")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetAllUserPosts(CancellationToken token)
        {
            var userPosts = await _context.UserPosts
                .Include(item => item.Comments)
                .Include(item => item.Likes)
                .Where(item => item.UserId == GetUserId())
                .AsSplitQuery()
                .AsNoTrackingWithIdentityResolution()
                .ToListAsync(token);

            return Ok(userPosts);
        }

        [HttpGet("{userId}/list")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetPostsByUserId(Guid userId, CancellationToken token)
        {
            var userPosts = await _context.UserPosts
                .Include(item => item.Comments)
                .Include(item => item.Likes)
                .Where(item => item.UserId == userId)
                .AsSplitQuery()
                .AsNoTrackingWithIdentityResolution()
                .ToListAsync(token);
            
            return Ok(userPosts);
        }

        [HttpGet("{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetPostById(Guid id)
        {
            var userPost = await _context.UserPosts
                .Include(item => item.Comments)
                .Include(item => item.Likes)
                .FirstOrDefaultAsync(item => item.Id == id);

            var model = new PostModel
            {
                Id =  userPost.Id,
                HtmlContent = userPost.Message,
                LikeCount = userPost.Likes.Count,
                CommentCount = userPost.Comments.Count,
                MediaUrl = userPost.MediaUrl
            };

            return Ok(model);
        }

        private bool IsImage(IFormFile file)
        {
            return file.ContentType.StartsWith("image/");
        }
        private async Task<string> ProcessAndUploadImage(IFormFile formFile)
        {
            using (Image image = Image.Load(formFile.OpenReadStream()))
            {
                Image mutatedImage = MutateImage(image);

                byte[] imageBytes = ConvertImageToBytes(mutatedImage);
                string imageName = formFile.FileName;

                return await UploadImageToCloudinary(imageName, imageBytes);
            }
        }

        private Image MutateImage(Image image)
        {
            image.Mutate(x => x.Resize(new ResizeOptions
            {
                Size = new SixLabors.ImageSharp.Size(500, 500),
                Mode = ResizeMode.Max
            }));

            return image;
        }
        private byte[] ConvertImageToBytes(Image image)
        {
            using (var stream = new MemoryStream())
            {
                image.Save(stream, new JpegEncoder()); // Specify the appropriate encoder based on your image format
                return stream.ToArray();
            }
        }

        private async Task<string> UploadImageToCloudinary(string imageName, byte[] imageBytes)
        {
            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(imageName, new MemoryStream(imageBytes))
            };

            var cloudinary = new Cloudinary(new Account("dcjubzmeu", "248951749718234", "9SNdW4kehk_tR6jY4rkDexcz928"));

            ImageUploadResult uploadResult = await cloudinary.UploadAsync(uploadParams);

            return uploadResult.SecureUrl.ToString();
        }
        private async Task<string> UploadImageToCloudinary(string imageName, IFormFile image)
        {
            using var stream = image.OpenReadStream();

            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(imageName, stream)
            };

            var cloudinary = new Cloudinary(new Account("dcjubzmeu", "248951749718234", "9SNdW4kehk_tR6jY4rkDexcz928"));

            ImageUploadResult uploadResult = await cloudinary.UploadAsync(uploadParams);

            return uploadResult.SecureUrl.ToString();
        }
    }
    public class PostModel
    {
        public Guid Id { get; set; }
        public string HtmlContent { get; set; }
        public string MediaUrl { get; set; }
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
