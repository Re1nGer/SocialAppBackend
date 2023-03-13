
namespace Domain.Entities
{
    public class GroupFollower
    {
        public int Id { get; set; } 
        public int GroupId { get; set; }    
        public Group Group { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public int Type { get; set; } = 0;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
