using System.IO;
using Serilog.Parsing;
using Serilog.Sinks.Telegram.Alternative;

namespace Serilog.Sinks.Telegram.Output
{
    public class TextTokenRenderer
    {
        public static void Render(TextToken token, TextWriter output, TelegramSinkOptions options)
        {
            output.Write(token.Text.HtmlEscape(options.ShouldEscape));
        }
    }
}
