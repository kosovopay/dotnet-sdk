namespace KosovoPay;

/// <summary>
/// Configuration options for <see cref="KosovoPayClient"/>. All values except
/// <see cref="ApiKey"/> have sensible defaults.
/// </summary>
public sealed class KosovoPayOptions
{
    /// <summary>Default KosovoPay API base URL.</summary>
    public const string DefaultBaseUrl = "https://api.kosovo.sh";

    /// <summary>Default API version header value.</summary>
    public const string DefaultApiVersion = "2026-06-01";

    /// <summary>
    /// Your secret API key (<c>sk_live_…</c> or <c>sk_test_…</c>).
    /// </summary>
    public string ApiKey { get; set; } = string.Empty;

    /// <summary>Override the API base URL. Defaults to <see cref="DefaultBaseUrl"/>.</summary>
    public string BaseUrl { get; set; } = DefaultBaseUrl;

    /// <summary>API version sent in the <c>Kosovopay-Version</c> header.</summary>
    public string ApiVersion { get; set; } = DefaultApiVersion;

    /// <summary>Per-request timeout in seconds. Defaults to 30 s.</summary>
    public int RequestTimeoutSeconds { get; set; } = 30;

    /// <summary>Maximum retry attempts (network errors, 429, 5xx). Defaults to 3.</summary>
    public int MaxRetries { get; set; } = 3;
}
