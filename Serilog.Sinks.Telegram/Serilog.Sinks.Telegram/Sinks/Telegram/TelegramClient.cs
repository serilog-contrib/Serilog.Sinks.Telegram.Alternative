namespace Serilog.Sinks.Telegram
{
    using System;
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;
    using Newtonsoft.Json;

    /// <summary>
    /// The client to post to Telegram.
    /// </summary>
    public class TelegramClient
    {
        /// <summary>
        /// The API URL.
        /// </summary>
        private readonly Uri _apiUrl;

        /// <summary>
        /// The HTTP client.
        /// </summary>
        private readonly HttpClient _httpClient = new HttpClient();

        /// <summary>
        /// Initializes a new instance of the <see cref="TelegramClient"/> class.
        /// </summary>
        /// <param name="botToken">The Telegram bot token.</param>
        /// <param name="timeoutSeconds">The timeout seconds.</param>
        /// <exception cref="System.ArgumentException">Thrown if the bot token is null or empty.</exception>
        public TelegramClient(string botToken, int timeoutSeconds = 10)
        {
            if (string.IsNullOrWhiteSpace(botToken))
            {
                throw new ArgumentException("The bot token mustn't be empty.", nameof(botToken));
            }

            _apiUrl = new Uri(uriString: $"https://api.telegram.org/bot{botToken}/sendMessage");
            _httpClient.Timeout = TimeSpan.FromSeconds(value: timeoutSeconds);
        }

        /// <summary>
        /// Posts the <see cref="TelegramMessage"/>asynchronous.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="chatId">The chat identifier.</param>
        /// <returns>A <see cref="HttpResponseMessage"/>.</returns>
        public async Task<HttpResponseMessage> PostMessageAsync(TelegramMessage message, string chatId)
        {
            var payload = new { chat_id = chatId, text = message.Text, parse_mode = "markdown" };
            var json = JsonConvert.SerializeObject(value: payload);
            var response = await _httpClient.PostAsync(
                               requestUri: _apiUrl,
                               content: new StringContent(
                                   content: json,
                                   encoding: Encoding.UTF8,
                                   mediaType: "application/json"));

            return response;
        }
    }
}