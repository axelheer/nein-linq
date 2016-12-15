@echo off

dotnet --info || goto :eof

dotnet restore || goto :eof

if not defined build_options call init.cmd || goto :eof

dotnet build **\project.json %build_options:Release=Debug% || goto :eof
dotnet build **\project.json %build_options% || goto :eof

dotnet pack src\NeinLinq %build_options% || goto :eof
dotnet pack src\NeinLinq.Queryable %build_options% || goto :eof
dotnet pack src\NeinLinq.Interactive %build_options% || goto :eof
dotnet pack src\NeinLinq.EntityFramework %build_options% || goto :eof
dotnet pack src\NeinLinq.EntityFrameworkCore %build_options% || goto :eof
