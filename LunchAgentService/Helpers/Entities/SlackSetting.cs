namespace WebApplication1.Helpers.Entities
{
    public struct SlackSetting
    {
        public string BotToken { get; set; }
        public string ChannelName { get; set; }
        public string BotId { get; set; }

        public SlackSetting(string botToken, string channelName, string botId)
        {
            BotToken = botToken;
            ChannelName = channelName;
            BotId = botId;
        }
    }
}
