using System.Collections.Generic;

namespace LunchAgentService.Services.RestaurantService
{
    public interface IRestaurantService
    {
        List<RestaurantMenu> GetMenus();
    }
}