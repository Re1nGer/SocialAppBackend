
namespace Domain.Entities
{
    public class GroupMeta
    {
        public int Id { get; set; } 
        public int GroupId { get; set; }
        public Group Group { get; set; }    
        public string Key { get; set; }
        public string? Content { get; set; }
    }
}
