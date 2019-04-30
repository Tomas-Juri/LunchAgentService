using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using log4net;
using LunchAgentService.Entities;
using LunchAgentService.Services;
using LunchAgentService.Services.DatabaseService;
using Microsoft.AspNetCore.Mvc;

namespace LunchAgentService.Controllers
{
    [Route("api/slack")]
    public class SlackController : Controller
    {
        private IRestaurantService RestaurantService { get; }
        private ISlackService SlackService { get; }
        private IDatabaseService DatabaseService { get; }
        private ILog Log { get; }

        public SlackController(IRestaurantService restaurantService, ISlackService slackService, IDatabaseService databaseService, ILog log)
        {
            RestaurantService = restaurantService;
            SlackService = slackService;
            DatabaseService = databaseService;
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
            return new JsonResult(DatabaseService.Get<SlackSetting>().FirstOrDefault());
        }

        [HttpPost("setting")]
        public IActionResult SetSetting([FromBody][Required]SlackSetting setting)
        {
            return new JsonResult(DatabaseService.AddOrUpdate(setting));
        }
    }
}