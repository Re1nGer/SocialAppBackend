
namespace Domain.Entities
{
    public class UserBlocked
    {
        public int Id { get; set; }
        public int UserBlockedId { get; set; }
        public int UserId { get; set; }
        public virtual User User { get; set; }
    }
}
