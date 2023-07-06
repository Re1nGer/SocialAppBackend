﻿using CloudinaryDotNet;
using Ganss.Xss;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CloudinaryDotNet.Actions;
using Persistance;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using SixLabors.ImageSharp.Formats.Jpeg;
using SocialApp.Models;

namespace SocialApp.Controllers.v1
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class PostController : BaseController
    {
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

                if (request.Image is not null)
                {
                    var lowresImageUrl = await ProcessAndUploadImage(request.Image);

                    var highResImageUrl = await UploadImageToCloudinary(request.Image.Name, request.Image);

                    postModel.LowResMediaUrl = lowresImageUrl;

                    postModel.MediaUrl = highResImageUrl;
                }

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
            var userId = GetUserId();
            
            var userPosts = await _context.UserPosts
                .Include(item => item.Comments)
                .Include(item => item.Likes)
                .Where(item => item.UserId == userId)
                .AsSplitQuery()
                .AsNoTrackingWithIdentityResolution()
                .Select(item => new PostModel
                {
                    Id = item.Id,
                    Message = item.Message,
                    LikeCount = item.Likes.Count,
                    CommentCount = item.Comments.Count,
                    MediaUrl = item.LowResMediaUrl
                })
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
        public async Task<IActionResult> GetPostById(Guid id, CancellationToken token)
        {
            var userId = GetUserId();
            
            var userPost = await _context.UserPosts
                .AsNoTracking()
                .FirstOrDefaultAsync(item => item.Id == id, token);

            if (userPost is null) return NotFound();
            
            var user = await _context.Users
                .FirstOrDefaultAsync(item => item.Id == userPost.UserId, token);

            var hasUserLike = await _context
                .UserLikes
                .AnyAsync(item => item.PostId == id
                                  && item.UserId == userId, token);

            var hasUserSaved = await _context
                .PostBookmarks
                .AnyAsync(item => item.UserPostId == id && item.UserId == userId, token);

            var likeCount = await _context.UserLikes
                .Where(item => item.PostId == id)
                .CountAsync(token);

            var commentCount = await _context.UserComments
                .Where(item => item.PostId == id)
                .CountAsync(token);

            var model = new PostModel
            {
                Id =  userPost.Id,
                Message = userPost.Message,
                LikeCount = likeCount,
                HasUserLike = hasUserLike,
                MediaUrl = userPost.MediaUrl,
                CommentCount = commentCount,
                UserImageLink = user.LowResImageLink,
                Username = user.Username,
                HasUserSaved = hasUserSaved
            };
            
            return Ok(model);
        }
        
        [HttpPost("bookmark/{postId}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> PostBookmark(Guid postId, CancellationToken token)
        {
            var userId = GetUserId();

            var user = await _context.Users
                .FirstOrDefaultAsync(item => item.Id == userId, token);

            if (user is null)
                return NotFound("User Is Not Found");
            
            var userPost = await _context.UserPosts
                .AsNoTracking()
                .FirstOrDefaultAsync(item => item.Id == postId, token);

            if (userPost is null) return NotFound();

            var bookmark = new PostBookmark
            {
                UserPostId = postId,
                UserId = userId
            };

            if (user.PostBookmarks is null)
            {
                user.PostBookmarks = new List<PostBookmark> { bookmark };
            }
            
            else user.PostBookmarks.Add(bookmark);
            
            await _context.PostBookmarks.AddAsync(bookmark, token);

            await _context.SaveChangesAsync(token);
            
            return Ok();
        }
        
        [HttpDelete("bookmark/{postId:guid}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> DeleteBookmark(Guid postId, CancellationToken token)
        {
            var userId = GetUserId();
            
            var userPost = await _context.UserPosts
                .AsNoTracking()
                .FirstOrDefaultAsync(item => item.Id == postId, token);

            if (userPost is null) return NotFound();

            var bookmark = await _context.PostBookmarks
                .FirstOrDefaultAsync(item => item.UserPostId == postId && userId == item.UserId, token);

            if (bookmark is null)
                return NotFound("Book Mark Is Not Found");

            _context.PostBookmarks.Remove(bookmark);

            await _context.SaveChangesAsync(token);
            
            return Ok();
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
}
