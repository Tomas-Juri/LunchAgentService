//using System;
//using System.ComponentModel.DataAnnotations;
//using LunchAgentService.Entities;
//using LunchAgentService.Services;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.Extensions.Logging;

//namespace LunchAgentService.Controllers
//{
//    [Route("api/slack")]
//    [Authorize(Roles = Role.SuperAdmin)]
//    public class SlackController : Controller
//    {
//        private IRestaurantService RestaurantService { get; }
//        private ISlackService SlackService { get; }
//        private ILogger Log { get; }

//        public SlackController(IRestaurantService restaurantService, ISlackService slackService, ILogger<SlackController> log)
//        {
//            RestaurantService = restaurantService;
//            SlackService = slackService;
//            Log = log;
//        }

//        [HttpPost("forcePost")]
//        [AllowAnonymous]
//        public IActionResult ForcePost()
//        {
//            try
//            {
//                var menus = RestaurantService.GetMenus();

//                SlackService.ProcessMenus(menus);
//            }
//            catch (Exception e)
//            {
//                Log.LogError("Error occured while calling ForcePost", e);

//                return new StatusCodeResult(500);
//            }
//            return new OkResult();
//        }

//        //[HttpGet("setting")]
//        //public IActionResult GetSetting()
//        //{
//        //    return new JsonResult(StorageService.Get<SlackSettingMongo>().FirstOrDefault()?.ToApi());
//        //}

//        [HttpPost("setting")]
//        public IActionResult SetSetting([FromBody][Required]Slack setting)
//        {
//            return new JsonResult(StorageService.AddOrUpdate(setting));
//        }
//    }
//}