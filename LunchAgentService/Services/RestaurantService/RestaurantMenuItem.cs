namespace LunchAgentService.Services.RestaurantService
{
    public class RestaurantMenuItem
    {
        public FoodType FoodType { get; set; }
        public string Index { get; set; }
        public string Description { get; set; }
        public string Price { get; set; }

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
