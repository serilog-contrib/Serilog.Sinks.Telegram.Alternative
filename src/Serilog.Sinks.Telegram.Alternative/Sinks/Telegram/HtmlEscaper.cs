namespace Serilog.Sinks.Telegram.Alternative;

/// <summary>
///     Static class providing HTML escaping functionality.
/// </summary>
internal static class HtmlEscaper
{
    /// <summary>
    /// Correctly escapes strings taking options into consideration.
    /// </summary>
    /// <param name="options">The options specified by the consumer.</param>
    /// <param name="message">The string to escape.</param>
    /// <returns>The properly escaped string.</returns>
    internal static string Escape(TelegramSinkOptions options, string message)
    {
        return options.CustomHtmlFormatter is null
            ? message.HtmlEscape(options.ShouldEscape)
            : options.CustomHtmlFormatter.Invoke(message);
    }
}
