param([string]$Version='1.0.1')
$ErrorActionPreference='Stop'
if($Version -notmatch '^\d+\.\d+\.\d+$'){throw 'Versi harus berbentuk 1.0.0.'}
$taskRoot=$PSScriptRoot
$taskBin=Join-Path $taskRoot 'bin'
New-Item -ItemType Directory -Force -Path $taskBin | Out-Null
$taskCompiler=Join-Path $env:WINDIR 'Microsoft.NET\Framework64\v4.0.30319\csc.exe'
if(!(Test-Path -LiteralPath $taskCompiler)){throw '.NET Framework 4.x tidak ditemukan.'}
$taskAssembly=Join-Path $taskBin 'Version.cs'
@"
using System.Reflection;
[assembly: AssemblyTitle("Scenario Converter")]
[assembly: AssemblyProduct("Scenario Converter")]
[assembly: AssemblyVersion("$Version.0")]
[assembly: AssemblyFileVersion("$Version.0")]
"@ | Set-Content -LiteralPath $taskAssembly -Encoding utf8
$taskHelper=Join-Path $taskBin 'ScenarioConverter.Updater.exe'
& $taskCompiler /nologo /target:winexe "/out:$taskHelper" /reference:System.Windows.Forms.dll (Join-Path $taskRoot 'src\UpdateHelper.cs')
if($LASTEXITCODE){throw 'Build updater gagal.'}
$taskExe=Join-Path $taskBin 'Scenario-Converter.exe'
& $taskCompiler /nologo /target:winexe "/out:$taskExe" "/win32icon:$(Join-Path $taskRoot 'assets\app.ico')" "/resource:$taskHelper,ScenarioConverter.Updater.exe" /reference:System.Windows.Forms.dll /reference:System.Drawing.dll /reference:System.Web.Extensions.dll (Join-Path $taskRoot 'src\ScenarioConverter.cs') (Join-Path $taskRoot 'src\Updates.cs') (Join-Path $taskRoot 'src\ContentDocument.cs') (Join-Path $taskRoot 'src\ContentEditorForm.cs') (Join-Path $taskRoot 'src\ScenarioImport.cs') $taskAssembly
if($LASTEXITCODE){throw 'Build aplikasi gagal.'}
((Get-FileHash -LiteralPath $taskExe -Algorithm SHA256).Hash.ToLowerInvariant()+'  Scenario-Converter.exe') | Set-Content -LiteralPath ($taskExe+'.sha256') -Encoding ascii
Write-Output "Build selesai: $taskExe (v$Version)"
