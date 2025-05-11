<#
.SYNOPSIS
    Recursively deletes build artifacts (bin/, obj/ folders and compiled .dll files), then rebuilds a specified solution.

.DESCRIPTION
    This script searches from a specified root (default: script directory),
    removes all 'bin' and 'obj' directories, and any loose .dll files.
    It excludes hidden folders (like .git) to avoid accidental deletes.
    Finally, it runs a clean rebuild of ORBIT9000.sln in that root.

.PARAMETER RootPath
    The folder to start cleaning from. Defaults to the directory where this script resides.

.PARAMETER SolutionFile
    The name of the solution file to rebuild. Defaults to 'ORBIT9000.sln'.

.PARAMETER WhatIf
    Shows what would be deleted (and the rebuild command) without actually removing files or running the build.

.EXAMPLE
    # Dry-run to see what would be removed and the rebuild invocation
    .\Clean-BuildArtefacts.ps1 -WhatIf

.EXAMPLE
    # Clean & rebuild from a specific solution folder
    .\Clean-BuildArtefacts.ps1 -RootPath 'C:\Projects\MySolution'
#>
[CmdletBinding(SupportsShouldProcess=$true)]
param(
    [Parameter(Mandatory=$false)]
    [string]$RootPath = (Split-Path -Parent $MyInvocation.MyCommand.Definition),

    [Parameter(Mandatory=$false)]
    [string]$SolutionFile = 'ORBIT9000.sln'
)

Write-Host "Starting cleanup under: $RootPath" -ForegroundColor Cyan

# 1) Remove bin/ and obj/ directories
$dirs = Get-ChildItem -Path $RootPath -Recurse -Directory -Force |
    Where-Object {
        ($_.Name -in 'bin','obj') -and
        (-not ($_.FullName -match '\\\.(git|vs|idea|vscode)\\'))
    }

foreach ($d in $dirs) {
    if ($PSCmdlet.ShouldProcess($d.FullName, 'Remove directory')) {
        Remove-Item -LiteralPath $d.FullName -Recurse -Force -ErrorAction SilentlyContinue
        Write-Host "Deleted directory: $($d.FullName)" -ForegroundColor Green
    }
}

# 2) Remove loose .dll files (outside bin/obj)
$dlls = Get-ChildItem -Path $RootPath -Recurse -Include *.dll -File -Force |
    Where-Object {
        -not ($_.FullName -match '\\(bin|obj)\\')
    }

foreach ($f in $dlls) {
    if ($PSCmdlet.ShouldProcess($f.FullName, 'Remove file')) {
        Remove-Item -LiteralPath $f.FullName -Force -ErrorAction SilentlyContinue
        Write-Host "Deleted file: $($f.FullName)" -ForegroundColor Yellow
    }
}

Write-Host "Cleanup complete!" -ForegroundColor Cyan

# 3) Rebuild the solution
$solutionPath = Join-Path $RootPath $SolutionFile
if (Test-Path $solutionPath) {
    $buildCmd = "dotnet build `"$solutionPath`" -t:Rebuild"
    if ($PSCmdlet.ShouldProcess($solutionPath, "Rebuild solution")) {
        Write-Host "Rebuilding solution: $SolutionFile" -ForegroundColor Cyan
        Invoke-Expression $buildCmd
    }
} else {
    Write-Warning "Solution file not found at: $solutionPath"
}
