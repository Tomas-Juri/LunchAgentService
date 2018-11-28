using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;
using LunchAgentService.Services.RestaurantService;
using LunchAgentService.Services.SlackService;
using Microsoft.AspNetCore.Mvc;

namespace LunchAgentService.Controllers
{
    [Route("api/slack")]
    public class SlackController : Controller
    {
        private RestaurantService RestaurantService { get; }
        private SlackService SlackService { get; }
        private ILog Log { get; }

        public SlackController(RestaurantService restaurantService, SlackService slackService, ILog Log)
        {
            RestaurantService = restaurantService;
            SlackService = slackService;
            this.Log = Log;
        }

        [HttpPost("forcePost")]
        public IActionResult ForcePost()
        {
            try
            {
                var menus = RestaurantService.GetMenus();

                SlackService.ProcessMenus(menus);
            }
            catch (Exception e)
            {
                Log.Error("Error occured while calling ForcePost", e);

                return new StatusCodeResult(500);
            }

            return new OkResult();
        }

        [HttpGet("setting")]
        public IActionResult GetSetting()
        {
            return new JsonResult(SlackService.ServiceSetting);
        }

        [HttpPost("setting")]
        public IActionResult SetSetting([FromBody][Required]SlackServiceSetting setting)
        {
            SlackService.ServiceSetting = setting;

            return new OkResult();
        }
    }
}