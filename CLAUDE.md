# Project rules for Claude

## What this is

Serilog.Sinks.Telegram.Alternative is a Serilog sink. It takes log events, renders them as HTML and
posts them to a Telegram chat through the bot API (`sendMessage`). The repository is published as
the NuGet package
[Serilog.Sinks.Telegram.Alternative](https://www.nuget.org/packages/Serilog.Sinks.Telegram.Alternative/),
that package is the only deliverable. There is no application, no installer and no CI configuration
in this repository, no `.github` folder either.

One solution `src/Serilog.Sinks.Telegram.Alternative.sln` with exactly two projects:

- `src/Serilog.Sinks.Telegram.Alternative/Serilog.Sinks.Telegram.Alternative.csproj`, the library,
  `GeneratePackageOnBuild` is on.
- `src/Serilog.Sinks.Telegram.Alternative.Tests/Serilog.Sinks.Telegram.Alternative.Tests.csproj`,
  MSTest.

Layout inside `src/Serilog.Sinks.Telegram.Alternative`:

- `LoggerConfigurationTelegramExtensions.cs`: the public entry point, two `Telegram` extension
  methods on `LoggerSinkConfiguration`. One takes the individual arguments, the other a prepared
  `TelegramSinkOptions`. The first builds the options and calls the second, so new options are
  added in both places plus in `TelegramSinkOptions`.
- `Sinks/Telegram/Alternative/TelegramSinkOptions.cs`: every configuration value, all read only
  except `OutputTemplate`, `HttpClient` and `TopicId`.
- `Sinks/Telegram/Alternative/TelegramSink.cs`: the `IBatchedLogEventSink`. Groups a batch, renders
  it and posts it. Holds `MaxMessageLength`.
- `Sinks/Telegram/Alternative/TelegramClient.cs`: builds the API url and posts the JSON payload.
- `Sinks/Telegram/Alternative/ExtendedLogEvent.cs`: a log event plus first and last occurrence,
  used for the deduplication of repeated exceptions.
- `Sinks/Telegram/Alternative/HtmlEscaper.cs` and `StringExtensions.cs`: the escaping. Everything
  that ends up in a message goes through `HtmlEscaper.Escape`, never through
  `string.HtmlEscape` directly, because only the escaper honours a custom formatter.
- `Sinks/Telegram/Alternative/TelegramPropertyNames.cs`: the one property name the sink adds
  itself, `ApplicationName`.
- `Sinks/Telegram/Alternative/Output/*`: the renderers used when an `outputTemplate` is configured,
  one per token type plus `OutputTemplateRenderer` which parses the template once in its
  constructor and turns it into an array of render actions.
- `GlobalUsings.cs`: all usings of the project.

Namespaces do not follow the folders here, and that is deliberate, it is the public API:

- `LoggerConfigurationTelegramExtensions` sits in namespace `Serilog` so that `WriteTo.Telegram`
  works without an extra using. `RootNamespace` is `Serilog` for the same reason.
- Everything under `Sinks/Telegram/Alternative` is namespace `Serilog.Sinks.Telegram.Alternative`.
- Everything under `Sinks/Telegram/Alternative/Output` is namespace `Serilog.Sinks.Telegram.Output`,
  **without** `Alternative`. Folder and namespace disagree, renaming the namespace would be a
  breaking change for anyone implementing `IPropertyRenderer`.

Repository root: `README.md` (badges, target frameworks, links), `HowToUse.md` (the actual user
documentation with the option table and the JSON configuration sample), `Changelog.md`,
`Updating.md` (five lines describing the release), `License.txt` (MIT), `Icon.png`,
`BuildAndPushPackage.bat`, `Delete-BIN-OBJ-Folders.bat` and `.all-contributorsrc`. `Icon.png`,
`License.txt`, `README.md` and `Changelog.md` are packed into the nupkg from the root, they are not
copies inside `src`.

## Build

```powershell
dotnet build src/Serilog.Sinks.Telegram.Alternative.sln -c Release
```

```powershell
dotnet test src/Serilog.Sinks.Telegram.Alternative.sln
```

- The library multi-targets `net8.0;net9.0;net10.0`, the test project is `net10.0` only. Nothing in
  the code is platform specific.
- `src/Directory.Build.props` sets exactly one property, `GenerateDocumentationFile`. Everything
  else lives in the two `.csproj` files and is duplicated there.
- `TreatWarningsAsErrors` is enabled in both projects, so every warning breaks the build, NuGet
  warnings (`NU****`) from restore included. A clean build reports zero warnings, keep it that way.
  Because `GenerateDocumentationFile` is on, a missing XML doc comment on a public member is such a
  warning and therefore an error.
- `NU1803` (HTTP source usage during restore) is the one warning suppressed via `NoWarn`. Fix
  warnings instead of extending that list. `NuGetAudit` and `NuGetAuditMode=all` are on, so a
  vulnerable transitive package fails the build too.
- Versions come from GitVersion.MsBuild out of the git tags, for example `1.5.1-1` for the first
  commit after tag `1.5.0`. Never edit a version property or an assembly version by hand.
- Restore needs nuget.org. If a private feed is configured globally on the machine and refuses the
  connection, restore fails with `NU1301` and, because of `TreatWarningsAsErrors`, additionally
  with `NU1900`. Then build with an explicit source:
  `dotnet build src/Serilog.Sinks.Telegram.Alternative.sln --source https://api.nuget.org/v3/index.json`.
- `GeneratePackageOnBuild` is on, so **every** Release build writes a `.nupkg` and a `.snupkg` into
  `src/Serilog.Sinks.Telegram.Alternative/bin/Release`. Building is harmless, pushing is not, see
  Releasing.
- The test project holds two kinds of tests and they behave very differently:
  - `HtmlEscapingTests`, `OutputTemplateRendererTests`, `TelegramClientTests` and
    `TelegramSinkTests` run offline. They use `RichardSzalay.MockHttp` for everything that would
    otherwise leave the machine and they assert. New tests belong here.
  - `TestSink` is the old test class. Its methods read the environment variables `TelegramBotToken`
    and `TelegramChatId`, post to a real chat and assert nothing, they are eyeball tests. Without
    those variables the sink options constructor throws and the tests fail. Do not add to this
    class and never present a run of it as evidence that anything works.
- Never claim a test run happened without running it.

## Code conventions

Follow the surrounding code, it is consistent throughout every file:

- File header comment block with
  `<copyright file="..." company="SeppPenner and the Serilog contributors">` and a `<summary>`,
  then the file-scoped namespace. The only exception is the test class `TestSink`, whose header
  still says `Hämmer Electronics`.
- XML doc comments on every type and every member, private members and constants included, no
  exceptions. Implementations of `IPropertyRenderer` carry `<inheritdoc cref="IPropertyRenderer"/>`.
- `Nullable`, `ImplicitUsings` and `LangVersion latest` are enabled.
- New `using` directives go into the `GlobalUsings.cs` of the respective project, inside the
  existing `#pragma warning disable IDE0065` block, never at the top of a file. The editorconfig
  requires usings inside the namespace (`csharp_using_directive_placement=inside_namespace:warning`),
  which global usings cannot satisfy, that is what the pragma is for. Do not add other pragmas. The
  comment text in that block is German because Visual Studio generated it, leave it alone.
- Fields, properties, methods and events are always accessed with `this.` qualification
  (`dotnet_style_qualification_for_*` at severity `warning`).
- `src/.editorconfig` also enforces braces everywhere, no multiple blank lines, four spaces, CRLF,
  UTF-8, file scoped namespaces, `System` usings sorted first and `IDE0005` as warning. Analyzer
  warnings are fixed, not silenced.
- Anything that reaches a message goes through `HtmlEscaper.Escape(options, text)`. Writing an
  unescaped user string into the output is the bug class this sink keeps running into, see the
  linked issues in `Changelog.md`.

## Known quirks

Do not silently "clean up" these, they are existing behaviour:

- **Messages longer than 4096 characters are dropped, not split.** `TelegramSink.SendMessage`
  writes a line to `SelfLog` and returns. Splitting existed until version 1.4.3.0 and was removed
  because it tears HTML tags apart, see issue 33. `MaxMessageLength` is public because the tests
  use it.
- **The minimum level is checked twice.** Serilog filters by `restrictedToMinimumLevel` before the
  sink sees anything, and `EmitBatchAsync` filters again by `options.MinimumLogEventLevel`. The
  convenience extension method passes the same value into both, a caller who builds
  `TelegramSinkOptions` by hand can set them apart.
- **`ApplicationName` is added to every event before the level check**, so an event that the sink
  then discards has still been mutated. `LogEvent.AddPropertyIfAbsent` mutates the event that other
  sinks in the same pipeline also see.
- **Repeated exceptions are merged inside one batch.** Two events whose `Exception.Message` is
  equal become one `ExtendedLogEvent` with a first and a last occurrence. The comparison is the
  message text only, not the type and not the stack trace. With the default `batchSizeLimit` of 1
  a batch never holds two events, so this never triggers unless the caller raises the limit.
- **The merge uses `else if`.** A later event updates either `FirstOccurrence` or `LastOccurrence`,
  never both, and an event that lies inside the existing window updates neither.
- **`ExtendedLogEvent` loses the time zone.** Its properties are `DateTimeOffset`, its constructor
  takes `DateTime`, and the sink passes `logEvent.Timestamp.DateTime`. The offset therefore becomes
  the local one of the machine, whatever `dateFormat` says with `zzz`.
- **The exception message is printed twice**, once as `<strong>` and once as `Message: <code>`,
  both in `TelegramSink.RenderMessage` and in `ExceptionRenderer`. That is the shipped message
  layout, users recognise it.
- **`TelegramSinkOptions.HttpClient` defaults to a fresh `HttpClient` per options object** and is
  never disposed. That is the documented way to plug in a proxy or a mock, see issue 30.
  `TelegramSink.SendMessage` builds a new `TelegramClient` for every single message, but that
  client only wraps the shared `HttpClient`, so no socket is leaked.
- **`customHtmlFormatter` is only stored when `useCustomHtmlFormatting` is true.** Passing the
  formatter alone silently does nothing. `ShouldEscape` is nothing but the negation of
  `UseCustomHtmlFormatting`.
- **`System.Net.Http` is gone and must not come back.** The package was a legacy shim, on `net8.0`
  and newer the types come from the shared framework. Adding the target `net10.0` turned it into
  `NU1510` ("PackageReference is not trimmed, consider removing"), and warnings are errors here, so
  it was removed in version 1.6.0.0. That shortened the dependency list of the published package.
- **The remotes do not match the project url.** `origin` is
  `https://github.com/SeppPenner/Serilog.Sinks.Telegram`, GitHub redirects it to
  `serilog-contrib/Serilog.Sinks.Telegram.Alternative`, which is the url in the `.csproj` and in
  every badge. There is a second remote `jessicah-origin` pointing at the fork the sink originally
  came from.
- **`README.md` below `## Contributors` and `.all-contributorsrc` are generated** by the
  all-contributors bot. Edit the part above the contributors table only.
- **`TestSink.TestExceptionWithArrowSign` and `TestSink.TestExceptionWithBiggerSign` are the same
  test**, both log `>Something`, and `TestExceptionWithCommaSign` logs a backtick, not a comma.
  Legacy names, they are not worth a rename that would break nothing but also help nobody.
- **`src/Serilog.Sinks.Telegram.Alternative.sln.DotSettings`** is tracked and holds nothing but a
  ReSharper user dictionary. Leave it alone.

## Releasing

1. Make the change.
2. Add an entry at the top of `Changelog.md` in the existing format:
   `* **Version 1.6.0.0 (2026-08-17)** : Short description.`
3. Copy the same sentence into `PackageReleaseNotes` in
   `src/Serilog.Sinks.Telegram.Alternative/Serilog.Sinks.Telegram.Alternative.csproj`. The property
   holds only the newest entry, not a history.
4. If the set of target frameworks changed, update the `## Available for` list in `README.md`. If
   an option changed, update the table in `HowToUse.md`.
5. Commit that, then tag the commit with the plain version number, no `v` prefix (`1.5.0`, `1.4.3`,
   ...). The existing tags are lightweight tags, create new ones the same way.
6. Push the commits and the tag. The tag has to be pushed before the package is built, GitVersion
   turns it into the assembly version and an untagged commit produces a prerelease version like
   `1.6.1-1+Branch.master.Sha...` inside the nupkg.
7. Only then, and only when explicitly asked to publish, run `BuildAndPushPackage.bat`. It deletes
   every `bin` and `obj` below `src`, builds Release and pushes both the `.nupkg` and the `.snupkg`
   to nuget.org with `%NUGET_API_KEY%`. A push to nuget.org cannot be taken back, the version is
   burned even after unlisting.

The version in `Changelog.md` and in `PackageReleaseNotes` has four parts (`1.6.0.0`), the tag has
three (`1.6.0`).

## Git

- **Never amend a commit.** No `git commit --amend`, not for a typo in the message, not to add a
  forgotten file, not even when the commit is still local. Write a follow-up commit instead. The
  release versions come from tags on exact commits, an amended commit leaves its tag pointing at a
  commit that no longer exists in the branch.
- **Never `git add -A` or `git add .` in this repository.** Working notes live untracked in the
  repository root and in `tim_local/`, only the second one is ignored. Stage the files you changed
  by name.

## Writing style

- Commit messages are written **in English only**: short, precise subject line, explanatory body
  when needed.
- Code comments and comments in project files such as `.csproj` are **always English**, regardless
  of the language used in the conversation.
- **No em dashes or en dashes** (`—`, `–`), neither in prose, commit messages, code comments nor
  documentation. Use a regular hyphen, comma, colon, parentheses or a separate sentence.
- German texts (documentation, chat replies) always use real umlauts and ß, never ASCII
  transliterations such as `ae`, `oe`, `ue` or `ss`. Identifiers, file names and configuration keys
  stay unchanged where umlauts are technically undesirable.
