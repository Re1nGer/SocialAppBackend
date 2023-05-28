namespace Domain.Entities
{
    public class UserChat
    {
        public Guid Id { get; set; }
        public virtual List<UserMessage> Messages { get; set; }
        public int UserId { get; set; } 
        public virtual User User { get; set; }
    }
}
