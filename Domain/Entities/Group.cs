
namespace Domain.Entities
{
    public class Group
    {
        public int Id { get; set; } 
        public int CreatedBy { get; set; }
        public User UserCreatedBy { get; set; }
        public int UpdatedBy { get; set; }     
        public User UserUpdatedBy { get; set; }
        public string Title { get; set; }   
        public string? MetaTitle { get; set; }   
        public string Slug { get; set; }    
        public string? Summary { get; set; }
        public int Status { get; set; }
        public DateTime CreatedAt { get; set; } 
        public DateTime? UpdatedAt { get; set; } 
        public string? Profile { get; set; } 
        public string? Content { get; set; }
        public ICollection<GroupFollower>? GroupFollowers { get; set; }
        public ICollection<GroupPost>? GroupPosts { get; set; }
        public ICollection<GroupMessage>? GroupMessages { get; set; }
        public ICollection<GroupMember>? GroupMembers { get; set; }
        public ICollection<GroupMeta>? GroupMetas { get; set; }
    }
}
