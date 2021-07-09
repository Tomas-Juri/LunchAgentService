using System.ComponentModel.DataAnnotations;
using LunchAgentService.Services;
using LunchAgentService.Services.RestaurantService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LunchAgentService.Services.TeamsService;
using System.Collections.Generic;

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
            List<RestaurantMenu> menus = new List<RestaurantMenu>();
            menus = RestaurantService.GetMenus();

            TeamsService.Post(menus);

            return new JsonResult(menus);
        }


        //[HttpGet("setting")]
        //[AllowAnonymous]
        //public IActionResult GetSetting()
        //{
        //    return new JsonResult(RestaurantService.Get<Restaurant>());
        //}

        //[HttpGet("restaurant/{id}")]
        //[AllowAnonymous]
        //public IActionResult GetRestaurant([FromRoute][Required] string id)
        //{
        //    return new JsonResult(StorageService.Get<Restaurant>(id));
        //}

        //[HttpDelete("restaurant/{id}")]
        //public IActionResult DeleteRestaurant([FromRoute][Required] string id)
        //{
        //    StorageService.Delete<Restaurant>(id);
        //    return new OkResult();
        //}

        //[HttpPost("restaurant")]
        //public IActionResult AddOrUpdateRestaurant([FromBody][Required] Restaurant restaurant)
        //{
        //    return new JsonResult(StorageService.AddOrUpdate(restaurant));
        //}
    }
}
