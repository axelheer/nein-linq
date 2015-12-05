@echo off

set artifacts=%~dp0artifacts
set coverage=%~dp0coverage
set test=%~dp0test

set dnx=%UserProfile%\.dnx\runtimes\dnx-clr-win-x64.1.0.0-rc1-update1\bin\dnx.exe
set opencover=%UserProfile%\.dnx\packages\OpenCover\4.6.166\tools\OpenCover.Console.exe
set reportgenerator=%UserProfile%\.dnx\packages\ReportGenerator\2.3.5\tools\ReportGenerator.exe

if not exist coverage mkdir coverage

"%opencover%" -target:"%dnx%" -targetargs:"--lib \"%artifacts%\bin\NeinLinq\Release\net45\" --project \"%test%\NeinLinq.Tests\" test" -output:"%coverage%\NeinLinq.xml" -register:user -filter:+[NeinLinq]*
"%opencover%" -target:"%dnx%" -targetargs:"--lib \"%artifacts%\bin\NeinLinq.EF6\Release\net45\" --project \"%test%\NeinLinq.Tests.EF6\" test" -output:"%coverage%\NeinLinq.EF6.xml" -register:user -filter:+[NeinLinq.EF6]*
"%opencover%" -target:"%dnx%" -targetargs:"--lib \"%artifacts%\bin\NeinLinq.EF7\Release\net451\" --project \"%test%\NeinLinq.Tests.EF7\" test" -output:"%coverage%\NeinLinq.EF7.xml" -register:user -filter:+[NeinLinq.EF7]*

"%reportgenerator%" -reports:"%coverage%\NeinLinq.xml;%coverage%\NeinLinq.EF6.xml;%coverage%\NeinLinq.EF7.xml" -targetdir:"%coverage%\report"
