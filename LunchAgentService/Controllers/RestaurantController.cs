using LunchAgentService.Services.RestaurantService;
using Microsoft.AspNetCore.Mvc;
using LunchAgentService.Services.TeamsService;
using System.Collections.Generic;
using System;
using Microsoft.Extensions.Logging;
using System.Linq;

namespace LunchAgentService.Controllers
{
    [Route("Restaurant")]
    public class RestaurantController : Controller
    {
        private IRestaurantService RestaurantService { get; }
        private ITeamsService TeamsService { get; }
        private ILogger Log { get; }

        public RestaurantController(IRestaurantService restaurantService, ITeamsService teamsService, ILogger<RestaurantController> log)
        {
            Log = log;
            RestaurantService = restaurantService;
            TeamsService = teamsService;
        }

        [HttpGet("menus")]
        public IActionResult GetMenus()
        {
            var menus = new List<RestaurantMenu>();
            menus = RestaurantService.GetMenus();

            var timezone = TimeZoneInfo.GetSystemTimeZones().Single(tzi => tzi.DisplayName.Contains("Prague"));

            var currentTimeZone = TimeZoneInfo.Local;


            Log.LogInformation("Prague time zone" + timezone.ToString());
            Log.LogInformation("currentTimeZone" + currentTimeZone.ToString());

            Log.LogInformation("TimeZoneInfo.Local.DaylightName" + TimeZoneInfo.Local.DaylightName.ToString());








            TeamsService.Post(menus);

            return new JsonResult(menus);
        }
    }
}
