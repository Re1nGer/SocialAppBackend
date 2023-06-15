using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Domain.Entities;
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
    public class UserController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public UserController(ApplicationDbContext context)
        {
            _context = context;
        }

        private Guid GetUserId()
        {
            return Guid.Parse(User.Claims.FirstOrDefault(item => item.Type == "UserId")?.Value);
        }

        //add pagination
        [HttpGet("list")]
        public async Task<IActionResult> GetUserList([FromQuery] string? q)
        {
            return Ok(await _context.Users.Where(item => item.Username.Contains(q) || q.Contains(item.Username)).ToListAsync());
        }

        [HttpGet("")]
        public async Task<IActionResult> GetUser()
        {
            var userId = GetUserId();

            var user = await _context.Users
                .Include(item => item.UserPosts)
                .FirstOrDefaultAsync(item => item.Id == userId);
            
            var requests = await _context.UserRequests
                .Where(item => item.UserReceivingRequestId == userId && item.Status == "Pending")
                .ToListAsync();
            
            var response = new UserResponse()
            {
                Id = userId,
                Username = user.Username,
                UserPosts = user.UserPosts.ToList(),
                UserRequests = requests,
                HighResImageLink = user.HighResImageLink,
                LowResImageLink = user.LowResImageLink,
                ProfileBackgroundImagelink = user.ProfileBackgroundImagelink
            };

            return user is null ? NotFound() : Ok(response);
        }
        
        [HttpGet("{userId}")]
        public async Task<IActionResult> GetUserById(Guid userId)
        {
            var user = await _context.Users
                .Include(item => item.UserPosts)
                .FirstOrDefaultAsync(item => item.Id == userId);
            

            return Ok(user);
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

            var user = await _context.Users.FirstOrDefaultAsync(item => item.Id == userId);

            var lowResImage = await ProcessAndUploadImage(request.Image);

            var highResImage = await UploadImageToCloudinary(request.Image.Name, request.Image);

            //probably extract out into dependency

            user.HighResImageLink = highResImage;

            user.LowResImageLink = lowResImage;

            _context.Users.Update(user);
                
            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpPost("backgroundimage")]
        public async Task<IActionResult> UpdateUserBackgroundProfilePicture([FromForm] UpdateProfilePictureRequest request, CancellationToken token)
        {
            var userId = GetUserId();

            var user = await _context.Users.FirstOrDefaultAsync(item => item.Id == userId);

            var backgroundImageLink = await UploadImageToCloudinary(request.Image.Name, request.Image);

            user.ProfileBackgroundImagelink = backgroundImageLink;

            _context.Users.Update(user);
                
            await _context.SaveChangesAsync();

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
    public class UpdateProfilePictureRequest
    {
        public IFormFile Image { get; set; }
    }
}
