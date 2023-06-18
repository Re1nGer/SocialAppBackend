namespace SocialApp.Models;

public class PostCommentRequest
{
    public Guid PostId { get; set; }
    public string Message { get; set; }
}