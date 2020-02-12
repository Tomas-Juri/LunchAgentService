using System.ComponentModel.DataAnnotations;
using System.Linq;
using LunchAgentService.Entities;
using LunchAgentService.Services;
using LunchAgentService.Services.DatabaseService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;

namespace LunchAgentService.Controllers
{
    [Route("api/restaurant")]
    [Authorize(Roles = Role.SuperAdmin + "," + Role.Admin)]
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
        [AllowAnonymous]
        public IActionResult GetMenus()
        {
            return new JsonResult(RestaurantService.GetMenus());
        }

        [HttpGet("setting")]
        [AllowAnonymous]
        public IActionResult GetSetting()
        {
            return new JsonResult(DatabaseService.Get<RestaurantMongo>().Select(x => x.ToApi()));
        }

        [HttpGet("restaurant/{id}")]
        [AllowAnonymous]
        public IActionResult GetRestaurant([FromRoute][Required]string id)
        {
            return new JsonResult(DatabaseService.Get<RestaurantMongo>(ObjectId.Parse(id)).ToApi());
        }

        [HttpDelete("restaurant/{id}")]
        public IActionResult DeleteRestaurant([FromRoute][Required]string id)
        {
            return new JsonResult(DatabaseService.Delete<RestaurantMongo>(ObjectId.Parse(id)));
        }

        [HttpPost("restaurant")]
        public IActionResult AddOrUpdateRestaurant([FromBody][Required]RestaurantApi restaurant)
        {
            return new JsonResult(DatabaseService.AddOrUpdate(restaurant.ToMongo()));
        }
    }
}
