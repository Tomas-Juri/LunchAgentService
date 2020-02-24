using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using LunchAgentService.Entities;
using LunchAgentService.Services.DatabaseService;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace LunchAgentService.Services
{
    public class SlackService : ISlackService
    {
        private static readonly string PostMessageUri = "https://slack.com/api/chat.postMessage";
        private static readonly string UpdateMessageUri = "https://slack.com/api/chat.update";
        private static readonly string ChatHistoryUri = "https://slack.com/api/channels.history";

        private ILogger Log { get; }
        private IStorageService StorageService { get; }

        public SlackService(IStorageService storageService, ILogger<SlackService> log)
        {
            StorageService = storageService;
            Log = log;
        }

        public void ProcessMenus(List<RestaurantMenu> menus)
        {
            var settings = StorageService.Get<Slack>().First();

            Log.LogDebug("Getting slack history");

            var history = GetSlackChannelHistory(settings);

            Log.LogDebug("Filtering messages for messages from today");

            var todayMessages = history.Messages.FindAll(message =>
                message.Date.Date == DateTime.Today && message.BotId == settings.BotId);

            Log.LogDebug($"I have {todayMessages.Count} messages from myself today");

            var todayMessage = todayMessages
                .OrderByDescending(message => message.Date)
                .FirstOrDefault();

            if (todayMessage == null)
            {
                Log.LogDebug("Posting new menus to slack");

                PostToSlack(menus, settings);
            }
            else
            {
                Log.LogDebug("Updating already existing menu on slack");

                UpdateToSlack(menus, todayMessage.Timestamp, settings);
            }
        }

        public void PostToSlack(List<RestaurantMenu> menus, Slack settings)
        {
            dynamic postRequestObject = GetRequestObjectFromSlackConfiguration(settings);

            postRequestObject.text = FormatMenuForSlack(menus);

            PostToSlack(postRequestObject, PostMessageUri);
        }

        public void UpdateToSlack(List<RestaurantMenu> menus, string timestamp, Slack settings)
        {
            dynamic postRequestObject = GetRequestObjectFromSlackConfiguration(settings);

            postRequestObject.text = FormatMenuForSlack(menus);
            postRequestObject.ts = timestamp;

            PostToSlack(postRequestObject, UpdateMessageUri);
        }

        public IList<Reaction> GetReactionsToLunch()
        {
            var settings = StorageService.Get<Slack>().First();

            var channelHistory = GetSlackChannelHistory(settings);

            var promptingMessage = channelHistory.Messages
                .FirstOrDefault(x => x.Date.Date == DateTime.Today &&
                                     x.Text.Contains("Kam na oběd?"));

            if (promptingMessage == null)
            {
                return new List<Reaction>();
            }

            return promptingMessage.Reactions.ToList();
        }

        private string PostToSlack(dynamic requestObject, string requestUri)
        {
            Log.LogDebug($"Posting request to slack uri: {requestUri}");

            var data = new StringContent(JObject.FromObject(requestObject).ToString(), Encoding.UTF8, "application/json");

            using (var client = new HttpClient())
            {
                try
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", requestObject.token);
                    var response = client.PostAsync(requestUri, data);

                    response.Wait();
                    var result = response.Result.Content.ReadAsStringAsync().Result;

                    Log.LogDebug($"Request posted successfuly. Response: {response.Result.StatusCode}");

                    return result;
                }
                catch (Exception e)
                {
                    Log.LogError("Failed posting request", e);

                    return null;
                }
            }
        }

        private SlackChannelHistory GetSlackChannelHistory(Slack slackSetting)
        {
            var stringResponse = "";

            var formData = new Dictionary<string, string>();

            using (var client = new HttpClient())
            {
                Log.LogDebug($"Posting request to slack uri: {ChatHistoryUri}");

                formData["token"] = slackSetting.BotToken;
                formData["channel"] = slackSetting.ChannelName;
                formData["bot_id"] = slackSetting.BotId;

                var data = new FormUrlEncodedContent(formData);

                try
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", formData["token"]);
                    var response = client.PostAsync(ChatHistoryUri, data);

                    response.Wait();
                    stringResponse = response.Result.Content.ReadAsStringAsync().Result;

                    Log.LogDebug($"Request posted successfuly. Response: {response.Result.StatusCode}");

                }
                catch (Exception e)
                {
                    Log.LogError("Failed posting request", e);

                    return new SlackChannelHistory();
                }
            }

            var result = JsonConvert.DeserializeObject<SlackChannelHistory>(stringResponse);

            return result;
        }

        private string FormatMenuForSlack(List<RestaurantMenu> menus)
        {
            Log.LogDebug("Formating menu for slack");

            var result = new List<string>();

            foreach (var parsedMenu in menus)
            {
                parsedMenu.Items.FindAll(x => x.FoodType == FoodType.Soup).ForEach(x => x.Description = "_" + x.Description + "_");

                var formatedFood = string.Join(Environment.NewLine, parsedMenu.Items);

                result.Add($"{parsedMenu.Restaurant.Emoji}*     {parsedMenu.Restaurant.Name}*{Environment.NewLine}{formatedFood}");
            }

            return string.Join(Environment.NewLine + Environment.NewLine, result);
        }

        private ExpandoObject GetRequestObjectFromSlackConfiguration(Slack slackSetting)
        {
            dynamic result = new ExpandoObject();

            result.token = slackSetting.BotToken;
            result.channel = slackSetting.ChannelName;
            result.bot_id = slackSetting.BotId;

            return result;
        }
    }
}
