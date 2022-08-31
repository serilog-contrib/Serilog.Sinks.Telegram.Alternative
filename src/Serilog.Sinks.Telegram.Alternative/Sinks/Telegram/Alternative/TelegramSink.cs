// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TelegramSink.cs" company="SeppPenner and the Serilog contributors">
// The project is licensed under the MIT license.
// </copyright>
// <summary>
//   Implements <see cref="IBatchedLogEventSink" /> and provides means needed for sending Serilog log events to Telegram.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Serilog.Sinks.Telegram.Alternative;

/// <summary>
/// Implements <see cref="IBatchedLogEventSink"/> and provides means needed for sending Serilog log events to Telegram.
/// </summary>
public class TelegramSink : IBatchedLogEventSink
{
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
    public TelegramSink(TelegramSinkOptions options)
    {
        this.options = options;

        if (!string.IsNullOrWhiteSpace(options.OutputTemplate))
        {
            this.outputTemplateRenderer = new OutputTemplateRenderer(options.OutputTemplate!, options);
        }
    }

    /// <inheritdoc cref="IBatchedLogEventSink" />
    /// <summary>
    /// Emit a batch of log events, running asynchronously.
    /// </summary>
    /// <param name="events">The events to emit.</param>
    /// <returns></returns>
    /// <exception cref="LoggingFailedException">Received failed result {result.StatusCode} when posting events to Microsoft Teams</exception>
    /// <remarks>
    /// Override either <see cref="M:Serilog.Sinks.PeriodicBatching.IBatchedLogEventSink.EmitBatch(System.Collections.Generic.IEnumerable{Serilog.Events.LogEvent})" /> or <see cref="M:Serilog.Sinks.PeriodicBatching.IBatchedLogEventSink.EmitBatchAsync(System.Collections.Generic.IEnumerable{Serilog.Events.LogEvent})" />,
    /// not both. Overriding EmitBatch() is preferred.
    /// </remarks>
    public async Task EmitBatchAsync(IEnumerable<LogEvent> events)
    {
        var messagesToSend = new List<ExtendedLogEvent>();

        foreach (var logEvent in events)
        {
            logEvent.AddPropertyIfAbsent(new LogEventProperty(TelegramPropertyNames.ApplicationName, new ScalarValue(this.options.ApplicationName)));

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
                                  : this.outputTemplateRenderer is null ? RenderMessage(extendedLogEvent, this.options) : this.outputTemplateRenderer.Format(extendedLogEvent);
                await this.SendMessage(this.options.HttpClient, this.options.BotApiUrl, this.options.BotToken, this.options.ChatId, message);
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
                                  : this.outputTemplateRenderer is null ? RenderMessage(extendedLogEvent, this.options) : this.outputTemplateRenderer.Format(extendedLogEvent);

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
            await this.SendMessage(this.options.HttpClient, this.options.BotApiUrl,this.options.BotToken, this.options.ChatId, messageToSend);
        }
    }

    /// <inheritdoc cref="IBatchedLogEventSink" />
    /// <summary>
    /// Allows sinks to perform periodic work without requiring additional threads or
    /// timers (thus avoiding additional flush/shut-down complexity).   
    /// </summary>
    public Task OnEmptyBatchAsync()
    {
        return Task.CompletedTask;
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
    /// <param name="httpClient">The HTTP client.</param>
    /// <param name="botApiUrl">The Telegram bot API url.</param>
    /// <param name="token">The token.</param>
    /// <param name="chatId">The chat identifier.</param>
    /// <param name="message">The message.</param>
    /// <returns>A <see cref="Task"/> representing any asynchronous operation.</returns>
    private async Task SendMessage(HttpClient httpClient, string? botApiUrl, string token, string chatId, string message)
    {
        this.TryWriteToSelflog($"Trying to send message to chatId '{chatId}': '{message}'.");

        var client = new TelegramClient(httpClient, token, botApiUrl);

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
