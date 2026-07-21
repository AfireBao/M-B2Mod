@echo off
setlocal
cd /d "F:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client"

if "%~1"=="" (
  echo Usage:
  echo   upload.bat new
  echo   upload.bat update
  exit /b 1
)

if /I "%~1"=="new" (
  TaleWorlds.MountAndBlade.SteamWorkshop.exe "G:\M&B2Mod\TORCompanionAssimilationSkip\workshop\UploadNew.xml"
  goto :eof
)

if /I "%~1"=="update" (
  TaleWorlds.MountAndBlade.SteamWorkshop.exe "G:\M&B2Mod\TORCompanionAssimilationSkip\workshop\UpdateExisting.xml"
  goto :eof
)

echo Unknown mode: %~1
exit /b 1
