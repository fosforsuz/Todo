using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Todo.SharedKernel.Abstraction;
using Todo.User.Infrastructure.Abstraction;
using Todo.User.Infrastructure.Data;
using Todo.User.Infrastructure.Persistence;

namespace Todo.User.Infrastructure;

public static class ConfigureInfrastructure
{
    public static void AddInfrastructure(this IServiceCollection services, string connectionString)
    {
        services.ConfigureDbContext(connectionString);
        services.ConfigurePersistence();
        services.ConfigureUnitOfWork();
    }

    private static void ConfigureDbContext(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<UserDbContext>(
            options => options.UseNpgsql(connectionString)
                .EnableSensitiveDataLogging()
                .EnableDetailedErrors()
        );
    }

    private static void ConfigurePersistence(this IServiceCollection services)
    {
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<ILoginHistoryRepository, LoginHistoryRepository>();
    }

    private static void ConfigureUnitOfWork(this IServiceCollection services)
    {
        services.AddScoped<IUnitOfWork, UnitOfWork>();
    }
}