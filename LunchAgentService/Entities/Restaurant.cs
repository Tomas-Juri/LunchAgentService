using MongoDB.Bson.Serialization.Attributes;

namespace LunchAgentService.Entities
{
    public class Restaurant : MongoEntity
    {
        [BsonElement("name")]
        public string Name { get; set; }

        [BsonElement("url")]
        public string Url { get; set; }

        [BsonElement("emoji")]
        public string Emoji { get; set; }
    }
}