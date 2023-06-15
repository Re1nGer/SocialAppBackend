using Domain.Entities;

namespace SocialApp.Models;

public class UserResponse
{
    public Guid Id { get; set; }
    public string Username { get; set; }
    public string? LowResImageLink { get; set; }
    public string? HighResImageLink { get; set; }
    public string? ProfileBackgroundImagelink { get; set; }
    public ICollection<UserRequest>? UserRequests { get; set; }
    public ICollection<UserPost>? UserPosts { get; set; }
}