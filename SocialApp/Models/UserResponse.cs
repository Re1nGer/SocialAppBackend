using Domain.Entities;

namespace SocialApp.Models;

public class UserResponse
{
    public Guid Id { get; set; }
    public string Username { get; set; }
    public string? LowResImageLink { get; set; }
    public string? HighResImageLink { get; set; }
    public string? ProfileBackgroundImagelink { get; set; }
    public List<string>? PostBookmarks { get; set; }
    public List<UserRequest>? UserRequests { get; set; }
    public List<UserPostResponse>? UserPosts { get; set; }
}

public class UserPostResponse
{
    public Guid Id { get; set; }
    public string Message { get; set; }
    public string MediaUrl { get; set; }
    public string LowResMediaUrl { get; set; }
    public int LikeCount { get; set; }
    public int CommentCount { get; set; }
}