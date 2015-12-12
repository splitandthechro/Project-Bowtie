@echo off
mkdir bin >nul
mkdir bin\Debug >nul
mkdir bin\Debug\assets >nul
robocopy "dist-static" "bin\Debug" /mir
robocopy "assets-static" "bin\Debug\assets" /mir
set msbuildpath="%programfiles(x86)%\MSBuild\14.0\Bin\"
cd src\ProjectBowtie
%msbuildpath%msbuild.exe /p:Configuration=Debug /p:outdir="..\..\bin"
echo Done.
pause >nul