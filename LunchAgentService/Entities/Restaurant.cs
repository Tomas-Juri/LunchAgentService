using Microsoft.WindowsAzure.Storage.Table;

namespace LunchAgentService.Entities
{
    public class Restaurant : TableEntity
    {
        public Restaurant()
        {
            
        }

        public Restaurant(string name)
        {
            PartitionKey = nameof(Restaurant);
            RowKey = name;
        }

        public string Name => RowKey;

        public string Url { get; set; }

        public string Emoji { get; set; }
    }
}