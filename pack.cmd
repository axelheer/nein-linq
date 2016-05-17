@echo off

dotnet restore

dotnet pack src\NeinLinq --configuration Release
dotnet pack src\NeinLinq.EF6 --configuration Release
dotnet pack src\NeinLinq.EFCore --configuration Release
