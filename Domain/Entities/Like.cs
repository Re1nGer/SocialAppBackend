namespace Domain.Entities
{
    public class Like
    {
        public int Id { get; set; } 
        public Guid UserId { get;set; }
        public Guid? PostId { get; set; }
        public UserPost? UserPost { get; set; } 
        public DateTime CreatedAt { get; set; } 
    }
}
