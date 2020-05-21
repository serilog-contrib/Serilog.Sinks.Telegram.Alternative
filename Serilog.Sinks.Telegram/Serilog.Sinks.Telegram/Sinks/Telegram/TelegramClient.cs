// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TelegramClient.cs" company="Haemmer Electronics">
// The project is licensed under the MIT license.
// </copyright>
// <summary>
//   The client to post to Telegram.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

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
        /// The Telegram bot API URL.
        /// </summary>
        private const string TelegramBotApiUrl = "https://api.telegram.org/bot";

        /// <summary>
        /// The API URL.
        /// </summary>
        private readonly Uri apiUrl;

        /// <summary>
        /// The HTTP client.
        /// </summary>
        private readonly HttpClient httpClient = new HttpClient();

        /// <summary>
        /// Initializes a new instance of the <see cref="TelegramClient"/> class.
        /// </summary>
        /// <param name="botToken">The Telegram bot token.</param>
        /// <param name="timeoutSeconds">The timeout seconds.</param>
        /// <exception cref="ArgumentException">Thrown if the bot token is null or empty.</exception>
        public TelegramClient(string botToken, int timeoutSeconds = 10)
        {
            if (string.IsNullOrWhiteSpace(botToken))
            {
                throw new ArgumentException("The bot token mustn't be empty.", nameof(botToken));
            }

            this.apiUrl = new Uri($"{TelegramBotApiUrl}{botToken}/sendMessage");
            this.httpClient.Timeout = TimeSpan.FromSeconds(timeoutSeconds);
        }

        /// <summary>
        /// Posts the message asynchronous.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="chatId">The chat identifier.</param>
        /// <returns>A <see cref="HttpResponseMessage"/>.</returns>
        public async Task<HttpResponseMessage> PostMessageAsync(string message, string chatId)
        {
            var payload = new { chat_id = chatId, text = message, parse_mode = "markdown" };
            var json = JsonConvert.SerializeObject(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await this.httpClient.PostAsync(this.apiUrl, content);
            return response;
        }
    }
}