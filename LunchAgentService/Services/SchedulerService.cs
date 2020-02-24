using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace LunchAgentService.Services
{
    public class SchedulerService : HostedService
    {
        private ISlackService SlackService { get; }
        private IRestaurantService RestaurantService { get; }
        private ILogger Log { get; }

        public SchedulerService(IRestaurantService restaurantService, ISlackService slackService, ILogger<SchedulerService> log)
        {
            RestaurantService = restaurantService;
            SlackService = slackService;
            Log = log;
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            Log.LogDebug("Starting scheduling");

            while (cancellationToken.IsCancellationRequested == false)
            {
                if (DateTime.Now.Hour > 11 || DateTime.Now.DayOfWeek == DayOfWeek.Sunday || DateTime.Now.DayOfWeek == DayOfWeek.Saturday)
                {
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
