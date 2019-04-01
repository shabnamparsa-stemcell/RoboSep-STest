@echo off
copy /y C:\Projects\TESLA\SOURCE\console\OperatorConsoleControls\bin\Debug\OperatorConsoleControls.dll C:\Projects\TESLA\SOURCE\separator\bin\Debug\
if errorlevel 1 goto CSharpReportError
goto CSharpEnd
:CSharpReportError
echo Project error: A tool returned an error code from the build event
exit 1
:CSharpEnd