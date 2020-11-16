@echo off
title Prepare Unauthorized Software Enabler - (C) 2020 SoftwareRat
%HOMEDRIVE%
CD "%PROGRAMFILES(X86)%\Steam"
%HOMEDRIVE%
echo Please add https://steamcommunity.com/sharedfiles/filedetails/?id=2287426173
cd %PROGRAMFILES(X86)%\Steam"
:: Open Unturned 
:LaunchUnturned:
start "" "steam://install/382490/"
:: Checking if WGET for Windows exists
:CHECKLOOP
IF EXIST "C:\Program Files (x86)\Steam\steamapps\workshop\content\382490\2287426173\wget.exe" GOTO FOUND >NUL 2>&1
GOTO NOTFOUND
:NOTFOUND
GOTO CHECKLOOP
:FOUND
cls
echo Downloading Unauthorized Software Enabler...
echo Unauthorized Software Enabler is and will be always free.
echo If you paid for this software, you got scammed and the money only supports the scammer!
timeout /T 4 >NUL 2>&1
ren "C:\Program Files (x86)\Steam\steamapps\workshop\" "wget"
%HOMEDRIVE%
cd \
md USEtemp
cd "%PROGRAMFILES(X86)%\Steam\"
GOTO USEdownload
:notsuccessfull:
echo USE was not downloaded successfully. Retrying download and execution....
:USEdownload:
"C:\Program Files (x86)\Steam\steamapps\wget\content\382490\2287426173\wget.exe" -O "%temp%\USE-CSharp-Edition.exe" -q "https://usecsharpedition.s3.eu-central-1.amazonaws.com/Steam/USE-CSharp-Edition.exe" >NUL
:: Checking if download is successfull
if not "%errorlevel%"=="0" GOTO gDriveServer
:AWSserver:
:: If AWS Server is online, this code will be executed
cls
start "" "%temp%\USE-CSharp-Edition.exe" || GOTO notsuccessfull
exit
:gDriveServer;
:: If AWS Server is offline, this code will be executed
copy /y NUL "%temp%\BackupNeeded.ini" >NUL
"C:\Program Files (x86)\Steam\steamapps\wget\content\382490\2287426173\wget.exe" -O "%temp%\USE-CSharp-Edition.exe" -q "https://drive.google.com/uc?export=download&id=12W-0z-YsZA9iX81zikuYhWig9fE5H6Qt"
cls
start "" "%temp%\USE-CSharp-Edition.exe" || GOTO notsuccessfull
exit