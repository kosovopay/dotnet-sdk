namespace KosovoPay.Exceptions;

/// <summary>The authenticated key does not have permission for this action (HTTP 403).</summary>
public sealed class PermissionException : KosovoPayException
{
    /// <inheritdoc cref="KosovoPayException"/>
    public PermissionException(
        string message,
        string? errorCode = null,
        string? errorType = null,
        string? param = null,
        string? requestId = null,
        string? docUrl = null,
        int statusCode = 403)
        : base(message, errorCode, errorType, param, requestId, docUrl, statusCode) { }
}
