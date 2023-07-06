namespace Domain.Entities;

public class PostBookmark
{
    public int Id { get; set; }
    public Guid UserPostId { get; set; }
    public Guid UserId { get; set; }
    public virtual User User { get; set; }
    public virtual UserPost UserPost { get; set; }
}