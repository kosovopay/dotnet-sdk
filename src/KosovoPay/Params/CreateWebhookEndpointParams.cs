using KosovoPay.Enums;

namespace KosovoPay.Params;

/// <summary>Parameters for registering a webhook endpoint.</summary>
public sealed class CreateWebhookEndpointParams
{
    /// <summary>
    /// Creates webhook endpoint params.
    /// </summary>
    /// <param name="url">The HTTPS URL to receive events (http/https).</param>
    /// <param name="enabledEvents">At least one event type must be specified.</param>
    /// <param name="description">Optional description.</param>
    public CreateWebhookEndpointParams(
        string url,
        IReadOnlyList<WebhookEventType> enabledEvents,
        string? description = null)
    {
        if (enabledEvents is null || enabledEvents.Count == 0)
            throw new ArgumentException("enabledEvents must contain at least one event type.", nameof(enabledEvents));

        if (!Uri.TryCreate(url, UriKind.Absolute, out var uri) ||
            (uri.Scheme != "http" && uri.Scheme != "https"))
        {
            throw new ArgumentException("url must be an http or https URL.", nameof(url));
        }

        Url = url;
        EnabledEvents = enabledEvents;
        Description = description;
    }

    /// <summary>The endpoint URL.</summary>
    public string Url { get; }
    /// <summary>Event types to subscribe to.</summary>
    public IReadOnlyList<WebhookEventType> EnabledEvents { get; }
    /// <summary>Optional description.</summary>
    public string? Description { get; }

    internal Dictionary<string, object?> ToBody()
    {
        var body = new Dictionary<string, object?>
        {
            ["url"] = Url,
            ["enabled_events"] = EnabledEvents
                .Select(e => EnumWireValues.WebhookEventTypeValues.TryGetValue(e, out var v) ? v : e.ToString())
                .ToArray(),
        };
        if (Description is not null)
            body["description"] = Description;
        return body;
    }
}
