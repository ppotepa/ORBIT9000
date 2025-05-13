<#
.SYNOPSIS
   Recursively deletes build artifacts (bin/, obj/ folders and loose .dll files), then forcefully rebuilds the ORBIT9000.sln solution.

.DESCRIPTION
   This script is meant to run from the '.scripts' folder directly below ORBIT9000.sln.
   It prevents cleaning outside that folder, accepts relative paths like './', and ensures all work stays scoped to ORBIT9000.

.PARAMETER RootPath
   Optional relative or absolute path to start cleaning from. Must be within the ORBIT9000 folder.

.PARAMETER SolutionFile
   Optional name of the solution file to rebuild. Defaults to 'ORBIT9000.sln'.

.PARAMETER WhatIf
   Simulates deletions and rebuild without executing them.

.PARAMETER NoBuild (-p)
   Skips rebuilding the solution after cleanup.

.EXAMPLE
   .\Clean-BuildArtefacts.ps1 -RootPath './'

.EXAMPLE
   .\Clean-BuildArtefacts.ps1 -p

.EXAMPLE
   .\Clean-BuildArtefacts.ps1 -WhatIf
#>

[CmdletBinding(SupportsShouldProcess=$true)]
param(
   [Parameter(Mandatory=$false)]
   [string]$RootPath = ".",

   [Parameter(Mandatory=$false)]
   [string]$SolutionFile = 'ORBIT9000.sln',

   [Parameter(Mandatory=$false)]
   [Alias("p")]
   [switch]$NoBuild
)

# Get the directory of the script and go one level up
$scriptLocation = Split-Path -Parent $MyInvocation.MyCommand.Definition
$solutionRoot = Split-Path $scriptLocation -Parent

# Make sure the .sln file exists at that location
$solutionPath = Join-Path $solutionRoot $SolutionFile
if (-not (Test-Path $solutionPath)) {
    throw "ERROR: Expected solution file '$SolutionFile' not found at $solutionRoot."
}

# Resolve and validate RootPath
try {
    $resolvedRootPath = Resolve-Path -Path $RootPath -ErrorAction Stop | Select-Object -ExpandProperty Path
} catch {
    throw "ERROR: Could not resolve RootPath '$RootPath'."
}

if (-not ($resolvedRootPath.StartsWith($solutionRoot))) {
    throw "ERROR: RootPath '$resolvedRootPath' must be inside the ORBIT9000 folder '$solutionRoot'."
}

Write-Host "Starting cleanup under: $resolvedRootPath" -ForegroundColor Cyan

# 1) Remove bin/ and obj/ directories
$dirs = Get-ChildItem -Path $resolvedRootPath -Recurse -Directory -Force |
   Where-Object {
       ($_.Name -in 'bin','obj') -and
       (-not ($_.FullName -match '\\\.(git|vs|idea|vscode)\\')) -and
       (-not ($_.FullName -match '\\Binaries\\'))
   }

foreach ($d in $dirs) {
   if ($PSCmdlet.ShouldProcess($d.FullName, 'Remove directory')) {
       try {
           Remove-Item -LiteralPath $d.FullName -Recurse -Force -ErrorAction Stop
           Write-Host "Deleted directory: $($d.FullName)" -ForegroundColor Green
       } catch {
           Write-Warning "Failed to delete directory: $($d.FullName) - $_"
       }
   }
}

# 2) Remove loose .dll files (outside bin/obj)
$dlls = Get-ChildItem -Path $resolvedRootPath -Recurse -Include *.dll -File -Force |
   Where-Object {
       -not ($_.FullName -match '\\(bin|obj)\\') -and
       -not ($_.FullName -match '\\Binaries\\')
   }

foreach ($f in $dlls) {
   if ($PSCmdlet.ShouldProcess($f.FullName, 'Remove file')) {
       try {
           Remove-Item -LiteralPath $f.FullName -Force -ErrorAction Stop
           Write-Host "Deleted file: $($f.FullName)" -ForegroundColor Yellow
       } catch {
           Write-Warning "Failed to delete file: $($f.FullName) - $_"
       }
   }
}

Write-Host "Cleanup complete!" -ForegroundColor Cyan

# 3) Force rebuild the solution unless -p is specified
if (-not $NoBuild) {
   if ($PSCmdlet.ShouldProcess($solutionPath, "Force rebuild solution")) {
       Write-Host "Force rebuilding solution: $SolutionFile" -ForegroundColor Cyan
       Invoke-Expression "dotnet clean `"$solutionPath`""
       Invoke-Expression "dotnet build `"$solutionPath`" -t:Rebuild --no-incremental"
   }
} else {
   Write-Host "Skipping build step (-p flag set)." -ForegroundColor DarkGray
}
