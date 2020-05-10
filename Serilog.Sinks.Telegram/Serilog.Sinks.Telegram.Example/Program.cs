// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="Hämmer Electronics">
// The project is licensed under the MIT license.
// </copyright>
// <summary>
//   A class to test the <see cref="TelegramSink" />.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Serilog.Sinks.Telegram.Example
{
    using System;
    using System.Threading;

    /// <summary>
    /// A class to test the <see cref="TelegramSink"/>.
    /// </summary>
    public class Program
    {
        /// <summary>
        /// Defines the entry point of the application.
        /// </summary>
        /// <param name="args">The arguments.</param>
        public static void Main(string[] args)
        {
            Console.WriteLine("Testing Serilog.Sinks.Telegram");
            var log = new LoggerConfiguration()
                .WriteTo.Telegram("123151488:AAgshf4r373rffsdfOfsdzgfwezfzqwfr7zewE", "12345", 3)
                .CreateLogger();

            TestDifferentExceptions(log);
            TestSameExceptions(log);
            Console.WriteLine("Done");
            Console.ReadLine();
        }

        /// <summary>
        /// Tests the <see cref="TelegramSink"/> with different exceptions.
        /// </summary>
        /// <param name="log">The log.</param>
        private static void TestDifferentExceptions(ILogger log)
        {
            var exception1 = new Exception("test1");
            log.Error(exception1, exception1.Message);
            var exception2 = new Exception("test2");
            log.Error(exception2, exception2.Message);
            var exception3 = new Exception("test3");
            log.Error(exception3, exception3.Message);
        }

        /// <summary>
        /// Tests the <see cref="TelegramSink"/> with the same exceptions.
        /// </summary>
        /// <param name="log">The log.</param>
        private static void TestSameExceptions(ILogger log)
        {
            var exception1 = new Exception("test1");
            log.Error(exception1, exception1.Message);
            Thread.Sleep(1000);
            log.Error(exception1, exception1.Message);
            Thread.Sleep(1000);
            log.Error(exception1, exception1.Message);
        }
    }
}
