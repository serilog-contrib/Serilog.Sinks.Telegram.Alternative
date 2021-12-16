using System.IO;
using Serilog.Sinks.Telegram.Alternative;

namespace Serilog.Sinks.Telegram.Output
{
    /// <summary>
    /// Contract for property renderers.
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
}
