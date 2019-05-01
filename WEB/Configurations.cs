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
            services.AddScoped<IDictionaryRepository, DictionaryRepository>();
            services.AddScoped<IHashService, SHA256Service>();
            services.AddScoped<IEmailService, EmailService>();

            return services;
        }

        public static IServiceCollection ConfigureAuthentication(this IServiceCollection services)
        {
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                   .AddCookie(options =>
                   {      
                       options.ExpireTimeSpan = TimeSpan.FromMinutes(20);
                       options.SlidingExpiration = true;
                       options.LoginPath = "/Auth/Login";
                       options.ReturnUrlParameter = "returnUrl";
                       options.AccessDeniedPath = "/Auth/AccessDenied";
                       options.Cookie.IsEssential = true;
                   });

            return services;
        }


    }
}
