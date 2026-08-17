// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TelegramSinkTests.cs" company="SeppPenner and the Serilog contributors">
// The project is licensed under the MIT license.
// </copyright>
// <summary>
//   A class to test the <see cref="TelegramSink"/> class without talking to Telegram.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Serilog.Sinks.Telegram.Alternative.Tests;

/// <summary>
/// A class to test the <see cref="TelegramSink"/> class without talking to Telegram. Every test disposes its
/// logger before it asserts, that is what flushes the batching sink, a sleep would only make the test slow and
/// flaky.
/// </summary>
[TestClass]
public class TelegramSinkTests
{
    /// <summary>
    /// Checks whether a single log event produces a single request to the bot API.
    /// </summary>
    [TestMethod]
    public void OneEventProducesOneRequest()
    {
        using var api = new TelegramApiMock();

        using (var logger = new LoggerConfiguration().WriteTo.Telegram(TestDataProvider.CreateOptions(api)).CreateLogger())
        {
            logger.Information("A message");
        }

        Assert.AreEqual(1, api.RequestCount);
        Assert.AreEqual(TestDataProvider.DefaultSendMessageUrl, api.LastRequest.Url?.ToString());
        StringAssert.Contains(api.LastRequest.Text, "A message");
    }

    /// <summary>
    /// Checks whether the angle brackets and the ampersand are escaped exactly once. Escaping the ampersand last
    /// escaped the ampersands of the entities written before it, so a single "&lt;" arrived as the literal text
    /// "&amp;lt;" in the chat.
    /// </summary>
    [TestMethod]
    public void TheMessageIsEscapedExactlyOnce()
    {
        using var api = new TelegramApiMock();

        using (var logger = new LoggerConfiguration().WriteTo.Telegram(TestDataProvider.CreateOptions(api)).CreateLogger())
        {
            logger.Information("<b>bold</b> & more");
        }

        var text = api.LastRequest.Text;
        StringAssert.Contains(text, "&lt;b&gt;bold&lt;/b&gt; &amp; more");
        Assert.IsFalse(text.Contains("&amp;lt;"), $"The message was escaped twice: '{text}'.");
    }

    /// <summary>
    /// Checks whether custom HTML survives when the caller asked for it.
    /// </summary>
    [TestMethod]
    public void CustomHtmlFormattingLeavesTheMarkupAlone()
    {
        using var api = new TelegramApiMock();
        var options = TestDataProvider.CreateOptions(api, useCustomHtmlFormatting: true);

        using (var logger = new LoggerConfiguration().WriteTo.Telegram(options).CreateLogger())
        {
            logger.Information("<b>bold</b>");
        }

        StringAssert.Contains(api.LastRequest.Text, "<b>bold</b>");
    }

    /// <summary>
    /// Checks whether a custom HTML formatter replaces the built in escaping.
    /// </summary>
    [TestMethod]
    public void ACustomHtmlFormatterReplacesTheBuiltInEscaping()
    {
        using var api = new TelegramApiMock();
        var options = TestDataProvider.CreateOptions(
            api,
            useCustomHtmlFormatting: true,
            customHtmlFormatter: value => value.Replace("secret", "<tg-spoiler>secret</tg-spoiler>"));

        using (var logger = new LoggerConfiguration().WriteTo.Telegram(options).CreateLogger())
        {
            logger.Information("A secret message");
        }

        StringAssert.Contains(api.LastRequest.Text, "A <tg-spoiler>secret</tg-spoiler> message");
    }

    /// <summary>
    /// Checks whether a message that exceeds <see cref="TelegramSink.MaxMessageLength"/> is dropped. Splitting
    /// such a message was removed in version 1.4.3.0 because it tears HTML tags apart.
    /// </summary>
    [TestMethod]
    public void AMessageAboveTheLimitIsDropped()
    {
        using var api = new TelegramApiMock();

        using (var logger = new LoggerConfiguration().WriteTo.Telegram(TestDataProvider.CreateOptions(api)).CreateLogger())
        {
            logger.Information(new string('a', TelegramSink.MaxMessageLength));
        }

        Assert.AreEqual(0, api.RequestCount);
    }

    /// <summary>
    /// Checks whether the application name and the date are rendered into the italic line below the message.
    /// </summary>
    [TestMethod]
    public void TheApplicationNameAndTheDateAreRendered()
    {
        using var api = new TelegramApiMock();
        var options = TestDataProvider.CreateOptions(api, TestDataProvider.DateFormat, "My App");

        using (var logger = new LoggerConfiguration().WriteTo.Telegram(options).CreateLogger())
        {
            logger.Information("A message");
        }

        StringAssert.Contains(api.LastRequest.Text, "<i>My App: The message occurred on ");
    }

