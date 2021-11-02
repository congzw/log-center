cd ../src/NbSites.LogCenter

@REM call dotnet nuget list source
call dotnet restore -r linux-x64
call dotnet publish -c Release -r linux-x64 --self-contained false --no-restore /p:EnvironmentName=Development

pause