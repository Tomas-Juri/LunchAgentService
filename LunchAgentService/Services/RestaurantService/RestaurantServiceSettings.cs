using System;
using System.Collections.Generic;
using System.Linq;

namespace LunchAgentService.Services.RestaurantService
{
    public class RestaurantServiceSetting 
    {
        public List<RestaurantSetting> Restaurants { get; set; }

        public RestaurantServiceSetting Clone()
        {
            return new RestaurantServiceSetting
            {
                Restaurants = Restaurants.ToList()
            };
        }
    }
}