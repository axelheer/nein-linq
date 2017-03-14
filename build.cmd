@echo off

if [%appveyor_repo_branch%]==[release] (
  set build_options=/p:VersionSuffix=
) else if defined appveyor_build_number (
  set build_options=/p:VersionSuffix=ci%appveyor_build_number%
) else (
  set build_options=/p:VersionSuffix=yolo
)

dotnet --info || goto :eof

dotnet restore %build_options% || goto :eof

dotnet build --configuration Debug %build_options% || goto :eof
dotnet build --configuration Release %build_options% || goto :eof

dotnet pack --configuration Release --include-symbols %build_options% || goto :eof
