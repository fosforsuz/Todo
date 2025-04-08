using Elastic.Clients.Elasticsearch;
using Todo.LogService.Service.Abstraction;
using Todo.SharedKernel.Events;

namespace Todo.LogService.Service;

public class LogEventHandler : ILogEventHandler
{
    private readonly ElasticsearchClient _client;
    private readonly ILogger<LogEventHandler> _logger;
    private readonly ILogEventPublisher _publisher;

    public LogEventHandler(ElasticsearchClient elasticsearchClient, ILogger<LogEventHandler> logger,
        ILogEventPublisher publisher)
    {
        _client = elasticsearchClient ?? throw new ArgumentNullException(nameof(elasticsearchClient));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _publisher = publisher ?? throw new ArgumentNullException(nameof(publisher));
    }

    public async Task HandleAsync(LogEvent logEvent, CancellationToken cancellationToken)
    {
        var response = await _client.IndexAsync(logEvent, cancellationToken: cancellationToken);

        if (!response.IsSuccess())
        {
            _logger.LogError("Failed to index log event: {Reason}", response.DebugInformation);
            logEvent.IncrementRetryCount();
            await _publisher.PublishAsync(logEvent, cancellationToken);
        }
        else
        {
            _logger.LogDebug("Log event indexed. ID: {Id}", response.Id);
        }
    }
}