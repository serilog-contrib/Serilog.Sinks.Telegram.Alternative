using System.IO;
using Serilog.Parsing;
using Serilog.Sinks.Telegram.Alternative;

namespace Serilog.Sinks.Telegram.Output
{
    /// <summary>
    ///     Renders exceptions.
    /// </summary>
    public class ExceptionRenderer : IPropertyRenderer
    {
        private readonly PropertyToken _propertyToken;
        private readonly TelegramSinkOptions _options;

        /// <summary>
        ///     Creates a new instance of the renderer.
        /// </summary>
        /// <param name="propertyToken">The property token to render.</param>
        /// <param name="options">The sink options.</param>
        public ExceptionRenderer(PropertyToken propertyToken, TelegramSinkOptions options)
        {
            _propertyToken = propertyToken;
            _options = options;
        }

        /// <inheritdoc />
        public void Render(ExtendedLogEvent extLogEvent, TextWriter output)
        {
            if (extLogEvent.LogEvent.Exception is null)
            {
                return;
            }

            var message = HtmlEscaper.Escape(_options, extLogEvent.LogEvent.Exception.Message);
            var exceptionType = HtmlEscaper.Escape(_options, extLogEvent.LogEvent.Exception.GetType().Name);

            output.WriteLine($"\n<strong>{message}</strong>\n");
            output.WriteLine($"Message: <code>{message}</code>");
            output.WriteLine($"Type: <code>{exceptionType}</code>\n");

            if (extLogEvent.IncludeStackTrace)
            {
                var exception = HtmlEscaper.Escape(_options, $"{extLogEvent.LogEvent.Exception}");
                output.WriteLine($"Stack Trace\n<code>{exception}</code>");
            }
        }
    }
}
