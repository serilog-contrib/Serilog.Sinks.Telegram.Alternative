using System.IO;
using Serilog.Parsing;
using Serilog.Sinks.Telegram.Alternative;

namespace Serilog.Sinks.Telegram.Output
{
    /// <summary>
    ///     Fallback renderer for log event properties.
    /// </summary>
    public class DefaultPropertyRenderer : IPropertyRenderer
    {
        private readonly PropertyToken _propertyToken;

        /// <summary>
        ///     Instantiates a new default renderer for the given property token.
        /// </summary>
        /// <param name="propertyToken"></param>
        public DefaultPropertyRenderer(PropertyToken propertyToken)
        {
            _propertyToken = propertyToken;
        }

        /// <inheritdoc />
        public void Render(ExtendedLogEvent logEvent, TextWriter output)
        {
            if (!logEvent.LogEvent.Properties.TryGetValue(_propertyToken.PropertyName, out var propertyValue))
            {
                output.Write(_propertyToken.ToString());
                return;
            }

            propertyValue.Render(output, _propertyToken.Format);
        }
    }
}
