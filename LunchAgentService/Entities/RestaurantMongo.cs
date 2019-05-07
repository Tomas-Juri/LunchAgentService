using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace LunchAgentService.Entities
{
    public class RestaurantMongo : MongoEntity
    {
        [BsonElement("name")]
        public string Name { get; set; }

        [BsonElement("url")]
        public string Url { get; set; }

        [BsonElement("emoji")]
        public string Emoji { get; set; }

        public RestaurantApi ToApi()
        {
            return new RestaurantApi()
            {
                Id = Id.ToString(),
                Name = Name,
                Url = Url,
                Emoji = Emoji
            };
        }
    }

    public class RestaurantApi : ApiEntity
    {
        public string Name { get; set; }

        public string Url { get; set; }

        public string Emoji { get; set; }

        public RestaurantMongo ToMongo()
        {
            return new RestaurantMongo()
            {
                Id = ObjectId.Parse(Id),
                Name = Name,
                Url = Url,
                Emoji = Emoji
            };
        }
    }
}