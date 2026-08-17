// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MessageRenderer.cs" company="SeppPenner and the Serilog contributors">
// The project is licensed under the MIT license.
// </copyright>
// <summary>
//   A renderer for the messages.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Serilog.Sinks.Telegram.Output;

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
    /// <remarks>
    /// All tokens are rendered unescaped into one writer and the result is escaped once. Writing the
    /// text tokens straight to the output and only the properties into a second writer would put every
    /// rendered property behind the whole text, so "Hello {Name}!" came out as "Hello !John".
    /// </remarks>
    public void Render(ExtendedLogEvent logEvent, TextWriter output)
    {
        using var writer = new StringWriter(new StringBuilder());

        foreach (MessageTemplateToken token in logEvent.LogEvent.MessageTemplate.Tokens)
        {
            switch (token)
            {
                case TextToken textToken:
                    writer.Write(textToken.Text);
                    break;
                case PropertyToken propertyToken:
                    new DefaultPropertyRenderer(propertyToken).Render(logEvent, writer);
                    break;
            }
        }

        output.Write(HtmlEscaper.Escape(this.options, writer.ToString()));
    }
}
