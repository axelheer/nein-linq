@echo off

if [%appveyor_repo_branch%]==[release] (
  set build_options=--configuration Release
) else if defined appveyor_build_number (
  set build_options=--configuration Release --version-suffix ci%appveyor_build_number%
) else (
  set build_options=--configuration Release --version-suffix yolo
)

dotnet --info || goto :eof
dotnet restore || goto :eof
dotnet build %build_options% || goto :eof
dotnet build %build_options:Release=Debug% || goto :eof
dotnet pack --include-symbols %build_options% || goto :eof
