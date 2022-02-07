// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TelegramSink.cs" company="SeppPenner and the Serilog contributors">
// The project is licensed under the MIT license.
// </copyright>
// <summary>
//   Implements <see cref="PeriodicBatchingSink" /> and provides means needed for sending Serilog log events to Telegram.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Serilog.Sinks.Telegram.Alternative
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;

    using Debugging;
    using Events;
    using PeriodicBatching;

    using Serilog.Sinks.Telegram.Output;

    /// <summary>
    /// Implements <see cref="PeriodicBatchingSink"/> and provides means needed for sending Serilog log events to Telegram.
    /// </summary>
    public class TelegramSink : PeriodicBatchingSink
    {
        /// <summary>
        /// The client.
        /// </summary>
        private static readonly HttpClient Client = new();

        /// <summary>
        /// The options.
        /// </summary>
        private readonly TelegramSinkOptions options;

        /// <summary>
        /// The output template renderer.
        /// </summary>
        private readonly OutputTemplateRenderer? outputTemplateRenderer;

        /// <summary>
        /// Initializes a new instance of the <see cref="TelegramSink"/> class.
        /// </summary>
        /// <param name="options">Telegram options object.</param>
        public TelegramSink(TelegramSinkOptions options) : base(options.BatchSizeLimit, options.Period)
        {
            this.options = options;

            if (!string.IsNullOrWhiteSpace(options.OutputTemplate))
            {
                outputTemplateRenderer = new OutputTemplateRenderer(options.OutputTemplate!, options);
            }
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
                logEvent.AddPropertyIfAbsent(new LogEventProperty(TelegramPropertyNames.ApplicationName, new ScalarValue(options.ApplicationName)));

                if (logEvent.Level < this.options.MinimumLogEventLevel)
                {
                    continue;
                }

                var foundSameLogEvent = messagesToSend.FirstOrDefault(l => l.LogEvent.Exception.Message == logEvent.Exception.Message);

                if (foundSameLogEvent is null)
                {
                    messagesToSend.Add(new ExtendedLogEvent(logEvent.Timestamp.DateTime, logEvent.Timestamp.DateTime, logEvent, this.options.IncludeStackTrace));
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
                foreach (var extendedLogEvent in messagesToSend)
                {
                    var message = this.options.FormatProvider != null
                                      ? extendedLogEvent.LogEvent.RenderMessage(this.options.FormatProvider)
                                      : outputTemplateRenderer is null ? RenderMessage(extendedLogEvent, options) : outputTemplateRenderer.Format(extendedLogEvent);
                    await this.SendMessage(this.options.BotApiUrl, this.options.BotToken, this.options.ChatId, message);
                }
            }
            else
            {
                var sb = new StringBuilder();
                var count = 0;

                foreach (var extendedLogEvent in messagesToSend)
                {
                    var message = this.options.FormatProvider != null
                                      ? extendedLogEvent.LogEvent.RenderMessage(this.options.FormatProvider)
                                      : outputTemplateRenderer is null ? RenderMessage(extendedLogEvent, options) : outputTemplateRenderer.Format(extendedLogEvent);

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
                await this.SendMessage(this.options.BotApiUrl, this.options.BotToken, this.options.ChatId, messageToSend);
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
        /// <param name="options">The options.</param>
        /// <returns>The rendered message.</returns>
        private static string RenderMessage(ExtendedLogEvent extLogEvent, TelegramSinkOptions options)
        {
            var sb = new StringBuilder();
            var emoji = LogLevelRenderer.GetEmoji(extLogEvent.LogEvent);
            var renderedMessage = HtmlEscaper.Escape(options, extLogEvent.LogEvent.RenderMessage());

            sb.AppendLine($"{emoji} {renderedMessage}");
            sb.AppendLine(string.Empty);

            if (!string.IsNullOrWhiteSpace(options.ApplicationName) ||
                !string.IsNullOrWhiteSpace(options.DateFormat))
            {
                string applicationNamePart = string.IsNullOrWhiteSpace(options.ApplicationName)
                    ? string.Empty
                    : $"{HtmlEscaper.Escape(options, options.ApplicationName)}: ";

                string datePart = string.IsNullOrWhiteSpace(options.DateFormat)
                    ? string.Empty
                    : extLogEvent.FirstOccurrence != extLogEvent.LastOccurrence
                        ? $"The message occurred first on {extLogEvent.FirstOccurrence.ToString(options.DateFormat)} and last on {extLogEvent.LastOccurrence.ToString(options.DateFormat)}"
                        : $"The message occurred on {extLogEvent.FirstOccurrence.ToString(options.DateFormat)}";

                sb.AppendLine($"<i>{applicationNamePart}{datePart}</i>");
            }

            if (extLogEvent.LogEvent.Exception is null)
            {
                return sb.ToString();
            }

            var message = HtmlEscaper.Escape(options, extLogEvent.LogEvent.Exception.Message);
            var exceptionType = HtmlEscaper.Escape(options, extLogEvent.LogEvent.Exception.GetType().Name);

            sb.AppendLine($"\n<strong>{message}</strong>\n");
            sb.AppendLine($"Message: <code>{message}</code>");
            sb.AppendLine($"Type: <code>{exceptionType}</code>\n");

            if (extLogEvent.IncludeStackTrace)
            {
                var exception = HtmlEscaper.Escape(options, $"{extLogEvent.LogEvent.Exception}");
                sb.AppendLine($"Stack Trace\n<code>{exception}</code>");
            }

            return sb.ToString();
        }

        /// <summary>
        /// Sends the message.
        /// </summary>
        /// <param name="botApiUrl">The Telegram bot API url</param>
        /// <param name="token">The token.</param>
        /// <param name="chatId">The chat identifier.</param>
        /// <param name="message">The message.</param>
        /// <returns>A <see cref="Task"/> representing any asynchronous operation.</returns>
        private async Task SendMessage(string? botApiUrl, string token, string chatId, string message)
        {
            this.TryWriteToSelflog($"Trying to send message to chatId '{chatId}': '{message}'.");
            
            var client = new TelegramClient(token, 5, botApiUrl);

            try
            {
                var result = await client.PostMessage(message, chatId);

                if (result != null)
                {
                    this.TryWriteToSelflog($"Message sent to chatId '{chatId}': '{result.StatusCode}'.");
                }
            }
            catch (Exception ex)
            {
                SelfLog.WriteLine($"{ex.Message} {ex.StackTrace}");
                this.options.FailureCallback?.Invoke(ex);
            }
        }

        /// <summary>
        /// Tries to write to Selflog and throws an exception if a formatting error occurred.
        /// </summary>
        /// <param name="messageTemplate">The message template.</param>
        private void TryWriteToSelflog(string messageTemplate)
        {
            try
            {
                SelfLog.WriteLine(messageTemplate);
            }
            catch (Exception ex)
            {
                SelfLog.WriteLine($"{ex.Message} {ex.StackTrace}");
                this.options.FailureCallback?.Invoke(ex);
            }
        }
    }
}