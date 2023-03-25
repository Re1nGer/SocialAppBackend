using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace SocialApp.Models
{
    public class UserDocument
    {
        [BsonId]
        public ObjectId Id { get; set; }
        public int UserId { get; set; }
        public string UserImageName { get; set; }
        public byte[] UserImage { get; set; }
        public byte[] UserLowResolutionImage { get; set; }
    }
}
