using System.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Http;

namespace LagosSmartMeter
{
   public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            services.AddControllers();
            // remove default model validation
            services.Configure<ApiBehaviorOptions>(options => {
                options.SuppressModelStateInvalidFilter = true;
            });

            // add database context
            services.AddDbContext<DatabaseContext>(options => options.UseSqlServer(Configuration.GetConnectionString("sqlserver_online")));

            // add subscriber identity
            services.AddIdentity<SubscriberModel, IdentityRole>()
            .AddEntityFrameworkStores<DatabaseContext>()
            .AddDefaultTokenProviders();

            // configure subsciber identity options
            services.Configure<IdentityOptions>(options => {
                // user requires unique email
                options.User.RequireUniqueEmail = true;
                // TODO :: add password criteria
                options.Password.RequireDigit =false;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
            });

             // Add JWT Authentication 
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                 .AddJwtBearer(options =>
                 {
                    options.TokenValidationParameters = new TokenValidationParameters
                     {
                         ValidateIssuer = true,
                         ValidIssuer = Configuration["jwt:issuer"],
                         ValidateIssuerSigningKey = true,
                         IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Configuration["jwt:signingkey"])),
                         ValidateAudience = true,
                         ValidAudience = Configuration["jwt:audience"],
                         ValidateActor = false,
                         ValidateLifetime = true,
                        };
                 });

            // add device service 
            services.AddTransient<DeviceService>();
            // add SignalR
            services.AddSignalR();
            
            // add Event processor host service as Hosted Service
            services.AddHostedService<EventProcessorService>();
            // add Database cleanup service as hosted service
            services.AddHostedService<DatabaseCleanUpService>();

            // add fire and forget jobs manager
            services.AddHostedService<FireForgetJobsExecutorService>();
            
            // add test data
            services.AddTransient<TemporaryData>();
            services.BuildServiceProvider().GetRequiredService<TemporaryData>().Add();

        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env,IServiceProvider serviceProvider)
        {
           // app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<SubscriberHub>(SubscriberApiRoutes.SubscriberHub);
            });
            app.Run(async (context) => {
               await context.Response.WriteAsync("Lagos Smart Meter");
            });

        }
    }
}
