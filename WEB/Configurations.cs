using Application.Interfaces;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Application.Logger;
using Application.Repositories;
using Application.Services;
using Core.Context;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Web
{
    public static class Configurations
    {
        public static IServiceCollection ConfigureDatabase(this IServiceCollection services, IConfiguration configuration)
        {
            return services.AddDbContext<PlanningDbContext>(options =>
               options.UseSqlServer(configuration.GetConnectionString("PlanningDatabase")));
        }

        public static IServiceCollection AddLocalServices(this IServiceCollection services)
        {
            services.AddScoped<ILoggerManager, LoggerManager>();
            services.AddScoped<IPlanRepository, PlanRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IHashService, SHA256Service>();

            return services;
        }

        public static IServiceCollection ConfigureAuthentication(this IServiceCollection services)
        {
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                   .AddCookie(options =>
                   {
                       options.SlidingExpiration = true;
                       options.ReturnUrlParameter = "returnUrl";
                       options.ExpireTimeSpan = TimeSpan.FromMinutes(20);
                       options.LoginPath = "/Auth/Login";
                       options.Cookie.IsEssential = true;
                       options.AccessDeniedPath = "/Auth/AccessDenied";

                   });

            return services;
        }


    }
}
