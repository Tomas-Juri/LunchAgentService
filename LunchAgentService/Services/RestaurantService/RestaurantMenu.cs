using System;
using System.Collections.Generic;
using LunchAgentService.Entities;

namespace LunchAgentService.Services
{
    public class RestaurantMenu
    {
        public RestaurantMongo Restaurant { get; set; }
        public List<RestaurantMenuItem> Items { get; set; }
    }
}