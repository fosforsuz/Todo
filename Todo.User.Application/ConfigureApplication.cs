using FluentValidation;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Todo.Shared.Contracts.Config;
using Todo.SharedKernel.Messaging;
using Todo.User.Application.Abstraction;
using Todo.User.Application.Behaviour;
using Todo.User.Application.Services;
using Todo.User.Infrastructure;
using Todo.User.Infrastructure.Abstraction;
using Todo.User.Infrastructure.Messaging;

namespace Todo.User.Application;

public static class ConfigureApplication
{
    public static void ConfigureModules(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = GetConnectionString(configuration);

        services.AddInfrastructure(connectionString);
        services.AddApplication(configuration);
    }

    private static void AddApplication(this IServiceCollection services, IConfiguration configuration)
    {
        // Bind RabbitMQ settings (IOptions<RabbitMqConfig>)
        services.Configure<RabbitMqConfig>(configuration.GetSection(nameof(RabbitMqConfig)));

        // Register application-specific services
        services.AddDomainServices();
        services.ConfigureRabbitMqPublishers();

        // Register MediatR handlers
        services.AddMediatR(cfg => { cfg.RegisterServicesFromAssembly(typeof(ConfigureApplication).Assembly); });

        // Register FluentValidation validators
        services.AddValidatorsFromAssembly(typeof(ConfigureApplication).Assembly);

        // Register Validation pipeline for MediatR
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationPipeline<,>));
    }

    private static void AddDomainServices(this IServiceCollection services)
    {
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<ILoginHistoryService, LoginHistoryService>();
        services.AddScoped<IRefreshTokenService, RefreshTokenService>();
        services.AddScoped<IUserService, UserService>();
    }

    private static void ConfigureRabbitMqPublishers(this IServiceCollection services)
    {
        services.AddSingleton<ILogEventPublisher, RabbitMqLogEventPublisher>();
        services.AddSingleton<IRabbitMqEmailPublisher, RabbitMqEmailPublisher>();
    }

    private static string GetConnectionString(IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        if (string.IsNullOrEmpty(connectionString))
            throw new ArgumentException("Connection string is missing");

        return connectionString;
    }
}