using System.Text.Json;
using System.Text.Json.Serialization;

namespace Todo.SharedKernel.Events;

public abstract class DomainEvent
{
    public Guid EventId { get; init; } = Guid.NewGuid();
    public DateTime OccurredOn { get; init; } = DateTime.UtcNow;

    public int RetryCount { get; private set; }

    public Dictionary<string, string> MetaData { get; } = new();

    public void IncrementRetryCount() => RetryCount++;
    public void ResetRetryCount() => RetryCount = 0;

    public void AddMetaData(string key, string value)
    {
        if (string.IsNullOrWhiteSpace(key))
            throw new ArgumentException("Key cannot be null or whitespace.", nameof(key));

        if (MetaData.ContainsKey(key))
            throw new InvalidOperationException($"Key '{key}' already exists in MetaData.");

        MetaData[key] = value;
    }

    public bool TryAddMetaData(string key, string value)
    {
        if (string.IsNullOrWhiteSpace(key) || MetaData.ContainsKey(key))
            return false;

        MetaData[key] = value;
        return true;
    }

    public void SetMetaData(string key, string value)
    {
        if (string.IsNullOrWhiteSpace(key))
            throw new ArgumentException("Key cannot be null or whitespace.", nameof(key));

        MetaData[key] = value;
    }

    public string? GetMetaData(string key)
    {
        return MetaData.TryGetValue(key, out var value) ? value : null;
    }

    public string ToJson(bool indented = false)
    {
        var options = new JsonSerializerOptions
        {
            WriteIndented = indented,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        };

        return JsonSerializer.Serialize(this, GetType(), options);
    }
}