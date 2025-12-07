using ArcAI.Application.Common.Interfaces;
using ArcAI.Domain.Interfaces;
using ArcAI.Infrastructure.Persistence;
using ArcAI.Infrastructure.Repositories;
using ArcAI.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace ArcAI.Infrastructure;

/// <summary>
/// Extension methods for registering Infrastructure layer services.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Adds Infrastructure layer services to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The configuration.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Database context
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        if (string.IsNullOrEmpty(connectionString))
        {
            // Use in-memory database for development/testing
            services.AddDbContext<AppDbContext>(options =>
                options.UseInMemoryDatabase("ArcAI_InMemory"));
        }
        else
        {
            // Use SQL Server for production
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(connectionString, sqlOptions =>
                {
                    sqlOptions.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName);
                    sqlOptions.EnableRetryOnFailure(
                        maxRetryCount: 3,
                        maxRetryDelay: TimeSpan.FromSeconds(30),
                        errorNumbersToAdd: null);
                }));
        }

        // Unit of Work
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Repositories
        services.AddScoped<IProductRepository, ProductRepository>();

        // Services
        services.AddScoped<IDateTimeService, DateTimeService>();
        services.AddScoped<ICurrentUserService, CurrentUserService>();

        // HTTP Context Accessor (required for CurrentUserService)
        services.AddHttpContextAccessor();

        return services;
    }
}