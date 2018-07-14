using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using HtmlAgilityPack;
using LunchAgentService.Helpers.Entities;

namespace LunchAgentService.Helpers
{
    public class RestaurantHelper
    {
        private List<RestaurantSettings> _restaurantSettingses;

        public List<RestaurantSettings> RestaurantSettingses
        {
            get
            {
                lock (_restaurantSettingses)
                {
                    return _restaurantSettingses;
                }
            }
            set
            {
                lock (_restaurantSettingses)
                {
                    _restaurantSettingses = value;
                }
            }
        }

        public RestaurantHelper(IEnumerable<RestaurantSettings> restaurantSettings)
        {
            _restaurantSettingses = restaurantSettings.ToList();
        }

        public List<Tuple<RestaurantSettings, List<MenuItem>>> GetMenus()
        {
            var result = new List<Tuple<RestaurantSettings, List<MenuItem>>>();

            var document = new HtmlDocument();

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            lock (_restaurantSettingses)
            {
                foreach (var setting in _restaurantSettingses)
                {
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
                            Console.WriteLine(e);
                        }
                    }

                    try
                    {
                        var parsedMenu = setting.Url.Contains("makalu")
                            ? ParseMenuFromMakalu(document.DocumentNode)
                            : ParseMenuFromMenicka(document.DocumentNode);

                        result.Add(Tuple.Create(setting, parsedMenu));
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                }
            }

            return result;
        }

        private static List<MenuItem> ParseMenuFromMenicka(HtmlNode todayMenu)
        {
            var result = new List<MenuItem>();

            var foodMenus = todayMenu.SelectNodes(".//tr")
                .Where(node => node.GetClasses().Contains("soup") || node.GetClasses().Contains("main"));

            foreach (var food in foodMenus)
            {
                var item = new MenuItem();

                if (food.GetClasses().Contains("soup") == true)
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

        private static List<MenuItem> ParseMenuFromMakalu(HtmlNode todayMenu)
        {
            var result = new List<MenuItem>();

            var todayString = GetTodayInCzech();

            var todayNode = string.Join(" ", todayMenu.SelectNodes(".//div[contains(@class,TJStrana)]").Where(x => x.GetClasses().Contains("TJStrana")).Select(x => x.InnerHtml));

            var start = todayNode.IndexOf(todayString) + 13;

            var end = todayNode.Substring(start, todayNode.Length - start).IndexOf("Mix denn");

            var body = todayNode.Substring(start, end);

            var soupString = Regex.Match(body, "Polévky:<br>.+?(?=(1.))");

            foreach (Match item in Regex.Matches(soupString.Value, "[r]>.+?(?=<[bs])"))
            {
                var newItem = new MenuItem();

                newItem.FoodType = FoodType.Soup;
                newItem.Description = item.Value.Substring(2);

                result.Add(newItem);
            }

            var matches = Regex.Matches(body, "<b>.+?<\\/b>");

            foreach (Match match in matches)
            {
                var item = new MenuItem();

                item.FoodType = FoodType.Main;
                item.Price = Regex.Match(match.Value, "(?='>)(.*)(?=</span)").Value.Substring(2);
                item.Description = Regex.Match(match.Value, "(?=<b>)(.+?)(?=<span)").Value.Substring(3) + "  ";

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
