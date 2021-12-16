using System.IO;
using Serilog.Events;
using Serilog.Parsing;
using Serilog.Sinks.Telegram.Alternative;

namespace Serilog.Sinks.Telegram.Output
{
    public class TimestampRenderer : IPropertyRenderer
    {
        private readonly PropertyToken _propertyToken;

        public TimestampRenderer(PropertyToken propertyToken)
        {
            _propertyToken = propertyToken;
        }

        public void Render(ExtendedLogEvent logEvent, TextWriter output)
        {
            var sv = new ScalarValue(logEvent.LogEvent.Timestamp);
            sv.Render(output, _propertyToken.Format);
        }
    }
}
