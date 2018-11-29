using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using LunchAgentService.Services.SlackService;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LunchAgentServiceTests
{
    [TestClass]
    public class RestaurantServiceTests
    {
        [TestMethod]
        public  void Test()
        {
            var a = new SlackChannelMessage()
            {
                Timestamp = "1543399226.008600",
            };

            var b = a.Date;
        }


    }
}
