using System.Text.Json;
using FluentAssertions;
using Flurl.Http.Testing;
using KosovoPay;
using KosovoPay.Dtos;
using Xunit;

namespace KosovoPay.Tests;

/// <summary>
/// Verifies that cursor pagination auto-pages and yields typed items.
/// </summary>
public sealed class PaginationTests
{
    [Fact]
    public async Task ListAsync_Streams_Across_Two_Pages()
    {
        using var http = new HttpTest();

        // Page 1 — has_more: true
        http.RespondWith(TestHelpers.ListEnvelope(
            new[] { TestHelpers.PaymentRow("pi_1"), TestHelpers.PaymentRow("pi_2") },
            hasMore: true), 200);

        // Page 2 — has_more: false
        http.RespondWith(TestHelpers.ListEnvelope(
            new[] { TestHelpers.PaymentRow("pi_3") },
            hasMore: false), 200);

        var client = new KosovoPayClient(TestHelpers.DefaultOptions);
        var ids = new List<string>();

        await foreach (var payment in client.Payments.ListAsync())
        {
            payment.Should().NotBeNull();
            ids.Add(payment.Id);
        }

        ids.Should().BeEquivalentTo(new[] { "pi_1", "pi_2", "pi_3" }, opts => opts.WithStrictOrdering());
    }

    [Fact]
    public async Task ListAsync_Empty_First_Page_Yields_Nothing()
    {
        using var http = new HttpTest();

        http.RespondWith(TestHelpers.ListEnvelope(Enumerable.Empty<object>(), hasMore: false), 200);

        var client = new KosovoPayClient(TestHelpers.DefaultOptions);
        var count = 0;

        await foreach (var _ in client.Payments.ListAsync())
            count++;

        count.Should().Be(0);
    }

    [Fact]
    public async Task ListAsync_Second_Page_Uses_StartingAfter_Cursor()
    {
        using var http = new HttpTest();

        http.RespondWith(TestHelpers.ListEnvelope(
            new[] { TestHelpers.PaymentRow("pi_1"), TestHelpers.PaymentRow("pi_2") },
            hasMore: true), 200);

        http.RespondWith(TestHelpers.ListEnvelope(
            new[] { TestHelpers.PaymentRow("pi_3") },
            hasMore: false), 200);

        var client = new KosovoPayClient(TestHelpers.DefaultOptions);
        await client.Payments.ListAsync().ToListAsync();

        // The second call should include starting_after=pi_2 in the query string
        http.ShouldHaveCalled("*payments*")
            .WithQueryParamValue("starting_after", "pi_2")
            .Times(1);
    }
}

// Async enumerable extension to collect results
internal static class AsyncEnumerableExtensions
{
    public static async Task<List<T>> ToListAsync<T>(this IAsyncEnumerable<T> source)
    {
        var list = new List<T>();
        await foreach (var item in source)
            list.Add(item);
        return list;
    }
}
