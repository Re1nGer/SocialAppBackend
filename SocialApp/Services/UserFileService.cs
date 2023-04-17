using MongoDB.Bson;
using MongoDB.Driver;
using SocialApp.Models;
using System.Linq.Expressions;

namespace SocialApp.Services
{
    public class UserFileService
    {
         private readonly IMongoCollection<UserDocument> _collection;

        public UserFileService(IMongoClient client)
        {
            var database = client.GetDatabase("mydatabase");
            _collection = database.GetCollection<UserDocument>("user");
        }

        public List<UserDocument> FilterByField(Expression<Func<UserDocument, object>> fieldSelector, int value)
        {
            var filter = Builders<UserDocument>.Filter.Eq(fieldSelector, value);

            return _collection.Find(filter).ToList();
        }

        public IQueryable<UserDocument> Where(Expression<Func<UserDocument, bool>> predicate)
        {
            return _collection.AsQueryable().Where(predicate);
        }

        public List<UserDocument> GetAll()
        {
            return _collection.AsQueryable().ToList();
        }

        public async Task<UserDocument> GetByIdAsync(string id)
        {
            var objectId = new ObjectId(id);
            var filter = Builders<UserDocument>.Filter.Eq(d => d.Id, objectId);
            return await _collection.Find(filter).FirstOrDefaultAsync();
        }

        public async Task InsertAsync(UserDocument document)
        {
            await _collection.InsertOneAsync(document);
        }

        public async Task<UserDocument> GetFileByUserIdAsync(int userId)
        {
            var filter = Builders<UserDocument>.Filter.Eq(d => d.UserId, userId);
            return await _collection.Find(filter).FirstOrDefaultAsync();
        }

        public async Task DeleteAsync(string id)
        {
            var objectId = new ObjectId(id);
            var filter = Builders<UserDocument>.Filter.Eq(d => d.Id, objectId);
            await _collection.DeleteOneAsync(filter);
        }
    }
}
