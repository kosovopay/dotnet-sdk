using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using Flurl.Http;
using KosovoPay.Dtos;
using KosovoPay.Http;

namespace KosovoPay.Resources;

/// <summary>
/// Base class for resource clients. Provides typed deserialisation helpers on top of
/// <see cref="KosovoPayHttpClient"/>.
/// </summary>
internal abstract class ResourceBase
{
    private static readonly JsonSerializerOptions _serializeOptions = new()
    {
        DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull,
    };

    private protected readonly KosovoPayHttpClient Http;

    protected ResourceBase(KosovoPayHttpClient http) => Http = http;

    /// <summary>Executes a GET and deserialises the response body.</summary>
    private protected async Task<T> GetAsync<T>(
        string path,
        Dictionary<string, object>? query,
        JsonTypeInfo<T> typeInfo,
        CancellationToken ct = default)
    {
        var response = await Http.SendAsync(
            token => Http.Request(path)
                         .SetQueryParams(query ?? new Dictionary<string, object>())
                         .GetAsync(cancellationToken: token, completionOption: HttpCompletionOption.ResponseContentRead),
            ct).ConfigureAwait(false);

        return await DeserialiseAsync(response, typeInfo, ct).ConfigureAwait(false);
    }

    /// <summary>
    /// Executes a POST with an auto-generated (or caller-supplied) Idempotency-Key.
    /// The body is serialised via <see cref="JsonSerializer"/> with Flurl's
    /// <c>PostStringAsync</c> so we control the exact wire format.
    /// </summary>
    private protected async Task<T> PostAsync<T>(
        string path,
        object? body,
        JsonTypeInfo<T> typeInfo,
        string? idempotencyKey = null,
        CancellationToken ct = default)
    {
        var key = string.IsNullOrEmpty(idempotencyKey) ? Guid.NewGuid().ToString() : idempotencyKey;

        // Body dicts produced by ToBody() already contain string wire values for enums.
        // Use WhenWritingNull to omit null properties automatically.
        var jsonBody = body is null
            ? "{}"
            : JsonSerializer.Serialize(body, _serializeOptions);

        var response = await Http.SendAsync(
            token => Http.Request(path)
                         .WithHeader("Idempotency-Key", key)
                         .WithHeader("Content-Type", "application/json")
                         .PostStringAsync(jsonBody, cancellationToken: token,
                                          completionOption: HttpCompletionOption.ResponseContentRead),
            ct).ConfigureAwait(false);

        return await DeserialiseAsync(response, typeInfo, ct).ConfigureAwait(false);
    }

    /// <summary>Executes a DELETE and deserialises the response.</summary>
    private protected async Task<T> DeleteAsync<T>(
        string path,
        JsonTypeInfo<T> typeInfo,
        CancellationToken ct = default)
    {
        var response = await Http.SendAsync(
            token => Http.Request(path)
                         .DeleteAsync(cancellationToken: token, completionOption: HttpCompletionOption.ResponseContentRead),
            ct).ConfigureAwait(false);

        return await DeserialiseAsync(response, typeInfo, ct).ConfigureAwait(false);
    }

    private static async Task<T> DeserialiseAsync<T>(
        IFlurlResponse response,
        JsonTypeInfo<T> typeInfo,
        CancellationToken ct)
    {
        var stream = await response.GetStreamAsync().ConfigureAwait(false);
        var result = await JsonSerializer.DeserializeAsync(stream, typeInfo, ct).ConfigureAwait(false);
        return result!;
    }

    /// <summary>
    /// Streams cursor-paginated results, advancing via <c>starting_after</c> until
    /// <c>has_more</c> is false.
    /// </summary>
    private protected async IAsyncEnumerable<TItem> PaginateAsync<TItem>(
        string path,
        Dictionary<string, object> query,
        JsonTypeInfo<ListEnvelope<TItem>> envelopeTypeInfo,
        Func<TItem, string?> getId,
        [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken ct = default)
        where TItem : class
    {
        bool hasMore = true;
        string? lastId = null;

        while (hasMore)
        {
            ct.ThrowIfCancellationRequested();

            var pageQuery = new Dictionary<string, object>(query);
            if (lastId is not null)
                pageQuery["starting_after"] = lastId;

            var response = await Http.SendAsync(
                token => Http.Request(path)
                             .SetQueryParams(pageQuery)
                             .GetAsync(cancellationToken: token, completionOption: HttpCompletionOption.ResponseContentRead),
                ct).ConfigureAwait(false);

            var stream = await response.GetStreamAsync().ConfigureAwait(false);
            var envelope = await JsonSerializer.DeserializeAsync(stream, envelopeTypeInfo, ct).ConfigureAwait(false)
                ?? new ListEnvelope<TItem>(Array.Empty<TItem>(), false, string.Empty);

            foreach (var item in envelope.Data)
            {
                lastId = getId(item);
                yield return item;
            }

            hasMore = envelope.HasMore;
        }
    }
}
