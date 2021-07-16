using LunchAgentService.Services.RestaurantService;
using Microsoft.AspNetCore.Mvc;
using LunchAgentService.Services.TeamsService;
using System.Collections.Generic;

using System.Diagnostics;


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

            Trace.TraceInformation("Datetime.Now from controller" + DateTime.Now.ToString());

            TeamsService.Post(menus);

            return new JsonResult(menus);
        }
    }
}
