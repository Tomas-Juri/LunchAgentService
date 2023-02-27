namespace LunchAgent.Core.Restaurants.Entitties;

public record Restaurant
{
    public string Name { get; init; } = string.Empty;

    public string Url { get; init; } = string.Empty;

    public string Emoji { get; init; } = string.Empty;
}