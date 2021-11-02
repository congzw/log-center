cd ../src/NbSites.LogCenter

@REM call dotnet nuget list source
call dotnet restore -r win-x64
call dotnet publish -c Release -r win-x64 --self-contained true --no-restore --output .\bin\publish /p:EnvironmentName=Development

pause