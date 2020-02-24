using Microsoft.WindowsAzure.Storage.Table;

namespace LunchAgentService.Entities
{
    public class Slack : TableEntity
    {
        public Slack()
        {
            
        }

        public Slack(string name)
        {
            PartitionKey = nameof(Slack);
            RowKey = name;
        }

        public string Name => RowKey;

        public string BotToken { get; set; }

        public string ChannelName { get; set; }

        public string BotId { get; set; }
    }
}