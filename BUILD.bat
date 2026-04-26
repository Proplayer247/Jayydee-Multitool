@echo off
title Jayydee Multitool - Source Compiler
echo #################################################
echo #                                               #
echo #     Jayydee Multitool - Source Compiler       #
echo #                                               #
echo #################################################
echo.
echo [SYSTEM] Locating C# Compiler...

set "csc=C:\Windows\Microsoft.NET\Framework64\v4.0.30319\csc.exe"

if not exist "%csc%" (
    echo [ERROR] C# Compiler not found! 
    echo Please make sure .NET Framework 4.8 is installed.
    pause
    exit
)

echo [SYSTEM] Compiling JayydeeMultitool.cs...
"%csc%" /win32manifest:app.manifest /r:System.IO.Compression.FileSystem.dll /r:System.Management.dll /out:JayydeeMultitool_Custom.exe JayydeeMultitool.cs

if %errorlevel% neq 0 (
    echo.
    echo [ERROR] Compilation failed! Check the source code for errors.
    pause
    exit
)

echo.
echo [SUCCESS] JayydeeMultitool_Custom.exe has been created!
echo [INFO] You can now safely delete this BUILD.bat.
echo.
pause
