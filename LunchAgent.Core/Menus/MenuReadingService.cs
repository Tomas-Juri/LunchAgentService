using HtmlAgilityPack;
using LunchAgent.Core.Menus.Entities;
using LunchAgent.Core.Restaurants.Entitties;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace LunchAgent.Core.Menus;

public class MenuReadingService : IMenuReadingService
{
    private ILogger Log { get; }

    public MenuReadingService(ILogger log)
    {
        Log = log;
    }

    public List<RestaurantMenu> GetMenus(IReadOnlyCollection<Restaurant> restaurants)
    {
        var result = new List<RestaurantMenu>();
        var document = new HtmlDocument();

        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

        foreach (var restaurant in restaurants)
        {
            // TODO Do htttp client
            using (var client = new WebClient())
            {
                try
                {
                    var data = restaurant.Url.Contains("makalu")
                        ? Encoding.UTF8.GetString(client.DownloadData(restaurant.Url))
                        : Encoding.GetEncoding(1250).GetString(client.DownloadData(restaurant.Url));

                    document.LoadHtml(data);
                }
                catch (Exception e)
                {
                    Log.LogDebug($"Failed to get menu from {restaurant.Name}", e);
                }
            }

            try
            {
                var parsedMenu = restaurant.Url.Contains("makalu")
                    ? ParseMenuFromMakalu(document.DocumentNode)
                    : ParseMenuFromMenicka(document.DocumentNode);

                result.Add(new RestaurantMenu()
                {
                    Items = parsedMenu,
                    Restaurant = restaurant with { }
                });
            }
            catch (Exception e)
            {
                Log.LogDebug($"Failed to parse menu from {restaurant.Name}", e);
            }

            Log.LogDebug($"Sucessfully got menu from {restaurant.Name}");
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
                item = item with
                {
                    FoodType = FoodType.Soup,
                    Description = Regex.Replace(
                        food.SelectNodes(".//td").Single(x => x.GetClasses().Contains("food")).InnerText,
                        "\\d+.?", string.Empty),
                    Price = food.SelectNodes(".//td").Single(x => x.GetClasses().Contains("prize")).InnerText
                };
            }
            else
            {
                item = item with
                {
                    FoodType = FoodType.Main,
                    Description = Regex.Replace(
                        food.SelectNodes(".//td").Single(x => x.GetClasses().Contains("food")).InnerText,
                        "\\d+.?", string.Empty),
                    Price = food.SelectNodes(".//td").Single(x => x.GetClasses().Contains("prize")).InnerText,
                    Index = food.SelectNodes(".//td").Single(x => x.GetClasses().Contains("no")).InnerText,
                };
            }

            result.Add(item);
        }

        return result;
    }

    private static List<RestaurantMenuItem> ParseMenuFromMakalu(HtmlNode todayMenu)
    {
        var todayString = GetTodayInCzech();

        var menuNodes = todayMenu.Descendants("div")
            .Where(node => node.HasClass("weeklyDayCont"))
            .Where(node => node.InnerText.Contains(todayString, StringComparison.InvariantCultureIgnoreCase))
            .SelectMany(node => node.Descendants())
            .Where(node => node.HasClass("menuPageMealName"));

        var menuItems = menuNodes.Select(node =>
        {
            var childNodes = node.Descendants("td").ToList();
            var isSoup = node.InnerText.Contains("polévka", StringComparison.InvariantCultureIgnoreCase);

            return new RestaurantMenuItem
            {
                FoodType = isSoup ? FoodType.Soup : FoodType.Main,
                Index = isSoup ? string.Empty : childNodes[0].InnerText.Trim(),
                Description = childNodes[isSoup ? 0 : 1].InnerText.Trim() + (isSoup ? " / " + childNodes[1].InnerText.Trim() : " " + childNodes[2].InnerText.Trim()),
                Price = isSoup ? string.Empty : childNodes[3].InnerText.Trim()
            };
        }).ToList();

        return menuItems;
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