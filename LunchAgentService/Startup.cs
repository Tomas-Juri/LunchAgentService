using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LunchAgentService.Entities;
using LunchAgentService.Helpers;
using LunchAgentService.Services;
using LunchAgentService.Services.DatabaseService;
using LunchAgentService.Services.MachineLearningService;
using LunchAgentService.Services.UserService;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
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
            services.AddCors();
            services.AddMvc();

            var databaseService = new DatabaseService(Configuration["connectionString"], Configuration["databaseName"], Logger);

            LoadConfigFileIntoDatabase(Configuration, databaseService);
            
            services.AddSingleton<IDatabaseService>(m => databaseService);
            services.AddSingleton<ISlackService, SlackService>();
            services.AddSingleton<IRestaurantService, RestaurantService>();
            services.AddSingleton<IMachineLearningService, MachineLearningService>();
            services.AddSingleton<IHostedService, SchedulerService>();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });
            });

            // configure strongly typed settings objects
            var appSettingsSection = Configuration.GetSection("AppSettings");
            services.Configure<AppSettings>(appSettingsSection);

            // configure jwt authentication
            var appSettings = appSettingsSection.Get<AppSettings>();
            var key = Encoding.ASCII.GetBytes(appSettings.Secret);
            services.AddAuthentication(x =>
                {
                    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(x =>
                {
                    x.Events = new JwtBearerEvents
                    {
                        OnTokenValidated = context =>
                        {
                            var userService = context.HttpContext.RequestServices.GetRequiredService<IUserService>();
                            var userId = context.Principal.Identity.Name;
                            var user = userService.GetById(userId);
                            if (user == null)
                            {
                                // return unauthorized if user no longer exists
                                context.Fail("Unauthorized");
                            }
                            return Task.CompletedTask;
                        }
                    };
                    x.RequireHttpsMetadata = false;
                    x.SaveToken = true;
                    x.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(key),
                        ValidateIssuer = false,
                        ValidateAudience = false
                    };
                });
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
            app.UseCors(x => x
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader()
                .Build());
            app.UseAuthentication();
            app.UseStaticFiles();
            app.UseMvc();
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
            });

            if (env.IsDevelopment())
            {
                app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }
        }
    }
}
