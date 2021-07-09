using System.Collections.Generic;
using LunchAgentService.Services.RestaurantService;
using LunchAgentService.Services.TeamsService;


namespace LunchAgentService.Services.TeamsService
{
    public interface ITeamsService
    {
        string Post(List<RestaurantMenu> menus);
    }
}