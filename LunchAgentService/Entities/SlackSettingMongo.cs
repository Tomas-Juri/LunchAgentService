using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace LunchAgentService.Entities
{
    public class SlackSettingMongo : MongoEntity
    {
        [BsonElement("botToken")]
        public string BotToken { get; set; }

        [BsonElement("channelName")]
        public string ChannelName { get; set; }

        [BsonElement("botId")]
        public string BotId { get; set; }

        public SlackSettingApi ToApi()
        {
            return new SlackSettingApi()
            {
                Id = Id.ToString(),
                BotId = BotId,
                BotToken = BotToken,
                ChannelName = ChannelName
            };
        }
    }

    public class SlackSettingApi : ApiEntity
    {
        public string BotToken { get; set; }

        public string ChannelName { get; set; }

        public string BotId { get; set; }

        public SlackSettingMongo ToMongo()
        {
            return new SlackSettingMongo()
            {
                Id = ObjectId.Parse(Id),
                BotId = BotId,
                BotToken = BotToken,
                ChannelName = ChannelName
            };
        }
    }
}