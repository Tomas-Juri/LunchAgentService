using System;
using System.Threading;
using System.Threading.Tasks;
using LunchAgentService.Services.TeamsService;
using LunchAgentService.Services.RestaurantService;
using System.Diagnostics;

namespace LunchAgentService.Services
{
    public class SchedulerService : HostedService
    {
        private ITeamsService TeamsService { get; }
        private IRestaurantService RestaurantService { get; }

        public SchedulerService(IRestaurantService restaurantService, ITeamsService teamsService)
        {
            RestaurantService = restaurantService;
            TeamsService = teamsService;
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            Trace.TraceInformation("Starting scheduling");            

            while (cancellationToken.IsCancellationRequested == false)
            {
                if (DateTime.Now.DayOfWeek == DayOfWeek.Sunday || DateTime.Now.DayOfWeek == DayOfWeek.Saturday)
                {
                    Trace.TraceInformation("Sleeping until tomorrow");
                    await Task.Delay(TimeSpan.FromDays(1), cancellationToken);
                }

                if(DateTime.Now.Hour == 10)
                {
                    try
                    {
                        Trace.TraceInformation("Getting menus");
                        var menus = RestaurantService.GetMenus();

                        Trace.TraceInformation("Posting menus to teams");
                        TeamsService.Post(menus);

                        await Task.Delay(TimeSpan.FromHours(23), cancellationToken);
                        Trace.TraceInformation("Menus posted sucessfully");
                    }
                    catch (Exception exc)
                    {
                        Trace.TraceError("Failed to post menus to teams", exc);

                        await Task.Delay(TimeSpan.FromMinutes(5), cancellationToken);
                        Trace.TraceInformation("Sleeping for 5 minutes");
                    }
                }
            }
        }
    }
}
