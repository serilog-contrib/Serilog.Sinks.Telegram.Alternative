// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TextTokenRenderer.cs" company="SeppPenner and the Serilog contributors">
// The project is licensed under the MIT license.
// </copyright>
// <summary>
//   A renderer for the text tokens.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Serilog.Sinks.Telegram.Output
{
    using System.IO;
    using Serilog.Parsing;
    using Serilog.Sinks.Telegram.Alternative;

    /// <summary>
    ///     A renderer for the text tokens.
    /// </summary>
    public class TextTokenRenderer
    {
        /// <summary>
        /// Renders the given text <paramref name="token"/>. Results are written to the <paramref name="output"/>.
        /// </summary>
        /// <param name="token">The text token.</param>
        /// <param name="output">The output.</param>
        /// <param name="options">The sink options.</param>
        public static void Render(TextToken token, TextWriter output, TelegramSinkOptions options)
        {
            output.Write(HtmlEscaper.Escape(options, token.Text));
        }
    }
}
