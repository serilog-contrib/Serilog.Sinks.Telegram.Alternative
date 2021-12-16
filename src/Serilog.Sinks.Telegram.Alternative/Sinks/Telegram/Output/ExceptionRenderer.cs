using System.IO;
using Serilog.Parsing;
using Serilog.Sinks.Telegram.Alternative;

namespace Serilog.Sinks.Telegram.Output
{
    public class ExceptionRenderer : IPropertyRenderer
    {
        private readonly PropertyToken _propertyToken;
        private readonly TelegramSinkOptions _options;

        public ExceptionRenderer(PropertyToken propertyToken, TelegramSinkOptions options)
        {
            _propertyToken = propertyToken;
            _options = options;
        }

        public void Render(ExtendedLogEvent extLogEvent, TextWriter output)
        {
            if (extLogEvent.LogEvent.Exception is null)
            {
                return;
            }

            var message = extLogEvent.LogEvent.Exception.Message.HtmlEscape(_options.ShouldEscape);
            var exceptionType = extLogEvent.LogEvent.Exception.GetType().Name.HtmlEscape(_options.ShouldEscape);

            output.WriteLine($"\n<strong>{message}</strong>\n");
            output.WriteLine($"Message: <code>{message}</code>");
            output.WriteLine($"Type: <code>{exceptionType}</code>\n");

            if (extLogEvent.IncludeStackTrace)
            {
                var exception = $"{extLogEvent.LogEvent.Exception}".HtmlEscape(_options.ShouldEscape);
                output.WriteLine($"Stack Trace\n<code>{exception}</code>");
            }
        }
    }
}
