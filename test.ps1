param([string]$SamplePath)
$ErrorActionPreference='Stop'
$taskCompiler=Join-Path $env:WINDIR 'Microsoft.NET\Framework64\v4.0.30319\csc.exe'
$taskApp=Join-Path $PSScriptRoot 'bin\Scenario-Converter.exe'
if(!(Test-Path -LiteralPath $taskApp)){throw 'Jalankan build.ps1 terlebih dahulu.'}
$taskTest=Join-Path $PSScriptRoot 'bin\ContentEditor.Tests.exe'
Push-Location (Join-Path $PSScriptRoot 'bin')
try {
    & $taskCompiler /nologo /target:exe /out:ContentEditor.Tests.exe /reference:Scenario-Converter.exe /reference:System.Windows.Forms.dll /reference:System.Web.Extensions.dll (Join-Path $PSScriptRoot 'tests\ContentEditorTests.cs')
    if($LASTEXITCODE){throw 'Build pengujian gagal.'}
} finally { Pop-Location }
if($SamplePath){& $taskTest $SamplePath}else{& $taskTest}
if($LASTEXITCODE){throw 'Pengujian gagal.'}
