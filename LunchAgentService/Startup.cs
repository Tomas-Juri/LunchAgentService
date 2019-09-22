using System.Linq;
using LunchAgentService.Entities;
using LunchAgentService.Services;
using LunchAgentService.Services.DatabaseService;
using LunchAgentService.Services.MachineLearningService;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;

namespace LunchAgentService
{
    public class Startup
    {
        public Startup(ILogger<Startup> logger, IConfiguration configuration)
        {
            Logger = logger;
            Configuration = configuration;
        }

        public ILogger Logger { get; }
        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var databaseService = new DatabaseService(Configuration["connectionString"], Configuration["databaseName"], Logger);

            LoadConfigFileIntoDatabase(Configuration, databaseService);
            
            services.AddSingleton<IDatabaseService>(m => databaseService);
            services.AddSingleton<ISlackService, SlackService>();
            services.AddSingleton<IRestaurantService, RestaurantService>();
            services.AddSingleton<IMachineLearningService, MachineLearningService>();
            services.AddSingleton<IHostedService, SchedulerService>();

            services.AddCors();

            services.AddMvc();
        }

        private void LoadConfigFileIntoDatabase(IConfiguration configuration, DatabaseService databaseService)
        {
            var slackSetting = databaseService.Get<SlackSettingMongo>();
            var restaurantSetting = databaseService.Get<RestaurantMongo>();

            if (slackSetting.Any() == false)
                databaseService.AddOrUpdate(configuration.GetSection("SlackServiceSetting").Get<SlackSettingMongo>());

            if (restaurantSetting.Any() == false)
                databaseService.AddOrUpdate(configuration.GetSection("RestaurantServiceSettings").Get<RestaurantMongo[]>());
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }

            app.UseStaticFiles();

            app.UseAuthentication();

            app.UseCors(
                options => options.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()
            );

            app.UseMvc();
        }
    }
}
