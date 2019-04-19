using Application.Interfaces;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Application.Logger;
using Application.Repositories;
using Application.Services;
using Core.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

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
    }
}
