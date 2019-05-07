using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace LunchAgentService.Entities
{
    public abstract class MongoEntity
    {
        [BsonId]
        public ObjectId Id { get; set; }
    }
}