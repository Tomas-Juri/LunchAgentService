using System.IO;
using log4net;
using Newtonsoft.Json;

namespace LunchAgentService.Services
{
    public class SettingService
    {
        private ILog Log { get; }
        private string Path { get; }

        private string SlackFile => Path + "\\SlackSettings.json";
        private string RestaurantFile => Path + "\\RestaurantSettings.json";

        public SettingService(string path, SlackServiceSetting slackSetting, RestaurantServiceSetting restaurantSetting, ILog log)
        {
            Path = path;
            Log = log;

            Directory.CreateDirectory(Path);

            if (File.Exists(SlackFile) == false)
                SaveSlackSetting(slackSetting);

            if (File.Exists(RestaurantFile) == false)
                SaveRestaurantSettting(restaurantSetting);
        }

        public RestaurantServiceSetting GetRestaurantSetting()
        {
            Log.Debug("Getting Resturant Setting");

            return JsonConvert.DeserializeObject<RestaurantServiceSetting>(File.ReadAllText(RestaurantFile));
        }

        public SlackServiceSetting GetSlackSetting()
        {
            Log.Debug("Getting Slack Setting");

            return JsonConvert.DeserializeObject<SlackServiceSetting>(File.ReadAllText(SlackFile));
        }

        public void SaveRestaurantSettting(RestaurantServiceSetting setting)
        {
            Log.Debug("Saving Resturant Setting");

            File.WriteAllText(RestaurantFile, JsonConvert.SerializeObject(setting));
        }

        public void SaveSlackSetting(SlackServiceSetting setting)
        {
            Log.Debug("Saving Slack Setting");

            File.WriteAllText(SlackFile, JsonConvert.SerializeObject(setting));
        }
    }
}