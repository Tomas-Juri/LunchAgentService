using System.Collections.Generic;
using Newtonsoft.Json;

namespace LunchAgentService.Services
{
    public class SlackChannelHistory
    {
        [JsonProperty("ok")]
        public bool Ok { get; set; }
        [JsonProperty("messages")]
        public List<SlackChannelMessage> Messages { get; set; }
        [JsonProperty("has_more")]
        public bool HasMore { get; set; }
        [JsonProperty("is_limited")]
        public bool IsLimited { get; set; }
    }
}