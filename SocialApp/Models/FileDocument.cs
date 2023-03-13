using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace SocialApp.Models
{
    public class FileDocument
    {
        [BsonId]
        public ObjectId Id { get; set; }
        public int UserId { get; set; }

        public string Filename { get; set; }

        public byte[] Data { get; set; }
    }
}
