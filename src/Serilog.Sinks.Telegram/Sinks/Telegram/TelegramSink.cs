// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TelegramSink.cs" company="Hämmer Electronics">
// The project is licensed under the MIT license.
// </copyright>
// <summary>
//   Implements <see cref="PeriodicBatchingSink" /> and provides means needed for sending Serilog log events to Telegram.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Serilog.Sinks.Telegram
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;

    using Serilog.Debugging;
    using Serilog.Events;
    using Serilog.Sinks.PeriodicBatching;

    /// <summary>
    /// Implements <see cref="PeriodicBatchingSink"/> and provides means needed for sending Serilog log events to Telegram.
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "Reviewed. Suppression is OK here.")]
    public class TelegramSink : PeriodicBatchingSink
    {
        /// <summary>
        /// The client.
        /// </summary>
        private static readonly HttpClient Client = new HttpClient();

        /// <summary>
        /// The options.
        /// </summary>
        private readonly TelegramSinkOptions options;

        /// <summary>
        /// Initializes a new instance of the <see cref="TelegramSink"/> class.
        /// </summary>
        /// <param name="options">Telegram options object.</param>
        public TelegramSink(TelegramSinkOptions options) : base(options.BatchSizeLimit, options.Period)
        {
            this.options = options;
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
                if (logEvent.Level < this.options.MinimumLogEventLevel)
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
                                FirstOccurrence = logEvent.Timestamp,
                                LastOccurrence = logEvent.Timestamp
                            });
                }
                else
                {
                    if (foundSameLogEvent.FirstOccurrence > logEvent.Timestamp)
                    {
                        foundSameLogEvent.FirstOccurrence = logEvent.Timestamp;
                    }
                    else if (foundSameLogEvent.LastOccurrence < logEvent.Timestamp)
                    {
                        foundSameLogEvent.LastOccurrence = logEvent.Timestamp;
                    }
                }
            }

            if (this.options.SendBatchesAsSingleMessages)
            {
                // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
                foreach (var extendedLogEvent in messagesToSend)
                {
                    var message = this.options.FormatProvider != null
                                      ? extendedLogEvent.LogEvent.RenderMessage(this.options.FormatProvider)
                                      : RenderMessage(extendedLogEvent);
                    await SendMessage(this.options.BotToken, this.options.ChatId, message);
                }
            }
            else
            {
                var sb = new StringBuilder();
                var count = 0;

                // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
                foreach (var extendedLogEvent in messagesToSend)
                {
                    var message = this.options.FormatProvider != null
                                      ? extendedLogEvent.LogEvent.RenderMessage(this.options.FormatProvider)
                                      : RenderMessage(extendedLogEvent);

                    if (count == messagesToSend.Count)
                    {
                        sb.AppendLine(message);
                        sb.AppendLine(Environment.NewLine);
                    }
                    else
                    {
                        sb.AppendLine(message);
                    }

                    count++;
                }

                var messageToSend = sb.ToString();
                await SendMessage(this.options.BotToken, this.options.ChatId, messageToSend);
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
        private static string RenderMessage(ExtendedLogEvent extLogEvent)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"{GetEmoji(extLogEvent.LogEvent)} {extLogEvent.LogEvent.RenderMessage()}");

            sb.AppendLine(
                extLogEvent.FirstOccurrence != extLogEvent.LastOccurrence
                    ? $"The message occured first on {extLogEvent.FirstOccurrence:dd.MM.yyyy HH:mm:sszzz} and last on {extLogEvent.LastOccurrence:dd.MM.yyyy HH:mm:sszzz}"
                    : $"The message occured on {extLogEvent.FirstOccurrence:dd.MM.yyyy HH:mm:sszzz}");

            if (extLogEvent.LogEvent.Exception == null)
            {
                return sb.ToString();
            }

            sb.AppendLine($"\n*{extLogEvent.LogEvent.Exception.Message}*\n");
            sb.AppendLine($"Message: `{extLogEvent.LogEvent.Exception.Message}`");
            sb.AppendLine($"Type: `{extLogEvent.LogEvent.Exception.GetType().Name}`\n");
            sb.AppendLine($"Stack Trace\n```{extLogEvent.LogEvent.Exception}```");

            return sb.ToString();
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
                    return string.Empty;
            }
        }

        /// <summary>
        /// Sends the message.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <param name="chatId">The chat identifier.</param>
        /// <param name="message">The message.</param>
        /// <returns>A <see cref="Task"/> representing any asynchronous operation.</returns>
        private static async Task SendMessage(string token, string chatId, string message)
        {
            SelfLog.WriteLine($"Trying to send message to chatId '{chatId}': '{message}'.");
            var client = new TelegramClient(token, 5);
            var result = await client.PostMessageAsync(message, chatId);

            if (result != null)
            {
                SelfLog.WriteLine($"Message sent to chatId '{chatId}': '{result.StatusCode}'.");
            }
        }
    }
}