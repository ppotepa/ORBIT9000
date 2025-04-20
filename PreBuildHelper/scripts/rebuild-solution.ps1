param (
    [string]$SolutionPath = "../../ORBIT9000.sln",
    [string]$Configuration = "Debug"
)

function Get-MSBuildPath {
    $vswherePath = "${env:ProgramFiles(x86)}\Microsoft Visual Studio\Installer\vswhere.exe"

    if (-Not (Test-Path $vswherePath)) {
        Write-Error "vswhere.exe not found at $vswherePath"
        exit 1
    }

    $msbuildPath = & $vswherePath -latest -requires Microsoft.Component.MSBuild -find MSBuild\**\Bin\MSBuild.exe | Select-Object -First 1

    if (-Not $msbuildPath) {
        Write-Error "MSBuild.exe not found"
        exit 1
    }

    return $msbuildPath
}

if (-Not (Test-Path $SolutionPath)) {
    Write-Error "Solution file not found: $SolutionPath"
    exit 1
}

$msbuildExe = Get-MSBuildPath

Write-Host "Using MSBuild: $msbuildExe"

& "$msbuildExe" "$SolutionPath" /t:Rebuild /p:Configuration=$Configuration