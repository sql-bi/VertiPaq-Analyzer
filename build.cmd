@ECHO OFF
SETLOCAL

dotnet restore VertiPaq-Analyzer.sln || GOTO :error
dotnet build   VertiPaq-Analyzer.sln || GOTO :error
dotnet test    VertiPaq-Analyzer.sln || GOTO :error
dotnet pack    VertiPaq-Analyzer.sln || GOTO :error

GOTO :EOF
:error
EXIT /b %ERRORLEVEL%