using System;
using System.Threading;
using System.Threading.Tasks;
using log4net;
using LunchAgentService.Helpers;

namespace LunchAgentService.Services
{
    public class SchedulerService : HostedService
    {
        private SlackHelper SlackHelper { get; set; }
        private RestaurantHelper RestaurantHelper { get; set; }
        private ILog Log { get; }

        private ScheduleStatus _status;
        private ScheduleStatus Status
        {
            get
            {
                return _status;
            }
            set
            {
                Log.Debug($"Setting status to '{value.ToString()}'");
                _status = value;
            }
        }

        public SchedulerService(RestaurantHelper restauranthelper, SlackHelper slackHelper, ILog Log)
        {
            RestaurantHelper = restauranthelper;
            SlackHelper = slackHelper;
            this.Log = Log;
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            Log.Debug("Starting scheduling");

            Status = ScheduleStatus.Post;

            while (cancellationToken.IsCancellationRequested == false)
            {
                if (Status == ScheduleStatus.DoneToday || DateTime.Now.DayOfWeek == DayOfWeek.Sunday || DateTime.Now.DayOfWeek == DayOfWeek.Saturday)
                {
                    Log.Debug("Done for today, sleeping until next day");

                    // Sleep until 7:00 of next day
                    await Task.Delay(DateTime.Today.AddDays(1).AddHours(7) - DateTime.Now, cancellationToken);

                    Status = ScheduleStatus.Post;

                    continue;
                }

                Log.Debug("Getting menus");

                var menus = RestaurantHelper.GetMenus();

                if (Status == ScheduleStatus.Post)
                {
                    Log.Debug("Posting menus to slack");

                    SlackHelper.PostMenu(menus);
                }

                if (Status == ScheduleStatus.Update)
                {
                    Log.Debug("Updating menus to slack");

                    SlackHelper.UpdateMenu(menus);
                }

                Status = ScheduleStatus.Update;

                // End updating after 11:00
                if (DateTime.Now.Hour > 11)
                {
                    Log.Debug("Its past 11, stopping for today");

                    Status = ScheduleStatus.DoneToday;
                }

                Log.Debug("Sleeping for 15 minutes");

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
