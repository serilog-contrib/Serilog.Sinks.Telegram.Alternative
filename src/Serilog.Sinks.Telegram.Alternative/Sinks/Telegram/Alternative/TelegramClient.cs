// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TelegramClient.cs" company="SeppPenner and the Serilog contributors">
// The project is licensed under the MIT license.
// </copyright>
// <summary>
//   The client to post to Telegram.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Serilog.Sinks.Telegram.Alternative;

/// <summary>
/// The client to post to Telegram.
/// </summary>
public class TelegramClient
{
    /// <summary>
    /// The default value for the Telegram bot API url.
    /// </summary>
    private const string DefaultTelegramBotApiUrl = "https://api.telegram.org/bot";

    /// <summary>
    /// The API URL.
    /// </summary>
    private readonly Uri apiUrl;

    /// <summary>
    /// The HTTP client.
    /// </summary>
    private readonly HttpClient httpClient;

    /// <summary>
    /// Initializes a new instance of the <see cref="TelegramClient"/> class.
    /// </summary>
    /// <param name="httpClient">The HTTP client.</param>
    /// <param name="botToken">The Telegram bot token.</param>
    /// <param name="botApiUrl">The Telegram bot API url.</param>
    /// <exception cref="ArgumentException">Thrown if the bot token is null or empty.</exception>
    public TelegramClient(HttpClient httpClient, string botToken, string? botApiUrl = DefaultTelegramBotApiUrl)
    {
        if (string.IsNullOrWhiteSpace(botToken))
        {
            throw new ArgumentException("The bot token mustn't be empty.", nameof(botToken));
        }

        if (string.IsNullOrWhiteSpace(botApiUrl))
        {
            botApiUrl = DefaultTelegramBotApiUrl;
        }
        
        this.httpClient = httpClient ?? throw new ArgumentNullException("The http client mustn't be null.");
        this.apiUrl = new Uri($"{botApiUrl}{botToken}/sendMessage");
    }

    /// <summary>
    /// Posts the message.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="chatId">The chat identifier.</param>
    /// <param name="topicId">The topic identifier in chats with topic feature.</param>
    /// <returns>A <see cref="HttpResponseMessage"/>.</returns>
    public async Task<HttpResponseMessage> PostMessage(string message, string chatId, int? topicId = null)
    {
        var payload = new { chat_id = chatId, text = message, parse_mode = "HTML", message_thread_id = topicId };
        var json = JsonConvert.SerializeObject(payload);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var response = await this.httpClient.PostAsync(this.apiUrl, content);
        return response;
    }
}
