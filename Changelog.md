Change history
--------------

* **Version 1.4.2.0 (2024-05-16)** : Removed support for Net7.0.
* **Version 1.4.1.0 (2024-03-03)**: Updated NuGet packages.
* **Version 1.4.0.0 (2023-11-21)**: Updated NuGet packages, removed support for netstandard, added support for Net8.0.
* **Version 1.3.0.0 (2023-04-07)**: Updated NuGet packages, removed support for NetCore3.1, added message chunking, added topic support.
* **Version 1.2.0.0 (2022-11-20)**: Updated nuget packages, removed support for Net5.0, added support for Net7.0.
* **Version 1.1.10.0 (2022-10-30)** : Updated nuget packages.
* **Version 1.1.9.0 (2022-08-31)** : Updated NuGet packages, exposed HTTP client to allow e.g. proxy support (Thanks to @Azaferany).
* **Version 1.1.8.0 (2022-06-01)** : Updated NuGet packages.
* **Version 1.1.7.0 (2022-04-04)** : Updated NuGet packages.
* **Version 1.1.6.0 (2022-02-16)** : Updated NuGet packages, added nullable checks, added editorconfig, added file scoped namespaces, added global usings, removed native support for Net Framework (Breaking change).
* **Version 1.1.5.0 (2022-01-12)** : NuGet packages updated, added support for better custom HTML formatting (Thanks to @nebula2, see https://github.com/serilog-contrib/Serilog.Sinks.Telegram.Alternative/issues/26).
* **Version 1.1.4.0 (2021-12-07)** : NuGet packages updated, added support to suppress the date time in the output (Thanks to @nebula2).
* **Version 1.1.3.0 (2021-11-18)** : NuGet packages updated, added support for custom bot API urls (Thanks to @azhdari).
* **Version 1.1.2.0 (2021-11-09)** : NuGet packages updated, added support for Net6.0.
* **Version 1.1.1.0 (2021-11-04)** : Updated NuGet packages.
* **Version 1.1.0.0 (2021-09-12)** : Added option `useCustomHtmlFormatting` to allow custom HTML formatting for the message and application name, updated NuGet packages.
* **Version 1.0.17.0 (2021-09-03)** : Updated license to fit the new owning repository, updated readme and so on to fit new package name.
* **Version 1.0.16.0 (2021-08-29)** : Updated nuget packages.
* **Version 1.0.15.0 (2021-08-17)** : Adjusted sink to ignore issues when writing to Selflog (but throw an error in Selflog itself if a formatting issue occurred), added check for Github issue #10 (Thanks to @nklv).
* **Version 1.0.14.0 (2021-08-09)** : Removed support for soon deprecated NetCore 2.1.
* **Version 1.0.13.0 (2021-07-25)** : Updated nuget packages, fixed bugs with HTML escaping.
* **Version 1.0.12.0 (2021-06-04)** : Updated nuget packages.
* **Version 1.0.11.0 (2021-05-10)** : Updated nuget packages, moved to HTML formatting as markdown sometimes does strange things when using special chars.
* **Version 1.0.10.0 (2021-04-21)** : Updated nuget packages, added new test for message that contains a underscore.
* **Version 1.0.9.0 (2021-02-21)** : Updated nuget packages.
* **Version 1.0.8.0 (2021-01-28)** : Updated nuget packages, added option to better configure messages (Thanks to @zoinkydoink).
* **Version 1.0.7.0 (2021-01-04)** : Added failure callback to the sink, updated readme to show the correct use of the minimum level (Thanks to @marce1994), added better test project.
* **Version 1.0.6.0 (2021-01-03)** : Added missing configuration option in the sink.
* **Version 1.0.5.0 (2021-01-03)** : Updated nuget packages, added option to exclude stack traces (Thanks to @jessicah), added support for .Net 5.0.
* **Version 1.0.4.0 (2020-06-05)** : Updated nuget packages, added script to upload packages.
* **Version 1.0.3.0 (2020-06-05)** : Updated nuget packages, adjusted build to Visual Studio, moved changelog to extra file.
* **Version 1.0.2.0 (2020-05-10)** : Updated nuget packages, fixed bug with queueing.
* **Version 1.0.1.0 (2019-11-08)** : Updated nuget packages.
* **Version 1.0.0.1 (2019-06-24)** : Added option to only show from and to dates when the dates are not equal.
* **Version 1.0.0.0 (2019-06-23)** : 1.0 release.