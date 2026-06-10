using System.Text.Json;
using Flurl.Http;
using KosovoPay.Exceptions;
using Polly;
using Polly.Retry;

namespace KosovoPay.Http;

/// <summary>
/// Shared HTTP context: base URL, bearer auth, version + User-Agent headers,
/// timeout, retry policy (network + 429 + safe 5xx), and typed-error conversion.
/// </summary>
internal sealed class KosovoPayHttpClient : IDisposable
{
    internal const string SdkVersion = "1.0.0";

    private readonly IFlurlClient _flurl;
    private readonly ResiliencePipeline _pipeline;
    private bool _disposed;

    internal KosovoPayHttpClient(KosovoPayOptions options)
    {
        if (string.IsNullOrWhiteSpace(options.ApiKey))
            throw new ArgumentException("An API key is required.", nameof(options));

        _flurl = new FlurlClient(options.BaseUrl.TrimEnd('/'))
            .WithTimeout(options.RequestTimeoutSeconds)
            .WithHeader("Authorization", $"Bearer {options.ApiKey}")
            .WithHeader("Kosovopay-Version", options.ApiVersion)
            .WithHeader("User-Agent", $"kosovopay-dotnet/{SdkVersion} (dotnet/{Environment.Version})")
            .WithHeader("Accept", "application/json");

        _pipeline = BuildPipeline(options.MaxRetries);
    }

    /// <summary>
    /// Creates a <see cref="KosovoPayHttpClient"/> using an externally-provided <see cref="IFlurlClient"/>.
    /// Used in tests to inject Flurl's <c>HttpTest</c> mock.
    /// </summary>
    internal KosovoPayHttpClient(IFlurlClient flurlClient, KosovoPayOptions options)
    {
        _flurl = flurlClient;
        _pipeline = BuildPipeline(options.MaxRetries);
    }

    /// <summary>Builds the Polly resilience pipeline for retry on transient failures.</summary>
    private static ResiliencePipeline BuildPipeline(int maxRetries)
    {
        // maxRetries counts total attempts; Polly's MaxRetryAttempts counts only the retries after
        // the first attempt. When maxRetries <= 1 there is nothing to retry, so return an empty pipeline.
        var retryCount = maxRetries - 1;
        if (retryCount <= 0)
            return ResiliencePipeline.Empty;

        return new ResiliencePipelineBuilder()
            .AddRetry(new RetryStrategyOptions
            {
                MaxRetryAttempts = retryCount,
                BackoffType = DelayBackoffType.Exponential,
                Delay = TimeSpan.FromMilliseconds(500),
                UseJitter = true,
                ShouldHandle = new PredicateBuilder()
                    .Handle<FlurlHttpException>(ex =>
                    {
                        // Retry network errors
                        if (ex.StatusCode is null) return true;
                        // Retry 429
                        if (ex.StatusCode == 429) return true;
                        // Retry 5xx
                        if (ex.StatusCode >= 500) return true;
                        return false;
                    })
                    .Handle<HttpRequestException>()
                    .Handle<TaskCanceledException>(ex => ex.InnerException is TimeoutException),
            })
            .Build();
    }

    /// <summary>Returns a request builder rooted at <paramref name="path"/>.</summary>
    internal IFlurlRequest Request(string path) => _flurl.Request(path);

    /// <summary>
    /// Executes <paramref name="factory"/> through the resilience pipeline.
    /// On error Flurl throws <see cref="FlurlHttpException"/>, which is caught here
    /// and converted to a typed <see cref="KosovoPayException"/>.
    /// </summary>
    internal async Task<IFlurlResponse> SendAsync(
        Func<CancellationToken, Task<IFlurlResponse>> factory,
        CancellationToken cancellationToken)
    {
        try
        {
            return await _pipeline.ExecuteAsync(
                async ct => await factory(ct).ConfigureAwait(false),
                cancellationToken).ConfigureAwait(false);
        }
        catch (KosovoPayException)
        {
            throw;
        }
        catch (FlurlHttpException ex)
        {
            throw await MapFlurlExceptionAsync(ex).ConfigureAwait(false);
        }
        catch (HttpRequestException ex)
        {
            throw new ApiException($"Network error: {ex.Message}");
        }
        catch (TaskCanceledException ex) when (ex.InnerException is TimeoutException)
        {
            throw new ApiException("The request timed out.");
        }
    }

    private static async Task<KosovoPayException> MapFlurlExceptionAsync(FlurlHttpException ex)
    {
        int status = ex.StatusCode ?? 0;
        int? retryAfter = null;
        string? message = null, code = null, type = null, param = null, requestId = null, docUrl = null;

        if (ex.Call.Response is not null)
        {
            var retryAfterHeader = ex.Call.Response.Headers.FirstOrDefault("Retry-After");
            if (int.TryParse(retryAfterHeader, out var ra))
                retryAfter = ra;

            try
            {
                var body = await ex.GetResponseStringAsync().ConfigureAwait(false);
                if (!string.IsNullOrEmpty(body))
                {
                    var envelope = JsonSerializer.Deserialize(body, KosovoPayJsonContext.Default.ErrorEnvelope);
                    if (envelope?.Error is { } err)
                    {
                        message = err.Message;
                        code = err.Code;
                        type = err.Type;
                        param = err.Param;
                        requestId = err.RequestId;
                        docUrl = err.DocUrl;
                    }
                }
            }
            catch
            {
                // Ignore JSON parse errors — ErrorMapper produces a safe ApiException
            }
        }

        return ErrorMapper.Make(message, code, type, param, requestId, docUrl, status, retryAfter);
    }

    /// <inheritdoc />
    public void Dispose()
    {
        if (!_disposed)
        {
            _flurl.Dispose();
            _disposed = true;
        }
    }
}
