
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
    }
}
