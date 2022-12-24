using LunchAgent.Core.Restaurants.Entitties;

namespace LunchAgent.Core.Menus.Entities;

public record RestaurantMenu
{
    public Restaurant Restaurant { get; init; } = new();

    public List<RestaurantMenuItem> Items { get; init; } = new();
}