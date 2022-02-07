// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExceptionRenderer.cs" company="SeppPenner and the Serilog contributors">
// The project is licensed under the MIT license.
// </copyright>
// <summary>
//   A renderer for exceptions.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Serilog.Sinks.Telegram.Output
{
    using System.IO;
    using Serilog.Parsing;
    using Serilog.Sinks.Telegram.Alternative;

    /// <summary>
    /// A renderer for exceptions.
    /// </summary>
    public class ExceptionRenderer : IPropertyRenderer
    {
        /// <summary>
        /// The Telegram sink options.
        /// </summary>
        private readonly TelegramSinkOptions options;

        /// <summary>
        ///     Initializes a new instance of the <see cref="DefaultPropertyRenderer"/> class.
        /// </summary>
        /// <param name="options">The Telegram sink options.</param>
        public ExceptionRenderer(TelegramSinkOptions options)
        {
            this.options = options;
        }

        /// <inheritdoc cref="IPropertyRenderer"/>
        public void Render(ExtendedLogEvent extLogEvent, TextWriter output)
        {
            if (extLogEvent.LogEvent.Exception is null)
            {
                return;
            }

            var message = HtmlEscaper.Escape(this.options, extLogEvent.LogEvent.Exception.Message);
            var exceptionType = HtmlEscaper.Escape(this.options, extLogEvent.LogEvent.Exception.GetType().Name);

            output.WriteLine($"\n<strong>{message}</strong>\n");
            output.WriteLine($"Message: <code>{message}</code>");
            output.WriteLine($"Type: <code>{exceptionType}</code>\n");

            if (extLogEvent.IncludeStackTrace)
            {
                var exception = HtmlEscaper.Escape(this.options, $"{extLogEvent.LogEvent.Exception}");
                output.WriteLine($"Stack Trace\n<code>{exception}</code>");
            }
        }
    }
}
