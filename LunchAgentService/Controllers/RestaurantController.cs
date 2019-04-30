using System;
using System.ComponentModel.DataAnnotations;
using LunchAgentService.Entities;
using LunchAgentService.Services;
using LunchAgentService.Services.DatabaseService;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;

namespace LunchAgentService.Controllers
{
    [Route("api/restaurant")]
    public class RestaurantController : Controller
    {
        private IRestaurantService RestaurantService { get; }
        private IDatabaseService DatabaseService { get; }

        public RestaurantController(IRestaurantService restaurantService, IDatabaseService databaseService)
        {
            RestaurantService = restaurantService;
            DatabaseService = databaseService;
        }

        [HttpGet("menus")]
        public IActionResult GetMenus()
        {
            return new JsonResult(RestaurantService.GetMenus());
        }

        [HttpGet("setting")]
        public IActionResult GetSetting()
        {
            return new JsonResult(DatabaseService.Get<Restaurant>());
        }

        [HttpGet("restaurant")]
        public IActionResult GetRestaurant([Required][FromBody]ObjectId id)
        {
            return new JsonResult(DatabaseService.Get<Restaurant>(id)); 
        }

        [HttpDelete("restaurant")]
        public IActionResult DeleteRestaurant([Required][FromBody]ObjectId id)
        {
            return new JsonResult(DatabaseService.Delete<Restaurant>(id));
        }

        [HttpPost("restaurant")]
        public IActionResult AddOrUpdateRestaurant([Required][FromBody]Restaurant restaurant)
        {
            return new JsonResult(DatabaseService.AddOrUpdate(restaurant));
        }

    }
}
