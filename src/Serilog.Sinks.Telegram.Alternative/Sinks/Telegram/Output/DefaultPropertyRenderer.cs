using System.IO;
using Serilog.Parsing;
using Serilog.Sinks.Telegram.Alternative;

namespace Serilog.Sinks.Telegram.Output
{
    public class DefaultPropertyRenderer : IPropertyRenderer
    {
        private readonly PropertyToken _propertyToken;

        public DefaultPropertyRenderer(PropertyToken propertyToken)
        {
            _propertyToken = propertyToken;
        }

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
