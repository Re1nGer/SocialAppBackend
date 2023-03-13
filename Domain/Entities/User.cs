
namespace Domain.Entities
{
    public class User
    {
        public int Id { get; set; } 
        public string FirstName { get; set; }    
        public string LastName { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }   
        public string Picture { get; set; }
        public DateTime RegisteredAt { get; set; }
        public DateTime? LastLogin { get; set; } 
        public string? Profile { get; set; }
        public string? Intro { get; set; }   
        public ICollection<Group>? GroupsCreatedBy { get; set; }
        public ICollection<Group>? GroupsUpdatedBy { get; set; }
        public ICollection<UserFriend>? UserFriendSources { get; set; }
        public ICollection<UserFriend>? UserFriendTargets { get; set; }
        public ICollection<UserFollower>? UserFollowerSources { get; set; }
        public ICollection<UserFollower>? UserFollowerTargets { get; set; }
        public ICollection<UserMessage>? UserMessageSources { get; set; }
        public ICollection<UserMessage>? UserMessageTargets { get; set; }
        public ICollection<UserPost>? UserPosts { get; set; }
        public ICollection<UserPost>? UserPostSenders { get; set; }
        public ICollection<GroupFollower>? GroupFollowers { get; set; }
        public ICollection<GroupPost>? GroupPosts { get; set; }  
        public ICollection<GroupMessage>? GroupMessages { get; set; }
        public ICollection<GroupMember>? GroupMembers { get; set; }

    }
}
