namespace Domain.Entities
{
    public class UserRequest
    {
        public Guid Id { get; set; }
        public Guid SenderUserId { get; set; }
        public virtual User SendUser { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime? DateUpdated { get; set; } 
        public Guid UserReceivingRequestId { get; set; }
        //Could be pending, accepted
        public string Status { get; set; }
    }
}
