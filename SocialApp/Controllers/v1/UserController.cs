using Domain.Entities;
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
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class UserController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        private readonly UserFileService _fileService;

        public UserController(ApplicationDbContext context, UserFileService fileService)
        {
            _context = context;
            _fileService = fileService;
        }

        private string? GetUserId()
        {
            return User.Claims.FirstOrDefault(item => item.Type == "UserId")?.Value;
        }

        [HttpGet("list")]
        public async Task<IActionResult> GetUserList([FromQuery] string? q)
        {
            var userList = _context.Users.AsQueryable();

            userList = userList.Where(item => item.Email.Contains(q));

            var fileList = new List<UserDocument>();

            var files = _fileService.GetAll();

            foreach(var user in userList)
            {

                foreach(var file in files)
                {
                    if (user.Id == file.UserId)
                    {
                        user.Picture = Convert.ToBase64String(file.UserLowResolutionImage);
                    }
                }

            }

            return Ok(await userList.ToListAsync());
        }

        [HttpGet("")]
        public async Task<IActionResult> GetUser()
        {
            var userId = int.Parse(GetUserId());

            var user = await _context.Users.FirstOrDefaultAsync(item => item.Id == userId);

            var userImage = await _fileService.GetFileByUserIdAsync(userId);

            var base64String = Convert.ToBase64String(userImage.UserImage);

            var base64StringLowRes = Convert.ToBase64String(userImage.UserLowResolutionImage);

            var highResSrc = $"data:image/{GetFileExtension(userImage.UserImageName)};base64,{base64String}";

            var lowResSrc = $"data:image/{GetFileExtension(userImage.UserImageName)};base64,{base64StringLowRes}";

            return Ok(new { username = user.Username, userImageSrc = highResSrc, lowResUserImageSrc = lowResSrc });
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetUserById(int userId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(item => item.Id == userId);

            var userImage = await _fileService.GetFileByUserIdAsync(userId);

            if (userImage is not null)
            {
                var base64String = Convert.ToBase64String(userImage.UserImage);

                var base64StringLowRes = Convert.ToBase64String(userImage.UserLowResolutionImage);

                var highResSrc = $"data:image/{GetFileExtension(userImage.UserImageName)};base64,{base64String}";

                var lowResSrc = $"data:image/{GetFileExtension(userImage.UserImageName)};base64,{base64StringLowRes}";

                return Ok(new { username = user.Username, userImageSrc = highResSrc, lowResUserImageSrc = lowResSrc });
            }

            return Ok(new { username = user.Username });
        }

        [HttpGet("profileimage")]
        public async Task<IActionResult> GetProfileImageByUserId()
        {
            var userId = int.Parse(GetUserId());

            var user = await _context.Users.FirstOrDefaultAsync(item => item.Id == userId);

            var userImage = await _fileService.GetFileByUserIdAsync(userId);

            if (userImage is not null)
            {
                var base64StringLowRes = Convert.ToBase64String(userImage.UserLowResolutionImage);

                var lowResSrc = $"data:image/{GetFileExtension(userImage.UserImageName)};base64,{base64StringLowRes}";

                return Ok(new { lowResUserImageSrc = lowResSrc });
            }

            return Ok(new { lowResUserImageSrc = "" });
        }

        [HttpPut("image")]
        public async Task<IActionResult> UpdateUserProfilePicture([FromForm] UpdateProfilePictureRequest request, CancellationToken token)
        {
            var userId = int.Parse(GetUserId());

            var user = await _context.Users.FirstOrDefaultAsync(item => item.Id == userId);

            var lowResImage = await GenerateLowResImage(request.Image);

            byte[] lowResBytes;

            using (var stream = new MemoryStream())
            {
                await lowResImage.SaveAsync(stream, new JpegEncoder(), token);
                lowResBytes = stream.ToArray();
            }

            byte[] highResBytes;

            using (var stream = new MemoryStream())
            {
                await request.Image.CopyToAsync(stream);
                highResBytes = stream.ToArray();
            }

            UserDocument userImage = new UserDocument();
            var imageId = MongoDB.Bson.ObjectId.GenerateNewId();

            if (user?.ImageId is not null)
            {
                await _fileService.DeleteAsync(user.Id);
                //userImage = await _fileService.GetByIdAsync(user.ImageId);
                userImage.UserLowResolutionImage = lowResBytes;
                userImage.UserId = userId;
                user.ImageId = imageId.ToString();
                userImage.UserImageName = request.Image.FileName;
                userImage.UserImage = highResBytes;
                await _fileService.InsertAsync(userImage);
            }
            else
            {

                var newUserImage = new UserDocument
                {
                    Id = imageId,
                    UserId = userId,
                    UserLowResolutionImage = lowResBytes,
                    UserImage = highResBytes,
                    UserImageName = request.Image.FileName
                };

                if (user.ImageId is null)
                {
                    user.ImageId = imageId.ToString();
                    userImage.UserId = userId;
                }

                await _fileService.InsertAsync(newUserImage);

            }

            _context.Users.Update(user);
                
            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpPost("blockeduser")]
        public async Task<IActionResult> BlockUser([FromBody] BlockUserRequest request)
        {
            if (await _context.UsersBlocked.AnyAsync(item => item.UserBlockedId == request.BlockedUserId))
                return BadRequest("User is already blocked");

            var userId = int.Parse(GetUserId());

            var userBlocked = new UserBlocked
            {
                UserBlockedId = request.BlockedUserId,
                UserId = userId,    
            };

            await _context.UsersBlocked.AddAsync(userBlocked);

            await _context.SaveChangesAsync();

            return Ok();
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
    public class BlockUserRequest
    {
        public int BlockedUserId { get; set; }
    }
    public class UpdateProfilePictureRequest
    {
        public IFormFile Image { get; set; }
    }
}
