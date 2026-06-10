using KosovoPay.Dtos;
using KosovoPay.Http;
using KosovoPay.Params;

namespace KosovoPay.Resources;

/// <summary>Manages webhook endpoint registrations.</summary>
public sealed class WebhookEndpointsResource : ResourceBase
{
    internal WebhookEndpointsResource(KosovoPayHttpClient http) : base(http) { }

    /// <summary>Registers a new webhook endpoint.</summary>
    public async Task<WebhookEndpoint> CreateAsync(
        CreateWebhookEndpointParams @params,
        CancellationToken cancellationToken = default) =>
        await PostAsync("/webhook-endpoints", @params.ToBody(),
                        KosovoPayJsonContext.Default.WebhookEndpoint,
                        idempotencyKey: null, cancellationToken).ConfigureAwait(false);

    /// <summary>Returns all registered webhook endpoints.</summary>
    public async Task<ListEnvelope<WebhookEndpoint>> ListAsync(CancellationToken cancellationToken = default) =>
        await GetAsync("/webhook-endpoints", null,
                       KosovoPayJsonContext.Default.ListEnvelopeWebhookEndpoint, cancellationToken)
              .ConfigureAwait(false);

    /// <summary>Deletes a webhook endpoint.</summary>
    public async Task<DeletedResource> DeleteAsync(string id, CancellationToken cancellationToken = default) =>
        await DeleteAsync($"/webhook-endpoints/{Uri.EscapeDataString(id)}",
                          KosovoPayJsonContext.Default.DeletedResource, cancellationToken).ConfigureAwait(false);

    /// <summary>Rotates the signing secret for a webhook endpoint. The new secret is returned once.</summary>
    public async Task<WebhookEndpoint> RotateSecretAsync(string id, CancellationToken cancellationToken = default) =>
        await PostAsync($"/webhook-endpoints/{Uri.EscapeDataString(id)}/rotate-secret",
                        body: null,
                        KosovoPayJsonContext.Default.WebhookEndpoint,
                        idempotencyKey: null, cancellationToken).ConfigureAwait(false);
}
