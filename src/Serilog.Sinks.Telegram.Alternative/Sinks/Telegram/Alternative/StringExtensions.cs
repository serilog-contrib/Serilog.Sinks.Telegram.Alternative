// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StringExtensions.cs" company="SeppPenner and the Serilog contributors">
// The project is licensed under the MIT license.
// </copyright>
// <summary>
//   A class containing extension methods for the <see cref="string" /> type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Serilog.Sinks.Telegram.Alternative;

/// <summary>
/// A class containing extension methods for the <see cref="string"/> type.
/// </summary>
internal static class StringExtensions
{
    /// <summary>
    /// Escapes the invalid chars for Telegram to valid HTML encoded ones.
    /// </summary>
    /// <param name="value">The <see cref="string"/> value to escape.</param>
    /// <param name="shouldEscape">A value indicating whether the value should be escaped or not.</param>
    /// <returns>A new escaped <see cref="string"/>.</returns>
    /// <remarks>
    /// The ampersand has to be replaced first. Replacing it last would escape the ampersands of the
    /// entities written just before it, so a single left angle bracket would end up as "&amp;amp;lt;".
    /// </remarks>
    public static string HtmlEscape(this string value, bool shouldEscape)
    {
        return shouldEscape ? value.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;") : value;
    }
}
