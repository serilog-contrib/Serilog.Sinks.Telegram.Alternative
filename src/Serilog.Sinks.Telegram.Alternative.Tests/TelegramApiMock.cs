// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TelegramApiMock.cs" company="SeppPenner and the Serilog contributors">
// The project is licensed under the MIT license.
// </copyright>
// <summary>
//   A mocked Telegram bot API that records every request instead of sending it.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Serilog.Sinks.Telegram.Alternative.Tests;

/// <summary>
/// A mocked Telegram bot API that records every request instead of sending it. It answers every url, so a
/// request that went to the wrong address shows up as a recorded request with an unexpected url instead of
/// disappearing in the exception handling of the batching sink.
/// </summary>
internal sealed class TelegramApiMock : IDisposable
{
    /// <summary>
    /// The recorded requests.
    /// </summary>
    private readonly List<RecordedRequest> requests = new();

    /// <summary>
    /// The mocked message handler.
    /// </summary>
    private readonly MockHttpMessageHandler handler = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="TelegramApiMock"/> class.
    /// </summary>
    internal TelegramApiMock()
    {
        this.handler.When("*").Respond(async request =>
        {
            var payload = request.Content is null ? string.Empty : await request.Content.ReadAsStringAsync();

            lock (this.requests)
            {
                this.requests.Add(new RecordedRequest(request.RequestUri, payload));
            }

            return new HttpResponseMessage(HttpStatusCode.OK);
        });

        this.Client = this.handler.ToHttpClient();
    }

    /// <summary>
    /// Gets the HTTP client that talks to this mock.
    /// </summary>
    internal HttpClient Client { get; }

    /// <summary>
    /// Gets the number of requests that reached the mock.
    /// </summary>
    internal int RequestCount
    {
        get
        {
            lock (this.requests)
            {
                return this.requests.Count;
            }
        }
    }

    /// <summary>
    /// Gets the request that reached the mock last.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown if no request reached the mock at all.</exception>
    internal RecordedRequest LastRequest
    {
        get
        {
            lock (this.requests)
            {
                return this.requests.Count == 0
                    ? throw new InvalidOperationException("No request reached the mocked Telegram bot API.")
                    : this.requests[^1];
            }
        }
    }

    /// <inheritdoc cref="IDisposable"/>
    public void Dispose()
    {
        this.Client.Dispose();
        this.handler.Dispose();
    }
}
