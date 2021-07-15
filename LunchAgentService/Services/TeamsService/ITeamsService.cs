using System.Collections.Generic;
using LunchAgentService.Services.RestaurantService;


namespace LunchAgentService.Services.TeamsService
{
    public interface ITeamsService
    {
        string Post(List<RestaurantMenu> menus);
    }
}