namespace Domain.Entities
{
    public class UserChat
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; } 
        public Guid UserWithChatId { get; set; }
        public string ChannelId { get; set; }
        public virtual User User { get; set; }
    }
}
