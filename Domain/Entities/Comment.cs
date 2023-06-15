
namespace Domain.Entities
{
    public class Comment 
    {
        public Guid Id { get; set; }
        public string Message { get; set; }
        public Guid UserId { get; set; }
        public string Username { get; set; }
        public Guid? PostId { get; set; } 
        public UserPost? UserPost { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime? DateUpdated { get; set; }
    }
}
