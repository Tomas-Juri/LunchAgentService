using System.Collections.Generic;

namespace LunchAgentService.Services.MachineLearningService
{
    public interface IMachineLearningService
    {
        void ProcessSlackLunchReactions(IEnumerable<Reaction> reactions, List<RestaurantMenu> menus);
    }
}