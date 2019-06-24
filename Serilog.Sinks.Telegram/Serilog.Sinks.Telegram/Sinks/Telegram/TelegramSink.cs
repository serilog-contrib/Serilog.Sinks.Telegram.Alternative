using Serilog.Debugging;
using Serilog.Events;
using Serilog.Sinks.PeriodicBatching;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Serilog.Sinks.Telegram
{
    using System;
    using System.Text;

    /// <summary>
    /// Implements <see cref="PeriodicBatchingSink"/> and provides means needed for sending Serilog log events to Telegram.
    /// </summary>
    public class TelegramSink : PeriodicBatchingSink
    {
        /// <summary>
        /// The client.
        /// </summary>
        private static readonly HttpClient Client = new HttpClient();

        /// <summary>
        /// The options.
        /// </summary>
        private readonly TelegramSinkOptions _options;

        /// <summary>
        /// Initializes new instance of <see cref="TelegramSink"/>.
        /// </summary>
        /// <param name="options">Telegram options object.</param>
        public TelegramSink(TelegramSinkOptions options)
                : base(options.BatchSizeLimit, options.Period)
        {
            _options = options;
        }

        /// <inheritdoc cref="PeriodicBatchingSink" />
        /// <summary>
        /// Emit a batch of log events, running asynchronously.
        /// </summary>
        /// <param name="events">The events to emit.</param>
        /// <returns></returns>
        /// <exception cref="LoggingFailedException">Received failed result {result.StatusCode} when posting events to Telegram</exception>
        /// <remarks>
        /// Override either <see cref="M:Serilog.Sinks.PeriodicBatching.PeriodicBatchingSink.EmitBatch(System.Collections.Generic.IEnumerable{Serilog.Events.LogEvent})" /> or <see cref="M:Serilog.Sinks.PeriodicBatching.PeriodicBatchingSink.EmitBatchAsync(System.Collections.Generic.IEnumerable{Serilog.Events.LogEvent})" />,
        /// not both. Overriding EmitBatch() is preferred.
        /// </remarks>
        protected override async Task EmitBatchAsync(IEnumerable<LogEvent> events)
        {
            var messagesToSend = new List<ExtendedLogEvent>();

            foreach (var logEvent in events)
            {
                if (logEvent.Level < _options.MinimumLogEventLevel)
                {
                    continue;
                }

                var foundSameLogEvent = messagesToSend.FirstOrDefault(l => l.LogEvent.Exception.Message == logEvent.Exception.Message);

                if (foundSameLogEvent == null)
                {
                    messagesToSend.Add(
                        new ExtendedLogEvent
                            {
                                LogEvent = logEvent,
                                FirstOccurance = logEvent.Timestamp,
                                LastOccurance = logEvent.Timestamp
                            });
                }
                else
                {
                    if (foundSameLogEvent.FirstOccurance > logEvent.Timestamp)
                    {
                        foundSameLogEvent.FirstOccurance = logEvent.Timestamp;
                    }
                    else if (foundSameLogEvent.LastOccurance < logEvent.Timestamp)
                    {
                        foundSameLogEvent.LastOccurance = logEvent.Timestamp;
                    }
                }
            }

            foreach (var extendedLogEvent in messagesToSend)
            {
                var message = this._options.FormatProvider != null
                                  ? new TelegramMessage(extendedLogEvent.LogEvent.RenderMessage(formatProvider: this._options.FormatProvider))
                                  : RenderMessage(extendedLogEvent);
                await this.SendMessage(this._options.BotToken, this._options.ChatId, message);
            }
        }

        /// <summary>
        /// Free resources held by the sink.
        /// </summary>
        /// <param name="disposing">If true, called because the object is being disposed; if false,
        /// the object is being disposed from the finalizer.</param>
        /// <inheritdoc cref="PeriodicBatchingSink"/>
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            Client.Dispose();
        }

        /// <summary>
        /// Renders the message.
        /// </summary>
        /// <param name="extLogEvent">The log event.</param>
        /// <returns>The rendered message.</returns>
        protected static TelegramMessage RenderMessage(ExtendedLogEvent extLogEvent)
        {
            var sb = new StringBuilder();
            sb.AppendLine(value: $"{GetEmoji(log: extLogEvent.LogEvent)} {extLogEvent.LogEvent.RenderMessage()}");

            if (extLogEvent.FirstOccurance != extLogEvent.LastOccurance)
            {
                sb.AppendLine(value: $"The message occured first on {extLogEvent.FirstOccurance:dd.MM.yyyy HH:mm:sszzz} and last on {extLogEvent.LastOccurance:dd.MM.yyyy HH:mm:sszzz}");
            }
            else
            {
                sb.AppendLine(value: $"The message occured on {extLogEvent.FirstOccurance:dd.MM.yyyy HH:mm:sszzz}");
            }

            if (extLogEvent.LogEvent.Exception != null)
            {
                sb.AppendLine(value: $"\n*{extLogEvent.LogEvent.Exception.Message}*\n");
                sb.AppendLine(value: $"Message: `{extLogEvent.LogEvent.Exception.Message}`");
                sb.AppendLine(value: $"Type: `{extLogEvent.LogEvent.Exception.GetType().Name}`\n");
                sb.AppendLine(value: $"Stack Trace\n```{extLogEvent.LogEvent.Exception}```");
            }
            return new TelegramMessage(text: sb.ToString());
        }

        /// <summary>
        /// Gets the emoji.
        /// </summary>
        /// <param name="log">The log.</param>
        /// <returns>The emoji as <see cref="string"/>.</returns>
        private static string GetEmoji(LogEvent log)
        {
            switch (log.Level)
            {
                case LogEventLevel.Debug:
                    return "👉";
                case LogEventLevel.Error:
                    return "❗";
                case LogEventLevel.Fatal:
                    return "‼";
                case LogEventLevel.Information:
                    return "ℹ";
                case LogEventLevel.Verbose:
                    return "⚡";
                case LogEventLevel.Warning:
                    return "⚠";
                default:
                    return "";
            }
        }

        /// <summary>
        /// Sends the message.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <param name="chatId">The chat identifier.</param>
        /// <param name="message">The message.</param>
        protected async Task SendMessage(string token, string chatId, TelegramMessage message)
        {
            SelfLog.WriteLine($"Trying to send message to chatId '{chatId}': '{message}'.");
            var client = new TelegramClient(botToken: token, timeoutSeconds: 5);
            var result = await client.PostMessageAsync(message, chatId);

            if (result != null)
            {
                SelfLog.WriteLine($"Message sent to chatId '{chatId}': '{result.StatusCode}'.");
            }
        }
    }
}