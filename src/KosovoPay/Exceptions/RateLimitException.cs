namespace KosovoPay.Exceptions;

/// <summary>
/// Too many requests — the client should back off and retry.
/// Check <see cref="RetryAfter"/> for the server-suggested delay in seconds.
/// </summary>
public sealed class RateLimitException : KosovoPayException
{
    /// <summary>Seconds to wait before retrying, if the server provided a <c>Retry-After</c> header.</summary>
    public int? RetryAfter { get; }

    /// <inheritdoc cref="KosovoPayException"/>
    public RateLimitException(
        string message,
        int? retryAfter = null,
        string? errorCode = null,
        string? errorType = null,
        string? param = null,
        string? requestId = null,
        string? docUrl = null,
        int statusCode = 429)
        : base(message, errorCode, errorType, param, requestId, docUrl, statusCode)
    {
        RetryAfter = retryAfter;
    }
}
