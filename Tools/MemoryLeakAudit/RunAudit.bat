@echo off
echo ========================================
echo Memory Leak Audit Tool for Skua Scripts
echo ========================================

cd /d "%~dp0"

echo Building audit tool...
dotnet build -c Release --verbosity quiet

if %ERRORLEVEL% NEQ 0 (
    echo Failed to build audit tool!
    pause
    exit /b 1
)

echo Running memory leak audit...
dotnet run ImprovedMemoryLeakAudit.cs

echo.
echo Audit complete! Check the Logs folder for detailed results.
pause