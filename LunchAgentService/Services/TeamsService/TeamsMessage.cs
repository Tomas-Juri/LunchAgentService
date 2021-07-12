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
        /*
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("summary")]
        public string Summary { get; set; }

        [JsonProperty("text")]
        public string Text { get; set; }

        [JsonProperty("themeColor")]
        public string ThemeColor { get; set; }

        [JsonProperty("$schema")]
        public string Schema { get; set; }

        [JsonProperty("version")]
        public string Version { get; set; }

        [JsonProperty("body")]
        public List<object> Body { get; set; } = new List<object>();
        */

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("summary")]
        public string Summary { get; set; }

        [JsonProperty("text")]
        public string Text { get; set; }


    }


    public class RestaurantContainer
    {
        [JsonProperty("type")]
        public string Type { get; set; }
        [JsonProperty("items")]
        public List<MessageItem> Dishes { get; set; } = new List<MessageItem>();

    }

    public class MessageItem
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("text")]
        public string Text { get; set; }

        [JsonProperty("wrap")]
        public bool Wrap { get; set; }

        [JsonProperty("spacing")]
        public string Spacing { get; set; }

        [JsonProperty("size")]
        public string Size { get; set; }

        [JsonProperty("weight")]
        public string Weight { get; set; }
        [JsonProperty("color")]
        public string Color { get; set; }

    }



}
