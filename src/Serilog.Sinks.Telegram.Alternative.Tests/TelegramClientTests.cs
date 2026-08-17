// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TelegramClientTests.cs" company="SeppPenner and the Serilog contributors">
// The project is licensed under the MIT license.
// </copyright>
// <summary>
//   A class to test the <see cref="TelegramClient"/> class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Serilog.Sinks.Telegram.Alternative.Tests;

/// <summary>
/// A class to test the <see cref="TelegramClient"/> class. These tests run offline, they talk to
/// <see cref="TelegramApiMock"/> instead of Telegram.
/// </summary>
[TestClass]
public class TelegramClientTests
{
    /// <summary>
    /// Checks whether an empty bot token is rejected and names the parameter it complains about.
    /// </summary>
    [TestMethod]
    public void ConstructorRejectsAnEmptyBotToken()
    {
        using var api = new TelegramApiMock();

        var exception = Assert.ThrowsExactly<ArgumentException>(() => new TelegramClient(api.Client, string.Empty));

        Assert.AreEqual("botToken", exception.ParamName);
    }

    /// <summary>
    /// Checks whether a missing HTTP client is reported with the parameter name in the parameter name and the
    /// explanation in the message. The single argument overload of <see cref="ArgumentNullException"/> takes the
    /// parameter name, so passing the explanation to it used to produce
    /// "Value cannot be null. (Parameter 'The http client mustn't be null.')".
    /// </summary>
    [TestMethod]
    public void ConstructorRejectsAMissingHttpClient()
    {
        var exception = Assert.ThrowsExactly<ArgumentNullException>(() => new TelegramClient(null!, TestDataProvider.BotToken));

        Assert.AreEqual("httpClient", exception.ParamName);
        StringAssert.Contains(exception.Message, "The http client mustn't be null.");
    }

    /// <summary>
    /// Checks whether the client posts to the official bot API when no custom url is configured.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing any asynchronous operation.</returns>
    [TestMethod]
    public async Task PostMessageUsesTheDefaultBotApiUrl()
    {
        using var api = new TelegramApiMock();
        var client = new TelegramClient(api.Client, TestDataProvider.BotToken);

        await client.PostMessage("Message", TestDataProvider.ChatId);

        Assert.AreEqual(1, api.RequestCount);
        Assert.AreEqual(TestDataProvider.DefaultSendMessageUrl, api.LastRequest.Url?.ToString());
    }

    /// <summary>
    /// Checks whether a custom bot API url is used unchanged.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing any asynchronous operation.</returns>
    [TestMethod]
    public async Task PostMessageUsesACustomBotApiUrl()
    {
        using var api = new TelegramApiMock();
        var client = new TelegramClient(api.Client, TestDataProvider.BotToken, "https://telegram.example.com/bot");

        await client.PostMessage("Message", TestDataProvider.ChatId);

        Assert.AreEqual($"https://telegram.example.com/bot{TestDataProvider.BotToken}/sendMessage", api.LastRequest.Url?.ToString());
    }

    /// <summary>
    /// Checks whether a blank bot API url falls back to the official one instead of building a broken url.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing any asynchronous operation.</returns>
    [TestMethod]
    public async Task PostMessageFallsBackToTheDefaultBotApiUrlForABlankOne()
    {
        using var api = new TelegramApiMock();
        var client = new TelegramClient(api.Client, TestDataProvider.BotToken, "   ");

        await client.PostMessage("Message", TestDataProvider.ChatId);

        Assert.AreEqual(TestDataProvider.DefaultSendMessageUrl, api.LastRequest.Url?.ToString());
    }

    /// <summary>
    /// Checks whether the payload carries the chat id, the message, the HTML parse mode and the topic id.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing any asynchronous operation.</returns>
    [TestMethod]
    public async Task PostMessageSendsTheChatIdTheParseModeAndTheTopicId()
    {
        using var api = new TelegramApiMock();
        var client = new TelegramClient(api.Client, TestDataProvider.BotToken);

        await client.PostMessage("Message", TestDataProvider.ChatId, 42);

        var request = api.LastRequest;
        Assert.AreEqual(TestDataProvider.ChatId, request.GetValue("chat_id"));
        Assert.AreEqual("Message", request.Text);
        Assert.AreEqual("HTML", request.GetValue("parse_mode"));
        Assert.AreEqual("42", request.GetValue("message_thread_id"));
    }

    /// <summary>
    /// Checks whether the topic id is sent as a JSON null when there is none. Telegram treats the absent and the
    /// null case the same way, a chat without topics would reject any other value.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing any asynchronous operation.</returns>
    [TestMethod]
    public async Task PostMessageSendsANullTopicIdWhenThereIsNone()
    {
        using var api = new TelegramApiMock();
        var client = new TelegramClient(api.Client, TestDataProvider.BotToken);

        await client.PostMessage("Message", TestDataProvider.ChatId);

        Assert.IsNull(api.LastRequest.GetValue("message_thread_id"));
    }
}
