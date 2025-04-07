using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Todo.User.Application.Abstraction;
using Todo.User.Application.Services;
using Todo.User.Infrastructure;

namespace Todo.User.Application;

public static class ConfigureApplication
{
    public static void ConfigureModules(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = GetConnectionString(configuration);
        services.AddInfrastructure(connectionString);
        services.AddApplication();
    }

    private static void AddApplication(this IServiceCollection services)
    {
        services.ConfigureServices();
    }

    private static void ConfigureServices(this IServiceCollection services)
    {
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<ILoginHistoryService, LoginHistoryService>();
        services.AddScoped<IRefreshTokenService, RefreshTokenService>();
        services.AddScoped<IUserService, UserService>();
    }


    private static string GetConnectionString(IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        if (string.IsNullOrEmpty(connectionString))
            throw new ArgumentException("Connection string is missing");

        return connectionString;
    }
}