@echo off

set opencover=%UserProfile%\.nuget\packages\OpenCover\4.6.519\tools\OpenCover.Console.exe
set reportgenerator=%UserProfile%\.nuget\packages\ReportGenerator\2.5.1\tools\ReportGenerator.exe

if not exist TestResults mkdir TestResults || goto :eof

"%opencover%" -target:dotnet.exe -targetargs:"test test\NeinLinq.Tests --configuration Release --framework net461 -xml TestResults\NeinLinq.netframework.result.xml" -output:TestResults\NeinLinq.netframework.report.xml -register:user -filter:+[NeinLinq]* -returntargetcode || goto :eof
"%opencover%" -target:dotnet.exe -targetargs:"test test\NeinLinq.Tests --configuration Release --framework netcoreapp1.0 -xml TestResults\NeinLinq.netcoreapp.result.xml" -output:TestResults\NeinLinq.netcoreapp.report.xml -register:user -filter:+[NeinLinq]* -returntargetcode || goto :eof
"%opencover%" -target:dotnet.exe -targetargs:"test test\NeinLinq.Tests.EF --configuration Release --framework net461 -xml TestResults\NeinLinq.EF.netframework.result.xml" -output:TestResults\NeinLinq.EF.netframework.report.xml -register:user -filter:+[NeinLinq.EF]* -returntargetcode || goto :eof
"%opencover%" -target:dotnet.exe -targetargs:"test test\NeinLinq.Tests.EFCore --configuration Release --framework net461 -xml TestResults\NeinLinq.EFCore.netframework.result.xml" -output:TestResults\NeinLinq.EFCore.netframework.report.xml -register:user -filter:+[NeinLinq.EFCore]* -returntargetcode || goto :eof
"%opencover%" -target:dotnet.exe -targetargs:"test test\NeinLinq.Tests.EFCore --configuration Release --framework netcoreapp1.0 -xml TestResults\NeinLinq.EFCore.netcoreapp.result.xml" -output:TestResults\NeinLinq.EFCore.netcoreapp.report.xml -register:user -filter:+[NeinLinq.EFCore]* -returntargetcode || goto :eof
"%opencover%" -target:dotnet.exe -targetargs:"test test\NeinLinq.Tests.IX --configuration Release --framework net461 -xml TestResults\NeinLinq.IX.netframework.result.xml" -output:TestResults\NeinLinq.IX.netframework.report.xml -register:user -filter:+[NeinLinq.IX]* -returntargetcode || goto :eof
"%opencover%" -target:dotnet.exe -targetargs:"test test\NeinLinq.Tests.IX --configuration Release --framework netcoreapp1.0 -xml TestResults\NeinLinq.IX.netcoreapp.result.xml" -output:TestResults\NeinLinq.IX.netcoreapp.report.xml -register:user -filter:+[NeinLinq.IX]* -returntargetcode || goto :eof

"%reportgenerator%" -reports:TestResults\*.report.xml -targetdir:TestResults\report -reporttypes:Badges;Html || goto :eof
