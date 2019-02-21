using System.ComponentModel.DataAnnotations;
using LunchAgentService.Services;
using Microsoft.AspNetCore.Mvc;

namespace LunchAgentService.Controllers
{
    [Route("api/restaurant")]
    public class RestaurantController : Controller
    {
        private RestaurantService RestaurantService { get; }
        private SettingService SettingService { get; }

        public RestaurantController(RestaurantService restaurantService, SettingService settingService)
        {
            RestaurantService = restaurantService;
            SettingService = settingService;
        }

        [HttpGet("menus")]
        public IActionResult GetMenus()
        {
            return new JsonResult(RestaurantService.GetMenus());
        }

        [HttpGet("setting")]
        public IActionResult GetSetting()
        {
            return new JsonResult(SettingService.GetRestaurantSetting());
        }

        [HttpPost("setting")]
        public IActionResult SetSetting([FromBody][Required]RestaurantServiceSetting setting)
        {
            SettingService.SaveRestaurantSettting(setting);

            return new OkResult();
        }
    }
}
