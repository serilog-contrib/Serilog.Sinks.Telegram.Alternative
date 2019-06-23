dotnet build Serilog.Sinks.Telegram.sln -c Release
xcopy /s .\Serilog.Sinks.Telegram\bin\Release ..\Nuget\Source\
xcopy /s .\Help ..\Nuget\Documentation\
pause