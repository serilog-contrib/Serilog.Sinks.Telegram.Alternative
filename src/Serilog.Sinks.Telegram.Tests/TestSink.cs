// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TestSink.cs" company="Hämmer Electronics">
// The project is licensed under the MIT license.
// </copyright>
// <summary>
//   A class to test the <see cref="TelegramSink"/>.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Serilog.Sinks.Telegram.Tests
{
    using System;
    using System.Threading;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// A class to test the <see cref="TelegramSink"/>.
    /// </summary>
    [TestClass]
    public class TestSink
    {
        /// <summary>
        /// The Telegram bot token.
        /// </summary>
        private readonly string telegramBotToken = Environment.GetEnvironmentVariable("TelegramBotToken");

        /// <summary>
        /// The Telegram bot token.
        /// </summary>
        private readonly string telegramChatId = Environment.GetEnvironmentVariable("TelegramChatId");

        /// <summary>
        /// Tests the sink with different exceptions.
        /// </summary>
        [TestMethod]
        public void TestDifferentExceptions()
        {
            var logger = new LoggerConfiguration()
                .WriteTo.Telegram(this.telegramBotToken, this.telegramChatId,  3)
                .CreateLogger();

            var exception1 = new Exception("test1");
            logger.Error(exception1, exception1.Message);
            var exception2 = new Exception("test2");
            logger.Error(exception2, exception2.Message);
            var exception3 = new Exception("test3");
            logger.Error(exception3, exception3.Message);

            Thread.Sleep(1000);
        }

        /// <summary>
        /// Tests the sink with the same exceptions.
        /// </summary>
        [TestMethod]
        public void TestSameExceptions()
        {
            var logger = new LoggerConfiguration()
                .WriteTo.Telegram(this.telegramBotToken, this.telegramChatId, 3)
                .CreateLogger();

            var exception1 = new Exception("test1");
            logger.Error(exception1, exception1.Message);
            logger.Error(exception1, exception1.Message);
            logger.Error(exception1, exception1.Message);

            Thread.Sleep(1000);
        }
    }
}
