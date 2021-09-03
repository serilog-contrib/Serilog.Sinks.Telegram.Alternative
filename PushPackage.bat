dotnet nuget push "src\Serilog.Sinks.Telegram\bin\Release\Serilog.Sinks.Telegram.*.nupkg" -s "github" --skip-duplicate
dotnet nuget push "src\Serilog.Sinks.Telegram\bin\Release\Serilog.Sinks.Telegram.*.nupkg" -s "nuget.org" --skip-duplicate -k "%NUGET_API_KEY%"
PAUSE