
namespace Domain.Entities
{
    public class Comment 
    {
        public int Id { get; set; }
        public string Message { get; set; }
        public int UserId { get; set; }
        public int PostId { get; set; } 
        public DateTime DateCreated { get; set; }
        public DateTime? DateUpdated { get; set; }
    }
}
