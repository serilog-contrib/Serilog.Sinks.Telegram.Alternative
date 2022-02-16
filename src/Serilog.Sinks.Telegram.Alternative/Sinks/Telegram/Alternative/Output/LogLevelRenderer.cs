// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LogLevelRenderer.cs" company="SeppPenner and the Serilog contributors">
// The project is licensed under the MIT license.
// </copyright>
// <summary>
//   A renderer for the log event levels.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Serilog.Sinks.Telegram.Output;

/// <summary>
/// A renderer for the log event levels.
/// </summary>
internal class LogLevelRenderer : IPropertyRenderer
{
    /// <summary>
    /// The property token.
    /// </summary>
    private readonly PropertyToken propertyToken;

    /// <summary>
    /// Initializes a new instance of the <see cref="LogLevelRenderer"/> class.
    /// </summary>
    /// <param name="propertyToken">The property token.</param>
    internal LogLevelRenderer(PropertyToken propertyToken)
    {
        this.propertyToken = propertyToken;
    }

    /// <inheritdoc cref="IPropertyRenderer"/>
    public void Render(ExtendedLogEvent logEvent, TextWriter output)
    {
        string stringLevel = logEvent.LogEvent.Level.ToString();

        switch (this.propertyToken.Format)
        {
            case "e":
                stringLevel = GetEmoji(logEvent.LogEvent);
                break;
            case "u":
                stringLevel = stringLevel.ToUpperInvariant();
                break;
            case "l":
                stringLevel = stringLevel.ToLowerInvariant();
                break;
        }

        output.Write(stringLevel);
    }

    /// <summary>
    /// Gets the emoji.
    /// </summary>
    /// <param name="logEvent">The log event.</param>
    /// <returns>The emoji as <see cref="string"/>.</returns>
    internal static string GetEmoji(LogEvent logEvent)
    {
        return logEvent.Level switch
        {
            LogEventLevel.Debug => "👉",
            LogEventLevel.Error => "❗",
            LogEventLevel.Fatal => "‼",
            LogEventLevel.Information => "ℹ",
            LogEventLevel.Verbose => "⚡",
            LogEventLevel.Warning => "⚠",
            _ => string.Empty
        };
    }
}
