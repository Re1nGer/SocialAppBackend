namespace Domain.Entities
{
    public class UserReceivingRequest
    {
        public int Id { get; set; }
        public Guid TargetUserId { get; set; }   
    }
}
