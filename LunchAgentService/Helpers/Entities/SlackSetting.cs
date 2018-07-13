using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LunchAgentService.Helpers.Entities
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
