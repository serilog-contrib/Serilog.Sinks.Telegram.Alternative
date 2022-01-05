// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TestSink.cs" company="H�mmer Electronics">
// The project is licensed under the MIT license.
// </copyright>
// <summary>
//   A class to test the <see cref="TelegramSink"/>.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Serilog.Sinks.Telegram.Alternative.Tests
{
    using System;
    using System.Threading;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Serilog.Debugging;

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
                .WriteTo.Telegram(this.telegramBotToken, this.telegramChatId, 1)
                .CreateLogger();

            var exception1 = new Exception("test1");
            logger.Error(exception1, exception1.Message);
            var exception2 = new Exception("test2");
            logger.Error(exception2, exception2.Message);
            var exception3 = new Exception("test3");
            logger.Error(exception3, exception3.Message);

            Thread.Sleep(1000);
            Log.CloseAndFlush();
        }

        /// <summary>
        /// Tests the sink with the same exceptions.
        /// </summary>
        [TestMethod]
        public void TestSameExceptions()
        {
            var logger = new LoggerConfiguration()
                .WriteTo.Telegram(this.telegramBotToken, this.telegramChatId, 1)
                .CreateLogger();

            var exception1 = new Exception("test1");
            logger.Error(exception1, exception1.Message);
            logger.Error(exception1, exception1.Message);
            logger.Error(exception1, exception1.Message);

            Thread.Sleep(1000);
            Log.CloseAndFlush();
        }

        /// <summary>
        /// Tests the sink with an exception with an underscore sign.
        /// </summary>
        [TestMethod]
        public void TestExceptionWithUnderscoreSign()
        {
            var logger = new LoggerConfiguration()
                .WriteTo.Telegram(this.telegramBotToken, this.telegramChatId, 1)
                .CreateLogger();

            var exception1 = new Exception("_Something");
            logger.Error(exception1, exception1.Message);

            Thread.Sleep(1000);
            Log.CloseAndFlush();
        }

        /// <summary>
        /// Tests the sink with an exception with a multiply sign.
        /// </summary>
        [TestMethod]
        public void TestExceptionWithMultiplySign()
        {
            var logger = new LoggerConfiguration()
                .WriteTo.Telegram(this.telegramBotToken, this.telegramChatId, 1)
                .CreateLogger();

            var exception1 = new Exception("*Something");
            logger.Error(exception1, exception1.Message);

            Thread.Sleep(1000);
            Log.CloseAndFlush();
        }

        /// <summary>
        /// Tests the sink with an exception with a left brackets sign.
        /// </summary>
        [TestMethod]
        public void TestExceptionWithLeftBracketSign()
        {
            var logger = new LoggerConfiguration()
                .WriteTo.Telegram(this.telegramBotToken, this.telegramChatId, 1)
                .CreateLogger();

            var exception1 = new Exception("[Something");
            logger.Error(exception1, exception1.Message);

            Thread.Sleep(1000);
            Log.CloseAndFlush();
        }

        /// <summary>
        /// Tests the sink with an exception with a right brackets sign.
        /// </summary>
        [TestMethod]
        public void TestExceptionWithRightBracketSign()
        {
            var logger = new LoggerConfiguration()
                .WriteTo.Telegram(this.telegramBotToken, this.telegramChatId, 1)
                .CreateLogger();

            var exception1 = new Exception("]Something");
            logger.Error(exception1, exception1.Message);

            Thread.Sleep(1000);
            Log.CloseAndFlush();
        }

        /// <summary>
        /// Tests the sink with an exception with a left rounded brackets sign.
        /// </summary>
        [TestMethod]
        public void TestExceptionWithLeftRoundedBracketSign()
        {
            var logger = new LoggerConfiguration()
                .WriteTo.Telegram(this.telegramBotToken, this.telegramChatId, 1)
                .CreateLogger();

            var exception1 = new Exception("(Something");
            logger.Error(exception1, exception1.Message);

            Thread.Sleep(1000);
            Log.CloseAndFlush();
        }

        /// <summary>
        /// Tests the sink with an exception with a right rounded brackets sign.
        /// </summary>
        [TestMethod]
        public void TestExceptionWithRightRoundedBracketSign()
        {
            var logger = new LoggerConfiguration()
                .WriteTo.Telegram(this.telegramBotToken, this.telegramChatId, 1)
                .CreateLogger();

            var exception1 = new Exception(")Something");
            logger.Error(exception1, exception1.Message);

            Thread.Sleep(1000);
            Log.CloseAndFlush();
        }

        /// <summary>
        /// Tests the sink with an exception with a tilde sign.
        /// </summary>
        [TestMethod]
        public void TestExceptionWithTildeSign()
        {
            var logger = new LoggerConfiguration()
                .WriteTo.Telegram(this.telegramBotToken, this.telegramChatId, 1)
                .CreateLogger();

            var exception1 = new Exception("~Something");
            logger.Error(exception1, exception1.Message);

            Thread.Sleep(1000);
            Log.CloseAndFlush();
        }

        /// <summary>
        /// Tests the sink with an exception with a comma sign.
        /// </summary>
        [TestMethod]
        public void TestExceptionWithCommaSign()
        {
            var logger = new LoggerConfiguration()
                .WriteTo.Telegram(this.telegramBotToken, this.telegramChatId, 1)
                .CreateLogger();

            var exception1 = new Exception("`Something");
            logger.Error(exception1, exception1.Message);

            Thread.Sleep(1000);
            Log.CloseAndFlush();
        }

        /// <summary>
        /// Tests the sink with an exception with an arrow sign.
        /// </summary>
        [TestMethod]
        public void TestExceptionWithArrowSign()
        {
            var logger = new LoggerConfiguration()
                .WriteTo.Telegram(this.telegramBotToken, this.telegramChatId, 1)
                .CreateLogger();

            var exception1 = new Exception(">Something");
            logger.Error(exception1, exception1.Message);

            Thread.Sleep(1000);
            Log.CloseAndFlush();
        }

        /// <summary>
        /// Tests the sink with an exception with a cross sign.
        /// </summary>
        [TestMethod]
        public void TestExceptionWithCross()
        {
            var logger = new LoggerConfiguration()
                .WriteTo.Telegram(this.telegramBotToken, this.telegramChatId, 1)
                .CreateLogger();

            var exception1 = new Exception("#Something");
            logger.Error(exception1, exception1.Message);

            Thread.Sleep(1000);
            Log.CloseAndFlush();
        }

        /// <summary>
        /// Tests the sink with an exception with a plus sign.
        /// </summary>
        [TestMethod]
        public void TestExceptionWithPlusSign()
        {
            var logger = new LoggerConfiguration()
                .WriteTo.Telegram(this.telegramBotToken, this.telegramChatId, 1)
                .CreateLogger();

            var exception1 = new Exception("+Something");
            logger.Error(exception1, exception1.Message);

            Thread.Sleep(1000);
            Log.CloseAndFlush();
        }

        /// <summary>
        /// Tests the sink with an exception with a minus sign.
        /// </summary>
        [TestMethod]
        public void TestExceptionWithMinusSign()
        {
            var logger = new LoggerConfiguration()
                .WriteTo.Telegram(this.telegramBotToken, this.telegramChatId, 1)
                .CreateLogger();

            var exception1 = new Exception("-Something");
            logger.Error(exception1, exception1.Message);

            Thread.Sleep(1000);
            Log.CloseAndFlush();
        }

        /// <summary>
        /// Tests the sink with an exception with an equal sign.
        /// </summary>
        [TestMethod]
        public void TestExceptionWithEqualSign()
        {
            var logger = new LoggerConfiguration()
                .WriteTo.Telegram(this.telegramBotToken, this.telegramChatId, 1)
                .CreateLogger();

            var exception1 = new Exception("=Something");
            logger.Error(exception1, exception1.Message);

            Thread.Sleep(1000);
            Log.CloseAndFlush();
        }

        /// <summary>
        /// Tests the sink with an exception with a separator sign.
        /// </summary>
        [TestMethod]
        public void TestExceptionWithSeparatorSign()
        {
            var logger = new LoggerConfiguration()
                .WriteTo.Telegram(this.telegramBotToken, this.telegramChatId, 1)
                .CreateLogger();

            var exception1 = new Exception("|Something");
            logger.Error(exception1, exception1.Message);

            Thread.Sleep(1000);
            Log.CloseAndFlush();
        }

        /// <summary>
        /// Tests the sink with an exception with a left curly brackets sign.
        /// </summary>
        [TestMethod]
        public void TestExceptionWithLeftCurlyBracketSign()
        {
            var logger = new LoggerConfiguration()
                .WriteTo.Telegram(this.telegramBotToken, this.telegramChatId, 1)
                .CreateLogger();

            var exception1 = new Exception("}Something");
            logger.Error(exception1, exception1.Message);

            Thread.Sleep(1000);
            Log.CloseAndFlush();
        }

        /// <summary>
        /// Tests the sink with an exception with a right curly brackets sign.
        /// </summary>
        [TestMethod]
        public void TestExceptionWithRightCurlyBracketSign()
        {
            var logger = new LoggerConfiguration()
                .WriteTo.Telegram(this.telegramBotToken, this.telegramChatId, 1)
                .CreateLogger();

            var exception1 = new Exception("{Something");
            logger.Error(exception1, exception1.Message);

            Thread.Sleep(1000);
            Log.CloseAndFlush();
        }

        /// <summary>
        /// Tests the sink with an exception with a dot sign.
        /// </summary>
        [TestMethod]
        public void TestExceptionWithDotSign()
        {
            var logger = new LoggerConfiguration()
                .WriteTo.Telegram(this.telegramBotToken, this.telegramChatId, 1)
                .CreateLogger();

            var exception1 = new Exception(".Something");
            logger.Error(exception1, exception1.Message);

            Thread.Sleep(1000);
            Log.CloseAndFlush();
        }

        /// <summary>
        /// Tests the sink with an exception with a question sign.
        /// </summary>
        [TestMethod]
        public void TestExceptionWithQuestionSign()
        {
            var logger = new LoggerConfiguration()
                .WriteTo.Telegram(this.telegramBotToken, this.telegramChatId, 1)
                .CreateLogger();

            var exception1 = new Exception("!Something");
            logger.Error(exception1, exception1.Message);

            Thread.Sleep(1000);
            Log.CloseAndFlush();
        }

        /// <summary>
        /// Tests the sink with an exception with a smaller sign.
        /// </summary>
        [TestMethod]
        public void TestExceptionWithSmallerSign()
        {
            var logger = new LoggerConfiguration()
                .WriteTo.Telegram(this.telegramBotToken, this.telegramChatId, 1)
                .CreateLogger();

            var exception1 = new Exception("<Something");
            logger.Error(exception1, exception1.Message);

            Thread.Sleep(1000);
            Log.CloseAndFlush();
        }

        /// <summary>
        /// Tests the sink with an exception with a bigger sign.
        /// </summary>
        [TestMethod]
        public void TestExceptionWithBiggerSign()
        {
            var logger = new LoggerConfiguration()
                .WriteTo.Telegram(this.telegramBotToken, this.telegramChatId, 1)
                .CreateLogger();

            var exception1 = new Exception(">Something");
            logger.Error(exception1, exception1.Message);

            Thread.Sleep(1000);
            Log.CloseAndFlush();
        }

        /// <summary>
        /// Tests the sink with an exception with an and sign.
        /// </summary>
        [TestMethod]
        public void TestExceptionWithAndSign()
        {
            var logger = new LoggerConfiguration()
                .WriteTo.Telegram(this.telegramBotToken, this.telegramChatId, 1)
                .CreateLogger();

            var exception1 = new Exception("&Something");
            logger.Error(exception1, exception1.Message);

            Thread.Sleep(1000);
            Log.CloseAndFlush();
        }

        /// <summary>
        /// Tests the sink with an exception with an issue that occurred in https://github.com/serilog-contrib/Serilog.Sinks.Telegram.Alternative/issues/10.
        /// </summary>
        [TestMethod]
        // ReSharper disable once StyleCop.SA1650
        public void TestIssue10Exception()
        {
            var logger = new LoggerConfiguration()
                .WriteTo.Telegram(this.telegramBotToken, this.telegramChatId, 1)
                .CreateLogger();

            SelfLog.Enable(Console.WriteLine);
            logger.Warning("whatever contains {}");

            Thread.Sleep(1000);
            Log.CloseAndFlush();
        }

        /// <summary>
        /// Tests the sink with not escaped messages according to https://github.com/serilog-contrib/Serilog.Sinks.Telegram.Alternative/issues/11.
        /// </summary>
        [TestMethod]
        // ReSharper disable once StyleCop.SA1650
        public void TestNotEscapedMessage()
        {
            var logger = new LoggerConfiguration()
                .WriteTo.Telegram(this.telegramBotToken, this.telegramChatId, useCustomHtmlFormatting: true)
                .CreateLogger();

            SelfLog.Enable(Console.WriteLine);
            logger.Warning("<b>Some bold message</b>");

            Thread.Sleep(1000);
            Log.CloseAndFlush();
        }

        /// <summary>
        /// Tests the sink with user defined html escaping according to https://github.com/serilog-contrib/Serilog.Sinks.Telegram.Alternative/issues/26.
        /// </summary>
        [TestMethod]
        // ReSharper disable once StyleCop.SA1650
        public void TestCustomHtmlFormatter()
        {
            var logger = new LoggerConfiguration()
                .WriteTo.Telegram(
                    this.telegramBotToken, 
                    this.telegramChatId, 
                    useCustomHtmlFormatting: true, 
                    customHtmlFormatter: (s) => 
                    {
                        return s
                            .Replace("<", "&lt;")
                            .Replace(">", "&gt;")
                            .Replace("&", "&amp;")
                            .Replace("&amp;lt;tg-spoiler&amp;gt;", "<tg-spoiler>")
                            .Replace("&amp;lt;/tg-spoiler&amp;gt;", "</tg-spoiler>");
                    }
                )
                .CreateLogger();

            SelfLog.Enable(Console.WriteLine);
            logger.Warning("Here comes a spoiler: <tg-spoiler>Tadaaa</tg-spoiler>");

            Thread.Sleep(1000);
            Log.CloseAndFlush();
        }
    }
}
