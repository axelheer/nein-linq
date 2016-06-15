@echo off

dotnet restore

dotnet build src\*\project.json --configuration Debug
dotnet build src\*\project.json --configuration Release
dotnet build test\*\project.json --configuration Debug
dotnet build test\*\project.json --configuration Release

dotnet pack src\NeinLinq --configuration Release
dotnet pack src\NeinLinq.EF6 --configuration Release
dotnet pack src\NeinLinq.EFCore --configuration Release
