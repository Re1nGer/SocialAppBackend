namespace Domain.Entities
{
    public class UserReceivingRequest
    {
        public int Id { get; set; }
        public int UserRequestId { get; set; }  
        public int TargetUserId { get; set; }   
        public virtual User TargetUser { get; set; }
        public virtual UserRequest UserRequest { get; set; }
    }
}
