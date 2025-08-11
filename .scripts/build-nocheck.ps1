<#
.SYNOPSIS
    Builds the solution without performing up-to-date checks.
.DESCRIPTION
    This script builds the solution using MSBuild with the DisableFastUpToDateCheck option,
    which forces a rebuild regardless of file timestamps.
.PARAMETER Configuration
    Build configuration (Debug or Release). Defaults to Debug.
.PARAMETER SolutionPath
    Path to the solution file. If not specified, the script will look for a .sln file in the current directory.
.PARAMETER ExactSolutionPath
    Exact path to the solution file. If specified, this will be used directly without any search logic.
.PARAMETER AdditionalArgs
    Additional arguments to pass to dotnet build.
#>

param(
    [string]$Configuration = "Debug",
    [string]$SolutionPath = "",
    [string]$ExactSolutionPath = "",
    [string]$AdditionalArgs = ""
)

# Use ExactSolutionPath if specified, otherwise fall back to SolutionPath logic
if (-not [string]::IsNullOrEmpty($ExactSolutionPath)) {
    if (Test-Path $ExactSolutionPath) {
        $SolutionPath = $ExactSolutionPath
    }
    else {
        Write-Error "The specified solution file does not exist: $ExactSolutionPath"
        exit 1
    }
}
# Find solution file if not specified
elseif ([string]::IsNullOrEmpty($SolutionPath)) {
    $solutionFiles = Get-ChildItem -Path (Get-Location) -Filter "*.sln" -Recurse -Depth 2
    if ($solutionFiles.Count -eq 0) {
        Write-Error "No solution file found. Please specify the solution path using -SolutionPath or -ExactSolutionPath."
        exit 1
    }
    elseif ($solutionFiles.Count -gt 1) {
        Write-Warning "Multiple solution files found. Using the first one: $($solutionFiles[0].FullName)"
        $SolutionPath = $solutionFiles[0].FullName
    }
    else {
        $SolutionPath = $solutionFiles[0].FullName
    }
}

# Build command with DisableFastUpToDateCheck property
$buildCommand = "dotnet build `"$SolutionPath`" -c $Configuration /p:DisableFastUpToDateCheck=true"

# Add any additional arguments
if (-not [string]::IsNullOrEmpty($AdditionalArgs)) {
    $buildCommand += " $AdditionalArgs"
}

Write-Host "Building solution: $SolutionPath" -ForegroundColor Cyan
Write-Host "Command: $buildCommand" -ForegroundColor DarkGray

# Execute the build command
Invoke-Expression $buildCommand

# Return the exit code from the build process
exit $LASTEXITCODE