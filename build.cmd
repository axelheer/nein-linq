@echo off

dotnet --info || goto :eof

dotnet restore || goto :eof

if not defined build_options call init.cmd || goto :eof

dotnet build **\*\project.json %build_options:Release=Debug% || goto :eof
dotnet build **\*\project.json %build_options% || goto :eof

dotnet pack src\NeinLinq %build_options% || goto :eof
dotnet pack src\NeinLinq.EF6 %build_options% || goto :eof
dotnet pack src\NeinLinq.EFCore %build_options% || goto :eof
dotnet pack src\NeinLinq.IX %build_options% || goto :eof
