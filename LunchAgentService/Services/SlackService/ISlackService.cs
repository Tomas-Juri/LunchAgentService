using System.Collections.Generic;
using LunchAgentService.Entities;

namespace LunchAgentService.Services
{
    public interface ISlackService
    {
        void PostToSlack(List<RestaurantMenu> menus, SlackSettingMongo settings);
        void ProcessMenus(List<RestaurantMenu> menus);
        void UpdateToSlack(List<RestaurantMenu> menus, string timestamp, SlackSettingMongo settings);
        IList<Reaction> GetReactionsToLunch();
    }
}