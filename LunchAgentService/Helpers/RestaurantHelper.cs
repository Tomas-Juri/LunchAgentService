using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LunchAgentService.Helpers.Entities;

namespace LunchAgentService.Helpers
{
    public class RestaurantHelper
    {
        public RestaurantHelper(string jsonString)
        {

        }

        public List<Tuple<RestaurantSettings, List<MenuItem>>> GetMenus()
        {
            throw new NotImplementedException();
        }

        public bool CheckMenus(List<Tuple<RestaurantSettings, List<MenuItem>>> menus)
        {
            throw new NotImplementedException();
        }
    }
}
