using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using LunchAgentService.Helpers;
using LunchAgentService.Services.RestaurantService;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace LunchAgentService.Services.TeamsService
{
    public class TeamsService : ITeamsService
    {
        private ILogger Log { get; }
        private readonly AppSettings _appSettings;

        public TeamsService(ILogger<TeamsService> log, IOptions<AppSettings> appSettings)
        {
            Log = log;
            _appSettings = appSettings.Value;
        }      


        public string Post(List<RestaurantMenu> menus)
        {
            string requestUri = _appSettings.TeamsBotLink;



            Log.LogDebug($"Posting request to teams uri: {requestUri}");

            TeamsMessage requestObject = new TeamsMessage();
            requestObject.Title = "Menicka na den " + DateTime.Now;
            requestObject.Summary = "Menicka na den " + DateTime.Now;
            requestObject.Text = FormatMenuForTeams(menus);




            var data = new StringContent(JObject.FromObject(requestObject).ToString(), Encoding.UTF8, "application/json");
            var test = JObject.FromObject(requestObject);

            using (var client = new HttpClient())
            {
                try
                {                   
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
       
        private string FormatMenuForTeams(List<RestaurantMenu> menus)
        {
            Log.LogDebug("Formating menu for teams");


            //TeamsMessage teamsMessage = new TeamsMessage();
            //teamsMessage.Type = "AdaptiveCard";
            //teamsMessage.Schema = "http://adaptivecards.io/schemas/adaptive-card.json";
            //teamsMessage.Version = "1.3";
            //teamsMessage.Title = "Title";
            //teamsMessage.Summary = "summary";
            //teamsMessage.Text = "text";
            //teamsMessage.ThemeColor = "#00A0B0";

            //foreach (var menu in menus)
            //{
            //    RestaurantContainer restaurantContainer = new RestaurantContainer();
            //    restaurantContainer.Type = "Container";
            //    foreach (var dish in menu.Items)
            //    {
            //        MessageItem messageItem = new MessageItem();
            //        messageItem.Type = "TextBlock";
            //        messageItem.Text = dish.Description;
            //        messageItem.Wrap = true;
            //        messageItem.Spacing = "none";
            //        messageItem.Size = "default";
            //        messageItem.Weight = "default";
            //        messageItem.Color = "default";

            //        restaurantContainer.Dishes.Add(messageItem);
            //    }
            //    teamsMessage.Body.Add(restaurantContainer);

            //}



            Log.LogDebug("Formating menu for teams");

            var result = new List<string>();

            foreach (var parsedMenu in menus)
            {
                parsedMenu.Items.FindAll(x => x.FoodType == FoodType.Soup).ForEach(x => x.Description = "_" + x.Description.Trim() + "_");

                var formatedFood = string.Join(Environment.NewLine, parsedMenu.Items);

                result.Add($"## {parsedMenu.Restaurant.Name} ##{Environment.NewLine} {formatedFood}");
            }

            return string.Join(Environment.NewLine + Environment.NewLine, result);
        }
    }
}
