#!/usr/bin/env pwsh

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Memory Leak Audit Tool for Skua Scripts" -ForegroundColor Cyan  
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Change to script directory
Set-Location $PSScriptRoot

Write-Host "Building audit tool..." -ForegroundColor Yellow
$buildResult = dotnet build -c Release --verbosity quiet
if ($LASTEXITCODE -ne 0) {
    Write-Host "Failed to build audit tool!" -ForegroundColor Red
    Read-Host "Press Enter to exit"
    exit 1
}

Write-Host "Running memory leak audit..." -ForegroundColor Yellow
dotnet run

Write-Host ""
Write-Host "Audit complete! Check the Logs folder for detailed results." -ForegroundColor Green
Write-Host ""

# Open logs folder if requested
$response = Read-Host "Open logs folder? (y/n)"
if ($response -eq 'y' -or $response -eq 'Y') {
    if (Test-Path "Logs") {
        Start-Process "Logs"
    }
}