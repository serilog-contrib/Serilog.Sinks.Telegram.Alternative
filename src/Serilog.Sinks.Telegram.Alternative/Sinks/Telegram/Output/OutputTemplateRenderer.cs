using Serilog.Events;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Serilog.Formatting.Display;
using Serilog.Parsing;
using Serilog.Sinks.Telegram.Alternative;

namespace Serilog.Sinks.Telegram.Output
{
    internal class OutputTemplateRenderer
    {
        private const int DefaultWriteCapacity = 256;

        private readonly Action<ExtendedLogEvent, TextWriter>[] _renderActions;

        internal OutputTemplateRenderer(string outputTemplate, TelegramSinkOptions options)
        {
            MessageTemplate template = new MessageTemplateParser().Parse(outputTemplate);
            var renderActions = new List<Action<ExtendedLogEvent, TextWriter>>();
            foreach (MessageTemplateToken token in template.Tokens)
            {
                switch (token)
                {
                    case TextToken tt:
                        renderActions.Add((_,w) => TextTokenRenderer.Render(tt, w, options));
                        break;
                    case PropertyToken pt:
                        switch (pt.PropertyName)
                        {
                            case OutputProperties.LevelPropertyName:
                                renderActions.Add(new LogLevelRenderer(pt).Render);
                                break;
                            case OutputProperties.NewLinePropertyName:
                                renderActions.Add((_,w) => w.WriteLine());
                                break;
                            case OutputProperties.ExceptionPropertyName:
                                renderActions.Add(new ExceptionRenderer(pt, options).Render);
                                break;
                            case OutputProperties.MessagePropertyName:
                                renderActions.Add(new MessageRenderer(pt, options).Render);
                                break;
                            case OutputProperties.TimestampPropertyName:
                                renderActions.Add(new TimestampRenderer(pt).Render);
                                break;
                            default:
                                renderActions.Add(new DefaultPropertyRenderer(pt).Render);
                                break;
                        }
                        break;
                }
            }

            _renderActions = renderActions.ToArray();
        }

        public string Format(ExtendedLogEvent logEvent)
        {
            using var sw = new StringWriter(new StringBuilder(DefaultWriteCapacity));
            foreach (Action<ExtendedLogEvent, TextWriter> renderAction in _renderActions)
            {
                renderAction(logEvent, sw);
            }
            return sw.ToString();
        }
    }
}
