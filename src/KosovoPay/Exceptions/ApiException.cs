namespace KosovoPay.Exceptions;

/// <summary>A generic server-side error not covered by a more specific type.</summary>
public sealed class ApiException : KosovoPayException
{
    /// <inheritdoc cref="KosovoPayException"/>
    public ApiException(
        string message,
        string? errorCode = null,
        string? errorType = null,
        string? param = null,
        string? requestId = null,
        string? docUrl = null,
        int statusCode = 0)
        : base(message, errorCode, errorType, param, requestId, docUrl, statusCode) { }
}
