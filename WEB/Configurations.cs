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
using Web.Helpers;

namespace Web
{
    /// <summary>
    /// Contains Configuring extension methods for IServiceCollection
    /// </summary>
    public static class Configurations
    {
        /// <summary>
        /// Configures database according to connection string from appsettings.json
        /// </summary>
        /// <param name="services">this object</param>
        /// <param name="configuration">Configuration, that should contain information about connection string</param>
        /// <returns></returns>
        public static IServiceCollection ConfigureDatabase(this IServiceCollection services, IConfiguration configuration)
        {
            return services.AddDbContext<PlanningDbContext>(options =>
               options.UseSqlServer(configuration.GetConnectionString("PlanningDatabase")));
        }

        /// <summary>
        /// Add local services from Application and Web layers
        /// </summary>
        /// <param name="services">This object</param>
        /// <returns></returns>
        public static IServiceCollection AddLocalServices(this IServiceCollection services)
        {
            // Application services
            services.AddScoped<ILoggerManager, LoggerManager>();
            services.AddScoped<IPlanRepository, PlanRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IFileRepository, FileRepository>();
            services.AddScoped<IDictionaryRepository, DictionaryRepository>();
            services.AddScoped<ISettingRepository, SettingRepository>();
            services.AddScoped<IHashService, SHA256Service>();
            services.AddScoped<IEmailService, EmailService>();

            //Web services
            services.AddTransient<HtmlHelper>();

            return services;
        }

        /// <summary>
        /// Configures authentication scheme
        /// </summary>
        /// <param name="services">This object</param>
        /// <returns></returns>
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
                       options.Cookie.HttpOnly = true;
                   });

            return services;
        }


    }
}
