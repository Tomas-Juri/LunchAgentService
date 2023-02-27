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
                Name = "Makalu",
                Url = "https://nepalska-restaurace-makalu.cz/zlin/weekly",
                Emoji = "\uD83C\uDDEE\uD83C\uDDF3"
            },
            new()
            {
                Name = "Prostě restaurace",
                Url = "http://www.menicka.cz/api/iframe/?id=8250&bg=vhite&color=black&size=18&datum=dnes",
                Emoji = "\uD83D\uDED7"
            },
            new()
            {
                Name = "Puor",
                Url = "http://www.menicka.cz/api/iframe/?id=2992&bg=vhite&color=black&size=18&datum=dnes",
                Emoji = "\uD83C\uDF7A"
            },
            new()
            {
                Name = "Flip",
                Url = "http://www.menicka.cz/api/iframe/?id=24&bg=vhite&color=black&size=18&datum=dnes",
                Emoji = "\u2B50"
            },
            new()
            {
                Name = "Bistrotéka Valachy",
                Url = "http://www.menicka.cz/api/iframe/?id=6114&bg=vhite&color=black&size=18&datum=dnes",
                Emoji = "\uD83C\uDF3F"
            },
            new()
            {
                Name = "Bistro a café Baltaci",
                Url = "http://www.menicka.cz/api/iframe/?id=5696&bg=vhite&color=black&size=18&datum=dnes",
                Emoji = "\uD83D\uDFE5"
            },
            new()
            {
                Name = "Canada",
                Url = "http://www.menicka.cz/api/iframe/?id=6&bg=vhite&color=black&size=18&datum=dnes",
                Emoji = "\uD83C\uDDE8\uD83C\uDDE6"
            },
            new()
            {
                Name = "Celnice",
                Url = "http://www.menicka.cz/api/iframe/?id=17&bg=vhite&color=black&size=18&datum=dnes",
                Emoji = "\uD83D\uDC10"
            },
            new()
            {
                Name = "Budvarka",
                Url = "http://www.menicka.cz/api/iframe/?id=5881&bg=vhite&color=black&size=18&datum=dnes",
                Emoji = "\uD83C\uDF77"
            }
        };
    }
}