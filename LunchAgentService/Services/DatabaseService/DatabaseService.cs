using System.Collections.Generic;
using System.Security.Authentication;
using LunchAgentService.Entities;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;

namespace LunchAgentService.Services.DatabaseService
{
    public class DatabaseService : IDatabaseService
    {
        private ILogger Log { get; }
        private IMongoClient Client { get; }
        private IMongoDatabase Database { get; }

        public DatabaseService(string connectionString, string databaseName, ILogger log)
        {
            Log = log;
            var settings = MongoClientSettings.FromUrl(new MongoUrl(connectionString));
            settings.SslSettings = new SslSettings { EnabledSslProtocols = SslProtocols.Tls12 };
            Client = new MongoClient(settings);

            Database = Client.GetDatabase(databaseName);
        }

        public T Get<T>(ObjectId id) where T : MongoEntity
        {
            return Database
                .GetCollection<T>(typeof(T).Name)
                .Find(Builders<T>.Filter.Eq("_id", id))
                .FirstOrDefault();
        }

        public IEnumerable<T> Get<T>() where T : MongoEntity
        {
            return Database
                .GetCollection<T>(typeof(T).Name)
                .Find(Builders<T>.Filter.Empty)
                .ToList();
        }

        public T AddOrUpdate<T>(T entity) where T : MongoEntity
        {
            if (entity == null)
                return entity;

            var collection = Database.GetCollection<T>(typeof(T).Name);

            if (entity.Id == null ||  collection.Find(Builders<T>.Filter.Eq("_id", entity.Id)).CountDocuments() == 0)
            {
                collection.InsertOne(entity);

                return entity;
            }

            return collection.FindOneAndUpdate(Builders<T>.Filter.Eq("_id", entity.Id), entity.ToBsonDocument());
        }

        public IEnumerable<T> AddOrUpdate<T>(T[] entities) where T : MongoEntity
        {
            foreach (var entity in entities)
                AddOrUpdate(entity);

            return entities;
        }

        public DeleteResult Delete<T>(ObjectId id) where T : MongoEntity
        {
            return Database
                .GetCollection<T>(typeof(T).Name)
                .DeleteOne(Builders<T>.Filter.Eq("_id", id));
        }
    }
}