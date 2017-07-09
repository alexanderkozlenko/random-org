[string]$workspace = "$PSScriptRoot\..\.."

ForEach ($directory in (Get-ChildItem -Path "$workspace\" -Directory -Include @("bin", "obj") -Recurse)) {
    If (Test-Path -Path $directory) {
        Write-Output "Removing `"$directory`"..."
        Remove-Item -Path $directory -Recurse -Force
    }
}