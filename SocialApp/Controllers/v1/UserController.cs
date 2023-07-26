using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Persistance;
using SixLabors.ImageSharp.Formats.Jpeg;
using SocialApp.Models;

namespace SocialApp.Controllers.v1
{
    [ApiController]
    [Route("api/v1/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class UserController : BaseController
    {
        private readonly ApplicationDbContext _context;

        public UserController(ApplicationDbContext context)
        {
            _context = context;
        }

        //add pagination
        [HttpGet("list")]
        public async Task<IActionResult> GetUserList([FromQuery] string? q, CancellationToken token)
        {
            return Ok(await _context.Users
                    .Where(item => item.Username.ToLower().Contains(q.ToLower())
                                   || q.Contains(item.Username.ToLower().ToLower())
                                   || item.Email.ToLower().Contains(q.ToLower()))
                                        .ToListAsync(token));
        }

        [HttpGet("")]
        public async Task<IActionResult> GetUser(CancellationToken token)
        {
            var userId = GetUserId();

            var user = await _context.Users
                .Include(item => item.PostBookmarks)
                .AsSingleQuery()
                .FirstOrDefaultAsync(item => item.Id == userId, token);

            var userPosts = await _context.UserPosts
                .Include(item => item.Comments)
                .Include(item => item.Likes)
                .AsSplitQuery()
                .Where(item => item.UserId == userId)
                .ToListAsync(token);
            
            var requests = await _context.UserRequests
                .Where(item => item.UserReceivingRequestId == userId && item.Status == "Pending")
                .ToListAsync(token);
            
            var response = new UserResponse
            {
                Id = userId,
                Username = user.Username,
                UserPosts = userPosts.Select(item => new UserPostResponse
                {
                    Id = item.Id,
                    LowResMediaUrl = item.LowResMediaUrl,
                    Message = item.Message,
                    LikeCount = item.Likes.Count,
                    CommentCount = item.Comments.Count
                }).ToList(),
                UserRequests = requests,
                HighResImageLink = user.HighResImageLink,
                LowResImageLink = user.LowResImageLink,
                ProfileBackgroundImagelink = user.ProfileBackgroundImagelink,
                PostBookmarks = user?.PostBookmarks?.Select(item => item.UserPostId.ToString()).ToList(),
                Intro = user.Intro
            };

            return user is null ? NotFound() : Ok(response);
        }
        
        [HttpGet("{userId}")]
        public async Task<IActionResult> GetUserById(Guid userId, CancellationToken token)
        {

            var currentUserId = GetUserId();
            
            var user = await _context.Users
                .Include(item => item.UserPosts)
                .AsSplitQuery()
                .FirstOrDefaultAsync(item => item.Id == userId, token);

            if (user is null) return NotFound();
            
            var userPosts = await _context.UserPosts
                .Include(item => item.Comments)
                .Include(item => item.Likes)
                .AsSplitQuery()
                .AsNoTracking()
                .Where(item => item.UserId == userId)
                .ToListAsync(token);
            
            var requests = await _context.UserRequests
                .Where(item => item.UserReceivingRequestId == userId && item.Status == "Pending")
                .ToListAsync(token);

            var isFollowing = await _context.UserRequests
                .AnyAsync(item =>
                    item.SenderUserId == currentUserId && item.UserReceivingRequestId == userId &&
                    item.Status == "Accepted", token);
            
            var response = new UserResponse()
            {
                Id = userId,
                Username = user.Username,
                UserPosts = userPosts.Select(item => new UserPostResponse
                {
                    Id = item.Id,
                    LowResMediaUrl = item.LowResMediaUrl,
                    Message = item.Message,
                    LikeCount = item.Likes.Count,
                    CommentCount = item.Comments.Count
                }).ToList(),
                UserRequests = requests,
                HighResImageLink = user.HighResImageLink,
                LowResImageLink = user.LowResImageLink,
                ProfileBackgroundImagelink = user.ProfileBackgroundImagelink,
                IsFollowing = isFollowing
            };
            
            return Ok(response);
        }
        
        [HttpGet("requests/{userId}")]
        public async Task<IActionResult> GetUserRequestsBy(Guid userId, CancellationToken token)
        {
            var requests = await _context.UserRequests
                .Where(item => item.UserReceivingRequestId == userId)
                .ToListAsync(token);
            
            return Ok(requests);
        }
        
        [HttpPut("image")]
        public async Task<IActionResult> UpdateUserProfilePicture([FromForm] UpdateProfilePictureRequest request, CancellationToken token)
        {
            var userId = GetUserId();

            var user = await _context.Users.FirstOrDefaultAsync(item => item.Id == userId, token);

            var lowResImageLink = await ProcessAndUploadImage(request.Image);

            var highResImageLink = await UploadImageToCloudinary(request.Image.Name, request.Image);

            //probably extract out into dependency

            user.HighResImageLink = highResImageLink;

            user.LowResImageLink = lowResImageLink;

            _context.Users.Update(user);
                
            await _context.SaveChangesAsync(token);

            return Ok(new { highResImageLink, lowResImageLink });
        }
        
        [HttpPut("")]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateUserRequest request, CancellationToken token)
        {
            var userId = GetUserId();
            
            var user = await _context.Users.
                FirstOrDefaultAsync(item => item.Id == userId, token);

            user.Username = request.Username;
            
            user.Intro = request.Intro;

            _context.Users.Update(user);

            await _context.SaveChangesAsync(token);

            return Ok(new { Username = user.Username, Intro = user.Intro });
        }

        [HttpPost("backgroundimage")]
        public async Task<IActionResult> UpdateUserBackgroundProfilePicture([FromForm] UpdateProfilePictureRequest request, CancellationToken token)
        {
            var userId = GetUserId();

            var user = await _context.Users.FirstOrDefaultAsync(item => item.Id == userId, token);

            var backgroundImageLink = await UploadImageToCloudinary(request.Image.Name, request.Image);

            user.ProfileBackgroundImagelink = backgroundImageLink;

            _context.Users.Update(user);
                
            await _context.SaveChangesAsync(token);

            return Ok(new { backgroundImageLink });
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

    public class UpdateUserRequest
    {
        public string Username { get; set; }
        public string Intro { get; set; }
    }
}
