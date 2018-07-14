using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LunchAgentService.Setting;

namespace LunchAgentService.Services
{
    public class RestaurantGetterService
    {
        private RestaurantSetting _restaurantSetting;

        public RestaurantGetterService(RestaurantSetting restaurantSetting)
        {
            _restaurantSetting = restaurantSetting;
        }
    }
}
