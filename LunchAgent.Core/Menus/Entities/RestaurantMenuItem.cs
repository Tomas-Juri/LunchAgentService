namespace LunchAgent.Core.Menus.Entities;

public record RestaurantMenuItem
{
    public FoodType FoodType { get; init; }
    
    public string Index { get; init; } = string.Empty;
    
    public string Description { get; init; } = string.Empty;
    
    public string Price { get; init; } = string.Empty;
}