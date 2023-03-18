
namespace Domain.Entities
{
    public class UserPost
    {
        public int Id { get; set; } 
        public int UserId { get; set; } 
        public User User { get; set; }
        public string Message { get; set; } 
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }    
        public List<int>? CommentIds { get; set; }
        public List<int>? LikeIds { get; set; }
        public virtual List<Like>? Likes { get; set; }
        public virtual List<Comment>? Comments { get; set; }
    }
}
