using System;
using System.ComponentModel.DataAnnotations;
using log4net;
using LunchAgentService.Services;
using Microsoft.AspNetCore.Mvc;

namespace LunchAgentService.Controllers
{
    [Route("api/slack")]
    public class SlackController : Controller
    {
        private RestaurantService RestaurantService { get; }
        private SlackService SlackService { get; }
        private SettingService SettingService { get; }
        private ILog Log { get; }

        public SlackController(RestaurantService restaurantService, SlackService slackService, SettingService settingService, ILog log)
        {
            RestaurantService = restaurantService;
            SlackService = slackService;
            SettingService = settingService;
            Log = log;
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
            return new JsonResult(SettingService.GetSlackSetting());
        }

        [HttpPost("setting")]
        public IActionResult SetSetting([FromBody][Required]SlackServiceSetting setting)
        {
            SettingService.SaveSlackSetting(setting);

            return new OkResult();
        }
    }
}