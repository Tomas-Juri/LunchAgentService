using System;
using System.Threading;
using System.Threading.Tasks;
using LunchAgentService.Services.MachineLearningService;
using Microsoft.Extensions.Logging;

namespace LunchAgentService.Services
{
    public class SchedulerService : HostedService
    {
        private ISlackService SlackService { get; }
        private IRestaurantService RestaurantService { get; }
        private IMachineLearningService MachineLearningService { get; }
        private ILogger Log { get; }

        public SchedulerService(IRestaurantService restaurantService, ISlackService slackService, IMachineLearningService machineLearningService, ILogger<SchedulerService> log)
        {
            RestaurantService = restaurantService;
            SlackService = slackService;
            MachineLearningService = machineLearningService;
            Log = log;
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            Log.LogDebug("Starting scheduling");

            while (cancellationToken.IsCancellationRequested == false)
            {
                if (DateTime.Now.Hour > 11 || DateTime.Now.DayOfWeek == DayOfWeek.Sunday || DateTime.Now.DayOfWeek == DayOfWeek.Saturday)
                {
                    try
                    {
                        Log.LogDebug("Getting slack reactions");

                        var reactions = SlackService.GetReactionsToLunch();
                        var reactedMenus = RestaurantService.GetMenus();

                        Log.LogDebug("Processing slack reactions");

                        MachineLearningService.ProcessSlackLunchReactions(reactions, reactedMenus);
                    }
                    catch (Exception e)
                    {
                        Log.LogError("Error occured during processing slack reactions", e);
                    }

                    Log.LogDebug("Sleeping until tomorrow");

                    await Task.Delay(DateTime.Today.AddHours(7).AddDays(1) - DateTime.Now, cancellationToken);

                    continue;
                }

                Log.LogDebug("Getting menus");

                var menus = RestaurantService.GetMenus();

                Log.LogDebug("Posting menus to slack");

                try
                {
                    SlackService.ProcessMenus(menus);

                    Log.LogDebug("Menus posted sucessfully");
                }
                catch (Exception exception)
                {
                    Log.LogError("Failed to post menus to slack", exception);
                }

                Log.LogDebug("Sleeping for 15 minutes");

                await Task.Delay(TimeSpan.FromMinutes(15), cancellationToken);
            }
        }
    }
}
