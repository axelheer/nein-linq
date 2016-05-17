@echo off

set test=%~dp0test
set results=%~dp0TestResults

set opencover=%UserProfile%\.nuget\packages\OpenCover\4.6.519\tools\OpenCover.Console.exe
set reportgenerator=%UserProfile%\.nuget\packages\ReportGenerator\2.4.5\tools\ReportGenerator.exe

if not exist %results% mkdir %results%

"%opencover%" -target:dotnet.exe -targetargs:"test \"%test%\NeinLinq.Tests\"" -output:"%results%\NeinLinq.xml" -register:user -filter:+[NeinLinq]*
"%opencover%" -target:dotnet.exe -targetargs:"test \"%test%\NeinLinq.Tests.EF6\"" -output:"%results%\NeinLinq.EF6.xml" -register:user -filter:+[NeinLinq.EF6]*
"%opencover%" -target:dotnet.exe -targetargs:"test \"%test%\NeinLinq.Tests.EFCore\"" -output:"%results%\NeinLinq.EFCore.xml" -register:user -filter:+[NeinLinq.EFCore]*

"%reportgenerator%" -reports:"%results%\NeinLinq.xml;%results%\NeinLinq.EF6.xml;%results%\NeinLinq.EFCore.xml" -targetdir:"%results%\report"
