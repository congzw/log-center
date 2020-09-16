cd ../src/

call dotnet restore
call dotnet build --verbosity minimal /p:IsPublish=false /p:IsRunWebpack=false

pause