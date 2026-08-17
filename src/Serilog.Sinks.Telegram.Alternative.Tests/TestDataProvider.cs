// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TestDataProvider.cs" company="SeppPenner and the Serilog contributors">
// The project is licensed under the MIT license.
// </copyright>
// <summary>
//   A class that provides the values the offline tests share.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Serilog.Sinks.Telegram.Alternative.Tests;

/// <summary>
/// A class that provides the values the offline tests share.
/// </summary>
internal static class TestDataProvider
{
    /// <summary>
    /// The bot token used by the offline tests. It contains no colon on purpose, that keeps the resulting
    /// url easy to compare.
    /// </summary>
    internal const string BotToken = "testtoken";

    /// <summary>
    /// The chat id used by the offline tests.
    /// </summary>
    internal const string ChatId = "12345";

    /// <summary>
    /// The date format the convenience extension method uses as its default.
    /// </summary>
    internal const string DateFormat = "dd.MM.yyyy HH:mm:sszzz";

    /// <summary>
    /// The default Telegram bot API url.
    /// </summary>
    internal const string DefaultBotApiUrl = "https://api.telegram.org/bot";

    /// <summary>
    /// The url the client posts to when no custom bot API url is configured.
    /// </summary>
    internal const string DefaultSendMessageUrl = $"{DefaultBotApiUrl}{BotToken}/sendMessage";

    /// <summary>
    /// Creates sink options that post to the given mocked API. The date format and the application name are
    /// empty, so the rendered message holds nothing but the emoji and the message itself.
    /// </summary>
    /// <param name="api">The mocked Telegram bot API.</param>
    /// <param name="dateFormat">The date format.</param>
    /// <param name="applicationName">The application name.</param>
    /// <param name="minimumLogEventLevel">The minimum log event level the sink itself applies.</param>
    /// <param name="sendBatchesAsSingleMessages">A value indicating whether the batches are sent as single messages.</param>
    /// <param name="useCustomHtmlFormatting">A value indicating whether custom HTML formatting is allowed.</param>
    /// <param name="outputTemplate">The output template.</param>
    /// <param name="customHtmlFormatter">The custom HTML formatter.</param>
    /// <param name="topicId">The Telegram topic id.</param>
    /// <returns>The <see cref="TelegramSinkOptions"/> for the test.</returns>
    internal static TelegramSinkOptions CreateOptions(
        TelegramApiMock api,
        string dateFormat = "",
        string applicationName = "",
        LogEventLevel minimumLogEventLevel = LogEventLevel.Verbose,
        bool sendBatchesAsSingleMessages = true,
        bool useCustomHtmlFormatting = false,
        string? outputTemplate = null,
        Func<string, string>? customHtmlFormatter = null,
        int? topicId = null)
    {
        return new TelegramSinkOptions(
            BotToken,
            ChatId,
            dateFormat,
            applicationName,
            batchSizeLimit: 10,
            // A long period makes sure that the only flush of a test is the one the disposal of the logger triggers.
            period: TimeSpan.FromMinutes(5),
            minimumLogEventLevel: minimumLogEventLevel,
            sendBatchesAsSingleMessages: sendBatchesAsSingleMessages,
            useCustomHtmlFormatting: useCustomHtmlFormatting,
            outputTemplate: outputTemplate,
            customHtmlFormatter: customHtmlFormatter,
            topicId: topicId)
        {
            HttpClient = api.Client
        };
    }

    /// <summary>
    /// Creates a log event for the tests that call <see cref="TelegramSink.EmitBatchAsync"/> directly. Whether
    /// several events end up in the same batch is up to the timing of the batching sink, so everything that is
    /// about batch behaviour hands the batch over itself instead of logging and hoping.
    /// </summary>
    /// <param name="messageTemplate">The message template.</param>
    /// <param name="level">The log event level.</param>
    /// <param name="exception">The exception.</param>
    /// <param name="secondsFromNow">The offset of the timestamp against the current time.</param>
    /// <returns>The <see cref="LogEvent"/> for the test.</returns>
    internal static LogEvent CreateLogEvent(
        string messageTemplate,
        LogEventLevel level = LogEventLevel.Information,
        Exception? exception = null,
        int secondsFromNow = 0)
    {
        return new LogEvent(
            DateTimeOffset.Now.AddSeconds(secondsFromNow),
            level,
            exception,
            new MessageTemplateParser().Parse(messageTemplate),
            []);
    }
}
