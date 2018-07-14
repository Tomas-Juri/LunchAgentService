using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LunchAgentService.Helpers;
using LunchAgentService.Helpers.Entities;
using LunchAgentService.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

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
            services.AddSingleton(m => new SlackHelper(Configuration.GetSection("DefaultSlackConfiguration").Get<SlackSetting>()));
            services.AddSingleton(m => new RestaurantHelper(Configuration.GetSection("DefaultRestaurants").Get<RestaurantSettings[]>()));

            services.AddSingleton<IHostedService, SchedulerService>();

            services.AddMvc();
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
            app.UseMvc();
        }
    }
}
