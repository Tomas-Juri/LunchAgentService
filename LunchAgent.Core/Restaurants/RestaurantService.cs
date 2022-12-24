using LunchAgent.Core.Restaurants.Entitties;

namespace LunchAgent.Core.Restaurants;

public class RestaurantService : IRestaurantService
{
    public IReadOnlyCollection<Restaurant> Get()
    {
        return new List<Restaurant>
        {
            new()
            {
                Name = "Canada",
                Url = "http://www.menicka.cz/api/iframe/?id=6&bg=vhite&color=black&size=18&datum=dnes"
            },
            new()
            {
                Name = "Flip",
                Url = "http://www.menicka.cz/api/iframe/?id=24&bg=vhite&color=black&size=18&datum=dnes"
            },
            new()
            {
                Name = "Puor",
                Url = "http://www.menicka.cz/api/iframe/?id=2992&bg=vhite&color=black&size=18&datum=dnes"
            },
            new()
            {
                Name = "Celnice",
                Url = "http://www.menicka.cz/api/iframe/?id=17&bg=vhite&color=black&size=18&datum=dnes"
            },
            new()
            {
                Name = "Budvarka",
                Url = "http://www.menicka.cz/api/iframe/?id=5881&bg=vhite&color=black&size=18&datum=dnes"
            },
            new()
            {
                Name = "Bistrotéka Valachy",
                Url = "http://www.menicka.cz/api/iframe/?id=6114&bg=vhite&color=black&size=18&datum=dnes"
            },
            new()
            {
                Name = "Bistro a café Baltaci",
                Url = "http://www.menicka.cz/api/iframe/?id=5696&bg=vhite&color=black&size=18&datum=dnes"
            },
            new()
            {
                Name = "Prostě restaurace",
                Url = "http://www.menicka.cz/api/iframe/?id=8250&bg=vhite&color=black&size=18&datum=dnes"
            }
        };
    }
}