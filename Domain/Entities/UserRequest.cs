namespace Domain.Entities
{
    public class UserRequest
    {
        public int Id { get; set; }
        public int SenderUserId { get; set; }
        public virtual User SendUser { get; set; }
        public int UserReceivingRequestId { get; set; }
        //Could be pending, accepted
        public string Status { get; set; }
    }
}
