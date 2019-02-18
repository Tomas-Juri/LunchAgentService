using System;
using System.Threading;
using System.Threading.Tasks;
using log4net;

namespace LunchAgentService.Services
{
    public class SchedulerService : HostedService
    {
        private SlackService SlackService { get; }
        private RestaurantService RestaurantService { get; }
        private ILog Log { get; }

        public SchedulerService(RestaurantService restaurantService, SlackService slackService, ILog log)
        {
            RestaurantService = restaurantService;
            SlackService = slackService;
            Log = log;
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            Log.Debug("Starting scheduling");

            while (cancellationToken.IsCancellationRequested == false)
            {
                if (DateTime.Now.Hour > 11 || DateTime.Now.DayOfWeek == DayOfWeek.Sunday || DateTime.Now.DayOfWeek == DayOfWeek.Saturday)
                {
                    Log.Debug("Sleeping until tomorrow");

                    await Task.Delay(DateTime.Today.AddHours(7).AddDays(1) - DateTime.Now, cancellationToken);

                    continue;
                }

                Log.Debug("Getting menus");

                var menus = RestaurantService.GetMenus();

                Log.Debug("Posting menus to slack");

                try
                {
                    SlackService.ProcessMenus(menus);

                    Log.Debug("Menus posted sucessfully");
                }
                catch (Exception exception)
                {
                    Log.Error("Failed to post menus to slack", exception);
                }

                Log.Debug("Sleeping for 15 minutes");

                await Task.Delay(TimeSpan.FromMinutes(15), cancellationToken);
            }
        }
    }
}
