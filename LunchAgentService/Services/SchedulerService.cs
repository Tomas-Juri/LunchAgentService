using System;
using System.Threading;
using System.Threading.Tasks;
using WebApplication1.Helpers;

namespace WebApplication1.Services
{
    public class SchedulerService : HostedService
    {
        private SlackHelper SlackHelper { get; set; }
        private RestaurantHelper RestaurantHelper { get; set; }

        private ScheduleStatus Status { get; set; }

        public SchedulerService(RestaurantHelper restauranthelper, SlackHelper slackHelper)
        {
            RestaurantHelper = restauranthelper;
            SlackHelper = slackHelper;
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            Status = ScheduleStatus.Post;

            while (cancellationToken.IsCancellationRequested == false)
            {
                if (Status == ScheduleStatus.DoneToday)
                {
                    // Sleep until 7:00 of next day
                    await Task.Delay(DateTime.Today.AddDays(1).AddHours(7) - DateTime.Now, cancellationToken);

                    Status = ScheduleStatus.Post;

                    continue;
                }

                var menus = RestaurantHelper.GetMenus();

                if (Status == ScheduleStatus.Post)
                {
                    SlackHelper.PostMenu(menus);
                }

                if (Status == ScheduleStatus.Update)
                {
                    SlackHelper.UpdateMenu(menus);
                }

                if (RestaurantHelper.CheckMenus(menus) == false)
                {
                    Status = ScheduleStatus.Update;
                }
                else
                {
                    Status = ScheduleStatus.DoneToday;
                }

                // End updating after 11:00
                if (DateTime.Now.Hour > 11)
                {
                    Status = ScheduleStatus.DoneToday;
                }

                await Task.Delay(TimeSpan.FromMinutes(15), cancellationToken);
            }
        }


        private enum ScheduleStatus
        {
            DoneToday,
            Update,
            Post
        }
    }
}
