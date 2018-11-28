using System;

namespace LunchAgentService.Services.SlackService
{
    public class SlackServiceSetting 
    {
        public string BotToken { get; set; }
        public string ChannelName { get; set; }
        public string BotId { get; set; }

        public SlackServiceSetting Clone()
        {
            return new SlackServiceSetting()
            {
                BotId = BotId,
                BotToken = BotToken,
                ChannelName = ChannelName
            };
        }
    }
}
