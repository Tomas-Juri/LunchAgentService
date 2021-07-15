using System.Collections.Generic;

namespace LunchAgentService.Helpers
{
    public class AppSettings
    {
        public string TeamsBotLink { get; set; }
        public List<RestaurantSettings> RestaurantSettings { get; set; }
    }

    public class RestaurantSettings
    {
        public string Name { get; set; }
        public string Url { get; set; }
    }
}