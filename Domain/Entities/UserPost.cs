
namespace Domain.Entities
{
    public class UserPost
    {
        public Guid Id { get; set; } 
        public Guid UserId { get; set; } 
        public User User { get; set; }
        public string Message { get; set; } 
        public string? MediaUrl { get; set; }
        public string? LowResMediaUrl { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }    
        public virtual List<PostBookmark>? PostBookmarks { get; set; }
        public virtual List<Like>? Likes { get; set; }
        public virtual List<Comment>? Comments { get; set; }
    }
}
