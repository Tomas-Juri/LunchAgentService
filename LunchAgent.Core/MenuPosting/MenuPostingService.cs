using Google.Apis.HangoutsChat.v1.Data;
using LunchAgent.Core.Google;
using LunchAgent.Core.Menus;
using LunchAgent.Core.Menus.Entities;
using LunchAgent.Core.Messages;
using LunchAgent.Core.Restaurants;
using Microsoft.Extensions.Logging;

namespace LunchAgent.Core.MenuPosting;

public class MenuPostingService : IMenuPostingService
{
    private readonly ILogger _logger;
    private readonly IRestaurantService _restaurantService;
    private readonly IMenuReadingService _menuReadingService;
    private readonly IGoogleChatService _googleChatService;
    private readonly IStoredMessagesService _storedMessagesService;

    public MenuPostingService(ILogger logger, IRestaurantService restaurantService,
        IMenuReadingService menuReadingService, IGoogleChatService googleChatService,
        IStoredMessagesService storedMessagesService)
    {
        _logger = logger;
        _restaurantService = restaurantService;
        _menuReadingService = menuReadingService;
        _googleChatService = googleChatService;
        _storedMessagesService = storedMessagesService;
    }

    public async Task PostMenus()
    {
        var today = DateTime.Today;

        _logger.LogInformation("Getting restaurant settings.");
        var restaurants = _restaurantService.Get();

        _logger.LogInformation("Getting menus for restaurants.");
        var menus = _menuReadingService.GetMenus(restaurants);

        _logger.LogInformation("Getting available spaces.");
        var spaces = await _googleChatService.GetSpaces();

        //         spaces = spaces.Where(x => x.DisplayName == "Lunch Agent ZL - TEST").ToList();
        
        _logger.LogInformation("Start sending messages.");
        foreach (var space in spaces)
        {
            _logger.LogInformation("Sending message to {spaceName} ({spaceId}) space.", space.DisplayName, space.Name);

            var storedMessageName = await _storedMessagesService.Get(space.Name, today);

            var menuText = GetMenuText(menus);
            var now = DateTime.Now;
            var message = new Message
            {
                Text = $"*Meníčka na den {now.Day}/{now.Month}/{now.Year}*\n\n{menuText}",
                Space = space,
                Name = string.IsNullOrEmpty(storedMessageName) ? null : storedMessageName,
            };

            var messageResponse = string.IsNullOrEmpty(storedMessageName)
                ? await _googleChatService.CreateMessage(message, space.Name)
                : await _googleChatService.UpdateMessage(message, storedMessageName);

            await _storedMessagesService.Store(space.Name, today, messageResponse.Name);
        }
    }

    private static string GetMenuText(List<RestaurantMenu> menus)
    {
        var result = new List<string>();

        foreach (var menu in menus)
        {
            var soups = string.Join("\n", menu.Items
                .Where(item => item.FoodType == FoodType.Soup)
                .Select(item => $"_{item.Description.Trim()}_ {item.Price}"));

            var mains = string.Join("\n", menu.Items
                .Where(item => item.FoodType == FoodType.Main)
                .Select((item, index) => $"  {index + 1}. {item.Description} {item.Price}"));

            var items = string.Join("\n", soups, mains).Trim('\n');

            result.Add($"*{menu.Restaurant.Name}:* \n {items}");
        }

        return string.Join("\n\n", result);
    }
}