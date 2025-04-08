using System.Text.Json;
using System.Text.Json.Serialization;

namespace Todo.SharedKernel.Events;

public class LogEvent : DomainEvent
{
    [JsonPropertyName("level")] public required string Level { get; set; }

    [JsonPropertyName("message")] public string Message { get; set; } = null!;

    [JsonPropertyName("exceptionMessage")] public string? ExceptionMessage { get; set; }

    [JsonPropertyName("exceptionType")] public string? ExceptionType { get; set; }

    [JsonPropertyName("stackTrace")] public string? StackTrace { get; set; }

    [JsonPropertyName("source")] public string Source { get; set; } = null!;

    [JsonPropertyName("metadata")] public Dictionary<string, string>? Metadata { get; set; }

    [JsonPropertyName("traceId")] public string? TraceId { get; set; }

    [JsonPropertyName("spanId")] public string? SpanId { get; set; }

    [JsonPropertyName("correlationId")] public string? CorrelationId { get; set; }

    public string ToJson()
    {
        return JsonSerializer.Serialize(this);
    }
}