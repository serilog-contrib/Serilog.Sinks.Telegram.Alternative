dotnet build Serilog.Sinks.Telegram.sln -c Release
xcopy /s .\Serilog.Sinks.Telegram\bin\Release ..\Nuget\Source\
pause