using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using HtmlAgilityPack;
using log4net;
using LunchAgentService.Entities;
using LunchAgentService.Services.DatabaseService;
using Newtonsoft.Json.Linq;

namespace LunchAgentService.Services
{
    public class RestaurantService : IRestaurantService
    {
        private ILog Log { get; }
        private IDatabaseService DatabaseService { get; }

        public RestaurantService(IDatabaseService databaseService, ILog log)
        {
            Log = log;
            DatabaseService = databaseService;
        }

        public List<RestaurantMenu> GetMenus()
        {
            var result = new List<RestaurantMenu>();

            var document = new HtmlDocument();

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            var restaurants = DatabaseService.Get<Restaurant>();

            foreach (var setting in restaurants)
            {
                if (setting.Name.Contains("Bistrotéka"))
                    using (var client = new HttpClient())
                    {
                        try
                        {
                            var data = client.GetStringAsync(setting.Url).Result;

                            result.Add(new RestaurantMenu()
                            {
                                Items = ParseMenuFromBistroteka(data),
                                Restaurant = setting
                            });
                        }
                        catch (Exception e)
                        {
                            Log.Debug($"Failed to get menu from {setting.Name}", e);
                        }
                    }

                using (var client = new WebClient())
                {
                    try
                    {
                        var data = setting.Url.Contains("makalu")
                            ? Encoding.UTF8.GetString(client.DownloadData(setting.Url))
                            : Encoding.GetEncoding(1250).GetString(client.DownloadData(setting.Url));

                        document.LoadHtml(data);
                    }
                    catch (Exception e)
                    {
                        Log.Debug($"Failed to get menu from {setting.Name}", e);
                    }
                }

                try
                {
                    var parsedMenu = setting.Url.Contains("makalu")
                        ? ParseMenuFromMakalu(document.DocumentNode)
                        : ParseMenuFromMenicka(document.DocumentNode);

                    result.Add(new RestaurantMenu()
                    {
                        Restaurant = setting,
                        Items = parsedMenu
                    });
                }
                catch (Exception e)
                {
                    Log.Debug($"Failed to parse menu from {setting.Name}", e);
                }

                Log.Debug($"Sucessfully got menu from {setting.Name}");
            }


            return result;
        }

        private List<RestaurantMenuItem> ParseMenuFromBistroteka(string data)
        {
            var result = new List<RestaurantMenuItem>();

            var items = JArray.Parse(data);

            var todayItems = items.Where(x => DateTime.TryParse(x["dateFrom"].Value<string>(), out var value) == true && value == DateTime.Today);

            foreach (var todayItem in todayItems)
            {
                var item = new RestaurantMenuItem();

                if (todayItem["type"].Value<string>() == "polevka")
                {
                    item.FoodType = FoodType.Soup;
                    item.Description = todayItem["name"].Value<string>();
                    item.Price = todayItem["price"].Value<string>();
                }
                else
                {
                    item.FoodType = FoodType.Main;
                    item.Description = todayItem["name"].Value<string>();
                    item.Price = todayItem["price"].Value<string>();
                }

                result.Add(item);
            }

            return result;
        }

        private static List<RestaurantMenuItem> ParseMenuFromMenicka(HtmlNode todayMenu)
        {
            var result = new List<RestaurantMenuItem>();

            var foodMenus = todayMenu.SelectNodes(".//tr")
                .Where(node => node.GetClasses().Contains("soup") || node.GetClasses().Contains("main"));

            foreach (var food in foodMenus)
            {
                var item = new RestaurantMenuItem();

                if (food.GetClasses().Contains("soup"))
                {
                    item.FoodType = FoodType.Soup;
                    item.Description = Regex.Replace(food.SelectNodes(".//td").Single(x => x.GetClasses().Contains("food")).InnerText, "\\d+.?", string.Empty);
                    item.Price = food.SelectNodes(".//td").Single(x => x.GetClasses().Contains("prize")).InnerText;
                }
                else
                {
                    item.FoodType = FoodType.Main;
                    item.Description = Regex.Replace(food.SelectNodes(".//td").Single(x => x.GetClasses().Contains("food")).InnerText, "\\d+.?", string.Empty);
                    item.Price = food.SelectNodes(".//td").Single(x => x.GetClasses().Contains("prize")).InnerText;
                    item.Index = food.SelectNodes(".//td").Single(x => x.GetClasses().Contains("no")).InnerText;
                }
                result.Add(item);
            }

            return result;
        }

        private static List<RestaurantMenuItem> ParseMenuFromMakalu(HtmlNode todayMenu)
        {
            var result = new List<RestaurantMenuItem>();

            var todayString = GetTodayInCzech();

            var todayNode = string.Join(" ", todayMenu.SelectNodes(".//div[contains(@class,TJStrana)]").Where(x => x.GetClasses().Contains("TJStrana")).Select(x => x.InnerHtml));

            var start = todayNode.IndexOf(todayString) + 13;

            var end = todayNode.Substring(start, todayNode.Length - start).IndexOf("Mix denn");

            var body = todayNode.Substring(start, end);

            var soupString = Regex.Match(body, "Polévky:<br>.+?(?=(1.))");

            foreach (Match item in Regex.Matches(soupString.Value, "[r]>.+?(?=<[bs])"))
            {
                var newItem = new RestaurantMenuItem
                {
                    FoodType = FoodType.Soup,
                    Description = item.Value.Substring(2)
                };

                result.Add(newItem);
            }

            var matches = Regex.Matches(body, "<b>.+?<\\/b>");

            foreach (Match match in matches)
            {
                var item = new RestaurantMenuItem
                {
                    FoodType = FoodType.Main,
                    Price = Regex.Match(match.Value, "(?='>)(.*)(?=</span)").Value.Substring(2),
                    Description = Regex.Match(match.Value, "(?=<b>)(.+?)(?=<span)").Value.Substring(3) + "  "
                };

                result.Add(item);
            }

            return result;
        }

        private static string GetTodayInCzech()
        {
            switch (DateTime.Today.DayOfWeek)
            {
                case DayOfWeek.Monday:
                    return "Pondělí";
                case DayOfWeek.Tuesday:
                    return "Úterý";
                case DayOfWeek.Wednesday:
                    return "Středa";
                case DayOfWeek.Thursday:
                    return "Čtvrtek";
                case DayOfWeek.Friday:
                    return "Pátek";
#if DEBUG
                case DayOfWeek.Saturday:
                case DayOfWeek.Sunday:
                    return "Pátek";
#endif
            }

            return string.Empty;
        }

    }
}
