@echo off

dotnet --info

dotnet restore

if not defined build_options call init.cmd

dotnet build **\*\project.json %build_options:Release=Debug%
dotnet build **\*\project.json %build_options%

dotnet pack src\NeinLinq %build_options%
dotnet pack src\NeinLinq.EF6 %build_options%
dotnet pack src\NeinLinq.EFCore %build_options%
