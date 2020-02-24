using System.Collections.Generic;
using LunchAgentService.Entities;

namespace LunchAgentService.Services
{
    public interface ISlackService
    {
        void PostToSlack(List<RestaurantMenu> menus, Slack settings);
        void ProcessMenus(List<RestaurantMenu> menus);
        void UpdateToSlack(List<RestaurantMenu> menus, string timestamp, Slack settings);
        IList<Reaction> GetReactionsToLunch();
    }
}