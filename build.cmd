@ECHO OFF
SETLOCAL

dotnet restore VertiPaq-Analyzer.slnf || GOTO :error
dotnet build   VertiPaq-Analyzer.slnf || GOTO :error
dotnet test    VertiPaq-Analyzer.slnf || GOTO :error
dotnet pack    VertiPaq-Analyzer.slnf || GOTO :error

GOTO :EOF
:error
EXIT /b %ERRORLEVEL%