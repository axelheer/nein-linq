@echo off

set opencover=%UserProfile%\.nuget\packages\OpenCover\4.6.519\tools\OpenCover.Console.exe
set reportgenerator=%UserProfile%\.nuget\packages\ReportGenerator\2.4.5\tools\ReportGenerator.exe

if not exist TestResults mkdir TestResults

"%opencover%" -target:dotnet.exe -targetargs:"test test\NeinLinq.Tests --configuration Release -xml TestResults\NeinLinq.result.xml" -output:TestResults\NeinLinq.report.xml -register:user -filter:+[NeinLinq]*
"%opencover%" -target:dotnet.exe -targetargs:"test test\NeinLinq.Tests.EF6 --configuration Release -xml TestResults\NeinLinq.EF6.result.xml" -output:TestResults\NeinLinq.EF6.report.xml -register:user -filter:+[NeinLinq.EF6]*
"%opencover%" -target:dotnet.exe -targetargs:"test test\NeinLinq.Tests.EFCore --configuration Release -xml TestResults\NeinLinq.EFCore.result.xml" -output:TestResults\NeinLinq.EFCore.report.xml -register:user -filter:+[NeinLinq.EFCore]*

"%reportgenerator%" -reports:TestResults\NeinLinq.report.xml;TestResults\NeinLinq.EF6.report.xml;TestResults\NeinLinq.EFCore.report.xml -targetdir:TestResults\report -reporttypes:Badges;Html
