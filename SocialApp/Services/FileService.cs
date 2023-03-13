﻿using MongoDB.Bson;
using MongoDB.Driver;
using SocialApp.Models;

namespace SocialApp.Services
{
    public class FileService
    {
         private readonly IMongoCollection<FileDocument> _collection;

        public FileService(IMongoClient client)
        {
            var database = client.GetDatabase("mydatabase");
            _collection = database.GetCollection<FileDocument>("files");
        }

        public async Task<FileDocument> GetByIdAsync(string id)
        {
            var objectId = new ObjectId(id);
            var filter = Builders<FileDocument>.Filter.Eq(d => d.Id, objectId);
            return await _collection.Find(filter).FirstOrDefaultAsync();
        }

        public async Task InsertAsync(FileDocument document)
        {
            await _collection.InsertOneAsync(document);
        }

        public async Task ReplaceAsync(FileDocument document)
        {
            var filter = Builders<FileDocument>.Filter.Eq(d => d.Id, document.Id);
            await _collection.ReplaceOneAsync(filter, document);
        }

        public async Task DeleteAsync(string id)
        {
            var objectId = new ObjectId(id);
            var filter = Builders<FileDocument>.Filter.Eq(d => d.Id, objectId);
            await _collection.DeleteOneAsync(filter);
        }
    }
}
