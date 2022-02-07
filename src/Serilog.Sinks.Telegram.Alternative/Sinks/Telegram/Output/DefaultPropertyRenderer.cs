// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultPropertyRenderer.cs" company="SeppPenner and the Serilog contributors">
// The project is licensed under the MIT license.
// </copyright>
// <summary>
//   A fallback renderer for log event properties.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Serilog.Sinks.Telegram.Output
{
    using System.IO;
    using Serilog.Parsing;
    using Serilog.Sinks.Telegram.Alternative;

    /// <summary>
    /// A fallback renderer for log event properties.
    /// </summary>
    public class DefaultPropertyRenderer : IPropertyRenderer
    {
        /// <summary>
        /// The property token.
        /// </summary>
        private readonly PropertyToken propertyToken;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultPropertyRenderer"/> class.
        /// </summary>
        /// <param name="propertyToken">The property token.</param>
        public DefaultPropertyRenderer(PropertyToken propertyToken)
        {
            this.propertyToken = propertyToken;
        }

        /// <inheritdoc cref="IPropertyRenderer"/>
        public void Render(ExtendedLogEvent logEvent, TextWriter output)
        {
            if (!logEvent.LogEvent.Properties.TryGetValue(this.propertyToken.PropertyName, out var propertyValue))
            {
                output.Write(this.propertyToken.ToString());
                return;
            }

            propertyValue.Render(output, this.propertyToken.Format);
        }
    }
}
