using System;
using System.Collections.Generic;
using LunchAgentService.Services;
using MongoDB.Bson.Serialization.Attributes;

namespace LunchAgentService.Entities
{
    public class ReactionMongo : MongoEntity
    {
        [BsonElement("user")]
        public string User { get; set; }
        [BsonElement("date")]
        public DateTime Date { get; set; }
        [BsonElement("url")]
        public string Url { get; set; }
        [BsonElement("name")]
        public string Name { get; set; }
        [BsonElement("emoji")]
        public string Emoji { get; set; }
        [BsonElement("items")]
        public IList<RestaurantMenuItem> Items { get; set; }
    }
}