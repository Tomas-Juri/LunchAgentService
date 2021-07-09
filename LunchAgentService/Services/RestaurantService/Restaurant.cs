using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LunchAgentService.Helpers;

namespace LunchAgentService.Services.RestaurantService
{
    public class Restaurant
    {
        public string Name { get; set; }
        public string Url { get; set; }

        public Restaurant(RestaurantSettings restaurantSettings)
        {
            Name = restaurantSettings.Name;
            Url = restaurantSettings.Url;
                
        }
    }
}
