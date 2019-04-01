@echo off
copy /y C:\Projects\TESLA\SOURCE\console\OperatorConsoleControls\bin\Release\OperatorConsoleControls.dll C:\Projects\TESLA\SOURCE\separator\bin\Release\
if errorlevel 1 goto CSharpReportError
goto CSharpEnd
:CSharpReportError
echo Project error: A tool returned an error code from the build event
exit 1
:CSharpEnd