## Basic usage

```csharp
var logger = new LoggerConfiguration()
	.WriteTo.Telegram("botToken", "chatId")
    .CreateLogger();
```

The project can be found on [nuget](https://www.nuget.org/packages/Serilog.Sinks.Telegram.Alternative/).

## Configuration options

|Parameter|Meaning|Example|Default value|
|-|-|-|-|
|botToken|The Telegram bot token.|`"123151488:AAgshf4r373rffsdfOfsdzgfwezfzqwfr7zewE"`|None, is mandatory.|
|chatId|The Telegram chat id.|`"12345"`|None, is mandatory.|
|dateFormat|The date time format showing how the date and time should be formatted. Set this to `null` if you don't wish to output.|`dateFormat: "dd.MM.yyyy HH:mm:ssZ"`|`"dd.MM.yyyy HH:mm:sszzz"`|
|applicationName|The name of the application sending the events in case multiple apps write to the same channel.|`applicationName: "My App"`|`string.Empty`|
|batchSizeLimit|The maximum number of events to include in a single batch.|`batchSizeLimit: 40`|`30`|
|period|The time to wait between checking for event batches.|`period: new TimeSpan(0, 0, 20)`|`00:00:05`|
|formatProvider|The `IFormatProvider` to use. Supplies culture-specific formatting information. Check https://docs.microsoft.com/en-us/dotnet/api/system.iformatprovider?view=netframework-4.8.|`new CultureInfo("de-DE")`|`null`|
|minimumLogEventLevel|The minimum level of the logging.|`minimumLogEventLevel: LogEventLevel.Verbose`|`LogEventLevel.Verbose`|
|sendBatchesAsSingleMessages|A value indicating whether the batches are sent as single messages or as one block of messages.|`false`|`true`|
|includeStackTrace|A value indicating whether the stack trace should be shown or not.|`false`|`true`|
|~~failureCallback~~|~~Adds an option to add a failure callback action.~~  (Deprecated, use fallback logging instead.Check https://nblumhardt.com/2024/10/fallback-logging/.)|~~`failureCallback: e => Console.WriteLine($"Sink error: {e.Message}")`~~|~~`null`~~|
|useCustomHtmlFormatting|An option to allow custom HTML formatting inside the message text and the application name (Use only if really needed). Make sure to use the HTML formatting from https://core.telegram.org/bots/api#html-style.|`true`|`false`|
|botApiUrl|A custom bot API url.|`https://telegram.something.com/bot`|`https://api.telegram.org/bot`|
|outputTemplate|A custom output template following the Serilog standard. Check https://github.com/serilog/serilog/wiki/Formatting-Output.|`"[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}"`|`null`|
|customHtmlFormatter|A custom HTML format function which you can set in addition to `useCustomHtmlFormatting`. See https://github.com/serilog-contrib/Serilog.Sinks.Telegram.Alternative/issues/26.|`(s) => return s.Replace("<", "&lt;").Replace(">", "&gt;").Replace("&", "&amp;").Replace("&amp;lt;tg-spoiler&amp;gt;", "<tg-spoiler>").Replace("&amp;lt;/tg-spoiler&amp;gt;", "</tg-spoiler>");`|`null`|
|topicId|The Telegram topic id.|`12345`|`null`|

## Configuration via JSON options

```json
{
    "Serilog": {
        "Using": [ "Serilog.Sinks.Telegram" ],
        "MinimumLevel": {
            "Default": "Warning"
        },
        "WriteTo": [
            {
                "Name": "Telegram",
                "Args": {
                    "botToken": "123151488:AAgshf4r373rffsdfOfsdzgfwezfzqwfr7zewE",
                    "chatId": "12345",
                    "minimumLogEventLevel": "Warning",
                    "applicationName": "My App",
                    "dateFormat": "yyyy-MM-dd HH:mm:sszzz",
                    "botApiUrl": "https://telegram.something.com/bot"
                }
            }
        ]
    }
}
```

# Further information
This sink is basically the same as https://github.com/oxozle/serilog-sinks-telegram but is maintained and provides badging of events.

You need to get a bot API token following https://core.telegram.org/api#bot-api.