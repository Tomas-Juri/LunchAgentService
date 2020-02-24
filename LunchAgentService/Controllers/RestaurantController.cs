using System.ComponentModel.DataAnnotations;
using LunchAgentService.Entities;
using LunchAgentService.Services;
using LunchAgentService.Services.DatabaseService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LunchAgentService.Controllers
{
    [Route("api/restaurant")]
    [Authorize(Roles = Role.SuperAdmin + "," + Role.Admin)]
    public class RestaurantController : Controller
    {
        private IRestaurantService RestaurantService { get; }
        private IStorageService StorageService { get; }

        public RestaurantController(IRestaurantService restaurantService, IStorageService storageService)
        {
            RestaurantService = restaurantService;
            StorageService = storageService;
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
            return new JsonResult(StorageService.Get<Restaurant>());
        }

        [HttpGet("restaurant/{id}")]
        [AllowAnonymous]
        public IActionResult GetRestaurant([FromRoute][Required]string id)
        {
            return new JsonResult(StorageService.Get<Restaurant>(id));
        }

        [HttpDelete("restaurant/{id}")]
        public IActionResult DeleteRestaurant([FromRoute][Required]string id)
        {
            StorageService.Delete<Restaurant>(id);
            return new OkResult();
        }

        [HttpPost("restaurant")]
        public IActionResult AddOrUpdateRestaurant([FromBody][Required]Restaurant restaurant)
        {
            return new JsonResult(StorageService.AddOrUpdate(restaurant));
        }
    }
}
