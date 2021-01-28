Serilog.Sinks.Telegram
====================================

Serilog.Sinks.Telegram is a library to save logging information from [Serilog](https://github.com/serilog/serilog) to [Telegram](https://telegram.org/).
The assembly was written and tested in .Net 5.0.

[![Build status](https://ci.appveyor.com/api/projects/status/q160mdmc5la3b4u0?svg=true)](https://ci.appveyor.com/project/SeppPenner/serilog-sinks-telegram)
[![GitHub issues](https://img.shields.io/github/issues/SeppPenner/Serilog.Sinks.Telegram.svg)](https://github.com/SeppPenner/Serilog.Sinks.Telegram/issues)
[![GitHub forks](https://img.shields.io/github/forks/SeppPenner/Serilog.Sinks.Telegram.svg)](https://github.com/SeppPenner/Serilog.Sinks.Telegram/network)
[![GitHub stars](https://img.shields.io/github/stars/SeppPenner/Serilog.Sinks.Telegram.svg)](https://github.com/SeppPenner/Serilog.Sinks.Telegram/stargazers)
[![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg)](https://raw.githubusercontent.com/SeppPenner/Serilog.Sinks.Telegram/master/License.txt)
[![Nuget](https://img.shields.io/badge/Serilog.Sinks.Telegram-Nuget-brightgreen.svg)](https://www.nuget.org/packages/HaemmerElectronics.SeppPenner.Serilog.Sinks.Telegram/)
[![NuGet Downloads](https://img.shields.io/nuget/dt/HaemmerElectronics.SeppPenner.Serilog.Sinks.Telegram.svg)](https://www.nuget.org/packages/HaemmerElectronics.SeppPenner.Serilog.Sinks.Telegram/)
[![Known Vulnerabilities](https://snyk.io/test/github/SeppPenner/Serilog.Sinks.Telegram/badge.svg)](https://snyk.io/test/github/SeppPenner/Serilog.Sinks.Telegram)
[![Gitter](https://badges.gitter.im/Serilog-Sinks-Telegram/community.svg)](https://gitter.im/Serilog-Sinks-Telegram/community?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge)

## Available for
* NetFramework 4.6
* NetFramework 4.6.2
* NetFramework 4.7
* NetFramework 4.7.2
* NetFramework 4.8
* NetStandard 2.0
* NetStandard 2.1
* NetCore 2.1
* NetCore 3.1
* Net 5.0

## Net Core and Net Framework latest and LTS versions
* https://dotnet.microsoft.com/download/dotnet-framework
* https://dotnet.microsoft.com/download/dotnet-core
* https://dotnet.microsoft.com/download/dotnet/5.0

## Basic usage:

```csharp
var logger = new LoggerConfiguration()
	.WriteTo.Telegram("botToken", "chatId")
    .CreateLogger();
```

The project can be found on [nuget](https://www.nuget.org/packages/HaemmerElectronics.SeppPenner.Serilog.Sinks.Telegram/).

## Configuration options:

|Parameter|Meaning|Example|Default value|
|-|-|-|-|
|botToken|The Telegram bot token.|`"123151488:AAgshf4r373rffsdfOfsdzgfwezfzqwfr7zewE"`|None, is mandatory.|
|chatId|The Telegram chat id.|`"12345"`|None, is mandatory.|
|batchSizeLimit|The maximum number of events to include in a single batch.|`batchSizeLimit: 40`|`30`|
|period|The time to wait between checking for event batches.|`period: new TimeSpan(0, 0, 20)`|`00:00:05`|
|formatProvider|The `IFormatProvider` to use. Supplies culture-specific formatting information. Check https://docs.microsoft.com/en-us/dotnet/api/system.iformatprovider?view=netframework-4.8.|`new CultureInfo("de-DE")`|`null`|
|restrictedToMinimumLevel|The minimum level of the logging.|`restrictedToMinimumLevel: LogEventLevel.Verbose`|`LogEventLevel.Verbose`|
|sendBatchesAsSingleMessages|A value indicating whether the batches are sent as single messages or as one block of messages.|`false`|`true`|
|includeStackTrace|A value indicating whether the stack trace should be shown or not.|`false`|`true`|
|failureCallback|Adds an option to add a failure callback action.|`failureCallback: e => Console.WriteLine($"Sink error: {e.Message}")`|`null`|
|dateFormat|The date time format showing how the date and time should be formatted.|`dateFormat: "dd.MM.yyyy HH:mm:ssZ"`|`"dd.MM.yyyy HH:mm:sszzz"`|
|applicationName|The name of the application sending the events in case multiple apps write to the same channel.|`applicationName: "My App"`|`string.Empty`|

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
                    "restrictedToMinimumLevel": "Warning",
                    "applicationName": "My App",
                    "dateFormat": "yyyy-MM-dd HH:mm:sszzz"
                }
            }
        ]
    }
}
```

# Further information
This sink is basically the same as https://github.com/oxozle/serilog-sinks-telegram but is maintained and provides badging of events.

You need to get a bot API token following https://core.telegram.org/api#bot-api.

Change history
--------------

See the [Changelog](https://github.com/SeppPenner/Serilog.Sinks.Telegram/blob/master/Changelog.md).
