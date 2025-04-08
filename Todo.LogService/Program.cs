using Serilog;
using Todo.LogService.Messaging;
using Todo.LogService.Service;
using Todo.LogService.Service.Abstraction;
using Todo.Shared.Contracts.Config;
using Elastic.Clients.Elasticsearch;

var builder = Host.CreateApplicationBuilder(args);

// Serilog global config
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .Enrich.FromLogContext()
    .WriteTo.File(
        path: "logs/fallback-log-.txt",
        rollingInterval: RollingInterval.Day,
        retainedFileCountLimit: 7,
        fileSizeLimitBytes: 10_000_000,
        rollOnFileSizeLimit: true,
        shared: true,
        flushToDiskInterval: TimeSpan.FromSeconds(1)
    )
    .CreateLogger();

builder.Logging.ClearProviders();
builder.Logging.AddSerilog(Log.Logger);

// RabbitMQ config binding
builder.Services.Configure<RabbitMqConfig>(builder.Configuration.GetSection(nameof(RabbitMqConfig)));

// Elasticsearch client (Elastic.Clients.Elasticsearch)
builder.Services.AddSingleton(_ =>
{
    var config = builder.Configuration["Elastic:Url"];
    if (string.IsNullOrWhiteSpace(config))
        throw new ArgumentException("Elastic:Url is not configured.");
    var settings = new ElasticsearchClientSettings(new Uri(config))
        .DefaultIndex("log-events");

    return new ElasticsearchClient(settings);
});

// App services
builder.Services.AddSingleton<ILogEventHandler, LogEventHandler>();
builder.Services.AddSingleton<ILogEventPublisher, LogEventPublisher>();
builder.Services.AddSingleton<IFallbackLogWriter, SerilogFallbackLogWriter>();

// Background worker services
builder.Services.AddHostedService<LogEventConsumer>();
builder.Services.AddHostedService<LogEventDlqConsumer>();

var host = builder.Build();
await host.RunAsync(); 