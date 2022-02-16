// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TimestampRenderer.cs" company="SeppPenner and the Serilog contributors">
// The project is licensed under the MIT license.
// </copyright>
// <summary>
//   A renderer for the timestamps.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Serilog.Sinks.Telegram.Output;

/// <summary>
///     Renders a timestamp property of a log event.
/// </summary>
public class TimestampRenderer : IPropertyRenderer
{
    /// <summary>
    /// The property token.
    /// </summary>
    private readonly PropertyToken propertyToken;

    /// <summary>
    ///     Initializes a new instance of the <see cref="TimestampRenderer"/> class.
    /// </summary>
    /// <param name="propertyToken">The property token.</param>
    public TimestampRenderer(PropertyToken propertyToken)
    {
        this.propertyToken = propertyToken;
    }

    /// <inheritdoc cref="IPropertyRenderer"/>
    public void Render(ExtendedLogEvent logEvent, TextWriter output)
    {
        var scalarValue = new ScalarValue(logEvent.LogEvent.Timestamp);
        scalarValue.Render(output, this.propertyToken.Format);
    }
}
