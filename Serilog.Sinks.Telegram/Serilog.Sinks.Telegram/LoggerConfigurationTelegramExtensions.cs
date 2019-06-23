using Serilog.Configuration;
using Serilog.Events;
using Serilog.Sinks.Telegram;
using System;

namespace Serilog
{
    /// <summary>
    /// Provides extension methods on <see cref="LoggerSinkConfiguration"/>.
    /// </summary>
    public static class LoggerConfigurationTelegramExtensions
    {
        /// <summary>
        /// <see cref="LoggerSinkConfiguration"/> extension that provides configuration chaining.
        /// <example>
        ///     new LoggerConfiguration()
        ///         .MinimumLevel.Verbose()
        ///         .WriteTo.Telegram("botToken", "chatId")
        ///         .CreateLogger();
        /// </example>
        /// </summary>
        /// <param name="loggerSinkConfiguration">Instance of <see cref="LoggerSinkConfiguration"/> object.</param>
        /// <param name="botToken">The Telegram bot token.</param>
        /// <param name="chatId">The Telegram chat id.</param>
        /// <param name="batchSizeLimit">The maximum number of events to post in a single batch; defaults to 1 if
        /// not provided i.e. no batching by default.</param>
        /// <param name="period">The time to wait between checking for event batches; defaults to 1 sec if not
        /// provided.</param>
        /// <param name="formatProvider">The format provider used for formatting the message.</param>
        /// <param name="restrictedToMinimumLevel"><see cref="LogEventLevel"/> value that specifies minimum logging
        /// level that will be allowed to be logged.</param>
        /// <returns>Instance of <see cref="LoggerConfiguration"/> object.</returns>
        public static LoggerConfiguration Telegram(
            this LoggerSinkConfiguration loggerSinkConfiguration,
            string botToken,
            string chatId = null,
            int? batchSizeLimit = null,
            TimeSpan? period = null,
            IFormatProvider formatProvider = null,
            LogEventLevel restrictedToMinimumLevel = LevelAlias.Minimum)
        {
            var telegramSinkOptions = new TelegramSinkOptions(botToken, chatId, batchSizeLimit, period,
                formatProvider);

            return loggerSinkConfiguration.Telegram(telegramSinkOptions, restrictedToMinimumLevel);
        }

        /// <summary>
        /// <see cref="LoggerSinkConfiguration"/> extension that provides configuration chaining.
        /// </summary>
        /// <param name="loggerSinkConfiguration">Instance of <see cref="LoggerSinkConfiguration"/> object.</param>
        /// <param name="telegramSinkOptions">The Telegram sink options object.</param>
        /// <param name="restrictedToMinimumLevel"><see cref="LogEventLevel"/> value that specifies minimum logging
        /// level that will be allowed to be logged.</param>
        /// <returns>Instance of <see cref="LoggerConfiguration"/> object.</returns>
        public static LoggerConfiguration Telegram(
            this LoggerSinkConfiguration loggerSinkConfiguration,
            TelegramSinkOptions telegramSinkOptions,
            LogEventLevel restrictedToMinimumLevel = LevelAlias.Minimum)
        {
            if (loggerSinkConfiguration == null)
            {
                throw new ArgumentNullException(nameof(loggerSinkConfiguration));
            }

            if (telegramSinkOptions == null)
            {
                throw new ArgumentNullException(nameof(telegramSinkOptions));
            }

            if (string.IsNullOrWhiteSpace(telegramSinkOptions.BotToken))
            {
                throw new ArgumentNullException(nameof(telegramSinkOptions.BotToken));
            }

            return loggerSinkConfiguration.Sink(new TelegramSink(telegramSinkOptions), restrictedToMinimumLevel);
        }
    }
}