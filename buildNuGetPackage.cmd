SET version="0.1.1"

msbuild /p:Configuration=Release FxSyncNet\FxSyncNet.csproj
MKDIR nuget\lib\net45
COPY FxSyncNet\bin\Release\FxSyncNet.dll nuget\lib\net45\

msbuild /p:Configuration=Release FxSyncNet.Universal\FxSyncNet.Universal.csproj
MKDIR nuget\lib\wpa
COPY FxSyncNet.Universal\bin\Release\FxSyncNet.dll nuget\lib\wpa\
COPY FxSyncNet.Universal\bin\Release\JWT.dll nuget\lib\wpa\

msbuild /p:Configuration=Release FxSyncNet.Universal\FxSyncNet.Universal.csproj
MKDIR nuget\lib\windows8
COPY FxSyncNet.Universal\bin\Release\FxSyncNet.dll nuget\lib\windows8\
COPY FxSyncNet.Universal\bin\Release\JWT.dll nuget\lib\windows8\

COPY FxSyncNet.nuspec nuget\

Tools\NuGet\nuget.exe pack nuget\FxSyncNet.nuspec -Version %version%

RMDIR nuget /S /Q