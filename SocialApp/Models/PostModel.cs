namespace SocialApp.Models;

public class PostModel
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Username { get; set;  }
    public string UserImageLink { get; set; }
    public string Message { get; set; }
    public string MediaUrl { get; set; }
    public bool HasUserLike { get; set; }
    public bool HasUserSaved { get; set; }
    public int LikeCount { get; set; }
    public int CommentCount { get; set; }
    public DateTime DateCreated { get; set; }
    public string Location { get; set; }
}