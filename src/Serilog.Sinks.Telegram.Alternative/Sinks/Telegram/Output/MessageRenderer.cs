// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MessageRenderer.cs" company="SeppPenner and the Serilog contributors">
// The project is licensed under the MIT license.
// </copyright>
// <summary>
//   A renderer for the messages.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Serilog.Sinks.Telegram.Output
{
    using System.IO;
    using System.Text;
    using Serilog.Parsing;
    using Serilog.Sinks.Telegram.Alternative;

    /// <summary>
    /// A renderer for the messages.
    /// </summary>
    public class MessageRenderer : IPropertyRenderer
    {
        /// <summary>
        /// The Telegram sink options.
        /// </summary>
        private readonly TelegramSinkOptions options;

        /// <summary>
        ///     Creates a new instance of the renderer.
        /// </summary>
        /// <param name="options">The Telegram sink options.</param>
        public MessageRenderer(TelegramSinkOptions options)
        {
            this.options = options;
        }

        /// <inheritdoc cref="IPropertyRenderer"/>
        public void Render(ExtendedLogEvent logEvent, TextWriter output)
        {
            using var writer = new StringWriter(new StringBuilder());

            foreach (MessageTemplateToken token in logEvent.LogEvent.MessageTemplate.Tokens)
            {
                switch (token)
                {
                    case TextToken textToken:
                        TextTokenRenderer.Render(textToken, output, options);
                        break;
                    case PropertyToken propertyToken:
                        new DefaultPropertyRenderer(propertyToken).Render(logEvent, writer);
                        break;
                }
            }

            output.Write(HtmlEscaper.Escape(options, writer.ToString()));
        }
    }
}
