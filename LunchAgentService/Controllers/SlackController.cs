using System;
using System.ComponentModel.DataAnnotations;
using LunchAgentService.Entities;
using LunchAgentService.Services;
using LunchAgentService.Services.DatabaseService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace LunchAgentService.Controllers
{
    [Route("api/slack")]
    [Authorize(Roles = Role.SuperAdmin)]
    public class SlackController : Controller
    {
        private IRestaurantService RestaurantService { get; }
        private ISlackService SlackService { get; }
        private IDatabaseService DatabaseService { get; }
        private ILogger Log { get; }

        public SlackController(IRestaurantService restaurantService, ISlackService slackService, IDatabaseService databaseService, ILogger<SlackController> log)
        {
            RestaurantService = restaurantService;
            SlackService = slackService;
            DatabaseService = databaseService;
            Log = log;
        }

        [HttpPost("forcePost")]
        [AllowAnonymous]
        public IActionResult ForcePost()
        {
            try
            {
                var menus = RestaurantService.GetMenus();

                SlackService.ProcessMenus(menus);
            }
            catch (Exception e)
            {
                Log.LogError("Error occured while calling ForcePost", e);

                return new StatusCodeResult(500);
            }
            return new OkResult();
        }

        //[HttpGet("setting")]
        //public IActionResult GetSetting()
        //{
        //    return new JsonResult(DatabaseService.Get<SlackSettingMongo>().FirstOrDefault()?.ToApi());
        //}

        [HttpPost("setting")]
        public IActionResult SetSetting([FromBody][Required]SlackSettingApi setting)
        {
            return new JsonResult(DatabaseService.AddOrUpdate(setting.ToMongo()));
        }
    }
}