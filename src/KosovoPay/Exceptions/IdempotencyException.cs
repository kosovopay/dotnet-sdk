namespace KosovoPay.Exceptions;

/// <summary>A conflict in an Idempotency-Key was detected by the server.</summary>
public sealed class IdempotencyException : KosovoPayException
{
    /// <inheritdoc cref="KosovoPayException"/>
    public IdempotencyException(
        string message,
        string? errorCode = null,
        string? errorType = null,
        string? param = null,
        string? requestId = null,
        string? docUrl = null,
        int statusCode = 409)
        : base(message, errorCode, errorType, param, requestId, docUrl, statusCode) { }
}
