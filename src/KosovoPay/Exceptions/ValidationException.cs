namespace KosovoPay.Exceptions;

/// <summary>The request body is invalid — check <see cref="KosovoPayException.Param"/> for the offending field.</summary>
public sealed class ValidationException : KosovoPayException
{
    /// <inheritdoc cref="KosovoPayException"/>
    public ValidationException(
        string message,
        string? errorCode = null,
        string? errorType = null,
        string? param = null,
        string? requestId = null,
        string? docUrl = null,
        int statusCode = 422)
        : base(message, errorCode, errorType, param, requestId, docUrl, statusCode) { }
}
