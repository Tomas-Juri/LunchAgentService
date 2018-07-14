using System;
using System.Globalization;

namespace LunchAgentService.Helpers.Entities
{
    public class MenuItem
    {
        public FoodType FoodType { get; set; }
        public string Index { get; set; }
        public string Description { get; set; }
        public string Price { get; set; }

        public MenuItem(FoodType foodType, string index, string description, string price)
        {
            FoodType = foodType;
            Index = Convert.ToString(index, CultureInfo.CurrentCulture);
            Description = Convert.ToString(description, CultureInfo.CurrentCulture);
            Price = Convert.ToString(price, CultureInfo.CurrentCulture);
        }

        public MenuItem()
        {
        }

        public override string ToString()
        {
            return $"{Index} {Description} {Price}";
        }
    }

    public enum FoodType
    {
        Soup,
        Main
    }
}
