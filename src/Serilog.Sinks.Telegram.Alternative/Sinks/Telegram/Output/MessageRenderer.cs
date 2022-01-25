using System.IO;
using System.Text;
using Serilog.Parsing;
using Serilog.Sinks.Telegram.Alternative;

namespace Serilog.Sinks.Telegram.Output
{
    /// <summary>
    ///     Renders the Message property of a log event.
    /// </summary>
    public class MessageRenderer : IPropertyRenderer
    {
        private readonly PropertyToken _propertyToken;
        private readonly TelegramSinkOptions _options;

        /// <summary>
        ///     Creates a new instance of the renderer.
        /// </summary>
        /// <param name="propertyToken">The property token to render.</param>
        /// <param name="options">The sink options.</param>
        public MessageRenderer(PropertyToken propertyToken, TelegramSinkOptions options)
        {
            _propertyToken = propertyToken;
            _options = options;
        }

        /// <inheritdoc />
        public void Render(ExtendedLogEvent logEvent, TextWriter output)
        {
            using var sw = new StringWriter(new StringBuilder());
            foreach (MessageTemplateToken token in logEvent.LogEvent.MessageTemplate.Tokens)
            {
                switch (token)
                {
                    case TextToken tt:
                        TextTokenRenderer.Render(tt, output, _options);
                        break;
                    case PropertyToken pt:
                        new DefaultPropertyRenderer(pt).Render(logEvent, sw);
                        break;
                }
            }
            output.Write(HtmlEscaper.Escape(_options, sw.ToString()));
        }
    }
}
