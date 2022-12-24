using LunchAgent.Core.Menus.Entities;
using LunchAgent.Core.Restaurants.Entitties;

namespace LunchAgent.Core.Menus;

public interface IMenuReadingService
{
    List<RestaurantMenu> GetMenus(IReadOnlyCollection<Restaurant> restaurants);
}