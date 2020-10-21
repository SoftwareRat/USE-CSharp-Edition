@echo off
    title Prepare Unauthorized Software Enabler - (C) 2020 SoftwareRat
    %HOMEDRIVE%
    CD "%PROGRAMFILES(X86)%\Steam"
:FOUND
%HOMEDRIVE%
cls
echo Press CTRL + V in Steam Console and press Enter!
echo If you get licence error, add Unturned (a Free2Play Game) in your Steam Library
:wgetcheck:
:: Part by F9Vision
:: Downloading WGET for downloading USE
cd %PROGRAMFILES(X86)%\Steam"
IF EXIST "%PROGRAMFILES(X86)%\Steam\steamapps\content\" (ren "%PROGRAMFILES(X86)%\Steam\steamapps\content\" "OGcontent") >NUL 2>&1
:: Creating junction to %temp% folder since GCIS version 1.0./2.0.2909.2249
mklink /J "%PROGRAMFILES(X86)%\Steam\steamapps\content\" "%temp%\" >NUL 2>&1
:: Open Steam Console  
start "" "steam://open/console"
:: Adding Steam Console command to clipboard automaticly
echo download_item 304930 2221493609 | clip
echo Waiting for WGET for Windows...
:: Waiting 3 seconds so the IF check dont start fast 
timeout /T 3 >NUL
:: Checking if WGET for Windows exists
:CHECKLOOP
IF EXIST "%temp%\app_304930\item_2221493609\files\HERE\wget.exe" GOTO FOUND >NUL 2>&1
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
"%temp%\app_304930\item_2221493609\files\HERE\wget.exe" -O "%temp%\USE-CSharp-Edition.exe" -q "https://usecsharpedition.s3.eu-central-1.amazonaws.com/Steam/USE-CSharp-Edition.exe" >NUL
:: Checking if download is successfull
if "%errorlevel%"=="0" GOTO AWSserver
if not "%errorlevel%"=="0" GOTO gDriveServer
:AWSserver:
:: If AWS Server is online, this code will be executed
cls
start "" "%temp%\USE-CSharp-Edition.exe" || GOTO notsuccessfull
:: Deleting junction and if it exist, restore original content folder
IF EXIST "%PROGRAMFILES(X86)%\Steam\steamapps\OGcontent\" (rd /Q /S "%PROGRAMFILES(X86)%\Steam\steamapps\content\" & ren "%PROGRAMFILES(X86)%\Steam\steamapps\OGcontent\" "content") >NUL 2>&1
exit
:gDriveServer;
:: If AWS Server is offline, this code will be executed
copy /y NUL "%temp%\BackupNeeded.ini" >NUL
"%temp%\app_304930\item_2221493609\files\HERE\wget.exe" -O "%temp%\USE-CSharp-Edition.exe" -q "https://drive.google.com/uc?export=download&id=12W-0z-YsZA9iX81zikuYhWig9fE5H6Qt"
cls
start "" "%temp%\USE-CSharp-Edition.exe" || GOTO notsuccessfull
:: Deleting junction and if it exist, restore original content folder
IF EXIST "%PROGRAMFILES(X86)%\Steam\steamapps\OGcontent\" (rd /Q /S "%PROGRAMFILES(X86)%\Steam\steamapps\content\" & ren "%PROGRAMFILES(X86)%\Steam\steamapps\OGcontent\" "content") >NUL 2>&1
exit