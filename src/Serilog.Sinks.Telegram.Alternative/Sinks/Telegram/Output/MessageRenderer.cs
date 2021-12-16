using System.IO;
using System.Text;
using Serilog.Parsing;
using Serilog.Sinks.Telegram.Alternative;

namespace Serilog.Sinks.Telegram.Output
{
    public class MessageRenderer : IPropertyRenderer
    {
        private readonly PropertyToken _propertyToken;
        private readonly TelegramSinkOptions _options;

        public MessageRenderer(PropertyToken propertyToken, TelegramSinkOptions options)
        {
            _propertyToken = propertyToken;
            _options = options;
        }

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
            output.Write(sw.ToString().HtmlEscape(_options.ShouldEscape));
        }
    }
}
