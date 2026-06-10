namespace KosovoPay.Exceptions;

/// <summary>
/// Base class for all exceptions thrown by the KosovoPay SDK. Carries the full
/// server error envelope so callers can branch on a stable machine code, surface
/// the doc URL, and quote the request ID in support tickets.
/// </summary>
public abstract class KosovoPayException : Exception
{
    /// <summary>Machine-readable error code from the server, e.g. <c>amount_below_minimum</c>.</summary>
    public string? ErrorCode { get; }

    /// <summary>Error type family from the server, e.g. <c>payment_error</c>.</summary>
    public string? ErrorType { get; }

    /// <summary>The field/parameter that triggered the error, if applicable.</summary>
    public string? Param { get; }

    /// <summary>The request ID — quote this in support tickets.</summary>
    public string? RequestId { get; }

    /// <summary>A URL to the documentation for this error.</summary>
    public string? DocUrl { get; }

    /// <summary>HTTP status code of the response.</summary>
    public int StatusCode { get; }

    /// <summary>Initialises the exception with all server envelope fields.</summary>
    protected KosovoPayException(
        string message,
        string? errorCode = null,
        string? errorType = null,
        string? param = null,
        string? requestId = null,
        string? docUrl = null,
        int statusCode = 0)
        : base(message)
    {
        ErrorCode = errorCode;
        ErrorType = errorType;
        Param = param;
        RequestId = requestId;
        DocUrl = docUrl;
        StatusCode = statusCode;
    }
}
