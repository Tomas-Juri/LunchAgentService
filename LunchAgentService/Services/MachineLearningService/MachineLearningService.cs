using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using log4net;
using LunchAgentService.Entities;
using LunchAgentService.Services.DatabaseService;

namespace LunchAgentService.Services.MachineLearningService
{
    public class MachineLearningService : IMachineLearningService
    {
        private ILog Log { get; }
        private IDatabaseService DatabaseService { get; }

        public MachineLearningService(ILog log, IDatabaseService databaseService)
        {
            DatabaseService = databaseService;
            Log = log;
        }

        public void ProcessSlackLunchReactions(IEnumerable<Reaction> reactions, List<RestaurantMenu> menus)
        {
            var matches = MatchReactionsToMenus(reactions, menus)
                .Select(x => new ReactionMongo
                {
                    Name = x.Value.Restaurant.Name,
                    Emoji = x.Value.Restaurant.Emoji,
                    Url = x.Value.Restaurant.Url,
                    User = x.Key,
                    Date = DateTime.Today,
                    Items = x.Value.Items
                })
                .ToArray();

            DatabaseService.AddOrUpdate(matches);

            //TODO: Add ML training and processing 
        }

        private IDictionary<string, RestaurantMenu> MatchReactionsToMenus(IEnumerable<Reaction> reactions, List<RestaurantMenu> menus)
        {
            var users = reactions.SelectMany(x => x.Users, (x, user) => new { User = user, Emoji = x.Name });

            var matchQuery =
                from u in users
                join m in menus on u.Emoji equals m.Restaurant.Emoji into matches
                from m in matches.DefaultIfEmpty()
                select
                    new
                    {
                        User = u,
                        Menu = m ?? new RestaurantMenu()
                        {
                            Items = new List<RestaurantMenuItem>(),
                            Restaurant = new RestaurantMongo() { Emoji = u.Emoji }
                        }
                    };

            return matchQuery.ToDictionary(key => key.User.User, value => value.Menu);
        }
    }
}