using LunchAgent.Core.Restaurants.Entitties;

namespace LunchAgent.Core.Restaurants;

public interface IRestaurantService
{
    IReadOnlyCollection<Restaurant> Get();
}