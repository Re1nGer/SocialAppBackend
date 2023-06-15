
namespace Domain.Entities
{
    public class UserFollower
    {
        public int Id { get; set; } 
        public Guid FollowerId { get; set; }   
        public User? FollowerUser { get; set; }    
        public Guid FollowingId { get; set; }   
        public User? FollowingUser { get; set; }    
        public int Type { get; set; }   
        public DateTime CreatedAt { get; set; } 
        public DateTime? UpdatedAt { get; set; } 
    }
}
