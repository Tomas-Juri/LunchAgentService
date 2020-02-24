using System.Collections.Generic;
using System.Security.Authentication;
using LunchAgentService.Entities;
using LunchAgentService.Helpers;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;

namespace LunchAgentService.Services.DatabaseService
{
    public class DatabaseService : IDatabaseService
    {
        private ILogger Log { get; }
        private IMongoClient Client { get; }
        private IMongoDatabase Database { get; }

        public DatabaseService(IOptions<AppSettings> options, ILogger log)
        {
            Log = log;
            var settings = MongoClientSettings.FromUrl(new MongoUrl(options.Value.ConnectionString));
            settings.SslSettings = new SslSettings { EnabledSslProtocols = SslProtocols.Tls12 };
            Client = new MongoClient(settings);

            Database = Client.GetDatabase(options.Value.DatabaseName);
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
                return null;

            var dbEntity = Get<T>(entity.Id);
            if (dbEntity == null)
            {
                Database
                    .GetCollection<T>(typeof(T).Name)
                    .InsertOne(entity);
            }
            else
            {
                Database.GetCollection<T>(typeof(T).Name)
                    .ReplaceOne(new BsonDocument("_id", entity.Id), entity);
            }


            return entity;
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