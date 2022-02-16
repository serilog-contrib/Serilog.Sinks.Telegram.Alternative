// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OutputTemplateRenderer.cs" company="SeppPenner and the Serilog contributors">
// The project is licensed under the MIT license.
// </copyright>
// <summary>
//   A renderer for the output templates.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Serilog.Sinks.Telegram.Output;

/// <summary>
/// A renderer for the output templates.
/// </summary>
internal class OutputTemplateRenderer
{
    /// <summary>
    /// The default write capacity.
    /// </summary>
    private const int DefaultWriteCapacity = 256;

    /// <summary>
    /// The render actions.
    /// </summary>
    private readonly Action<ExtendedLogEvent, TextWriter>[] renderActions;

    /// <summary>
    /// Initializes a new instance of the <see cref="OutputTemplateRenderer"/> class.
    /// </summary>
    /// <param name="outputTemplate">The output template.</param>
    /// <param name="options">The options.</param>
    internal OutputTemplateRenderer(string outputTemplate, TelegramSinkOptions options)
    {
        MessageTemplate template = new MessageTemplateParser().Parse(outputTemplate);
        var renderActions = new List<Action<ExtendedLogEvent, TextWriter>>();

        foreach (MessageTemplateToken token in template.Tokens)
        {
            switch (token)
            {
                case TextToken textToken:
                    renderActions.Add((_, w) => TextTokenRenderer.Render(textToken, w, options));
                    break;
                case PropertyToken propertyToken:
                    switch (propertyToken.PropertyName)
                    {
                        case OutputProperties.LevelPropertyName:
                            renderActions.Add(new LogLevelRenderer(propertyToken).Render);
                            break;
                        case OutputProperties.NewLinePropertyName:
                            renderActions.Add((_, w) => w.WriteLine());
                            break;
                        case OutputProperties.ExceptionPropertyName:
                            renderActions.Add(new ExceptionRenderer(options).Render);
                            break;
                        case OutputProperties.MessagePropertyName:
                            renderActions.Add(new MessageRenderer(options).Render);
                            break;
                        case OutputProperties.TimestampPropertyName:
                            renderActions.Add(new TimestampRenderer(propertyToken).Render);
                            break;
                        default:
                            renderActions.Add(new DefaultPropertyRenderer(propertyToken).Render);
                            break;
                    }

                    break;
            }
        }

        this.renderActions = renderActions.ToArray();
    }

    /// <summary>
    /// Formats the log event.
    /// </summary>
    /// <param name="logEvent">The log event.</param>
    /// <returns>The formatted log event as <see cref="string"/>.</returns>
    public string Format(ExtendedLogEvent logEvent)
    {
        using var writer = new StringWriter(new StringBuilder(DefaultWriteCapacity));

        foreach (Action<ExtendedLogEvent, TextWriter> renderAction in this.renderActions)
        {
            renderAction(logEvent, writer);
        }

        return writer.ToString();
    }
}
