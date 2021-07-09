using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Globalization;
using Newtonsoft.Json;

namespace LunchAgentService.Services.TeamsService
{
    public class TeamsMessage
    {
        [JsonProperty("@type")]
        public string Type { get; set; }

        [JsonProperty("summary")]
        public string Summary { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("text")]
        public string Text { get; set; }

        [JsonProperty("themeColor")]
        public string ThemeColor { get; set; }

    }
}
