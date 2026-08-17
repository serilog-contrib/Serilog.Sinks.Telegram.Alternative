// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OutputTemplateTests.cs" company="SeppPenner and the Serilog contributors">
// The project is licensed under the MIT license.
// </copyright>
// <summary>
//   A class to test the renderers behind the outputTemplate option.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Serilog.Sinks.Telegram.Alternative.Tests;

/// <summary>
/// A class to test the renderers behind the outputTemplate option. They are internal, so the tests drive them
/// the way a user does, through a configured sink, and read the message out of the recorded payload.
/// </summary>
[TestClass]
public class OutputTemplateTests
{
    /// <summary>
    /// Renders one log event through the given output template and returns the message that reached the API.
    /// </summary>
    /// <param name="outputTemplate">The output template.</param>
    /// <param name="write">The action that writes the log event.</param>
    /// <returns>The rendered message as <see cref="string"/>.</returns>
    private static string Render(string outputTemplate, Action<ILogger> write)
    {
        using var api = new TelegramApiMock();
        var options = TestDataProvider.CreateOptions(api, outputTemplate: outputTemplate);

        using (var logger = new LoggerConfiguration().WriteTo.Telegram(options).CreateLogger())
        {
            write(logger);
        }

        return api.LastRequest.Text;
    }

    /// <summary>
    /// Checks whether the text and the properties of a message template keep their order. The renderer used to
    /// write the text tokens straight to the output and to collect the properties in a second writer that was
    /// appended afterwards, so "Hello {Name}!" came out as "Hello !\"World\"".
    /// </summary>
    [TestMethod]
    public void TheMessageKeepsTheOrderOfTextAndProperties()
    {
        var text = Render("{Message}", logger => logger.Information("Hello {Name}!", "World"));

        StringAssert.Contains(text, "Hello \"World\"!");
    }

    /// <summary>
    /// Checks whether a property value is escaped once, not twice, and that the surrounding text is escaped too.
    /// </summary>
    [TestMethod]
    public void TheMessageEscapesTextAndPropertiesExactlyOnce()
    {
        var text = Render("{Message}", logger => logger.Information("<i>{Value}</i>", "<b>"));

        StringAssert.Contains(text, "&lt;i&gt;");
        StringAssert.Contains(text, "&lt;b&gt;");
        Assert.IsFalse(text.Contains("&amp;lt;"), $"The message was escaped twice: '{text}'.");
    }

    /// <summary>
    /// Checks whether the level is rendered in upper case with the "u" format.
    /// </summary>
    [TestMethod]
    public void TheLevelIsRenderedInUpperCase()
    {
        var text = Render("{Level:u} {Message}", logger => logger.Warning("A message"));

        StringAssert.StartsWith(text, "WARNING A message");
    }

    /// <summary>
    /// Checks whether the level is rendered in lower case with the "l" format.
    /// </summary>
    [TestMethod]
    public void TheLevelIsRenderedInLowerCase()
    {
        var text = Render("{Level:l} {Message}", logger => logger.Warning("A message"));

        StringAssert.StartsWith(text, "warning A message");
    }

    /// <summary>
    /// Checks whether the level is rendered as an emoji with the "e" format. The expected value is written as an
    /// escape sequence so that the test still means the warning sign if the encoding of this file is ever
    /// mangled.
    /// </summary>
    [TestMethod]
    public void TheLevelIsRenderedAsAnEmoji()
    {
        var text = Render("{Level:e}", logger => logger.Warning("A message"));

        Assert.AreEqual("\u26A0", text);
    }

    /// <summary>
    /// Checks whether the timestamp honours the format of the token.
    /// </summary>
    [TestMethod]
    public void TheTimestampHonoursItsFormat()
    {
        var text = Render("{Timestamp:yyyy}", logger => logger.Information("A message"));

        Assert.AreEqual(DateTimeOffset.Now.Year.ToString(), text);
    }

    /// <summary>
    /// Checks whether an exception is rendered with its message, its type and its stack trace.
    /// </summary>
    [TestMethod]
    public void TheExceptionIsRendered()
    {
        var text = Render("{Exception}", logger => logger.Error(new InvalidOperationException("Boom"), "It went wrong"));

        StringAssert.Contains(text, "Message: <code>Boom</code>");
        StringAssert.Contains(text, "Type: <code>InvalidOperationException</code>");
    }

    /// <summary>
    /// Checks whether the exception token renders nothing at all when the event carries no exception.
    /// </summary>
    [TestMethod]
    public void TheExceptionTokenRendersNothingWithoutAnException()
    {
        var text = Render("{Exception}", logger => logger.Information("A message"));

        Assert.AreEqual(string.Empty, text);
    }

    /// <summary>
    /// Checks whether a property that the log event does not carry is written as its own token. That is the
    /// fallback of the default renderer, it does not throw and it does not write an empty string.
    /// </summary>
    [TestMethod]
    public void AnUnknownPropertyIsRenderedAsItsToken()
    {
        var text = Render("{Unknown}", logger => logger.Information("A message"));

        Assert.AreEqual("{Unknown}", text);
    }

    /// <summary>
    /// Checks whether the application name that the sink adds to every event is available to the template.
    /// </summary>
    [TestMethod]
    public void TheApplicationNameIsAvailableAsAProperty()
    {
        using var api = new TelegramApiMock();
        var options = TestDataProvider.CreateOptions(api, applicationName: "My App", outputTemplate: "{ApplicationName:l}");

        using (var logger = new LoggerConfiguration().WriteTo.Telegram(options).CreateLogger())
        {
            logger.Information("A message");
        }

        Assert.AreEqual("My App", api.LastRequest.Text);
    }

    /// <summary>
    /// Checks whether the new line token writes a line break.
    /// </summary>
    [TestMethod]
    public void TheNewLineTokenWritesALineBreak()
    {
        var text = Render("A{NewLine}B", logger => logger.Information("A message"));

        Assert.AreEqual($"A{Environment.NewLine}B", text);
    }
}
