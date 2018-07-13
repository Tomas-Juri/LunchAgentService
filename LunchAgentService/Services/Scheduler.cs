using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LunchAgentService.Helpers;
using LunchAgentService.Helpers.Entities;
using Newtonsoft.Json;

namespace LunchAgentService.Scheduler
{
    public class Scheduler
    {
        private RestaurantHelper RestaurantHelper { get; set; }
        private SlackHelper SlackHelper { get; set; }

        public Scheduler(RestaurantHelper restaurantHelper, SlackHelper slackHelper)
        {
            RestaurantHelper = restaurantHelper;
            SlackHelper = slackHelper;
        }

        public void Start()
        {
        }
    }

    public interface IHostedService
    {
        Task StartAsync(CancellationToken cancellationToken);

        Task StopAsync(CancellationToken cancellationToken);
    }

    public abstract class HostedService : IHostedService
    {

        private Task _executingTask;
        private CancellationTokenSource _cts;

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

            _executingTask = ExecuteAsync(_cts.Token);

            return _executingTask.IsCompleted ? _executingTask : Task.CompletedTask;
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            if (_executingTask == null)
            {
                return;
            }

            _cts.Cancel();

            await Task.WhenAny(_executingTask, Task.Delay(-1, cancellationToken));

            cancellationToken.ThrowIfCancellationRequested();
        }

        protected abstract Task ExecuteAsync(CancellationToken cancellationToken);
    }
}
