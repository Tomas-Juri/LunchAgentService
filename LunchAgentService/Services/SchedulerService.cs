using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using LunchAgentService.Services.TeamsService;
using LunchAgentService.Services.RestaurantService;
using System.Linq;

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
            Log.LogDebug("TimeZoneInfo.FindSystemTimeZoneById(Central Europe Standard Time).GetUtcOffset(DateTime.Now)).Hour: " + TimeZoneInfo.FindSystemTimeZoneById("Central Europe Standard Time").GetUtcOffset(DateTime.Now).Hours);
            Log.LogDebug("(DateTime.Now - TimeZoneInfo.FindSystemTimeZoneById(Central Europe Standard Time).GetUtcOffset(DateTime.Now)).Hour: " + (DateTime.Now - TimeZoneInfo.FindSystemTimeZoneById("Central Europe Standard Time").GetUtcOffset(DateTime.Now)).Hour);

            while (cancellationToken.IsCancellationRequested == false)
            {
                if (DateTime.Now.DayOfWeek == DayOfWeek.Sunday || DateTime.Now.DayOfWeek == DayOfWeek.Saturday)
                {
                    Log.LogDebug("Sleeping until tomorrow");

                    await Task.Delay(TimeSpan.FromDays(1), cancellationToken);
                }

                if ((DateTime.Now - TimeZoneInfo.FindSystemTimeZoneById("Central Europe Standard Time").GetUtcOffset(DateTime.Now)).Hour == 10)
                {
                    try
                    {
                        Log.LogDebug("Getting menus");

                        var menus = RestaurantService.GetMenus();

                        TeamsService.Post(menus);

                        Log.LogDebug("Posting menus to teams");

                        await Task.Delay(TimeSpan.FromHours(23), cancellationToken);

                        Log.LogDebug("Menus posted sucessfully");

                    }
                    catch (Exception exc)
                    {
                        Log.LogError("Failed to post menus to teams", exc);

                        Log.LogDebug("Sleeping for 5 minutes");

                        await Task.Delay(TimeSpan.FromMinutes(5), cancellationToken);
                    }
                }
            }
        }
    }
}
