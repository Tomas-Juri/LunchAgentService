using MongoDB.Bson;

namespace LunchAgentService.Entities
{
    public class UserMongo : MongoEntity
    {
        public string Username { get; set; }
        public string Role { get; set; }
        public string Token { get; set; }
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }

        public UserApi ToApi()
        {
            return new UserApi
            {
                Id = Id.ToString(),
                Username = Username,
            };
        }
    }

    public class UserApi : ApiEntity
    {
        public string Username { get; set; }
        public string Password { get; set; }

        public UserMongo ToMongo()
        {
            return new UserMongo
            {
                Id = Id != null ? ObjectId.Parse(Id) : ObjectId.Empty,
                Username = Username
            };
        }
    }
}