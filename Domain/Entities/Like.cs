namespace Domain.Entities
{
    public class Like
    {
        public int Id { get; set; } 
        public int UserId { get;set; }
        public int? PostId { get; set; }
        public UserPost? UserPost { get; set; } 
        public DateTime CreatedAt { get; set; } 
    }
}
