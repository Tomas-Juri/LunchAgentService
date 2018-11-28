using System;
using System.Threading;
using System.Threading.Tasks;
using log4net;
using LunchAgentService.Services.RestaurantService;
using LunchAgentService.Services.SlackService;

namespace LunchAgentService.Services
{
    public class SchedulerService : HostedService
    {
        private SlackService.SlackService SlackService { get; set; }
        private RestaurantService.RestaurantService RestaurantService { get; set; }
        private ILog Log { get; }

        public SchedulerService(RestaurantService.RestaurantService restaurantService, SlackService.SlackService slackService, ILog log)
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
                    await Task.Delay(DateTime.Today.AddHours(7).AddDays(1) - DateTime.Now, cancellationToken);
                }

                var menus = RestaurantService.GetMenus();

                Log.Debug("Posting menus to slack");

                try
                {
                    SlackService.ProcessMenus(menus);
                }
                catch (Exception exception)
                {
                    Log.Error("Failed to post menus to slack", exception);
                }

                await Task.Delay(TimeSpan.FromMinutes(15), cancellationToken);
            }
        }
    }
}
