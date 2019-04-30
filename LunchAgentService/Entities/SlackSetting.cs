using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace LunchAgentService.Entities
{
    public class SlackSetting : MongoEntity
    {
        [BsonElement("botToken")]
        public string BotToken { get; set; }

        [BsonElement("channelName")]
        public string ChannelName { get; set; }

        [BsonElement("botId")]
        public string BotId { get; set; }
    }
}