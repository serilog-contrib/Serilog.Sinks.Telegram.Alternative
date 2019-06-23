
namespace Serilog.Sinks.Telegram
{
    using Serilog.Events;
    using System;

    /// <summary>
    /// Added a new class to store the first and last occurance timestamps.
    /// </summary>
    public class ExtendedLogEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExtendedLogEvent"/> class.
        /// </summary>
        public ExtendedLogEvent()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExtendedLogEvent"/> class.
        /// </summary>
        /// <param name="firstOccurance">The first occurance.</param>
        /// <param name="lastOccurance">The last occurance.</param>
        /// <param name="logEvent">The log event.</param>
        public ExtendedLogEvent(DateTime firstOccurance, DateTime lastOccurance, LogEvent logEvent)
        {
            FirstOccurance = firstOccurance;
            LastOccurance = lastOccurance;
            LogEvent = logEvent;
        }

        /// <summary>
        /// Gets or sets the log event.
        /// </summary>
        public LogEvent LogEvent { get; set; }

        /// <summary>
        /// Gets or sets the first occurance.
        /// </summary>
        public DateTimeOffset FirstOccurance { get; set; }

        /// <summary>
        /// Gets or sets the last occurance.
        /// </summary>
        public DateTimeOffset LastOccurance { get; set; }
    }
}