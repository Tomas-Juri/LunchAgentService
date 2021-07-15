using System.Collections.Generic;


namespace LunchAgentService.Services.RestaurantService
{
    public class RestaurantMenu
    {
        public Restaurant Restaurant { get; set; }
        public List<RestaurantMenuItem> Items { get; set; }
    }


}