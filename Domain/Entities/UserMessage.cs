namespace Domain.Entities
{
    public class UserMessage
    {
        public int Id { get; set; }
        public int SourceId { get; set; }   
        public User SourceUser { get; set; }    
        public int TargetId { get; set; }   
        public User TargetUser { get; set; }    
        public string Message { get; set; }
        public DateTime CreatedAt { get; set; } 
        public DateTime? UpdatedAt { get; set; }    
    }
}