    /// <summary>
    /// Checks whether the topic id reaches the payload.
    /// </summary>
    [TestMethod]
    public void TheTopicIdReachesThePayload()
    {
        using var api = new TelegramApiMock();
        var options = TestDataProvider.CreateOptions(api, topicId: 42);

        using (var logger = new LoggerConfiguration().WriteTo.Telegram(options).CreateLogger())
        {
            logger.Information("A message");
        }

        Assert.AreEqual("42", api.LastRequest.GetValue("message_thread_id"));
    }

    /// <summary>
    /// Checks whether a batch becomes one request holding all messages when the caller does not want single
    /// messages.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing any asynchronous operation.</returns>
    [TestMethod]
    public async Task ABatchBecomesOneRequestWhenSingleMessagesAreOff()
    {
        using var api = new TelegramApiMock();
        var sink = new TelegramSink(TestDataProvider.CreateOptions(api, sendBatchesAsSingleMessages: false));

        await sink.EmitBatchAsync(
        [
            TestDataProvider.CreateLogEvent("First"),
            TestDataProvider.CreateLogEvent("Second"),
            TestDataProvider.CreateLogEvent("Third")
        ]);

        Assert.AreEqual(1, api.RequestCount);
        var text = api.LastRequest.Text;
        StringAssert.Contains(text, "First");
        StringAssert.Contains(text, "Second");
        StringAssert.Contains(text, "Third");
    }

    /// <summary>
    /// Checks whether a batch becomes one request per event by default.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing any asynchronous operation.</returns>
    [TestMethod]
    public async Task ABatchBecomesOneRequestPerEventByDefault()
    {
        using var api = new TelegramApiMock();
        var sink = new TelegramSink(TestDataProvider.CreateOptions(api));

        await sink.EmitBatchAsync(
        [
            TestDataProvider.CreateLogEvent("First"),
            TestDataProvider.CreateLogEvent("Second")
        ]);

        Assert.AreEqual(2, api.RequestCount);
    }

    /// <summary>
    /// Checks whether the sink applies its own minimum level on top of the one Serilog already applied. The
    /// convenience extension method passes the same value into both, a caller who builds the options by hand can
    /// set them apart, which is what this test does.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing any asynchronous operation.</returns>
    [TestMethod]
    public async Task TheSinkAppliesItsOwnMinimumLevel()
    {
        using var api = new TelegramApiMock();
        var sink = new TelegramSink(TestDataProvider.CreateOptions(api, minimumLogEventLevel: LogEventLevel.Error));

        await sink.EmitBatchAsync(
        [
            TestDataProvider.CreateLogEvent("Dropped by the sink"),
            TestDataProvider.CreateLogEvent("Kept by the sink", LogEventLevel.Error)
        ]);

        Assert.AreEqual(1, api.RequestCount);
        StringAssert.Contains(api.LastRequest.Text, "Kept by the sink");
    }

    /// <summary>
    /// Checks whether two events carrying the same exception message are merged into one message. The comparison
    /// looks at the message text of the exception only, not at its type.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing any asynchronous operation.</returns>
    [TestMethod]
    public async Task RepeatedExceptionsAreMergedInsideOneBatch()
    {
        using var api = new TelegramApiMock();
        var sink = new TelegramSink(TestDataProvider.CreateOptions(api, TestDataProvider.DateFormat));

        await sink.EmitBatchAsync(
        [
            TestDataProvider.CreateLogEvent("First", LogEventLevel.Error, new InvalidOperationException("Boom")),
            TestDataProvider.CreateLogEvent("Second", LogEventLevel.Error, new ArgumentException("Boom"), 60)
        ]);

        Assert.AreEqual(1, api.RequestCount);
        var text = api.LastRequest.Text;
        StringAssert.Contains(text, "First");
        StringAssert.Contains(text, "The message occurred first on ");
        Assert.IsFalse(text.Contains("Second"), $"The second event was not merged away: '{text}'.");
    }

    /// <summary>
    /// Checks whether the exception type and the stack trace are part of the message when a stack trace exists.
    /// </summary>
    [TestMethod]
    public void TheExceptionTypeAndTheStackTraceAreRendered()
    {
        using var api = new TelegramApiMock();

        using (var logger = new LoggerConfiguration().WriteTo.Telegram(TestDataProvider.CreateOptions(api)).CreateLogger())
        {
            logger.Error(new InvalidOperationException("Boom"), "It went wrong");
        }

        var text = api.LastRequest.Text;
        StringAssert.Contains(text, "Message: <code>Boom</code>");
        StringAssert.Contains(text, "Type: <code>InvalidOperationException</code>");
        StringAssert.Contains(text, "Stack Trace");
    }
}
