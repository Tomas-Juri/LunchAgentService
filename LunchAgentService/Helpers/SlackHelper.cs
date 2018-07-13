using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using LunchAgentService.Helpers.Entities;
using Newtonsoft.Json.Linq;

namespace LunchAgentService.Helpers
{
    public class SlackHelper
    {
        private static readonly string PostMessageUri = "https://slack.com/api/chat.postMessage";
        private static readonly string UpdateMessageUri = "https://slack.com/api/chat.update";
        private static readonly string ChatHistoryUri = "https://slack.com/api/channels.history";

        private SlackSetting _slackConfiguration;

        public SlackHelper(string configurationJson)
        {
            //_slackConfiguration= JsonConvert.DeserializeObject<SlackSetting>(configurationJson);
        }

        public void PostMenu(List<Tuple<RestaurantSettings, List<MenuItem>>> menus)
        {
            dynamic postRequestObject = new JObject();

            postRequestObject.token = _slackConfiguration.BotToken;
            postRequestObject.channel = _slackConfiguration.ChannelName;
            postRequestObject.bot_id = _slackConfiguration.BotId;
            postRequestObject.text = FormatMenuForSlack(menus);

            var data = new StringContent(postRequestObject.ToString(), Encoding.UTF8, "application/json");

            using (var client = new HttpClient())
            {
                client.PostAsync(PostMessageUri, data);
            }
        }

        public void UpdateMenu(List<Tuple<RestaurantSettings, List<MenuItem>>> menus)
        {
            var timestamp = GetLastMessageTimestamp();

            if (string.IsNullOrEmpty(timestamp))
                return;

            dynamic postRequestObject = new JObject();

            postRequestObject.token = _slackConfiguration.BotToken;
            postRequestObject.channel = _slackConfiguration.ChannelName;
            postRequestObject.text = FormatMenuForSlack(menus);
            postRequestObject.ts = timestamp;

            var data = new StringContent(postRequestObject.ToString(), Encoding.UTF8, "application/json");

            using (var client = new HttpClient())
            {
                var response = client.PostAsync(UpdateMessageUri, data);
            }
        }

        private string GetLastMessageTimestamp()
        {
            dynamic postRequestObject = new JObject();

            postRequestObject.token = _slackConfiguration.BotToken;
            postRequestObject.channel = _slackConfiguration.ChannelName;
            postRequestObject.bot_id = _slackConfiguration.BotId;

            var data = new StringContent(postRequestObject.ToString(), Encoding.UTF8, "application/json");

            var timeStamp = DateTime.Today;
            var result = "";

            using (var client = new HttpClient())
            {
                var response = client.PostAsync(ChatHistoryUri, data);

                var stringResponse = response.Result.Content.ReadAsStringAsync().Result;

                dynamic jsonJObject = JObject.Parse(stringResponse);

                var ar = ((JArray)jsonJObject.messages).ToList();

                foreach (dynamic arElement in ar)
                {
                    if (arElement.bot_id != postRequestObject.bot_id)
                        continue;

                    string rawTs = arElement.ts;

                    var tsInt = (int)Convert.ToDouble(rawTs.Replace(".", ","));

                    var tsDate = (new DateTime(1970, 1, 1)).AddSeconds(tsInt);

                    if (tsDate > timeStamp)
                    {
                        timeStamp = tsDate;
                        result = rawTs;
                    }
                }
            }

            return result;
        }

        private string FormatMenuForSlack(List<Tuple<RestaurantSettings, List<MenuItem>>> parsedMenus)
        {
            var result = new List<string>();

            foreach (var parsedMenu in parsedMenus)
            {
                parsedMenu.Item2.FindAll(x => x.FoodType == FoodType.Soup).ForEach(x => x.Description = "_" + x.Description + "_");

                var formatedFood = string.Join(Environment.NewLine, parsedMenu.Item2);

                result.Add($"{parsedMenu.Item1.Emoji}*     {parsedMenu.Item1.Name}*{Environment.NewLine}{formatedFood}");
            }

            return string.Join(Environment.NewLine + Environment.NewLine, result);
        }
    }
}
