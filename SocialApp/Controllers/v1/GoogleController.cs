using System.Security.Claims;
using FirebaseAdmin;
using FirebaseAdmin.Auth;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Persistance;
using SocialApp.Services;
using StreamChat.Clients;
using StreamChat.Models;
using User = Domain.Entities.User;

namespace SocialApp.Controllers.v1;

[ApiController]
[Route("api/v1/[controller]")]
public class GoogleController : BaseController
{
    private readonly ApplicationDbContext _context;
    private readonly IConfiguration _configuration;
    private readonly FirebaseApp _defaultInstace;

    public GoogleController(ApplicationDbContext context, IConfiguration configuration, FirebaseApp defaultInstace)
    {
        _context = context;
        _configuration = configuration;
        _defaultInstace = defaultInstace;
    }

    // GET
    [HttpGet("signin/{token}")]
    public async Task<IActionResult> SignInWithGoogle(string token)
    {
        
        var defaultAuth = FirebaseAuth.GetAuth(_defaultInstace);

        var test = await defaultAuth.VerifyIdTokenAsync(token);

        if (test is null)
        {
            return NotFound();
        }

        var userEmail = test.Claims.FirstOrDefault(item => item.Key == "email").Value;

        var user = await _context.Users.FirstOrDefaultAsync(item => item.Email == userEmail);

        if (user is null)
        {
            return NotFound();
        }
        
        StreamClientFactory factory = new (_configuration.GetSection("StreamPubKey").Value, _configuration.GetSection("StreamPrivKey").Value);
        
        var userClient = factory.GetUserClient();
        
        var claims = new List<Claim>() {  new Claim("UserId", user.Id.ToString()) }.ToArray();

        var accessToken = JwtService.GenerateJwtToken(30, claims);

        var refreshToken = JwtService.GenerateJwtToken(60, claims);

        var streamToken = userClient.CreateToken(user.Id.ToString(), DateTimeOffset.UtcNow.AddHours(1));
        
        SetTokenCookie(refreshToken);

        return Ok(new { token = accessToken, streamToken });
    }
    
    [HttpGet("signup/{token}")]
    public async Task<IActionResult> SignUpWithGoogle(string token)
    {
        
        var defaultAuth = FirebaseAuth.GetAuth(_defaultInstace);

        var test = await defaultAuth.VerifyIdTokenAsync(token);

        if (test is null)
        {
            return NotFound();
        }
        
        var userEmail = test.Claims.FirstOrDefault(item => item.Key == "email").Value as string;

        var displayName = test.Claims.FirstOrDefault(item => item.Key == "name").Value as string;

        var pictureLink = test.Claims.FirstOrDefault(item => item.Key == "picture").Value as string;
        
        StreamClientFactory factory = new (_configuration.GetSection("StreamPubKey").Value, _configuration.GetSection("StreamPrivKey").Value);
        
        var userClient = factory.GetUserClient();
        
        var userExists = await _context.Users.AnyAsync(item => item.Email == userEmail);

        if (userExists)
        {
            return BadRequest("User Already Exists");
        }
        
        var user = new User
        {
            Email = userEmail,
            Username = displayName,
            RegisteredAt = DateTime.UtcNow,
            LowResImageLink = pictureLink,
            HighResImageLink = pictureLink,
        };
        
        await _context.Users.AddAsync(user);

        await _context.SaveChangesAsync();
        
        var streamUser = new UserRequest
        {
            Id = user.Id.ToString(),
            Role = Role.User,
            Name = user.Email
        };
        
        await userClient.UpsertAsync(streamUser);
        
        var claims = new List<Claim>() {  new Claim("UserId", user.Id.ToString()) }.ToArray();

        var accessToken = JwtService.GenerateJwtToken(30, claims);

        var refreshToken = JwtService.GenerateJwtToken(60, claims);

        var streamToken = userClient.CreateToken(user.Id.ToString(), DateTimeOffset.UtcNow.AddHours(1));
        
        SetTokenCookie(refreshToken);

        return Ok(new { token = accessToken, streamToken });
    }
}