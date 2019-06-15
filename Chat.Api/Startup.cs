using System;
using System.Text;
using Chat.Api.Extensions;
using Chat.Api.Helpers;
using Chat.Api.Middleware;
using Chat.Primary.Adapters;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace Chat.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration) => Configuration = configuration;

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var appSettingsSection = Configuration.GetSection("AppSettings");
            services.Configure<AppSettings>(appSettingsSection);

            var appSettings = appSettingsSection.Get<AppSettings>();
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            services.AddAdapters(appSettings.ConnectionString, appSettings.Secret);
            services.AddCors(options =>
                                 options.AddPolicy("MyPolicy", builder =>
                                                               {
                                                                   builder.WithOrigins("http://localhost:4200")
                                                                          .AllowAnyMethod()
                                                                          .AllowCredentials()
                                                                          .AllowAnyHeader();
                                                               }));
            services.AddWebSocketManager();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IServiceProvider serviceProvider)
        {
            app.UseWebSockets();
            if (env.IsDevelopment()) {
                app.UseDeveloperExceptionPage();
            }
            else {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            
            app.UseCors("MyPolicy");

          //  app.UseHttpsRedirection();
          //  app.UseAuthentication();
            app.UseMvc();
            app.MapWebSocketManager("/notification", serviceProvider.GetService<NotificationsMessageHandler>());
        }
    }
}