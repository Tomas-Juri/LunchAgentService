using Newtonsoft.Json;

namespace LunchAgentService.Services.TeamsService
{

    public class TeamsMessage
    {
        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("summary")]
        public string Summary { get; set; }

        [JsonProperty("text")]
        public string Text { get; set; }

    }
}
