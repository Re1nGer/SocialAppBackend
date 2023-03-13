
namespace Domain.Entities
{
    public class GroupMember
    {
        public int Id { get; set; } 
        public int GroupId { get; set; }
        public Group Group { get; set; }    
        public int UserId { get; set; }
        public User User { get; set; }
        public int RoleId { get; set; }
        public int Type { get; set; } = 0;
        public int Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? Notes { get; set; }  
    }
}
