using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using log4net;
using LunchAgentService.Helpers.Entities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace LunchAgentService.Helpers
{
    public class SlackHelper
    {
        private static readonly string PostMessageUri = "https://slack.com/api/chat.postMessage";
        private static readonly string UpdateMessageUri = "https://slack.com/api/chat.update";
        private static readonly string ChatHistoryUri = "https://slack.com/api/channels.history";

        private SlackSetting _slackConfiguration;
        public SlackSetting SlackConfiguration
        {
            get { return _slackConfiguration; }
            set
            {
                lock (_slackConfiguration)
                {
                    _slackConfiguration = value;
                }
            }
        }

        private ILog Log { get; set; }
        
        public SlackHelper(SlackSetting slackConfiguration, ILog log)
        {
            _slackConfiguration = slackConfiguration;
            Log = log;
        }

        public void PostMenu(List<Tuple<RestaurantSettings, List<MenuItem>>> menus)
        {
            dynamic postRequestObject = GetRequestObjectFromSlackConfiguration();

            Log.Debug("Adding text to request object");

            postRequestObject.text = FormatMenuForSlack(menus);

            Log.Debug($"Added text to request object {postRequestObject.text}");

            PostToSlack(postRequestObject, PostMessageUri);
        }

        public void UpdateMenu(List<Tuple<RestaurantSettings, List<MenuItem>>> menus)
        {
            var timestamp = GetLastMessageTimestamp();

            if (string.IsNullOrEmpty(timestamp))
                return;

            dynamic postRequestObject = GetRequestObjectFromSlackConfiguration();

            Log.Debug("Adding text and timestamp to request object");

            postRequestObject.text = FormatMenuForSlack(menus);
            postRequestObject.ts = timestamp;

            Log.Debug($"Added text and timestamp to request object {postRequestObject.text}");

            PostToSlack(postRequestObject, UpdateMessageUri);
        }

        private string PostToSlack(dynamic requestObject, string requestUri)
        {
            Log.Debug($"Posting request to slack uri: {requestUri}");

            var data = new StringContent(JObject.FromObject(requestObject).ToString(), Encoding.UTF8, "application/json");

            using (var client = new HttpClient())
            {
                try
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", requestObject.token);
                    var response = client.PostAsync(requestUri, data);

                    response.Wait();
                    var result = response.Result.Content.ReadAsStringAsync().Result;

                    Log.Debug($"Request posted successfuly. Response: {result}");

                    return result;
                }
                catch (Exception e)
                {
                    Log.Error("Error while posting request", e);

                    return null;
                }
            }
        }

        private string GetLastMessageTimestamp()
        {
            Log.Debug("Getting timestamp of last message posted to slack");

            var timeStamp = DateTime.Today;
            var result = "";
            var stringResponse = "";

            var formData = new Dictionary<string, string>();

            using (var client = new HttpClient())
            {
                Log.Debug($"Posting request to slack uri: {ChatHistoryUri}");

                lock (_slackConfiguration)
                {
                    formData["token"] = _slackConfiguration.BotToken;
                    formData["channel"] = _slackConfiguration.ChannelName;
                    formData["bot_id"] = _slackConfiguration.BotId;
                }

                var data = new FormUrlEncodedContent(formData);

                try
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", formData["token"]);
                    var response = client.PostAsync(ChatHistoryUri, data);

                    response.Wait();
                    stringResponse = response.Result.Content.ReadAsStringAsync().Result;

                    Log.Debug($"Request posted successfuly. Response: {result}");

                }
                catch (Exception e)
                {
                    Log.Error("Error while posting request", e);

                }

            }

            if (string.IsNullOrEmpty(stringResponse))
                return "";

            dynamic jsonJObject = JObject.Parse(stringResponse);

            var ar = ((JArray)jsonJObject.messages).ToList();

            foreach (dynamic arElement in ar)
            {
                if (arElement.bot_id != formData["bot_id"])
                    continue;

                string rawTs = arElement.ts;

                var tsInt = (long)Convert.ToDouble(rawTs.Replace(".", ",")) / 1000000;

                var tsDate = (new DateTime(1970, 1, 1)).AddSeconds(tsInt);

                if (tsDate > timeStamp)
                {
                    timeStamp = tsDate;
                    result = rawTs;
                }
            }

            Log.Debug($"Result of last timestamp is: '{result}'");

            return result;
        }

        private string FormatMenuForSlack(List<Tuple<RestaurantSettings, List<MenuItem>>> parsedMenus)
        {
            Log.Debug("Formating menu for slack");

            var result = new List<string>();

            foreach (var parsedMenu in parsedMenus)
            {
                parsedMenu.Item2.FindAll(x => x.FoodType == FoodType.Soup).ForEach(x => x.Description = "_" + x.Description + "_");

                var formatedFood = string.Join(Environment.NewLine, parsedMenu.Item2);

                result.Add($"{parsedMenu.Item1.Emoji}*     {parsedMenu.Item1.Name}*{Environment.NewLine}{formatedFood}");
            }

            return string.Join(Environment.NewLine + Environment.NewLine, result);
        }

        private ExpandoObject GetRequestObjectFromSlackConfiguration()
        {
            Log.Debug("Creating request object from slack configuration");

            dynamic result = new ExpandoObject();

            lock (_slackConfiguration)
            {
                result.token = _slackConfiguration.BotToken;
                result.channel = _slackConfiguration.ChannelName;
                result.bot_id = _slackConfiguration.BotId;
            }

            Log.Debug($"Created request object: {JObject.FromObject(result).ToString()}");

            return result;
        }
    }
}
