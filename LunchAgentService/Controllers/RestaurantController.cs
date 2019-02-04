using System.ComponentModel.DataAnnotations;
using LunchAgentService.Services;
using Microsoft.AspNetCore.Mvc;

namespace LunchAgentService.Controllers
{
    [Route("api/restaurant")]
    public class RestaurantController : Controller
    {
        private RestaurantService RestaurantService { get; }

        public RestaurantController(RestaurantService restaurantService)
        {
            RestaurantService = restaurantService;
        }

        [HttpGet("menus")]
        public IActionResult GetMenus()
        {
            return new JsonResult(RestaurantService.GetMenus());
        }

        [HttpGet("setting")]
        public IActionResult GetSetting()
        {
            return new JsonResult(RestaurantService.ServiceSetting);
        }

        [HttpPost("setting")]
        public IActionResult SetSetting([FromBody][Required]RestaurantServiceSetting setting)
        {
            RestaurantService.ServiceSetting = setting;

            return new OkResult();
        }
    }
}
