namespace Serilog.Sinks.Telegram
{
    /// <summary>
    /// The telegram message.
    /// </summary>
    public class TelegramMessage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TelegramMessage"/> class.
        /// </summary>
        /// <param name="text">The text.</param>
        public TelegramMessage(string text)
        {
            Text = text;
        }

        /// <summary>
        /// Gets the text.
        /// </summary>
        public string Text { get; }
    }
}