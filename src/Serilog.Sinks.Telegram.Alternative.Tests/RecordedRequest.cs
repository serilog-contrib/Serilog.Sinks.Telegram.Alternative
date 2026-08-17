// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RecordedRequest.cs" company="SeppPenner and the Serilog contributors">
// The project is licensed under the MIT license.
// </copyright>
// <summary>
//   A request that the mocked Telegram bot API received.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Serilog.Sinks.Telegram.Alternative.Tests;

/// <summary>
/// A request that the mocked Telegram bot API received.
/// </summary>
internal sealed class RecordedRequest
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RecordedRequest"/> class.
    /// </summary>
    /// <param name="url">The url the request went to.</param>
    /// <param name="payload">The JSON payload of the request.</param>
    internal RecordedRequest(Uri? url, string payload)
    {
        this.Url = url;
        this.Payload = payload;
    }

    /// <summary>
    /// Gets the url the request went to.
    /// </summary>
    internal Uri? Url { get; }

    /// <summary>
    /// Gets the JSON payload of the request.
    /// </summary>
    internal string Payload { get; }

    /// <summary>
    /// Gets the message text out of the payload. Reading the decoded value keeps the tests free of the JSON
    /// escaping that would otherwise hide behind every quotation mark and every line break.
    /// </summary>
    internal string Text => this.GetValue("text") ?? string.Empty;

    /// <summary>
    /// Gets the value of the given payload property, or null if the payload holds a JSON null for it.
    /// </summary>
    /// <param name="propertyName">The name of the payload property.</param>
    /// <returns>The value as <see cref="string"/> or null.</returns>
    internal string? GetValue(string propertyName)
    {
        using var document = JsonDocument.Parse(this.Payload);
        var property = document.RootElement.GetProperty(propertyName);
        return property.ValueKind == JsonValueKind.Null ? null : property.ToString();
    }
}
