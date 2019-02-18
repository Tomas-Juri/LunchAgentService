using System;
using System.Globalization;
using Newtonsoft.Json;

namespace LunchAgentService.Services
{
    public class SlackChannelMessage
    {
        [JsonProperty("type")]
        public string Type { get; set; }
        [JsonProperty("user")]
        public string User { get; set; }
        [JsonProperty("text")]
        public string Text { get; set; }
        [JsonProperty("client_msg_id")]
        public string ClientMessageId { get; set; }
        [JsonProperty("thread_ts")]
        public string ThreadTimestamp { get; set; }
        [JsonProperty("parent_user_id")]
        public string ParrentUserId { get; set; }
        [JsonProperty("ts")]
        public string Timestamp { get; set; }
        [JsonProperty("reply_count")]
        public int ReplyCount { get; set; }
        [JsonProperty("replies")]
        public object Replies { get; set; }
        [JsonProperty("bot_id")]
        public string BotId { get; set; }
        [JsonIgnore]
        public DateTime Date
        {
            get
            {
                return new DateTime(1970, 1, 1, 1, 0, 0).AddSeconds((long)Convert.ToDouble(Timestamp, CultureInfo.InvariantCulture));
            }
        }
    }
}