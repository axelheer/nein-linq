@echo off

set build_options=--configuration Release --version-suffix yolo
if defined appveyor_build_number set build_options=--configuration Release --version-suffix ci%appveyor_build_number%
if /i [%appveyor_repo_tag%]==[true] set build_options=--configuration Release
