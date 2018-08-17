using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using MongoDB.Driver;

namespace LunchAgentService.Services
{
    public class MongoDatabaseAcessService
    {
        private IMongoClient MongoClient { get; set; }
        private IMongoDatabase MongoDatabase { get; set; }

        public MongoDatabaseAcessService(string databaseConnectionString)
        {
            MongoClient = new MongoClient(databaseConnectionString);
            MongoDatabase = MongoClient.GetDatabase("LunchAgentService");
        }

        public void InsertUserResponses(Dictionary<string, string[]> userResponses)
        {
            var collection = MongoDatabase.GetCollection<KeyValuePair<string, string[]>>("UserResponses");

            foreach (var userResponse in userResponses)
            {
                collection.InsertOne(userResponse);
            }
        }

    }
}
