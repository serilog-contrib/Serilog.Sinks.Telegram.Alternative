using System;

namespace Serilog.Sinks.Telegram
{
    using Serilog.Events;

    /// <summary>
    /// Container for all Telegram sink configurations.
    /// </summary>
    public class TelegramSinkOptions
    {
        /// <summary>
        /// The default period.
        /// </summary>
        private static readonly TimeSpan DefaultPeriod = TimeSpan.FromSeconds(1);

        /// <summary>
        /// The default batch size limit.
        /// </summary>
        private const int DefaultBatchSizeLimit = 1;

        /// <summary>
        /// Create an instance of the Telegram options container.
        /// </summary>
        /// <param name="botToken">The Telegram bot token.</param>
        /// <param name="chatId">The Telegram chat id.</param>
        /// <param name="batchSizeLimit">The maximum number of events to post in a single batch; defaults to 1 if
        /// not provided i.e. no batching by default.</param>
        /// <param name="period">The time to wait between checking for event batches; defaults to 1 sec if not
        /// provided.</param>
        /// <param name="formatProvider">The format provider used for formatting the message.</param>
        /// <param name="minimumLogEventLevel">The minimum log event level to use.</param>
        public TelegramSinkOptions(string botToken, string chatId, int? batchSizeLimit = null,
            TimeSpan? period = null, IFormatProvider formatProvider = null, LogEventLevel minimumLogEventLevel = LogEventLevel.Verbose)
        {
            if (botToken == null)
            {
                throw new ArgumentNullException(nameof(botToken));
            }

            if (string.IsNullOrEmpty(botToken))
            {
                throw new ArgumentException(nameof(botToken));
            }

            this.BotToken = botToken;
            this.ChatId = chatId;
            this.BatchSizeLimit = batchSizeLimit ?? DefaultBatchSizeLimit;
            this.Period = period ?? DefaultPeriod;
            this.FormatProvider = formatProvider;
            this.MinimumLogEventLevel = minimumLogEventLevel;
        }

        /// <summary>
        /// The Telegram bot token.
        /// </summary>
        public string BotToken { get; }

        /// <summary>
        /// The Telegram chat id.
        /// </summary>
        public string ChatId { get; }

        /// <summary>
        /// The maximum number of events to post in a single batch.
        /// </summary>
        public int BatchSizeLimit { get; }

        /// <summary>
        /// The time to wait between checking for event batches.
        /// </summary>
        public TimeSpan Period { get; }

        /// <summary>
        /// The format provider used for formatting the message.
        /// </summary>
        public IFormatProvider FormatProvider { get; }

        /// <summary>
        /// Gets the minimum log event level.
        /// </summary>
        public LogEventLevel MinimumLogEventLevel { get; }
    }
}