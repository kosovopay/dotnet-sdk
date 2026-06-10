namespace KosovoPay.Exceptions;

/// <summary>The API key is missing, invalid, or revoked (HTTP 401).</summary>
public sealed class AuthenticationException : KosovoPayException
{
    /// <inheritdoc cref="KosovoPayException"/>
    public AuthenticationException(
        string message,
        string? errorCode = null,
        string? errorType = null,
        string? param = null,
        string? requestId = null,
        string? docUrl = null,
        int statusCode = 401)
        : base(message, errorCode, errorType, param, requestId, docUrl, statusCode) { }
}
