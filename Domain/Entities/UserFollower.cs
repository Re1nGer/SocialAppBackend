
namespace Domain.Entities
{
    public class UserFollower
    {
        public int Id { get; set; } 
        public int SourceId { get; set; }   
        public User SourceUser { get; set; }    
        public int TargetId { get; set; }   
        public User TargetUser { get; set; }    
        public int Type { get; set; }   
        public DateTime CreatedAt { get; set; } 
        public DateTime? UpdatedAt { get; set; } 
    }
}
