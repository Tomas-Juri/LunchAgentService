using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using log4net;
using log4net.Config;
using LunchAgentService.Entities;
using LunchAgentService.Services;
using LunchAgentService.Services.DatabaseService;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace LunchAgentService
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());

            XmlConfigurator.Configure(logRepository, new FileInfo("log4net.config"));

            var logger = LogManager.GetLogger(typeof(Startup));
            var databaseService = new DatabaseService(Configuration["connectionString"], Configuration["databaseName"], logger);

            LoadConfigFileIntoDatabase(Configuration, databaseService);

            services.AddSingleton(m => logger);
            services.AddSingleton<IDatabaseService>(m => databaseService);
            services.AddSingleton<ISlackService>(m => new SlackService(databaseService, logger));
            services.AddSingleton<IRestaurantService>(m => new RestaurantService(databaseService, logger));
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
