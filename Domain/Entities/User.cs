namespace Domain.Entities
{
    public class User
    {
        public Guid Id { get; set; } 
        public string Username { get; set; }
        public string Email { get; set; }   
        public string? LowResImageLink { get; set; }
        public string? HighResImageLink { get; set; }
        public string? ProfileBackgroundImagelink { get; set; }
        public DateTime RegisteredAt { get; set; }
        public DateTime? LastLogin { get; set; } 
        public string? Intro { get; set; }   
        public ICollection<PostBookmark>? PostBookmarks { get; set; }
        public ICollection<UserMessage>? UserMessageSources { get; set; }
        public ICollection<UserMessage>? UserMessageTargets { get; set; }
        public ICollection<UserPost>? UserPosts { get; set; }
        public ICollection<UserChat>? UserChats { get; set; }
        public ICollection<UserFollower>? Followers { get; set; }
        public ICollection<UserFollower>? Following { get; set; }
        public ICollection<UserRequest>? UserRequests { get; set; }
        public ICollection<UserReceivingRequest>? UserReceivingRequests { get; set; }
        public ICollection<UserBlocked>? UsersBlocked { get; set; }
    }
}
