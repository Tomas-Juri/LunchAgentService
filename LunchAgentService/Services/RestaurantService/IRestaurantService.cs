using System.Collections.Generic;

namespace LunchAgentService.Services
{
    public interface IRestaurantService
    {
        List<RestaurantMenu> GetMenus();
    }
}