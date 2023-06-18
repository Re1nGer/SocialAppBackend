namespace SocialApp.Models;
public class PutCommentRequest: PostCommentRequest
{
    public Guid CommentId { get; set; }
}