using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using LunchAgentService.Services.TeamsService;
using LunchAgentService.Services.RestaurantService;

namespace LunchAgentService.Services
{
    public class SchedulerService : HostedService
    {
        private ITeamsService TeamsService { get; }
        private IRestaurantService RestaurantService { get; }
        private ILogger Log { get; }

        public SchedulerService(IRestaurantService restaurantService, ITeamsService teamsService, ILogger<SchedulerService> log)
        {
            RestaurantService = restaurantService;
            TeamsService = teamsService;
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

                try
                {
                    if (DateTime.Now.Hour == 10)
                    {
                        Log.LogDebug("Getting menus");

                        var menus = RestaurantService.GetMenus();

                        TeamsService.Post(menus);

                        Log.LogDebug("Posting menus to teams");

                    }
                    Log.LogDebug("Menus posted sucessfully");
                }
                catch (Exception exception)
                {
                    Log.LogError("Failed to post menus to teams", exception);
                }

                Log.LogDebug("Sleeping for 15 minutes");

                await Task.Delay(TimeSpan.FromMinutes(15), cancellationToken);
            }
        }
    }
}
