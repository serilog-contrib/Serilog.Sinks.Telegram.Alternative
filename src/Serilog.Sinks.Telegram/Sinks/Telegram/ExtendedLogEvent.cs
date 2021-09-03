// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExtendedLogEvent.cs" company="SeppPenner and the Serilog contributors">
// The project is licensed under the MIT license.
// </copyright>
// <summary>
//   Added a new class to store the first and last occurrence timestamps.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Serilog.Sinks.Telegram
{
    using System;

    using Serilog.Events;

    /// <summary>
    /// Added a new class to store the first and last occurrence timestamps.
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
        /// <param name="firstOccurrence">The first occurrence.</param>
        /// <param name="lastOccurrence">The last occurrence.</param>
        /// <param name="logEvent">The log event.</param>
        /// <param name="includeStackTrace">A value indicating whether the stack trace should be shown or not.</param>
        // ReSharper disable once UnusedMember.Global
        public ExtendedLogEvent(DateTime firstOccurrence, DateTime lastOccurrence, LogEvent logEvent, bool? includeStackTrace = true)
        {
            this.FirstOccurrence = firstOccurrence;
            this.LastOccurrence = lastOccurrence;
            this.LogEvent = logEvent;
            this.IncludeStackTrace = includeStackTrace ?? true;
        }

        /// <summary>
        /// Gets or sets the log event.
        /// </summary>
        public LogEvent LogEvent { get; set; }

        /// <summary>
        /// Gets or sets the first occurrence.
        /// </summary>
        public DateTimeOffset FirstOccurrence { get; set; }

        /// <summary>
        /// Gets or sets the last occurrence.
        /// </summary>
        public DateTimeOffset LastOccurrence { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the stack trace should be shown or not.
        /// </summary>
        public bool IncludeStackTrace { get; set; }
    }
}