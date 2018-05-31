﻿namespace Metrics_Track
{
    using AutoMapper;
    using Metrics_Track.Data.Models;
    using Metrics_Track.Infrastructure.Extensions;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using System;

    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;            
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<TrackerDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddIdentity<User, IdentityRole>(options =>  
                     {options.Password.RequireUppercase = false;
                      options.Password.RequireDigit = false;
                      options.Password.RequireNonAlphanumeric = false;
                      options.Password.RequireUppercase = false;
                     })
                .AddEntityFrameworkStores<TrackerDbContext>()
                .AddDefaultTokenProviders();

            services.ConfigureApplicationCookie(options =>
            { 
                options.Cookie.Name = ".MetricsTrack.Identity.Application";
                //options.ExpireTimeSpan = TimeSpan.FromSeconds(10);
                //options.SlidingExpiration = false;
            });

            services.AddAutoMapper();

            services.AddDomainServices();

            services.AddRouting(options => options.LowercaseUrls = true);

            services.AddDistributedMemoryCache();

            services.AddSession(options => {
                options.Cookie.Name = ".MetricsTrack.Session";
                options.IdleTimeout = TimeSpan.FromMinutes(30);
            });

            services.AddMvc();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseDatabaseMigration();

            if (env.IsDevelopment())
            {
                app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseSession();

            app.UseStaticFiles();

            app.UseAuthentication();

            app.UseAntiforgeryTokens();            

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                 name: "dashboard",
                 template: "{id}",
                 defaults: new { controller = "Dashboard", action = "Index" });

                routes.MapRoute(
                  name: "areas",
                  template: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
