using System;
using System.Threading.Tasks;
using LunchAgent.Core.Google;
using LunchAgent.Core.MenuPosting;
using LunchAgent.Core.Menus;
using LunchAgent.Core.Messages;
using LunchAgent.Core.Restaurants;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace LunchAgent.Function;

public static class PostMenus
{
    [FunctionName("PostMenus")]
    public static async Task RunAsync([TimerTrigger("0 0,30 7,8,9 * * MON-FRI")] TimerInfo myTimer, ILogger logger)
    {
        logger.LogInformation($"PostMenus function started at: {DateTime.UtcNow}");

        var storageAccountConnectionString = Environment.GetEnvironmentVariable("StorageAccountConnectionString");
        var googleCreds = Environment.GetEnvironmentVariable("GoogleCreds");

        if (storageAccountConnectionString == null)
            throw new Exception("Connection string cannot be null");
        
        if (googleCreds == null)
            throw new Exception("Google credentials cannot be null");

        var restaurantService = new RestaurantService();
        var menuReadingService = new MenuReadingService(logger);

        menuReadingService.GetMenus(restaurantService.Get());
        
        var googleChatService = new GoogleChatService(googleCreds);
        var storedMessagesService = new StoredMessagesService(storageAccountConnectionString);

        var menuPostingService = new MenuPostingService(logger, restaurantService, menuReadingService,
            googleChatService, storedMessagesService);

        await menuPostingService.PostMenus();

        logger.LogInformation($"PostMenus function ended at: {DateTime.UtcNow}");
    }
}