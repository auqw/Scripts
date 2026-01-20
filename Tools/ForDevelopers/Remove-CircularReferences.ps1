#!/usr/bin/env pwsh
<#
.SYNOPSIS
    Removes circular self-references from C# script files.

.DESCRIPTION
    Scans all .cs files in the Scripts directory for //cs_include directives that reference
    the file itself (circular self-references) and removes those lines.

.PARAMETER ScriptsPath
    Path to the Scripts directory. Defaults to current directory.

.PARAMETER WhatIf
    Shows what would be changed without making actual changes.

.EXAMPLE
    .\Remove-CircularReferences.ps1
    
.EXAMPLE
    .\Remove-CircularReferences.ps1 -WhatIf
#>

[CmdletBinding(SupportsShouldProcess)]
param(
    [Parameter()]
    [string]$ScriptsPath = (Get-Location).Path
)

function Get-ScriptFileName {
    param([string]$FilePath)
    
    return [System.IO.Path]::GetFileNameWithoutExtension($FilePath)
}

function Test-CircularReference {
    param(
        [string]$FilePath,
        [string]$IncludeLine
    )
    
    if ($IncludeLine -match '//cs_include\s+Scripts/(.+\.cs)') {
        $includedPath = $matches[1]
        $includedFileName = [System.IO.Path]::GetFileNameWithoutExtension($includedPath)
        $currentFileName = Get-ScriptFileName -FilePath $FilePath
        
        if ($includedFileName -eq $currentFileName) {
            return $true
        }
    }
    
    return $false
}

function Remove-CircularReferences {
    param([string]$Path)
    
    Write-Host "Scanning for circular references in: $Path" -ForegroundColor Cyan
    Write-Host ""
    
    $filesFound = 0
    $linesRemoved = 0
    
    $csFiles = Get-ChildItem -Path $Path -Filter "*.cs" -Recurse
    
    foreach ($file in $csFiles) {
        try {
            $content = Get-Content -LiteralPath $file.FullName -Raw -ErrorAction Stop
            $lines = Get-Content -LiteralPath $file.FullName -ErrorAction Stop
        } catch {
            Write-Warning "Skipping file (cannot read): $($file.FullName)"
            continue
        }
        
        $circularReferences = @()
        $lineNumber = 0
        
        foreach ($line in $lines) {
            $lineNumber++
            
            if ($line -match '^\s*//cs_include\s+') {
                if (Test-CircularReference -FilePath $file.FullName -IncludeLine $line) {
                    $circularReferences += @{
                        LineNumber = $lineNumber
                        Line = $line.Trim()
                    }
                }
            }
        }
        
        if ($circularReferences.Count -gt 0) {
            $filesFound++
            $relativePath = $file.FullName.Replace($Path, "").TrimStart('\')
            
            Write-Host "Found circular reference(s) in: $relativePath" -ForegroundColor Yellow
            
            foreach ($ref in $circularReferences) {
                Write-Host "  Line $($ref.LineNumber): $($ref.Line)" -ForegroundColor Red
            }
            
            if ($PSCmdlet.ShouldProcess($relativePath, "Remove $($circularReferences.Count) circular reference(s)")) {
                $newLines = @()
                $lineNumber = 0
                $linesToRemove = $circularReferences.LineNumber
                
                foreach ($line in $lines) {
                    $lineNumber++
                    if ($lineNumber -notin $linesToRemove) {
                        $newLines += $line
                    }
                }
                
                $newLines | Set-Content -LiteralPath $file.FullName -Encoding UTF8
                $linesRemoved += $circularReferences.Count
                
                Write-Host "  [SUCCESS] Removed $($circularReferences.Count) circular reference(s)" -ForegroundColor Green
            } else {
                Write-Host "  [WhatIf] Would remove $($circularReferences.Count) line(s)" -ForegroundColor Gray
            }
            
            Write-Host ""
        }
    }
    
    # Summary
    Write-Host "========================================" -ForegroundColor Cyan
    Write-Host "Summary:" -ForegroundColor Cyan
    Write-Host "  Files scanned: $($csFiles.Count)" -ForegroundColor White
    Write-Host "  Files with circular references: $filesFound" -ForegroundColor $(if ($filesFound -gt 0) { 'Yellow' } else { 'Green' })
    
    Write-Host "  Lines removed: $linesRemoved" -ForegroundColor $(if ($linesRemoved -gt 0) { 'Green' } else { 'White' })
    Write-Host "========================================" -ForegroundColor Cyan
}

# Main execution
if (-not (Test-Path $ScriptsPath)) {
    Write-Error "Scripts path not found: $ScriptsPath"
    exit 1
}

Remove-CircularReferences -Path $ScriptsPath
