// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TelegramSinkOptions.cs" company="SeppPenner and the Serilog contributors">
// The project is licensed under the MIT license.
// </copyright>
// <summary>
//   Container for all Telegram sink configurations.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Serilog.Sinks.Telegram.Alternative;

/// <summary>
/// Container for all Telegram sink configurations.
/// </summary>
public class TelegramSinkOptions
{
    /// <summary>
    /// The default batch size limit.
    /// </summary>
    private const int DefaultBatchSizeLimit = 1;

    /// <summary>
    /// The default period.
    /// </summary>
    private static readonly TimeSpan DefaultPeriod = TimeSpan.FromSeconds(1);

    /// <summary>
    /// Initializes a new instance of the <see cref="TelegramSinkOptions"/> class.
    /// </summary>
    /// <param name="botToken">The Telegram bot token.</param>
    /// <param name="chatId">The Telegram chat id.</param>
    /// <param name="dateFormat">The date time format showing how the date and time should be formatted.</param>
    /// <param name="applicationName">The name of the application sending the events in case multiple apps write to the same channel.</param>
    /// <param name="topicId">The Telegram topic id.</param>
    /// <param name="batchSizeLimit">The maximum number of events to post in a single batch; defaults to 1 if
    /// not provided i.e. no batching by default.</param>
    /// <param name="period">The time to wait between checking for event batches; defaults to 1 sec if not
    /// provided.</param>
    /// <param name="formatProvider">The format provider used for formatting the message.</param>
    /// <param name="minimumLogEventLevel">The minimum log event level to use.</param>
    /// <param name="sendBatchesAsSingleMessages">A value indicating whether the batches are sent as single messages or as one block of messages.</param>
    /// <param name="includeStackTrace">A value indicating whether the stack trace should be shown or not.</param>
    /// <param name="failureCallback">The failure callback.</param>
    /// <param name="useCustomHtmlFormatting">A value indicating whether custom HTML formatting in the messages could be used. (Use this carefully and only if really needed).</param>
    /// <param name="botApiUrl">The Telegram bot API url, defaults to https://api.telegram.org/bot.</param>
    /// <param name="outputTemplate">The output template.</param>
    /// <param name="customHtmlFormatter">
    ///    You can pass a func in addition to <see cref="UseCustomHtmlFormatting"/> to set your custom function for escaping HTML strings.
    ///    This will only be considered if <see cref="UseCustomHtmlFormatting"/> is set to true.
    /// </param>
    public TelegramSinkOptions(
        string botToken,
        string chatId,
        string dateFormat,
        string applicationName,
        int? batchSizeLimit = null,
        TimeSpan? period = null,
        IFormatProvider? formatProvider = null,
        LogEventLevel minimumLogEventLevel = LogEventLevel.Verbose,
        bool? sendBatchesAsSingleMessages = true,
        bool? includeStackTrace = true,
        Action<Exception>? failureCallback = null,
        bool useCustomHtmlFormatting = false,
        string? botApiUrl = null,
        string? outputTemplate = null,
        Func<string, string>? customHtmlFormatter = null,
        int? topicId = null)
    {
        if (string.IsNullOrWhiteSpace(botToken))
        {
            throw new ArgumentException("The bot token is invalid.", nameof(botToken));
        }

        this.BotToken = botToken;
        this.ChatId = chatId;
        this.TopicId = topicId;
        this.BatchSizeLimit = batchSizeLimit ?? DefaultBatchSizeLimit;
        this.Period = period ?? DefaultPeriod;
        this.FormatProvider = formatProvider;
        this.MinimumLogEventLevel = minimumLogEventLevel;
        this.SendBatchesAsSingleMessages = sendBatchesAsSingleMessages ?? true;
        this.IncludeStackTrace = includeStackTrace ?? true;
        this.FailureCallback = failureCallback;
        this.DateFormat = dateFormat;
        this.ApplicationName = applicationName;
        this.UseCustomHtmlFormatting = useCustomHtmlFormatting;

        if (useCustomHtmlFormatting)
        {
            this.CustomHtmlFormatter = customHtmlFormatter;
        }

        this.BotApiUrl = botApiUrl;
        this.OutputTemplate = outputTemplate;
    }

    /// <summary>
    /// Gets the Telegram bot API Url
    /// </summary>
    public string? BotApiUrl { get; }

    /// <summary>
    /// Gets the Telegram bot token.
    /// </summary>
    public string BotToken { get; }

    /// <summary>
    /// Gets the Application name.
    /// </summary>
    public string ApplicationName { get; }

    /// <summary>
    /// Gets the Telegram date format.
    /// </summary>
    public string DateFormat { get; }

    /// <summary>
    /// Gets the Telegram chat id.
    /// </summary>
    public string ChatId { get; }

    /// <summary>
    /// Gets the maximum number of events to post in a single batch.
    /// </summary>
    public int BatchSizeLimit { get; }

    /// <summary>
    /// Gets the time to wait between checking for event batches.
    /// </summary>
    public TimeSpan Period { get; }

    /// <summary>
    /// Gets the format provider used for formatting the message.
    /// </summary>
    public IFormatProvider? FormatProvider { get; }

    /// <summary>
    /// Gets the minimum log event level.
    /// </summary>
    public LogEventLevel MinimumLogEventLevel { get; }

    /// <summary>
    /// Gets a value indicating whether the batches are sent as single messages or as one block of messages.
    /// </summary>
    public bool SendBatchesAsSingleMessages { get; }

    /// <summary>
    /// Gets a value indicating whether the stack trace should be shown or not.
    /// </summary>
    public bool IncludeStackTrace { get; }

    /// <summary>
    /// Gets the failure callback.
    /// </summary>
    public Action<Exception>? FailureCallback { get; }

    /// <summary>
    /// Gets a value indicating whether custom HTML formatting in the messages could be used. (Use this carefully and only if really needed).
    /// </summary>
    public bool UseCustomHtmlFormatting { get; }

    /// <summary>
    /// Gets a value whether string literals should be escaped or not.
    /// </summary>
    public bool ShouldEscape => !this.UseCustomHtmlFormatting;

    /// <summary>
    /// Gets or sets the output template.
    /// </summary>
    public string? OutputTemplate { get; set; }

    /// <summary>
    /// Gets the custom html formatting method. You need to also set <see cref="UseCustomHtmlFormatting"/>.
    /// </summary>
    public Func<string, string>? CustomHtmlFormatter { get; }

    /// <summary>
    /// Gets or sets the HTTP client used for requests to Telegram.
    /// </summary>
    public HttpClient HttpClient { get; set; } = new HttpClient();

    /// <summary>
    /// Gets the Telegram topic id for selected chat.
    /// </summary>
    public int? TopicId { get; set; }
}
