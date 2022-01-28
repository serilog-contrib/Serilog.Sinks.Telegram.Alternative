using System.IO;
using Serilog.Events;
using Serilog.Parsing;
using Serilog.Sinks.Telegram.Alternative;

namespace Serilog.Sinks.Telegram.Output
{
    /// <summary>
    /// Formats the log level of a log event for using a property token.
    /// </summary>
    internal class LogLevelRenderer : IPropertyRenderer
    {
        private readonly PropertyToken _propertyToken;

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        /// <param name="propertyToken">The property token.</param>
        internal LogLevelRenderer(PropertyToken propertyToken)
        {
            _propertyToken = propertyToken;
        }

        /// <summary>
        /// Renders the given <paramref name="logEvent"/> and writes the results into <paramref name="output"/>.
        /// </summary>
        /// <param name="logEvent">The log event.</param>
        /// <param name="output">The output.</param>
        public void Render(ExtendedLogEvent logEvent, TextWriter output)
        {
            string stringLevel = logEvent.LogEvent.Level.ToString();
            switch (_propertyToken.Format)
            {
                case "e":
                    stringLevel = GetEmoji(logEvent.LogEvent);
                    break;
                case "u":
                    stringLevel = stringLevel.ToUpperInvariant();
                    break;
                case "l":
                    stringLevel = stringLevel.ToLowerInvariant();
                    break;
            }

            output.Write(stringLevel);
        }

        /// <summary>
        /// Gets the emoji.
        /// </summary>
        /// <param name="log">The log.</param>
        /// <returns>The emoji as <see cref="string"/>.</returns>
        internal static string GetEmoji(LogEvent log)
        {
            return log.Level switch
            {
                LogEventLevel.Debug => "👉",
                LogEventLevel.Error => "❗",
                LogEventLevel.Fatal => "‼",
                LogEventLevel.Information => "ℹ",
                LogEventLevel.Verbose => "⚡",
                LogEventLevel.Warning => "⚠",
                _ => string.Empty
            };
        }
    }
}
