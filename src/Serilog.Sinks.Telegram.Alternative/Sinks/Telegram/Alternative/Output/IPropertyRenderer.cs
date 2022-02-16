// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IPropertyRenderer.cs" company="SeppPenner and the Serilog contributors">
// The project is licensed under the MIT license.
// </copyright>
// <summary>
//   A contract for property renderers.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Serilog.Sinks.Telegram.Output;

/// <summary>
/// A contract for property renderers.
/// </summary>
public interface IPropertyRenderer
{
    /// <summary>
    /// Renders the given <paramref name="logEvent"/>. Results are written to the <paramref name="output"/>.
    /// </summary>
    /// <param name="logEvent">The log event.</param>
    /// <param name="output">The output.</param>
    void Render(ExtendedLogEvent logEvent, TextWriter output);
}
