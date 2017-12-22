param (
    [parameter(mandatory)][String]$configuration
)

function execute {
    [outputtype([void])]
    param(
        [parameter(mandatory)][ScriptBlock]$command
    )

    & $command;
    
    if ($lastexitcode -eq $null) {
        throw "`"$command`" command failed";
    }
    if ($lastexitcode -ne 0) {
        throw "`"$command`" command failed with code $lastexitcode";
    }
}

[String]$sources = "$PSScriptRoot/src";
[String]$namespace = "Community.RandomOrg";

execute { dotnet clean "$sources/" -c $configuration /nologo };
execute { dotnet build "$sources/" -c $configuration };
execute { dotnet test "$sources/$namespace.Tests/$namespace.Tests.csproj" -c $configuration --no-restore --no-build };
execute { dotnet pack "$sources/$namespace/$namespace.csproj" -c $configuration --no-restore --no-build /nologo };