
namespace Domain.Entities
{
    public class UserBlocked
    {
        public int Id { get; set; }
        public Guid UserBlockedId { get; set; }
        public Guid UserId { get; set; }
        public virtual User User { get; set; }
    }
}
