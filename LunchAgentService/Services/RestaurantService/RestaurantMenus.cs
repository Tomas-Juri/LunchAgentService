using System;
using System.Collections.Generic;

namespace LunchAgentService.Services
{
    public class RestaurantMenu
    {
        public RestaurantSetting Restaurant { get; set; }
        public List<RestaurantMenuItem> Items { get; set; }
    }
}