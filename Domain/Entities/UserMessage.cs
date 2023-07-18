﻿namespace Domain.Entities
{
    public class UserMessage
    {
        public int Id { get; set; }
        public Guid SourceUserId { get; set; }   
        public User SourceUser { get; set; }    
        public Guid TargetUserId { get; set; }   
        public User TargetUser { get; set; }    
        public Guid UserChatId { get; set; }
        public UserChat UserChat { get; set; }
        public string Message { get; set; }
        public DateTime CreatedAt { get; set; } 
        public DateTime? UpdatedAt { get; set; }    
    }
}
