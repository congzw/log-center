cd ../src/

call dotnet restore
call dotnet publish --verbosity minimal -r win-x64 --self-contained true --configuration Release  /p:IsPublish=true /p:IsRunWebpack=false

pause