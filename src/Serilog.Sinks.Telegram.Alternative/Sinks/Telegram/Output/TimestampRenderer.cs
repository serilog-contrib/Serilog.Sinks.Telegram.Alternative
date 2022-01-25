using System.IO;
using Serilog.Events;
using Serilog.Parsing;
using Serilog.Sinks.Telegram.Alternative;

namespace Serilog.Sinks.Telegram.Output
{
    /// <summary>
    ///     Renders a timestamp property of a log event.
    /// </summary>
    public class TimestampRenderer : IPropertyRenderer
    {
        private readonly PropertyToken _propertyToken;

        /// <summary>
        ///     Creates a new instance of the renderer.
        /// </summary>
        /// <param name="propertyToken">The property token to render.</param>
        public TimestampRenderer(PropertyToken propertyToken)
        {
            _propertyToken = propertyToken;
        }

        /// <inheritdoc />
        public void Render(ExtendedLogEvent logEvent, TextWriter output)
        {
            var sv = new ScalarValue(logEvent.LogEvent.Timestamp);
            sv.Render(output, _propertyToken.Format);
        }
    }
}
