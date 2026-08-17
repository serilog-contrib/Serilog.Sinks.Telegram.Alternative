@ECHO off
cls
cd .\src

REM Never turn echo on below this line. Cmd echoes a command with its variables already expanded,
REM so an echoed push command puts the NuGet API key in plain text into the console and into every
REM log or screenshot of it.

IF "%NUGET_API_KEY%"=="" (
	ECHO.NUGET_API_KEY is not set, nothing was uploaded.
	PAUSE
	EXIT /B 1
)

FOR /d /r . %%d in (bin,obj) DO (
	IF EXIST "%%d" (
		ECHO %%d | FIND /I "\node_modules\" > Nul && (
			ECHO.Skipping: %%d
		) || (
			ECHO.Deleting: %%d
			rd /s/q "%%d"
		)
	)
)

ECHO.Building solution...
dotnet restore
IF ERRORLEVEL 1 (
	ECHO.Restore failed, nothing was uploaded.
	PAUSE
	EXIT /B 1
)

dotnet build -c Release
IF ERRORLEVEL 1 (
	ECHO.Build failed, nothing was uploaded.
	PAUSE
	EXIT /B 1
)

cd .\Serilog.Sinks.Telegram.Alternative\bin\Release
ECHO.Build successful.

ECHO.Pushing the package...
dotnet nuget push *.nupkg -s "nuget.org" --skip-duplicate -k "%NUGET_API_KEY%"
IF ERRORLEVEL 1 (
	ECHO.Pushing the package failed.
	PAUSE
	EXIT /B 1
)

ECHO.Pushing the symbols...
dotnet nuget push *.snupkg -s "nuget.org" --skip-duplicate -k "%NUGET_API_KEY%"
IF ERRORLEVEL 1 (
	ECHO.Pushing the symbols failed, the package itself is already on nuget.org.
	PAUSE
	EXIT /B 1
)

ECHO.Upload success. Press any key to exit.
PAUSE
