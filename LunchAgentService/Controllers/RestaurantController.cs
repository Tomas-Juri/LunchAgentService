using LunchAgentService.Services.RestaurantService;
using Microsoft.AspNetCore.Mvc;
using LunchAgentService.Services.TeamsService;
using System.Collections.Generic;
using System.Diagnostics;
using System;

namespace LunchAgentService.Controllers
{
    [Route("Restaurant")]
    public class RestaurantController : Controller
    {
        private IRestaurantService RestaurantService { get; }
        private ITeamsService TeamsService { get; }

        public RestaurantController(IRestaurantService restaurantService, ITeamsService teamsService)
        {
            RestaurantService = restaurantService;
            TeamsService = teamsService;
        }

        [HttpGet("menus")]
        public IActionResult GetMenus()
        {
            var menus = new List<RestaurantMenu>();
            menus = RestaurantService.GetMenus();

            Trace.TraceInformation(DateTime.Now.ToString());

            TeamsService.Post(menus);

            return new JsonResult(menus);
        }
    }
}
